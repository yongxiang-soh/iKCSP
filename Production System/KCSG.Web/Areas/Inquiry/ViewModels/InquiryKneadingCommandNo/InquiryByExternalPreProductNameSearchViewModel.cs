using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace KCSG.Web.Areas.Inquiry.ViewModels.InquiryKneadingCommandNo
{
    public class KneadingCommandControlViewModel
    {
        /// <summary>
        /// Kneading line
        /// </summary>
        public Constants.KndLine KneadingLine { get; set; }
        

        [Display(Name = @"Kneading Command No")]
        public string KneadingNo { get; set; }
        [Required(ErrorMessage = "This field must be inputted!")]
        [Display(Name = @"Pre-Product Code")]
        public string PreProductCode { get; set; }
        [Display(Name = @"Pre-Product Name")]
        public string PreProductName { get; set; }

        [Display(Name = @"Pre-Product LotNo")]
        public string PreProLotNo { get; set; }

        /// <summary>
        /// Result grid which is used for display filtering results.
        /// This property shouldn't be bound by client request.
        /// </summary>
        [BindNever]
        public Grid Grid { get; set; }
    }
    public class KneadingCommandLineViewModel
    {
        /// <summary>
        /// Kneading line
        /// </summary>
        public Constants.KndLine KneadingLine { get; set; }

        [Required(ErrorMessage = "This field must be inputted!")]
        [Display(Name = @"Kneading Command No")]
        public string KneadingNo { get; set; }
        
        [Display(Name = @"Pre-Product Code")]
        public string PreProductCode { get; set; }
        [Display(Name = @"Pre-Product Name")]
        public string PreProductName { get; set; }

        [Display(Name = @"Pre-Product LotNo")]
        public string PreProLotNo { get; set; }

        /// <summary>
        /// Result grid which is used for display filtering results.
        /// This property shouldn't be bound by client request.
        /// </summary>
        [BindNever]
        public Grid Grid { get; set; }
    }
}

