using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Domain.Models.Inquiry.ByExternalPre_ProductName;

namespace KCSG.Domain.Models.ManagementReport
{
    public class PrintManagementReportItem
    {
        public string Page { get; set; }

        public string Datetime { get; set; }

        public string CompanyName { get; set; }
        public string Month { get; set; }

        public double Total1 { get; set; }
        public double Total2 { get; set; }
        public string Total { get; set; }
        public IList<FindPrintManagementReportItem> FindPrintManagementReportItem { get; set; }
        public IList<GroupItems> GroupItemses { get; set; }
    }
}
