using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.Inquiry.BySupplierName
{
   public class PrintSupplierNameGroupItem
    {

        public double LotTotal { get; set; }
        public string LotTotalString { get; set; }


        public string MaterialCode { get; set; }
        //public string PreProductLotNo { get; set; }

        public IList<FindPrintSupplierNameItem> FindPrintSupplierNameItem { get; set; }

        public PrintSupplierNameGroupItem()
        {
            FindPrintSupplierNameItem = new List<FindPrintSupplierNameItem>();
        }
    }
}
