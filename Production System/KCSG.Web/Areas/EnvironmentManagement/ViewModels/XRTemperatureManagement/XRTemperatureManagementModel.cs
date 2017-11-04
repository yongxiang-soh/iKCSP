using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace KCSG.Web.Areas.EnvironmentManagement.ViewModels
{
    public class XRTemperatureManagementModel
    {
        public SearchCriteriaModel SearchCriteriaModel { get; set; }
        public ChartModel TempModel { get; set; }
        public ChartModel HimIdModel { get; set; }
        

    }

    public class ChartModel
    {
        public string ChartName { get; set; }
        [DisplayName("High")]
        public string High { get; set; }
        [DisplayName("Low")]
        public string Low { get; set; }
        [DisplayName("Range")]
        public string Range { get; set; }
        [DisplayName("Mean")]
        public string Mean { get; set; }
        [DisplayName("Sigma")]
        public string Sigma { get; set; }
        [DisplayName("UCL")]
        public string UCL { get; set; }
        [DisplayName("LCL")]
        public string LCL { get; set; }
        [DisplayName("Cp")]
        public string Cp { get; set; }
        [DisplayName("Cpk")]
        public string Cpk { get; set; }

        public List<string> LstTime { get; set; }
        public List<double> LstData { get; set; }
    }
}