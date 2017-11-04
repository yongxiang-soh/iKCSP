using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.PreProductManagement
{
    public class ForcedRetrievalOfPreProductDomain :BaseDomain, IForcedRetrievalOfPreProductDomain
    {
        #region Properties

        /// <summary>
        /// Service which handles notifications between domains and communication server.
        /// </summary>
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Domain which handles common business.
        /// </summary>
        private readonly ICommonDomain _commonDomain;

        /// <summary>
        /// Unit of work
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructor

        /// <summary>
        /// Initiate domain with inversion of controls.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="notificationService"></param>
        /// <param name="configurationService"></param>
        /// <param name="commonDomain"></param>
        public ForcedRetrievalOfPreProductDomain(IUnitOfWork unitOfWork, INotificationService notificationService, IConfigurationService configurationService, ICommonDomain commonDomain) : base(unitOfWork,configurationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _commonDomain = commonDomain;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Search force retrieval of pre-product.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        public ResponseResult<GridResponse<ForcedRetrievalOfPreProductItem>> SearchCriteria(GridSettings gridSettings,
            string groupNameValue)
        {
            var cmdStatus = "";
            if (!string.IsNullOrEmpty(groupNameValue))
            {
                if (groupNameValue.Equals(Constants.GroupName.Tabletised.ToString("D")))
                {
                    cmdStatus = Constants.F42_Status.TX42_Sts_Tbtd;
                }
                else
                {
                    cmdStatus = Constants.F42_Status.TX42_Sts_Stored;
                }
            }
            else
            {
                cmdStatus = Constants.F42_Status.TX42_Sts_Stored;
            }

            var shfStatus = Constants.F37_ShelfStatus.Stock.ToString("D");
            var outsideclass = Constants.F42_OutSideClass.PreProduct.ToString("D");

            //find all records in tx42_kndcmd
            var kneadingCommands = _unitOfWork.KneadingCommandRepository.GetAll();

            //find all records in TM03_PreProduct
            var preProducts = _unitOfWork.PreProductRepository.GetAll();

            //find all records in TX49_PrePdtShfStk
            var preProductShelfStocks = _unitOfWork.PreProductShelfStockRepository.GetAll();

            //find all records in TX37_PrePdtShfSts
            var preProductShelfStatuses = _unitOfWork.PreProductShelfStatusRepository.GetAll();


            var results = from kneadingCommand in kneadingCommands
                from preProduct in preProducts
                from preProductShelfStock in preProductShelfStocks
                from preProductShelfStatus in preProductShelfStatuses
                where (
                    preProduct.F03_PreProductCode.Trim().Equals(kneadingCommand.F42_PreProductCode.Trim()) &&
                    preProduct.F03_PreProductCode.Trim().Equals(preProductShelfStock.F49_PreProductCode.Trim()) &&
                    preProductShelfStock.F49_ContainerCode.Trim()
                        .Equals(preProductShelfStatus.F37_ContainerCode.Trim()) &&
                    kneadingCommand.F42_PrePdtLotNo.Trim().Equals(preProductShelfStock.F49_PreProductLotNo) &&
                    kneadingCommand.F42_Status.Equals(cmdStatus) &&
                    preProductShelfStatus.F37_ShelfStatus.Equals(shfStatus) &&
                    kneadingCommand.F42_OutSideClass.Equals(outsideclass)
                    )
                select new ForcedRetrievalOfPreProductItem
                {
                    F42_KndCmdNo = kneadingCommand.F42_KndCmdNo,
                    F42_PreProductCode = kneadingCommand.F42_PreProductCode,
                    PreProductName = preProduct.F03_PreProductName,
                    F42_PrePdtLotNo = kneadingCommand.F42_PrePdtLotNo,
                    F49_ContainerCode = preProductShelfStock.F49_ContainerCode,
                    F37_ContainerNo = preProductShelfStatus.F37_ContainerNo,
                              F37_ShelfNo = preProductShelfStatus.F37_ShelfRow + preProductShelfStatus.F37_ShelfBay + preProductShelfStatus.F37_ShelfLevel
                };

            var itemCount = results.Count();
            // Sort and paging
            OrderByAndPaging(ref results, gridSettings);

            var lstResult =
                Mapper.Map<IEnumerable<TX42_KndCmd>, IEnumerable<ForcedRetrievalOfPreProductItem>>(results);

            var resultModel = new GridResponse<ForcedRetrievalOfPreProductItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<ForcedRetrievalOfPreProductItem>>(resultModel, true);
        }

        public List<ForcedRetrievalOfPreProductItem> Getallitem(string lotno, string groupNameValue)
        {
            var cmdStatus = "";
            if (!string.IsNullOrEmpty(groupNameValue))
            {
                if (groupNameValue.Equals(Constants.GroupName.Tabletised.ToString("D")))
                {
                    cmdStatus = Constants.F42_Status.TX42_Sts_Tbtd;
                }
                else
                {
                    cmdStatus = Constants.F42_Status.TX42_Sts_Stored;
                }
            }
            else
            {
                cmdStatus = Constants.F42_Status.TX42_Sts_Stored;
            }

            var shfStatus = Constants.F37_ShelfStatus.Stock.ToString("D");
            var outsideclass = Constants.F42_OutSideClass.PreProduct.ToString("D");

            //find all records in tx42_kndcmd
            var kneadingCommands = _unitOfWork.KneadingCommandRepository.GetAll();

            //find all records in TM03_PreProduct
            var preProducts = _unitOfWork.PreProductRepository.GetAll();

            //find all records in TX49_PrePdtShfStk
            var preProductShelfStocks = _unitOfWork.PreProductShelfStockRepository.GetAll();

            //find all records in TX37_PrePdtShfSts
            var preProductShelfStatuses = _unitOfWork.PreProductShelfStatusRepository.GetAll();


            var results = from kneadingCommand in kneadingCommands
                          from preProduct in preProducts
                          from preProductShelfStock in preProductShelfStocks
                          from preProductShelfStatus in preProductShelfStatuses
                          where (
                              preProduct.F03_PreProductCode.Trim().Equals(kneadingCommand.F42_PreProductCode.Trim()) &&
                              preProduct.F03_PreProductCode.Trim().Equals(preProductShelfStock.F49_PreProductCode.Trim()) &&
                              preProductShelfStock.F49_ContainerCode.Trim()
                                  .Equals(preProductShelfStatus.F37_ContainerCode.Trim()) &&
                              kneadingCommand.F42_PrePdtLotNo.Trim().Equals(preProductShelfStock.F49_PreProductLotNo) &&
                              kneadingCommand.F42_Status.Equals(cmdStatus) &&
                              preProductShelfStatus.F37_ShelfStatus.Equals(shfStatus) &&
                              kneadingCommand.F42_OutSideClass.Equals(outsideclass)
                              )
                          select new ForcedRetrievalOfPreProductItem
                          {
                              F42_KndCmdNo = kneadingCommand.F42_KndCmdNo,
                              F42_PreProductCode = kneadingCommand.F42_PreProductCode,
                              PreProductName = preProduct.F03_PreProductName,
                              F42_PrePdtLotNo = kneadingCommand.F42_PrePdtLotNo,
                              F49_ContainerCode = preProductShelfStock.F49_ContainerCode,
                              F37_ContainerNo = preProductShelfStatus.F37_ContainerNo,
                              F37_ShelfNo = preProductShelfStatus.F37_ShelfRow + preProductShelfStatus.F37_ShelfBay + preProductShelfStatus.F37_ShelfLevel
                          };


            var lstResult =
                Mapper.Map<IEnumerable<TX42_KndCmd>, IEnumerable<ForcedRetrievalOfPreProductItem>>(results).Where(m => m.F42_PrePdtLotNo.Equals(lotno)).ToList();
            return lstResult;

        }

        /// <summary>
        /// Retrieval of pre-product.
        /// </summary>
        /// <param name="commandNo"></param>
        /// <param name="lotNo"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public ResultSuccess Edit(string commandNo, string lotNo, string terminalNo, string containercode, string containerno, string shelfno)
        {
            var stoctype = "0";

            //Get record for tx42_kndcmd  
            var kneadingCommand =
                _unitOfWork.KneadingCommandRepository.GetMany(
                    i => i.F42_KndCmdNo.Trim().Equals(commandNo) && i.F42_PrePdtLotNo.Trim().Equals(lotNo));
            //Update tx42_kndcmd
            if (kneadingCommand.Any())
            {
                foreach (var item in kneadingCommand)
                {
                    item.F42_Status = Constants.F42_Status.TX42_Sts_FrcRtr;
                    _unitOfWork.KneadingCommandRepository.Update(item);
                }

            }
            //TODO implement for all item
            var lsRow = shelfno.Substring(0, 2);
            var lsBay = shelfno.Substring(2, 2);
            var lsLevel = shelfno.Substring(4, 2);
            //For each record on the table whose [Lot No.] = the value of [Lot No.] of the selected item,the system will update the data in database as follow
            //Update data for tx37 and tx49
            try
            {
                var stockStatus = Constants.F37_ShelfStatus.Stock.ToString("D");
                var prepdtshfstses = _unitOfWork.PreProductShelfStatusRepository.GetAll();
                prepdtshfstses = prepdtshfstses.Where(x => x.F37_ShelfRow.Equals(lsRow));
                prepdtshfstses = prepdtshfstses.Where(x => x.F37_ShelfBay.Equals(lsBay));
                prepdtshfstses = prepdtshfstses.Where(x => x.F37_ShelfLevel.Equals(lsLevel));
                prepdtshfstses = prepdtshfstses.Where(x => x.F37_ShelfStatus.Equals(stockStatus));
                if (prepdtshfstses.Any())
                {
                foreach (var item in prepdtshfstses)
                {
                    item.F37_ShelfStatus = Constants.F37_ShelfStatus.ReservedForRetrieval.ToString("D");
                    item.F37_TerminalNo = terminalNo;
                    item.F37_UpdateDate = DateTime.Now;
                    _unitOfWork.PreProductShelfStatusRepository.Update(item);

                    break;
                }
                }
                else
                {
                    return new ResultSuccess()
                    {
                        Success = false,
                        MessageError = "This record was modified by others, please retry"
                    };
                }

            }
            catch (Exception ex)
            {
                return new ResultSuccess()
                {
                    Success = false,
                    MessageError = "This record was modified by others, please retry"
                };
            }

            var preProductShelfStock =
                _unitOfWork.PreProductShelfStockRepository.GetMany(i => i.F49_ContainerCode.Trim().Equals(containercode));

            try
            {
                if (preProductShelfStock.Any())
                {
                foreach (var item in preProductShelfStock)
                {
                //update tx49
                    item.F49_ShelfStatus = Constants.F49_ShelfStatus.TX49_StkFlg_Rtr;
                    _unitOfWork.PreProductShelfStockRepository.Update(item);
            }
                }
                else
                {
                    return new ResultSuccess()
                    {
                        Success = false,
                        MessageError = "This record was modified by others, please retry"
                    };
                }


            }
            catch (Exception e)
            {
                return new ResultSuccess()
                {
                    Success = false,
                    MessageError = "This record was modified by others, please retry"
                };
            }
            //TODO  If the system updates data successfully, it will continue updating data of the following table:
            //TODO If there is any above actions is fail, the system will write the log file as “[TCIP061F] Can't update pre-product shelf status”.
            /*TODO The system will check the data for Conveyor and Warehouse as BR 8.
               TODO: If there is any action fail, the system will write a new line into the log file as “[TCIP061F] Get material warehouse command serial number failed”.
            */
            //The system will insert new record into tx50
            var preProductShelfStatusItem =
                _unitOfWork.PreProductShelfStatusRepository.GetMany(
                    i => i.F37_ContainerCode.Trim().Equals(containerno))
                    .FirstOrDefault();
            int result = 0;

                var isNomanage = true;
                // 	The system will check the status of the Conveyor and Pre-product Warehouse as BR 8. 
            result = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNomanage,
                    Constants.GetColumnInNoManager.PrePdtWhsCmdNo, prePdtWhsCmdNo: 1);
                //	If the system update the data on BR 8 successfully, it will insert new record into the table “tx50_prepdtwhscmd” in database as follow
                _unitOfWork.PreProductWarehouseCommandRepository.AddCommand(
                _commonDomain.FindConveyor(terminalNo).F05_ConveyorCode, result, containerno,
                containercode, shelfno, terminalNo, Constants.PictureNo.TCIP061F,
                    Constants.F50_CommandNo.CmdNoRtr.ToString("D"),
                Constants.F50_StrRtrType.StrRtrType_PrePdt, Constants.CommandType.CmdType_0);
            //if (preProductShelfStatusItem != null)
            //{
            //    var lsFrom = preProductShelfStatusItem.F37_ShelfRow  + preProductShelfStatusItem.F37_ShelfBay + 
            //                 preProductShelfStatusItem.F37_ShelfLevel;
            //    var containerNo = preProductShelfStatusItem.F37_ContainerNo;
            //    var containerCode = preProductShelfStatusItem.F37_ContainerCode;
 
            //    //  var conveyor = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo);

            //    //INSERT INTO tx50_prepdtwhscmd 
            //    //todo code cũ
            //    //var prePdtWhsCmd = new TX50_PrePdtWhsCmd();
            //    //prePdtWhsCmd.F50_CommandNo = Constants.F50_CommandNo.CmdNoRtr.ToString("D");
            //    //prePdtWhsCmd.F50_CmdSeqNo = _commonDomain.UpdateConveyorAndPreProductWarehouse();
            //    //prePdtWhsCmd.F50_CommandType = Constants.F50_CommandType.CmdType_0.ToString("D");
            //    //prePdtWhsCmd.F50_StrRtrType = Constants.F50_StrRtrType.StrRtrType_PrePdt.ToString("D");
            //    //prePdtWhsCmd.F50_Status = Constants.TC_CMDSTS.TC_CMDSTS_0;
            //    //prePdtWhsCmd.F50_ContainerNo = containerNo;
            //    //prePdtWhsCmd.F50_ContainerCode = containerCode;
            //    //prePdtWhsCmd.F50_Priority = 0;
            //    //prePdtWhsCmd.F50_From = lsFrom;
            //    //prePdtWhsCmd.F50_To = _commonDomain.FindConveyor(terminalNo).F05_ConveyorCode;
            //    //prePdtWhsCmd.F50_CommandSendDate = DateTime.Now;
            //    //prePdtWhsCmd.F50_CommandEndDate = DateTime.Now;
            //    //prePdtWhsCmd.F50_TerminalNo = terminalNo;
            //    //prePdtWhsCmd.F50_PictureNo = Constants.PictureNo.TCIP042F;

            //    //prePdtWhsCmd.F50_AddDate = DateTime.Now;
            //    //prePdtWhsCmd.F50_UpdateDate = DateTime.Now;
            //    //prePdtWhsCmd.F50_UpdateCount = 0;
            //    //prePdtWhsCmd.F50_RetryCount = 0;

            //    //_unitOfWork.PreProductWarehouseCommandRepository.Add(prePdtWhsCmd);

            //}


            _unitOfWork.Commit();
            

            // Send message as BR11
            _notificationService.SendMessageToC2(Constants.F50_CommandNo.CmdNoRtr.ToString("D"), result.ToString());

            return new ResultSuccess()
                {
                    Success = true,
                    MessageError = ""
                };
        }

        /// <summary>
        /// UC 19 - SRS 1.1
        /// Handle business of force retrieval of pre-product.
        /// </summary>
        /// <param name="preProductCode"></param>
        /// <param name="shelfNo"></param>
        /// <param name="cmdNo"></param>
        /// <param name="lotNo"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public List<SecondCommunicationResponse> ForcedRetrievalMessageC2Reply(string preProductCode, string shelfNo, string cmdNo, string lotNo,
            string terminalNo,bool isNotCommand)
        {
          
            //Write log with the content “tcip061f receive message from c2”
            //The system retrieve all the Command No. whose Terminal No. is the current terminal and the Screen ID is “TCIP021F” as follow:
            var pictureNo = Constants.PictureNo.TCIP061F;
            var cmdsts6 = Constants.TC_CMDSTS.TC_CMDSTS_6;
            var cmdsts7 = Constants.TC_CMDSTS.TC_CMDSTS_7;
            var cmdsts9 = Constants.TC_CMDSTS.TC_CMDSTS_9;

            var lstPreProductWarehouseCommand =
                _unitOfWork.PreProductWarehouseCommandRepository.GetMany(
                    i =>
                        i.F50_TerminalNo.Trim().Equals(terminalNo) && i.F50_PictureNo.Trim().Equals(pictureNo) &&
                        (i.F50_Status.Equals(cmdsts6) || i.F50_Status.Equals(cmdsts7) || i.F50_Status.Equals(cmdsts9)));
                   
            var items = new List<SecondCommunicationResponse>();
            foreach (var preProductWarehouseCommand in lstPreProductWarehouseCommand)
            {
                 var item = Mapper.Map<SecondCommunicationResponse>(preProductWarehouseCommand);
                item.PreProductCode = preProductCode;
                    var newStatus = preProductWarehouseCommand.F50_Status;
                    if (newStatus.Equals(cmdsts6))
                        newStatus = Constants.TC_CMDSTS.TC_CMDSTS_C;
                    else if (newStatus.Equals(cmdsts7))
                        newStatus = Constants.TC_CMDSTS.TC_CMDSTS_D;
                    else if (newStatus.Equals(cmdsts9))
                        newStatus = Constants.TC_CMDSTS.TC_CMDSTS_F;
                  
                        preProductWarehouseCommand.F50_Status = newStatus;
                        _unitOfWork.PreProductWarehouseCommandRepository.Update(preProductWarehouseCommand);
                        items.Add(item);
                    var lsFromold = preProductWarehouseCommand.F50_From;
                    //The system will check, if the ls_fromold which retrieved on SQL 60 row is the same the [Container Code] column of the selected row
                    if (lsFromold.Equals(shelfNo))
                    {
                        //If the [Tabletised] is ticked on the Screen 10, the system will close the waiting icon.
                        if (newStatus == Constants.TC_CMDSTS.TC_CMDSTS_C)
                        {
                            if (isNotCommand)
                            {
                                //The system will check the number of the Pre-product Warehouse Command using function f_tcpdtwhscmdover as defined on KCSG_Overall Requirement_SRS_v0.5 document. If the number > 0 and [No Command] is ticked on the Screen 10, the system will perform:
                            var kndcmd =
                                _unitOfWork.KneadingCommandRepository.GetMany(
                                    i => i.F42_KndCmdNo.Trim().Equals(cmdNo) && i.F42_PrePdtLotNo.Trim().Equals(lotNo))
                                    .FirstOrDefault();

                            if (kndcmd != null)
                                kndcmd.F42_Status = Constants.F42_Status.TX42_Sts_FrcCmp;


                            _unitOfWork.KneadingCommandRepository.Update(kndcmd);
                        }
                        }
                    }
                
            }

            _unitOfWork.Commit();
            return items;
        }

        #endregion
    }

    public class ResultSuccess
    {
        public bool Success { get; set; }
        public string MessageError { get; set; }
    }
}