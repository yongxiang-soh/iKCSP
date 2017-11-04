using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.PreProductManagement
{
    public class PreProductShelfStatusItem: TX37_PrePdtShfSts
    {
        public string F49_PreProductLotNo { get; set; }

        public string F49_ContainerCode { get; set; }

        public double F49_Amount { get; set; }

        public double GrandTotal { get; set; }
        public string ShelfNo { get; set; }
    }
}
