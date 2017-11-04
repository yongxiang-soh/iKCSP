using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace KCSG.Web.Areas.ProductionPlanning.ViewModels.MaterialSimulation
{
    public class MaterialViewModel
    {   
        [DisplayName(@"Material Code")]
        public string MaterialCode { get; set; }
        [DisplayName(@"Material Name")]
        public string MaterialName { get; set; }

        public Dictionary<string, double> Chart  { get; set; }
    }
}