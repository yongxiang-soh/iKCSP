using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.EnvironmentManagement
{
    public class ControlLimitEditItem
    {
        public string Location { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public double? TempUCL { get; set; }

        public double? TempCp { get; set; }

        public double? TempLCL { get; set; }

        public double? TempCpk { get; set; }

        public double? TempMean { get; set; }

        public double? TempRange { get; set; }

        public double? TempSigma { get; set; }



        public double? HumUCL { get; set; }

        public double? HumCp { get; set; }

        public double? HumLCL { get; set; }

        public double? HumCpk { get; set; }

        public double? HumMean { get; set; }

        public double? HumRange { get; set; }

        public double? HumSigma { get; set; }
    }
}
