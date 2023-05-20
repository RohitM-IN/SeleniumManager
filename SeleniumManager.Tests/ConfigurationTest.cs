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
            Assert.AreEqual(config.configSettings.GridHost, "localhost:4444");
        }
    }
}