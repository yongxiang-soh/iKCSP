using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.Domain.Models.PreProductManagement;

namespace KCSG.Domain.Interfaces.PreProductManagement
{
    public interface IPreProductManagementDomain
    {
        /// <summary>
        /// Get Storage Shelf No
        /// </summary>
        /// <returns></returns>
        string GetStorageShelfNo();
        /// <summary>
        /// Insert And Update
        /// </summary>
        /// <param name="containerNo"></param>
        /// <param name="storageShelfNo"></param>
        /// <param name="containerType"></param>
        /// <param name="terminalNo"></param>
        void InsertAndUpdate(int containerNo, string storageShelfNo, string containerType, string terminalNo);
        /// <summary>
        /// Check record in tx50
        /// </summary>
        /// <returns></returns>
        bool CheckExistsTX50();

        /// <summary>
        /// CheckedValidation
        /// </summary>
        /// <param name="containerNo"></param>
        /// <returns></returns>
        bool CheckedValidation(string containerNo);

        /// <summary>
        ///     Please refer BR9 - Displaying rules - SQL10
        /// </summary>
        /// <param name="kneadingLine"></param>
        /// <returns></returns>
        Task<IList<StorageOfPreProductItem>> FindEmptyContainers(Constants.KndLine kneadingLine);

        /// <summary>
        ///     Find conveyor by using kneading line & terminal no & colorClass.
        ///     Please refer (BR9 - Displaying rules - SQL11 & SQL12)
        /// </summary>
        /// <param name="kneadingLine"></param>
        /// <param name="terminalNo"></param>
        /// <param name="colorClass"></param>
        /// <returns></returns>
        Task<IList<string>> FindConveyors(Constants.KndLine kneadingLine, string terminalNo, string colorClass);

        /// <summary>
        ///     This function is for initiating information after messages have been sent from C2.
        ///     Please refer BR12 for more information.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="preProductCode"></param>
        IList<SecondCommunicationResponse> InitiateInformationFromC2(string terminalNo,string preProductCode);

        /// <summary>
        ///     Please refer Br29 in the document for more information.
        ///     This function is for analyzing shelf
        /// </summary>
        void Br29(string shelfNo, out string lsRow, out string lsBay, out string lsLevel);

        /// <summary>
        ///     Find list of pre-product warehouse commands.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        IQueryable<TX50_PrePdtWhsCmd> FindPreProductWarehouseCommandAsync(string terminalNo);

        /// <summary>
        ///     Please refer BR10
        /// </summary>
        /// <param name="containerType"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        Task<TX37_PrePdtShfSts> FindPrePdtShfSts(string containerType, string terminalNo);

        /// <summary>
        ///     Please refer BR16 in document for more information.
        /// </summary>
        /// <param name="lsRow"></param>
        /// <param name="lsBay"></param>
        /// <param name="lsLevel"></param>
        /// <param name="containerNo"></param>
        /// <param name="containerCode"></param>
        /// <param name="kneadingLine"></param>
        /// <param name="terminalNo"></param>
        /// <param name="colorClass"></param>
        /// <returns></returns>
        Task PreProductStorageOk(string lsRow, string lsBay, string lsLevel, string containerNo, string containerCode,
            Constants.KndLine kneadingLine,
            string terminalNo, string colorClass);

        /// <summary>
        ///     Please refer BR18 in document for more information.
        /// </summary>
        /// <param name="lsRow"></param>
        /// <param name="lsBay"></param>
        /// <param name="lsLevel"></param>
        /// <param name="containerNo"></param>
        /// <param name="containerCode"></param>
        /// <param name="conveyorCode"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        void ConfirmStorageNg(string lsRow, string lsBay, string lsLevel, string containerNo, string containerCode,
            string conveyorCode, string terminalNo);

        void UpdateStatusForPreProductWarehouse(string commandNo, string cmdSeqNo, string oldStatus, string newStatus);

        /// <summary>
        /// Analyze result after messages from C2 having been received.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="containerType"></param>
        /// <param name="containerNo"></param>
        /// <param name="shelfNo"></param>
        /// <returns></returns>
        List<SecondCommunicationResponse> ReceiveMessageFromC2(string terminalNo, string containerType, string containerNo);

        /// <summary>
        ///     Please UC 6 - SRS 1.1 Sign off.
        /// </summary>
        /// <param name="isChecked"></param>
        /// <param name="preProductCode"></param>
        /// <param name="containerCode"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        IList<SecondCommunicationResponse> AnalyzeC2MessageUc6(bool isChecked, string preProductCode, string containerCode,
            string terminalNo);

        /// <summary>
        /// Retrieval
        /// </summary>
        /// <param name="containerType"></param>
        /// <param name="terminalNo"></param>
        /// <param name="kneadingLine"></param>
        /// <param name="colorClass"></param>
        /// <returns></returns>
        RetrieveOfEmptyContainerItem Retrieval(string containerType, string terminalNo, Constants.KndLine kneadingLine,
            string colorClass);

        bool UpdateTX37(string temperature, string terminalNo,string preProductCode);

        /// <summary>
        /// Store back material into warehouse.
        /// </summary>
        /// <param name="commandNo"></param>
        /// <param name="containerCode"></param>
        /// <param name="preProductLotNo"></param>
        /// <param name="preProductCode"></param>
        /// <param name="quantity"></param>
        /// <param name="containerNo"></param>
        /// <param name="containerType"></param>
        /// <param name="terminalNo"></param>
        /// <param name="lotEndFlag"></param>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <param name="colorClass"
        void Storage(string commandNo, string containerCode, string preProductLotNo, string preProductCode,
            double quantity, string containerNo, string containerType, string terminalNo, string lotEndFlag, string row, string bay, string level, string colorClass);


        IList<SecondCommunicationResponse> ReceiveMessageFromC2WhenClickStorage(string terminalNo,
            string preProductCode, string row,
            string bay, string level, string containerCode, string containerNo, string containerType, string commandNo,
            string preProductLotNo);

    }
}