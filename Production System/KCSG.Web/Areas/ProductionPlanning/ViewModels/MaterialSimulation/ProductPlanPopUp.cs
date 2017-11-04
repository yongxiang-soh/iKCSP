using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.MaterialSimulation
{
    public class ProductPlanPopUp
    {
        [DisplayName("Production Command")]
        public string Command { get; set; }
        [DisplayName("Pre-product Code")]
        public string PreProductCode { get; set; }
        [DisplayName("Lot Quantity")]
        public string LotQuantity { get; set; }
        [DisplayName("Pre-product Name")]
        public string PreProductName { get; set; }
        public Grid Grid { get; set; }
        public List<SimulationPopUpItem> SimulationPopUpItems { get; set; } 
    }
}