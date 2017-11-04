using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Resources;
using System.Web.Mvc;
using System.Web.Services.Description;
using DocumentFormat.OpenXml;
using KCSG.Data.DataModel;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.ProductCertification
{
    public class ProductCertificationViewModel
    {
        [Key, Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(12)]
        [Display(Name = @"Product Code")]
        [Remote("CheckExistCode", "ProductCertification", HttpMethod = "OPTIONS", AdditionalFields = "F67_ProductLotNo,IsCreate", ErrorMessageResourceType = typeof(ProductManagementResources), ErrorMessageResourceName = "MSG21")]
        public string F67_ProductCode { get; set; }

        [StringLength(15)]
        [Display(Name = @"Product Name")]
        //public string F09_ProductDesp { get; set; }
        public string F09_ProductDesp { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(10)]
        [Display(Name = @"Product Lot No")]
        public string F67_ProductLotNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(10)]
        [Display(Name = @"PreProduct Lot No")]
        public string F67_PrePdtLotNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(10)]
        [Display(Name = @"Product Flg")]
        public string F67_ProductFlg { get; set; }



        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        //[StringLength(11)]
        [Display(Name = @"Quantity")]
        public double? F67_Amount { get; set; }


        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]       
        [Display(Name = @"Certification Date")]
        [DisplayFormat(DataFormatString = "{DD/MM/YYYY}", ApplyFormatInEditMode = false)]
        public string CertificationDate { get; set; }



        //[Required(ErrorMessageResourceType = typeof(ProductManagementResources), ErrorMessageResourceName = "MSG37")]
        //public DateTime F67_AddDate { get; set; }
        //[Required(ErrorMessageResourceType = typeof(ProductManagementResources), ErrorMessageResourceName = "MSG37")]
        //public DateTime F67_UpdateDate { get; set; }
        //public int? F67_UpdateCount { get; set; }
        public bool IsCreate { get; set; }
        public int KndLine { get; set; }
    }
}