using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.PreProductPlan
{
    public class PreProductPlanSearchViewModel
    {
        [Display(Name = @"Year-Month")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [DataType(DataType.Date)]
        public string YearMonth { get; set; }
        public Grid Grid { get; set; }
    }
}