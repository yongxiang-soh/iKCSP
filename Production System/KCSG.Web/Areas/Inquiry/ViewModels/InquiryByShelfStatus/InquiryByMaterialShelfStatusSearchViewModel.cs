using KCSG.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KCSG.Web.Areas.Inquiry.ViewModels.InquiryByShelfStatus
{
    public class InquiryByShelfStatusSearchViewModel
    {
        public Constants.InquirySearchConditionWarehouseLocation Type { get; set; }
        public Constants.InquirySearchConditionShelfStatus SearchCondition { get; set; }
        public Constants.ExecutingClassification ExecutingClassification { get; set; }
    }
}