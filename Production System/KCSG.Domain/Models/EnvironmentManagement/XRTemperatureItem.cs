using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.EnvironmentManagement
{
    public class XRTemperatureItem
    {
        public List<double> tblTemp { get; set; }

        public string HighTemp { get; set; }
        
        public string LowTemp { get; set; }
        
        public string RangeTemp { get; set; }
        
        public string MeanTemp { get; set; }
        
        public string SigmaTemp { get; set; }
        
        public string UCLTemp { get; set; }
        
        public string LCLTemp { get; set; }
        
        public string CpTemp { get; set; }
        
        public string CpkTemp { get; set; }
        public List<double> tblHumid { get; set; }
        public string HighHumid { get; set; }
        public string LowHumid { get; set; }
        public string RangeHumid { get; set; }
        public string MeanHumid { get; set; }
        public string SigmaHumid { get; set; }
        public string UCLHumid { get; set; }
        public string LCLHumid { get; set; }
        public string CpHumid { get; set; }
        public string CpkHumid { get; set; }
        public List<string> TimeTemp { get; set; }
        public List<string> TimeHumid { get; set; }
    }
}
