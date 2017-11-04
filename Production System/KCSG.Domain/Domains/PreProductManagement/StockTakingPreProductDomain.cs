using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.VariantTypes;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.PreProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.PreProductManagement;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;
using log4net;

namespace KCSG.Domain.Domains.PreProductManagement
{
    public class StockTakingPreProductDomain : IStockTakingPreProductDomain
    {
        #region 

        /// <summary>
        /// Provides repostories to access to database.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        private IConfigurationService _configurationService;


        /// <summary>
        /// Handles common tasks.
        /// </summary>
        private readonly ICommonDomain _commonDomain;

        /// <summary>
        /// Service which handles notifications.
        /// </summary>
        private readonly INotificationService _notificationService;

        private readonly ILog _log;

        /// <summary>
        ///     Initiate controller with dependency injections.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="notificationService"></param>
        /// <param name="commonDomain"></param>
        public StockTakingPreProductDomain(IUnitOfWork unitOfWork,
            INotificationService notificationService,
            ICommonDomain commonDomain, IConfigurationService configurationService,
            ILog log)
        {

            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _commonDomain = commonDomain;
            _log = log;
            _configurationService = configurationService;
        }
        #endregion
        #region method Screen TCIP041F

        /// <summary>
        ///     Find stock taking pre-product by using specific conditions.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <param name="fromRow"></param>
        /// <param name="fromBay"></param>
        /// <param name="fromLevel"></param>
        /// <param name="toRow"></param>
        /// <param name="toBay"></param>
        /// <param name="toLevel"></param>
        /// <returns></returns>
        public ResponseResult<GridResponse<object>> SearchStockTakingPreProduct(GridSettings gridSettings,
            int fromRow, int fromBay, int fromLevel,
            int toRow, int toBay, int toLevel)
        {
            // Calculate the shelf no (start - end)
            var shelfStart = fromRow + fromBay + fromLevel;
            var shelfEnd = toRow + toBay + toLevel;
            // TODO: Move this variable to constant.
            var tx37_stktkgflg_invnotchk = "0";
            var tx37_shfsts_stk = "3";

            // Find all stock taking pre-product items.
            var results = _unitOfWork.Context.FindStockTakingPreProduct(shelfStart, shelfEnd, tx37_stktkgflg_invnotchk,
                tx37_shfsts_stk);

            // Find total items received.
            var total = results.Count();

            var pageIndex = gridSettings.PageIndex - 1;
            if (pageIndex < 0)
                pageIndex = 0;

            var items = _unitOfWork.Context.FindStockTakingPreProduct(shelfStart, shelfEnd, tx37_stktkgflg_invnotchk,
                    tx37_shfsts_stk).Skip(pageIndex * gridSettings.PageSize)
                .Take(gridSettings.PageSize)
                .ToList()
                .Select(x => new SearchStockTakingPreProductItem
                {
                    F37_ShelfRow = x.f37_shelfrow,
                    F37_ShelfBay = x.f37_shelfbay,
                    F37_ShelfLevel = x.f37_shelflevel,
                    F49_PreProductCode = x.f49_preproductcode,
                    F03_PreProductName = x.f03_preproductname,
                    F49_PreProductLotNo = x.f49_preproductlotno,
                    F49_Amount = x.f49_amount,
                    F37_ContainerType = x.f37_containertype,
                    F49_ContainerCode = x.f49_containercode,
                    F37_UpdateDate = x.f37_updatedate,
                    F37_ContainerNo = x.f37_containerno,
                    F49_KneadingCommandNo = x.f49_kndcmdno
                });

            // Build a grid and respond to client.
            var resultModel = new GridResponse<object>(items, total);
            return new ResponseResult<GridResponse<object>>(resultModel, true);
        }

