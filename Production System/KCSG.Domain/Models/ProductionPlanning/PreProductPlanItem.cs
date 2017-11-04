using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.ProductionPlanning
{
    public class PreProductPlanItem : TX94_Prepdtplan
    {

        public bool IsCreate { get; set; }
        public new string amount { get { return this.F94_amount.ToString("N"); }}
        public string PreProductName
        {
            get ;
            set;
        }
    }


}
