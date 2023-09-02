using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using SeleniumManager.Core.DataContract;
using SeleniumManager.Core.Enum;
using SeleniumManager.Core.Exception;
using SeleniumManager.Core.Interface;
using SeleniumManager.Core.Utils;
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System;

namespace SeleniumManager.Core
{
    public class SeleniumGridManager : ISeleniumManager
    {
        #region Declerations

        #region Private Properties
        private readonly SemaphoreSlim _semaphore;
        private readonly SemaphoreSlim _availableStereotypesSemaphore = new SemaphoreSlim(1, 1);
        private readonly ConcurrentQueue<ActionWithBrowser> _queue;
        private readonly ConfigurationSettings _configSettings;
        private readonly HttpClient httpClient;
        #endregion

        #region Public Properties
        public int MaxSessions { get; private set; } = 0;
        public int FreeSessions { get; private set; } = 0;
        public int ConcurrentSessions { get; private set; } = 0;
        public int AvailableSessions { get; private set; } = 0;
        public int TotalSessions { get; private set; } = 0;
        public Dictionary<string, long> MaxStereotypes { get; private set; } = new();
        public Dictionary<string, long> ConcurrentStereotypes { get; private set; } = new();
        public Dictionary<string, long> AvailableStereotypes { get; private set; } = new();
        public DateTime LastSessionDetails { get; private set; }
        public delegate void ActionWithBrowser(IWebDriver driver, string? browserName);
        #endregion

        #endregion

        #region Constructor
        public SeleniumGridManager(ConfigManager configManager)
        {
            _configSettings = configManager.configSettings;
            httpClient = new HttpClient();
            _semaphore = new SemaphoreSlim(GetAvailableInstances().Result, 1000);
            _queue = new ConcurrentQueue<ActionWithBrowser>();
        }

        #endregion

        #region Public Methods

