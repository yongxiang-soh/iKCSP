using System.Collections.Generic;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Interfaces.MaterialManagement
{
    public interface IRestorageOfMaterialDomain
    {
        void RestoreMaterial(string palletNo, string materialCode,string terminalNo, IList<RestorageMaterialItem> restorageMaterialItems);

        /// <summary>
        /// This function is for unassigning material by using pallet number and material code.
        /// </summary>
        /// <param name="palletNo"></param>
        /// <param name="materialCode"></param>
        void UnassginMaterial(string palletNo, string materialCode);

        /// <summary>
        /// Empty material storage by using material code.
        /// </summary>
        /// <param name="materialCode"></param>
        void EmptyMaterialStorage(string materialCode,string terminalNo);

        /// <summary>
        /// Check whether device is valid or not.
        /// </summary>
        /// <returns></returns>
        bool IsValidDevice();

        /// <summary>
        /// Find material shelf stocks in database base on specific information.
        /// </summary>
        /// <param name="materialCode"></param>
        /// <param name="palletNo"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        IList<MaterialDetailInformationViewModel> FindMaterialShelfStocks(string materialCode, string palletNo, string terminalNo);
    }
}