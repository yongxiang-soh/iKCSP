using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KCSG.Core.Constants;
using Resources;

namespace KCSG.Web.Areas.EnvironmentManagement.ViewModels.EnvironmentDataEdit
{
    public class EnvironmentDataEditViewModel
    {
        [Display(Name = @"Mode")]
        public Constants.EnvMode EnvMode { get; set; }

        [Display(Name = @"Date")]
        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        public string EnvironmentDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string Location { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string Time { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? Temperature { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? Humidity { get; set; }


    }
}