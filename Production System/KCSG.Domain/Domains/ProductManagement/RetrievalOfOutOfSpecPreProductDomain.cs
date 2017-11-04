using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Core.Resources;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;
using log4net;

namespace KCSG.Domain.Domains.ProductManagement
{
    public class RetrievalOfOutOfSpecPreProductDomain : BaseDomain, IRetrievalOfOutOfSpecPreProductDomain
    {
        #region Properties

        /// <summary>
        ///     Service which is used for handling communication service.
        /// </summary>
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Domain which handles common tasks.
        /// </summary>
        private readonly ICommonDomain _commonDomain;

        /// <summary>
        /// Instance which is used for logging purpose.
        /// </summary>
        private readonly ILog _log;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initiate domain with IoC.
        /// </summary>
        /// <param name="iUnitOfWork"></param>
        /// <param name="notificationService"></param>
        /// <param name="commonDomain"></param>
        /// <param name="log"></param>
        public RetrievalOfOutOfSpecPreProductDomain(IUnitOfWork iUnitOfWork, 
            INotificationService notificationService,
            ICommonDomain commonDomain,
            IConfigurationService configurationService,
            ILog log)
            : base(iUnitOfWork, configurationService)
        {
            _notificationService = notificationService;
            _commonDomain = commonDomain;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Search items and display 'em to grid.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        public ResponseResult<GridResponse<TempTableItem>> SearchAsync(string[] status, GridSettings gridSettings)
        {
            var lstShelfStatus =
                _unitOfWork.ProductShelfStatusRepository.GetMany(
                    i => status.Contains(i.F51_ShelfStatus) && (i.F51_ShelfType == "1"));

            // Count total item before pagination happens.
            var totalRecords = lstShelfStatus.Count();
            // Order and paging.
            if (gridSettings != null)
                OrderByAndPaging(ref lstShelfStatus, gridSettings);
            var result = Mapper.Map<IEnumerable<TempTableItem>>(lstShelfStatus);
            var resultModel = new GridResponse<TempTableItem>(result, totalRecords);
            return new ResponseResult<GridResponse<TempTableItem>>(resultModel, true);
        }

        /// <summary>
        ///     Storage or retrieve pre-product asynchronously.
        /// </summary>
        /// <param name="shelfNo"></param>
        /// <param name="terminalNo"></param>
        /// <param name="shelfStatus"></param>
        /// <param name="emptyPallet"></param>
        /// <param name="storage"></param>
        /// <returns></returns>
        public ResponseResult StorageOrRetrieval(string shelfNo, string terminalNo, string shelfStatus, bool emptyPallet,
            bool storage)
        {
            //           	Suppose Row, Bay and Level are temporary variables.
            //o	If length of [Shelf No] = 8, then:

            var productShelfNo = _commonDomain.FindProductShelfInformation(shelfNo);
            if (productShelfNo == null)
                return new ResponseResult(false, "Invalid product shelf status");
                

            //	System will get Serial No by doing as follow:
            //o	Suppose Sequence No is selected from [f48_pdtwhscmdno] in TX48_NOMANAGE, where [f48_systemid] = “00000”.
            //o	If there is any record found:
            //•	Increment Sequence No by 1 (if it > 9999, then reset to 1),
            //•	Update [f48_pdtwhscmdno] in TX48_NOMANAGE = Sequence No, where [f48_systemid] = "00000".
            //•	Suppose Serial No is the temporary variable which set equal to 4 last characters of {“0000” + Sequence No}.
            //o	If no record found:
            //•	Insert data to TX48_NOMANAGE:
            //Set [f48_systemid] =”00000”,
            //Set [f48_megakndcmdno] = 0,
            //Set [f48_gnrkndcmdno] = 0,
            //Set [f48_mtrwhscmdno] = 0,
            //Set [f48_prepdtwhscmdno] = 0,
            //Set [f48_pdtwhscmdno] = 1,
            //Set [f48_kndcmdbookno] = 0,
            //Set [f48_adddate] and [f48_updatedate] = Current date time,
            //Set [f48_kneadsheefno] = 0,
            //Set [f48_outkndcmdno] = 0,
            //Set [f48_cnrkndcmdno] = 0,
            //•	Set Serial No to “0001”
            var isNoManage = false;
            var sequenceNo =
                _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                    Constants.GetColumnInNoManager.Pdtwhscmdno, 0, 0, 0, 0, 1).ToString("D4");

            //	Update table TX51_PDTSHFSTS, where:
            //o	[f51_shelfrow] = Row,
            //o	AND [f51_shelfBay] = Bay,
            //o	AND [f51_shelflevel] = Level, 
            //o	AND [f51_shelfstatus] = Shelf Status column value.
            //Then:
            //o	Set  [f51_shelfstatus] = “Assigned for Storage” (or 4), 
            //o	Set [f51_TerminalNo] = Terminal No. which is retrieved from section “server” in “toshiba.ini” configurable file,
            //o	Set [f51_Updatedate] = current date time.
            var productShelfStatusRepository =
                _unitOfWork.ProductShelfStatusRepository.GetMany(
                    i =>
                        (i.F51_ShelfRow == productShelfNo.Row) && (i.F51_ShelfBay == productShelfNo.Bay) && (i.F51_ShelfLevel == productShelfNo.Level) &&
                        i.F51_ShelfStatus.Trim().Equals(shelfStatus.Trim())).FirstOrDefault();

            // Product shelf status is not found.
            if (productShelfStatusRepository == null)
                return new ResponseResult(false, "Invalid product shelf status");

            if (storage)
            {
                _unitOfWork.ProductShelfStatusRepository.UpdateProductShelfStatus(productShelfStatusRepository,
                Constants.F51_ShelfStatus.TX51_ShfSts_RsvStr, terminalNo);
            }
            else
            {
                _unitOfWork.ProductShelfStatusRepository.UpdateProductShelfStatus(productShelfStatusRepository,
                Constants.F51_ShelfStatus.TX51_ShfSts_RsvRtr, terminalNo);
            }
            
            var commandNo = storage ? "1000" : "2000";

            // Find conveyor from database.
            var conveyor =
                _unitOfWork.ConveyorRepository.GetMany(i => i.F05_TerminalNo.Trim().Equals(terminalNo.Trim()))
                    .FirstOrDefault();

            // Conveyor is not found.
            if (conveyor == null)
                throw new Exception(HttpMessages.InvalidConveyorStatus);
            var status = Constants.F47_StrRtrType.PalletForWarehouse.ToString("D");
            if (!emptyPallet)
            {
                if (shelfStatus == "3")
                {
                    status = Constants.F47_StrRtrType.OutOfSpecPreProduct.ToString("D");
                }                
            }

            var from = storage ? conveyor.F05_ConveyorCode : productShelfNo.Row + productShelfNo.Bay + productShelfNo.Level;
            var to = storage ? productShelfNo.Row + productShelfNo.Bay + productShelfNo.Level : conveyor.F05_ConveyorCode;
            //var productWarehouseCommand =  _unitOfWork.ProductWarehouseCommandRepository.InsertProductWarehouseCommand(commandNo, sequenceNo, "0000",
            //    emptyPallet
            //        ? Constants.F47_StrRtrType.PalletForWarehouse.ToString("D")
            //        : Constants.F47_StrRtrType.OutOfSpecPreProduct.ToString("D"),
            //    Constants.F47_Status.AnInstruction.ToString("D"), "", from, to,
            //    terminalNo, Constants.PictureNo.TCPR111F);
            var productWarehouseCommand = _unitOfWork.ProductWarehouseCommandRepository.InsertProductWarehouseCommand(commandNo, sequenceNo, "0000",
                status,
                Constants.F47_Status.AnInstruction.ToString("D"), null, from, to,
                terminalNo, Constants.PictureNo.TCPR111F);


            //	Disable all buttons and reload the current page.
            //	System will send message to C3 by doing as follow:
            //o	Retrieve [f47_commandtype], [f47_status], [f47_palletno], [f47_from], [f47_to], [f47_terminalno], [f47_pictureNo] from TX47_PDTWHSCMD, where [f47_commandno] = “1000” (for Storage), [f47_cmdseqno] = Serial No above and [f47_status] = “An instruction” (or 0).
            //o	Send the following message to C3:
            //{“0001” + Text1 + Text2 + “0066” + Serial No + “1000” + Text3 + Text4 + Text5 + Text6 + Text 7 + Current system date}, in which:
            //•	Text1 is the retrieved value of [f47_terminalno] above. If its length < 4, then fill those first missing characters by blank space.
            //•	Text2 is the retrieved value of [f47_pictureNo] above. If its length < 8, then fill those first missing characters by blank space.
            //•	Text3 is the retrieved value of [f47_commandtype] above. If its length < 4, then fill those first missing characters by blank space.
            //•	Text4 is the retrieved value of [f47_status] above. If its length < 4, then fill those first missing characters by blank space.
            //•	Text5 is the retrieved value of [f47_from] above. If its length < 6, then fill those first missing characters by blank space.
            //•	Text6 is the retrieved value of [f47_to] above. If its length < 6, then fill those first missing characters by blank space.
            //•	Text7 is the retrieved value of [f47_palletno] above. If its length < 4, then fill those first missing characters by blank space.
            //o	Perform Responding Reply From C3 Rules (BR 81) once system receives response from C3

            //	Otherwise, if system already handles other functions, events or subroutines, display message MSG 7.
            _unitOfWork.Commit();

            // Format message which will be sent to C3.
            var message = _notificationService.FormatThirdCommunicationMessageResponse(productWarehouseCommand);

            // Broadcast message to C3.
            _log.Info(string.Format("Send {0} to C3", message));
            _notificationService.SendMessageToC3("TCPR101F", message);
            
            return new ResponseResult(true);
        }

