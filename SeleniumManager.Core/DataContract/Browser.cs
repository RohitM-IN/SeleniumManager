using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumManager.Core.DataContract
{
    public class Browser
    {
        public string Name { get; set; }
        public int TotalInstance { get; set; } = 0;
        public int AvailableInstance { get; set; } = 0;
        public bool IsExcluded { get; set; } = false;
    }
}
 