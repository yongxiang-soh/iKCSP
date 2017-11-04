using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.StorageOfExternalPreProduct
{
    public class StorageOfExternalPreProductViewModel
    {
        [Display(Name = @"Pre-product Code")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(12)]
        public string PreProductCode { get; set; }

        [Display(Name = @"Pre-product Name")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string PreProductName { get; set; }

        [Display(Name = @"Lot No")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(10)]
        public string LotNo { get; set; }

        [Display(Name = @"Pallet No")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(3)]
        public string PalletNo { get; set; }

        [Display(Name = @"Quantity")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double Quantity { get; set; }

        public string Kndcmdno { get; set; }
    }
}