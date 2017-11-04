using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using KCSG.Web.Validators;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.RetrievalOfSupplierPallet
{
    public class RetrievalOfSupplierPalletViewModel
    {
        /// <summary>
        /// Code of supplier (obtained from Supplier selection box)
        /// </summary>
        [DisplayName("Supplier Code")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string SupplierCode { get; set; }

        /// <summary>
        /// Name of supplier.
        /// </summary>
          [DisplayName("Supplier Name")]
        public string SupplierName { get; set; }

        /// <summary>
        /// Quantity which is requested.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [GreaterThan(0, ErrorMessageResourceType = typeof(MaterialResource), ErrorMessageResourceName = "MSG42")]
        //[StringLength(3)]
        [DisplayName("Requested Retrieval Quantity")]
        public double RequestedRetrievalQuantity { get; set; }


        [DisplayName("Possible Retrieval Quantity")]
        public double? PossibleRetrievalQuantity { get; set; }
        [DisplayName("Possible Retrieval Quantity of Pallet")]
        public double? PossibleRetrievalQuantityOfPallet { get; set; }
        
    }
}