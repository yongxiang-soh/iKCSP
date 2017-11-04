using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Core.Resources;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.PreProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.PreProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.PreProductManagement
{
    public class RetrievalOfPreProductDomain :BaseDomain, IRetrievalOfPreProductDomain
    {
        /// <summary>
        ///     Handles common tasks.
        /// </summary>
        private readonly ICommonDomain _commonDomain;

        /// <summary>
        ///     Service which handles configuration.
        /// </summary>
        private readonly IConfigurationService _configurationService;

        /// <summary>
        ///     Service which handles notification between modules.
        /// </summary>
        private readonly INotificationService _notificationService;

        /// <summary>
        ///     Provides repositories to database.
        /// </summary>
      

        #region Constructor

        /// <summary>
        ///     Initiate domain with IoC.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="notificationService"></param>
        /// <param name="commonDomain"></param>
        /// <param name="configurationService"></param>
        public RetrievalOfPreProductDomain(IUnitOfWork unitOfWork, INotificationService notificationService,
            ICommonDomain commonDomain, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            
            _notificationService = notificationService;
            _commonDomain = commonDomain;
            _configurationService = configurationService;
        }

        #endregion

        public ResponseResult<GridResponse<RetrievalOfPreProductItem>> SearchCriteria(GridSettings gridSettings)
        {
            var notTablet = Constants.F41_Status.NotTablet;
            var containerSet = Constants.F41_Status.ContainerSet;
            var containerSetWait = Constants.F41_Status.ContainerSetWait;
            var containerSetError = Constants.F41_Status.ContainerSetError;
            //find TX41_TbtCmd record with f41_tblcntamt > f41_rtrendcntamt
            var tabletCommands = _unitOfWork
                .TabletCommandRepository
                .GetMany(i => (i.F41_Status.Equals(notTablet) || i.F41_Status.Equals(containerSet) || i.F41_Status.Equals(containerSetWait) || i.F41_Status.Equals(containerSetError)) && (i.F41_TblCntAmt > i.F41_RtrEndCntAmt));


            //find TX42_KndCmd record with f42_outsideclass = tx42_outsidecls_prepdt(0) and f42_status =stored(4)
            var outSiteClass = Constants.F42_OutSideClass.PreProduct.ToString("D");
            var f42_status = Constants.F42_Status.TX42_Sts_Tbtcmd;
            var kneadingCommands =
                _unitOfWork.KneadingCommandRepository.GetMany(i => i.F42_OutSideClass.Equals(outSiteClass) && i.F42_Status.Equals(f42_status));

            //find TM03_PreProduct records 
            var preProducts = _unitOfWork.PreProductRepository.GetAll();

            var results = from tabletCommand in tabletCommands
                from kneadingCommand in kneadingCommands
                from preProduct in preProducts
                where tabletCommand.F41_KndCmdNo.Trim().Equals(kneadingCommand.F42_KndCmdNo.Trim()) &&
                      tabletCommand.F41_PrePdtLotNo.Trim().Equals(kneadingCommand.F42_PrePdtLotNo.Trim()) &&
                      tabletCommand.F41_PreproductCode.Trim().Equals(kneadingCommand.F42_PreProductCode.Trim()) &&
                      tabletCommand.F41_PreproductCode.Trim().Equals(preProduct.F03_PreProductCode.Trim())
                orderby
                    tabletCommand.F41_KndCmdNo,
                    kneadingCommand.F42_LotSeqNo
                select new RetrievalOfPreProductItem
                {
                    F41_KndCmdNo = tabletCommand.F41_KndCmdNo,
                    F41_PreproductCode = tabletCommand.F41_PreproductCode,
                    PreProductName = preProduct.F03_PreProductName,
                    F41_PrePdtLotNo = tabletCommand.F41_PrePdtLotNo,
                    ThrowAmount = kneadingCommand.F42_ThrowAmount,
                    F41_TabletLine = tabletCommand.F41_TabletLine,
                    F41_TblCntAmt = tabletCommand.F41_TblCntAmt,
                    F41_RtrEndCntAmt = tabletCommand.F41_RtrEndCntAmt,
                    LotSeqNo = kneadingCommand.F42_LotSeqNo,
                    Line = tabletCommand.F41_TabletLine.Substring(tabletCommand.F41_TabletLine.Trim().Length - 2, 2),
                    ContQuanty = tabletCommand.F41_TblCntAmt - tabletCommand.F41_RtrEndCntAmt
                };

            var itemCount = results.Count();

            // Sort and paging
            OrderByAndPaging(ref results, gridSettings);

            var lstResult =
                Mapper.Map<IEnumerable<TX41_TbtCmd>, IEnumerable<RetrievalOfPreProductItem>>(
                    results);

            var resultModel = new GridResponse<RetrievalOfPreProductItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<RetrievalOfPreProductItem>>(resultModel, true);
        }

        //BR28  Retrieving Container rules:
        public ResponseResult Edit(string commandNo, string preProductCode, string lotNo, string tabletisingLine,
            string terminalNo)
        {
            //Get and update tx41_tbtcmd
            var tabletCommand =
                _unitOfWork.TabletCommandRepository.GetMany(
                    i => i.F41_KndCmdNo.Trim().Equals(commandNo) && i.F41_PrePdtLotNo.Trim().Equals(lotNo))
                    .FirstOrDefault();

            if (tabletCommand != null)
            {
                tabletCommand.F41_TabletLine ="TAB0" + tabletisingLine;
                tabletCommand.F41_UpdateDate = DateTime.Now;
                _unitOfWork.TabletCommandRepository.Update(tabletCommand);
            }

            //Get data for tx49_prepdtshfstk  
            var shelfStatus = Constants.F49_ShelfStatus.TX49_StkFlg_Stk;
            var preProductShelfStocks =
                _unitOfWork.PreProductShelfStockRepository.GetMany(
                    i =>
                        i.F49_KndCmdNo.Trim().Equals(commandNo.Trim()) &&
                        i.F49_PreProductCode.Trim().Equals(preProductCode.Trim()) &&
                        i.F49_PreProductLotNo.Trim().Equals(lotNo.Trim()) &&
                        i.F49_ShelfStatus.Trim().Equals(shelfStatus))
                    .OrderBy(i => i.F49_ContainerSeqNo);

            //Declaration lsShelf and 	isAddress 
            var lsShelf = "";
            var isAddress = "";
            if (!preProductShelfStocks.Any())
                return new ResponseResult(false);
            foreach (var preProductShelfStock in preProductShelfStocks)
            {
                //Update status into TX49
                preProductShelfStock.F49_ShelfStatus = Constants.F49_ShelfStatus.TX49_StkFlg_Rtr;
                _unitOfWork.PreProductShelfStockRepository.Update(preProductShelfStock);

                var containerCode = preProductShelfStock.F49_ContainerCode;
                var lstPrePdtShfSts =
                    _unitOfWork.PreProductShelfStatusRepository.GetMany(
                        i =>
                            i.F37_ContainerCode.Trim().Equals(containerCode.Trim()));

                //	ls_Shelf = [as_Row] + [as_Bay] + [as_Level]
                var prePdtStsItem = lstPrePdtShfSts.FirstOrDefault();
                if (prePdtStsItem != null)
                {
                    lsShelf = prePdtStsItem.F37_ShelfRow + prePdtStsItem.F37_ShelfBay +
                              prePdtStsItem.F37_ShelfLevel;

                    foreach (var prePdtShfSts in lstPrePdtShfSts)
                    {
                        prePdtShfSts.F37_ShelfStatus =
                            Constants.F37_ShelfStatus.ReservedForRetrieval.ToString("D");
                        _unitOfWork.PreProductShelfStatusRepository.Update(prePdtShfSts);

                        break;
                    }
                }
                
                else
                {
                   return new ResponseResult(false);
                }

                //The system will check the status of the Conveyor and Pre-product Warehouse as BR 8

                //Insert Or Update tx48_nomanage
                var isNoManage = true;
                var cmdSeqNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                    Constants.GetColumnInNoManager.PrePdtWhsCmdNo, 0, 1, 0, 0, 0).ToString("D4");

                var f50commandNo = Constants.F50_CommandNo.CmdNoRtr.ToString("D");
                var tx50PrePdtWhsCmd =
                    _unitOfWork.PreProductWarehouseCommandRepository.Get(
                        i =>
                            i.F50_CommandNo.Trim().Equals(f50commandNo) &&
                            i.F50_CmdSeqNo.Trim().Equals(cmdSeqNo.Trim()));

                //Insert tx50_prepdtwhscmd
                if (tx50PrePdtWhsCmd == null)
                {
                    var prePdtWhsCmd = new TX50_PrePdtWhsCmd();
                    prePdtWhsCmd.F50_CommandNo =Constants.F50_CommandNo.CmdNoRtr.ToString("D");
                    prePdtWhsCmd.F50_CmdSeqNo = cmdSeqNo;
                    prePdtWhsCmd.F50_CommandType = Constants.CommandType.CmdType_0;
                    prePdtWhsCmd.F50_StrRtrType = Constants.F50_StrRtrType.StrRtrType_PrePdt.ToString("D");
                    prePdtWhsCmd.F50_Status = Constants.TC_CMDSTS.TC_CMDSTS_0;
                    prePdtWhsCmd.F50_ContainerNo = preProductShelfStock.F49_ContainerNo;
                    prePdtWhsCmd.F50_ContainerCode = preProductShelfStock.F49_ContainerCode;
                    prePdtWhsCmd.F50_Priority = 0;
                    prePdtWhsCmd.F50_From = lsShelf;
                    prePdtWhsCmd.F50_To = _commonDomain.FindConveyor(terminalNo).F05_ConveyorCode;
                    prePdtWhsCmd.F50_CommandSendDate = DateTime.Now;
                    prePdtWhsCmd.F50_CommandEndDate = DateTime.Now;
                    prePdtWhsCmd.F50_TerminalNo = terminalNo;
                    prePdtWhsCmd.F50_PictureNo = Constants.PictureNo.TCIP031F;
                    prePdtWhsCmd.F50_AbnormalCode = null;
                    prePdtWhsCmd.F50_AddDate = DateTime.Now;
                    prePdtWhsCmd.F50_UpdateDate = DateTime.Now;
                    prePdtWhsCmd.F50_UpdateCount = 0;
                    prePdtWhsCmd.F50_RetryCount = 0;
                    _unitOfWork.PreProductWarehouseCommandRepository.Add(prePdtWhsCmd);
                }
                

               // var lsMsg = "Shelf No [" + isAddress + "] retrieving...";
                //_notificationService.SendMessageToC2(lsMsg);
            }
            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        //UC 9: Receive message from C2 server for retrieving Container 
        public IList<SecondCommunicationResponse> ReceiveMessageFromC2(string terminalNo, string kndCmdNo, string prepdtLotNo,
            string preProductCode)
        {
            //var response = new C2Response();

            var cmdsts6 = Constants.TC_CMDSTS.TC_CMDSTS_6;
            var cmdsts7 = Constants.TC_CMDSTS.TC_CMDSTS_7;
            var cmdsts9 = Constants.TC_CMDSTS.TC_CMDSTS_9;
            var pictureNo = Constants.PictureNo.TCIP031F;
            var newStatus = "";

            //The system retrieve all the Command No. whose Terminal No. is the current terminal and the Screen ID is “TCIP031F” 
            var lstPrePdtWhsCmd =
                _unitOfWork.PreProductWarehouseCommandRepository.GetMany(
                    i =>
                        i.F50_TerminalNo.Trim().Equals(terminalNo) && i.F50_PictureNo.Trim().Equals(pictureNo) &&
                        (i.F50_Status.Equals(cmdsts6) || i.F50_Status.Equals(cmdsts7) ||
                         i.F50_Status.Equals(cmdsts9)))
                    .OrderBy(i => i.F50_AddDate);
            //If record found, for each record of retrieved Command, the system will retrieve the info of kneading command 

            var status = "";
            var items =new List<SecondCommunicationResponse>();

            foreach (var prePdtWhsCmd in lstPrePdtWhsCmd)
            {
                var item = Mapper.Map<SecondCommunicationResponse>(prePdtWhsCmd);
                item.OldStatus = prePdtWhsCmd.F50_Status;
                var containerCode = prePdtWhsCmd.F50_ContainerCode;

                switch (prePdtWhsCmd.F50_Status[0])
                {
                    case '6':
                        status = Constants.TC_CMDSTS.TC_CMDSTS_C;
                        break;
                    case '7':
                        status = Constants.TC_CMDSTS.TC_CMDSTS_D;
                        break;
                    case '9':
                        status = Constants.TC_CMDSTS.TC_CMDSTS_F;
                        break;
                }
                prePdtWhsCmd.F50_Status = status;
                prePdtWhsCmd.F50_UpdateDate = DateTime.Now;
                item.PreProductCode = preProductCode;
                _unitOfWork.PreProductWarehouseCommandRepository.Update(prePdtWhsCmd);

                items.Add(item);

                _unitOfWork.PreProductWarehouseCommandRepository.Update(prePdtWhsCmd);

                //After updating “tx50_ PrePdtWhsCmd” table completed, the system will retrieve the data of the Kneading Command for the Container 
                var prePdtShfStk =
                    _unitOfWork.PreProductShelfStockRepository.GetMany(
                        i => i.F49_ContainerCode.Trim().Equals(containerCode.Trim())).FirstOrDefault();

                if (item.OldStatus == Constants.TC_CMDSTS.TC_CMDSTS_6)
                {
                    var amount = 0;
                    wf_writedb_cont_qty(kndCmdNo, prepdtLotNo, ref amount);
                    if (amount > 1)
                    {
                        if ((kndCmdNo != prePdtShfStk.F49_KndCmdNo.Trim()) ||
                            (prepdtLotNo != prePdtShfStk.F49_PreProductLotNo.Trim()))
                            continue;
                        Assign49(kndCmdNo, preProductCode, prepdtLotNo);
                        var shelfNo = "";
                        Assign37(prePdtWhsCmd.F50_ContainerCode, terminalNo, ref shelfNo);

                        Instab(terminalNo, shelfNo, prePdtWhsCmd.F50_ContainerNo, prePdtWhsCmd.F50_ContainerCode);
                    }
                }
            }

            _unitOfWork.Commit();
            return items;
        }

        public ResponseResult CheckConveyor(string terminerNo)
        {
            //return _commonDomain.CheckingStatusOfConveyorAndPreproductWarehouse(terminerNo);
            var conveyor = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminerNo);
            if (conveyor == null)
                return new ResponseResult(false, "MSG13");
            var deviceCode = "";
            switch (conveyor.F05_StrRtrSts)
            {
                case "0":
                    deviceCode = _configurationService.MaterialDeviceCode;
                    break;
                case "1":
                    deviceCode = _configurationService.PreProductDeviceCode;
                    break;
                case "2":
                    deviceCode = _configurationService.ProductDeviceCode;

                    break;
                case "9":
                    return new ResponseResult(false, "MSG13");
            }

            var device = _unitOfWork.DeviceRepository.GetById(deviceCode);
            if (device== null)
                return new ResponseResult(false, "MSG14");
            if (
                (device.F14_DeviceStatus != Constants.F14_DeviceStatus.Online.ToString("d")) ||
                (device.F14_UsePermitClass == Constants.F14_UsePermitClass.Prohibited.ToString("d")))
                return new ResponseResult(false, "MSG14");
            return new ResponseResult(true);
        }

        private bool wf_writedb_cont_qty(string kndCmdNo, string prepdtLotNo, ref int as_remain_cont)
        {
            var tx41 =
                _unitOfWork.TabletCommandRepository.GetMany(
                    i => (i.F41_KndCmdNo == kndCmdNo) && (i.F41_PrePdtLotNo == prepdtLotNo)).FirstOrDefault();
            if (tx41 != null)
            {
                as_remain_cont = tx41.F41_TblCntAmt - tx41.F41_RtrEndCntAmt;
                tx41.F41_RtrEndCntAmt += 1;
                _unitOfWork.TabletCommandRepository.Update(tx41);
                return true;
            }
            return false;
        }

        private bool Assign49(string as_kno, string as_pcode, string as_lotno)
        {
            var lstTx49 =
                _unitOfWork.PreProductShelfStockRepository.GetMany(
                    i =>
                        (i.F49_KndCmdNo == as_kno) && (i.F49_PreProductCode == as_pcode) &&
                        (i.F49_PreProductLotNo == as_lotno) && (i.F49_ShelfStatus == "3"));
            if (lstTx49.Any())
            {
                foreach (var tx49 in lstTx49)
                {
                    tx49.F49_ShelfStatus = "2";
                    _unitOfWork.PreProductShelfStockRepository.Update(tx49);
                }

                return true;
            }
            return false;
        }

        private bool Assign37(string containerCode, string terminalNo, ref string shelfNo)
        {
// assign tx37
            var lstTx37 = _unitOfWork.PreProductShelfStatusRepository.GetMany(i => i.F37_ContainerCode == containerCode);
            if (!lstTx37.Any())
                return false;
            foreach (var tx37 in lstTx37)
            {
                shelfNo = tx37.F37_ShelfRow + tx37.F37_ShelfBay + tx37.F37_ShelfLevel;
                tx37.F37_ShelfStatus = Constants.F37_ShelfStatus.ReservedForRetrieval.ToString("D");
                tx37.F37_TerminalNo = terminalNo;
                _unitOfWork.PreProductShelfStatusRepository.Update(tx37);

                break;
            }
            return true;
        }

        private bool Instab(string terminalNo, string as_shelf, string as_conno, string as_concode)
        {
            var nomanage = true;
            var serialNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref nomanage,
                Constants.GetColumnInNoManager.PrePdtWhsCmdNo, 0, 1, 0, 0, 0);
            var conveyCode = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo);
            var tx50 = new TX50_PrePdtWhsCmd
            {
                F50_CommandNo = Constants.F50_CommandNo.CmdNoRtr.ToString("D"),
                F50_CmdSeqNo = serialNo.ToString("D4"),
                F50_CommandType = Constants.CommandType.CmdType_0,
                F50_StrRtrType = "0",
                F50_Status = "0",
                F50_ContainerNo = as_conno,
                F50_ContainerCode = as_concode,
                F50_Priority = 0,
                F50_From = as_shelf,
                F50_To = conveyCode.F05_ConveyorCode,
                F50_CommandSendDate = DateTime.Now,
                F50_CommandEndDate = DateTime.Now,
                F50_TerminalNo = terminalNo,
                F50_PictureNo = Constants.PictureNo.TCIP023F,
                F50_AbnormalCode = null,
                F50_AddDate = DateTime.Now,
                F50_UpdateDate = DateTime.Now,
                F50_UpdateCount = 0,
                F50_RetryCount = 0
            };
            _unitOfWork.PreProductWarehouseCommandRepository.Add(tx50);
            return true;
        }
    }
}