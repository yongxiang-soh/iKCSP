using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.StorageOfSupplementaryMaterial
{
    public class StorageOfSupplementMaterialSearchViewModel
    
    {
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Supplementary Material Code")]
        public string SubMaterialCode { get; set; }

        public Grid GridSupplementaryMaterial { get; set; }
    }
}