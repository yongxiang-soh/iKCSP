using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc.Html;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using KCSG.Core.Constants;
using KCSG.Core.Resources;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Interfaces;
using Resources;

namespace KCSG.Web.Services
{
    public class CommunicationService : ICommunicationService
    {
        #region Properties

        /// <summary>
        /// Provides function to access repositories.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Service which provides functions to access application settings.
        /// </summary>
        private readonly IConfigurationService _configurationService;

        /// <summary>
        /// Domain which handles common business.
        /// </summary>
        private readonly ICommonDomain _commonDomain;

        #endregion

        #region Constructors

        /// <summary>
        /// Initiate service with IoC.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="configurationService"></param>
        /// <param name="commonDomain"></param>
        public CommunicationService(IUnitOfWork unitOfWork, IConfigurationService configurationService, ICommonDomain commonDomain)
        {
            _unitOfWork = unitOfWork;
            _configurationService = configurationService;
            _commonDomain = commonDomain;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send message from a specific terminal to C3.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public async Task<string> AnalyzeThirdCommunicationBusinessAsync(string terminalNo)
        {
            // Message which should be response to client.
            string message = string.Empty;

            // Find list of product warehouse commands.
            var productWarehouseCommands = _unitOfWork.ProductWarehouseCommandRepository.GetAll();
            productWarehouseCommands = productWarehouseCommands.Where(x => x.F47_TerminalNo.Equals(terminalNo));
            productWarehouseCommands =
                productWarehouseCommands.Where(x => x.F47_PictureNo.Equals(Constants.PictureNo.TCPR041F)
                || Constants.PictureNo.TCPR071F.Equals(x.F47_PictureNo) || Constants.PictureNo.TCPR081F.Equals(x.F47_PictureNo)
                || Constants.PictureNo.TCPR091F.Equals(x.F47_PictureNo));

            productWarehouseCommands =
                productWarehouseCommands.Where(
                    x =>
                        x.F47_Status.Trim().Equals("6") || x.F47_Status.Trim().Equals("7") ||
                        x.F47_Status.Trim().Equals("9") || x.F47_Status.Trim().Equals("8"));

            foreach (var productWarehouseCommand in productWarehouseCommands)
            {
                if (!Constants.PictureNo.TCPR081F.Equals(productWarehouseCommand.F47_PictureNo))
                {
                    var productShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();
                    productShelfStocks =
                        productShelfStocks.Where(x => x.F40_PalletNo.Equals(productWarehouseCommand.F47_PalletNo));
                    productShelfStocks = productShelfStocks.Where(x => x.F40_AssignAmount > 0);

                    if (!"8".Equals(productWarehouseCommand.F47_Status.Trim()))
                    {

                        foreach (var productShelfStock in productShelfStocks)
                        {
                            if (Constants.F40_StockFlag.TX40_StkFlg_NotStkShip.Equals(productShelfStock.F40_StockFlag))
                                InsertProductShippingCommand(productWarehouseCommand, productShelfStock);

                            if (!"2".Equals(productShelfStock.F40_StockFlag) &&
                                !"3".Equals(productShelfStock.F40_StockFlag))
                            {
                                UpdateProductShelfStock(productShelfStock, productWarehouseCommand.F47_Status);
                                UpdateShippingPlan(productWarehouseCommand.F47_CommandNo,
                                    productShelfStock.F40_AssignAmount);
                            }

                            if ("1".Equals(productShelfStock.F40_StockFlag.Trim()))
                                UpdateProductShelfStatus(productWarehouseCommand.F47_To, terminalNo);

                            // Send UC32.
                            if ("7".Equals(productWarehouseCommand.F47_Status) && Constants.PictureNo.TCPR091F.Equals(productWarehouseCommand.F47_PictureNo))
                            {
                                // Find conveyor by using terminal no.
                                var conveyor = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo);

                                // Find device code.

                                // TODO: Configuration section.
                                var isDeviceAvailable = true;

                                if (conveyor != null && isDeviceAvailable)
                                {

                                    _unitOfWork.KneadingCommandRepository.Delete(x => x.F42_KndCmdNo.Equals(productWarehouseCommand.F47_CommandNo)
                                   && x.F42_PrePdtLotNo.Equals(productShelfStock.F40_PrePdtLotNo));
                                }
                                else
                                {
                                    // Find all kneading commands.
                                    var kneadingCommands = _unitOfWork.KneadingCommandRepository.GetAll();
                                    var kneadingCommand =
                                        await
                                            kneadingCommands.FirstOrDefaultAsync(
                                                x => x.F42_PrePdtLotNo.Equals(productShelfStock.F40_PalletNo)
                                                && x.F42_PreProductCode.Equals(productShelfStock.F40_ProductCode));

                                    if (kneadingCommand.F42_ThrowAmount == 0)
                                    {

                                    }
                                    else
                                    {
                                        // TODO: Update
                                    }

                                }
                            }
                        }

                        UpdateProductWarehouseCommand(productWarehouseCommand);
                    }
                    else
                    {
                        if (Constants.PictureNo.TCPR071F.Equals(productWarehouseCommand.F47_PictureNo))
                        {
                            foreach (var productShelfStock in productShelfStocks)
                            {
                                var product = _unitOfWork.ProductRepository.GetById(productShelfStock.F40_ProductCode);
                                var normalTemperature = Constants.Temperature.Normal.ToString("D");

                                // TODO: Refer XI
                                var productShelfStatuses = _unitOfWork.ProductShelfStatusRepository.GetAll();
                                productShelfStatuses = productShelfStatuses.Where(x => x.F51_ShelfStatus.Equals("0"));
                                productShelfStatuses = productShelfStatuses.Where(x => x.F51_ShelfType.Equals("0"));
                                productShelfStatuses = productShelfStatuses.Where(x => x.F51_LowTmpShfAgnOrd.HasValue);
                                productShelfStatuses = productShelfStatuses.OrderBy(x => x.F51_LowTmpShfAgnOrd);

                                if (!await productShelfStatuses.AnyAsync())
                                    throw new Exception("MSG19");

                                var shelfNo = string.Empty;

                                foreach (var productShelfStatus in productShelfStatuses)
                                {
                                    productShelfStatus.F51_ShelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvStr;
                                    productShelfStatus.F51_StockTakingFlag =
                                        Constants.F51_StockTakingFlag.TX51_StkTkgFlg_InvNotChk;
                                    productShelfStatus.F51_TerminalNo = terminalNo;
                                    productShelfStatus.F51_UpdateDate = DateTime.Now;

                                    shelfNo = string.Format("{0}{1}{2}", productShelfStatus.F51_ShelfRow,
                                        productShelfStatus.F51_ShelfBay, productShelfStatus.F51_ShelfLevel);
                                }

                                // TODO: Refer XXVI
                                var isNoManageUpdated = false;
                                var sequenceNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(
                                    ref isNoManageUpdated,
                                    Constants.GetColumnInNoManager.PrePdtWhsCmdNo, 0, 0, 0, 0, 1);

                                var conveyor = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo);

                                _unitOfWork.ProductWarehouseCommandRepository.InsertProductWarehouseCommand("1000",
                                    sequenceNo.ToString(), Constants.CmdType.cmdType, "4",
                                    Constants.F47_Status.AnInstruction.ToString("D"),
                                    productShelfStock.F40_PalletNo, conveyor.F05_ConveyorCode, shelfNo,
                                    terminalNo,
                                    Constants.PictureNo.TCPR071F);

                                UpdateProductWarehouseCommand(productWarehouseCommand);
                            }
                        }
                    }
                }
                else
                {
                    UpdateProductWarehouseCommand(productWarehouseCommand);
                }

                // Format response message.
                message = FormatThirdCommunicationMessageResponse(productWarehouseCommand);
            }

