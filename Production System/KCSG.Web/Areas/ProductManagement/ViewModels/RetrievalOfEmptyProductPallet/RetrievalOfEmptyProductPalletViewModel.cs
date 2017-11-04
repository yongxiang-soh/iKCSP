using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.RetrievalOfEmptyProductPallet
{
    public class RetrievalOfEmptyProductPalletViewModel
    {
        [Display(Name = @"Possible Retrieval Quantity")]
        public double? PossibleRetrievalQuantity { get; set; }

        [Display(Name = @"Requested Retrieval Quantity")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double RequestedRetrievalQuantity { get; set; }
    }
}