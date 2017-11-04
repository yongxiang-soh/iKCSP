using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.ProductShippingCommand
{
    public class ReplyThirdCommunicationViewModel
    {
        /// <summary>
        /// Product code
        /// </summary>
        [Required]
        public string ProductCode { get; set; }
    }
}