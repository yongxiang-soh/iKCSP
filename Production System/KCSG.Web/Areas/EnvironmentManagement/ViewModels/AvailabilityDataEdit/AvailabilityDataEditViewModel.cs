using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KCSG.Core.Constants;
using Resources;

namespace KCSG.Web.Areas.EnvironmentManagement.ViewModels.AvailabilityDataEdit
{
    public class AvailabilityDataEditViewModel
    {
       [Display(Name = @"Mode")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public Constants.EnvMode EnvMode { get; set; }
        [Display(Name = @"Date")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")] 
        public string EnvironmentDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")] 
        public string Machine { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]    
        public string Time { get; set; }
        public Constants.EnvironmentStatus EnvironmentStatus { get; set; }

    }
}