using System;
using System.Linq;
using KCSG.Core.Constants;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using System.Collections.Generic;
using KCSG.Domain.Models;
using AutoMapper;

namespace KCSG.Domain.Domains.MaterialManagement
{
    public class RetrieveSupplierPalletDomain : BaseDomain, IRetrieveSupplierPalletDomain
    {
        private readonly INotificationService _notificationService;

        #region Properties

        /// <summary>
        ///     Dependency injection of Unit of Work which provides functions to access to every repository.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize domain with dependency injections.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public RetrieveSupplierPalletDomain(IUnitOfWork unitOfWork, INotificationService notificationService, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        /// <summary>
        ///     Base on supplier code to find possible retrieval quantity.
        /// </summary>
        /// <param name="supplierCode"></param>
        /// <returns></returns>
        public double FindPossibleRetrievalQuantity(string supplierCode)
        {
            // Find all material shelf statuses first.
            var materialShelfStatuses = _unitOfWork.MaterialShelfStatusRepository.GetAll();

            var shelfStatus = Constants.F31_ShelfStatus.SupplierPallet.ToString("D");
            //total [f31_loadamount] of [tx31_mtrshfsts] records whose [f31_shelfstatus] is 2 and [f31_suppliercode] is current [Supplier Code]
            materialShelfStatuses = materialShelfStatuses.Where(x => x.F31_ShelfStatus.Trim().Equals(shelfStatus));
            materialShelfStatuses = materialShelfStatuses.Where(x => x.F31_SupplierCode.Trim().Equals(supplierCode));

            return materialShelfStatuses.Sum(x => x.F31_LoadAmount) ?? 0;
        }

        /// <summary>
        ///     	If there is no [tm14_device] record whose [f14_devicecode] is “ATW001”
        ///     or [f14_devicestatus] is “1” (Offline), [f14_devicestatus] is “2” (Error) or [f14_usepermitclass] is “1”
        ///     (Prohibited),
        ///     the system will show error message using template MSG 16
        /// </summary>
        /// <returns></returns>
        public bool IsValidDevice()
        {
            //// Find all device records in TM14 table.
            //var devices = _unitOfWork.DeviceRepository.GetAll();

            //// Filter devices with the given conditions above.
            //var result =
            //    devices.Any(
            //        x =>
            //            x.F14_DeviceCode.Equals("ATW001") || x.F14_DeviceStatus.Equals("1") ||
            //            x.F14_DeviceStatus.Equals("2") || x.F14_UsePermitClass.Equals("1"));

            //return result;            
            var devices = _unitOfWork.DeviceRepository.GetMany(i => i.F14_DeviceCode.Trim().Equals("ATW001") && i.F14_DeviceStatus.Trim() != "1"
                 && i.F14_DeviceStatus.Trim() != "2" && i.F14_UsePermitClass.Trim() != "1");            
            if (!devices.Any())
                return false;
            return true;
        }


        public int FindNumberOfMaterialShelfStatusRecord(string supplierCode)
        {
            // Find all material shelf statuses first.
            var materialShelfStatuses = _unitOfWork.MaterialShelfStatusRepository.GetAll();

            var shelfStatus = Constants.F31_ShelfStatus.SupplierPallet.ToString("D");
            //total [f31_loadamount] of [tx31_mtrshfsts] records whose [f31_shelfstatus] is 2 and [f31_suppliercode] is current [Supplier Code]
            materialShelfStatuses = materialShelfStatuses.Where(x => x.F31_ShelfStatus.Equals(shelfStatus));
            materialShelfStatuses = materialShelfStatuses.Where(x => x.F31_SupplierCode.Equals(supplierCode));

            var count = materialShelfStatuses.Count();
            return count;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find condition matched supplier pallet
        ///     Analyze and process those found record.
        /// </summary>
        /// <param name="supplierCode"></param>
        /// <param name="requestedLoadAmount"></param>
        public void ProcessSupplierPallet(string supplierCode, double requestedLoadAmount, string terminalNo)
        {
            // Find all material shelf status records from database first.
            var materialShelfStatuses = _unitOfWork.MaterialShelfStatusRepository.GetAll();

            // Find the condition matched material shelf status records first.
            materialShelfStatuses = FindMaterialShelfStatuses(materialShelfStatuses, supplierCode);

            // Find all NoManage record from database.
            //var noManages = _unitOfWork.NoManageRepository.GetAll();

            // Check whether there is a [tx48_nomanage] record whose [f48_systemid] = “0000” or not.
            //var noManage = noManages.FirstOrDefault(x => x.F48_SystemId.Equals("0000"));

            // Find current time in the server.
            var currentTime = DateTime.Now;

            // Amount of pallet which has been loaded successfully.
            var loadAmount = 0;

            //For each record in list of found records, the system will perform the following actions in order as a loop
            foreach (var materialShelfStatus in materialShelfStatuses)
            {
                // 	Increase total “load amount” by [f31_loadamount] of selected [tx31_mtrshfsts] record
                if (materialShelfStatus.F31_LoadAmount != null)
                    loadAmount += materialShelfStatus.F31_LoadAmount.Value;
                    //loadAmount = materialShelfStatus.F31_LoadAmount.Value;

                //if that total  is greater than or equal to [Requested Retrieval Quantity], the system will stop the loop
                
                    //if its [F31_LoadAmount] is greater than 0
                    if (materialShelfStatus.F31_LoadAmount > 0)
                    {
                        //	Set [F31_ShelfStatus] as 5
                        materialShelfStatus.F31_ShelfStatus = "5";

                        //Set [F31_TerminalNo] as current application terminal.
                        materialShelfStatus.F31_TerminalNo = terminalNo;

                        //	Set [F31_UpdateDate] as current date and time.
                        materialShelfStatus.F31_UpdateDate = currentTime;

                        //	If there is a [tx48_nomanage] record whose [f48_systemid] = “0000”, the system will:
                        var isNoManage = false;

                        var f48MtrWhsCmdNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                            Constants.GetColumnInNoManager.MtrWhsCmdNo);

                        var strShelfNo = materialShelfStatus.F31_ShelfRow + materialShelfStatus.F31_ShelfBay +
                                         materialShelfStatus.F31_ShelfLevel;
                        var gsCnvCode =
                            _unitOfWork.ConveyorRepository.Get(i => i.F05_TerminalNo.Trim().Equals(terminalNo))
                                .F05_ConveyorCode;

                        InsertTX34(Constants.F34_CommandNo.Retrieval, f48MtrWhsCmdNo,
                            Constants.CmdType.cmdType,
                            Constants.TX34_StrRtrType.SupplierPallet.ToString("D"), Constants.TC_CMDSTS.TC_CMDSTS_0, "",
                            strShelfNo, gsCnvCode, terminalNo, Constants.PictureNo.TCRM131F);
                        
                        _unitOfWork.MaterialShelfStatusRepository.Update(materialShelfStatus);
                    }
                if (loadAmount > requestedLoadAmount)
                {
                    break;
                }

                // send message to c1
                var msgId = "0011";
                var pictureNo = Constants.PictureNo.TCRM131F;

                _notificationService.SendMessageToC1(new
                {
                    msgId,
                    terminalNo,
                    pictureNo
                });
                // Commit the changes.

                break;
            }
            _unitOfWork.Commit();
        }

