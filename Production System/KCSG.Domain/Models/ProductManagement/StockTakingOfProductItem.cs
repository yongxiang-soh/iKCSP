using System;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.ProductManagement
{
    public class StockTakingOfProductItem : TX51_PdtShfSts
    {
        /// <summary>
        /// Shelf no
        /// </summary>
        public string ShelfNo
        {
            get { return string.Format("{0} - {1} - {2}", F51_ShelfRow, F51_ShelfBay, F51_ShelfLevel); }
        }

        public string LotNo { get; set; }
        public double Amount { get; set; }
        public string ShelfNo1 { get; set; }

        public string ProductLotNo { get; set; }
        public string PreProdLotNo { get; set; }
        public Nullable<System.DateTime> CerfDate { get; set; }
        public string CerfFlag { get; set; }
        //public string CerfFlag
        //{
        //    get
        //    {
        //        return
        //            EnumsHelper.GetEnumDescription(
        //                (Constants.F40_CertificationFlag)Int32.Parse(this.CerfFlag));
        //    }
        //    set {}
        //}

        //public string OutofSpecFlag
        //{
        //    get
        //    {
        //        return
        //            EnumsHelper.GetEnumDescription(
        //                (Constants.F40_CertificationFlag)Int32.Parse(this.CerfFlag));
        //    }
        //    set { CerfFlag = ""; }
        //}
    }
}