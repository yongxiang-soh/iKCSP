using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCSG.Core.Constants;

namespace KCSG.Web.Areas.ManagementReport.ViewModels.ManagementReportViewModel
{
    public class ReportStartEndSystemViewModel
    {
        public Constants.StatusEnd StatusEnd { get; set; }
        public string Status { get; set; }
        public bool Reload { get; set; }
        public Constants.StatusStart StatusStart { get; set; }
        public bool IsStart { get; set; }
        public bool Device { get; set; }
    }
}