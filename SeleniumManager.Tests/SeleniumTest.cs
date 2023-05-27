using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeleniumManager.Core;
using System;

namespace SeleniumManager.Tests
{
    [TestClass]
    public class SeleniumTest
    {
        private Core.SeleniumManager? _seleniumManager;
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
            var availableInstance = _seleniumManager?.GetAvailableInstances().Result;
            Assert.IsTrue(availableInstance > 0);
            Console.WriteLine("Available Sessions: " + availableInstance.ToString());
        }
    }
}