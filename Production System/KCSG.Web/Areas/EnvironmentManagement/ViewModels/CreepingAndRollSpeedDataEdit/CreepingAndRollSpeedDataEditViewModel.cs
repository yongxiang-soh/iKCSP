using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Windows.Threading;
using KCSG.Core.Constants;
using Resources;

namespace KCSG.Web.Areas.EnvironmentManagement.ViewModels.CreepingAndRollSpeedDataEdit
{
    public class CreepingAndRollSpeedDataEditViewModel
    {
        [Display(Name = @"Mode")]
        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        public Constants.EnvMode EnvMode { get; set; }

        [Display(Name = @"Date")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string EnvironmentDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string Machine { get; set; }


        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string Time { get; set; }

        [Display(Name = @"Left Creeping")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? LeftCreeping { get; set; }

        [Display(Name = @"Right Creeping")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? RightCreeping { get; set; }

        [Display(Name = @"Roll Speed")]
         [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? RollSpeed { get; set; }

        
    }
}