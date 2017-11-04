using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.StorageOfSupplementaryMaterial
{
    public class StorageOfSupplementaryMaterialViewModel
    {
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Supplementary Material Code")]
        public string F15_SubMaterialCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = @"Supplementary Material Name")]
        public string F15_MaterialDsp { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name=@"Unit")]
        public string F15_Unit { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name=@"Pack Unit")]
        public double F15_PackingUnit { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name=@"Pack Quantity")]
        public double PackQuantity { get; set; }

        [Display(Name = @"Fraction")]
        public double Fraction { get; set; }

        //[Range(0, 999999, ErrorMessage = @"Value is out of range!")]
        [Display(Name = @"Add Quantity")]
        public double AddQuantity { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]        
        //[Range(0, 999999, ErrorMessage = @"Value is out of range!")]
        [Display(Name = @"Inventory Quantity")]
        public int InventoryQuantity { get; set; }

        [Display(Name = @"Comment")]
        [MaxLength(40, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG52")]
        public string Comment { get; set; }


        public bool IsStore { get; set; }
    }
}