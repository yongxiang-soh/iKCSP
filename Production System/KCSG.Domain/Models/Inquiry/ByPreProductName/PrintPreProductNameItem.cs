using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Domain.Models.Inquiry.ByExternalPre_ProductName;

namespace KCSG.Domain.Models.Inquiry.ByPreProductName
{
    public class PrintPreProductNameItem
    {
        public string Page { get; set; }

        public string Datetime { get; set; }

        public string CompanyName { get; set; }

        public double Total1 { get; set; }

        public double Total2 { get; set; }

        public IList<FindPrintPreProductNameItem> FindPrintPreProductNameItem { get; set; } 

        public double PreproductTotal { get; set; }
        public string PreproductTotalString { get; set; }

        //public IList<PrintPreProductNameGroupItem> PrintPreProductNameGroupItem { get; set; }       
        public IList<PrintPreProductNameGroup> PrintPreProductNameGroup { get; set; }

        public PrintPreProductNameItem()
        {
            //PrintPreProductNameGroupItem = new List<PrintPreProductNameGroupItem>();
            PrintPreProductNameGroup = new List<PrintPreProductNameGroup>();
        }
    }
}
