using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.SubMaterialMasters
{
    public class SubMaterialViewModel
    {
        [Key, Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(12)]
        [Display(Name = "Sup. Material Code")]
        [Remote("CheckExistCode", "SubMaterialMaster", ErrorMessage = "The same record exists in database", HttpMethod = "Options", AdditionalFields = "IsCreate")]
        
        public string SubMaterialCode { get; set; }

        [StringLength(5, ErrorMessage = "Supplier Code cannot be longer than 5 characters.")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "Supplier Code")]
        public string SupplierCode { get; set; }

        [StringLength(16)]
        [Display(Name = "Sup. Material Name")]
        public string SubMaterialDsp { get; set; }
        [Display(Name = "EMP")]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG5")]
        public double? EMP { get; set; }

        [StringLength(1, ErrorMessage = "ModifyClass cannot be longer than 1 characters.")]
        [Display(Name = "Modify")]
        public string ModifyClass { get; set; }

        [StringLength(3, ErrorMessage = "Point cannot be longer than 3 characters.")]
        public string Point { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(1, ErrorMessage = "Unit cannot be longer than 1 characters.")]
        [Display(Name = "Unit")]
        public string Unit { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "Packing Unit")]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG5")]
        public double? PackingUnit { get; set; }

        [Display(Name = "Price")]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG5")]
        public double? Price { get; set; }

        [StringLength(4, ErrorMessage = "Department cannot be longer than 4 characters.")]
        [Display(Name = "Department")]
        public string Department { get; set; }

        [StringLength(3)]
        [Display(Name = "Factor")]
        public string FactoryClass { get; set; }

        [StringLength(1, ErrorMessage = "State cannot be longer than 1 characters.")]
        [Display(Name = "State")]
        public string State { get; set; }

        [Display(Name = "Bailment")]
        public int Baliment { get; set; }


        public int UpdateCount { get; set; }
        public bool IsCreate { get; set; }
    }
}