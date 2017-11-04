using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels
{
    public class FontStorageOfSupplierPalletViewModel
    {
        [DisplayName("Supplier Code")]
        [Required(ErrorMessageResourceName = "MSG2", ErrorMessageResourceType = typeof(MessageResource))]
        public string SupplierCode { get; set; }

        [DisplayName("Supplier Name")]
        public string SupplierName { get; set; }

        [DisplayName("Max Pallet")]
        public double? MaxPallet { get; set; }

        [DisplayName("Storage Quantity")]
        [Required(ErrorMessageResourceName = "MSG2", ErrorMessageResourceType = typeof(MessageResource))]
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(InQuiryResource),
            ErrorMessageResourceName = "TCFC034F01")]
        public int StorageQuantity { get; set; }
    }
}