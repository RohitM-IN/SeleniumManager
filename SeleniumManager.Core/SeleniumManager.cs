using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using SeleniumManager.Core.DataContract;
using SeleniumManager.Core.Interface;
using SeleniumManager.Core.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumManager.Core
{
    public class SeleniumManager: ISeleniumManager
    {
        #region Declerations

        private readonly SemaphoreSlim _semaphore;
        private readonly SemaphoreSlim _availableStereotypesSemaphore = new SemaphoreSlim(1, 1);
        private readonly ConcurrentQueue<Action<IWebDriver>> _queue;
        private readonly ConfigurationSettings _configSettings;
        private readonly HttpClient httpClient;

        public int MaxSessions { get; private set; } = 0;
        public int FreeSessions { get; private set; } = 0;
        public int ConcurrentSessions { get; private set; } = 0;
        public int AvailableSessions { get; private set; } = 0;
        public int TotalSessions { get; private set; } = 0;
        public Dictionary<string, long> MaxStereotypes { get; private set; } = new();
        public Dictionary<string, long> ConcurrentStereotypes { get; private set; } = new();
        public Dictionary<string, long> AvailableStereotypes { get; private set; } = new();
        public DateTime LastSessionDetails { get; private set; }
        enum AdjustType
        {
            Create = 1,
            Destroy = 2
        }
        #endregion

        #region Constructor
        public SeleniumManager(ConfigManager configManager)
        {
            _configSettings = configManager.configSettings;
            httpClient = new HttpClient();   
            _semaphore = new SemaphoreSlim(GetAvailableInstances().Result,1000);
            _queue = new ConcurrentQueue<Action<IWebDriver>>();
        }

        #endregion

        #region Public Methods

        public virtual Task<string> EnqueueAction(Func<IWebDriver, string> action)
        {
            var tcs = new TaskCompletionSource<string>();

            _queue.Enqueue(driver =>
            {
                try
                {
                    // Execute the action and get the result
                    var result = action(driver);

                    // Set the result as the task completion result
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    // Set the exception as the task completion exception
                    tcs.SetException(ex);
                    throw new Exception("Error Occoured inside Action", ex);
                }
                finally 
                { 
                    // Dispose the driver if not already done
                    driver?.Dispose();
                }
            });

            TryExecuteNext();
            return tcs.Task;
        }
        public async void TryExecuteNext()
        {
            await _semaphore.WaitAsync(); // Acquire the semaphore

            if (_queue.TryDequeue(out var action))
            {
                string browserName = "";
                // TODO: make it like get the driver first and then process the action 
                try
                {
                    // for now only using chrome for testing
                    IWebDriver _driver = CreateDriverInstance();
                    ICapabilities capabilities = ((RemoteWebDriver)_driver).Capabilities;
                    browserName = capabilities.GetCapability("browserName").ToString();
                    action(_driver);
                    
                    // Release driver
                    _driver.Dispose();

                    // Recursively call TryExecuteNext to process the next action in the queue
                    TryExecuteNext();
                }
                catch (Exception ex)
                {
                    // Release the semaphore even if an exception occurs
                    _semaphore.Release();

                    // Recursively call TryExecuteNext to process the next action in the queue
                    TryExecuteNext();

                    throw new Exception("There was error while performing the delegate action", ex);
                }
                finally 
                { 
                    _semaphore.Release();
                    await _availableStereotypesSemaphore.WaitAsync();
                    if (!string.IsNullOrEmpty(browserName))
                        AdjustInstance(browserName.ToLower(), AdjustType.Destroy);
                    _availableStereotypesSemaphore.Release();

                }
            }
        }
        public virtual async Task<int> GetAvailableInstances()
        {
            var nodeStatus = await GetStatus();

            if (nodeStatus == null) return 0;

            getSessions(nodeStatus);

            return AvailableSessions;
        }

        public virtual async Task<dynamic?> GetHeartBeat()
        {
            var nodeStatus = await GetStatus();

            if (nodeStatus == null) return null;

            return nodeStatus;
        }

        public virtual IWebDriver CreateDriverInstance(string? browserName = null)
        {
            IWebDriver driver;
            browserName = GetAvailableDriverName(browserName);
            switch (browserName.ToLower())
            {
                case "firefox":
                    driver = new RemoteWebDriver(new Uri(_configSettings.GridHost.ToString()), _configSettings.Options.firefoxOptions);

                    break;
                case "chrome":
                    driver = new RemoteWebDriver(new Uri(_configSettings.GridHost.ToString()), _configSettings.Options.chromeOptions);
                    break;
                case "microsoftedge":
                    driver = new RemoteWebDriver(new Uri(_configSettings.GridHost.ToString()), _configSettings.Options.edgeOptions);
                    break;
                case "safari":
                    driver = new RemoteWebDriver(new Uri(_configSettings.GridHost.ToString()), _configSettings.Options.safariOptions);
                    break;
                case "ie":
                    driver = new RemoteWebDriver(new Uri(_configSettings.GridHost.ToString()), _configSettings.Options.internetExplorerOptions);
                    break;
                default:
                    throw new ArgumentException("Browser not supported yet!");
            }
            return driver;
        }

        public string GetAvailableDriverName(string? browserName)
        {
            // check if last session was gotten in last 1 min get from config default 1 min
            if ((DateTime.Now - LastSessionDetails) > TimeSpan.FromSeconds(60))
            {
                var data = GetHeartBeat().Result;
                if (data != null)
                    getSessions(data);
                else
                    throw new Exception("Cannot Get Heart Beat");
            }

            // Check if the requested browser is available
            if (!string.IsNullOrEmpty(browserName) && IsBrowserAvailable(browserName))
                return browserName;

            // If the requested browser is not available, find the best available browser based on statistics
            string bestBrowser = FindBestAvailableBrowser().Result;

            // Return the best available browser
            return bestBrowser;         
        }
            
        #endregion

        #region Private Methods
        private async Task<dynamic?> GetStatus()
        {
            try
            {
                var response = await httpClient.GetAsync(_configSettings.GridHost + _configSettings.Endpoints.Status);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var nodeStatus = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(content);

                return nodeStatus;

            }
            catch (Exception ex)
            {
                throw new Exception("There was en error while getting status", ex);
            }
        }

        private void getSessions(dynamic nodeStatus)
        {
            if (nodeStatus == null)
            {
                ResetValues();
                return;
            }
            MaxSessions = 0;
            ResetValues();
            MaxStereotypes = new Dictionary<string, long>();
            ConcurrentStereotypes = new Dictionary<string, long>();
            AvailableStereotypes = new Dictionary<string, long>();

            foreach (var node in nodeStatus.value.nodes)
            {
                MaxSessions += (int)node.maxSessions;
                foreach (var slot in node.slots)
                {
                    TotalSessions++;
                    MaxStereotypes.TryAdd((string)slot.stereotype.browserName, 0);
                    ConcurrentStereotypes.TryAdd((string)slot.stereotype.browserName, 0);

                    if (slot.session == null)
                        FreeSessions++;
                    else
                        ConcurrentStereotypes[(string)slot.stereotype.browserName]++;
                    MaxStereotypes[(string)slot.stereotype.browserName]++;
                }
            }
            _availableStereotypesSemaphore.Wait();
            AvailableStereotypes = MaxStereotypes.ToDictionary(kv => kv.Key, kv => kv.Value - (ConcurrentStereotypes.TryGetValue(kv.Key, out var usedVal) ? usedVal : 0));
            _availableStereotypesSemaphore.Release();
            ConcurrentSessions = TotalSessions - FreeSessions;
            AvailableSessions = MaxSessions - ConcurrentSessions;
            LastSessionDetails = DateTime.Now;
        }

        private void ResetValues()
        {
            AvailableSessions = FreeSessions = TotalSessions = ConcurrentSessions = 0;
        }

        private bool IsBrowserAvailable(string browserName)
        {
            AvailableStereotypes.TryGetValue(browserName, out var _maxSessions);
            if(_maxSessions == 0 )
                return false;
            return true;
        }

        private async Task<string> FindBestAvailableBrowser()
        {
            var statistics = RatioDictionary.GetRatioDictionary(_configSettings.statistics, MaxSessions);

            foreach (var kvp in statistics.OrderByDescending(x => x.Value))
            {
                await _availableStereotypesSemaphore.WaitAsync();
                var browserName = kvp.Key;
                var maxInstances = kvp.Value;
                ConcurrentStereotypes.TryGetValue(kvp.Key.ToLower(), out var concurrentInstances);
                AvailableStereotypes.TryGetValue(kvp.Key.ToLower(), out var availableInstances);

                if (maxInstances >= concurrentInstances  && !string.IsNullOrEmpty(browserName))
                {

                    AdjustInstance(browserName.ToLower(), AdjustType.Create);

                    _availableStereotypesSemaphore.Release();
                    return browserName.ToString();
                }
                _availableStereotypesSemaphore.Release();
                continue;

            }

            // If no available browser is found, return the browser with the highest instances count
            return statistics.OrderByDescending(x => x.Value).FirstOrDefault().Key ?? "Chrome";
        }

        private void AdjustInstance(string key,AdjustType type)
        {
            switch (type)
            {
                case AdjustType.Create:
                    if(ConcurrentStereotypes.ContainsKey(key))
                        ConcurrentStereotypes[key]++;

                    if(AvailableStereotypes.ContainsKey(key))
                        AvailableStereotypes[key]--;

                    break;
                case AdjustType.Destroy:
                    if (ConcurrentStereotypes.ContainsKey(key))
                        ConcurrentStereotypes[key]--;

                    if (AvailableStereotypes.ContainsKey(key))
                        AvailableStereotypes[key]++;

                    break;
                default:
                    break;
            }
        }
        #endregion

    }
}
