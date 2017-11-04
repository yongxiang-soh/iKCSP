using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace KCSG.Web.Areas.KneadingCommand.ViewModels.InputOfKneadingCommand
{
    
    public class InputOfKneadingCommandSearchViewModel
    {
        [Display(Name = @"Line")]
        public Constants.KndLine KndLine { get; set; }

        [Display(Name = @"Within")]
        [Required(ErrorMessage = "This field must be inputted!")]
        [Range(1, Int32.MaxValue, ErrorMessage = "The days cannot be null or less than 0")]
        public int? Within { get; set; }


        public Grid Grid { get; set; }
        public Grid GridSelected { get; set; }
    }

}