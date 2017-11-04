using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Core.Resources;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.ProductManagement
{
    public class RetrievalOfExternalPreProductDomain : BaseDomain, IRetrievalOfExternalProductDomain
    {
        #region Properties

        /// <summary>
        /// Provides function to access common domain.
        /// </summary>
        private readonly ICommonDomain _commonDomain;
        
        /// <summary>
        /// Service which handles configuration.
        /// </summary>
        private readonly IConfigurationService _configurationService;

        /// <summary>
        /// Service which handles notification on system.
        /// </summary>
        private readonly INotificationService _notificationService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initiate domain with unit of work.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="commonDomain"></param>
        /// <param name="configurationService"></param>
        /// <param name="notificationService"></param>
        public RetrievalOfExternalPreProductDomain(IUnitOfWork unitOfWork,
            ICommonDomain commonDomain, 
            IConfigurationService configurationService,
            INotificationService notificationService) : base(unitOfWork, configurationService)
        {
            _commonDomain = commonDomain;
            _configurationService = configurationService;
            _notificationService = notificationService;
        }

        #endregion

        /// <summary>
        /// Find external pre-products asynchronously.
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseResult<GridResponse<ExternalPreProductItem>>> FindExternalPreProductsAsync(GridSettings gridSettings)
        {
            // Find all pre-products.
            var preProducts = _unitOfWork.PreProductRepository.GetAll();

            // Find all table listing commands.
            var tableListingCommands = _unitOfWork.TabletCommandRepository.GetAll();

            // Find all kneading commands.
            var kneadingCommands = _unitOfWork.KneadingCommandRepository.GetAll();

            /*
             * 	Data in the data window is retrieved from TM03_PREPRODUCT, TX41_TBTCMD and TX42_KNDCMD, in which:
             * 	[f41_status] = “not yet tabletised” (or 3), OR “Retrieval but stopped” (or 5),
             * 	[f41_tblcntamt] – [f41_rtrendcntamt] > 0,
             * 	[f42_outsideclass] = “Outside product” (or 1),
             * 	Ascending order by [f41_kndcmdno], then [f41_prepdtlotno].
             */
            var externalPreProductClass = Constants.F42_OutSideClass.ExternalPreProduct.ToString("D");


            // Find list of external pre products.
            var externalPreProducts = from tableListingCommand in tableListingCommands
                                      join kneadingCommand in kneadingCommands on new { key1 = tableListingCommand.F41_PrePdtLotNo, key2 = tableListingCommand.F41_PreproductCode } equals new { key1 = kneadingCommand.F42_PrePdtLotNo,key2=kneadingCommand.F42_PreProductCode }
                                      join preProduct in preProducts on tableListingCommand.F41_PreproductCode equals preProduct.F03_PreProductCode

                                      where
                                      Constants.F41_Status.NotTablet.Equals(tableListingCommand.F41_Status.Trim()) ||
                                      Constants.F41_Status.RetrievalingStoped.Equals(tableListingCommand.F41_Status.Trim())
                                      && (tableListingCommand.F41_TblCntAmt - tableListingCommand.F41_RtrEndCntAmt > 0)
                                      && externalPreProductClass.Equals(kneadingCommand.F42_OutSideClass)
                                      orderby new
                                      {
                                          tableListingCommand.F41_KndCmdNo,
                                          tableListingCommand.F41_PrePdtLotNo
                                      }
                                      select new ExternalPreProductItem()
                                      {
                                          F03_PreProductName = preProduct.F03_PreProductName,
                                          F41_KndCmdNo = tableListingCommand.F41_KndCmdNo,
                                          F41_RtrEndCntAmt = tableListingCommand.F41_RtrEndCntAmt,
                                          F41_TblCntAmt = tableListingCommand.F41_TblCntAmt,
                                          F41_PrePdtLotNo = tableListingCommand.F41_PrePdtLotNo,
                                          F41_PreProductCode = tableListingCommand.F41_PreproductCode,
                                          F41_TableLine = tableListingCommand.F41_TabletLine,
                                          F42_ThrowAmount = kneadingCommand.F42_ThrowAmount
                                      };

            // Count total number of records before pagination.
            var totalRecords = await externalPreProducts.CountAsync();

            // Order and paging.
            if (gridSettings != null)
                OrderByAndPaging(ref externalPreProducts, gridSettings);

            var resultModel = new GridResponse<ExternalPreProductItem>(externalPreProducts, totalRecords);
            return new ResponseResult<GridResponse<ExternalPreProductItem>>(resultModel, true);

        }

        /// <summary>
        /// Find table listing line.
        /// </summary>
        /// <returns></returns>
        public async Task<TM14_Device> FindDeviceTableListingLine(string tableListingLine)
        {
            // Find all devices.
            var devices = _unitOfWork.DeviceRepository.GetAll();

            /*
             * •	Search for [Tabletising Line Name] from [f14_devicename] in TM14_DEVICE, where [f14_devicecode] = "TAB0" + [Tabletising Line]. 
             * If no record found, set both [Tabletising Line] and [Tabletising Line Name] to blank
             */
            var deviceName = string.Format("TAB0{0}", tableListingLine);
            devices = devices.Where(x => x.F14_DeviceCode.Trim().Equals(deviceName));

            return await devices.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieve external pre product asynchronously by using specific conditions.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="commandNo"></param>
        /// <param name="preProductCode"></param>
        /// <param name="prePdtLotNo"></param>
        /// <param name="tableListingLine"></param>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public async Task RetrievalExternalPreProductAsync(string terminalNo, string commandNo, string preProductCode, string prePdtLotNo, string tableListingLine)
        {
            // o	Retrieve [f05_strrtrsts] in TM05_CONVEYOR, where [f05_terminalno] = Terminal No. which is retrieved from section “server” in “toshiba.ini” configurable file.
            var conveyor = await _commonDomain.FindConveyorCodeAsync(terminalNo);

            // Conveyor count.
            var statusError = Constants.F05_StrRtrSts.Error.ToString("D");
            if (conveyor == null || statusError.Equals(conveyor.F05_StrRtrSts))
                throw new Exception(HttpMessages.InvalidConveyorStatus);

            /*
             * o	Retrieve [f14_devicestatus] and [f14_usepermitclass] from TM14_DEVICE, where [f14_devicecode] = Pre-product Device Code under “Device” section of “toshiba.ini” configurable file.
             * If retrieval is failed OR retrieved status is “Error” (or 2) OR “Offline” (or 1) OR retrieved permit is “Prohibited” (or 1), system shows the message MSG 9, stop the use case.
             */
            var device = await _commonDomain.FindDeviceAvailabilityAsync(_configurationService.ProductDeviceCode);

            // Enum conversion.
            var statusOffline = Constants.F14_DeviceStatus.Offline.ToString("D");
            var statusDeviceError = Constants.F14_DeviceStatus.Error.ToString("D");
            var statusProhibited = Constants.F14_UsePermitClass.Prohibited.ToString("D");

            if (device == null || statusOffline.Equals(device.F14_DeviceStatus) ||
                statusDeviceError.Equals(device.F14_DeviceStatus) || device.F14_UsePermitClass.Equals(statusProhibited))
                throw new Exception(HttpMessages.InvalidDeviceAvailability);

            /*
             * 	System will get tablet line from “Line” column of selected row.
             * o	If it is not blank, then continue the use case.
             * o	If it is blank, then update TX41_TBTCMD, in which:
             * •	[f41_kndcmdno] = “Command No” column of selected line,
             * •	[f41_prepdtlotno] = “Pre-product Lot No" column of selected line.
             * Then:
             * •	Set [f41_tabletline] = "TAB0" + [Tabletising Line].
             * If no record is updated after this query, system shows the message MSG 47 and stops the use case.
             */

            var tableListingCommands = _unitOfWork.TabletCommandRepository.GetAll();
            tableListingCommands = tableListingCommands.Where(x => x.F41_KndCmdNo.Trim().Equals(commandNo));
            tableListingCommands = tableListingCommands.Where(x => x.F41_PrePdtLotNo.Trim().Equals(prePdtLotNo));

            var totalTableListingLines = await tableListingCommands.CountAsync();
            if (totalTableListingLines < 1)
                throw new Exception(HttpMessages.RecordIsBeingModified);
            
            foreach (var tableListingCommand in tableListingCommands)
            {
                if (string.IsNullOrWhiteSpace(tableListingCommand.F41_TabletLine))
                {
                    tableListingCommand.F41_TabletLine = "TAB0" + tableListingLine.Substring(tableListingLine.Length - 2);
                    _unitOfWork.TabletCommandRepository.Update(tableListingCommand);
                }

            }
            
            /*
             * System starts the Retrieval process by doing as follow:
             * o	Get Command No, Pre-product Code and Pre-product Lot No of selected row.
             * o	System will get pallet no for the pre-product by doing as follow:
             * •	Suppose Pallet No and Updated Date are temporary variables which are retrieved respectively from [f53_palletno] and [f53_updatedate] in TX53_OUTSIDEPREPDTSTK, in which:
             * 	[f53_outsideprepdtcode] = Pre-product Code of selected row,
             * 	AND [f53_outsideprepdtlotno] = Pre-product Lot No of selected row,
             * 	AND [f53_kndcmdno] = Command No of selected row,
             * 	AND [f53_stockflag] = “In Stock” (or 3), 
             * 	Ascending order by [f53_palletseqno].
             * If no matched record from the query, system shows the message MSG 30 and stops the use case.
             */
            var outsidePrePdtStks = _unitOfWork.OutSidePrePdtStkRepository.GetAll();
            outsidePrePdtStks = outsidePrePdtStks.Where(x => x.F53_OutSidePrePdtCode.Trim().Equals(preProductCode.Trim()));
            outsidePrePdtStks = outsidePrePdtStks.Where(x => x.F53_OutSidePrePdtLotNo.Trim().Equals(prePdtLotNo.Trim()));
            outsidePrePdtStks = outsidePrePdtStks.Where(x => x.F53_KndCmdNo.Trim().Equals(commandNo.Trim()));
            outsidePrePdtStks =
                outsidePrePdtStks.Where(x => Constants.F53_StokcFlag.TX53_StkFlg_Stk.Equals(x.F53_StockFlag.Trim()));
            outsidePrePdtStks = outsidePrePdtStks.OrderBy(x => x.F53_PalletSeqNo);

            var totalOutsidePrePdtStk = await outsidePrePdtStks.CountAsync();
            if (totalOutsidePrePdtStk < 1)
                throw new Exception(HttpMessages.RecordIsBeingModified);

            foreach (var outsidePrePdtStk in outsidePrePdtStks) {
                outsidePrePdtStk.F53_StockFlag = Constants.F53_StokcFlag.TX53_StkFlg_Rtr;
            }

            var outsidepPdtStk = await outsidePrePdtStks.FirstOrDefaultAsync();
            /*
             * o	System will assign shelf by doing as follow:
             * •	Suppose Row, Bay, Level and Updated Date1 are temporary variables which are retrieved respectively from 
             * [f51_shelfrow], [f51_shelfbay], [f51_shelflevel] and [f51_updatedate] in TX51_PDTSHFSTS, in which:
             * 	[f51_palletno] = Pallet No above,
             * 	AND [f51_shelfstatus] = “External Pre-Products Stocked” (or 8). 
             * If no record found, system shows the message MSG 47 and stops the use case.
             * •	(Reference xxiv) Update TX51_PDTSHFSTS, in which: 
             * 	[f51_shelfrow] = Row above,
             * 	AND [f51_shelfbay] = Bay above, 
             * 	AND [f51_shelflevel] = Level above,
             * 	AND [f51_shelfstatus] = “External Pre-Products Stocked” (or 8),
             * 	AND [f51_updatedate] = Updated Date 1 above.
             * If no record updated from the query, system shows the message MSG 47 and stops the use case.
             */

            var productShelfStatuses = _unitOfWork.ProductShelfStatusRepository.GetAll();
            productShelfStatuses = productShelfStatuses.Where(x => x.F51_PalletNo.Trim().Equals(outsidepPdtStk.F53_PalletNo.Trim()));
            productShelfStatuses =
                productShelfStatuses.Where(
                    x => Constants.F51_ShelfStatus.TX51_ShfSts_ExtPrePdt.Equals(x.F51_ShelfStatus));

            var totalProductShelfStatuses = await productShelfStatuses.CountAsync();
            if (totalProductShelfStatuses < 1)
            {
                throw new Exception(HttpMessages.RecordIsBeingModified);
            }

            foreach (var productShelfStatuse in productShelfStatuses)
            {
                productShelfStatuse.F51_ShelfStatus = "5";
                _unitOfWork.ProductShelfStatusRepository.Update(productShelfStatuse);

                break;
            }

            TX51_PdtShfSts pdtShfSts = null;
            foreach (var productShelfStatus in productShelfStatuses)
                pdtShfSts = productShelfStatus;

            /*
             * o	System will assign Tablet command by updating TX41_TBTCMD, in which: 
             * •	[f41_kndcmdno] = Command No of selected row,
             * •	AND [f41_prepdtlotno] = Pre-product Lot No of selected row,
             * •	AND [f41_status] = “not yet tabletised” (or 3), OR “Retrieval but stopped” (or 5), OR “Retrieval process” (or 4)
             * If no record updated from the query, system shows the message MSG 47 and stops the use case.
             */
            var tbtCmds = _unitOfWork.TabletCommandRepository.GetAll();
            tbtCmds = tbtCmds.Where(x => x.F41_KndCmdNo.Trim().Equals(commandNo.Trim())
                                         && x.F41_PrePdtLotNo.Trim().Equals(prePdtLotNo.Trim())
                                         &&
                                         (Constants.F41_Status.NotTablet.Equals(x.F41_Status.Trim()) ||
                                          Constants.F41_Status.RetrievalingStoped.Equals(x.F41_Status.Trim())));

            if (await tbtCmds.CountAsync() < 1)
                throw new Exception(HttpMessages.RecordIsBeingModified);

            var totalNoManageItems = await _commonDomain.InsertIntoNoManageAsync();
            await _unitOfWork.CommitAsync();
            var serialNo = string.Format("0000{0}", totalNoManageItems);
            serialNo = serialNo.Substring(serialNo.Length - 4);

            /*
             * •	Insert data to TX47_PDTWHSCMD:
             * 	[f47_commandno] = “2000” (for Retrieval),   
             * 	[f47_cmdseqno] = Serial No above,   
             * 	[f47_commandtype] = “0000”,   
             * 	[f47_strrtrtype] = “External pre-product” (or 2),   
             * 	[f47_status] = “An instruction” (or 0),   
             * 	[f47_priority] = 0,   
             * 	[f47_palletno] = Pallet No above,
             * 	[f47_from] = Row + Bay + Level above,   
             * 	[f47_to] = [f05_conveyorcode] from “tm05_conveyor” table, where [f05_terminalno] = Terminal No. which is retrieved from section “server” in “toshiba.ini” configurable file,   
             * 	[f47_commandsenddate] = blank,   
             * 	[f47_commandenddate] = blank,   
             * 	[f47_terminalno] = Terminal No. which is retrieved from section “server” in “toshiba.ini” configurable file,   
             * 	[f47_pictureno] = “TCPR101F”,   
             * 	[f47_abnormalcode] = blank,   
             * 	[f47_adddate] = current date time,   
             * 	[f47_updatedate] = current date time,   
             * 	[f47_updatecount] = blank.
             */
            var f47To = GetConveyorCode(terminalNo);
             var productWarehouseCommand = new TX47_PdtWhsCmd();
            productWarehouseCommand.F47_CommandNo = Constants.F47_CommandNo.Retrieval.ToString("D");
            productWarehouseCommand.F47_CmdSeqNo = serialNo;
            productWarehouseCommand.F47_CommandType = "0000";
            productWarehouseCommand.F47_StrRtrType = Constants.F47_StrRtrType.ExternalPreProduct.ToString("D");
            productWarehouseCommand.F47_Status = Constants.F47_Status.AnInstruction.ToString("D");
            productWarehouseCommand.F47_Priority = 0;
            productWarehouseCommand.F47_PalletNo = outsidepPdtStk.F53_PalletNo;
            if (pdtShfSts != null)
                productWarehouseCommand.F47_From = string.Format("{0}{1}{2}", pdtShfSts.F51_ShelfRow, pdtShfSts.F51_ShelfBay, pdtShfSts.F51_ShelfLevel);
            productWarehouseCommand.F47_To = f47To;
            productWarehouseCommand.F47_TerminalNo = terminalNo;
            productWarehouseCommand.F47_PictureNo = Constants.PictureNo.TCPR101F;
            productWarehouseCommand.F47_AddDate = DateTime.Now;
            productWarehouseCommand.F47_UpdateDate = DateTime.Now;
            productWarehouseCommand.F47_AbnormalCode = "";

           _unitOfWork.ProductWarehouseCommandRepository.Add(productWarehouseCommand);
            
            // Save changes into database.
            await _unitOfWork.CommitAsync();
            
            // Broadcast message to third communication.
            _notificationService.SendMessageToC3("TCPR101F", _notificationService.FormatThirdCommunicationMessageResponse(productWarehouseCommand));
        }

        /// <summary>
        /// Respond message sent back from C3.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="preProductCode"></param>
        /// <param name="kneadingCommandNo"></param>
        /// <param name="preProductLotNo"></param>
        public IList<ThirdCommunicationResponse> Reply(string terminalNo, string preProductCode, string kneadingCommandNo, string preProductLotNo)
        {
           var lstTx47 =
                _unitOfWork.ProductWarehouseCommandRepository.GetByTerminalNoAndPictureNoAndStatus679(terminalNo,
                    Constants.PictureNo.TCPR101F);
            
            var items = new List<ThirdCommunicationResponse>();

            foreach (var tx47 in lstTx47)
            {
                if (string.IsNullOrEmpty(tx47.F47_Status) || tx47.F47_Status.Length != 1)
                    continue;

                var oldStatus = tx47.F47_Status;
                switch (tx47.F47_Status[0])
                {
                    case '6'://Command End

                        tx47.F47_Status = "C";
                        tx47.F47_UpdateDate = DateTime.Now;

                        var tableListingCommands = _unitOfWork.TabletCommandRepository.GetAll();
                        tableListingCommands =
                            tableListingCommands.Where(x => x.F41_KndCmdNo.Trim().Equals(kneadingCommandNo.Trim()));
                        tableListingCommands =
                            tableListingCommands.Where(x => x.F41_PrePdtLotNo.Trim().Equals(preProductLotNo.Trim()));

                        foreach (var tableListingCommand in tableListingCommands)
                        {
                            if (tableListingCommand.F41_TblCntAmt - tableListingCommand.F41_RtrEndCntAmt <= 1)
                            {
                                tableListingCommand.F41_RtrEndCntAmt += 1;
                                tableListingCommand.F41_Status = Constants.F41_Status.RetrievalOver;
                                continue;
                            }

                            tableListingCommand.F41_RtrEndCntAmt += 1;
                        }
                        
                        break;
                    case '7'://Command Cancel
                        tx47.F47_Status = "D";
                        tx47.F47_UpdateDate = DateTime.Now;

                        break;
                    case '9'://Command Error
                        tx47.F47_Status = "F";
                        tx47.F47_UpdateDate = DateTime.Now;

                        break;
                }

                var item = AutoMapper.Mapper.Map<ThirdCommunicationResponse>(tx47);
                item.OldStatus = oldStatus;
                item.ProductCode = preProductCode;
                items.Add(item);
            }
            
            _unitOfWork.Commit();
                
            return items;
        }

        

        public string GetTabletingLine(string Line)
        {
            var result = _unitOfWork.DeviceRepository.GetAll().Where(i => i.F14_DeviceCode.ToLower().StartsWith("tab"));
            
               var item = result.Where(i => i.F14_DeviceCode.ToUpper().Contains(("TAB0" + Line).ToUpper())).FirstOrDefault();
            string tablelinename = string.Empty;
            if (item != null)
            {
                tablelinename = item.F14_DeviceName;
            }

            return tablelinename;
        }
    }
}