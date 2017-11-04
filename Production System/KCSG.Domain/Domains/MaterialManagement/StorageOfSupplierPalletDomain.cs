using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.jsGrid.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;

namespace KCSG.Domain.Domains.MaterialManagement
{
    public class StorageOfSupplierPalletDomain : BaseDomain,IStorageOfSupplierPalletDomain
    {

        private readonly INotificationService _notificationService;
        #region Contructor Declaration
        public StorageOfSupplierPalletDomain(IUnitOfWork unitOfWork, INotificationService notificationService, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
        }
        #endregion

        public ResponseResult<GridResponse<MaterialShelfStatusItem>> SearchCriteria(string supplierCode, int maxPallet, GridSettings gridSettings)
        {
            var result = _unitOfWork.MaterialShelfStatusRepository.GetAll();

            //if (!string.IsNullOrEmpty(supplierCode))
            //{
                var shelfStatusSupplierPallet = Constants.F31_ShelfStatus.SupplierPallet.ToString("D");
                var shelfStatusWarehousePallet = Constants.F31_ShelfStatus.WarehousePallet.ToString("D");
                result = result.Where(i => i.F31_SupplierCode.Trim().ToUpper().Equals(supplierCode.Trim().ToUpper()) && i.F31_ShelfStatus.Trim().Equals(shelfStatusSupplierPallet)
                                        && i.F31_LoadAmount < maxPallet
                                        || i.F31_ShelfStatus.Equals(shelfStatusWarehousePallet)
                                        && i.F31_CmnShfAgnOrd != null);
            //}

            // Sort and paging
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var lstResult = Mapper.Map<IEnumerable<TX31_MtrShfSts>, IEnumerable<MaterialShelfStatusItem>>(result.ToList());
            var resultModel = new GridResponse<MaterialShelfStatusItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<MaterialShelfStatusItem>>(resultModel, true);
        }


        public bool CheckConveyor(string terminalNo)
        {
            var strRtrStsField = Constants.F05_StrRtrSts.Error.ToString();
            var result = _unitOfWork.ConveyorRepository.GetAll().Any(i => i.F05_TerminalNo.Trim().Equals(terminalNo.Trim()) || i.F05_StrRtrSts == strRtrStsField);
            if (result)
                return true;
            return false;
        }

        public bool CheckDevice()
        {
            var deviceCode = Constants.DeviceCode.ATW001.ToString();
            var deviceStatusOffline = Constants.F14_DeviceStatus.Offline.ToString();
            var deviceStatusError = Constants.F14_DeviceStatus.Error.ToString();
            var userPermitClass=Constants.F14_UsePermitClass.Prohibited.ToString();

            var result = _unitOfWork.DeviceRepository.GetAll().Any(i => i.F14_DeviceCode.Trim().Equals(deviceCode) || i.F14_DeviceStatus.Equals(deviceStatusOffline) || i.F14_DeviceStatus.Equals(deviceStatusError) || i.F14_UsePermitClass.Equals(userPermitClass));
            if (result)
                return true;
            return false;
        }
        public ResponseResult CreateOrUpdate(string shelfRow, string shelfBay, string shelfLevel,string terminalNo)
        {
            var mtrShfStsItem = _unitOfWork.MaterialShelfStatusRepository.GetMany(i => i.F31_ShelfRow.Trim().Equals(shelfRow) && i.F31_ShelfBay.Trim().Equals(shelfBay) && i.F31_ShelfLevel.Trim().Equals(shelfLevel)).FirstOrDefault();
            if (CheckConveyor(terminalNo) && CheckDevice())
            {

                var isNoManage = false;
                var f48_MtrWhsCmdNo = CreateOrUpdateTX48(ref isNoManage);

                //Get [Ls_Status] is 2 if [f31_shelfstatus] of the selected record is 2. If not, [Ls_Status] is 1.
                
               var  lsStatus = mtrShfStsItem.F31_ShelfStatus == Constants.F31_ShelfStatus.SupplierPallet.ToString("D")
                    ? Constants.TX34_StrRtrType.SupplierPallet.ToString("D")
                    : Constants.TX34_StrRtrType.WarehousePallet.ToString("D");

               //[As_From] = [f31_shelfrow] + [f31_shelfbay] + [f31_shelflevel]. In which, [f31_shelfrow], [f31_shelfbay] and [f31_shelflevel] are corresponding field of [tx31_mtrshfsts] record.
                var asFrom = shelfRow + shelfBay + shelfLevel;

                //Get value of field [f05_conveyorcode] in [tm05_conveyor] record whose [f05_terminalno] is current application terminal.
                var conveyorItem =
                    _unitOfWork.ConveyorRepository.GetMany(i => i.F05_TerminalNo.Trim().Equals(terminalNo))
                        .FirstOrDefault();

                var asToCv = conveyorItem.F05_ConveyorCode;

                //Insert data into tx34 table
                InsertTX34(Constants.F34_CommandNo.Retrieval,f48_MtrWhsCmdNo, Constants.CmdType.cmdType,
                 lsStatus, Constants.TC_CMDSTS.TC_CMDSTS_0, "", asFrom, asToCv, terminalNo,
                    Constants.PictureNo.TCRM121F);
              
                //Update TX31_MtrShfSts
                mtrShfStsItem.F31_ShelfStatus = Constants.F31_ShelfStatus.ReservedForRetrieval.ToString("D");
                mtrShfStsItem.F31_UpdateDate = DateTime.Now;
                mtrShfStsItem.F31_TerminalNo = terminalNo;
                _unitOfWork.MaterialShelfStatusRepository.Update(mtrShfStsItem);

            }
            else{
                //TODO: Show message error
            }

            //Send message to c1
            var msgId = "0011";
            var pictureNo = Constants.PictureNo.TCRM121F;

            _notificationService.SendMessageToC1(new
            {
                msgId,
                terminalNo,
                pictureNo
            });
            _unitOfWork.Commit();
            return new ResponseResult(true);
        }


