using Resources;
using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.RetrievalOfExternalPreProduct
{
    public class FindTableListingLineViewModel
    {
        /// <summary>
        /// Table listing line which is used for searching device.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string TableListingLine { get; set; }
    }
}