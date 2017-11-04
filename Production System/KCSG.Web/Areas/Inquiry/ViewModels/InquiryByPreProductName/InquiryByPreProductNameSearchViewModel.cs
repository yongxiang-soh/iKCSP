using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace KCSG.Web.Areas.Inquiry.ViewModels.InquiryByPreProductName
{
    public class InquiryByPreProductNameSearchViewModel
    {
        [Display(Name = "")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{DD/MM/YYYY hh:mm:ss}", ApplyFormatInEditMode = false)]


        public DateTime DateTime { get; set; }

        [Required(ErrorMessage = "This field must be inputted!")]
        [Display(Name = @"Pre-product Code")]
        public string PreProductCode { get; set; }

        [StringLength(16)]
        [Display(Name = @"Pre-product Name")]
        public string PreProductName { get; set; }


        //[StringLength(4)]
        [Display(Name = @"Bailment Class")]
        public string BailmentClass { get; set; }

        //[StringLength(4)]
        [Display(Name = @"Grand Total")]
        public double GrandTotal { get; set; }

        public Constants.PrintOptionsPreProduct PrintOptions { get; set; }


        public Grid Grid { get; set; }
        //public Grid GridSelected { get; set; }
    }
}