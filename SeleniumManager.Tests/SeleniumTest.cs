using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeleniumManager.Core;
using System;

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
            _seleniumManager.GetAvailableInstances().Wait();
            Assert.IsTrue(_seleniumManager.AvailableSessions > 0);
            Console.WriteLine("Available Sessions: " + _seleniumManager.AvailableSessions.ToString());
        }
    }
}