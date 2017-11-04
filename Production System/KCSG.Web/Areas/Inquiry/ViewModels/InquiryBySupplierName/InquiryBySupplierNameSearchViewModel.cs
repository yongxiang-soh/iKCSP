using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Drawing.Charts;
namespace KCSG.Web.Areas.Inquiry.ViewModels.InquiryBySupplierName
{
    public class InquiryBySupplierNameSearchViewModel
    {
        [Display(Name = "")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{DD/MM/YYYY HH:mm:ss}", ApplyFormatInEditMode = false)]
        public DateTime DateTime { get; set; }

        [Required(ErrorMessage = "This field must be inputted!")]
        [Display(Name = @"Supplier Code")]
        public string SupplierCode { get; set; }

        //[StringLength(16)]
        [Display(Name = @"Supplier Name")]
        public string SupplierName { get; set; }


        //[StringLength(4)]
        [Display(Name = @"Grand Total")]
        public double GrandTotal { get; set; }

        public Constants.PrintOptions PrintOptions { get; set; }


        public Grid Grid { get; set; }
        //public Grid GridSelected { get; set; }
    }
}