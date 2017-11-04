using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.MaterialRequirementList
{
    public class MaterialReqListSearchViewModel
    {
        [Display(Name = @"Year-Month")]
        [DataType("Time")]
        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        public string DateSearch { get; set; }
        public Grid Grid { get; set; }
        public Grid DefaultGrid { get; set; }
    }
}