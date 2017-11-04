using System;
using KCSG.Core.Constants;

namespace KCSG.Domain.Models.MaterialManagement
{
    public class StockTakingOfMaterialItem
    {
        public string F33_MaterialCode { get; set; }
        public string F01_MaterialDsp { get; set; }
        public string F32_PrcOrdNo { get; set; }
        public string F32_PrtDvrNo { get; set; }

        public string F30_AcceptClass { get; set; }

        public string AcceptClass { get { return Enum.GetName(typeof(Constants.TX30_Reception), Convert.ToInt32(this.F30_AcceptClass)); } }
        public string F31_ShelfRow { get; set; }
        public string F31_ShelfBay { get; set; }
        public string F31_ShelfLevel { get; set; }
        public DateTime F31_UpdateDate { get; set; }
        public string F33_PalletNo { get; set; }
        public string ShelfNo
        {
            get
            {
                return this.F31_ShelfRow + "-" + this.F31_ShelfBay + "-" + this.F31_ShelfLevel;
            }
        }
    }
}