            #region Uc35 Uc36

            var tx47 =
               _unitOfWork.ProductWarehouseCommandRepository.GetMany(
                   i =>
                       i.F47_PictureNo.Trim() == "TCPR111F" && i.F47_TerminalNo.Trim().Equals(terminalNo) &&
                       (i.F47_Status.Equals("6") || i.F47_Status.Equals("7") || i.F47_Status.Equals("8") ||
                       i.F47_Status.Equals("9"))).OrderBy(i => i.F47_AddDate);

            var Status = "";
            var pdtShelfStatus = await _unitOfWork.ProductShelfStatusRepository.GetAll().FirstOrDefaultAsync(x => x.F51_TerminalNo.Equals(terminalNo) && x.F51_ShelfStatus.Equals("4"));

            foreach (var tx47PdtWhsCmd in tx47)
            {
                var shelftStatus = pdtShelfStatus.F51_ShelfStatus;

                switch (tx47PdtWhsCmd.F47_Status)
                {//	If Status 5 is “Instruction complete” (or 6), set New Status 5 = “An instruction complete confirmation” (or C),
                    case "6":
                        tx47PdtWhsCmd.F47_Status = Constants.F34_Status.statusC;
                        Status = "success.";
                        break;
                    //	If Status 5 is “Instruction cancel” (or 7), set New Status 5 = “An instruction cancel confirmation” (or D).
                    case "7":
                        Status = "cancel.";
                        tx47PdtWhsCmd.F47_Status = Constants.F34_Status.statusD;
                        break;

                    //	If Status 5 is “Double installation error”(or 8):
                    case "8":
                        Status = "two times storage ";
                        //o	If value of Shelf Status column of selected row = “Empty” (or 0):
                        if (shelftStatus == Constants.F51_ShelfStatus.TX51_ShfSts_Epy)
                        {
                            //•	Set New Status 5 = “A double installation confirmation” (or E),
                            //•	Re-run (Reference xiii).
                            tx47PdtWhsCmd.F47_Status = Constants.F34_Status.statusE;

                            FindConveyorCodeValidity(terminalNo);
                            var row = pdtShelfStatus.F51_ShelfRow;
                            var bay = pdtShelfStatus.F51_ShelfBay;
                            var level = pdtShelfStatus.F51_ShelfLevel;

                            var isCreate = true;
                            var sequenceNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isCreate,
                                Constants.GetColumnInNoManager.Pdtwhscmdno, 0, 0, 0, 0, 1);
                            //	Update table TX51_PDTSHFSTS, where:
                            //-	[f51_shelfrow] = Row,
                            //-	AND [f51_shelfBay] = Bay,
                            //-	AND [f51_shelflevel] = Level, 
                            //-	AND [f51_shelfstatus] = Shelf Status column value.
                            //Then:
                            //-	Set  [f51_shelfstatus] = “Assigned for Storage” (or 4), 
                            //-	Set [f51_TerminalNo] = Terminal No. which is retrieved from section “server” in “toshiba.ini” configurable file,
                            //-	Set [f51_Updatedate] = current date time.
                            var tx51 =
                _unitOfWork.ProductShelfStatusRepository.GetMany(
               i => i.F51_ShelfRow == row && i.F51_ShelfBay == bay && i.F51_ShelfLevel == level && i.F51_ShelfStatus.Trim().Equals(shelftStatus.Trim())).FirstOrDefault();
                            _unitOfWork.ProductShelfStatusRepository.UpdateProductShelfStatus(tx51, Constants.F51_ShelfStatus.TX51_ShfSts_RsvStr, terminalNo);

                            var Conveyor =
                                _unitOfWork.ConveyorRepository.GetMany(i => i.F05_TerminalNo.Trim().Equals(terminalNo))
                                    .FirstOrDefault();

                            _unitOfWork.ProductWarehouseCommandRepository.InsertProductWarehouseCommand("1001", sequenceNo.ToString(), "0000", tx47PdtWhsCmd.F47_StrRtrType,
                                Constants.F47_Status.AnInstruction.ToString("D"), "", Conveyor.F05_ConveyorCode, row + bay + level,
                               terminalNo, Constants.PictureNo.TCPR111F);
                        }
                        break;
                    case "9":
                        Status = "two times storage ";
                        tx47PdtWhsCmd.F47_Status = Constants.F34_Status.statusF;
                        break;
                    default:

                        tx47PdtWhsCmd.F47_Status = Constants.F34_Status.statusE;
                        //  message = "There is no empty location available in the warehouse now!";
                        break;

                }

