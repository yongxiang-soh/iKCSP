using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.Material
{
    public class SupplierCodeSelectViewModel
    {
        public string SupplierCode { get; set; }
        public Grid Grid { get; set; }
    }
}