        /// <summary>
        ///     Retrieve stock taking of pre-product
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="preProductCode"></param>
        /// <param name="lsRow"></param>
        /// <param name="lsBay"></param>
        /// <param name="lsLevel"></param>
        /// <param name="containerCode"></param>
        /// <returns></returns>
        public ResponseResult<string> RetrieveStockTakingOfPreProduct(string terminalNo, string preProductCode,
            string lsRow, string lsBay, string lsLevel, string containerCode, string containerNo, DateTime? updateDate)
        {

            // TODO: Move this to constant
            var tx37_shfsts_stk = "3";

            // Find all conveyors.
            var conveyors = _unitOfWork.ConveyorRepository.GetAll();
            var errorMessage = "";
            // Find all pre-products.
            var preProducts = _unitOfWork.PreProductRepository.GetAll();
            var convcode = "";
            var result = (from conveyor in conveyors
                          from preProduct in preProducts
                          where conveyor.F05_TerminalNo.Equals(terminalNo.Trim())
                                && conveyor.F05_LineNo.Trim().Equals(preProduct.F03_KneadingLine.Trim())
                                && conveyor.F05_ColorClass.Trim().Equals(preProduct.F03_ColorClass.Trim())
                                && preProduct.F03_PreProductCode.Trim().Equals(preProductCode.Trim())
                          select conveyor.F05_ConveyorCode).FirstOrDefault();

            if (result == null)
            {
                convcode = "CV211";
                errorMessage = "MSG13";
            }
            else
            {
                convcode = result;
            }
            var checkConveyor = CheckConveyorByCode(convcode);
            if (!checkConveyor.IsSuccess)
            {
                return new ResponseResult<string>(null, false, checkConveyor.ErrorMessages[0]);
            }
            var systemTime = DateTime.Now;

            // Find TX37 Record
            var prepdtshfstses = _unitOfWork.PreProductShelfStatusRepository.GetAll();
            prepdtshfstses = prepdtshfstses.Where(x => x.F37_ShelfRow.Equals(lsRow));
            prepdtshfstses = prepdtshfstses.Where(x => x.F37_ShelfBay.Equals(lsBay));
            prepdtshfstses = prepdtshfstses.Where(x => x.F37_ShelfLevel.Equals(lsLevel));
            prepdtshfstses = prepdtshfstses.Where(x => x.F37_ShelfStatus.Equals(tx37_shfsts_stk));
            if (!prepdtshfstses.Any())
            {
                errorMessage = "MSG28";
            }
            foreach (var prepdtshfsts in prepdtshfstses)
            {
                prepdtshfsts.F37_ShelfStatus = Constants.F37_ShelfStatus.ReservedForRetrieval.ToString("D");
                prepdtshfsts.F37_TerminalNo = terminalNo;
                prepdtshfsts.F37_UpdateDate = systemTime;
                _unitOfWork.PreProductShelfStatusRepository.Update(prepdtshfsts);

                break;
            }

            var prepdtshfstks = _unitOfWork.PreProductShelfStockRepository.GetAll();
            prepdtshfstks = prepdtshfstks.Where(x => x.F49_ContainerCode.Equals(containerCode));
            if (!prepdtshfstks.Any())
            {
                errorMessage = "MSG28";
            }
            if (!string.IsNullOrEmpty(errorMessage) && errorMessage == "MSG28")
            {
                return new ResponseResult<string>(null, false, errorMessage);
            }
            foreach (var prepdtshfstk in prepdtshfstks)
            {
                prepdtshfstk.F49_ShelfStatus = Constants.F49_ShelfStatus.TX49_StkFlg_Rtr;
                _unitOfWork.PreProductShelfStockRepository.Update(prepdtshfstk);
            }

            var isnoManager = true;
            try
            {
                var as_cmdno = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isnoManager,
                Constants.GetColumnInNoManager.PrePdtWhsCmdNo, prePdtWhsCmdNo: 1);

                var prepdtwhscmd = new TX50_PrePdtWhsCmd();
                prepdtwhscmd.F50_CommandNo = Constants.F50_CommandNo.CmdNoStkRtr.ToString("D");
                prepdtwhscmd.F50_CmdSeqNo = ("0000" + as_cmdno.ToString()).Substring(("0000" + as_cmdno.ToString()).Length - 4, 4);
                prepdtwhscmd.F50_CommandType = Constants.CommandType.CmdType_0;
                prepdtwhscmd.F50_StrRtrType = Constants.F50_StrRtrType.StrRtrType_PrePdt.ToString("D");
                prepdtwhscmd.F50_Status = Constants.TC_CMDSTS.TC_CMDSTS_0;
                prepdtwhscmd.F50_ContainerNo = containerNo;
                prepdtwhscmd.F50_ContainerCode = containerCode;
                prepdtwhscmd.F50_From = string.Format("{0}{1}{2}", lsRow, lsBay, lsLevel);
                prepdtwhscmd.F50_To = convcode;
                prepdtwhscmd.F50_CommandSendDate = systemTime;
                prepdtwhscmd.F50_CommandEndDate = systemTime;
                prepdtwhscmd.F50_TerminalNo = terminalNo;
                prepdtwhscmd.F50_PictureNo = Constants.PictureNo.TCIP041F;
                prepdtwhscmd.F50_AddDate = systemTime;
                prepdtwhscmd.F50_UpdateDate = systemTime;

                _unitOfWork.PreProductWarehouseCommandRepository.Add(prepdtwhscmd);
                _notificationService.SendMessageToC2(Constants.F50_CommandNo.CmdNoStkRtr.ToString("D"), as_cmdno.ToString("D4"));
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                RestoringStatusOfPreproductWarehouse(lsRow, lsBay, lsLevel,
                   terminalNo, containerCode, updateDate).RunSynchronously();
                _unitOfWork.Commit();
                _log.Error(ex.Message, ex);

            }