        public virtual Task<TResult> EnqueueAction<TResult>(Func<IWebDriver, TResult> action)
        {
            var tcs = new TaskCompletionSource<TResult>();

            _queue.Enqueue((driver, n) =>
            {
                try
                {
                    // Execute the action and get the result
                    var result = action(driver);

                    // Set the result as the task completion result
                    tcs.SetResult(result);
                }
                catch (System.Exception ex)
                {
                    // Set the exception as the task completion exception
                    tcs.SetException(ex);
                    throw new ActionExecutionException("Error Occoured inside Action", ex);
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

        public virtual Task<TResult> EnqueueAction<TResult>(Func<IWebDriver, TResult> action, string browserName)
        {
            var tcs = new TaskCompletionSource<TResult>();

            _queue.Enqueue((driver, bn) =>
            {
                try
                {
                    bn = browserName;

                    // Execute the action and get the result
                    var result = action(driver);

                    // Set the result as the task completion result
                    tcs.SetResult(result);
                }
                catch (System.Exception ex)
                {
                    // Set the exception as the task completion exception
                    tcs.SetException(ex);
                    throw new ActionExecutionException("Error Occoured inside Action", ex);
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
                string? browserName = null;
                if (action.Target != null)
                {
                    FieldInfo browserNameField = action.Target.GetType().GetField("browserName");
                    browserName = (string?)browserNameField?.GetValue(action.Target);
                }
                IWebDriver _driver = CreateDriverInstance(browserName);
                try
                {
                    ICapabilities capabilities = ((RemoteWebDriver)_driver).Capabilities;
                    browserName = capabilities.GetCapability("browserName").ToString();
                    action(_driver, browserName);

                    // Release driver
                    _driver.Dispose();

                    // Recursively call TryExecuteNext to process the next action in the queue
                    TryExecuteNext();
                }
                catch (System.Exception ex)
                {
                    // Release the semaphore even if an exception occurs
                    _semaphore.Release();
                    _driver?.Dispose();

                    // Recursively call TryExecuteNext to process the next action in the queue
                    TryExecuteNext();

                    throw new ActionExecutionException("There was error while performing the delegate action", ex);
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
            try
            {
                var nodeStatus = await GetStatus();

                if (nodeStatus == null) return 0;

                LastSessionDetails = DateTime.Now;

                getSessions(nodeStatus);

                return AvailableSessions;

            }
            catch (System.Exception ex)
            {

                throw new HttpRequestException("Cannot Get Heart Beat", ex);
            }
        }

        public virtual async Task<dynamic?> GetHeartBeat()
        {
            try
            {
                var nodeStatus = await GetStatus();

                if (nodeStatus == null) return null;

                return nodeStatus;

            }
            catch (System.Exception ex)
            {
                throw new HttpRequestException("Cannot Get Heart Beat", ex);
            }
        }


        public virtual IWebDriver CreateDriverInstance(string? browserName = null)
        {
            IWebDriver driver;
            browserName = GetAvailableDriverName(browserName);
            switch (browserName.ToLower())
            {
                case "firefox":
                case "geko":
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

                // Not Tested
                case "ie":
                case "internetexplorer":
                case "internet explorer":
                    driver = new RemoteWebDriver(new Uri(_configSettings.GridHost.ToString()), _configSettings.Options.internetExplorerOptions);
                    break;

                // Not Tested
                case "opera":
                    driver = new RemoteWebDriver(new Uri(_configSettings.GridHost.ToString()), _configSettings.Options.operaOptions);
                    break;

                default:
                    throw new UnsupportedBrowserException(browserName);
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
                if (!string.IsNullOrEmpty(_configSettings.UserName) && !string.IsNullOrEmpty(_configSettings.Password))
                {
                    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_configSettings.UserName}:{_configSettings.Password}"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
                }

                var response = await httpClient.GetAsync(_configSettings.GridHost + _configSettings.Endpoints.Status);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var nodeStatus = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(content);

                return nodeStatus;

            }
            catch (HttpRequestException ex)
            {
                if (ex.Message.Contains("401"))
                {
                    throw new GridAuthenticationException("Grid authentication failed. Please check your credentials.", ex);
                }

                throw;

            }
            catch (System.Exception ex)
            {
                throw new System.Exception("There was an error while getting status", ex);
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
        }

        private void ResetValues()
        {
            AvailableSessions = FreeSessions = TotalSessions = ConcurrentSessions = 0;
        }

        private bool IsBrowserAvailable(string browserName)
        {
            AvailableStereotypes.TryGetValue(browserName, out var _maxSessions);
            if (_maxSessions == 0)
                return false;
            return true;
        }

        private async Task<string> FindBestAvailableBrowser()
        {
            try
            {
                await _availableStereotypesSemaphore.WaitAsync();
                var statistics = RatioDictionary.GetRatioDictionary(_configSettings.statistics, MaxSessions);

                foreach (var kvp in statistics.OrderByDescending(x => x.Value))
                {
                    var browserName = kvp.Key;
                    var maxInstances = kvp.Value;
                    ConcurrentStereotypes.TryGetValue(kvp.Key, out var concurrentInstances);
                    AvailableStereotypes.TryGetValue(kvp.Key, out var availableInstances);

                    if (maxInstances > concurrentInstances && !string.IsNullOrEmpty(browserName))
                    {

                        AdjustInstance(browserName, AdjustType.Create);
                        return browserName.ToString();
                    }

                    continue;
                }

                // If no available browser is found, return the browser with the highest instances count
                return statistics.OrderByDescending(x => x.Value).FirstOrDefault().Key ?? WebDriverType.Chrome.GetDescription();

            }
            catch (System.Exception)
            {

                throw;
            }
            finally
            {
                _availableStereotypesSemaphore.Release();
            }
        }

        private void AdjustInstance(string key, AdjustType type)
        {
            switch (type)
            {
                case AdjustType.Create:
                    if (ConcurrentStereotypes.ContainsKey(key))
                        ConcurrentStereotypes[key]++;

                    if (AvailableStereotypes.ContainsKey(key))
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
