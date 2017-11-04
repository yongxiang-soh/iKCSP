using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Packaging;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.ReStorageOfProduct
{
    public class ReStorageOfProductViewModel
    {
        [Display(Name = @"Pallet No")]
        [Required(ErrorMessageResourceType = typeof (MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(3)]
        public string PalletNo { get; set; }

        [Display(Name = @"Product Code")]
        [StringLength(12)]
        public string ProductCode1 { get; set; }

        [StringLength(12)]
        public string ProductCode2 { get; set; }

        [StringLength(12)]
        public string ProductCode3 { get; set; }

        [StringLength(12)]
        public string ProductCode4 { get; set; }

        [StringLength(12)]
        public string ProductCode5 { get; set; }

        [Display(Name = @"Product Name")]
        [StringLength(20)]
        public string ProductName1 { get; set; }

        [StringLength(20)]
        public string ProductName2 { get; set; }

        [StringLength(20)]
        public string ProductName3 { get; set; }

        [StringLength(20)]
        public string ProductName4 { get; set; }

        [StringLength(20)]
        public string ProductName5 { get; set; }

        [Display(Name = @"Prepdt LotNo")]
        [StringLength(10)]
        public string PreProductLotNo1 { get; set; }

        [StringLength(10)]
        public string PreProductLotNo2 { get; set; }

        [StringLength(10)]
        public string PreProductLotNo3 { get; set; }

        [StringLength(10)]
        public string PreProductLotNo4 { get; set; }

        [StringLength(10)]
        public string PreProductLotNo5 { get; set; }

        [Display(Name = @"Lot No")]
        [StringLength(10)]
        public string LotNo1 { get; set; }

        [StringLength(10)]
        public string LotNo2 { get; set; }

        [StringLength(10)]
        public string LotNo3 { get; set; }

        [StringLength(10)]
        public string LotNo4 { get; set; }

        [StringLength(10)]
        public string LotNo5 { get; set; }

        [Display(Name = @"Pack Unit")]
        public double PackUnit1 { get; set; }

        public double PackUnit2 { get; set; }

        public double PackUnit3 { get; set; }

        public double PackUnit4 { get; set; }

        public double PackUnit5 { get; set; }

        [Display(Name = @"Remainder")]
        public int? Remainder1 { get; set; }

        public int? Remainder2 { get; set; }

        public int? Remainder3 { get; set; }

        public int? Remainder4 { get; set; }

        public int? Remainder5 { get; set; }

        [Display(Name = @"Fraction")]
        public double Fraction1 { get; set; }

        public double Fraction2 { get; set; }

        public double Fraction3 { get; set; }

        public double Fraction4 { get; set; }

        public double Fraction5 { get; set; }

        [Display(Name = @"Total")]
        public double Total1 { get; set; }

        public double Total2 { get; set; }

        public double Total3 { get; set; }

        public double Total4 { get; set; }

        public double Total5 { get; set; }

        public string EndDate1 { get; set; }
        public string EndDate2 { get; set; }
        public string EndDate3 { get; set; }
        public string EndDate4 { get; set; }
        public string EndDate5 { get; set; }

        public string CertificationFlag1 { get; set; }
        public string CertificationFlag2 { get; set; }
        public string CertificationFlag3 { get; set; }
        public string CertificationFlag4 { get; set; }
        public string CertificationFlag5 { get; set; }

        public string CertificationDate1 { get; set; }
        public string CertificationDate2 { get; set; }
        public string CertificationDate3 { get; set; }
        public string CertificationDate4 { get; set; }
        public string CertificationDate5 { get; set; }

        public string AddDate1 { get; set; }
        public string AddDate2 { get; set; }
        public string AddDate3 { get; set; }
        public string AddDate4 { get; set; }
        public string AddDate5 { get; set; }

        public bool isChecked { get; set; }
    }
}