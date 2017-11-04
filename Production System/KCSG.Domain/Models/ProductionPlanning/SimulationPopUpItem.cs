using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.ProductionPlanning
{
    public class SimulationPopUpItem
    {
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public double RequiredQuantity{ get; set; }
        public double? Stock { get; set; }
        public double Remainder { get; set; }

    }
}
