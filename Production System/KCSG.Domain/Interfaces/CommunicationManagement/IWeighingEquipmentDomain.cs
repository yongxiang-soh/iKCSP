using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using KCSG.Core.Models;
using KCSG.Domain.Models.CommunicationManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces
{
   public interface IWeighingEquipmentDomain
   {
       ResponseResult<GridResponse<WeighingEquipmentItem>> SearchByMaterial(WeighingEquipmentViewSearchModel model,
           GridSettings gridSettings);

       ResponseResult<GridResponse<WeighingEquipmentItem>> SearchByPreproduct(WeighingEquipmentViewSearchModel model,
           GridSettings gridSettings);

       ResponseResult<GridResponse<WeighingEquipmentItem>> SearchByRetrieval(WeighingEquipmentViewSearchModel model,
           GridSettings gridSettings);

       ResponseResult<GridResponse<WeighingEquipmentItem>> SearchByKndCommand(WeighingEquipmentViewSearchModel model,
           GridSettings gridSettings);

       ResponseResult<GridResponse<WeighingEquipmentItem>> SearchByKndResult(WeighingEquipmentViewSearchModel model,
           GridSettings gridSettings);

       bool DeleteDataOnQueue(WeighingEquipmentViewSearchModel searchModel);
       bool SendPreproductMaster(string terminalNo, string deviceCode);

       bool SendMaterialMaster(string terminalNo, string deviceCode);
   }
}
