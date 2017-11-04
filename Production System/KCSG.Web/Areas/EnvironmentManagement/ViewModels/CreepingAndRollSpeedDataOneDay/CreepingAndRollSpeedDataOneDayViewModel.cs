using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;

namespace KCSG.Web.Areas.EnvironmentManagement.ViewModels.CreepingAndRollSpeedDataOneDay
{
    public class CreepingAndRollSpeedDataOneDayViewModel
    {
        [Display(Name = @"Mode")]
        public Constants.EnvMode EnvMode { get; set; }
        [Display(Name = @"Date")]
        public string EnvironmentDate { get; set; }
        [Display(Name = @"Machine")]
        public string Machine { get; set; }
    }
}