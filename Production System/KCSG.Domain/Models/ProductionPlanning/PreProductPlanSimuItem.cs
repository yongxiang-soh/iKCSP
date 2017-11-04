using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.ProductionPlanning
{
   public class PreProductPlanSimuItem
    {
       public int Count { get; set; }
       public string Date { get; set;}
       public List<PreproductISimulerItem> PreproductISimulerItems { get; set; }
    }

    public class PreproductISimulerItem
    {
        public string PreProductCode { get; set; }
        public string Simulation { get; set; }
        public bool KndLine { get; set; }
        public int Code { get; set; }
        public string Command { get; set; }
        public string Name { get; set; }
        public int Lot { get; set; }
        public int batch { get; set; }
        public int Count { get; set; }
        public List<SimulationPopUpItem> SimulationPopUpItems { get; set; } 
    }
}
