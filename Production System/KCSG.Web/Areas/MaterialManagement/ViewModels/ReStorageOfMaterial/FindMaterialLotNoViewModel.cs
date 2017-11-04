using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.ReStorageOfMaterial
{
    public class FindMaterialLotNoViewModel
    {
        [Required]
        public string MaterialCode { get; set; }

        [Required]
        public string PalletNo { get; set; }
    }
}