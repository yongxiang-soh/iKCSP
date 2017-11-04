using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.jsGrid.MVC
{
    public class NumberOption
    {
        public double? Min { get; set; }
        public double? Max { get; set; }
        public string ASign { get; set; }
        public string PSign { get; set; }
        public int? NumberOfDecimal { get; set; }
    }
}
