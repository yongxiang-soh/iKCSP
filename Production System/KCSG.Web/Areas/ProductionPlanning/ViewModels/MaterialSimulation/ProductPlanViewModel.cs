using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using KCSG.Data.DataModel;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.MaterialSimulation
{
    public class ProductPlanViewModel
    {
        [DisplayName(@"Period")]
        [Required(ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName = "MSG2")]
        public string From { get; set; }
        public string To { get; set; }

        [DisplayName(@"Line")]
        public bool Line { get; set; }
        public IEnumerable<TM03_PreProduct> productName { get; set; }
        public  IEnumerable<PreProductPlanSimuItem> LstPreProductPlanSimuItem { get; set; }
        public Grid Grid { get; set; }
    }
}