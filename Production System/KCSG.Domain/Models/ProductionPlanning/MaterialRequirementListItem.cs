using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.ProductionPlanning
{
   public class MaterialRequirementListItem:TX94_Prepdtplan
   {
       public string F01_MaterialCode { get; set; }
       public string F01_MaterialDsp { get; set; }
       public double F01_Unit { get; set; }
       public double Sum3F4F { get; set; }
       public IEnumerator GetEnumerator()
       {
           throw new NotImplementedException();
       }
   }
}
