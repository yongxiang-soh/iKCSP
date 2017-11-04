using System;

namespace KCSG.Web.Areas.ManagementReport.ViewModels.ManagementReportViewModel
{
    public class UpdateManagementReportViewModel
    {
        public string MaterialCode { get; set; }
        public string PreProductCode { get; set; }
        public string ProductCode { get; set; }

        public string MaterialName { get; set; }
        public DateTime YearMonth { get; set; }

        public double Remain { get; set; }

        public double Used { get; set; }

        public double Recieved { get; set; }

        public DateTime Updatedate { get; set; }
    }
}