        public bool CheckDeviceStatus()
        {
            var offlineStatus = Constants.F14_DeviceStatus.Offline.ToString("D");
            var errorStatus = Constants.F14_DeviceStatus.Error.ToString("D");
            var usepermit = Constants.F14_UsePermitClass.Prohibited.ToString("D");
            var lstDevice = _unitOfWork.DeviceRepository.GetMany(
                i =>
                    i.F14_DeviceCode.Trim().Equals(Constants.DeviceCode.ATW001) ||
                    i.F14_DeviceStatus.Equals(offlineStatus) || i.F14_DeviceStatus.Equals(errorStatus) ||
                    i.F14_UsePermitClass.Equals(usepermit));
           
           
            if (lstDevice.Any() )
                return true;
            return false;

        }

        public bool CheckConveyorStatus(string terminalNo)
        {
            var error = Constants.F05_StrRtrSts.Error.ToString("D");
            var conveyor = _unitOfWork.ConveyorRepository.GetMany(i => i.F05_TerminalNo.Trim().Equals(terminalNo) && i.F05_StrRtrSts.Equals(error));
            return !conveyor.Any();
        }


        public IList<FirstCommunicationResponse> StorageOfSupplierPalletMessageC1Reply(string terminalNo)
        {
            var pictureNo = Constants.PictureNo.TCRM121F;
            var status1 = Constants.F34_Status.status6;
            var status2 = Constants.F34_Status.status7;
            var status3 = Constants.F34_Status.status9;

            var items = new List<FirstCommunicationResponse>();
            var materialWarehouseCommands =
                _unitOfWork.MaterialWarehouseCommandRepository.GetMany(
                    i =>
                        i.F34_TerminalNo.Trim().Equals(terminalNo.Trim()) && i.F34_PictureNo.Equals(pictureNo) &&
                        (i.F34_Status.Equals(status1) || i.F34_Status.Equals(status2)) || i.F34_Status.Equals(status3)).OrderBy(i => i.F34_AddDate);
            foreach (var materialWarehouseCommand in materialWarehouseCommands)
            {
                var item = Mapper.Map<FirstCommunicationResponse>(materialWarehouseCommand);
                item.OldStatus = materialWarehouseCommand.F34_Status;
                //item.MaterialCode=materialWarehouseCommand.c
                switch (materialWarehouseCommand.F34_Status)
                {
                    case "6":
                        materialWarehouseCommand.F34_Status = Constants.F34_Status.statusC;
                        break;
                    case "7":
                        materialWarehouseCommand.F34_Status = Constants.F34_Status.statusD;
                        break;
                    case "9":
                        materialWarehouseCommand.F34_Status = Constants.F34_Status.statusE;
                        break;
                }
                materialWarehouseCommand.F34_UpdateDate = DateTime.Now;

                var lsRow = materialWarehouseCommand.F34_From.Substring(0, 2);
                var lsBay = materialWarehouseCommand.F34_From.Substring(2, 2);
                var lsLevel= materialWarehouseCommand.F34_From.Substring(4, 2);

                var tx31mtrshfsts =
                    _unitOfWork.MaterialShelfStatusRepository.Get(
                        i =>
                            i.F31_ShelfRow.Equals(lsRow) && i.F31_ShelfBay.Equals(lsBay) &&
                            i.F31_ShelfLevel.Equals(lsLevel));
                var materialCode = "";
                var lsPalletno = "";
                if (tx31mtrshfsts != null)
                {
                    lsPalletno = tx31mtrshfsts.F31_PalletNo;
                    var tx33MtrShfStk =
                        _unitOfWork.MaterialShelfStockRepository.Get(
                            i => i.F33_PalletNo.Trim().Equals(tx31mtrshfsts.F31_PalletNo.Trim()));

                    if (tx33MtrShfStk!=null)
                        materialCode = tx33MtrShfStk.F33_MaterialCode;
                }
                item.MaterialCode = materialCode;
                item.F34_PalletNo = lsPalletno;
                items.Add(item);
            }
            _unitOfWork.Commit();
            return items;
        }


