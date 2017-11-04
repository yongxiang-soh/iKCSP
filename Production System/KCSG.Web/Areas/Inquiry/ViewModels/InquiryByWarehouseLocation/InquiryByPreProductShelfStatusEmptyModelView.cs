﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Core.Helper;

namespace KCSG.Web.Areas.Inquiry.ViewModels.InquiryByWarehouseLocation
{
    public class InquiryByPreProductShelfStatusEmptyModelView
    {
        [Display(Name = @"Shelf No")]
        public string ShelfNo { get; set; }

        [Display(Name = @"Status")]
        public string ShelfStatus { get; set; }

        [Required(ErrorMessage = @"Please input data for this required field.")]
        [Display(Name = @"Container Type")]
        public string ContainerType { get; set; }

        [Display(Name = @"Container Name")]
        public string ContainerName { get; set; }

        [Required(ErrorMessage = @"Please input data for this required field.")]
        [Display(Name = @"Container No")]
        //[Range(1, Int32.MaxValue, ErrorMessageResourceType = typeof(Resources.InQuiryResource),ErrorMessageResourceName = "TCSS000042")]
        [StringLength(3)]
        public string ContainerNo { get; set; }

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