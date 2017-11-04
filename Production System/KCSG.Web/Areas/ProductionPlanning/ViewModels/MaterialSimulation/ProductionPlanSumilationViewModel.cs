using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.MaterialSimulation
{
    public class ProductionPlanSumilationViewModel
    {
        public Grid Grid { get; set; }
         [DisplayName("Production Command")]
        public string Command { get; set; }
         [DisplayName("Pre-product Code")]
        public string PreProductCode { get; set; }
        [DisplayName("Pre-product Name")]
        public string PreProductName { get; set; }
         [DisplayName("Lost Quantity")]
        public string LostQuantity { get; set; }
    }
}