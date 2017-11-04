using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;
using System.Web.Services.Description;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.AcceptanceOfMaterial
{
    public class AcceptanceOfMaterialViewModel
    {
        /// <summary>
        /// P.O column in TX_30
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(7)]
        [Display(Name = @"P.O.No.")]
        public string PNo { get; set; }
        
        /// <summary>
        /// Partial delivery number.
        /// </summary>
       [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
       [StringLength(2)]
        public string PartialDelivery { get; set; }

        /// <summary>
        /// Grid which is for being displayed on Html page.
        /// This is shouldn't be bound by client request.
        /// </summary>
        [BindNever]
        public Grid Grid { get; set; }
    }
}