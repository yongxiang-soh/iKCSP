using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.RetrievalOfWarehousePallet
{
    public class RetrievalOfWarehousePalletViewModel
    {
        [Display(Name = @"Possible Retrieval Quantity")]
        public int PossibleRetrievalQuantity { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Requested Retrieval Quantity")]
        public int? RequestRetrievalQuantity { get; set; }
    }
}