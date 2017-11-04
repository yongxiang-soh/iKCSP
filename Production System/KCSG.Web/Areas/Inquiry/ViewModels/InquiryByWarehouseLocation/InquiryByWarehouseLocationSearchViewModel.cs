using System.Collections.Generic;
using System.Web.Mvc;
using KCSG.Core.Constants;

namespace KCSG.Web.Areas.Inquiry.ViewModels.InquiryByWarehouseLocation
{
    public class InquiryByWarehouseLocationSearchViewModel
    {
        public Constants.InquirySearchConditionWarehouseLocation SearchCondition { get; set; }
        public Constants.ExecutingClassification ExecutingClassification { get; set; }
        public string PrintOption { get; set; }
        public List<SelectListItem> ListPrintOption { get; set; }
        public string ShelfNoRow { get; set; }
        public List<SelectListItem> ListShelfNoRow { get; set; }
        public string ShelfNoBay { get; set; }
        public List<SelectListItem> ListShelfNoBay { get; set; }
        public string ShelfNoLevel { get; set; }
        public List<SelectListItem> ListShelfNoLevel { get; set; }
    }
}