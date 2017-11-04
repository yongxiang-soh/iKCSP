using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Resources;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.ProductShippingPlanning
{
    public class ProductShippingPlanningViewModel
    {
        [Key, Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [StringLength(8)]
        [Display(Name = @"Shipping No.")]
        public string F44_ShipCommandNo { get; set; }


        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Remote("CheckExistCode", "ProductShippingPlanning", HttpMethod = "OPTIONS", AdditionalFields = "F44_ProductLotNo,IsCreate", ErrorMessageResourceType = typeof(ProductManagementResources), ErrorMessageResourceName = "MSG1")]
        [Display(Name = @"Product Code")]
        
        public string F44_ProductCode { get; set; } 

                
        [Display(Name = @"Product Name")]
        public string F09_ProductDesp { get; set; }


        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        [Remote("CheckReqShippingQty", "ProductShippingPlanning", HttpMethod = "OPTIONS", AdditionalFields = "F44_ProductCode", ErrorMessageResourceType = typeof(ProductManagementResources), ErrorMessageResourceName = "MSG27")]
        [Range(1, 999999999, ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        //[StringLength(10)]
        [Display(Name = @"Req Shipping Qty")]
        public string F44_ShpRqtAmt { get; set; }

        //[Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]  
        [Remote("CheckExistCode", "ProductShippingPlanning", HttpMethod = "OPTIONS", AdditionalFields = "F44_ProductCode,IsCreate", ErrorMessageResourceType = typeof(ProductManagementResources), ErrorMessageResourceName = "MSG1")]
        [StringLength(10)]
        [Display(Name = @"Product LotNo")]
        public string F44_ProductLotNo { get; set; }



        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]        
        [DisplayFormat(DataFormatString = "{DD/MM/YYYY}", ApplyFormatInEditMode = false)]
        [Display(Name = @"Delivery Date")]
        public string DeliveryDate { get; set; }


        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]        
        [Display(Name = @"End User Code")]
        public string F44_EndUserCode { get; set; }


        //[Required(ErrorMessageResourceType = typeof(ProductManagementResources), ErrorMessageResourceName = "MSG37")]
        [StringLength(50)]
        [Display(Name = @"End User Name")]
        public string F10_EndUserName { get; set; }

        
        
        [Display(Name = @"Status")]
        public string F44_Status { get; set; }


        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]        
        public DateTime F44_AddDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public DateTime F44_UpdateDate { get; set; }
        public int? F44_UpdateCount { get; set; }
        public bool IsCreate { get; set; }
        public int KndLine { get; set; }
    }
}