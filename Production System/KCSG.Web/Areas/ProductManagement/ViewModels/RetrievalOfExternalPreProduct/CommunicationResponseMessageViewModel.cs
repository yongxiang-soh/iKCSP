using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.RetrievalOfExternalPreProduct
{
    public class CommunicationResponseMessageViewModel
    {
        [Required]
        public string PreProductCode { get; set; }

        [Required]
        public string KneadingCommandNo { get; set; }

        [Required]
        public string PreProductLotNo { get; set; }
    }
}