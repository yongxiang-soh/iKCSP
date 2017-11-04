using KCSG.Core.Models;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KCSG.Domain.Interfaces.ProductManagement
{
    public interface IOutOfPlanProductDomain
    {
        /// <summary>
        /// Find list of out of plan of product.
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        ResponseResult<GridResponse<OutOfPlanProductItem>> FindOutOfPlanProduct(
            string productCode, GridSettings gridSettings);

        OutOfPlanProductItem GetByProductsCode(string productCode,string prePdtLotNo);
        ResponseResult CreateOrUpdate(OutOfPlanProductItem model, bool isCreate);
        bool CheckUnique(string productCode,string prepdtlotno);
        bool Delete(string productCode, string prepdtlotno);
    }
}
