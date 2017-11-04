using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using Resources;

namespace KCSG.Web.Areas.EnvironmentManagement.ViewModels
{
    public class SearchCriteriaModel
    {
        public Constants.EnvMode Mode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Start Date")]
        public string StartDate { get; set; }

         [Required(ErrorMessageResourceType = typeof(Resources.MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"End Date")]
        public string EndDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Location")]
        public string Location { get; set; }

        public SelectList ListLocation { get; set; }

        [Display(Name = @"Environment Date")]
        public string EnvironmentDate { get; set; }
    }
}