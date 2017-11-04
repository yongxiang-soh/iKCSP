using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;

namespace KCSG.Web.Areas.EnvironmentManagement.ViewModels.LotDataCleanup
{
    public class LotDataCleanupViewModel
    {
        [Display(Name = @"Old Cut-off Date")]
        public string OldCutOffDate { get; set; }
        [Display(Name = @"Time")]
        public string OldCutOfTime { get; set; }
        [Display(Name = @"New Cut-off Date")]
        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        public string NewCutOfDate { get; set; }
        [Display(Name = @"Time")]
        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        public string NewCutOfTime { get; set; }
        [Display(Name = @"Lots")]
        public int? Lot1 { get; set; }
        [Display(Name = @"Lots")]
        public int? Lot2 { get; set; }
    }
}