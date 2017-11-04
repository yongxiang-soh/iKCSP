using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Models
{
   public class EndTabletisingItem
    {
      
        public string ProductCode { get; set; }

        public string ProductName { get; set; }
       
        public string LotNo { get; set; }

        public int Package { get; set; }

        public double Fraction { get; set; }

        public double PackingUnit { get; set; }
        public string CommandNo { get; set; }
        public string PreProductLotNo { get; set; }
    }
}
