using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.ViewModels
{
    public class StorageOfProductSelectedViewModel
    {
        public Constants.StorageOfProductStatus StorageOfProductStatus { get; set; }
        public Grid GridNormal { get; set; }
        public Grid GridOutOfPlan { get; set; }

        
        [Display(Name = @"Pallet No")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(3)]
        public string PalletNo { get; set; }

        [Display(Name = @"Out of Spec")]
        public bool OutOfSpec { get; set; }

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
        [StringLength(15)]
        public string ProductName1 { get; set; }

        [StringLength(15)]
        public string ProductName2 { get; set; }

        [StringLength(15)]
        public string ProductName3 { get; set; }

        [StringLength(15)]
        public string ProductName4 { get; set; }

        [StringLength(15)]
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

        [Display(Name = @"Pack Qty")]
        public int? PackQty1 { get; set; }

        public int? PackQty2 { get; set; }

        public int? PackQty3 { get; set; }

        public int? PackQty4 { get; set; }

        public int? PackQty5 { get; set; }

        [Display(Name = @"Fraction")]
        public double? Fraction1 { get; set; }

        public double? Fraction2 { get; set; }

        public double? Fraction3 { get; set; }

        public double? Fraction4 { get; set; }

        public double? Fraction5 { get; set; }

        [Display(Name = @"Command No")]
        public string CommandNo1 { get; set; }

        public string CommandNo2 { get; set; }

        public string CommandNo3 { get; set; }

        public string CommandNo4 { get; set; }

        public string CommandNo5 { get; set; }

        public double PackUnit1 { get; set; }
        public double PackUnit2 { get; set; }
        public double PackUnit3 { get; set; }
        public double PackUnit4 { get; set; }
        public double PackUnit5 { get; set; }

        public DateTime TabletingEndDate1 { get; set; }
        public DateTime TabletingEndDate2 { get; set; }
        public DateTime TabletingEndDate3 { get; set; }
        public DateTime TabletingEndDate4 { get; set; }
        public DateTime TabletingEndDate5 { get; set; }




     }
}