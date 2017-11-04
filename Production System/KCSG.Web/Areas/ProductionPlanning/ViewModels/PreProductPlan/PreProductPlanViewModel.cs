using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.PreProductPlan
{
    public class PreProductPlanViewModel
    {
        [Key, Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(12)]
        [Display(Name = @"Pre-Product  Code")]
        [Remote("CheckExistCode", "PreProductPlan", ErrorMessage = "The same record exists in database", HttpMethod = "Options", AdditionalFields = "F94_YearMonth,IsCreate")]
        public string F94_PrepdtCode { get; set; }
        [Display(Name = @"Pre-Product Name")]
        public string PreProductName { get; set; }
        [Key, Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
       // [Remote("CheckExistCode", "PreProductPlan", ErrorMessage = "The same record exists in database", HttpMethod = "Options", AdditionalFields = "F94_PrepdtCode,IsCreate")]
       // [Remote("CheckHoliday", "PreProductPlan", HttpMethod = "Options", AdditionalFields = "IsCreate", ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG29")]
        
        [Display(Name = @" Production Date")]
        public string F94_YearMonth { get; set; }
        [Display(Name = @"Production Quantity")]
        [ Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Range(0.01, double.MaxValue, ErrorMessageResourceName = "MSG36", ErrorMessageResourceType = typeof(MessageResource))]
        public double F94_amount { get; set; }
        public DateTime? F94_AddDate { get; set; }
        public DateTime? F94_UpdateDate { get; set; }
        public bool IsCreate { get; set; }
    }
}