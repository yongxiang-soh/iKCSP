using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.ProductManagement.ViewModels.ProductShippingPlanning
{
    public class ProductShippingPlanningSearchViewModel
    {
        //[Range(typeof(DateTime), "01/1991", "01/2100", ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG27")]
        [StringLength(8)]
        [Display(Name = "Shipping No.")]
        public string ShippingNo { get; set; }
        
        public Grid Grid { get; set; }
            
    }
}