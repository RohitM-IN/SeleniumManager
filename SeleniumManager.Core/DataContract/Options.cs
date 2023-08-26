using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;

namespace SeleniumManager.Core.DataContract
{
    public class Options
    {
        public ChromeOptions chromeOptions { get; set; } = GetChromeOptions();
        public FirefoxOptions firefoxOptions { get; set; } = GetFirefoxOptions();
        public EdgeOptions edgeOptions { get; set; } = GetEdgeOptions();
        public InternetExplorerOptions internetExplorerOptions { get; set; } = GetInternetExplorerOptions();
        public SafariOptions safariOptions { get; set; } = GetSafariOptions();
        public ChromeOptions operaOptions { get; set; } = GetChromeOptions();

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
            firefoxOptions.AddArgument("-headless");
#endif
            firefoxOptions.SetPreference("layers.acceleration.force-enabled", false);
            //firefoxOptions.SetPreference("dom.ipc.sandbox.enabled", false);
            firefoxOptions.SetPreference("permissions.default.image", 2);
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

        public static SafariOptions GetSafariOptions() => new SafariOptions();

    }
}
