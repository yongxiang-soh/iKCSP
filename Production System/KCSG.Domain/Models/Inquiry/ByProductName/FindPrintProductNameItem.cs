using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Helper;

namespace KCSG.Domain.Models.Inquiry.ByProductName
{
    public class FindPrintProductNameItem
    {
        public string ProductCode { get; set; }

        public string ProductName { get; set; }
        public string ProductLotNo { get; set; }
        public string PalletNo { get; set; }
        
        public string ShelfNo { get; set; }
        public double Amount { get; set; }
        public double Total { get; set; }
        public string AmountString
        {
            get { return String.Format("{0:#,##0.00}", Amount); }
        }        

        public string CerfFlag { get ; set; }

        public string CerfFlagEnum
        {
            get
            {
                return EnumsHelper.GetEnumDescription(
                      (Constants.CerFlag)Int32.Parse(this.CerfFlag));
            }
        }
    }
}