                // Format response message.
                message = FormatThirdCommunicationMessageResponse(tx47PdtWhsCmd);
                _unitOfWork.ProductWarehouseCommandRepository.Update(tx47PdtWhsCmd);

                #endregion
            }


            await _unitOfWork.CommitAsync();

            return message;
        }

        /// <summary>
        /// Send message from a specific terminal to C3.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public string AnalyzeThirdCommunicationBusiness(string terminalNo)
        {
            // Message which should be response to client.
            string message = string.Empty;

            // Find list of product warehouse commands.
            var productWarehouseCommands = _unitOfWork.ProductWarehouseCommandRepository.GetAll();
            productWarehouseCommands = productWarehouseCommands.Where(x => x.F47_TerminalNo.Equals(terminalNo));
            productWarehouseCommands =
                productWarehouseCommands.Where(x => x.F47_PictureNo.Equals(Constants.PictureNo.TCPR041F)
                || Constants.PictureNo.TCPR071F.Equals(x.F47_PictureNo) || Constants.PictureNo.TCPR081F.Equals(x.F47_PictureNo)
                || Constants.PictureNo.TCPR091F.Equals(x.F47_PictureNo));

            productWarehouseCommands =
                productWarehouseCommands.Where(
                    x =>
                        x.F47_Status.Trim().Equals("6") || x.F47_Status.Trim().Equals("7") ||
                        x.F47_Status.Trim().Equals("9") || x.F47_Status.Trim().Equals("8"));

            foreach (var productWarehouseCommand in productWarehouseCommands)
            {
                if (!Constants.PictureNo.TCPR081F.Equals(productWarehouseCommand.F47_PictureNo))
                {
                    var productShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();
                    productShelfStocks =
                        productShelfStocks.Where(x => x.F40_PalletNo.Equals(productWarehouseCommand.F47_PalletNo));
                    productShelfStocks = productShelfStocks.Where(x => x.F40_AssignAmount > 0);

                    if (!"8".Equals(productWarehouseCommand.F47_Status.Trim()))
                    {

                        foreach (var productShelfStock in productShelfStocks)
                        {
                            if (Constants.F40_StockFlag.TX40_StkFlg_NotStkShip.Equals(productShelfStock.F40_StockFlag))
                                InsertProductShippingCommand(productWarehouseCommand, productShelfStock);

                            if (!"2".Equals(productShelfStock.F40_StockFlag) &&
                                !"3".Equals(productShelfStock.F40_StockFlag))
                            {
                                UpdateProductShelfStock(productShelfStock, productWarehouseCommand.F47_Status);
                                UpdateShippingPlan(productWarehouseCommand.F47_CommandNo,
                                    productShelfStock.F40_AssignAmount);
                            }

                            if ("1".Equals(productShelfStock.F40_StockFlag.Trim()))
                                UpdateProductShelfStatus(productWarehouseCommand.F47_To, terminalNo);

                            // Send UC32.
                            if ("7".Equals(productWarehouseCommand.F47_Status) && Constants.PictureNo.TCPR091F.Equals(productWarehouseCommand.F47_PictureNo))
                            {
                                // Find conveyor by using terminal no.
                                var conveyor = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo);

                                // Find device code.

                                // TODO: Configuration section.
                                var isDeviceAvailable = true;

                                if (conveyor != null && isDeviceAvailable)
                                {

                                    _unitOfWork.KneadingCommandRepository.Delete(x => x.F42_KndCmdNo.Equals(productWarehouseCommand.F47_CommandNo)
                                   && x.F42_PrePdtLotNo.Equals(productShelfStock.F40_PrePdtLotNo));
                                }
                                else
                                {
                                    // Find all kneading commands.
                                    var kneadingCommands = _unitOfWork.KneadingCommandRepository.GetAll();
                                    var kneadingCommand =

                                            kneadingCommands.FirstOrDefault(
                                                x => x.F42_PrePdtLotNo.Equals(productShelfStock.F40_PalletNo)
                                                && x.F42_PreProductCode.Equals(productShelfStock.F40_ProductCode));

                                    if (kneadingCommand == null)
                                        throw new Exception(HttpMessages.InvalidKneadingCommand);

                                    if (kneadingCommand.F42_ThrowAmount.Equals(0))
                                    {

                                    }
                                    else
                                    {
                                        // TODO: Update
                                    }

                                }
                            }
                        }

                        UpdateProductWarehouseCommand(productWarehouseCommand);
                    }
                    else
                    {
                        if (Constants.PictureNo.TCPR071F.Equals(productWarehouseCommand.F47_PictureNo))
                        {
                            foreach (var productShelfStock in productShelfStocks)
                            {
                                var product = _unitOfWork.ProductRepository.GetById(productShelfStock.F40_ProductCode);
                                var normalTemperature = Constants.Temperature.Normal.ToString("D");

                                // TODO: Refer XI
                                var productShelfStatuses = _unitOfWork.ProductShelfStatusRepository.GetAll();
                                productShelfStatuses = productShelfStatuses.Where(x => x.F51_ShelfStatus.Equals("0"));
                                productShelfStatuses = productShelfStatuses.Where(x => x.F51_ShelfType.Equals("0"));
                                productShelfStatuses = productShelfStatuses.Where(x => x.F51_LowTmpShfAgnOrd.HasValue);
                                productShelfStatuses = productShelfStatuses.OrderBy(x => x.F51_LowTmpShfAgnOrd);

                                if (!productShelfStatuses.Any())
                                    throw new Exception(HttpMessages.InvalidProductShelfStatus);

                                var shelfNo = string.Empty;

                                foreach (var productShelfStatus in productShelfStatuses)
                                {
                                    productShelfStatus.F51_ShelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvStr;
                                    productShelfStatus.F51_StockTakingFlag =
                                        Constants.F51_StockTakingFlag.TX51_StkTkgFlg_InvNotChk;
                                    productShelfStatus.F51_TerminalNo = terminalNo;
                                    productShelfStatus.F51_UpdateDate = DateTime.Now;

                                    shelfNo = string.Format("{0}{1}{2}", productShelfStatus.F51_ShelfRow,
                                        productShelfStatus.F51_ShelfBay, productShelfStatus.F51_ShelfLevel);
                                }

                                // TODO: Refer XXVI
                                var isNoManageUpdated = false;
                                var sequenceNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(
                                    ref isNoManageUpdated,
                                    Constants.GetColumnInNoManager.PrePdtWhsCmdNo, 0, 0, 0, 0, 1);

                                var conveyor = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo);

                                _unitOfWork.ProductWarehouseCommandRepository.InsertProductWarehouseCommand("1000",
                                    sequenceNo.ToString(), Constants.CmdType.cmdType, "4",
                                    Constants.F47_Status.AnInstruction.ToString("D"),
                                    productShelfStock.F40_PalletNo, conveyor.F05_ConveyorCode, shelfNo,
                                    terminalNo,
                                    Constants.PictureNo.TCPR071F);

                                UpdateProductWarehouseCommand(productWarehouseCommand);
                            }
                        }
                    }
                }
                else
                {
                    UpdateProductWarehouseCommand(productWarehouseCommand);
                }

                // Format response message.
                message = FormatThirdCommunicationMessageResponse(productWarehouseCommand);
            }

            #region Uc35 Uc36

            var tx47 =
               _unitOfWork.ProductWarehouseCommandRepository.GetMany(
                   i =>
                       i.F47_PictureNo.Trim() == "TCPR111F" && i.F47_TerminalNo.Trim().Equals(terminalNo) &&
                       (i.F47_Status.Equals("6") || i.F47_Status.Equals("7") || i.F47_Status.Equals("8") ||
                       i.F47_Status.Equals("9"))).OrderBy(i => i.F47_AddDate);

            var pdtShelfStatus = _unitOfWork.ProductShelfStatusRepository.GetAll().FirstOrDefault(x => x.F51_TerminalNo.Equals(terminalNo) && x.F51_ShelfStatus.Equals("4"));

            foreach (var tx47PdtWhsCmd in tx47)
            {
                var shelftStatus = pdtShelfStatus.F51_ShelfStatus;

                switch (tx47PdtWhsCmd.F47_Status)
                {//	If Status 5 is “Instruction complete” (or 6), set New Status 5 = “An instruction complete confirmation” (or C),
                    case "6":
                        tx47PdtWhsCmd.F47_Status = Constants.F34_Status.statusC;
                        break;
                    //	If Status 5 is “Instruction cancel” (or 7), set New Status 5 = “An instruction cancel confirmation” (or D).
                    case "7":
                        tx47PdtWhsCmd.F47_Status = Constants.F34_Status.statusD;
                        break;

                    //	If Status 5 is “Double installation error”(or 8):
                    case "8":
                        //o	If value of Shelf Status column of selected row = “Empty” (or 0):
                        if (shelftStatus == Constants.F51_ShelfStatus.TX51_ShfSts_Epy)
                        {
                            //•	Set New Status 5 = “A double installation confirmation” (or E),
                            //•	Re-run (Reference xiii).
                            tx47PdtWhsCmd.F47_Status = Constants.F34_Status.statusE;

                            FindConveyorCodeValidity(terminalNo);
                            var row = pdtShelfStatus.F51_ShelfRow;
                            var bay = pdtShelfStatus.F51_ShelfBay;
                            var level = pdtShelfStatus.F51_ShelfLevel;

                            var isCreate = true;
                            var sequenceNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isCreate,
                                Constants.GetColumnInNoManager.Pdtwhscmdno, 0, 0, 0, 0, 1);
                            //	Update table TX51_PDTSHFSTS, where:
                            //-	[f51_shelfrow] = Row,
                            //-	AND [f51_shelfBay] = Bay,
                            //-	AND [f51_shelflevel] = Level, 
                            //-	AND [f51_shelfstatus] = Shelf Status column value.
                            //Then:
                            //-	Set  [f51_shelfstatus] = “Assigned for Storage” (or 4), 
                            //-	Set [f51_TerminalNo] = Terminal No. which is retrieved from section “server” in “toshiba.ini” configurable file,
                            //-	Set [f51_Updatedate] = current date time.
                            var tx51 =
                _unitOfWork.ProductShelfStatusRepository.GetMany(
               i => i.F51_ShelfRow == row && i.F51_ShelfBay == bay && i.F51_ShelfLevel == level && i.F51_ShelfStatus.Trim().Equals(shelftStatus.Trim())).FirstOrDefault();
                            _unitOfWork.ProductShelfStatusRepository.UpdateProductShelfStatus(tx51, Constants.F51_ShelfStatus.TX51_ShfSts_RsvStr, terminalNo);

                            var Conveyor =
                                _unitOfWork.ConveyorRepository.GetMany(i => i.F05_TerminalNo.Trim().Equals(terminalNo))
                                    .FirstOrDefault();

                            _unitOfWork.ProductWarehouseCommandRepository.InsertProductWarehouseCommand("1001", sequenceNo.ToString(), "0000", tx47PdtWhsCmd.F47_StrRtrType,
                                Constants.F47_Status.AnInstruction.ToString("D"), "", Conveyor.F05_ConveyorCode, row + bay + level,
                               terminalNo, Constants.PictureNo.TCPR111F);
                        }
                        break;
                    case "9":
                        tx47PdtWhsCmd.F47_Status = Constants.F34_Status.statusF;
                        break;
                    default:

                        tx47PdtWhsCmd.F47_Status = Constants.F34_Status.statusE;
                        //  message = "There is no empty location available in the warehouse now!";
                        break;

                }

                // Format response message.
                message = FormatThirdCommunicationMessageResponse(tx47PdtWhsCmd);
                _unitOfWork.ProductWarehouseCommandRepository.Update(tx47PdtWhsCmd);

                #endregion
            }


            _unitOfWork.Commit();

            return message;
        }

        public string AnalyzeSecondCommunicationBusiness(string terminalNo, string pictureNo)
        {
            var preProductWarehouseCommands = _unitOfWork.PreProductWarehouseCommandRepository.GetAll();
            preProductWarehouseCommands = preProductWarehouseCommands.Where(x => x.F50_TerminalNo.Equals(terminalNo));
            preProductWarehouseCommands = preProductWarehouseCommands.Where(x => x.F50_PictureNo.Equals(pictureNo));
            preProductWarehouseCommands =
                preProductWarehouseCommands.Where(x => Constants.TC_CMDSTS.TC_CMDSTS_6.Equals(x.F50_Status.Trim())
                                                       || Constants.TC_CMDSTS.TC_CMDSTS_7.Equals(x.F50_Status.Trim())
                                                       || Constants.TC_CMDSTS.TC_CMDSTS_9.Equals(x.F50_Status.Trim()));

            preProductWarehouseCommands = preProductWarehouseCommands.OrderBy(x => x.F50_AddDate);

            foreach (var preProductWarehouseCommand in preProductWarehouseCommands)
            {
                string lchNewStatus = null;

                // Find status of pre-product warehouse command.
                var prePdtWarehouseCommandStatus = preProductWarehouseCommand.F50_Status.Trim();

                if (Constants.TC_CMDSTS.TC_CMDSTS_6.Equals(prePdtWarehouseCommandStatus))
                    preProductWarehouseCommand.F50_Status = Constants.TC_CMDSTS.TC_CMDSTS_C;
                else if (Constants.TC_CMDSTS.TC_CMDSTS_7.Equals(prePdtWarehouseCommandStatus))
                    preProductWarehouseCommand.F50_Status = Constants.TC_CMDSTS.TC_CMDSTS_D;
                else if (Constants.TC_CMDSTS.TC_CMDSTS_9.Equals(prePdtWarehouseCommandStatus))
                    preProductWarehouseCommand.F50_Status = Constants.TC_CMDSTS.TC_CMDSTS_F;
                else if (Constants.TC_CMDSTS.TC_CMDSTS_8.Equals(prePdtWarehouseCommandStatus))
                {
                    var warehouseMessage = FormatWarehouseMessageBox("0", prePdtWarehouseCommandStatus,
                        /*TODO: ls_CnyvNo*/ null, /*ls_ShelfNo*/null, "1",
                        Constants.F50_StrRtrType.StrRtrType_Container.ToString("D"), "", "");

                    // Re-assign the container as BR4
                    var commandNo = AssignContainerNo();
                    if (string.IsNullOrEmpty(commandNo))
                        lchNewStatus = Constants.TC_CMDSTS.TC_CMDSTS_E;
                    else
                    {
                        // If success, the system will continue updating data ase BR 10.
                        // Call BR 10.
                        preProductWarehouseCommand.F50_UpdateDate = DateTime.Now;
                    }

                    // The system will separate the value of [ls_CnyvNo] into 3 parts;
                    // TODO: Shelf no
                    var productShelfNo = _commonDomain.FindProductShelfInformation( /*Shelf no*/null);
                    
                    // If fail, the system will set lch_NewStatus = tc_cmdsts_e and show message as MSG 24.
                    lchNewStatus = Constants.TC_CMDSTS.TC_CMDSTS_E;
                    warehouseMessage = HttpMessages.EmptyLocationNotAvailable;

                    // Find empty shelf status.
                    var emptyShelfStatus = Constants.F37_ShelfStatus.EmptyShelf.ToString("D");

                    // SQL 11.
                    var preProductShelfStatuses = _unitOfWork.PreProductShelfStatusRepository.GetAll();
                    preProductShelfStatuses =
                        preProductShelfStatuses.Where(x => x.F37_ShelfRow.Trim().Equals(productShelfNo.Row) && x.F37_ShelfBay.Trim().Equals(productShelfNo.Bay) && x.F37_ShelfLevel.Trim().Equals(productShelfNo.Level));
                    preProductShelfStatuses =
                        preProductShelfStatuses.Where(x => x.F37_ShelfStatus.Trim().Equals(emptyShelfStatus));
                }
                else
                    continue;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productWarehouseCommand"></param>
        /// <returns></returns>
        private string FormatThirdCommunicationMessageResponse(TX47_PdtWhsCmd productWarehouseCommand)
        {
            return string.Format("0002 {0} {1} 0066 {2} 1001 {3} {4} {5} {6} {7} {8}",
                productWarehouseCommand.F47_TerminalNo, productWarehouseCommand.F47_PictureNo,
                productWarehouseCommand.F47_CommandType, productWarehouseCommand.F47_Status,
                productWarehouseCommand.F47_From,
                productWarehouseCommand.F47_To, productWarehouseCommand.F47_PalletNo, DateTime.Now.ToString("dd/MM/yyyy"));
        }

        /// <summary>
        /// Insert records into product shipping command table.
        /// </summary>
        /// <param name="productWarehouseCommand"></param>
        /// <param name="productShelfStock"></param>
        private void InsertProductShippingCommand(TX47_PdtWhsCmd productWarehouseCommand, TX40_PdtShfStk productShelfStock)
        {
            var productShippingHistory = new TH70_PdtShipHst();
            productShippingHistory.F70_ShipCommandNo = productWarehouseCommand.F47_CommandNo;
            productShippingHistory.F70_PalletNo = productWarehouseCommand.F47_PalletNo;
            productShippingHistory.F70_PrePdtLotNo = productShelfStock.F40_PrePdtLotNo;
            productShippingHistory.F70_ProductCode = productShelfStock.F40_ProductCode;
            productShippingHistory.F70_ShelfNo = productWarehouseCommand.F47_From;
            productShippingHistory.F70_ProductLotNo = productShelfStock.F40_ProductLotNo;
            productShippingHistory.F70_ShippedAmount = productShelfStock.F40_AssignAmount ?? 0;
            productShippingHistory.F70_UpdateDate = DateTime.Now;
            productShippingHistory.F70_UpdateCount = 0;

            _unitOfWork.PdtShipHstRepository.Add(productShippingHistory);

        }

        /// <summary>
        /// Update records in product shelf stock.
        /// </summary>
        /// <param name="productShelfStock"></param>
        /// <param name="status"></param>
        private void UpdateProductShelfStock(TX40_PdtShfStk productShelfStock, string status)
        {
            switch (status)
            {
                case "7":
                case "9":
                    productShelfStock.F40_ShipCommandNo = null;
                    productShelfStock.F40_AssignAmount = 0;
                    productShelfStock.F40_UpdateDate = DateTime.Now;
                    break;
                case "6":

                    productShelfStock.F40_ShippedAmount += productShelfStock.F40_AssignAmount ?? 0;
                    productShelfStock.F40_ShipCommandNo = null;
                    productShelfStock.F40_AssignAmount = 0;
                    productShelfStock.F40_UpdateDate = DateTime.Now;

                    break;

            }
            _unitOfWork.ProductShelfStockRepository.Update(productShelfStock);

        }

        /// <summary>
        /// Update shipping plan record.
        /// </summary>
        /// <param name="commandNo"></param>
        /// <param name="assignedAmount"></param>
        private void UpdateShippingPlan(string commandNo, double? assignedAmount)
        {
            var productShippingPlans = _unitOfWork.ShippingPlanRepository.GetAll();
            productShippingPlans = productShippingPlans.Where(x => x.F44_ShipCommandNo.Trim().Equals(commandNo.Trim()));

            foreach (var productShippingPlan in productShippingPlans)
            {
                productShippingPlan.F44_ShippedAmount -= assignedAmount ?? 0;
                productShippingPlan.F44_UpdateDate = DateTime.Now;
            }
        }

        /// <summary>
        /// Update product warehouse command.
        /// </summary>
        /// <param name="productWarehouseCommand"></param>
        private void UpdateProductWarehouseCommand(TX47_PdtWhsCmd productWarehouseCommand)
        {
            string newStatus;

            switch (productWarehouseCommand.F47_Status)
            {
                case "6":
                    newStatus = Constants.F47_Status.AnInstructionCompleteConfirmation.ToString();
                    break;
                case "7":
                    newStatus = Constants.F47_Status.AnEmptyRetrievalConfirm.ToString();
                    break;
                case "8":
                    newStatus = Constants.F47_Status.ADoubleInstallationConfirmation.ToString();
                    break;
                case "9":
                    newStatus = Constants.F47_Status.AnInstructionCancelConfirmation.ToString();
                    break;
                default:
                    newStatus = productWarehouseCommand.F47_Status;
                    break;
            }

            productWarehouseCommand.F47_Status = newStatus;
            productWarehouseCommand.F47_UpdateDate = DateTime.Now;
        }

        /// <summary>
        /// Update product shelf status table (TX51)
        /// </summary>
        /// <param name="to"></param>
        /// <param name="terminalNo"></param>
        private void UpdateProductShelfStatus(string to, string terminalNo)
        {
            // Find shelf row, bay , level from to.
            var shelfRow = to.Substring(0, 2);
            var shelfBay = to.Substring(2, 2);
            var shelfLevel = to.Substring(4, 2);

            // Find all product shelf statuses first.
            var productShelfStatuses = _unitOfWork.ProductShelfStatusRepository.GetAll();
            productShelfStatuses = productShelfStatuses.Where(x => x.F51_ShelfRow.Trim().Equals(shelfRow));
            productShelfStatuses = productShelfStatuses.Where(x => x.F51_ShelfBay.Trim().Equals(shelfBay));
            productShelfStatuses = productShelfStatuses.Where(x => x.F51_ShelfLevel.Trim().Equals(shelfLevel));

            foreach (var productShelfStatus in productShelfStatuses)
            {
                productShelfStatus.F51_StockTakingFlag = "1";
                productShelfStatus.F51_TerminalNo = terminalNo;
            }

        }

        /// <summary>
        /// Check validity of conveyor and device.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public void FindConveyorCodeValidity(string terminalNo)
        {
            //              	Retrieve [f05_strrtrsts] in TM05_CONVEYOR, where [f05_terminalno] = Terminal No. which is retrieved from section “server” in “toshiba.ini” configurable file.
            var conveyor = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo);
            if (conveyor == null || conveyor.F05_StrRtrSts == Constants.F05_StrRtrSts.Error.ToString("D"))
            {
                //If retrieval is failed or retrieved status is “Error” (or 9), system shows the message MSG 8, stop the use case.
                throw new Exception(HttpMessages.InvalidConveyorStatus);
            }

            //	Retrieve [f14_devicestatus] and [f14_usepermitclass] from TM14_DEVICE, where [f14_devicecode] = Product Device Code under “Device” section of “toshiba.ini” configurable file.
            //If retrieval is failed OR retrieved status is “Error” (or 2) OR “Offline” (or 1) OR retrieved permit is “Prohibited” (or 1), system shows the message MSG 9, stop the use case.
            var device =
                _unitOfWork.DeviceRepository.GetAll()
                    .FirstOrDefault(x => x.F14_DeviceCode.Equals(_configurationService.ProductDeviceCode));

            if (device == null || device.F14_DeviceStatus == Constants.F14_DeviceStatus.Offline.ToString("D") ||
                device.F14_DeviceStatus == Constants.F14_DeviceStatus.Error.ToString("D") ||
                device.F14_UsePermitClass == Constants.F14_UsePermitClass.Prohibited.ToString("D"))
                throw new Exception(HttpMessages.InvalidWarehouseStatus);
        }
        
        /// <summary>
        /// From information to render message.
        /// </summary>
        /// <param name="aiInout"></param>
        /// <param name="asStatus"></param>
        /// <param name="asFrom"></param>
        /// <param name="asTo"></param>
        /// <param name="aiStock"></param>
        /// <param name="achType"></param>
        /// <param name="asCode"></param>
        /// <param name="asPltcntn"></param>
        private string FormatWarehouseMessageBox(string aiInout, string asStatus, string asFrom, string asTo, string aiStock, string achType, string asCode, string asPltcntn)
        {
            // Enumeration conversion.
            var TX34_StrRtrType_Mtr = Constants.TX34_StrRtrType.Material.ToString("D");
            var TX47_StrRtrType_Pdt = Constants.F47_StrRtrType.Product.ToString("D");
            var TX47_StrRtrType_ExtPrePdt = Constants.F47_StrRtrType.ExternalPreProduct.ToString("D");
            var TX47_StrRtrType_BadPrePdt = Constants.F47_StrRtrType.OutOfSpecPreProduct.ToString("D");
            var TX50_StrRtrType_PrePdt = Constants.F50_StrRtrType.StrRtrType_PrePdt.ToString("D");

            //	The system will generate the message for Warehouse as below
            var message = "Warehouse: ";

            //	The system will check the value of [ai_stock]:
            //o	If [ai_inout] = “0”: Set the [Message] as follow:
            if (aiInout.Equals("0"))
                message += "Storage ";
            else if (aiInout.Equals("1"))
                message += "Retrieval ";
            else if (aiInout.Equals("2"))
                message += "Move ";

            //	Set the [Message] as follow
            //[Message] + " from "+as_From+" to "+as_To +"~r~n"
            message = string.Format("{0} from {1} to {2} ", message, asFrom, asTo);

            //	The system will check the value of [ai_inout]:
            //o	If [ai_inout] = “1” and [ach_type] = “TX50_StrRtrType_PrePdt” : Set the [Message] as follow:
            if (aiInout.Equals("0") && achType.Equals(TX34_StrRtrType_Mtr))
            {
                //[Message] + "Material Code: " + as_code + "~r~n" + "Pallet No: " + as_pltcntn + "~r~n"
                message = string.Format("{0} Material Code: {1} Pallet No: {2}", message, asCode, asPltcntn);
            }
            else if (aiInout.Equals("1") && achType.Equals(TX50_StrRtrType_PrePdt))
            {
                //[Message] + "Pre-product Code: " + as_code + "~r~n" + "Container Code: " + as_pltcntn + "~r~n"
                message = string.Format("{0} Pre-product Code: {1} Container Code: {2}", message, asCode, asPltcntn);
            }
            else if (aiInout.Equals("2"))
            {
                //•	[ach_type] = “TX47_StrRtrType_Pdt” (which is “0” (Pre-product) as pre-configured on “constant.txt file) : Set the [Message] as follow
                if (achType.Equals(TX47_StrRtrType_Pdt))
                    message = string.Format("{0} Product Code: {1} Pallet No: {2}", message, asCode, asPltcntn);
                else if (achType.Equals(TX47_StrRtrType_ExtPrePdt))
                    message = string.Format("{0} External Pre-Product Code: {1} Pallet No: {2}", message, asCode,
                        asPltcntn);
                else if (achType.Equals(TX47_StrRtrType_BadPrePdt))
                    message = string.Format("{0} Out-of-sign Pre-product ", message);
            }

            //	The system will check the value of [as_status]:
            //o	If [ai_status] = “tc_cmdsts_6”, set [Message] as follow:
            if (asStatus.Equals(Constants.TC_CMDSTS.TC_CMDSTS_6))
                message = string.Format("{0} Status: success.", message);
            else if (asStatus.Equals(Constants.TC_CMDSTS.TC_CMDSTS_7))
                message = string.Format("{0} Status: cancel.", message);
            else if (asStatus.Equals(Constants.TC_CMDSTS.TC_CMDSTS_8))
                message = string.Format("{0} Status: two times storage.");
            else if (asStatus.Equals(Constants.TC_CMDSTS.TC_CMDSTS_9))
                message = string.Format("{0} Status: empty retrieval.");

            return message;
        }

        /// <summary>
        /// Assign container number (Refer BR 4 - PreProduct Management Sub System - 1.0 Sign off)
        /// </summary>
        /// <returns></returns>
        private string AssignContainerNo()
        {
            // TX37_ShfSts_Epy
            var shelfStatusEmpty = Constants.F37_ShelfStatus.EmptyShelf.ToString("D");

            var preProductShelfStatuses = _unitOfWork.PreProductShelfStatusRepository.GetAll();
            preProductShelfStatuses =
                preProductShelfStatuses.Where(x => x.F37_ShelfStatus.Trim().Equals(shelfStatusEmpty));
            preProductShelfStatuses =
                preProductShelfStatuses.Where(x => x.F37_CmnShfAgnOrd != null);
            preProductShelfStatuses = preProductShelfStatuses.OrderByDescending(x => x.F37_CmnShfAgnOrd);

            // Find the first match result.
            var preProductShelfStatus = preProductShelfStatuses.FirstOrDefault();
            if (preProductShelfStatus == null)
                return null;

            return string.Format("{0}{1}{2}", preProductShelfStatus.F37_ShelfRow, preProductShelfStatus.F37_ShelfBay,
                preProductShelfStatus.F37_ShelfLevel);

        }

        public void ShowMessage(int ai_inout, string as_status, string as_from, string as_to, int ai_stock,
            string ach_type, string as_code, string as_pltcntn)
        {
            var message = "";
            switch (ai_stock)
            {
                case 0:
                    message += " Material <br/>";
                    break;
                case 1:
                    message += " Pre-product <br/>";
                    break;
                case 2:
                    message += " Product <br/>";
                    break; 
            }
            switch (ai_inout)
            {
                case 0:
                    message += "Storage";
                    break; 
                case 1:
                    message += "Retrieval";
                    break; 
                case 2:
                    message += "Move";
                    break;
                    
            }
            message += " from " + as_from + " to " + as_to + "<br/>";
            switch (ai_stock)
            {
                case 0:
                    if (ach_type==Constants.TX34_StrRtrType.Material.ToString("D"))
                    {
                        message += "Material Code: " + as_code + "<br/>";
                        message += "Pallet No: " + as_pltcntn + "<br/>";
                    }
                    break;
                case 1:
                   if (ach_type==Constants.F50_StrRtrType.StrRtrType_PrePdt.ToString("D"))
                    {
                        message += "Pre-product Code: " + as_code + "<br/>";
                        message += "Pallet No:  " + as_pltcntn + "<br/>";
                    }
                    break;
                case 2:
                    switch (ach_type)
                    {
                        case "0":
                            message += "Product Code: " + as_code + "<br/>";
                            message += "Pallet No: " + as_pltcntn + "<br/>";
                            break;
                        case "2":
                             message += "External Pre-product Code:  " + as_code + "<br/>";
                            message += "Pallet No:" + as_pltcntn + "<br/>";
                            break; 
                        case "3":
                            message += "Out-of-sign Pre-product " + "<br/>";
                            break;
                    }
                   
                    break; 
            }
            switch (as_status)
            {
                case "6":
                    message += "Status: success.";
                    break; 
                case "7":
                    message += "Status: cancel.";
                    break;
                case "8":
                    message += "Status: two times storage.";
                    break; 
                case "9":
                    message += "Status: empty retrieval.";
                    break;
            }

            //todo linhnd20
//MessageBox("WareHouse Message", ls_message)
//openwithparm(w_hsmess_win, ls_message)
          
        }

        #endregion
    }
}