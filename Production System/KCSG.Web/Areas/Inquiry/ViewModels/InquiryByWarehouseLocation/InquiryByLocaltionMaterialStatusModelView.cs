using KCSG.Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Helper;

namespace KCSG.Web.Areas.Inquiry.ViewModels.InquiryByWarehouseLocation
{
    public class InquiryByLocaltionMaterialStatusModelView
    {
        public string TitlePopup { get; set; }

        public string ShelfRow { get; set; }
        public string ShelfBay { get; set; }
        public string ShelfLevel { get; set; }

        [Display(Name = @"Shelf No")]
        public string ShelfNo { get; set; }

        public string ShelfStatus { get; set; }
        public Constants.InquirySearchConditionWarehouseLocation SearchCondition { get; set; }
        public string Status { get; set; }
        public Constants.F51_ShelfType ProductShelfType { get; set; }

        public List<SelectListItem> ListStatus
        {
            get
            {
                if (SearchCondition == Constants.InquirySearchConditionWarehouseLocation.Material)
                {
                    return
                        EnumsHelper.GetListItemsWithDescription<Constants.TX31SheflStatusFor022F>(
                            Constants.TX31SheflStatusFor022F.TX31_MtrShfSts_Epy).OrderBy(x=>x.Value).ToList();
                }

                if (SearchCondition == Constants.InquirySearchConditionWarehouseLocation.PreProduct)
                {
                    return
                        EnumsHelper.GetListItemsWithDescription<Constants.TX37SheflStatusFor022F>(
                            Constants.TX37SheflStatusFor022F.TX37_ShfSts_Epy).OrderBy(x => x.Value).ToList();
                }
                
                if (SearchCondition == Constants.InquirySearchConditionWarehouseLocation.Product)
                {
                    if (ProductShelfType == Constants.F51_ShelfType.Normal)
                    {
                        return
                            EnumsHelper.GetListItemsWithDescription<Constants.TX51SheflStatusShelfTypeNormal>(
                                Constants.TX51SheflStatusShelfTypeNormal.TX51_ShfSts_Epy).OrderBy(x => x.Text).ToList();

                    }
                    else
                    {
                        return
                            EnumsHelper.GetListItemsWithDescription<Constants.TX51SheflStatusShelfTypeBad>(
                                Constants.TX51SheflStatusShelfTypeBad.TX51_ShfSts_Epy).OrderBy(x => x.Text).ToList();
                    }
                }
                return new List<SelectListItem>();
            }
        }
        [Display(Name = @"Old Status")]
        public string OldStatus
        {
            get
            {
                if (this.SearchCondition == Constants.InquirySearchConditionWarehouseLocation.Material)
                {
                    foreach (Constants.TX31OlDSheflStatus suit in Enum.GetValues(typeof(Constants.TX31OlDSheflStatus)))
                    {
                        if (((int)suit).ToString().Equals(this.ShelfStatus)) return EnumsHelper.GetDescription(suit);
                    }
                }
                if (this.SearchCondition == Constants.InquirySearchConditionWarehouseLocation.PreProduct)
                {
                    foreach (Constants.TX37OldSheflStatus suit in Enum.GetValues(typeof(Constants.TX37OldSheflStatus)))
                    {
                        if (((int)suit).ToString().Equals(this.ShelfStatus)) return EnumsHelper.GetDescription(suit);
                    }
                }
                if (this.SearchCondition == Constants.InquirySearchConditionWarehouseLocation.Product)
                {
                    foreach (Constants.TX51SheflStatus suit in Enum.GetValues(typeof(Constants.TX51SheflStatus)))
                    {
                        if (((int)suit).ToString().Equals(this.ShelfStatus)) return EnumsHelper.GetDescription(suit);
                    }
                }
                return "ERROR";
            }
            
        }

        public bool StatusIsHidden
        {
            get
            {
                if (this.SearchCondition == Constants.InquirySearchConditionWarehouseLocation.Material || this.SearchCondition == Constants.InquirySearchConditionWarehouseLocation.PreProduct)
                {
                    return this.OldStatus == "ERROR" || this.OldStatus == "Physic";
                }
                
                if (this.SearchCondition == Constants.InquirySearchConditionWarehouseLocation.Product)
                {
                    return this.OldStatus == "ERROR" || this.OldStatus == "Physic";
                }
                return false;
            }
        }
    }
}