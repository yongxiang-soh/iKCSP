using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Core.Helper;

namespace KCSG.Web.Areas.Inquiry.ViewModels.InquiryByWarehouseLocation
{
    public class InquiryByProductShelfStatusModelView
    {
        [Display(Name = @"Shelf No")]
        public string ShelfNo { get; set; }

        [Display(Name = @"Status")]
        public string ShelfStatus { get; set; }

        [StringLength(3)]
        [Required(ErrorMessage = "Please input data for this required field.")]
        [Display(Name = @"Pallet No")]
        public string PalletNo { get; set; }

        [Display(Name = @"Storage DateTime")]
        [Required(ErrorMessage = "Please input data for this required field.")]
        public string StorageDate { get; set; }

        [Display(Name = @"Product Classification")]
        public Constants.ProductClassification ProductClassification { get; set; }

        public string ProductCode1 { get; set; }
        public string ProductCode2 { get; set; }
        public string ProductCode3 { get; set; }
        public string ProductCode4 { get; set; }
        public string ProductCode5 { get; set; }

        public string PreProductLotNo1 { get; set; }
        public string PreProductLotNo2 { get; set; }
        public string PreProductLotNo3 { get; set; }
        public string PreProductLotNo4 { get; set; }
        public string PreProductLotNo5 { get; set; }

        public string ProductLotNo1 { get; set; }
        public string ProductLotNo2 { get; set; }
        public string ProductLotNo3 { get; set; }
        public string ProductLotNo4 { get; set; }
        public string ProductLotNo5 { get; set; }

        public int? PackQty1 { get; set; }
        public int? PackQty2 { get; set; }
        public int? PackQty3 { get; set; }
        public int? PackQty4 { get; set; }
        public int? PackQty5 { get; set; }

        public double? Fraction1 { get; set; }
        public double? Fraction2 { get; set; }
        public double? Fraction3 { get; set; }
        public double? Fraction4 { get; set; }
        public double? Fraction5 { get; set; }

        public double? Quantity1 { get; set; }
        public double? Quantity2 { get; set; }
        public double? Quantity3 { get; set; }
        public double? Quantity4 { get; set; }
        public double? Quantity5 { get; set; }

        public string Row { get; set; }
        public string Bay { get; set; }
        public string Level { get; set; }
        public bool ShowUpdate { get; set; }

        public Constants.InquirySearchConditionWarehouseLocation SearchCondition { get; set; }
        public Constants.F51_ShelfType ProductShelfType { get; set; }

        public List<SelectListItem> ListStatus
        {
            get
            {
                if (SearchCondition == Constants.InquirySearchConditionWarehouseLocation.Material)
                {
                    var selectValue = (Constants.TX31SheflStatus)Enum.Parse(typeof(Constants.TX31SheflStatus), ShelfStatus);
                    return
                        EnumsHelper.GetListItemsWithDescription<Constants.TX31SheflStatus>(
                            selectValue).OrderBy(x => x.Value).ToList();
                }

                if (SearchCondition == Constants.InquirySearchConditionWarehouseLocation.PreProduct)
                {
                    var selectValue = (Constants.TX37SheflStatus)Enum.Parse(typeof(Constants.TX37SheflStatus), ShelfStatus);
                    return
                        EnumsHelper.GetListItemsWithDescription<Constants.TX37SheflStatus>(
                            selectValue).OrderBy(x => x.Value).ToList();
                }

                if (SearchCondition == Constants.InquirySearchConditionWarehouseLocation.Product)
                {

                    var selectValue = (Constants.TX51SheflStatus)Enum.Parse(typeof(Constants.TX51SheflStatus), ShelfStatus);
                    return
                        EnumsHelper.GetListItemsWithDescription<Constants.TX51SheflStatus>(
                            selectValue).OrderBy(x => x.Value).ToList();
                }
                return new List<SelectListItem>();
            }
        }
    }
}