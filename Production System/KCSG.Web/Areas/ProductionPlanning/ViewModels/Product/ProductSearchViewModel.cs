using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.Product
{
    public class ProductSearchViewModel
    {
        [DisplayName("Product Code")]
        [StringLength(12)]
        public string ProductCode { get; set; }
        public Grid Grid { get; set; }
    }
}