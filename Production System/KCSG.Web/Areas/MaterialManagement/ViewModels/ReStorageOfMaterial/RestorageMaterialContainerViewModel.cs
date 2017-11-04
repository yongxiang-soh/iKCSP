using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.ReStorageOfMaterial
{
    public class RestorageMaterialContainerViewModel
    {
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "Msg1")]
        [StringLength(3)]
        [Display(Name = @"Pallet No.")]
        public string PalletNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "Msg1")]
        [StringLength(12, ErrorMessage = "Material Code cannot be longer than 12 characters.")]
        [Display(Name = @"Material Code")]
        public string MaterialCode { get; set; }

        [StringLength(16, ErrorMessage = "Material Name cannot be longer than 16 characters.")]
        [Display(Name = @"Material Name")]
        public string MaterialName { get; set; }

        /// <summary>
        /// List of storage materials.
        /// </summary>
        public IList<RestorageMaterialViewModel> RestorageMaterials { get; set; }
    }
}