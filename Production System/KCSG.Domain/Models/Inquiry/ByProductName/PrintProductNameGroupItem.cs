using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Domain.Models.Inquiry.ByExternalPre_ProductName;


namespace KCSG.Domain.Models.Inquiry.ByProductName
{
    public class PrintProductNameGroupItem
    {
        public double LotTotal { get; set; }
        public string LotTotalString { get; set; }
       

        public string ProductCode { get; set; }
        //public string PreProductLotNo { get; set; }

        public IList<FindPrintProductNameItem> FindPrintProductNameItem { get; set; }

        public PrintProductNameGroupItem()
        {
            FindPrintProductNameItem = new List<FindPrintProductNameItem>();
        }
    }
}
