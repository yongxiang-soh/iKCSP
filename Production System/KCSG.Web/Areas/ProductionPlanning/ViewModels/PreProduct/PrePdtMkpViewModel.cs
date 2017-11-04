using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.PreProduct
{
    public  class PrePdtMkpViewModel
    {
        [Key, Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(12)]
        [Display(Name = "Pre-Product Code")]
        public string F02_PreProductCode { get; set; }

        [StringLength(15)]
        [Display(Name = "Pre-Product Name")]
        public string F03_PreProductName { get; set; }

        [Key, Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(12)]
        [Display(Name = "Material Code")]
        [Remote("CheckExistMaterialCode", "PrePdtMkp", ErrorMessage = "The same record exists in database", HttpMethod = "Options", AdditionalFields = "F02_PreProductCode,IsCreate")]
        public string F02_MaterialCode { get; set; }

        //[StringLength(15)]
        [Display(Name = "Material Name")]
        public string F01_MaterialName { get; set; }

        [Display(Name = "Liquid Class")]
        public string F01_LiquidClass { get; set; }

        [StringLength(4)]
        [Display(Name = "Load Position")]
        public string F02_LoadPosition { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        //[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Charged Quantity 3F")]
        [Range(0.00, 100000000.00f, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Remote("CheckAmount", "PrePdtMkp", ErrorMessage = "A non-zero quantity must be charged from either 3F or 4F !", HttpMethod = "Options", AdditionalFields = "F02_4FLayinAmount")]
      
        public double? F02_3FLayinAmount { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        //[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        [Display(Name = "Charged Quantity 4F")]
        [Range(0.00, 100000000.00f, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Remote("CheckAmount", "PrePdtMkp", ErrorMessage = "A non-zero quantity must be charged from either 3F or 4F !", HttpMethod = "Options", AdditionalFields = "F02_3FLayinAmount")]
      
        public double? F02_4FLayinAmount { get; set; }


        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Range(1, 4, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG15")]
        [Display(Name = "Charge Priority")]
        [Remote("CheckPriority", "PrePdtMkp", ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG14", HttpMethod = "Options", AdditionalFields = "F02_Addtive")]
       
        public int? F02_LayinPriority { get; set; }

        

        [Key, Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(2)]
        [Display(Name = "Charge Seq No")]
        public string F02_ThrawSeqNo { get; set; }

        [StringLength(1)]
        [Display(Name = "Pot Seq No")]
        [Range(1, 8, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG12")]
       // [Remote("PotSeqNo", "PrePdtMkp", ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG21", HttpMethod = "Options", AdditionalFields = "F02_PreProductCode,IsCreate")]
       
        public string F02_PotSeqNo { get; set; }
        //[Remote("MsrSeqNo", "PrePdtMkp", ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG22", HttpMethod = "Options", AdditionalFields = "F02_PotSeqNo")]
        [StringLength(1)]
        [Display(Name = "Weigh Seq No")]
        [Range(1, 10, ErrorMessageResourceName = "MSG13", ErrorMessageResourceType = typeof(MessageResource))]
        public string F02_MsrSeqNo { get; set; }

        [Display(Name = "Weighing Method")]
        public int F02_WeighingMethod { get; set; }

        [Display(Name = "Additive")]
        public string F02_Addtive { get; set; }

        [Display(Name = "Crushing (1)")]
        public int F02_MilingFlag1 { get; set; }

        [Display(Name = "Crushing (2)")]
        public int F02_MilingFlag2 { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public DateTime F02_AddDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public DateTime F02_UpdateDate { get; set; }

        public int? F02_UpdateCount { get; set; }

        public bool IsCreate { get; set; }
  
        public string OldThrawSeqNo { get; set; }
    }
}