using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models.ProductionPlanning
{
    public class MaterialSimulationItem
    {
        public string ProductionDate { get; set; }
        public string PreProduct1 { get; set; }
        public string PreProduct2 { get; set; }
        public string PreProduct3 { get; set; }

        public string PreProduct4 { get; set; }
    }
}
