using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.Inquiry.ViewModels.InquiryByPreProductCode
{
    public class InquiryByPreProductCodeSearchViewModel
    {
        [Display(Name = "")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{DD/MM/YYYY hh:mm:ss}", ApplyFormatInEditMode = false)]


        public DateTime DateTime { get; set; }

        [Required(ErrorMessage = "This field must be inputted!")]
        [Display(Name = @"Pre-product Code")]
        public string PreproductCode { get; set; }

        [Required(ErrorMessage = "This field must be inputted!")]
        [Display(Name = @"Lot No")]
        public string LotNo { get; set; }

        [Display(Name = @"Grand Total")]
        public double Total { get; set; }
        
        [Display(Name = @"Pre-product Name")]
        public string PreProductName { get; set; }

        public Grid Grid { get; set; }
    }
}