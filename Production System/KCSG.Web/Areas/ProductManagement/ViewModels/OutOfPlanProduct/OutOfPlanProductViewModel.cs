using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Wordprocessing;
using KCSG.Web.Validators;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.OutOfPlanProduct
{
    public class OutOfPlanProductViewModel
    {
        public Grid GridOutOfPlanProduct { get; set; }

        [Display(Name = @"Product Code")]
        //[Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Remote("CheckDuplicate", "OutOfPlanProduct", HttpMethod = "OPTIONS", ErrorMessageResourceType = typeof(ProductManagementResources), ErrorMessageResourceName = "MSG41", AdditionalFields = "PrePdtLotNo,IsCreate")]
         
        [StringLength(12)]
        public string ProductCode { get; set; }

        [Display(Name = @"Product Name")]
        [StringLength(15)]
        public string ProductName { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Pre-Product Lot No")]
        [Remote("CheckDuplicate", "OutOfPlanProduct", HttpMethod = "OPTIONS", ErrorMessageResourceType = typeof(ProductManagementResources), ErrorMessageResourceName = "MSG41", AdditionalFields = "ProductCode,IsCreate")]
       
        [StringLength(10)]
        public string PrePdtLotNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Product Lot No")]
        [StringLength(10)]
        public string ProductLotNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Pack Quantity")]
        public double? PackQuantity { get; set; }

        [Display(Name = @"Fraction")]
        public double? Fraction { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [DisplayFormat(DataFormatString = "{DD/MM/YYYY}", ApplyFormatInEditMode = false)]
        [Display(Name = @"Tableting End Date")]
        public string F58_TbtEndDateString { get; set; }

        //[Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        //[Display(Name = @"Tableting End Date")]
        //public string TabletingEndDate { get; set; }
        public bool IsCreate { get; set; }

    }
}