using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace KCSG.Web.Areas.Inquiry.ViewModels.InquiryByExternalPreProductName
{
    public class InquiryByExternalPreProductNameSearchViewModel
    {
        [Display(Name = "")]
        //[DataType(DataType.DateTime)]
        //[DisplayFormat(DataFormatString = "{DD/MM/YYYY hh:mm:ss}", ApplyFormatInEditMode = false)]


        public DateTime DateTime { get; set; }

        [Required(ErrorMessage = "This field must be inputted!")]
        [Display(Name = @"External Pre-product Code")]
        public string ExtPreProductCode { get; set; }

        [StringLength(16)]
        [Display(Name = @"External Pre-product Name")]
        public string ExtPreProductName { get; set; }
        

        //[StringLength(4)]
        [Display(Name = @"Grand Total")]
        public double GrandTotal { get; set; }

        public Constants.PrintOptionsPreProduct PrintOptions { get; set; }


        public Grid Grid { get; set; }
        //public Grid GridSelected { get; set; }
    }
}