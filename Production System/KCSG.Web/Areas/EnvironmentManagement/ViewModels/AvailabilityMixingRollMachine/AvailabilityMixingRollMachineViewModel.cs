using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;

namespace KCSG.Web.Areas.EnvironmentManagement.ViewModels.AvailabilityMixingRollMachine
{
    public class AvailabilityMixingRollMachineViewModel
    {
        [Display(Name = @"Start Date")]
        [Required(ErrorMessage = @"Please input data for this required field.")]
        public string StartDate { get; set; }
        [Required(ErrorMessage = @"Please input data for this required field.")]
        [Display(Name = @"End Date")]
        public string EndDate { get; set; }
    }
}