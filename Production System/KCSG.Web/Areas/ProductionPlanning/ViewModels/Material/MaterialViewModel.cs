using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Resources;
using System.Web.Mvc;
using System.Web.Services.Description;
using DocumentFormat.OpenXml;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.Material
{
    public class MaterialViewModel
    {
        [Key, Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        
        [Display(Name = "Material Code")]
        [StringLength(12,ErrorMessage = "Material Code cannot be longer than 12 characters.")]
        [Remote("CheckExistCode", "Material", ErrorMessage = "The same record exists in database", HttpMethod = "Options", AdditionalFields = "IsCreate")]
        public string F01_MaterialCode { get; set; }

        [StringLength(5, ErrorMessage = "Supplier Code cannot be longer than 5 characters.")]
        [Display(Name = "Supplier Code")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string F01_SupplierCode { get; set; }

        [StringLength(16, ErrorMessage = "Material Name cannot be longer than 16 characters.")]
        [Display(Name = "Material Name")]
        public string F01_MaterialDsp { get; set; }
       // [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "EMP")]
        [Range(0,999999999, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG5")]
        public double? F01_EMP { get; set; }

        [StringLength(1, ErrorMessage = "Modify cannot be longer than 1 characters.")]
        [Display(Name = "Modify")]
        public string F01_ModifyClass { get; set; }

        //[StringLength(3, ErrorMessage = "Point cannot be longer than 3 characters.")]
        //public string F01_Point { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(1, ErrorMessage = "Unit cannot be longer than 1 characters.")]
        [RegularExpression("['K','P']",ErrorMessageResourceType= typeof(MessageResource), ErrorMessageResourceName = "MSG11")]
        [Display(Name = "Unit")]
        public string F01_Unit { get; set; }

       
        [Display(Name = "Bailment")]
        [StringLength(1, ErrorMessage = "Bailment cannot be longer than 1 characters.")]
        public string F01_EntrustedClass { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "Packing Unit")]
        [Range(0, 999999999, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG5")]
        public double? F01_PackingUnit { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "Price")]
        [Range(0.00,999999999.99, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG5")]
        public double? F01_Price { get; set; }

        [StringLength(4, ErrorMessage = "Department cannot be longer than 4 characters.")]
        [Display(Name = "Department")]
        public string F01_Department { get; set; }

    
        [DisplayName("Liquid")]
        [StringLength(1, ErrorMessage = "Liquid cannot be longer than 1 characters.")]
        public string F01_LiquidClass { get; set; }

        [StringLength(1, ErrorMessage = "Factory cannot be longer than 1 characters.")]
        [Display(Name = "Factory")]
        public string F01_FactoryClass { get; set; }

        [StringLength(3, ErrorMessage = "Factor cannot be longer than 3 characters.")]
        [Display(Name = "Factor")]
        public string F01_Point { get; set; }

       
        [DisplayName("Retrieval Location")]
        [StringLength(1, ErrorMessage = "Retrieval Location cannot be longer than 1 characters.")]
        public string F01_RtrPosCls { get; set; }

       
        [DisplayName("Weighing Machine Comms")]
        [StringLength(1, ErrorMessage = "Weighing Machine Comms cannot be longer than 1 characters.")]
        public string F01_MsrMacSndFlg { get; set; }

        [StringLength(1, ErrorMessage = "State cannot be longer than 1 characters.")]
        [Display(Name = "State")]
        public string F01_State { get; set; }

        [StringLength(1, ErrorMessage = "Color cannot be longer than 1 characters.")]
        [Display(Name = "Color")]
        public string F01_Color { get; set; }

        public DateTime F01_AddDate { get; set; }
        public DateTime F01_UpdateDate { get; set; }
        public int F01_UpdateCount { get; set; }
        public bool IsCreate { get; set; }
    }
}