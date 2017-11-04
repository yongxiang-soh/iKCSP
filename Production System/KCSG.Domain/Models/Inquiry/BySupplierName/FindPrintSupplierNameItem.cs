using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.Inquiry.BySupplierName
{
    public class FindPrintSupplierNameItem
    {
        public string MaterialCode { get; set; }        

        public string MaterialName { get; set; }

        public string PalletNo { get; set; }        
        public string MaterialLotNo { get; set; }        
        public string ShelfNo { get; set; }
        public double Amount { get; set; }
        public string AmountString
        {
            get { return String.Format("{0:#,##0.00}", Amount); }
        }

        public double Total { get; set; }

    }
}
