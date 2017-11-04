using System;
using System.Linq;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces
{
    public interface ICommonDomain
    {
        /// <summary>
        /// From terminal no, find the related conveyor code.
        /// </summary>
        /// <returns></returns>
        Task<TM05_Conveyor> FindConveyorCodeAsync(string terminalNo);

        /// <summary>
        /// Find conveyor by using terminal no synchronously.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        TM05_Conveyor FindConveyor(string terminalNo);

        /// <summary>
        /// Find device by its availability.
        /// </summary>
        /// <returns></returns>
        Task<TM14_Device> FindDeviceAvailabilityAsync();

        Task<TM14_Device> FindDeviceAvailabilityAsync(string productDeviceCode);

        /// <summary>
        /// Insert a record into no manage and calculate the seq no.
        /// </summary>
        /// <returns></returns>
        Task<int> InsertIntoNoManageAsync();

        /// <summary>
        /// Find product pallet no asynchronously.
        /// </summary>
        /// <param name="commandNo"></param>
        /// <param name="preProductCode"></param>
        /// <param name="preProductLotNo"></param>
        /// <returns></returns>
        Task<TX53_OutSidePrePdtStk> FindProductPalletNoAsync(string commandNo, string preProductCode,
            string preProductLotNo);
        
        /// <summary>
        /// Check conveyor status
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        bool CheckStatusAndNumberRecordOfConveyor(string terminalNo);

        bool CheckHoliday(DateTime day);
     
        bool CheckStatusOfDeviceRecord(string deviceCode);
        Task<int> UpdateProductShelfStockAsync(string row, string bay, string level, string status, DateTime updateDate);

        /// <summary>
        /// Find product shelf row, bay, level
        /// </summary>
        /// <param name="shelfNo"></param>
        /// <returns></returns>
        ProductShelfNoItem FindProductShelfInformation(string shelfNo);

        /// <summary>
        /// Check status record in tm14_device
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        bool CheckStatusOfDeviceRecordForProductManagement(string deviceCode);

        /// <summary>
        /// Update conveyor and pre-product warehouse.
        /// </summary>
        /// <returns></returns>
      

      

        ///// <summary>
        ///// Check status of conveyor and product warehouse.
        ///// </summary>
        ///// <param name="terminalNo"></param>
        ///// <returns></returns>
        //ResponseResult CheckingStatusOfConveyorAndPreproductWarehouse(string terminalNo);
    }
}