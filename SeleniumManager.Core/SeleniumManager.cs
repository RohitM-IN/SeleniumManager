using OpenQA.Selenium;
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

        public int? MaxSessions { get; private set; } = 0;
        public int? AvailableSessions { get; private set; } = 0;
        #endregion

        #region Constructor
        public SeleniumManager(ConfigManager configManager)
        {
            _configSettings = configManager.configSettings;
            _semaphore = new SemaphoreSlim(_configSettings.MaxConcurrency);
            _queue = new ConcurrentQueue<Action<IWebDriver>>();
            this.httpClient = new HttpClient();
        }

        #endregion

        #region Public Methods

        public virtual void EnqueueAction(Action<IWebDriver> action)
        {
            _queue.Enqueue(action);
        }

        public async Task<int?> GetAvailableInstances()
        {
            var nodeStatus = await GetStatus();

            if (nodeStatus == null) return null;

            getSessions(nodeStatus);

            return AvailableSessions;
        }

        #endregion

        #region Private Methods

        private async Task<dynamic?> GetStatus()
        {
            var response = await httpClient.GetAsync(_configSettings.GridHost + _configSettings.Endpoints.Status);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var nodeStatus = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(content);

            return nodeStatus;
        }

        private void getSessions(dynamic nodeStatus)
        {
            if (nodeStatus == null) 
            {
                AvailableSessions = MaxSessions =  null;
                return;
            }

            MaxSessions = 0;
            AvailableSessions = 0;
            foreach (var node in nodeStatus.value.nodes)
            {
                MaxSessions += (int)node.maxSessions;
                foreach (var slot in node.slots)
                {
                    if (slot.session == null)
                        AvailableSessions++;
                }
            }
        }

        #endregion

    }
}
