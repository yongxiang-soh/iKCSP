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
    public class InquiryByProductShelfStatusExternalPreProductModelView
    {
        [Display(Name = @"Shelf No")]
        public string ShelfNo { get; set; }

        [Display(Name = @"Status")]
        public string ShelfStatus { get; set; }

        [Required(ErrorMessage = @"Please input data for this required field.")]
        [Display(Name = @"Pre-product Code")]
        public string PreProductCode { get; set; }

        [Display(Name = @"Pre-product Name")]
        public string PreProductName { get; set; }

        [Required(ErrorMessage = @"Please input data for this required field.")]
        [Display(Name = @"Pre-product LotNo")]
        public string PreProductLotNo { get; set; }

        [Required(ErrorMessage = @"Please input data for this required field.")]
        [Display(Name = @"Pallet No")]
        public string PalletNo { get; set; }

        [Display(Name = @"Quantity")]
        [Required(ErrorMessage = @"Please input data for this required field.")]
        [Range(0.01, double.MaxValue, ErrorMessageResourceType = typeof(Resources.InQuiryResource), ErrorMessageResourceName = "TCFC034F01")]
        public double Amount { get; set; }

        [Display(Name = @"Kneading CmdNo")]
        [Required(ErrorMessage = @"Please input data for this required field.")]
        public string KneadingCommandNo { get; set; }

        [Display(Name = @"Pallet SeqNo")]
        [Required(ErrorMessage = @"Please input data for this required field.")]
        public int PalletSeqNo { get; set; }

        [Display(Name = @"Storage DateTime")]
        [Required(ErrorMessage = @"Please input data for this required field.")]
        public string StorageDate { get; set; }

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