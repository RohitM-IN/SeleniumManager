using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumManager.Core.Interface
{
    public interface ISeleniumManager
    {
        Task<string> EnqueueAction(Func<IWebDriver, string> action);
    }
}
