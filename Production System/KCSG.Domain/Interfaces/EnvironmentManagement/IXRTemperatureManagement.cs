using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Domain.Models.EnvironmentManagement;

namespace KCSG.Domain.Interfaces.EnvironmentManagement
{
   public interface IXRTemperatureManagement
   {
       ResponseResult<XRTemperatureItem> Search(string productCode, Constants.EnvMode envMode, DateTime startDate,
           DateTime endDate);

       SelectList GetProduct();
   }

    
}
