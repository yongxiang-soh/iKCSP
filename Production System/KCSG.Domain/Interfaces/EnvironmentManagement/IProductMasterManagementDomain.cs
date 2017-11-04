using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.EnvironmentManagement
{
    public interface IProductMasterManagementDomain
    {
        IList<TM09_Product> GetProducts();

        ResponseResult<GridResponse<ProductMasterManagementItem>> SearchCriteria(string location,
            jsGrid.MVC.GridSettings gridSettings);

        void Delete(string code);
        void Edit(ProductMasterManagementItem item);
        void Add(ProductMasterManagementItem item);
        bool CheckValueEntered(int locationId);
        bool CheckLocation(int id);
    }
}
