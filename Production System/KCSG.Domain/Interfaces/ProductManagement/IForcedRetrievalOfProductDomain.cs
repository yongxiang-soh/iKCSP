using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.ProductManagement
{
    public interface IForcedRetrievalOfProductDomain
    {
        /// <summary>
        /// Check Tx40 record exists
        /// refer br40 srs product management sub system 1.1
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="productLotNo"></param>
        /// <returns></returns>
        bool CheckRecordExistsFromTX40(string productCode, string productLotNo);

        bool CheckRecordExistsFromTX40ForButtonRetrieval(string productCode, string productLotNo);
        bool CheckRecordExistsFormTX40AndTX57(string productCode, string productLotNo);


        ResponseResult<GridResponse<TempTableItem>> GetData(string productCode, string productLotNo, bool isPallet, double requestRetrievalQuantity, GridSettings gridSettings);

        List<ForceRetrievalOfProductItem> ProductDetail(string productCode, string palletNo);
        void DeAssignAllAssignedPallet(string lstPalletNo, string productCode, string productLotNo, string terminalNo);

        /// <summary>
        /// Unassign all pallets belong to product code & product lot no.
        /// </summary>
        /// <param name="productCode"></param>
        /// <param name="productLotNo"></param>
        void UnassignPallet(string productCode, string productLotNo);

        void RetrieveProduct(IList<TempTableItem> items , string terminalNo);

        void DeasigningPallet(string shelfNo, string palletNo);
        double GetTotalTally(string productCode, string productLotNo,bool isPallet,double requestRetrievalQuantity);

        string GetPalletNo(string productCode, string productLotNo);
        /// <summary>
        /// Respond reply from C3.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        List<ThirdCommunicationResponse> RespondingReplyFromC3(string terminalNo, string productCode);


    }
}
