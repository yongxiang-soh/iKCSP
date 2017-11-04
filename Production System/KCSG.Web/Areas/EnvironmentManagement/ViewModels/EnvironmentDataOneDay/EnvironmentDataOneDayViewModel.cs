using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;

namespace KCSG.Web.Areas.EnvironmentManagement.ViewModels.EnvironmentDataOneDay
{
    public class EnvironmentDataOneDayViewModel
    {
        [Display(Name = @"Mode")]
        public Constants.EnvMode EnvMode { get; set; }

        [Display(Name = @"Date")]
        [Required(ErrorMessage = @"Please input data for this required field.")]
        public string EnvironmentDate { get; set; }
        [Display(Name = @"Location")]
        public string Location { get; set; }
    }
}