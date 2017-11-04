using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.Inquiry.ViewModels.InquiryByProductCode
{
    public class InquiryByProductCodeSearchViewModel
    {
        [Display(Name = "")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{DD/MM/YYYY hh:mm:ss}", ApplyFormatInEditMode = false)]


        public DateTime DateTime { get; set; }

        [Required(ErrorMessage = "This field must be inputted!")]
        [Display(Name = @"Product Code")]
        public string ProductCode { get; set; }

        
        [Display(Name = @"Product Name")]
        public string ProductName { get; set; }

        [Display(Name = @"End User Code")]
        public string EndUserCode { get; set; }

        [Display(Name = @"End User Name")]
        public string EndUserName { get; set; }
        public Grid Grid { get; set; }
    }
}