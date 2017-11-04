using System.ComponentModel.DataAnnotations;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.MaterialPostReceptionInput
{
    public class FindPostReceptionInputMaterialViewModel
    {
        #region Properties

        /// <summary>
        /// Material code.
        /// </summary>
        [Required]
        public string MaterialCode { get; set; }

        /// <summary>
        /// Pallet No.
        /// </summary>
        [Required]
        public string PalletNo { get; set; }

        #endregion
    }
}