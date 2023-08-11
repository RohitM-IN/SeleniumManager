using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumManager.Core.Enum
{
    public enum WebDriverType
    {
        None = 0,
        [Description("chrome")]
        Chrome = 1,
        [Description("MicrosoftEdge")]
        MicrosoftEdge = 2,
        [Description("firefox")]
        Firefox = 3,
        [Description("safari")]
        Safari = 4,
        [Description("internet explorer")]
        InternetExplorer = 5,
        [Description("opera")]
        Opera = 6,
        [Description("Custom")]
        Custom = 7,
    }
}
