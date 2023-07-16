using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chromium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumManager.Core.DataContract
{
    public class Options
    {
        public ChromeOptions chromeOptions { get; set; } = GetChromeOptions();
        public FirefoxOptions firefoxOptions { get; set; } = GetFirefoxOptions();
        public EdgeOptions edgeOptions { get; set; } = GetEdgeOptions();
        public InternetExplorerOptions internetExplorerOptions { get; set; } = GetInternetExplorerOptions();
        public SafariOptions safariOptions { get; set; } = GetSafariOptions();

        public static ChromeOptions GetChromeOptions()
        {
            var chromeOptions = new ChromeOptions();
#if !DEBUG
            chromeOptions.AddArgument("headless");
#endif
            chromeOptions.AddArgument("disable-gpu");
            chromeOptions.AddArgument("no-sandbox");
            chromeOptions.AddArgument("--blink-settings=imagesEnabled=false");
            return chromeOptions;
        }

        public static FirefoxOptions GetFirefoxOptions()
        {
            var firefoxOptions = new FirefoxOptions();
#if !DEBUG
            firefoxOptions.AddArgument("headless");
#endif
            firefoxOptions.AddArgument("disable-gpu");
            firefoxOptions.AddArgument("no-sandbox");
            firefoxOptions.AddArgument("--blink-settings=imagesEnabled=false");
            return firefoxOptions;
        }

        public static EdgeOptions GetEdgeOptions()
        {
            var edgeOptions = new EdgeOptions();
#if !DEBUG
            edgeOptions.AddArgument("headless");
#endif
            edgeOptions.AddArgument("disable-gpu");
            edgeOptions.AddArgument("no-sandbox");
            edgeOptions.AddArgument("--blink-settings=imagesEnabled=false");
            return edgeOptions;
        }

        public static InternetExplorerOptions GetInternetExplorerOptions() => new InternetExplorerOptions();

        public static SafariOptions GetSafariOptions()  =>  new SafariOptions();
        
    }
}
