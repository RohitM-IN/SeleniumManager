using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumManager.Core.DataContract
{
    public class ConfigurationSettings
    {
        public string GridHost { get; set; }
        public List<string> Drivers { get; set; }
        public string Secret { get; set; }
        public int MaxConcurrency { get; set; }
        public Endpoints Endpoints { get; set; }
        public Options Options { get; set; } = new Options();
    }
}
