using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;

namespace KCSG.Domain.Models.EnvironmentManagement
{
    public class Graphtbl
    {
        
        public double? Dec_data { get; set; }
        public double? Dec_mean { get; set; }
        public double? Dec_upper { get; set; }
        public double? Dec_lower { get; set; }
        public  Constants.TypeOfTable buf_type { get; set; }
        public DateTime dt_dtm { get; set; }
        public  string Lot { get; set; }
        public  int Id { get; set; }

        ///
        /// for f_calc_aval
        public string Ser { get; set;}
        public string rot_status { get; set; }
        public double val { get; set; }
        public string time { get; set; } 
        public string col { get; set; }

    }
}
