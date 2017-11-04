using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.EnvironmentManagement
{
   public class LotdataSamplingItem
    {
        public double HighTemp { get; set; }

        public double LowTemp { get; set; }

        public double RangeTemp { get; set; }

        public double MeanTemp { get; set; }

        public double SigmaTemp { get; set; }
       public string ChartName { get; set; }
       public List<double> Dec_upper { get; set; }
       public List<double> Dec_mean { get; set; }
       public List<double> Dec_lower { get; set; }
       public List<double> Dec_data { get; set; }
       public List<string> dt_dtm { get; set; }
    }
}
