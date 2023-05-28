using SeleniumManager.Core;
using System.Reflection;

namespace SeleniumManager.Tests
{
    [TestClass]
    public class ConfigurationTest
    {
        [TestMethod]
        public void ConfigTest()
        {
            var config = new ConfigManager();
            Assert.AreEqual(config.configSettings.GridHost, "http://127.0.0.1:4444");
            Console.WriteLine("Host: "+config.configSettings.GridHost + " is in "+ (config.configSettings.GridHost.ToString().Contains("http://127.0.0.1:4444") ? "Default ": "Custom ") + "Configuration");
        }
    }
}