using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KCSG.Domain.Models.Inquiry.ByExternalPre_ProductName
{
    public class PrintExternalPreProductNameGroupItem
    {
        public double LotTotal { get; set; }
        public string LotTotalString { get; set; }

        public string OutsidePreProductCode { get; set; }
        public string OutsidePreProductLotNo { get; set; }

        public string PreProductCode { get; set; }
        public string PreProductLotNo { get; set; }

        public IList<FindPrintExternalPreProductNameItem> FindPrintExternalPreProductNameItem { get; set; }

        public PrintExternalPreProductNameGroupItem()
        {
            FindPrintExternalPreProductNameItem = new List<FindPrintExternalPreProductNameItem>();
        }
    }
}
