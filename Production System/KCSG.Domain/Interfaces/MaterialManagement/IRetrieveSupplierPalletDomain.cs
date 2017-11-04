using KCSG.Domain.Models;
using System.Collections.Generic;

namespace KCSG.Domain.Interfaces.MaterialManagement
{
    public interface IRetrieveSupplierPalletDomain
    {
        #region Methods

        /// <summary>
        /// Find condition matched supplier pallet
        /// Analyze and process those found record.
        /// </summary>
        /// <param name="supplierCode"></param>
        /// <param name="requestedLoadAmount"></param>
        void ProcessSupplierPallet(string supplierCode, double requestedLoadAmount,string terminalNo);

        /// <summary>
        /// Find possible retrieval quantity.
        /// </summary>
        /// <param name="supplierCode"></param>
        /// <returns></returns>
        double FindPossibleRetrievalQuantity(string supplierCode);

        /// <summary>
        /// 	If there is no [tm14_device] record whose [f14_devicecode] is “ATW001” 
        /// or [f14_devicestatus] is “1” (Offline), [f14_devicestatus] is “2” (Error) or [f14_usepermitclass] is “1” (Prohibited), 
        /// the system will show error message using template MSG 16
        /// </summary>
        /// <returns></returns>
        bool IsValidDevice();

        /// <summary>
        /// Find Number Of Material ShelfStatus Record
        /// </summary>
        /// <param name="supplierCode"></param>
        /// <returns></returns>
        int FindNumberOfMaterialShelfStatusRecord(string supplierCode);

        /// <summary>
        /// Update production command using terminal number.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        IList<FirstCommunicationResponse> UpdateProductCommand(string terminalNo);

        #endregion
    }
}