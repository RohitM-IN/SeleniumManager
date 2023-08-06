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
        [Description("Chrome")]
        Chrome = 1,
        [Description("Microsoft Edge")]
        Microsoft_Edge = 2,
        [Description("Firefox")]
        Firefox = 3,
        [Description("Safari")]
        Safari = 4,
        [Description("InternetExplorer")]
        InternetExplorer = 5,
        [Description("Opera")]
        Opera = 6,
        [Description("Custom")]
        Custom = 7,
    }
}
