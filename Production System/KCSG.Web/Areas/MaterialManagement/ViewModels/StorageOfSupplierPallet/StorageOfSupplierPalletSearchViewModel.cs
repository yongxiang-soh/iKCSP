using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.StorageOfSupplierPallet
{
    public class StorageOfSupplierPalletSearchViewModel
    {
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name="Supplier Code")]
        public string SupplierCode { get; set; }

        [Display(Name = "Max Pallet")]

        public int MaxPallet { get; set; }

        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }


        [Display(Name = "Shelf No")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string ShelfNo { get; set; }

        [Display(Name = "Max Pallet")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public int MaxPalletDetail { get; set; }

        [Display(Name = "Stacked Pallet")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public int StackedPallet { get; set; }

        [Display(Name = "Increment of Pallet")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public int IncrementOfPallet { get; set; }

        public Grid Grid { get; set; }
    }
}