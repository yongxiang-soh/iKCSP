using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Windows.Threading;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.EnvironmentManagement.ViewModels.ControlLimitEdit
{
    public class ControlLimitEditViewModel
    {
        public Grid Grid { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string Location { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string From { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string To { get; set; }

        [Display(Name = @"UCL")]
        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        public double? TempUCL { get; set; }

        [Display(Name = @"Cp")]
        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        public double? TempCp { get; set; }

        [Display(Name = @"LCL")]
        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        public double? TempLCL { get; set; }

        [Display(Name = @"Cpk")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? TempCpk { get; set; }

        [Display(Name = @"Mean")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? TempMean { get; set; }

        [Display(Name = @"Range")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? TempRange { get; set; }

        [Display(Name = @"Sigma")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? TempSigma { get; set; }



        [Display(Name = @"UCL")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? HumUCL { get; set; }

        [Display(Name = @"Cp")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? HumCp { get; set; }

        [Display(Name = @"LCL")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? HumLCL { get; set; }

        [Display(Name = @"Cpk")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? HumCpk { get; set; }

        [Display(Name = @"Mean")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? HumMean { get; set; }

        [Display(Name = @"Range")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? HumRange { get; set; }

        [Display(Name = @"Sigma")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? HumSigma { get; set; }
    }
}