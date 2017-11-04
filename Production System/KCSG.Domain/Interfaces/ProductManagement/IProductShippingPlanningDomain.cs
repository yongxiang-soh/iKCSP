using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;


namespace KCSG.Domain.Interfaces.ProductManagement
{
    public interface IProductShippingPlanningDomain
    {
        ProductShippingPlanningItem GetById(string id,string productCode,string userCode);
        void Create(ProductShippingPlanningItem productShippingPlanning);
        void Update(ProductShippingPlanningItem productShippingPlanning);
        bool Delete(string code);
        bool CheckUnique(string productCode);
        ResponseResult CreateOrUpdate(ProductShippingPlanningItem model);
        ResponseResult<GridResponse<ProductShippingPlanningItem>> SearchCriteria(string codeShipNo,GridSettings gridSettings);
        IEnumerable<TX44_ShippingPlan> GetProductShippingPlanning(string productCode);
        //ResponseResult<GridResponse<ProductShippingPlanningItem>> SearchSample(string date, GridSettings gridSettings);  
        string GenShippingNo();
        bool CheckProductShelfStatus(string proCode, string productLotNo);
        //ProductShippingPlanningItem CheckReqShippingQty(ProductShippingPlanningItem model);
        ProductShippingPlanningItem CheckReqShippingQty(string productCode, double shippingQty);

        bool Exist(string shippingNo);
    }
}
