using KCSG.jsGrid.MVC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.PreProduct
{
    public class PreProductViewModel
    {
        [Key, Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(12, ErrorMessage = "Pre Product Code cannot be longer than 12 characters.")]
        [Remote("CheckPreProductCodeExistence", "PreProduct", ErrorMessage = "The same record exists in database", HttpMethod = "Options", AdditionalFields = "IsCreate")]
        [Display(Name = "Pre-Product Code")]
        public string F03_PreProductCode { get; set; }

        [StringLength(15)]
        [Display(Name = "Pre-Product Name")]
        public string F03_PreProductName { get; set; }

        //[Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "Days")]
        [Range(0, 9, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG56")]
        public int? Days { get; set; }

       // [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "Equilibrium Time")]
        [DataType(DataType.Time)]
       
        //[DisplayFormat(DataFormatString = "{0:hh:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan TmpRetTime { get; set; }

        //[Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(1)]
        [Display(Name = "Mixmode")]
        public string F03_MixMode { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(2)]
        [Display(Name = "Container Type")]
        public string F03_ContainerType { get; set; }
       
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Range(1, 5, ErrorMessage = "The inputted value is out of range.")]        
        [Display(Name = "Batch/Lot")]
        public int? F03_BatchLot { get; set; }

        [StringLength(1)]
        [Display(Name = "Kneading Line")]
        public string F03_KneadingLine { get; set; }

      //  [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(3)]
        [Display(Name = "Factor")]
        public string F03_Point { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Range(1, 100, ErrorMessage = "The inputted value is out of range.")]        
        [Display(Name = "Yield Rate")]
       // [Range(1, 100, ErrorMessage = "The inputted value is out of range.")]
        public double F03_YieldRate { get; set; }

        [Display(Name = "Mix Time 1")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}", ApplyFormatInEditMode = true)]
        public TimeSpan? MixDate1 { get; set; }

        [Display(Name = "Mix Time 2")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}", ApplyFormatInEditMode = true)]
        public TimeSpan? MixDate2 { get; set; }

        [Display(Name = "Mix Time 3")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:hh:mm:ss}", ApplyFormatInEditMode = true)]
        public TimeSpan? MixDate3 { get; set; }

        [StringLength(1)]
        [Display(Name = "Low Temperature")]
        public string F03_LowTmpClass { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Range(-1, 100, ErrorMessage = "The inputted value is out of range.")]
        [Display(Name = "Last No of Lot")]
        public int F03_LotNoEnd { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public DateTime F03_AddDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public DateTime F03_UpdateDate { get; set; }

        public int? F03_UpdateCount { get; set; }

        public bool IsCreate { get; set; }

        public List<PrePdtMkpViewModel> ListPrePdtMkp { get; set; }

        public Grid Grid { get; set; }
        
    }
}