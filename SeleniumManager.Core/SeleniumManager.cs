using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using SeleniumManager.Core.DataContract;
using SeleniumManager.Core.Interface;
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
        private readonly ConcurrentQueue<Action<IWebDriver>> _queue;
        private readonly ConfigurationSettings _configSettings;
        private readonly HttpClient httpClient;

        public int MaxSessions { get; private set; } = 0;
        public int FreeSessions { get; private set; } = 0;
        public int ConcurrentSessions { get; private set; } = 0;
        public int AvailableSessions { get; private set; } = 0;
        public int TotalSessions { get; private set; } = 0;
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
                // TODO: make it like get the driver first and then process the action 
                try
                {
                    // for now only using chrome for testing
                    IWebDriver _driver = CreateDriverInstance("Chrome");
                    action(_driver);

                    // Release driver
                    _driver.Dispose();

                    // Release the semaphore to allow other threads to acquire it
                    _semaphore.Release();

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

        public virtual IWebDriver CreateDriverInstance(string browserName)
        {
            IWebDriver driver;

            switch (GetAvailableDriverName(browserName).ToLower())
            {
                case "firefox":
                    driver = new RemoteWebDriver(new Uri(_configSettings.GridHost.ToString()), _configSettings.Options.firefoxOptions);

                    break;
                case "chrome":
                    driver = new RemoteWebDriver(new Uri(_configSettings.GridHost.ToString()), _configSettings.Options.chromeOptions);
                    break;
                case "edge":
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

        public string GetAvailableDriverName(string browserName)
        {
            var data = GetHeartBeat();
            if(data != null)
            {
                // TODO:check for availablity 

                // return if available
                return browserName;
            }

            throw new Exception("Cannot Get Heart Beat");
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

            foreach (var node in nodeStatus.value.nodes)
            {
                MaxSessions += (int)node.maxSessions;
                foreach (var slot in node.slots)
                {
                    TotalSessions++;
                    if (slot.session == null)
                        FreeSessions++;
                }
            }
            ConcurrentSessions = TotalSessions - FreeSessions;
            AvailableSessions = MaxSessions - ConcurrentSessions;
        }

        private void ResetValues()
        {
            AvailableSessions = FreeSessions = TotalSessions = ConcurrentSessions = 0;
        }

        #endregion

    }
}