        public void DetailStorage(string shelfNo, string supplierCode,int stackedPallet, int inCrementPallet,string terminalNo)
        {
            var shelfRow = shelfNo.Replace("~", "").Substring(0, 2);
            var shelfBay = shelfNo.Replace("~", "").Substring(2, 2);
            var shelfLevel = shelfNo.Replace("~", "").Substring(4, 2);
            var f31Status = Constants.F31_ShelfStatus.EmptyShelf.ToString("D");


            //get record in tx31
            var tX31MtrShfSts =
                _unitOfWork.MaterialShelfStatusRepository.GetMany(
                    i => i.F31_ShelfStatus.Equals(f31Status) && i.F31_CmnShfAgnOrd != null)
                    .OrderBy(i => i.F31_CmnShfAgnOrd);

            var lsShelfNo = "";
            foreach (var item in tX31MtrShfSts)
            {
                item.F31_ShelfStatus = Constants.F31_ShelfStatus.ReservedForStorage.ToString("D");
                item.F31_LoadAmount = stackedPallet + inCrementPallet;
                item.F31_SupplierCode = supplierCode;
                item.F31_StorageDate = DateTime.Now;
                item.F31_UpdateDate = DateTime.Now;
                item.F31_TerminalNo = terminalNo;
                _unitOfWork.MaterialShelfStatusRepository.Update(item);
                lsShelfNo = item.F31_ShelfRow + item.F31_ShelfBay + item.F31_ShelfLevel;

                break;
            }
            var isNoManage = false;
            var f48_MtrWhsCmdNo = CreateOrUpdateTX48(ref isNoManage);

            //Get value of field [f05_conveyorcode] in [tm05_conveyor] record whose [f05_terminalno] is current application terminal.
            var conveyorItem =
                _unitOfWork.ConveyorRepository.GetMany(i => i.F05_TerminalNo.Trim().Equals(terminalNo))
                    .FirstOrDefault();

            var asToCv = conveyorItem.F05_ConveyorCode;
            //insert tx34
            InsertTX34(Constants.F34_CommandNo.Storage, f48_MtrWhsCmdNo, Constants.CmdType.cmdType,
                 Constants.TX34_StrRtrType.SupplierPallet.ToString("D"), Constants.TC_CMDSTS.TC_CMDSTS_0, "", asToCv, lsShelfNo, terminalNo,
                    Constants.PictureNo.TCRM122F);

            _unitOfWork.Commit();

        }

        public IList<FirstCommunicationResponse> DetailStorageOfSupplierPalletMessageC1Reply(string terminalNo)
        {
            var pictureNo = Constants.PictureNo.TCRM122F;
            var status1 = Constants.F34_Status.status6;
            var status2 = Constants.F34_Status.status7;
            var status3 = Constants.F34_Status.status8;

            var items = new List<FirstCommunicationResponse>();
            var materialWarehouseCommands =
                _unitOfWork.MaterialWarehouseCommandRepository.GetMany(
                    i =>
                        i.F34_TerminalNo.Trim().Equals(terminalNo.Trim()) && i.F34_PictureNo.Equals(pictureNo) &&
                        (i.F34_Status.Equals(status1) || i.F34_Status.Equals(status2)) || i.F34_Status.Equals(status3)).OrderBy(i => i.F34_AddDate);
            foreach (var materialWarehouseCommand in materialWarehouseCommands)
            {
                var item = Mapper.Map<FirstCommunicationResponse>(materialWarehouseCommand);
                item.OldStatus = materialWarehouseCommand.F34_Status;
                //item.MaterialCode=materialWarehouseCommand.c
                switch (materialWarehouseCommand.F34_Status)
                {
                    case "6":
                        materialWarehouseCommand.F34_Status = Constants.F34_Status.statusC;
                        break;
                    case "7":
                        materialWarehouseCommand.F34_Status = Constants.F34_Status.statusD;
                        break;
                    case "8":
                        materialWarehouseCommand.F34_Status = Constants.F34_Status.statusB;
                        break;
                }
                materialWarehouseCommand.F34_UpdateDate = DateTime.Now;

                var lsRow = materialWarehouseCommand.F34_From.Substring(0, 2);
                var lsBay = materialWarehouseCommand.F34_From.Substring(2, 2);
                var lsLevel = materialWarehouseCommand.F34_From.Substring(4, 2);

                var tx31mtrshfsts =
                    _unitOfWork.MaterialShelfStatusRepository.Get(
                        i =>
                            i.F31_ShelfRow.Equals(lsRow) && i.F31_ShelfBay.Equals(lsBay) &&
                            i.F31_ShelfLevel.Equals(lsLevel));
                var lsPalletno = "";
                if (tx31mtrshfsts != null)
                    lsPalletno = tx31mtrshfsts.F31_PalletNo;
                item.F34_PalletNo = lsPalletno;
                items.Add(item);
            }
            _unitOfWork.Commit();
            return items;
        }


    }
}
