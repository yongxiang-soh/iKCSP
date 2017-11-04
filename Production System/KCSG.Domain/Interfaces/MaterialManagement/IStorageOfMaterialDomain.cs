using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.Domain.Models.MaterialManagement;

namespace KCSG.Domain.Interfaces.MaterialManagement
{
    public interface IStorageOfMaterialDomain
    {
        /// <summary>
        /// Find material shelf stocks by using pallet number.
        /// </summary>
        /// <param name="palleNo"></param>
        /// <returns></returns>
        Task<List<TX33_MtrShfStk>> FindMaterialShelfStockByPalletNo(string palleNo);
        
        void Store(StorageOfMaterialItem model, string terminalNo);
        bool CheckStorageMaterialShelfStatus(string liqClass);

        void DeleteNotStock(string palletNo);
        IQueryable<TX34_MtrWhsCmd> GetListMaterialWarehouseCommand();

        bool CheckUnitOfMaterialDB(string materialCode, double fraction1, double fraction2, double fraction3,
            double fraction4, double fraction5);

        bool CheckMaterialCode(string materialCode, string poNo, string partialDelivery);

        /// <summary>
        ///     process data when receive message for C1
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="pictureNo"></param>
        IList<FirstCommunicationResponse> PostStoreMaterial(string terminalNo, string pictureNo);
    }
}
