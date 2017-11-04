using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.MaterialManagement
{
    public class MaterialShelfStatusItem : TX31_MtrShfSts
    {
        public string ShelfNo
        {
            get
            {
                if (string.IsNullOrEmpty(this.F31_ShelfRow) ||
                    string.IsNullOrEmpty(this.F31_ShelfBay) ||
                    string.IsNullOrEmpty(this.F31_ShelfLevel))
                {
                    return null;
                }
                else
                {
                    return this.F31_ShelfRow.Trim() + "~" + this.F31_ShelfBay.Trim() + "~" + this.F31_ShelfLevel.Trim();
                }
                
            }
        }
        public string StockedPallet { get { return this.F31_ShelfStatus == "1" ? "0" : this.F31_LoadAmount.ToString(); } }

        public string F33_MaterialLotNo { get; set; }

        public double F33_Amount { get; set; }

        public double GrandTotal { get; set; }
        public string ShelfNo1 { get; set; }

        public string MaterialName { get; set; }
        public string MaterialCode { get; set; }
    }
}
