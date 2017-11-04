using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.Inquiry.ByExternalPre_ProductName
{
    public class FindPrintExternalPreProductNameItem
    {
        public string PreProductCode { get; set; }

        public string OutsidePreProductCode { get; set; }

        public string PreProductName { get; set; }

        public string PalletNo { get; set; }

        public string ContainerCode { get; set; }
        public string PreProductLotNo { get; set; }

        public string OutsidePreProductLotNo { get; set; }

        public string ShelfNo { get; set; }
        public double Amount { get; set; }
        public string AmountString
        {
            get { return String.Format("{0:#,##0.00}", Amount); }
        }
    }
}
