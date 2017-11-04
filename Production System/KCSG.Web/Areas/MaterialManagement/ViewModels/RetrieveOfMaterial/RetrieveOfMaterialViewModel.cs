using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.RetrieveOfMaterial
{
    public class RetrieveOfMaterialViewModel
    {
        /// <summary>
        /// Material code.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [DisplayName("Material Code")]
        public string MaterialCode { get; set; }

        /// <summary>
        /// Name of material which should be retrieved.
        /// </summary>
         [DisplayName("Material Name")]
        public string MaterialName { get; set; }

        /// <summary>
        /// Communication measurement system.
        /// </summary>
        [Display (Name = "Communication with measurement system")]
        public Constants.CommWthMeasureSys CommWthMeasureSys { get; set; }

        /// <summary>
        /// Requested retrieval quantity.
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "Requested Retrieval  Quantity")]
        public double RequestedRetrievalQuantity { get; set; }

        /// <summary>
        /// Tally.
        /// </summary>
        public double Tally { get; set; }

        public Grid PalletsGrid { get; set; }

        public string PalletNo { get; set; }
    }
}