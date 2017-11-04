using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.EnvironmentManagement
{
    public class CalculationOfControlLimitItem : Te80_Env_Mesp
    {
        public string DateFromTo
        {
            get
            {
                if (this.F80_D_From != null && this.F80_D_To != null)
                {
                    return this.F80_D_From.Value.ToString("dd/M/yyyy") + '-' + this.F80_D_To.Value.ToString("dd/M/yyyy");  
                }
                return "";
            }
        }
    }
}
