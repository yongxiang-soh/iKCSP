using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KCSG.Domain.Models.Inquiry.ByPreProductName
{
    public class PrintPreProductNameGroupItem
    {
        public double LotTotal { get; set; }
        public string LotTotalString { get; set; }

        //public double PreproductTotal { get; set; }
        //public string PreproductTotalString { get; set; }

        public string OutsidePreProductCode { get; set; }
        public string OutsidePreProductLotNo { get; set; }

        public string PreProductCode { get; set; }
        public string PreProductLotNo { get; set; }

        public IList<FindPrintPreProductNameItem> FindPrintPreProductNameItem { get; set; }

        public PrintPreProductNameGroupItem()
        {
            FindPrintPreProductNameItem = new List<FindPrintPreProductNameItem>();
        }
    }
}
