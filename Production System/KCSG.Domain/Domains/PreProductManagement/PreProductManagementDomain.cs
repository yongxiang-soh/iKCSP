using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.PreProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.PreProductManagement;

namespace KCSG.Domain.Domains.PreProductManagement
{
    public class PreProductManagementDomain : BaseDomain, IPreProductManagementDomain
    {
        #region Properties

        /// <summary>
        /// Service which handles communication between modules.
        /// </summary>
        protected readonly INotificationService _notificationService;

        protected readonly IConfigurationService _configurationService;

        /// <summary>
        /// Service which handles common businesses.
        /// </summary>
        private readonly ICommonDomain _commonDomain;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initiate domain with dependency injections.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="notificationService"></param>
        /// <param name="configurationService"></param>
        /// <param name="commonDomain"></param>
        public PreProductManagementDomain(IUnitOfWork unitOfWork,
            INotificationService notificationService, IConfigurationService configurationService,
            ICommonDomain commonDomain)
            : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
            _configurationService = configurationService;
            _commonDomain = commonDomain;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Please refer BR9 - Displaying rules - SQL10
        /// </summary>
        /// <param name="kneadingLine"></param>
        /// <returns></returns>
        public async Task<IList<StorageOfPreProductItem>> FindEmptyContainers(Constants.KndLine kneadingLine)
        {
            // 	By default, the [Line] = “Conventional”, the system will retrieve data for “Conventional” line
            //	If user change [Line] = “Megabit”, the system will automatically retrieve data for “Megabit” line and shows data on form
            //	The system will display the data for this selected value of [Line] as follow (SQL10)

            var selectedKneadingLine = kneadingLine.ToString("D");

            // Find all preproduct in database.
            var preProducts = _unitOfWork.PreProductRepository.GetAll();

            // Find all kneading commands in database.
            var kneadingCommands = _unitOfWork.KneadingCommandRepository.GetAll();

            var outsideClassPreProduct = Constants.F42_OutSideClass.PreProduct.ToString("D");

            var f42Status = Constants.F42_Status.TX42_Sts_Cmp;

            // Please refer (BR9 - SQL10)
            var result = from preProduct in preProducts
                from kneadingCommand in kneadingCommands
                where
                    kneadingCommand.F42_PreProductCode.Trim().Equals(preProduct.F03_PreProductCode.Trim())
                    && preProduct.F03_KneadingLine.Equals(selectedKneadingLine)
                    && kneadingCommand.F42_Status.Equals(f42Status)
                    && kneadingCommand.F42_OutSideClass.Equals(outsideClassPreProduct)
                orderby new
                {
                    kneadingCommand.F42_KndCmdBookNo,
                    kneadingCommand.F42_CommandSeqNo,
                    kneadingCommand.F42_LotSeqNo
                }
                select new StorageOfPreProductItem
                {
                    KneadingLine = kneadingLine,
                    Colour = preProduct.F03_ColorClass,
                    Temperature = preProduct.F03_LowTmpClass,
                    ContainerType = preProduct.F03_ContainerType,
                    CommandNo = kneadingCommand.F42_KndCmdNo,
                    PreProductCode = preProduct.F03_PreProductCode,
                    PreProductName = preProduct.F03_PreProductName,
                    PreProductLotNo = kneadingCommand.F42_PrePdtLotNo,
                    StoragedContainerQuantity = kneadingCommand.F42_StgCtnAmt,
                };

            return await result.ToListAsync();
        }

        /// <summary>
        ///     Find conveyor by using kneading line & terminal no & colorClass.
        ///     Please refer (BR9 - Displaying rules - SQL11 & SQL12)
        /// </summary>
        /// <param name="kneadingLine"></param>
        /// <param name="terminalNo"></param>
        /// <param name="colorClass"></param>
        /// <returns></returns>
        public async Task<IList<string>> FindConveyors(Constants.KndLine kneadingLine, string terminalNo,
            string colorClass)
        {
            // Find all conveyors in database.
            var conveyors = _unitOfWork.ConveyorRepository.GetAll();

            // Convert kneading line from enum to string.
            var kneadingLineNo = kneadingLine.ToString("D");

            //o	If [Line] = “Megabit”, the system will retrieve conveyor as follow
            if (kneadingLine == Constants.KndLine.Megabit)
                conveyors =
                    conveyors.Where(x => x.F05_TerminalNo.Equals(terminalNo) && x.F05_LineNo.Equals(kneadingLineNo));
            else
                conveyors =
                    conveyors.Where(x => x.F05_TerminalNo.Equals(terminalNo)
                                         && x.F05_LineNo.Equals(kneadingLineNo)
                                         && x.F05_ColorClass.Equals(colorClass));

            return await conveyors.Select(x => x.F05_ConveyorCode).ToListAsync();
        }


        /// <summary>
        ///     Please refer BR29 for more information.
        ///     This function is fore analyzing shelfNo into row, bay, level
        /// </summary>
        /// <param name="shelfNo"></param>
        /// <param name="lsRow"></param>
        /// <param name="lsBay"></param>
        /// <param name="lsLevel"></param>
        public void Br29(string shelfNo, out string lsRow, out string lsBay, out string lsLevel)
        {
            try
            {
                lsRow = shelfNo.Substring(0, 2);
                lsBay = shelfNo.Substring(2, 2);
                lsLevel = shelfNo.Substring(4, 2);
            }
            catch
            {
                // In case of parsing failed, 3 parameters should be null.
                lsRow = null;
                lsBay = null;
                lsLevel = null;
            }
        }

        /// <summary>
        ///     Please refer BR18 in document SRS 1.0 for more information.
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
        public async Task PreProductStorageOk(string lsRow, string lsBay, string lsLevel, string containerNo,
            string containerCode, Constants.KndLine kneadingLine, string terminalNo, string colorClass)
        {
            //	The system will release the shelf by updating the table “TX37_prepdtshfsts” in database as follow (Please refer to document)
            var preProductShelfStatuses = _unitOfWork.PreProductShelfStatusRepository.GetAll();

            preProductShelfStatuses = preProductShelfStatuses.Where(x => x.F37_ShelfRow.Equals(lsRow)
                                                                         && x.F37_ShelfBay.Equals(lsBay)
                                                                         && x.F37_ShelfLevel.Equals(lsLevel));

            foreach (var preProductShelfStatus in preProductShelfStatuses)
                preProductShelfStatus.F37_ShelfStatus = Constants.F37_ShelfStatus.EmptyShelf.ToString("D");

            var isNomanage = true;

            // 	The system will check the status of the Conveyor and Pre-product Warehouse as BR 8. 
            var result = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNomanage,
                Constants.GetColumnInNoManager.PrePdtWhsCmdNo, prePdtWhsCmdNo: 1);

            //var conveyor = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo);
            // Find conveyor base on color class and terminal.
            var conveyor = await _unitOfWork.ConveyorRepository.GetAll().FirstOrDefaultAsync(x => x.F05_ColorClass.Trim().Equals(colorClass) && x.F05_TerminalNo.Trim().Equals(terminalNo));
            if (conveyor == null)
                return;
            

