using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.PreProductManagement
{
    public interface IStockTakingPreProductDomain
    {
        /// <summary>
        /// Find and build a grid of preproduct status.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <param name="fromRow"></param>
        /// <param name="fromBay"></param>
        /// <param name="fromLevel"></param>
        /// <param name="toRow"></param>
        /// <param name="toBay"></param>
        /// <param name="toLevel"></param>
        /// <returns></returns>
        ResponseResult<GridResponse<object>> SearchStockTakingPreProduct(GridSettings gridSettings,
            int fromRow, int fromBay, int fromLevel,
            int toRow, int toBay, int toLevel);

        /// <summary>
        /// Retrieve stock taking pre-product (Please refer document for more information)
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="preProductCode"></param>
        /// <param name="lsRow"></param>
        /// <param name="lsBay"></param>
        /// <param name="lsLevel"></param>
        /// <param name="containerCode"></param>
        /// <returns></returns>
        ResponseResult<string> RetrieveStockTakingOfPreProduct(string terminalNo, string preProductCode,
            string lsRow, string lsBay, string lsLevel, string containerCode, string containerNo, DateTime? updateDate);

       
        /// <summary>
        /// Please refer UC 13 - SRS 1.0 Sign off
        /// Allows the system to process data that sent back from C2 server after it process the function that requested for retrieving data by the system.
        /// </summary>
        /// <returns></returns>
        List<SecondCommunicationResponse> ReceiveMessageFromC2ServerForStoring(string terminalNo, string containerCode, string preProductCode,
            string shelfNo);
        //UC16 
        ResponseResult CreateAndUpdate(string preProductCode, string shelfNo, string containerCode, double quantity, string lotNo, string terminalNo, DateTime? updateDate, int containerType);
        

        /// <summary>
        /// Please refer BR 41 - SRS 1.0 Sign off
        /// </summary>
        /// <returns></returns>
        Task RetrievePreProductContainerOk(string lsRow, string lsBay, string lsLevel, string itemContainerCode,
            string itemContainerNo, string terminalNo, int containerType);
        Task RetrievePreProductContainerNg(string lsRow, string lsBay, string lsLevel, string itemContainerCode,
            string itemContainerNo, string terminalNo);

        List<SecondCommunicationResponse> ReceivingMessageFromC2(string termimalNo, string preProductCode);

        /// <summary>
        /// Please refer UC 15.
        /// </summary>
        /// <param name="containerCode"></param>
        /// <param name="lch_status"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        List<SecondCommunicationResponse> AnalyzeMessageForStoringMovingPreProduct(string containerCode, string shelfNo,
            string terminalNo, bool oKClicked,string preProductCode);

      
    }
}