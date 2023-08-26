using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;

namespace SeleniumManager.Core.DataContract
{
    public class Options
    {
        public Options()
        {
            chromeOptions = GetChromeOptions();
            firefoxOptions = GetFirefoxOptions();
            edgeOptions = GetEdgeOptions();
            internetExplorerOptions = GetInternetExplorerOptions();
            safariOptions = GetSafariOptions();
            operaOptions = GetChromeOptions();
        }
        public ChromeOptions chromeOptions { get; set; }
        public FirefoxOptions firefoxOptions { get; set; }
        public EdgeOptions edgeOptions { get; set; }
        public InternetExplorerOptions internetExplorerOptions { get; set; }
        public SafariOptions safariOptions { get; set; }
        public ChromeOptions operaOptions { get; set; }

        public virtual ChromeOptions GetChromeOptions()
        {
            var chromeOptions = new ChromeOptions();
#if !DEBUG
            chromeOptions.AddArgument("headless");
#endif
            chromeOptions.AddArgument("disable-gpu");
            //chromeOptions.AddArgument("no-sandbox");
            chromeOptions.AddArgument("--blink-settings=imagesEnabled=false");
            return chromeOptions;
        }

        public virtual FirefoxOptions GetFirefoxOptions()
        {
            var firefoxOptions = new FirefoxOptions();

#if !DEBUG
            firefoxOptions.AddArgument("-headless");
#endif
            firefoxOptions.SetPreference("layers.acceleration.force-enabled", false);
            //firefoxOptions.SetPreference("dom.ipc.sandbox.enabled", false);
            firefoxOptions.SetPreference("permissions.default.image", 2);
            return firefoxOptions;
        }

        public virtual EdgeOptions GetEdgeOptions()
        {
            var edgeOptions = new EdgeOptions();
#if !DEBUG
            edgeOptions.AddArgument("headless");
#endif
            edgeOptions.AddArgument("disable-gpu");
            //edgeOptions.AddArgument("no-sandbox");
            edgeOptions.AddArgument("--blink-settings=imagesEnabled=false");
            return edgeOptions;
        }

        public virtual InternetExplorerOptions GetInternetExplorerOptions() => new InternetExplorerOptions();

        public virtual SafariOptions GetSafariOptions() => new SafariOptions();

    }
}
