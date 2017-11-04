using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.ManagementReport
{
    public class PrintMaterialNameItem
    {
        public string Page { get; set; }

        public string Datetime { get; set; }

        public string CompanyName { get; set; }

        public string Total { get; set; }
        public IList<FindPrintMaterialNameGroupItem> groupitems { get; set; }
        public IList<FindPrintMaterialNameItem> FindPrintMaterialNameItem { get; set; }
        public IList<SubFindPrintMaterialNameItem> FindPrintSubMaterialNameItem { get; set; }
        public IList<FindPrintMaterialSheifItem> FindPrintMaterialSheifItem { get; set; }
    }
}
