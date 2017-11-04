using System;
using System.ComponentModel.DataAnnotations;

namespace KCSG.Domain.Models.ProductManagement
{
    public class StockTakingOfProductConfirmItem
    {
        [Required]
        public string F40_PrePdtLotNo { get; set; }

        [Required]
        public string F40_ProductCode { get; set; }
        
        [Required]
        public string F40_ProductLotNo { get; set; }

        public double F09_PackingUnit { get; set; }

        public int PackQty { get; set; }
        
        [Range(0, 999)]
        public double Fraction { get; set; }

        public DateTime F40_Tabletingenddate { get; set; }

        public string F40_CertificationFlg { get; set; }

        public DateTime? F40_Certificationdate { get; set; }

        public DateTime F40_AddDate { get; set; }
    }
}