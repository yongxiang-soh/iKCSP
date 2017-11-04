using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.ForcedRetrievalOfRejectedMaterial
{
    public class AssignRejectedMaterialViewModel
    {
        /// <summary>
        /// Material code.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [DisplayName("Material Code")]
        public string MaterialCode { get; set; }

        /// <summary>
        /// Name of material.
        /// </summary>
         [DisplayName("Material Name")]
        public string MaterialName { get; set; }

        /// <summary>
        /// Product order number.
        /// </summary>
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [DisplayName("P.O. No.")]
        [StringLength(15)]
        public string ProductOrderNumber { get; set; }

        /// <summary>
        /// Partial delivery code.
        /// </summary>
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [DisplayName("Partial Delivery")]
        [StringLength(2)]
        public string PartialDelivery { get; set; }

        /// <summary>
        /// Amount of assigned quantity.
        /// </summary>
        //[Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Assigned Quantity")]
        public double AssignedQuantity { get; set; }

        [Display(Name = @"Assigned")]
        public double Assigned { get; set; }
    }
}