        /// <summary>
        ///     Find material shelf status records by using supplier code.
        /// </summary>
        /// <param name="materialShelfStatuses" />
        /// <param name="supplierCode"></param>
        /// <returns></returns>
        private IQueryable<TX31_MtrShfSts> FindMaterialShelfStatuses(IQueryable<TX31_MtrShfSts> materialShelfStatuses,
            string supplierCode)
        {
            var shelfStatus = Constants.F31_ShelfStatus.SupplierPallet.ToString("D");
            // SQL Query defined in [3.14.1	UC 30: Retrieve Supplier Pallet]
            materialShelfStatuses =
                materialShelfStatuses.Where(
                    x => x.F31_ShelfStatus.Equals(shelfStatus));
            materialShelfStatuses = materialShelfStatuses.Where(x => x.F31_SupplierCode.Equals(supplierCode));
            materialShelfStatuses = materialShelfStatuses.Where(x => x.F31_CmnShfAgnOrd != null);
            materialShelfStatuses = materialShelfStatuses.OrderBy(x => x.F31_LoadAmount).ThenByDescending(x => x.F31_CmnShfAgnOrd);
            return materialShelfStatuses;
        }

        /// <summary>
        /// Update production command using terminal number.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public IList<FirstCommunicationResponse> UpdateProductCommand(string terminalNo)
        {
            var pendingCommandStatuses = new[] { "6", "7", "8", "9" };

            var materialWarehouseCommands = _unitOfWork.MaterialWarehouseCommandRepository.GetAll();
            materialWarehouseCommands = materialWarehouseCommands.Where(x => x.F34_PictureNo.Equals("TCRM131F", StringComparison.InvariantCultureIgnoreCase) 
            && x.F34_TerminalNo.Equals(terminalNo, StringComparison.InvariantCultureIgnoreCase)
            && pendingCommandStatuses.Contains(x.F34_Status));

            // Initiate items list.
            var items = new List<FirstCommunicationResponse>();

            foreach (var materialWarehouseCommand in materialWarehouseCommands)
            {
                if (string.IsNullOrEmpty(materialWarehouseCommand.F34_Status) || materialWarehouseCommand.F34_Status.Length != 1)
                    continue;

                var newStatus = "";
                var firstCommunicationItem = Mapper.Map<FirstCommunicationResponse>(materialWarehouseCommand);
                firstCommunicationItem.MaterialCode = "";
                if (string.IsNullOrEmpty(firstCommunicationItem.F34_PalletNo))
                    firstCommunicationItem.F34_PalletNo = "";
                firstCommunicationItem.OldStatus = firstCommunicationItem.F34_Status;

                switch (firstCommunicationItem.F34_Status[0])
                {
                    case '6': //Command End
                        newStatus = "C";
                        break;
                    case '7': //Command Cancel
                        newStatus = "D";
                        break;
                    case '8':
                        newStatus = "E";
                        break;
                    case '9':
                        newStatus = "F";
                        break;
                    default:
                        continue;
                }

                firstCommunicationItem.F34_Status = newStatus;
                firstCommunicationItem.F34_UpdateDate = DateTime.Now;
                items.Add(firstCommunicationItem);

                materialWarehouseCommand.F34_Status = newStatus;
                materialWarehouseCommand.F34_UpdateDate = DateTime.Now;
            }

            _unitOfWork.Commit();
            return items;
        }
        #endregion
    }
}