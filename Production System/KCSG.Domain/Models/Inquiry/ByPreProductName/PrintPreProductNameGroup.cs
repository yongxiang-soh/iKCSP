using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KCSG.Domain.Models.Inquiry.ByPreProductName
{
    public class PrintPreProductNameGroup
    {
        public string PreProductCode { get; set; }
        public double PreproductTotal { get; set; }
        public string PreproductTotalString { get; set; }

        //public IList<FindPrintPreProductNameItem> FindPrintPreProductNameItem { get; set; }

        public IList<PrintPreProductNameGroupItem> PrintPreProductNameGroupItem { get; set; }

        public PrintPreProductNameGroup()
        {
            PrintPreProductNameGroupItem = new List<PrintPreProductNameGroupItem>();
        }                        
    }
}
