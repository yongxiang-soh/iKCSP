using Resources;
using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.PreProductManagement.ViewModels
{
    public class StorageOfPreProductUc6ViewModel
    {
        /// <summary>
        /// Status of storage of pre-product.
        /// </summary>
        [Required]
        public string Status { get; set; }

        /// <summary>
        /// Pre-product code.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string PreProductCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string ContainerCode { get; set; }
    }
}