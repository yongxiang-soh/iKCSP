using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.PckMtr
{
    public class PckMtrViewModel
    {
        [Key]
        //[Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "Product Code")]
        [StringLength(12, ErrorMessage = "Product Code cannot be longer than 12 characters.")]
        public string F11_ProductCode { get; set; }
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Key]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "Sup. Material Code")]
        [StringLength(12, ErrorMessage = "SubMaterial Code cannot be longer than 12 characters.")]
        [Remote("CheckExistSubCode", "Product", ErrorMessage = "The supplementary material code cannot duplicate !", HttpMethod = "Options", AdditionalFields = "F11_ProductCode,IsCreate")]
        public string F11_SubMaterialCode { get; set; }
        
        
        [Display(Name = "Sup. Material Name")]
        public string SubMaterialName { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Range(0.01, 999999.99, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "Quantity")]
        public double F11_Amount { get; set; }

        //[Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "Unit")]
        [StringLength(1, ErrorMessage = "Unit cannot be longer than 1 characters.")]
        public string F11_Unit { get; set; }
        public DateTime F11_AddDate { get; set; }
        public DateTime F11_UpdateDate { get; set; }
        public int F11_UpdateCount { get; set; }
        public bool IsCreate { get; set; }
        public bool IsUpdate { get; set; }
    }
}