            //	If the system update the data on BR 8 successfully, it will insert new record into the table “tx50_prepdtwhscmd” in database as follow
            _unitOfWork.PreProductWarehouseCommandRepository.AddCommand(conveyor.F05_ConveyorCode, result, containerNo,
                containerCode, lsRow + lsBay + lsLevel, terminalNo, Constants.PictureNo.TCIP022F,
                Constants.F50_CommandNo.CmdNoRtr.ToString("D"), Constants.F50_StrRtrType.StrRtrType_Container,
                Constants.CommandType.CmdType_1);

            _unitOfWork.Commit();
        }

        /// <summary>
        ///     Please refer the BR20 in SRS 1.0 for more information
        /// </summary>
        /// <param name="lsRow"></param>
        /// <param name="lsBay"></param>
        /// <param name="lsLevel"></param>
        /// <param name="containerNo"></param>
        /// <param name="containerCode"></param>
        /// <param name="conveyorCode"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public void ConfirmStorageNg(string lsRow, string lsBay, string lsLevel, string containerNo,
            string containerCode, string conveyorCode, string terminalNo)
        {
            var isNomanage = true;
            var result = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNomanage,
                Constants.GetColumnInNoManager.PrePdtWhsCmdNo, prePdtWhsCmdNo: 1);

            var conveyor = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo);

            //	If the system update the data on BR 8 successfully, it will insert new record into the table “tx50_prepdtwhscmd” in database as follow
            _unitOfWork.PreProductWarehouseCommandRepository.AddCommand(lsRow + lsBay + lsLevel, result, containerNo,
                containerCode, "CV214", terminalNo, Constants.PictureNo.TCIP022F,
                Constants.F50_CommandNo.CmdNoMove.ToString("D"), Constants.F50_StrRtrType.StrRtrType_Container,
                Constants.CommandType.CmdType_0);

            // TODO: C2 message handling

            _unitOfWork.Commit();
        }

        /// <summary>
        /// Retrieval of Pre-product rules - refer br 12 srs PreProduct Management Sub System 1.0
        /// </summary>
        /// <param name="containerType"></param>
        /// <param name="terminalNo"></param>
        /// <param name="kneadingLine"></param>
        /// <param name="colorClass"></param>
        public RetrieveOfEmptyContainerItem Retrieval(string containerType, string terminalNo,
            Constants.KndLine kneadingLine,
            string colorClass)
        {
            var f37ShelfStatus = Constants.F37_ShelfStatus.EmptyContainer.ToString("D");
            //get records in tx37
            var preProductShelfStatuses =
                _unitOfWork.PreProductShelfStatusRepository.GetMany(
                    i =>
                        i.F37_ContainerType.Trim().Equals(containerType) && i.F37_ShelfStatus.Equals(f37ShelfStatus) &&
                        i.F37_CmnShfAgnOrd != null && i.F37_ContainerNo != null).OrderBy(i => i.F37_CmnShfAgnOrd);

            if (!preProductShelfStatuses.Any())
                return null;

            var retrieveOfEmptyContainerItem = new RetrieveOfEmptyContainerItem();
            var containerNo = "";
            var containerCode = "";
            var from = "";
            var shelfRow = "";
            var shelfBay = "";
            var shelfLevel = "";
            var systemDate = DateTime.Now;
            containerCode = systemDate.ToString("yyMMddHHmmss");
            //If exists record update tx37 table
            foreach (var preProductShelfStatus in preProductShelfStatuses)
            {
                preProductShelfStatus.F37_ShelfStatus = Constants.F37_ShelfStatus.ReservedForRetrieval.ToString("D");
                _unitOfWork.PreProductShelfStatusRepository.Update(preProductShelfStatus);

                containerNo = preProductShelfStatus.F37_ContainerNo;
                from = preProductShelfStatus.F37_ShelfRow + preProductShelfStatus.F37_ShelfBay +
                       preProductShelfStatus.F37_ShelfLevel;

                shelfRow = preProductShelfStatus.F37_ShelfRow;
                shelfBay = preProductShelfStatus.F37_ShelfBay;
                shelfLevel = preProductShelfStatus.F37_ShelfLevel;

                break;
            }

            var isNomanage = true;
            // 	The system will check the status of the Conveyor and Pre-product Warehouse as BR 8. 
            var cmdSeqNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNomanage,
                Constants.GetColumnInNoManager.PrePdtWhsCmdNo, prePdtWhsCmdNo: 1);


            // Find all conveyors in database.
            var conveyors = _unitOfWork.ConveyorRepository.GetAll();

            // Convert kneading line from enum to string.
            var kneadingLineNo = kneadingLine.ToString("D");

            //o	If [Line] = “Megabit”, the system will retrieve conveyor as follow
            if (kneadingLine == Constants.KndLine.Megabit)
                conveyors =
                    conveyors.Where(x => x.F05_TerminalNo.Equals(terminalNo) && x.F05_LineNo.Equals(kneadingLineNo));
            else
                conveyors =
                    conveyors.Where(x => x.F05_TerminalNo.Equals(terminalNo)
                                         && x.F05_LineNo.Equals(kneadingLineNo)
                                         && x.F05_ColorClass.Equals(colorClass));

            var convcode = conveyors.FirstOrDefault().F05_ConveyorCode;
            var pictureNo = Constants.PictureNo.TCIP021F;
            var commandNo = Constants.F50_CommandNo.CmdNoRtr.ToString("D");
            //  var conveyor = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo);
            //	If the system update the data on BR 8 successfully, it will insert new record into the table “tx50_prepdtwhscmd” in database as follow
            _unitOfWork.PreProductWarehouseCommandRepository.AddCommand(convcode, cmdSeqNo, containerNo, containerCode,
                from, terminalNo, pictureNo, commandNo, Constants.F50_StrRtrType.StrRtrType_Container,
                Constants.CommandType.CmdType_0);

            _unitOfWork.Commit();
            retrieveOfEmptyContainerItem.ContainerCode = containerCode;
            retrieveOfEmptyContainerItem.ContainerNo = containerNo;
            retrieveOfEmptyContainerItem.ContainerType = containerType;
            retrieveOfEmptyContainerItem.ShelfRow = shelfRow;
            retrieveOfEmptyContainerItem.ShelfBay = shelfBay;
            retrieveOfEmptyContainerItem.ShelfLevel = shelfLevel;
            return retrieveOfEmptyContainerItem;
        }

        /// <summary>
        ///     Please refer BR10 - Retrieval of Pre-product rules -
        /// </summary>
        /// <returns></returns>
        public async Task<TX37_PrePdtShfSts> FindPrePdtShfSts(string containerType, string terminalNo)
        {
            var statusEmptyContainer = Constants.F37_ShelfStatus.EmptyContainer.ToString("D");

            // Find all product shelf statuses.
            var prePdtShfStses = _unitOfWork.PreProductShelfStatusRepository.GetAll();
            var prePdtShfSts = await prePdtShfStses.Where(x =>
                x.F37_ShelfStatus.Equals(statusEmptyContainer)
                && x.F37_ContainerType.Equals(containerType)
                && (x.F37_CmnShfAgnOrd != null)
                && (x.F37_ContainerNo != null))
                .OrderBy(x => x.F37_CmnShfAgnOrd)
                .FirstOrDefaultAsync();

            if (prePdtShfSts != null)
                prePdtShfSts.F37_ShelfStatus = Constants.F37_ShelfStatus.ReservedForRetrieval.ToString("D");
            else
                throw new Exception("MSG21");

            var isNomanage = true;
            // 	The system will check the status of the Conveyor and Pre-product Warehouse as BR 8. 
            var result = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNomanage,
                Constants.GetColumnInNoManager.PrePdtWhsCmdNo, prePdtWhsCmdNo: 1);
            //  var conveyor = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo);
            //	If the system update the data on BR 8 successfully, it will insert new record into the table “tx50_prepdtwhscmd” in database as follow
            _unitOfWork.PreProductWarehouseCommandRepository.AddCommand(prePdtShfSts.F37_ContainerNo, result,
                prePdtShfSts.F37_ContainerNo,
                "   ", GetGsCnvCode(terminalNo), terminalNo, Constants.PictureNo.TCIP011F,
                Constants.F50_CommandNo.CmdNoStrg.ToString("D"), Constants.F50_StrRtrType.StrRtrType_Container,
                Constants.CommandType.CmdType_0);
            _unitOfWork.Commit();

            return prePdtShfSts;
        }

        //BR4 Get ContainerNo after click button Assign
        public string GetStorageShelfNo()
        {
            var storageShelfNo = "";
            var emptyShelf = Constants.F37_ShelfStatus.EmptyShelf.ToString("D");
            var preProductShelfStatusItem =
                _unitOfWork.PreProductShelfStatusRepository.GetMany(
                    i =>
                        i.F37_ShelfStatus.Equals(emptyShelf) &&
                        (i.F37_CmnShfAgnOrd != null)).OrderByDescending(i => i.F37_CmnShfAgnOrd).FirstOrDefault();
            if (preProductShelfStatusItem != null)
                storageShelfNo = preProductShelfStatusItem.F37_ShelfRow.Trim() + "-" +
                                 preProductShelfStatusItem.F37_ShelfBay.Trim() + "-" +
                                 preProductShelfStatusItem.F37_ShelfLevel.Trim();
            return storageShelfNo;
        }

        //Br5 Validation rules
        public bool CheckedValidation(string containerNo)
        {
            var shelfStatusEmpCtn = Constants.F37_ShelfStatus.EmptyContainer.ToString("D");
            var preShelfStatus_stk = Constants.F37_ShelfStatus.Stock.ToString("D");

            var result =
                _unitOfWork.PreProductShelfStatusRepository.GetMany(
                    i =>
                        i.F37_ContainerNo.Trim().Equals(containerNo) &&
                        (i.F37_ShelfStatus.Equals(shelfStatusEmpCtn) || i.F37_ShelfStatus.Equals(preShelfStatus_stk)));

            if (result.Any())
                return false;
            return true;
        }

        /// <summary>
        /// BR6 Pre-productManagement Checking Status of Conveyor and Pre-product Warehouse
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>

        //Get [gs_CnvCode] is retrieved from [f05_conveyorcode] in table tm05_conveyor with terminalno current
        public string GetGsCnvCode(string terminalNo)
        {
            //Get record have terminalNo equals current terminalNo
            var conveyorItem =
                _unitOfWork.ConveyorRepository.GetMany(i => i.F05_TerminalNo.Trim().Equals(terminalNo))
                    .FirstOrDefault();

            if (conveyorItem == null)
                throw new Exception("Invalid conveyor");

            //Return value F05_ConveyorCode
            return conveyorItem.F05_ConveyorCode;
        }

        //BR7
        public void InsertAndUpdate(int containerNo, string storageShelfNo, string containerType, string terminalNo)
        {
            // Find shelf information.
            var shelfInformation = _commonDomain.FindProductShelfInformation(storageShelfNo);

            var shelfrow = shelfInformation.Row;
            var shelfbay = shelfInformation.Bay;
            var shelflevel = shelfInformation.Level;
            var shelfStatus = Constants.F37_ShelfStatus.EmptyShelf.ToString("D");

            var f37ContainerNo = containerNo.ToString("D3");

            var preProductShelfStatusItem =
                _unitOfWork.PreProductShelfStatusRepository.GetMany(
                    i =>
                        i.F37_ShelfRow.Trim().Equals(shelfrow) && i.F37_ShelfBay.Trim().Equals(shelfbay) &&
                        i.F37_ShelfLevel.Trim().Equals(shelflevel) && i.F37_ShelfStatus.Trim().Equals(shelfStatus))
                    .FirstOrDefault();


            //update table PreProductShelfStatus
            if (preProductShelfStatusItem != null)
            {
                preProductShelfStatusItem.F37_ShelfStatus =
                    Constants.F37_ShelfStatus.ReservedForStorage.ToString("D");
                preProductShelfStatusItem.F37_ContainerType = containerType;
                preProductShelfStatusItem.F37_ContainerNo = f37ContainerNo;
                preProductShelfStatusItem.F37_TerminalNo = terminalNo;
                preProductShelfStatusItem.F37_StorageDate = DateTime.Now;
                preProductShelfStatusItem.F37_UpdateDate = DateTime.Now;
                _unitOfWork.PreProductShelfStatusRepository.Update(preProductShelfStatusItem);
            }

            //Insert or update tx48
            var isNoManage = true;
            var as_cmdno = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                Constants.GetColumnInNoManager.PrePdtWhsCmdNo, 0, 1, 0, 0, 0).ToString("D4");
            //get tx50 record
            var commandNo = Constants.F50_CommandNo.CmdNoStrg.ToString("D");
            var tx50prePdtWhsCmd =
                _unitOfWork.PreProductWarehouseCommandRepository.Get(
                    i => i.F50_CommandNo.Trim().Equals(commandNo) && i.F50_CmdSeqNo.Equals(as_cmdno));

            //Insert data into tx50_prepdtwhscmd
            if (tx50prePdtWhsCmd == null)
            {
                var tx50_prePdtWhsCmd = new TX50_PrePdtWhsCmd();
                tx50_prePdtWhsCmd.F50_CommandNo = commandNo;
                tx50_prePdtWhsCmd.F50_CmdSeqNo = as_cmdno;
                tx50_prePdtWhsCmd.F50_CommandType = Constants.CommandType.CmdType_0;
                tx50_prePdtWhsCmd.F50_StrRtrType = Constants.F50_StrRtrType.StrRtrType_Container.ToString("D");
                tx50_prePdtWhsCmd.F50_Status = Constants.TC_CMDSTS.TC_CMDSTS_0;
                tx50_prePdtWhsCmd.F50_ContainerNo = f37ContainerNo;
                tx50_prePdtWhsCmd.F50_ContainerCode = "   ";
                tx50_prePdtWhsCmd.F50_Priority = 0;
                tx50_prePdtWhsCmd.F50_From = GetGsCnvCode(terminalNo);
                tx50_prePdtWhsCmd.F50_To = string.Format("{0}{1}{2}", shelfInformation.Row, shelfInformation.Bay,
                    shelfInformation.Level);
                tx50_prePdtWhsCmd.F50_CommandSendDate = DateTime.Now;
                tx50_prePdtWhsCmd.F50_CommandEndDate = DateTime.Now;
                tx50_prePdtWhsCmd.F50_TerminalNo = terminalNo;
                tx50_prePdtWhsCmd.F50_PictureNo = Constants.PictureNo.TCIP011F;
                tx50_prePdtWhsCmd.F50_AbnormalCode = null;
                tx50_prePdtWhsCmd.F50_AddDate = DateTime.Now;
                tx50_prePdtWhsCmd.F50_UpdateDate = DateTime.Now;
                tx50_prePdtWhsCmd.F50_UpdateCount = 0;
                tx50_prePdtWhsCmd.F50_RetryCount = 0;

                _unitOfWork.PreProductWarehouseCommandRepository.Add(tx50_prePdtWhsCmd);
            }
            _unitOfWork.Commit();

            //o	The system will send message to C2 system with the following parameters:
            //	TX50_CmdNo_Strg = “1000” (Storage) as configured on “constant.txt” file.
            //	[as_cmdno] as calculated as above.
            _notificationService.SendMessageToC2(Constants.F50_CommandNo.CmdNoStrg.ToString("D"), as_cmdno);
        }

        public bool CheckExistsTX50()
        {
            var isNomanage = true;
            var asCmdNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNomanage,
                Constants.GetColumnInNoManager.PrePdtWhsCmdNo, prePdtWhsCmdNo: 1);
            var commandNo = Constants.F50_CommandNo.CmdNoStrg.ToString("D");
            var preProductWarehouseCommand =
                _unitOfWork.PreProductWarehouseCommandRepository.GetAll()
                    .FirstOrDefault(
                        i => i.F50_CommandNo.Trim().Equals(commandNo) && i.F50_CmdSeqNo.Trim().Equals(asCmdNo));

            if (preProductWarehouseCommand != null)
                return false;
            return true;
        }


        public bool UpdateTX37(string temperature, string terminalNo, string preProductCode)
        {
            var preProduct =
                _unitOfWork.PreProductRepository.Get(i => i.F03_PreProductCode.Trim().Equals(preProductCode.Trim()));
            if (preProduct == null)
                return false;

            var shelfStatus = Constants.F37_ShelfStatus.EmptyShelf.ToString("D");
            var preProductShelfStatuses =
                _unitOfWork.PreProductShelfStatusRepository.GetMany(i => i.F37_ShelfStatus.Equals(shelfStatus));

            if (temperature == "0")
            {
                preProductShelfStatuses =
                    preProductShelfStatuses.Where(i => i.F37_LowTmpShfAgnOrd != null)
                        .OrderBy(i => i.F37_LowTmpShfAgnOrd);
            }
            else
            {
                preProductShelfStatuses =
                    preProductShelfStatuses.Where(i => i.F37_CmnShfAgnOrd != null).OrderBy(i => i.F37_CmnShfAgnOrd);
            }

            if (!preProductShelfStatuses.Any())
                return false;

            foreach (var preProductShelfStatus in preProductShelfStatuses)
            {
                preProductShelfStatus.F37_ShelfStatus = Constants.F37_ShelfStatus.ReservedForRetrieval.ToString("D");
                preProductShelfStatus.F37_StockTakingFlag =
                    Constants.F37_StockTakingFlag.InventoryNoChecked.ToString("D");
                preProductShelfStatus.F37_TerminalNo = terminalNo;
                _unitOfWork.PreProductShelfStatusRepository.Update(preProductShelfStatus);

                break;
            }
            _unitOfWork.Commit();
            return true;
        }

        /// <summary>
        /// Store material back into warehouse after it has been retrieved.
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
        /// <param name="colorClass"></param>
        public void Storage(string commandNo, string containerCode, string preProductLotNo, string preProductCode,
            double quantity, string containerNo, string containerType, string terminalNo, string lotEndFlag, string row, string bay, string level, string colorClass)
        {
            var containerSeqNo = 1;
            var preProductShelfStocks =
                _unitOfWork.PreProductShelfStockRepository.GetMany(
                    i =>
                        i.F49_KndCmdNo.Trim().Equals(commandNo.Trim()) &&
                        i.F49_PreProductLotNo.Trim().Equals(preProductLotNo.Trim()) &&
                        i.F49_PreProductCode.Trim().Equals(preProductCode.Trim()));
            if (preProductShelfStocks.Any())
                containerSeqNo = preProductShelfStocks.Max(i => i.F49_ContainerSeqNo) + 1;

            // Base on terminal number and color class to find the suitable conveyor.
            var conveyor = _unitOfWork.ConveyorRepository.GetAll()
                .FirstOrDefault(x => x.F05_ColorClass.Trim().Equals(colorClass) && x.F05_TerminalNo.Trim().Equals(terminalNo));

            // Convery is not found.
            if (conveyor == null)
                return;
            
            //Insert tx49
            var preProductShelfStock =
                _unitOfWork.PreProductShelfStockRepository.Get(i => i.F49_ContainerCode.Trim().Equals(containerCode));
            if (preProductShelfStock == null)
            {
                var item = new TX49_PrePdtShfStk();
                item.F49_ContainerCode = containerCode;
                item.F49_KndCmdNo = commandNo;
                item.F49_PreProductCode = preProductCode;
                item.F49_PreProductLotNo = preProductLotNo;
                item.F49_Amount = quantity;
                item.F49_ShelfStatus = Constants.F49_ShelfStatus.TX49_StkFlg_Str;
                item.F49_StorageDate = DateTime.Now;
                item.F49_RetrievalDate = null;
                item.F49_ContainerSeqNo = containerSeqNo;
                item.F49_AddDate = DateTime.Now;
                item.F49_UpdateDate = DateTime.Now;
                item.F49_UpdateCount = 0;
                item.F49_ContainerNo = containerNo;
                _unitOfWork.PreProductShelfStockRepository.Add(item);
            }

            //Update tx37
            var f37ShelfStatus = Constants.F37_ShelfStatus.EmptyShelf.ToString("D");
            var preProductShelfStatuses =
                _unitOfWork.PreProductShelfStatusRepository.GetAll().Where(w => w.F37_ShelfRow == row && w.F37_ShelfBay == bay && w.F37_ShelfLevel == level).FirstOrDefault();
                
            var lsShelf = "";
            preProductShelfStatuses.F37_ShelfStatus = Constants.F37_ShelfStatus.ReservedForStorage.ToString("D");
            preProductShelfStatuses.F37_ContainerCode = containerCode;
            preProductShelfStatuses.F37_ContainerNo = containerNo;
            preProductShelfStatuses.F37_TerminalNo = terminalNo;
            preProductShelfStatuses.F37_ContainerType = containerType;
            preProductShelfStatuses.F37_UpdateDate = DateTime.Now;

            lsShelf = preProductShelfStatuses.F37_ShelfRow + preProductShelfStatuses.F37_ShelfBay +
                          preProductShelfStatuses.F37_ShelfLevel;
            _unitOfWork.PreProductShelfStatusRepository.Update(preProductShelfStatuses);
         

            //Insert or update tx48
            var isNoManage = true;
            var as_cmdno = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                Constants.GetColumnInNoManager.PrePdtWhsCmdNo, 0, 1, 0, 0, 0).ToString("D4");

            //get tx50 record
            var f50CommandNo = Constants.F50_CommandNo.CmdNoStrg.ToString("D");
            var tx50prePdtWhsCmd =
                _unitOfWork.PreProductWarehouseCommandRepository.Get(
                    i => i.F50_CommandNo.Trim().Equals(f50CommandNo) && i.F50_CmdSeqNo.Equals(as_cmdno));

            //Insert data into tx50_prepdtwhscmd
            if (tx50prePdtWhsCmd == null)
            {
                var tx50_prePdtWhsCmd = new TX50_PrePdtWhsCmd();
                tx50_prePdtWhsCmd.F50_CommandNo = f50CommandNo;
                tx50_prePdtWhsCmd.F50_CmdSeqNo = as_cmdno;
                tx50_prePdtWhsCmd.F50_CommandType = Constants.CommandType.CmdType_0;
                tx50_prePdtWhsCmd.F50_StrRtrType = Constants.F50_StrRtrType.StrRtrType_PrePdt.ToString("D");
                tx50_prePdtWhsCmd.F50_Status = Constants.TC_CMDSTS.TC_CMDSTS_0;
                tx50_prePdtWhsCmd.F50_ContainerNo = containerNo;
                tx50_prePdtWhsCmd.F50_ContainerCode = containerCode;
                tx50_prePdtWhsCmd.F50_Priority = 0;
                tx50_prePdtWhsCmd.F50_From = conveyor.F05_ConveyorCode;
                tx50_prePdtWhsCmd.F50_To = lsShelf;
                tx50_prePdtWhsCmd.F50_CommandSendDate = DateTime.Now;
                tx50_prePdtWhsCmd.F50_CommandEndDate = DateTime.Now;
                tx50_prePdtWhsCmd.F50_TerminalNo = terminalNo;
                tx50_prePdtWhsCmd.F50_PictureNo = Constants.PictureNo.TCIP023F;
                tx50_prePdtWhsCmd.F50_AbnormalCode = null;
                tx50_prePdtWhsCmd.F50_AddDate = DateTime.Now;
                tx50_prePdtWhsCmd.F50_UpdateDate = DateTime.Now;
                tx50_prePdtWhsCmd.F50_UpdateCount = 0;
                tx50_prePdtWhsCmd.F50_RetryCount = 0;
                tx50_prePdtWhsCmd.F50_LotEndFlg = lotEndFlag;

                _unitOfWork.PreProductWarehouseCommandRepository.Add(tx50_prePdtWhsCmd);
            }
            _unitOfWork.Commit();
        }

        public IList<SecondCommunicationResponse> ReceiveMessageFromC2WhenClickStorage(string terminalNo,
            string preProductCode, string row,
            string bay, string level, string containerCode, string containerNo, string containerType, string commandNo,
            string preProductLotNo)
        {
            //The system will perform the following actions after it receives the message from C2:
            //	The system retrieve all the Command No. whose Terminal No. is the current terminal and the Screen ID is “TCIP021F” as follow: (Please refer SQL15)
            var prePdtWhsCmds = _unitOfWork.PreProductWarehouseCommandRepository.GetAll();
            prePdtWhsCmds = prePdtWhsCmds.Where(x => x.F50_TerminalNo.Equals(terminalNo)
                                                     && x.F50_PictureNo.ToUpper().Equals("TCIP023F")
                                                     &&
                                                     (x.F50_Status.Equals(Constants.TC_CMDSTS.TC_CMDSTS_6) ||
                                                      x.F50_Status.Equals(Constants.TC_CMDSTS.TC_CMDSTS_7) ||
                                                      x.F50_Status.Equals(Constants.TC_CMDSTS.TC_CMDSTS_8)));

            var status = "";
            var items = new List<SecondCommunicationResponse>();

            // Find the current system datetime.

            //	For each retrieved record on SQL 15, the system will perform
            foreach (var prePdtWhsCmd in prePdtWhsCmds)
            {
                var item = Mapper.Map<SecondCommunicationResponse>(prePdtWhsCmd);
                item.OldStatus = prePdtWhsCmd.F50_Status;

                //	The system will check the value of [Command_No]:
                //o	If the [Old_Status] = “tc_cmdsts_6” (that means Command End), the system will set a new parameter [New_Status] = “tc_cmdsts_c” (‘C’) as pre-configured on “constant.txt” file
                switch (prePdtWhsCmd.F50_Status[0])
                {
                    case '6':
                        status = Constants.TC_CMDSTS.TC_CMDSTS_C;
                        break;
                    case '7':
                        status = Constants.TC_CMDSTS.TC_CMDSTS_D;
                        break;
                    case '8':
                        status = Constants.TC_CMDSTS.TC_CMDSTS_E;
                        break;
                }

                if (prePdtWhsCmd.F50_Status[0] == '8')
                {
                    var preProductShelfStatus =
                        _unitOfWork.PreProductShelfStatusRepository.Get(
                            i =>
                                i.F37_ShelfRow.Equals(row) && i.F37_ShelfBay.Equals(bay) &&
                                i.F37_ShelfLevel.Equals(level));
                    if (preProductShelfStatus != null)
                    {
                        preProductShelfStatus.F37_ContainerCode = containerCode;
                        preProductShelfStatus.F37_ContainerNo = containerNo;
                        preProductShelfStatus.F37_ContainerType = containerType;
                        preProductShelfStatus.F37_TerminalNo = terminalNo;
                        preProductShelfStatus.F37_UpdateDate = DateTime.Now;
                        _unitOfWork.PreProductShelfStatusRepository.Update(preProductShelfStatus);
                    }
                }

                if (prePdtWhsCmd.F50_Status[0] == '6')
                {
                    var preProductShelfStocks =
                        _unitOfWork.PreProductShelfStockRepository.GetMany(
                            i =>
                                i.F49_KndCmdNo.Trim().Equals(commandNo.Trim()) &&
                                i.F49_PreProductCode.Trim().Equals(preProductCode.Trim()) &&
                                i.F49_PreProductLotNo.Trim().Equals(preProductLotNo.Trim()));
                    double lotAmount = 0;
                    if (preProductShelfStocks.Any())
                    {
                        lotAmount = preProductShelfStocks.Sum(i => i.F49_Amount);
                    }
                    var kneadingCommands =
                        _unitOfWork.KneadingCommandRepository.GetMany(
                            i =>
                                i.F42_KndCmdNo.Trim().Equals(commandNo.Trim()) &&
                                i.F42_PrePdtLotNo.Trim().Equals(preProductLotNo.Trim()));
                    foreach (var kneadingCommand in kneadingCommands)
                    {
                        kneadingCommand.F42_StgCtnAmt += 1;
                        kneadingCommand.F42_ThrowAmount = lotAmount;
                        kneadingCommand.F42_TrwEndDate = DateTime.Now;
                        kneadingCommand.F42_UpdateDate = DateTime.Now;


                        if (prePdtWhsCmd.F50_LotEndFlg[0] != '0')
                        {
                            kneadingCommand.F42_Status = Constants.F42_Status.TX42_Sts_Stored;
                        }

                        _unitOfWork.KneadingCommandRepository.Update(kneadingCommand);
                    }
                }
                prePdtWhsCmd.F50_Status = status;
                prePdtWhsCmd.F50_UpdateDate = DateTime.Now;
                item.PreProductCode = preProductCode;
                items.Add(item);
                _unitOfWork.PreProductWarehouseCommandRepository.Update(prePdtWhsCmd);
            }

            // Save changes into database.
            _unitOfWork.Commit();

            return items;
        }

        /// <summary>
        ///     Please refer BR12 for more information.
        /// </summary>
        /// <returns></returns>
        public IList<SecondCommunicationResponse> InitiateInformationFromC2(string terminalNo, string preProductCode)
        {
            //The system will perform the following actions after it receives the message from C2:
            //	The system retrieve all the Command No. whose Terminal No. is the current terminal and the Screen ID is “TCIP021F” as follow: (Please refer SQL15)
            var prePdtWhsCmds = _unitOfWork.PreProductWarehouseCommandRepository.GetAll();
            prePdtWhsCmds = prePdtWhsCmds.Where(x => x.F50_TerminalNo.Equals(terminalNo)
                                                     && x.F50_PictureNo.ToUpper().Equals("TCIP021F")
                                                     &&
                                                     (x.F50_Status.Equals(Constants.TC_CMDSTS.TC_CMDSTS_6) ||
                                                      x.F50_Status.Equals(Constants.TC_CMDSTS.TC_CMDSTS_7) ||
                                                      x.F50_Status.Equals(Constants.TC_CMDSTS.TC_CMDSTS_9)));

            var status = "";
            var items = new List<SecondCommunicationResponse>();

            // Find the current system datetime.

            //	For each retrieved record on SQL 15, the system will perform
            foreach (var prePdtWhsCmd in prePdtWhsCmds)
            {
                var item = Mapper.Map<SecondCommunicationResponse>(prePdtWhsCmd);
                item.OldStatus = prePdtWhsCmd.F50_Status;

                //	The system will check the value of [Command_No]:
                //o	If the [Old_Status] = “tc_cmdsts_6” (that means Command End), the system will set a new parameter [New_Status] = “tc_cmdsts_c” (‘C’) as pre-configured on “constant.txt” file
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
                items.Add(item);
                _unitOfWork.PreProductWarehouseCommandRepository.Update(prePdtWhsCmd);
            }

            // Save changes into database.
            _unitOfWork.Commit();

            return items;
        }

        /// <summary>
        ///     Please refer to BR13 for more information.
        /// </summary>
        public void InitiateWarehouseMessageBox()
        {
            // TODO: Implement this function.
        }

        /// <summary>
        ///     Find pre-product warehouse command asynchronously.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public IQueryable<TX50_PrePdtWhsCmd> FindPreProductWarehouseCommandAsync(string terminalNo)
        {
            // Find all pre-product warehouse commands.
            var prePdtWhsCmds = _unitOfWork.PreProductWarehouseCommandRepository.GetAll();
            prePdtWhsCmds = prePdtWhsCmds.Where(x => x.F50_TerminalNo.Equals(terminalNo)
                                                     && x.F50_PictureNo.ToUpper().Equals("TCIP021F")
                                                     &&
                                                     (x.F50_Status.Equals(Constants.TC_CMDSTS.TC_CMDSTS_6) ||
                                                      x.F50_Status.Equals(Constants.TC_CMDSTS.TC_CMDSTS_7) ||
                                                      x.F50_Status.Equals(Constants.TC_CMDSTS.TC_CMDSTS_9)));
            return prePdtWhsCmds;
        }


        /// <summary>
        ///     Processing data after receiving message from C2
        ///     Refer UC2 - Br9 - SRS PreProduct Management for more information
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="containerType"></param>
        /// <param name="containerNo"></param>
        /// <returns></returns>
        public List<SecondCommunicationResponse> ReceiveMessageFromC2(string terminalNo, string containerType,
            string containerNo)
        {
            //The system retrieve all the Command No. whose Terminal No. is the current terminal and the Screen ID is “TCIP022F”
            var pictureNo = Constants.PictureNo.TCIP011F;
            var cmdsts6 = Constants.TC_CMDSTS.TC_CMDSTS_6;
            var cmdsts7 = Constants.TC_CMDSTS.TC_CMDSTS_7;
            var cmdsts8 = Constants.TC_CMDSTS.TC_CMDSTS_8;

            var lstPreProductWarehouseCommand =
                _unitOfWork.PreProductWarehouseCommandRepository.GetMany(
                    i =>
                        i.F50_TerminalNo.Trim().Equals(terminalNo) && i.F50_PictureNo.Trim().Equals(pictureNo) &&
                        (i.F50_Status.Equals(cmdsts6) || i.F50_Status.Equals(cmdsts7) ||
                         i.F50_Status.Equals(cmdsts8)))
                    .OrderBy(i => i.F50_AddDate);

            var status = "";
            var items = new List<SecondCommunicationResponse>();
            //For each retrieved record on SQL 18, the system will perform The system will check the value of [Command_No]:

            foreach (var preProductWarehouseCommand in lstPreProductWarehouseCommand)
            {
                var item = Mapper.Map<SecondCommunicationResponse>(preProductWarehouseCommand);
                item.OldStatus = preProductWarehouseCommand.F50_Status;

                switch (preProductWarehouseCommand.F50_Status[0])
                {
                    case '6':
                        status = Constants.TC_CMDSTS.TC_CMDSTS_C;
                        break;
                    case '7':
                        status = Constants.TC_CMDSTS.TC_CMDSTS_D;
                        break;
                    case '8':
                        status = Constants.TC_CMDSTS.TC_CMDSTS_F;
                        break;
                }
                if (preProductWarehouseCommand.F50_Status == "8")
                {
                    var shelfNo = GetStorageShelfNo();
                    var shelfRow = shelfNo.Split('-')[0];
                    var shelfBay = shelfNo.Split('-')[1];
                    var shelfLevel = shelfNo.Split('-')[2];
                    var shelfStatus = Constants.F37_ShelfStatus.EmptyShelf.ToString("D");
                    var preProductShelfStatus =
                        _unitOfWork.PreProductShelfStatusRepository.Get(
                            i =>
                                i.F37_ShelfRow.Equals(shelfRow) && i.F37_ShelfBay.Equals(shelfBay) &&
                                i.F37_ShelfLevel.Equals(shelfLevel) && i.F37_ShelfStatus.Equals(shelfStatus));
                    if (preProductShelfStatus != null)
                    {
                        preProductShelfStatus.F37_ShelfStatus =
                            Constants.F37_ShelfStatus.ReservedForStorage.ToString("D");
                        preProductShelfStatus.F37_TerminalNo = terminalNo;
                        preProductShelfStatus.F37_ContainerType = containerType;
                        preProductShelfStatus.F37_ContainerNo = containerNo;
                        preProductShelfStatus.F37_StorageDate = DateTime.Now;
                        preProductShelfStatus.F37_UpdateDate = DateTime.Now;
                        _unitOfWork.PreProductShelfStatusRepository.Update(preProductShelfStatus);
                    }
                }
                preProductWarehouseCommand.F50_Status = status;
                preProductWarehouseCommand.F50_UpdateDate = DateTime.Now;
                item.F50_ContainerCode = preProductWarehouseCommand.F50_ContainerCode;
                _unitOfWork.PreProductWarehouseCommandRepository.Update(preProductWarehouseCommand);
                item.PreProductCode = "";
                items.Add(item);
            }
            //o	Otherwise, the system will skip the current record and loop to the next record.
            _unitOfWork.Commit();

            return items;
        }

        /// <summary>
        ///     Updating Status for Pre-product Warehouse
        ///     Refer Br10 - Srs 1.0 PreProduct Management for more information
        /// </summary>
        /// <param name="commandNo"></param>
        /// <param name="cmdSeqNo"></param>
        /// <param name="oldStatus"></param>
        /// <param name="newStatus"></param>
        public void UpdateStatusForPreProductWarehouse(string commandNo, string cmdSeqNo, string oldStatus,
            string newStatus)
        {
            var preProductWarehouseCommand =
                _unitOfWork.PreProductWarehouseCommandRepository.GetAll().FirstOrDefault(
                    i =>
                        i.F50_CommandNo.Trim().Equals(commandNo) && i.F50_CmdSeqNo.Trim().Equals(cmdSeqNo) &&
                        i.F50_Status.Equals(oldStatus));
            if (preProductWarehouseCommand != null)
            {
                preProductWarehouseCommand.F50_Status = newStatus;
                preProductWarehouseCommand.F50_UpdateDate = DateTime.Now;
                _unitOfWork.PreProductWarehouseCommandRepository.Update(preProductWarehouseCommand);
            }

            //	If fail, the system will loop the next record.
            //	If success and if (the [Old_Status] = “tc_cmdsts_6”) or (the [Old_Status] = “tc_cmdsts_7”), the system will working with the Warehouse message box as BR 15 with the following parameters:
            //o	ai_inout = “0”.
            //o	as_status = Old_Status retrieved on SQL 10
            //o	ls_CnyvNo = From retrieved on SQL 21.
            //o	ls_ShelfNo = To retrieved on SQL 21.
            //o	ai_stock = “1”.
            //o	ach_type = “TX50_StrRtrType_Container” (which is “1” (Container) as pre-configured on “constant.txt” file).
            //o	as_code = “”.
            //o	as_pltcntn = “”.
            _unitOfWork.Commit();
        }

        /// <summary>
        /// Please refer UC-6 for more information.
        /// </summary>
        /// <returns></returns>
        public IList<SecondCommunicationResponse> AnalyzeC2MessageUc6(bool isChecked, string preProductCode,
            string containerCode,
            string terminalNo)
        {
            var pictureNo = Constants.PictureNo.TCIP022F;
            var cmdsts6 = Constants.TC_CMDSTS.TC_CMDSTS_6;
            var cmdsts7 = Constants.TC_CMDSTS.TC_CMDSTS_7;
            var lsStatus = Constants.TC_CMDSTS.TC_CMDSTS_8;
            if (isChecked)
                lsStatus = Constants.TC_CMDSTS.TC_CMDSTS_9;


            // SQL 21
            var prePdtWhsCmds =
                _unitOfWork.PreProductWarehouseCommandRepository.GetMany(x => x.F50_TerminalNo.Equals(terminalNo)
                                                                              && x.F50_PictureNo.Equals(pictureNo)
                                                                              &&
                                                                              (x.F50_Status.Equals(cmdsts6) ||
                                                                               x.F50_Status.Equals(cmdsts7) ||
                                                                               x.F50_Status.Equals(lsStatus)));
            prePdtWhsCmds = prePdtWhsCmds.OrderBy(x => x.F50_AddDate);

            var status = "";
            var items = new List<SecondCommunicationResponse>();
            foreach (var prePdtWhsCmd in prePdtWhsCmds)
            {
                var item = Mapper.Map<SecondCommunicationResponse>(prePdtWhsCmd);
                item.OldStatus = prePdtWhsCmd.F50_Status;

                switch (prePdtWhsCmd.F50_Status[0])
                {
                    case '6':
                        status = Constants.TC_CMDSTS.TC_CMDSTS_C;
                        break;
                    case '7':
                        status = Constants.TC_CMDSTS.TC_CMDSTS_D;
                        break;
                }

                prePdtWhsCmd.F50_Status = status;
                prePdtWhsCmd.F50_UpdateDate = DateTime.Now;
                _unitOfWork.PreProductWarehouseCommandRepository.Update(prePdtWhsCmd);
                item.PreProductCode = preProductCode;
                items.Add(item);
            }
            _unitOfWork.Commit();
            return items;
        }

        public string AnalyzeC2MessageTCIP023F(string lchStatus, string preProductCode, string shelfNo,
            string containerCode, string containerType, string containerNo, string terminalNo, string commandNo,
            string preProductLotNo, bool containerEndChecked)
        {
            // SQL 21
            var prePdtWhsCmds = _unitOfWork.PreProductWarehouseCommandRepository.GetAll();
            prePdtWhsCmds = prePdtWhsCmds.Where(x => x.F50_TerminalNo.Equals(terminalNo)
                                                     && x.F50_PictureNo.ToUpper().Equals(Constants.PictureNo.TCIP023F)
                                                     &&
                                                     (Constants.TC_CMDSTS.TC_CMDSTS_6.Equals(x.F50_Status.Trim()) ||
                                                      Constants.TC_CMDSTS.TC_CMDSTS_7.Equals(x.F50_Status.Trim()) ||
                                                      Constants.TC_CMDSTS.TC_CMDSTS_8.Equals(x.F50_Status.Trim())));
            prePdtWhsCmds = prePdtWhsCmds.OrderBy(x => x.F50_AddDate);

            var as_status = "";
            var as_from = "";
            var as_to = "";
            var ai_stock = "1";
            var ach_type = Constants.F50_StrRtrType.StrRtrType_PrePdt.ToString("D");
            var message = "";
            foreach (var prePdtWhsCmd in prePdtWhsCmds)
            {
                var as_code =
                    _unitOfWork.PreProductShelfStockRepository.GetMany(
                        i => i.F49_ContainerCode == prePdtWhsCmd.F50_ContainerCode);
                as_status = prePdtWhsCmd.F50_Status;
                if (prePdtWhsCmd.F50_Status.Trim().Equals(Constants.TC_CMDSTS.TC_CMDSTS_6))
                {
                    prePdtWhsCmd.F50_Status = Constants.TC_CMDSTS.TC_CMDSTS_C;
                    var tx42 =
                        _unitOfWork.KneadingCommandRepository.GetMany(
                            i => i.F42_KndCmdNo == commandNo && i.F42_PrePdtLotNo == preProductLotNo).FirstOrDefault();
                    if (tx42 != null)
                    {
                        if (prePdtWhsCmd.F50_LotEndFlg != "0")
                        {
                            tx42.F42_Status = Constants.F42_Status.TX42_Sts_Stored;
                        }
                        tx42.F42_StgCtnAmt += 1;
                        tx42.F42_ThrowAmount = getAmount(containerCode);
                        tx42.F42_TrwBgnDate = DateTime.Now;
                        tx42.F42_UpdateDate = DateTime.Now;
                        _unitOfWork.KneadingCommandRepository.Update(tx42);
                    }
                }
                if (prePdtWhsCmd.F50_Status.Trim().Equals(Constants.TC_CMDSTS.TC_CMDSTS_7))
                    prePdtWhsCmd.F50_Status = Constants.TC_CMDSTS.TC_CMDSTS_D;
                else
                {
                    if (shelfNo == prePdtWhsCmd.F50_To)
                    {
                        var tm03 = _unitOfWork.PreProductRepository.GetById(preProductCode);
                        if (tm03 != null)
                        {
                            var row = shelfNo.Substring(0, 2);
                            var bay = shelfNo.Substring(2, 2);
                            var level = shelfNo.Substring(4, 2);
                            var tx37 = _unitOfWork.PreProductShelfStatusRepository.GetByRowBayLevel(row, bay, level);
                            tx37.F37_ContainerCode = containerCode;
                            tx37.F37_ContainerNo = containerNo;
                            tx37.F37_TerminalNo = terminalNo;
                            tx37.F37_ContainerType = containerType;
                            tx37.F37_UpdateDate = DateTime.Now;
                            _unitOfWork.PreProductShelfStatusRepository.Update(tx37);
                            var noManage = true;
                            var seqNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref noManage,
                                Constants.GetColumnInNoManager.PrePdtWhsCmdNo, 0, 1, 0, 0, 0);
                            var conveCode = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo).F05_ConveyorCode;
                            var ls_lotendflg = containerEndChecked ? "0" : "1";
                            var tx50 = new TX50_PrePdtWhsCmd()
                            {
                                F50_CommandNo = Constants.F50_CommandNo.CmdNoTwoTimes.ToString("D"),
                                F50_CmdSeqNo = seqNo.ToString("D4"),
                                F50_CommandType = Constants.CommandType.CmdType_0,
                                F50_StrRtrType = "0",
                                F50_Status = "0",
                                F50_ContainerNo = containerNo,
                                F50_ContainerCode = containerCode,
                                F50_Priority = 0,
                                F50_From = conveCode,
                                F50_To = shelfNo,
                                F50_LotEndFlg = ls_lotendflg,
                                F50_CommandSendDate = DateTime.Now,
                                F50_CommandEndDate = DateTime.Now,
                                F50_TerminalNo = terminalNo,
                                F50_PictureNo = Constants.PictureNo.TCIP023F,
                                F50_AbnormalCode = null,
                                F50_AddDate = DateTime.Now,
                                F50_UpdateDate = DateTime.Now,
                                F50_UpdateCount = 0,
                                F50_RetryCount = 0,
                            };
                            _unitOfWork.PreProductWarehouseCommandRepository.Add(tx50);
                            prePdtWhsCmd.F50_Status = Constants.TC_CMDSTS.TC_CMDSTS_E;
                        }
                    }
                    else
                    {
                        prePdtWhsCmd.F50_Status = Constants.TC_CMDSTS.TC_CMDSTS_E;
                    }
                }

                prePdtWhsCmd.F50_UpdateDate = DateTime.Now;
                _unitOfWork.PreProductWarehouseCommandRepository.Update(prePdtWhsCmd);
                if (as_status == Constants.TC_CMDSTS.TC_CMDSTS_6 || as_status == Constants.TC_CMDSTS.TC_CMDSTS_7)
                {
                    //todo linhnd20 
                    // message= f_tcwhsmsgbox(0, lch_OldStatus, ls_CnyvNo, ls_ShelfNo, 1, TX50_StrRtrType_PrePdt, ls_PrePdtCode, ls_container_code )
                }
            }
            _unitOfWork.Commit();

            return message;
        }

        /// <summary>
        /// From information to render message.
        /// </summary>
        /// <param name="ai_inout"></param>
        /// <param name="as_status"></param>
        /// <param name="as_from"></param>
        /// <param name="as_to"></param>
        /// <param name="ai_stock"></param>
        /// <param name="ach_type"></param>
        /// <param name="as_code"></param>
        /// <param name="as_pltcntn"></param>
        private string WorkingWithWarehouseMessagebox(string ai_inout, string as_status, string as_from, string as_to,
            string ai_stock, string ach_type, string as_code, string as_pltcntn)
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
            if (ai_inout.Equals("0"))
                message += "Storage ";
            else if (ai_inout.Equals("1"))
                message += "Retrieval ";
            else if (ai_inout.Equals("2"))
                message += "Move ";

            //	Set the [Message] as follow
            //[Message] + " from "+as_From+" to "+as_To +"~r~n"
            message = string.Format("{0} from {1} to {2} ", message, as_from, as_to);

            //	The system will check the value of [ai_inout]:
            //o	If [ai_inout] = “1” and [ach_type] = “TX50_StrRtrType_PrePdt” : Set the [Message] as follow:
            if (ai_inout.Equals("0") && ach_type.Equals(TX34_StrRtrType_Mtr))
            {
                //[Message] + "Material Code: " + as_code + "~r~n" + "Pallet No: " + as_pltcntn + "~r~n"
                message = string.Format("{0} Material Code: {1} Pallet No: {2}", message, as_code, as_pltcntn);
            }
            else if (ai_inout.Equals("1") && ach_type.Equals(TX50_StrRtrType_PrePdt))
            {
                //[Message] + "Pre-product Code: " + as_code + "~r~n" + "Container Code: " + as_pltcntn + "~r~n"
                message = string.Format("{0} Pre-product Code: {1} Container Code: {2}", message, as_code, as_pltcntn);
            }
            else if (ai_inout.Equals("2"))
            {
                //•	[ach_type] = “TX47_StrRtrType_Pdt” (which is “0” (Pre-product) as pre-configured on “constant.txt file) : Set the [Message] as follow
                if (ach_type.Equals(TX47_StrRtrType_Pdt))
                    message = string.Format("{0} Product Code: {1} Pallet No: {2}", message, as_code, as_pltcntn);
                else if (ach_type.Equals(TX47_StrRtrType_ExtPrePdt))
                    message = string.Format("{0} External Pre-Product Code: {1} Pallet No: {2}", message, as_code,
                        as_pltcntn);
                else if (ach_type.Equals(TX47_StrRtrType_BadPrePdt))
                    message = string.Format("{0} Out-of-sign Pre-product ", message);
            }

            //	The system will check the value of [as_status]:
            //o	If [ai_status] = “tc_cmdsts_6”, set [Message] as follow:
            if (as_status.Equals(Constants.TC_CMDSTS.TC_CMDSTS_6))
                message = string.Format("{0} Status: success.", message);
            else if (as_status.Equals(Constants.TC_CMDSTS.TC_CMDSTS_7))
                message = string.Format("{0} Status: cancel.", message);
            else if (as_status.Equals(Constants.TC_CMDSTS.TC_CMDSTS_8))
                message = string.Format("{0} Status: two times storage.");
            else if (as_status.Equals(Constants.TC_CMDSTS.TC_CMDSTS_9))
                message = string.Format("{0} Status: empty retrieval.");

            return message;
        }

        private bool PreAssign(ref string address)
        {
            String row, bay, level;
            var tx37 =
                _unitOfWork.PreProductShelfStatusRepository.GetMany(
                    i => i.F37_ShelfStatus == "0" && i.F37_CmnShfAgnOrd.HasValue).FirstOrDefault();
            if (tx37 == null)
            {
                return false;
            }
            address = tx37.F37_ShelfRow + tx37.F37_ShelfBay + tx37.F37_ShelfLevel;
            return true;
        }

        private double getAmount(string as_concode)
        {
            var tx49 = _unitOfWork.PreProductShelfStockRepository.GetById(as_concode);

            return
                _unitOfWork.PreProductShelfStockRepository.GetMany(
                    i =>
                        i.F49_KndCmdNo == tx49.F49_KndCmdNo && i.F49_PreProductCode ==
                        tx49.F49_PreProductCode && i.F49_PreProductLotNo == tx49.F49_PreProductLotNo)
                    .Sum(i => i.F49_Amount);
        }

        /// <summary>
        /// Convert container type to container no.
        /// </summary>
        /// <param name="containerType"></param>
        /// <returns></returns>
        public string FindContainerNo(string containerType)
        {
            // Container type 
            if (string.IsNullOrEmpty(containerType))
                return "   ";

            switch (containerType.Length)
            {
                case 0:
                    return string.Format("   ");
                case 1:
                    return string.Format("00{0}", containerType);
                case 2:
                    return string.Format("0{0}", containerType);
                case 3:
                    return containerType;
            }

            return "   ";
        }

        #endregion
    }
}