            return new ResponseResult<string>(convcode, true, errorMessage);
        }



        #endregion
        #region  method screen  TCIP042F



        //BR46 Re-storing rules
        public ResponseResult CreateAndUpdate(string preProductCode, string shelfNo, string containerCode,
            double quantity, string lotNo, string terminalNo, DateTime? updateDate, int containerType)
        {
            //The system will separate the value of [Shelf No.] of the selected item in to 3 parts: [ls_row], [ls_bay], [ls_level] as defined on BR 31.
            var lstShelfNo = shelfNo.Split('-');
            var lsRow = lstShelfNo[0];
            var lsBay = lstShelfNo[1];
            var lsLevel = lstShelfNo[2];
            var preProductShelfStatus =
                _unitOfWork.PreProductShelfStatusRepository.GetMany(
                    i =>
                        i.F37_ShelfRow.Trim().Equals(lsRow) && i.F37_ShelfBay.Trim().Equals(lsBay) &&
                        i.F37_ShelfLevel.Trim().Equals(lsLevel)).FirstOrDefault();
            try
            {
                preProductShelfStatus.F37_ShelfStatus = Constants.F37_ShelfStatus.ReservedForStorage.ToString("D");
                preProductShelfStatus.F37_UpdateDate = DateTime.Now;
                _unitOfWork.PreProductShelfStatusRepository.Update(preProductShelfStatus);
            }
            catch (Exception ex)
            {
                return new ResponseResult(false);
            }

            //If the system updates data successfully, it will continue updating data of the following table tx49_prepdtshfstk  
            var commandNo = "";
            var containerNo = "";
            try
            {
                var preProductShelfStock =
                    _unitOfWork.PreProductShelfStockRepository.GetMany(
                            i => i.F49_ContainerCode.Trim().Equals(containerCode))
                        .FirstOrDefault();
                if (preProductShelfStock != null)
                {
                    commandNo = preProductShelfStock.F49_KndCmdNo;
                    containerNo = preProductShelfStock.F49_ContainerNo;

                    preProductShelfStock.F49_ShelfStatus = Constants.F49_ShelfStatus.TX49_StkFlg_Str;
                    preProductShelfStock.F49_Amount = quantity;
                    preProductShelfStock.F49_UpdateDate = DateTime.Now;

                    _unitOfWork.PreProductShelfStockRepository.Update(preProductShelfStock);
                }
            }
            catch (Exception e)
            {
                return new ResponseResult(false);
            }
            var lstPreProductShelfStock =
                _unitOfWork.PreProductShelfStockRepository.GetMany(
                    i => i.F49_PreProductLotNo.Trim().Equals(lotNo) && i.F49_KndCmdNo.Trim().Equals(commandNo));

            var ldb_prepdtamount = 0.0;

            var liCount = lstPreProductShelfStock.Count();
            foreach (var item in lstPreProductShelfStock)
                ldb_prepdtamount += item.F49_Amount;

            //Update tx42_kndcmd
            var storedStatus = Constants.F42_Status.TX42_Sts_Stored;

            var kneadingCommand =
                _unitOfWork.KneadingCommandRepository.GetMany(
                    i =>
                        i.F42_KndCmdNo.Trim().Equals(commandNo) && i.F42_PrePdtLotNo.Trim().Equals(lotNo) &&
                        i.F42_Status.Equals(storedStatus)).FirstOrDefault();
            if (kneadingCommand != null)
            {
                kneadingCommand.F42_ThrowAmount = ldb_prepdtamount;
                kneadingCommand.F42_StgCtnAmt = liCount;

                _unitOfWork.KneadingCommandRepository.Update(kneadingCommand);
            }
            var isNomanage = true;
            var as_cmdNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNomanage,
                Constants.GetColumnInNoManager.PrePdtWhsCmdNo, prePdtWhsCmdNo: 1);

