using System;

namespace KCSG.Domain.Models.PreProductManagement
{
    public class SearchStockTakingPreProductItem
    {
        public string F37_ShelfRow { get; set; }

        public string F37_ShelfBay { get; set; }

        public string F37_ShelfLevel { get; set; }

        public string F49_PreProductCode { get; set; }

        public string F03_PreProductName { get; set; }

        public string F49_PreProductLotNo { get; set; }

        public double F49_Amount { get; set; }

        public string F37_ContainerType { get; set; }

        public string F49_ContainerCode { get; set; }

        public DateTime F37_UpdateDate { get; set; }

        public string F37_ContainerNo { get; set; }

        public string ShelfNo
        {
            get { return string.Format("{0}-{1}-{2}", F37_ShelfRow, F37_ShelfBay, F37_ShelfLevel); }
        }

        public string F49_KneadingCommandNo { get; set; }
    }

    public class SearchInquiryByPreProductCode
    {
        public string F01_Materialdsp { get; set; }
        public string F43_MaterialLotNo { get; set; }
        public string F43_MaterialCode { get; set; }
        public string F43_Materiallotno { get; set; }
        public string F43_KndCmdNo { get; set; }
        public string F43_PrePdtLotNo { get; set; }
        public string F42_PreProductCode { get; set; }
        public double F43_LayinginAmount { get; set; }
        public string F43_MaterialLotno { get; set; }
        public double Total { get; set; }
        public String Total1 { get; set; }
    }
}