using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Controls.CustomerValidate;
using KCSG.jsGrid.MVC;
using KCSG.Web.Areas.ProductionPlanning.ViewModels.PckMtr;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.Product
{
    public class ProductViewModel
    {
        [Key, Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "Product Code")]
        [StringLength(12, ErrorMessage = "Product Code cannot be longer than 12 characters.")]
        [Remote("CheckExistCode", "Product", ErrorMessage = "The same record exists in database", HttpMethod = "Options", AdditionalFields = "IsCreate")]
        public string F09_ProductCode { get; set; }

        [StringLength(15, ErrorMessage = "Product Name cannot be longer than 15 characters.")]
        [Display(Name = "Product Name")]
        public string F09_ProductDesp { get; set; }

        [StringLength(15, ErrorMessage = "Tablet Type cannot be longer than 15 characters.")]
        [Display(Name = "Tablet Type")]
        public string F09_TabletType { get; set; }

        //[StringLength(4, ErrorMessage = "Tablet Size cannot be longer than 4 characters.")]
        [Display(Name = "Tablet Size")]
        public string F09_TabletSize { get; set; }

        //[StringLength(5, ErrorMessage = "Tablet Size cannot be longer than 5 characters.")]
        [Display(Name = "Tablet Size 2")]
        public string F09_TabletSize2 { get; set; }
        [Display(Name = "Lead Time")]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG5")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public double? F09_NeedTime { get; set; }



         //[Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
         [Display(Name = "Tablet Quantity")]
         //[ExtRange(1.00, 9999999.99)]
         [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG5")]
        public double? F09_TabletAmount { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "Kneading Time")]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG5")]
         public double? F09_KneadingTime { get; set; }

        [StringLength(5, ErrorMessage = "End User Code cannot be longer than 5 characters.")]
        [Display(Name = "End User Code")]
        public string F09_EndUserCode { get; set; }

         [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "Yield Rate")]
        public double? F09_YieldRate { get; set; }

        [StringLength(3, ErrorMessage = "Factor cannot be longer than 3 characters.")]
        [Display(Name = "Factor")]
        public string F09_Factor { get; set; }

        //[Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "Standard In-stock Period")]
        [Range(0, double.MaxValue, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG5")]
        public double? F09_StdStkMtn { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "Packing Unit")]
        
        [Remote("CheckPackingUnit", "Product", ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG5", HttpMethod = "Options", AdditionalFields = "F09_Unit")]
       
        public double? F09_PackingUnit { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(12, ErrorMessage = "Pre Product Code cannot be longer than 12 characters.")]
        [Display(Name = "Pre-Product Code")]
        public string F09_PreProductCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(1, ErrorMessage = "Unit cannot be longer than 1 characters.")]
        [Display(Name = "Unit")]
        [RegularExpression("['K','P']", ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG11")]
      
        public string F09_Unit { get; set; }

        [StringLength(2, ErrorMessage = "Valid Period cannot be longer than 2 characters.")]
        [Display(Name = "Shelf Life")]
        public string F09_ValidPeriod { get; set; }

        [StringLength(1, ErrorMessage = "Inside Label Class cannot be longer than 1 characters.")]
        [Display(Name = "Inner Label Req.")]
        public string F09_InsideLabelClass { get; set; }
       
        [StringLength(1, ErrorMessage = "F09_LowTmpCls cannot be longer than 1 characters.")]
        [Display(Name = @"Temperature")]
        public string F09_LowTmpCls { get; set; }
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public DateTime F09_AddDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public DateTime F09_UpdateDate { get; set; }
        public int F09_UpdateCount { get; set; }
        [StringLength(20)]
        [Display(Name = "Label")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string F09_Label { get; set; }

        public bool IsCreate { get; set; }
         //  public List<PckMtrViewModel> ListPckMtr { get; set; }
        public Grid GridSupMaterial { get; set; }
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Display(Name = "Sup. Material")]
        public string SupMaterial { get; set; }
    }
}