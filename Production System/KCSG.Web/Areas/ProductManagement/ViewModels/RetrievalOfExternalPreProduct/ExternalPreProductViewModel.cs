using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.RetrievalOfExternalPreProduct
{
    public class ExternalPreProductViewModel
    {
        /// <summary>
        /// Line of pre-product.
        /// </summary>
        [Required]
        public string Line { get; set; }

        /// <summary>
        /// Command no.
        /// </summary>
        [Required]
        public string F41_KndCmdNo { get; set; }

        /// <summary>
        /// Pre-product lot no.
        /// </summary>
        [Required]
        public string F41_PrePdtLotNo { get; set; }

        /// <summary>
        /// Pre-product code.
        /// </summary>
        public string F41_PreProductCode { get; set; }
    }
}