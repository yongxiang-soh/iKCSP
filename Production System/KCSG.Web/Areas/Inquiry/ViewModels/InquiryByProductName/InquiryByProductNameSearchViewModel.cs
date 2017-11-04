using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace KCSG.Web.Areas.Inquiry.ViewModels.InquiryByProductName
{
    public class InquiryByProductNameSearchViewModel
    {
        [Display(Name = "")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{DD/MM/YYYY hh:mm:ss}", ApplyFormatInEditMode = false)]


        public DateTime DateTime { get; set; }

        [Required(ErrorMessage = "This field must be inputted!")]
        [Display(Name = @"Product Code")]
        public string ProductCode { get; set; }

        [StringLength(16)]
        [Display(Name = @"Product Name")]
        public string ProductName { get; set; }


        //[StringLength(4)]
        [Display(Name = @"Grand Total")]
        public double GrandTotal { get; set; }

        [Display(Name = @"Delivery Total")]
        public double DeliveryTotal { get; set; }

        [Display(Name = @"Certified Total")]
        public double CertifiedTotal { get; set; }

        [Display(Name = @"Non-Certified Total")]
        public double NonCertifiedTotal { get; set; }

        public Constants.PrintOptionsProduct PrintOptions { get; set; }


        public Grid Grid { get; set; }
        //public Grid GridSelected { get; set; }
    }
}