        /// <summary>
        /// Respond messages sent back from C3.
        /// </summary>
        /// <param name="shelfNo"></param>
        /// <param name="terminalNo"></param>
        /// <param name="shelftStatus"></param>
        /// <returns></returns>
        public List<ThirdCommunicationResponse> ResponeMessageC3(string shelfNo, string terminalNo, string shelftStatus)
        {
            //            After received reply from C3, it will proceed properly by doing as follow:
            //	Suppose Command No 5, Sequence No 5, Status 5, Type 5, From 5 and To 5 are temporary variables which are retrieved from TX47_PDTWHSCMD, where:
            //	[F47_TerminalNo] = Terminal No. which is retrieved from section “server” in “toshiba.ini” configurable file 
            //	AND [F47_PictureNo] = “TCPR111F” 
            //	AND [F47_Status] = “Instruction complete” (6) or “Instruction cancel” (7) or “Double installation error” (8) or “Empty retrieval” (or 9), 
            //	Ascending order by [F47_AddDate].
            var tx47 =
                _unitOfWork.ProductWarehouseCommandRepository.GetMany(
                    i =>
                        (i.F47_PictureNo.Trim() == "TCPR111F") && i.F47_TerminalNo.Trim().Equals(terminalNo) &&
                        (i.F47_Status.Equals("6") || i.F47_Status.Equals("7") || i.F47_Status.Equals("8") ||
                         i.F47_Status.Equals("9"))).OrderBy(i => i.F47_AddDate);
            //	Suppose New Status 5 is the new status of the pre-product after system responded to C3.
            //	If Status 5 of the current record is neither “Instruction complete” (or 6), nor “Instruction cancel” (or 7), nor “Double installation error” (or 8), nor “Empty retrieval” (or 9), then continue with the next record which retrieved from the retrieval query. Otherwise, continue the process.
          var status = "";
            var items = new List<ThirdCommunicationResponse>();
            foreach (var tx47PdtWhsCmd in tx47)
            {
                var oldStatus = tx47PdtWhsCmd.F47_Status;

                switch (tx47PdtWhsCmd.F47_Status[0])
                {
                    case '6':
                        status  = Constants.F34_Status.statusC;
                        break;
                    case '7':
                        status = Constants.F34_Status.statusD;
                       
                        break;
                    case '8':
                        //o	If value of Shelf Status column of selected row = “Empty” (or 0):
                        if (shelftStatus == Constants.F51_ShelfStatus.TX51_ShfSts_Epy)
                        {
                           
                            var row = "";
                            var bay = "";
                            var level = "";
                            LoadShelfNo(shelfNo, ref row, ref bay, ref level);
                            var isCreate = true;

                          
                            var sequenceNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isCreate,
                                Constants.GetColumnInNoManager.Pdtwhscmdno, 0, 0, 0, 0, 1);
                            if (shelftStatus == "0")
                            {
                                shelftStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvStr;
                            }
                            else
                            {
                                shelftStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvRtr;
                            }
                            var tx51 =
                                _unitOfWork.ProductShelfStatusRepository.GetMany(
                                    i =>
                                        (i.F51_ShelfRow == row) && (i.F51_ShelfBay == bay) &&
                                        (i.F51_ShelfLevel == level) &&
                                        i.F51_ShelfStatus.Trim().Equals(shelftStatus.Trim())).FirstOrDefault();

                            if (tx51 != null)
                                _unitOfWork.ProductShelfStatusRepository.UpdateProductShelfStatus(tx51, Constants.F51_ShelfStatus.TX51_ShfSts_RsvStr, terminalNo);

                            var conveyor =
                                _unitOfWork.ConveyorRepository.GetMany(
                                        i => i.F05_TerminalNo.Trim().Equals(terminalNo.Trim()))
                                    .FirstOrDefault();
                            if (conveyor != null)
                            {
                                _unitOfWork.ProductWarehouseCommandRepository.InsertProductWarehouseCommand("1001",
                                    sequenceNo.ToString("D4"), "0000", tx47PdtWhsCmd.F47_StrRtrType,
                                    Constants.F47_Status.AnInstruction.ToString("D"), "", conveyor.F05_ConveyorCode,
                                    row + bay + level,
                                    terminalNo, Constants.PictureNo.TCPR111F);
                            }
                            status = Constants.F34_Status.statusE;
                        }
                        else
                        {
                            status = Constants.F34_Status.statusE;
                        }
                        break;
                    case '9':
                        status = Constants.F34_Status.statusF;
                        break;
                    
                }
                
                tx47PdtWhsCmd.F47_Status = status;
                _unitOfWork.ProductWarehouseCommandRepository.Update(tx47PdtWhsCmd);

                var item = Mapper.Map<ThirdCommunicationResponse>(tx47PdtWhsCmd);
                item.OldStatus = oldStatus;

                items.Add(item);
                
            }
            _unitOfWork.Commit();
            return items;
        }

