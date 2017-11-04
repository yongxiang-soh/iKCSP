using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.Inquiry
{
    public class PrintMaterialNameItem
    {
        public string Page { get; set; }

        public string Datetime { get; set; }

        public string CompanyName { get; set; }

        public double Total { get; set; }
        public string TotalNormalString { get; set; }
        public string TotalBailmentString { get; set; }
        public IList<FindPrintMaterialNameItem> FindPrintMaterialNameItem { get; set; }

        public IList<PrintMaterialNameGroupItem> PrintMaterialNameGroupItem { get; set; }

        public PrintMaterialNameItem()
        {
            PrintMaterialNameGroupItem = new List<PrintMaterialNameGroupItem>();
        }
    }
}
