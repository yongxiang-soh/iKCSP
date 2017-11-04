using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.EnvironmentManagement.ViewModels
{
    public class LotDataSamplingModel
    {
        public Constants.EnvMode Mode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.EnvironmentResource),ErrorMessageResourceName = "MSG12")]
        [DisplayName(@"Lot No.")]
        public string LotNo { get; set; }

         [Required(ErrorMessageResourceType = typeof(Resources.EnvironmentResource), ErrorMessageResourceName = "MSG18")]
        public string Date { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.EnvironmentResource), ErrorMessageResourceName = "MSG13")]
        public string Product { get; set; }

        [DisplayName(@"New Lot Number")]
        public bool NewLotNumber { get; set; }

         [DisplayName("High")]
        public string HighTemp { get; set; }

        [DisplayName("Low")]
        public string LowTemp { get; set; }

        [DisplayName("Range")]
        public string RangeTemp { get; set; }

        [DisplayName("Mean")]
        public string MeanTemp { get; set; }

        [DisplayName("Sigma")]
        public string SigmaTemp { get; set; }

        //[Remote("ValidateTime", "LotDataSampling", ErrorMessageResourceType = typeof(Resources.EnvironmentResource), ErrorMessageResourceName = "MSG16",HttpMethod = "Options")]
        [Required(ErrorMessage = @"No input from the time or temperature value!")]
        public TimeSpan Time { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.EnvironmentResource), ErrorMessageResourceName = "MSG14")]
        public double? Temperature { get; set; }

        public Grid Grid { get; set; }
        public TimeSpan Time1 { get; set; }
        public TimeSpan Time2 { get; set; }
        public TimeSpan Time3 { get; set; }
        public double Temperature3 { get; set; }
        public double Temperature1 { get; set; }
        public double Temperature2 { get; set; }
    }
}