            //[ls_from] = the value of [is_convcode] retrieved on SQL 15
            //Get record TM03_PreProduct where F03_PreProductCode equals preProductCode
            var preProductItem =
                _unitOfWork.PreProductRepository.GetMany(i => i.F03_PreProductCode.Trim().Equals(preProductCode.Trim()))
                    .FirstOrDefault();

            var szColorClass = Constants.ColorClass.Color.ToString("D");
            if (containerType == 30)
                szColorClass = Constants.ColorClass.Black.ToString("D");

            //Get record TM05_Conveyor
            var conveyorItem =
                _unitOfWork.ConveyorRepository.GetAll()
                .FirstOrDefault(
                    i =>
                        i.F05_TerminalNo.Trim().Equals(terminalNo) && i.F05_ColorClass.Trim().Equals(szColorClass));

            if (conveyorItem == null)
                return new ResponseResult(false, "The corresponding conveyor does not exist!");

            //assign lsForm=ConveyorCode
            var lsForm = conveyorItem.F05_ConveyorCode;

            //Insert data into tx50_prepdtwhscmd
            try
            {

                var tx50_prePdtWhsCmd = new TX50_PrePdtWhsCmd();
                tx50_prePdtWhsCmd.F50_CommandNo = Constants.F50_CommandNo.CmdNoStkStr.ToString("D");
                tx50_prePdtWhsCmd.F50_CmdSeqNo = as_cmdNo.ToString("D4");
                tx50_prePdtWhsCmd.F50_CommandType = Constants.CommandType.CmdType_0;
                tx50_prePdtWhsCmd.F50_StrRtrType = Constants.F50_StrRtrType.StrRtrType_PrePdt.ToString("D");
                tx50_prePdtWhsCmd.F50_Status = Constants.TC_CMDSTS.TC_CMDSTS_0;
                tx50_prePdtWhsCmd.F50_ContainerNo = containerNo;
                tx50_prePdtWhsCmd.F50_ContainerCode = containerCode;
                tx50_prePdtWhsCmd.F50_Priority = 0;
                tx50_prePdtWhsCmd.F50_From = lsForm;
                tx50_prePdtWhsCmd.F50_To = $"{lsRow}{lsBay}{lsLevel}";
                tx50_prePdtWhsCmd.F50_CommandSendDate = DateTime.Now;
                tx50_prePdtWhsCmd.F50_CommandEndDate = DateTime.Now;
                tx50_prePdtWhsCmd.F50_TerminalNo = terminalNo;
                tx50_prePdtWhsCmd.F50_PictureNo = Constants.PictureNo.TCIP042F;
                tx50_prePdtWhsCmd.F50_AbnormalCode = null;
                tx50_prePdtWhsCmd.F50_AddDate = DateTime.Now;
                tx50_prePdtWhsCmd.F50_UpdateDate = DateTime.Now;
                tx50_prePdtWhsCmd.F50_UpdateCount = 0;
                tx50_prePdtWhsCmd.F50_RetryCount = 0;

                _unitOfWork.PreProductWarehouseCommandRepository.Add(tx50_prePdtWhsCmd);
            }
            catch (Exception e)
            {

                RestoringStatusOfPreproductWarehouse(lsRow, lsBay, lsLevel, terminalNo, containerCode, updateDate).RunSynchronously();
                return new ResponseResult(false);
            }
            // Restore the status of the Pre-product Warehouse as BR 37.

            // Send message to C2 as BR 13 with the following parameters
            _notificationService.SendMessageToC2(Constants.F50_CommandNo.CmdNoStkRtr.ToString("D"), as_cmdNo.ToString("D4"));

            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        #endregion
        #region method screen TCIP043F