        public bool CheckDeviceStatus(string deviceCode)
        {
            return _unitOfWork.DeviceRepository.CheckDeviceStatus(deviceCode);
        }

        public ResponseResult CheckConveyorAndDevice(string terminerNo, string deviceCode)
        {
            //              	Retrieve [f05_strrtrsts] in TM05_CONVEYOR, where [f05_terminalno] = Terminal No. which is retrieved from section “server” in “toshiba.ini” configurable file.
            var conveyor = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminerNo);
            if ((conveyor == null) || (conveyor.F05_StrRtrSts == Constants.F05_StrRtrSts.Error.ToString("D")))
                return new ResponseResult(false, "MSG8");
            var device = _unitOfWork.DeviceRepository.GetById(deviceCode);
            if ((device == null) || (device.F14_DeviceStatus == Constants.F14_DeviceStatus.Offline.ToString("D")) ||
                (device.F14_DeviceStatus == Constants.F14_DeviceStatus.Error.ToString("D")) ||
                (device.F14_UsePermitClass == Constants.F14_UsePermitClass.Prohibited.ToString("D")))
                return new ResponseResult(false, "MSG9");
            return new ResponseResult(true, "");
            //If retrieval is failed or retrieved status is “Error” (or 9), system shows the message MSG 8, stop the use case.
            //	Retrieve [f14_devicestatus] and [f14_usepermitclass] from TM14_DEVICE, where [f14_devicecode] = Product Device Code under “Device” section of “toshiba.ini” configurable file.
            //If retrieval is failed OR retrieved status is “Error” (or 2) OR “Offline” (or 1) OR retrieved permit is “Prohibited” (or 1), system shows the message MSG 9, stop the use case.
        }

        private void LoadShelfNo(string shelfNo, ref string row, ref string bay, ref string level)
        {
            //•	Set New Status 5 = “A double installation confirmation” (or E),
            //•	Re-run (Reference xiii).
            //•	Re-insert the command by doing as follow:
            //	Suppose Row, Bay and Level are temporary variables.
            //-	If length of [Shelf No] = 8, then:
            //	Set Row = 2 first characters of [Shelf No],
            //	Set Bay = 2 middle characters of [Shelf No] starts from the 4th position,
            //	Set Level = 2 last characters of [Shelf No].
            //-	Otherwise:
            //	Set Row = 2 first characters of [Shelf No],
            //	Set Bay = 2 middle characters of [Shelf No] starts from the 3rd position,
            //	Set Level = 2 last characters of [Shelf No].

            var slashedShelfNoRegex = new Regex(@"^\d{2}\-\d{2}\-\d{2}$");
            if (slashedShelfNoRegex.IsMatch(shelfNo))
            {
                var infos = shelfNo.Split('-');
                if (infos.Length != 3)
                    throw new Exception(HttpMessages.InvalidShelfNo);

                //•	Set Row = 2 first characters of [Shelf No],
                row = infos[0];

                //•	Set Bay = 2 middle characters of [Shelf No] starts from the 4th position,
                bay = infos[1];

                //•	Set Level = 2 last characters of [Shelf No].
                level = infos[2];
            }
            else
            {
                //o	Otherwise:
                //•	Set Row = 2 first characters of [Shelf No],
                row = shelfNo.Substring(0, 2);
                //•	Set Bay = 2 middle characters of [Shelf No] starts from the 3rd position,
                bay = shelfNo.Substring(3, 2);
                //•	Set Level = 2 last characters of [Shelf No].
                level = shelfNo.Substring(4, 2);
            }
        }

        #endregion
    }
}