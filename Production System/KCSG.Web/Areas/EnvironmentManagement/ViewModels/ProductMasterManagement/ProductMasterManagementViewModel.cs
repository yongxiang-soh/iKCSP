using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Windows.Threading;
using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.EnvironmentManagement.ViewModels.ProductMasterManagement
{
    public class ProductMasterManagementViewModel
    {
        public Grid Grid { get; set; }

        [Required(ErrorMessageResourceType=typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        public string Location { get; set; }
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public Constants.EnvMode Mode { get; set; }

        [Display(Name = @"Product Name")]
        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        public string ProductName { get; set; }

        [Display(Name = @"New Product Name")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string NewProductName { get; set; }

        [Display(Name = @"USL")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? USLMean { get; set; }

        [Display(Name = @"UCL")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? UCLMean { get; set; }

        [Display(Name = @"LSL")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? LSLMean { get; set; }

        [Display(Name = @"LCL")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? LCLMean { get; set; }

        [Display(Name = @"USL")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? USLRange { get; set; }

        [Display(Name = @"LSL")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? LSLRange { get; set; }

        [Display(Name = @"No. of Lot")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public int? NoOFLot { get; set; }

        public bool isCreate { get; set; }

        public string F85_Code { get; set; }

    }
}