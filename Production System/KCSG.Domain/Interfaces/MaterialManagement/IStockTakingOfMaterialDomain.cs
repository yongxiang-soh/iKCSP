using System.Collections.Generic;
using KCSG.Core.Models;
using KCSG.Domain.Models;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.MaterialManagement
{
   public interface IStockTakingOfMaterialDomain
    {
       ResponseResult<GridResponse<StockTakingOfMaterialItem>> SearchMaterialStock(string shelfNoFrom, string shelfNoTo, GridSettings gridSettings);
       MaterialShelfStatusItem SearchMaterialShelfStatus(string shelfRow, string shelfBay, string shelfLevel);
       IEnumerable<StockTakingOfMaterialStockItem> GetMaterialShelfStocks(string shelfNo, string materialCode);
        void Retrieve(string firstRowShelfNo, string firstRowPalletNo, string firstRowMaterialCode, string firstRowMaterialName, string currentRowPalletNo,string terminalNo);
        void UpdateMaterialShelfStatus(MaterialShelfStatusItem shelfStatus);
        void RestoreMaterialStocks(RestoreMaterialViewModel model,string terminalNo);
        List<MaterialShelfStockItem> GetStockByPalletNo(string palletNo);

        /// <summary>
        /// Analyze data responded from C1.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="materialCode"></param>
        /// <returns></returns>
        IList<FirstCommunicationResponse> PostRetrieveMaterial(string terminalNo, string materialCode);

        IList<FirstCommunicationResponse> CompleteStoraging(string terminalNo, string materialCode);
    }
}