        /// <summary>
        ///     Please refer BR43 - SRS 1.0 Sign off for more information
        /// </summary>
        /// <param name="lsRow"></param>
        /// <param name="lsBay"></param>
        /// <param name="lsLevel"></param>
        /// <param name="itemContainerCode"></param>
        /// <param name="itemContainerNo"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public async Task RetrievePreProductContainerNg(string lsRow, string lsBay, string lsLevel,
            string itemContainerCode, string itemContainerNo, string terminalNo)
        {
            // Call BR8.
            var isNomanage = true;
            var prePdtShfStks = _unitOfWork.PreProductShelfStockRepository.GetAll();
            prePdtShfStks = prePdtShfStks.Where(x => x.F49_ContainerCode.Equals(itemContainerCode));
            var containerNo = await prePdtShfStks.Select(x => x.F49_ContainerNo).FirstOrDefaultAsync();
            var result = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNomanage,
              Constants.GetColumnInNoManager.PrePdtWhsCmdNo, prePdtWhsCmdNo: 1);
            //  var conveyor = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo);
            //	If the system update the data on BR 8 successfully, it will insert new record into the table “tx50_prepdtwhscmd” in database as follow

            //todo Code cũ
            //var result = _commonDomain.UpdateConveyorAndPreProductWarehouse();
            if (result != 0)
            {

                _unitOfWork.PreProductWarehouseCommandRepository.AddCommand(
                               string.Format("{0}{1}{2}", lsRow, lsBay, lsLevel), result, itemContainerNo,
                               itemContainerCode, "CV214", terminalNo, Constants.PictureNo.TCIP022F,
                               Constants.F50_CommandNo.CmdNoMove.ToString("D"), Constants.F50_StrRtrType.StrRtrType_PrePdt,
                               Constants.CommandType.CmdType_0);

            }
            await _unitOfWork.CommitAsync();
        }

        /// <summary>
        ///     Please refer UC 13 - SRS 1.0 Sign off
        ///     Allows the system to process data that sent back from C2 server after it process the function that requested for
        ///     retrieving data by the system.
        /// </summary>
        /// <returns></returns>
        public async Task RetrievePreProductContainerOk(string lsRow, string lsBay, string lsLevel,
            string itemContainerCode, string itemContainerNo, string terminalNo, int containerType)
        {
            var isNomanage = true;
            var prePdtShfStks = _unitOfWork.PreProductShelfStockRepository.GetAll();
            prePdtShfStks = prePdtShfStks.Where(x => x.F49_ContainerCode.Equals(itemContainerCode));

            var systemTime = DateTime.Now;

            // Find container number.
            var containerNo = await prePdtShfStks.Select(x => x.F49_ContainerNo).FirstOrDefaultAsync();
            // 	The system will check the status of the Conveyor and Pre-product Warehouse as BR 8. 
            var result = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNomanage,
                Constants.GetColumnInNoManager.PrePdtWhsCmdNo, prePdtWhsCmdNo: 1);
            //  var conveyor = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo);
            //	If the system update the data on BR 8 successfully, it will insert new record into the table “tx50_prepdtwhscmd” in database as follow
            var szColorClass = Constants.ColorClass.Color.ToString("D");
            if (containerType == 30)
                szColorClass = Constants.ColorClass.Black.ToString("D");

            // Base on the color class and terminal number to find the suitable conveyor.
            var conveyor = await _unitOfWork.ConveyorRepository.GetAll()
                .FirstOrDefaultAsync(x => x.F05_ColorClass.Trim().Equals(szColorClass) && x.F05_TerminalNo.Trim().Equals(terminalNo));

            if (conveyor == null)
                return;

            
            //todo Code cũ
            //var result = _commonDomain.UpdateConveyorAndPreProductWarehouse();
            if (!(result == 0))
            {

                _unitOfWork.PreProductWarehouseCommandRepository.AddCommand(conveyor.F05_ConveyorCode, result, itemContainerNo,
                               itemContainerCode, string.Format("{0}{1}{2}", lsRow, lsBay, lsLevel), terminalNo,
                               Constants.PictureNo.TCIP043F, Constants.F50_CommandNo.CmdNoStkRtr.ToString("D"),
                               Constants.F50_StrRtrType.StrRtrType_PrePdt, Constants.CommandType.CmdType_1);

            }
            await _unitOfWork.CommitAsync();
        }

        #endregion
        #region receiving message from C2


        /// <summary>
        /// TCIP041F
        /// </summary>
        /// <returns></returns>
        public List<SecondCommunicationResponse> ReceiveMessageFromC2ServerForStoring(string terminalNo, string containerCode, string preProductCode, string shelfNo)
        {
            var pictureNo = Constants.PictureNo.TCIP041F;
            var cmdsts6 = Constants.TC_CMDSTS.TC_CMDSTS_6;
            var cmdsts7 = Constants.TC_CMDSTS.TC_CMDSTS_7;
            var cmdsts9 = Constants.TC_CMDSTS.TC_CMDSTS_9;
            var lstPreProductWarehourseCommand =
                _unitOfWork.PreProductWarehouseCommandRepository.GetMany(
                    i =>
                        i.F50_TerminalNo.Trim().Equals(terminalNo) && i.F50_PictureNo.Trim().Equals(pictureNo) &&
                        (i.F50_Status.Equals(cmdsts6) || i.F50_Status.Equals(cmdsts7) || i.F50_Status.Equals(cmdsts9)));
            var status = "";
            var items = new List<SecondCommunicationResponse>();
            foreach (var preProductWarehourseCommand in lstPreProductWarehourseCommand)
            {
                var item = Mapper.Map<SecondCommunicationResponse>(preProductWarehourseCommand);
                item.OldStatus = preProductWarehourseCommand.F50_Status;
                item.PreProductCode = preProductCode;
                switch (preProductWarehourseCommand.F50_Status[0])
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
                preProductWarehourseCommand.F50_Status = status;
                preProductWarehourseCommand.F50_UpdateDate = DateTime.Now;
                _unitOfWork.PreProductWarehouseCommandRepository.Update(preProductWarehourseCommand);
                items.Add(item);
            }
            _unitOfWork.Commit();
            return items;
        }
        //BR47: Processing data after receiving message from C2:
        /// <summary>
        /// TCIP042F
        /// </summary>
        /// <param name="termimalNo"></param>
        public List<SecondCommunicationResponse> ReceivingMessageFromC2(string termimalNo, string preProductCode)
        {
            //	The system retrieve all the Command No. whose Terminal No. is the current terminal and the Screen ID is “TCIP022F” as follow
            var pictureNo = Constants.PictureNo.TCIP042F;
            var cmdsts6 = Constants.TC_CMDSTS.TC_CMDSTS_6;
            var cmdsts7 = Constants.TC_CMDSTS.TC_CMDSTS_7;
            var lstPreProductWarehourseCommand =
                _unitOfWork.PreProductWarehouseCommandRepository.GetMany(
                    i =>
                        i.F50_TerminalNo.Trim().Equals(termimalNo) && i.F50_PictureNo.Trim().Equals(pictureNo) &&
                        (i.F50_Status.Equals(cmdsts6) || i.F50_Status.Equals(cmdsts7)));
            var items = new List<SecondCommunicationResponse>();
            foreach (var preProductWarehourseCommand in lstPreProductWarehourseCommand)
            {
                var item = Mapper.Map<SecondCommunicationResponse>(preProductWarehourseCommand);

                preProductWarehourseCommand.F50_Status = preProductWarehourseCommand.F50_Status == cmdsts6
                    ? Constants.TC_CMDSTS.TC_CMDSTS_C
                    : Constants.TC_CMDSTS.TC_CMDSTS_D;
                preProductWarehourseCommand.F50_UpdateDate = DateTime.Now;
                _unitOfWork.PreProductWarehouseCommandRepository.Update(preProductWarehourseCommand);
                if (item.F50_Status == cmdsts6)
                {
                    UpdateAmoutOfPreProduct(preProductWarehourseCommand.F50_ContainerCode);
                }

                var preProductShelfStock =
                    _unitOfWork.PreProductShelfStockRepository.Get(
                        i => i.F49_ContainerCode.Equals(preProductWarehourseCommand.F50_ContainerCode));
                if (preProductShelfStock != null)
                {
                    var lrlotamount = preProductShelfStock.F49_Amount;
                    var kneadingCommand =
                        _unitOfWork.KneadingCommandRepository.Get(
                            i =>
                                i.F42_KndCmdNo.Equals(preProductShelfStock.F49_KndCmdNo) &&
                                i.F42_PrePdtLotNo.Equals(preProductShelfStock.F49_PreProductLotNo) &&
                                i.F42_PreProductCode.Equals(preProductShelfStock.F49_PreProductCode));
                    kneadingCommand.F42_ThrowAmount = lrlotamount;
                    _unitOfWork.KneadingCommandRepository.Update(kneadingCommand);
                }

                items.Add(item);
            }
            _unitOfWork.Commit();
            return items;
        }
        /// <summary>
        /// Function which analyze message sent back from C2 and continue tasking.
        /// Please refer UC-15
        /// 
        /// TCIP043F
        /// </summary>
        /// <returns></returns>
        public List<SecondCommunicationResponse> AnalyzeMessageForStoringMovingPreProduct(string containerCode, string shelfNo, string terminalNo, bool oKClicked, string preProductCode)
        {

            //	The system retrieve all the Command No. 
            //whose Terminal No. is the current terminal and the Screen ID is “TCIP022F” as follow:
            var lch_status = oKClicked ? Constants.TC_CMDSTS.TC_CMDSTS_9 : Constants.TC_CMDSTS.TC_CMDSTS_8;
            var prePdtWhsCmds = _unitOfWork.PreProductWarehouseCommandRepository.GetAll();
            prePdtWhsCmds = prePdtWhsCmds.Where(x => x.F50_TerminalNo.Equals(terminalNo)
                                                     && x.F50_PictureNo.ToUpper().Equals(Constants.PictureNo.TCIP043F)
                                                     && (x.F50_Status.Trim().Equals(Constants.TC_CMDSTS.TC_CMDSTS_6)
                                                         || x.F50_Status.Trim().Equals(Constants.TC_CMDSTS.TC_CMDSTS_7))
                                                     || x.F50_Status.Equals(lch_status));
            prePdtWhsCmds = prePdtWhsCmds.OrderBy(x => x.F50_AddDate);


            var items = new List<SecondCommunicationResponse>();

            //	For each retrieved record on SQL 50, the system will perform:
            foreach (var prePdtWhsCmd in prePdtWhsCmds)
            {

                var item = Mapper.Map<SecondCommunicationResponse>(prePdtWhsCmd);
                item.PreProductCode = preProductCode;
                if (prePdtWhsCmd.F50_Status.Trim().Equals(Constants.TC_CMDSTS.TC_CMDSTS_6))
                {
                    prePdtWhsCmd.F50_Status = Constants.TC_CMDSTS.TC_CMDSTS_C;

                }
                //o	If the [Old_Status] = “tc_cmdsts_7” (that means Command End), 
                //the system will set a new parameter [New_Status] = “tc_cmdsts_d” (‘D’) as pre-configured on “constant.txt” file
                else if (prePdtWhsCmd.F50_Status.Trim().Equals(Constants.TC_CMDSTS.TC_CMDSTS_7))
                {
                    prePdtWhsCmd.F50_Status = Constants.TC_CMDSTS.TC_CMDSTS_D;
                }
                //o	Otherwise, the system will skip the current record and loop to the next record.
                else
                {
                    continue;
                }
                items.Add(item);
                prePdtWhsCmd.F50_UpdateDate = DateTime.Now;
                _unitOfWork.PreProductWarehouseCommandRepository.Update(prePdtWhsCmd);
            }
            _unitOfWork.Commit();

            return items;
        }

        #endregion
        #region private Method
        //BR48 Updating amount of Pre-product rules:
        private void UpdateAmoutOfPreProduct(string lstconcode)
        {
            var lstPreProductShelfStock =
                _unitOfWork.PreProductShelfStockRepository.GetMany(i => lstconcode.Contains(i.F49_ContainerCode.Trim()));
            if (!lstPreProductShelfStock.Any())
            {
                //	If no record found, the system will write new line into the log file as “cannot find container in pre-product stock”.
            }
            foreach (var preProductShelfStock in lstPreProductShelfStock)
            {
                var lrLotAmount = 0.0;
                var lstPrePdtShfStk =
                    _unitOfWork.PreProductShelfStockRepository.GetMany(
                        i =>
                            i.F49_KndCmdNo.Trim().Equals(preProductShelfStock.F49_KndCmdNo.Trim()) &&
                            i.F49_PreProductLotNo.Trim().Equals(preProductShelfStock.F49_PreProductLotNo.Trim()) &&
                            i.F49_PreProductCode.Trim().Equals(preProductShelfStock.F49_PreProductCode.Trim()));
                if (lstPrePdtShfStk.Any())
                {
                    foreach (var prePdtShfStk in lstPrePdtShfStk)
                        lrLotAmount += prePdtShfStk.F49_Amount;
                    //If there is any record found, the system will update the table in database
                    var kneadingCommand =
                        _unitOfWork.KneadingCommandRepository.GetMany(
                                i =>
                                    i.F42_KndCmdNo.Trim().Equals(preProductShelfStock.F49_KndCmdNo.Trim()) &&
                                    i.F42_PrePdtLotNo.Trim().Equals(preProductShelfStock.F49_PreProductLotNo.Trim()) &&
                                    i.F42_PreProductCode.Trim().Equals(preProductShelfStock.F49_PreProductCode.Trim()))
                            .FirstOrDefault();
                    kneadingCommand.F42_ThrowAmount = lrLotAmount;
                    lrLotAmount = 0.0;
                    _unitOfWork.KneadingCommandRepository.Update(kneadingCommand);
                }
            }
        }
        public ResponseResult CheckConveyorByCode(string code)
        {
            //return _commonDomain.CheckingStatusOfConveyorAndPreproductWarehouse(terminerNo);
            var conveyor = _unitOfWork.ConveyorRepository.GetById(code);
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
            if (device == null)
                return new ResponseResult(false, "MSG14");
            if (
                (device.F14_DeviceStatus != Constants.F14_DeviceStatus.Online.ToString("d")) ||
                (device.F14_UsePermitClass == Constants.F14_UsePermitClass.Prohibited.ToString("d")))
                return new ResponseResult(false, "MSG14");
            return new ResponseResult(true);
        }

        /// <summary>
        ///     Please refer to BR37 SRS - 1.0 for more information.
        /// </summary>
        /// <param name="lsRow"></param>
        /// <param name="lsBay"></param>
        /// <param name="lsLevel"></param>
        /// <param name="terminalNo"></param>
        /// <param name="containerCode"></param>
        private async Task RestoringStatusOfPreproductWarehouse(string lsRow, string lsBay, string lsLevel,
            string terminalNo, string containerCode, DateTime? updateDate)
        {
            var TX37_StkTkgFlg_InvNotChk = "0";

            // Find all pre-product shelf statuses.
            var prepdtshfstses = _unitOfWork.PreProductShelfStatusRepository.GetAll();
            prepdtshfstses = prepdtshfstses.Where(x => x.F37_ShelfRow.Equals(lsRow));
            prepdtshfstses = prepdtshfstses.Where(x => x.F37_ShelfBay.Equals(lsBay));
            prepdtshfstses = prepdtshfstses.Where(x => x.F37_ShelfLevel.Equals(lsLevel));

            foreach (var prepdtshfsts in prepdtshfstses)
            {
                prepdtshfsts.F37_ShelfStatus = Constants.F37_ShelfStatus.Stock.ToString("D");
                prepdtshfsts.F37_TerminalNo = terminalNo;
                prepdtshfsts.F37_StockTakingFlag = TX37_StkTkgFlg_InvNotChk;
            }

            var prepdtshfstks = _unitOfWork.PreProductShelfStockRepository.GetAll();
            prepdtshfstks = prepdtshfstks.Where(x => x.F49_ContainerCode.Equals(containerCode));
            prepdtshfstks = prepdtshfstks.Where(x => EntityFunctions.TruncateTime(x.F49_UpdateDate).Equals(updateDate.Value));

            foreach (var prepdtshfstk in prepdtshfstks)
            {
                prepdtshfstk.F49_ShelfStatus = TX37_StkTkgFlg_InvNotChk;
                //_unitOfWork.PreProductShelfStockRepository.Update(prepdtshfstk);
            }

            await _unitOfWork.CommitAsync();
        }
        #endregion
    }
}