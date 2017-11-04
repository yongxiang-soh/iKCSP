using System;
using System.ComponentModel.DataAnnotations;
using KCSG.Core;
using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.PdtPln
{
    public class PdtPlnSearchViewModel
    {
        
        public string ProductCode { get; set; }
        [Display(Name = @"Year-Month")]
        //[Range(typeof(DateTime), "01/1991", "01/2100", ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG27")]
        [DataType(DataType.Date)]
       [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string YearMonth { get; set; }
        [Display(Name = @"Line")]
        public Constants.KndLine KndLine { get; set; }
        public Grid Grid { get; set; }
    }
}