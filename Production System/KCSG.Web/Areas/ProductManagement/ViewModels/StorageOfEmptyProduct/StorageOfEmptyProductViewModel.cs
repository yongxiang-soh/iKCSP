using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.StorageOfEmptyProduct
{
    public class StorageOfEmptyProductViewModel
    {
        [Display(Name = @"Pallet Load Number")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public int PalletNumber { get; set; }
    }
}