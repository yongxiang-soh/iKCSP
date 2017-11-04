using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.EnvironmentManagement
{
    public class CreepingAndRollSpeedDurationItem
    {
        public ChartModel LeftModel { get; set; }
        public ChartModel RightModel { get; set; }
        public ChartModel RollModel { get; set; }
        
    }

    public class ChartModel
    {
        public List<double> Data2 { get; set; }
        public string High { get; set; }

        public string Low { get; set; }

        public string Range { get; set; }

        public string Mean { get; set; }

        public string Sigma { get; set; }

        public string UCL { get; set; }

        public string LCL { get; set; }

        public string Cp { get; set; }

        public string Cpk { get; set; }
        public List<double> Data1 { get; set; }
        public List<double> Data3 { get; set; }
        public List<double> Data4 { get; set; }
        public string ChartName { get; set; }
        public List<string> lstTime { get; set; }
    }
}
