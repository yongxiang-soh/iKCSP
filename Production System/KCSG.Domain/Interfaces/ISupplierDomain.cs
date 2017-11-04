using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces
{
   public interface ISupplierDomain
   {
        bool CheckUnique(string code);
       IEnumerable<TM04_Supplier> GetSuppliers(string code);
        ResponseResult<GridResponse<SupplierItem>> GetSupplierCodes(string code, GridSettings gridSettings);
   }
}
