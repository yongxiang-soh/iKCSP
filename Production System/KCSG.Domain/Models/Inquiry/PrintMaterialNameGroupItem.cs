using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Domain.Models.Inquiry;
using KCSG.Domain.Models.Inquiry.ByPreProductName;


namespace KCSG.Domain.Models.Inquiry
{
    public class PrintMaterialNameGroupItem
    {
        public double LotTotal { get; set; }
        public string LotTotalString { get; set; }

        //public string OutsidePreProductCode { get; set; }
        //public string OutsidePreProductLotNo { get; set; }

        public string MaterialCode { get; set; }
        //public string PreProductLotNo { get; set; }

        public IList<FindPrintMaterialNameItem> FindPrintMaterialNameItem { get; set; }

        public PrintMaterialNameGroupItem()
        {
            FindPrintMaterialNameItem = new List<FindPrintMaterialNameItem>();
        }
    }
}
