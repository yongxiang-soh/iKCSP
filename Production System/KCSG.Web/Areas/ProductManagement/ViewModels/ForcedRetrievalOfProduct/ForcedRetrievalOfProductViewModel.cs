using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Packaging;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.ForcedRetrievalOfProduct
{
    public class ForcedRetrievalOfProductViewModel
    {
        [Display(Name = @"Product Code")]
        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        [StringLength(12)]
        public string ProductCode { get; set; }

        [Display(Name = @"Product Name")]
        [StringLength(15)]
        public string ProductName { get; set; }

        [Display(Name = @"Product Lot No")]
        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        [StringLength(10)]
        public string ProductLotNo { get; set; }

        [Display(Name = @"Requested Retrieval Quantity")]
        [Required(ErrorMessageResourceType = typeof(MessageResource),ErrorMessageResourceName = "MSG2")]
        public double RequestedRetrievalQuantity { get; set; }

        [Display(Name = @"Tally")]
        public double Tally { get; set; }

        public Grid Grid { get; set; }

        [Display(Name = @"Product Code")]
        public string ProductCode1 { get; set; }

        public string ProductCode2 { get; set; }

        public string ProductCode3 { get; set; }

        public string ProductCode4 { get; set; }

        public string ProductCode5 { get; set; }

        [Display(Name = @"Product Name")]
        public string ProductName1 { get; set; }

        public string ProductName2 { get; set; }

        public string ProductName3 { get; set; }

        public string ProductName4 { get; set; }

        public string ProductName5 { get; set; }

        [Display(Name = @"Lot No")]
        public string LotNo1 { get; set; }

        public string LotNo2 { get; set; }

        public string LotNo3 { get; set; }

        public string LotNo4 { get; set; }

        public string LotNo5 { get; set; }

        [Display(Name = @"Quantity")]
        public double Quantity1 { get; set; }

        public double Quantity2 { get; set; }

        public double Quantity3 { get; set; }

        public double Quantity4 { get; set; }

        public double Quantity5 { get; set; }

        [Display(Name = @"Cer. Flag")]
        public string CerFlag1 { get; set; }

        public string CerFlag2 { get; set; }

        public string CerFlag3 { get; set; }

        public string CerFlag4 { get; set; }

        public string CerFlag5 { get; set; }

        
    }
}