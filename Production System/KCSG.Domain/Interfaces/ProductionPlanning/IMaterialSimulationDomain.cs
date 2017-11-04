using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.ProductionPlanning
{
     public interface IMaterialSimulationDomain
     {
         Dictionary<string, double> GenerateMaterial(DateTime from, DateTime to, bool inventoryUnderRetrieval, bool acceptedMaterialOnly, bool materialUsedInOtherCommands, string selectMaterial);
         IEnumerable<PreProductPlanSimuItem> GenerateProductPlan(DateTime from, DateTime to, bool inventoryUnderRetrieval, bool acceptedMaterialOnly, bool materialUsedInOtherCommands);
         IEnumerable<TM03_PreProduct> GetPreProductName(DateTime from, DateTime to);
         ResponseResult<GridResponse<SimulationPopUpItem>> GetDataPopUp(DateTime convertToDateTimeFull,DateTime endDate ,string preProductCode, string date,  bool acceptedMaterialOnly, bool iRetrieval, bool materialUsedInOtherCommands, GridSettings gridSettings);
     }
}
