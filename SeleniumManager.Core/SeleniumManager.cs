using OpenQA.Selenium;
using SeleniumManager.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumManager.Core
{
    public class SeleniumManager: ISeleniumManager
    {
        private readonly ConfigManager configManager;

        public SeleniumManager(ConfigManager configManager)
        {
            this.configManager = configManager;
        }

        public void EnqueueAction(Action<IWebDriver> action)
        {
            throw new NotImplementedException();
        }
    }
}
