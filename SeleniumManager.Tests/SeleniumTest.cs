using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SeleniumManager.Core;
using System;
using System.Text.Json.Nodes;

namespace SeleniumManager.Tests
{
    [TestClass]
    public class SeleniumTest
    {
        private Core.SeleniumManager _seleniumManager;
        private ConfigManager? _configManager;

        [TestInitialize()]
        public void init()
        {
            _configManager = new ConfigManager();
            _seleniumManager = new Core.SeleniumManager(_configManager);
        }

        [TestMethod]
        public void HeartbeatTest()
        {
            dynamic nodeStatus = _seleniumManager.GetHeartBeat().Result;
            var slots = (JArray)nodeStatus?.value.nodes;
            Assert.IsTrue(slots?.Count() > 0);
            Console.WriteLine("Slots : " + slots?.Count());
            Console.WriteLine("Available Sessions: " + _seleniumManager.AvailableSessions.ToString());
            Console.WriteLine("Concurrent Sessions: " + _seleniumManager.ConcurrentSessions.ToString());
            Console.WriteLine("Total Sessions: " + _seleniumManager.TotalSessions.ToString());
            Console.WriteLine("Max Sessions: " + _seleniumManager.MaxSessions.ToString());
        }

        [TestMethod]
        public void GetAvailableSessions ()
        {
            _seleniumManager.GetAvailableInstances().Wait();
            Assert.IsTrue(_seleniumManager.AvailableSessions > 0);
            Console.WriteLine("Available Sessions: " + _seleniumManager.AvailableSessions.ToString());
        }
    }
}