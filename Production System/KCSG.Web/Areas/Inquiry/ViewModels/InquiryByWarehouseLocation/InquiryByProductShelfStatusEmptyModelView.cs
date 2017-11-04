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
    public class InquiryByProductShelfStatusEmptyModelView
    {
        [Display(Name = @"Shelf No")]
        public string ShelfNo { get; set; }

        [Display(Name = @"Status")]
        public string ShelfStatus { get; set; }

        [Display(Name = @"Pallet Load Amout")]
        [Range(1, Int32.MaxValue, ErrorMessageResourceType = typeof(Resources.InQuiryResource), ErrorMessageResourceName = "TCSS000042")]
        [Required(ErrorMessage = @"This field must be inputted!")]
        public int PalletLoadAmout { get; set; }

        public int MaxLoad { get; set; }

        public string Row { get; set; }
        public string Bay { get; set; }
        public string Level { get; set; }
        public bool ShowUpdate { get; set; }
        public Constants.InquirySearchConditionWarehouseLocation SearchCondition { get; set; }
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