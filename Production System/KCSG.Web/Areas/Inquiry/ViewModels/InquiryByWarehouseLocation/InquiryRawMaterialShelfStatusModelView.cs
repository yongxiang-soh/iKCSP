using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Web.Validators;

namespace KCSG.Web.Areas.Inquiry.ViewModels.InquiryByWarehouseLocation
{
    public class InquiryRawMaterialShelfStatusModelView
    {
        [Display(Name = @"Shelf No")]
        public string ShelfNo { get; set; }

        [Display(Name = @"Status")]
        public string ShelfStatus { get; set; }

        [StringLength(3)]
        [Required(ErrorMessage = @"Please input data for this required field.")]
        [Display(Name = @"Pallet No")]
        public string PalletNo { get; set; }

        [Required(ErrorMessage = @"Please input data for this required field.")]
        [Display(Name = @"Storage DateTime")]
        public string StorageDate { get; set; }

        [StringLength(12, ErrorMessage = @"Material Code cannot be longer than 12 characters.")]
        [Required(ErrorMessage = @"Please input data for this required field.")]
        [Display(Name = @"Material Code")]
        public string MaterialCode { get; set; }

        [StringLength(16, ErrorMessage = @"Material Name cannot be longer than 16 characters.")]
        [Display(Name = @"Material Name")]
        public string MaterialName { get; set; }

        [Display(Name = @"P.O.No")]
        [StringLength(15, ErrorMessage = @"P.O.NO cannot be longer than 7 characters.")]
        public string PrcordNo { get; set; }

        [Display(Name = @"Partial Delivery")]
        [StringLength(2, ErrorMessage = @"Partial Delivery cannot be longer than 7 characters.")]
        public string PrtdvrNo { get; set; }

        [Display(Name = @"Acceptance Classification")]
        public string AcceptanceClassification { get; set; }

        [Display(Name = @"Bailment Classification")]
        public string BailmentClassification { get; set; }

        public string MaterialLotNo1 { get; set; }
        public string MaterialLotNo2 { get; set; }
        public string MaterialLotNo3 { get; set; }
        public string MaterialLotNo4 { get; set; }
        public string MaterialLotNo5 { get; set; }
        public double Quantity1 { get; set; }
        public double Quantity2 { get; set; }
        public double Quantity3 { get; set; }
        public double Quantity4 { get; set; }
        public double Quantity5 { get; set; }

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
                    
                    if (ProductShelfType == Constants.F51_ShelfType.Normal)
                    {
                        var selectValue = (Constants.TX51SheflStatusShelfTypeNormal)Enum.Parse(typeof(Constants.TX51SheflStatusShelfTypeNormal), ShelfStatus);
                        return
                            EnumsHelper.GetListItemsWithDescription<Constants.TX51SheflStatusShelfTypeNormal>(
                                selectValue).OrderBy(x => x.Value).ToList();

                    }
                    else
                    {
                        var selectValue = (Constants.TX51SheflStatusShelfTypeBad)Enum.Parse(typeof(Constants.TX51SheflStatusShelfTypeBad), ShelfStatus);
                        return
                            EnumsHelper.GetListItemsWithDescription<Constants.TX51SheflStatusShelfTypeBad>(
                                selectValue).OrderBy(x => x.Value).ToList();
                    }
                }
                return new List<SelectListItem>();
            }
        }
    }
}