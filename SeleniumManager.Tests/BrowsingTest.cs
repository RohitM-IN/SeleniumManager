using OpenQA.Selenium;
using SeleniumManager.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumManager.Tests
{
    [TestClass]
    public class BrowsingTest
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
        public void TestBrouse()
        {
            _seleniumManager.EnqueueAction(BrouseWebsite);

            // Start processing the actions
            _seleniumManager.TryExecuteNext();
           
        }

        [TestMethod]
        public async Task ParallelTestBrouse()
        {
            List<Task> tasks = new List<Task>();

            for (int number = 1; number < 40; number++)
            {
                Task task = Task.Run(async () =>
                {
                    await _seleniumManager.EnqueueAction(BrouseGoogleWebsite);
                    _seleniumManager.TryExecuteNext();
                    Thread.Sleep(2000);
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

        [TestMethod]
        public void TestBrouseFail()
        {
            _seleniumManager.EnqueueAction(BrouseWebsiteFail);

            // Start processing the actions
            _seleniumManager.TryExecuteNext();

        }
        private string BrouseGoogleWebsite(IWebDriver driver)
        {
            // 
            try
            {
                driver.Url = "https://www.google.com/";

                //driver.Dispose();

            }
            catch (Exception ex)
            {

                throw;
            }
            return null;
        }

        private string BrouseWebsite(IWebDriver driver)
        {
            // 
            try
            {
                driver.Url = "https://dev.azure.com/Rohit-IN/Selenium%20Manager/";

                driver.FindElement(By.XPath("//a[@aria-label='Repos']")).Click();

                driver.Dispose();

            }
            catch (Exception ex)
            {

                throw;
            }
            return null;
        }
        private string BrouseWebsiteFail(IWebDriver driver)
        {
            // 
            driver.Url = "https://dev.azure.com/Rohit-IN/Selenium%20Manager/";

            IWebElement element = driver.FindElement(By.XPath("//div[@aria-label='Repos']"));
            if (element == null)
            {
                throw new NoSuchElementException("Element not found.");
            }
            return null;
        }

    }
}
