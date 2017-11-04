using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.EnvironmentManagement.ViewModels.CalculationOfControlLimit
{
    public class CalculationOfControlLimitViewModel
    {
        public Grid Grid { get; set; }
        [Display(Name = @"Start Date")]
        [Required(ErrorMessage = @"Please input data for this required field.")]
        public string StartDate { get; set; }
        [Display(Name = @"End Date")]
        [Required(ErrorMessage = @"Please input data for this required field.")]
        public string EndDate { get; set; }
        [Display(Name = @"Mode")]
        public Constants.EnvMode EnvMode { get; set; }
        [Display(Name = @"Duration From")]
        public string DurationFrom { get; set; }
        [Display(Name = @"To")]
        public string DurationTo { get; set; }

    }
}