using System;
using System.Collections.Generic;
using System.Linq;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.CommunicationManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.CommunicationManagement;
using KCSG.jsGrid.MVC;
using log4net;

namespace KCSG.Domain.Domains.CommunicationManagement
{
    public class Communication3Domain : BaseDomain, ICommunication3Domain
    {
        #region Properties

        /// <summary>
        /// Service which is used for broadcasting notifications.
        /// </summary>
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Instance which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion

        #region Constructor

        /// <summary>
        /// Initiate communication domain with IoC.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="notificationService"></param>
        /// <param name="configurationService"></param>
        /// <para></para>
        /// <param name="log"></param>
        public Communication3Domain(IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IConfigurationService configurationService,
            ILog log)
            : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
            _log = log;
        }

        #endregion

        public ResponseResult<GridResponse<HistoryItem>> GetHistory(DateTime? date, string terminal,
            GridSettings gridSettings)
        {
            var result = _unitOfWork.ProductWarehouseCommandHistoryRepository.GetAll();
            if (!string.IsNullOrEmpty(terminal))
            {
                result = result.Where(i => i.F66_TerminalNo.Trim().Equals(terminal.Trim()));
            }
            if (date.HasValue)
            {
                result = result.Where(i => i.F66_AddDate >= date);
            }
            var itemCount = result.Count();
            var lstResult = result.Select(i => new HistoryItem()
            {
                AbnormalCode = i.F66_AbnormalCode,
                AddDate = i.F66_AddDate,
                CommandNo = i.F66_CommandNo,
                CommandSeqNo = i.F66_CmdSeqNo,
                CommandType = i.F66_CommandType, //Enum.GetName(typeof(Constants.TX34_CmdType), i.F66_CommandType),
                From = i.F66_From,
                PalletNo = i.F66_PalletNo,
                Priority = i.F66_Priority,
                To = i.F66_To,
                CommandDate = i.F66_CommandSendDate
            });
            if (gridSettings != null)
                OrderByAndPaging(ref lstResult, gridSettings);


            var resultModel = new GridResponse<HistoryItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<HistoryItem>>(resultModel, true);

        }

        public ResponseResult<GridResponse<QueueItem>> GetQueue(DateTime? date, string terminal,
            GridSettings gridSettings)
        {
            var result = _unitOfWork.ProductWarehouseCommandRepository.GetAll();
            if (!string.IsNullOrEmpty(terminal))
            {
                result = result.Where(i => i.F47_TerminalNo.Trim().Equals(terminal.Trim()));
            }
            if (date.HasValue)
            {
                result = result.Where(i => i.F47_AddDate >= date);
            }
            var itemCount = result.Count();
            var lstResult = result.Select(i => new QueueItem()
            {
                AbnormalCode = i.F47_AbnormalCode,
                AddDate = i.F47_AddDate,
                CommandEndDate = i.F47_CommandEndDate,
                CommandNo = i.F47_CommandNo,
                CommandSendDate = i.F47_CommandSendDate,
                CommandSeqNo = i.F47_CmdSeqNo,
                CommandType = i.F47_CommandType, //Enum.GetName(typeof(Constants.TX34_CmdType), i.F47_CommandType),
                PalletNo = i.F47_PalletNo,
                From = i.F47_From,
                PictureNo = i.F47_PictureNo,
                Priority = i.F47_Priority,
                RetryCount = i.F47_RetryCount,
                Status = i.F47_Status,
                StrRtrTypeProduct = i.F47_StrRtrType,
                TerminalNo = i.F47_TerminalNo,
                To = i.F47_To,
                UpdateDate = i.F47_UpdateDate
            }).OrderBy(i => i.AddDate).AsQueryable(); 
            if (gridSettings != null)
                OrderByAndPaging(ref lstResult, gridSettings);
            var resultModel = new GridResponse<QueueItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<QueueItem>>(resultModel, true);
        }

        public bool SetDeviceStatus(bool online, string deviceCode)
        {
            if (online)
            {
                //	Update field [F47_RetryCount] of Preproduct Warehouse Command records whose [F47_Status] is 1, 2, 4 or 5 as 0.
                var lstTx47 =
                    _unitOfWork.ProductWarehouseCommandRepository.GetMany(
                        i => i.F47_Status == Constants.F34_Status.status2 ||
                             i.F47_Status == Constants.F34_Status.status1 ||
                             i.F47_Status == Constants.F34_Status.status4 ||
                             i.F47_Status == Constants.F34_Status.status5);

                foreach (var tx50PreproWhsCmd in lstTx47)
                {
                    tx50PreproWhsCmd.F47_RetryCount = 0;
                    _unitOfWork.ProductWarehouseCommandRepository.Update(tx50PreproWhsCmd);
                }
                //	Set [EventID] as “TID_RESEND”. 

                //	Update [F14_DeviceStatus] and [F14_UpdateDate] of [TM14_Device] whose [F14_DeviceCode] is the value
                //which is configured by key “DeviceCode” in the configuration file as 0 and current date and time.
                //If there is any occurred Error, the system will display message “Error occurred when set Device Status” in field [Edit Log].
               
            }
          
            var message = "Auto-Warehouse is at " + (online ? "online" : "offline") + " status...";
            var devices = _unitOfWork.DeviceRepository.GetById(deviceCode);
            if (devices != null)
            {
                devices.F14_DeviceStatus = online ? Constants.F14_DeviceStatus.Online.ToString("D") : Constants.F14_DeviceStatus.Offline.ToString("D");
                devices.F14_UpdateDate = DateTime.Now;
                _unitOfWork.DeviceRepository.Update(devices);
            }
            else
            {
                message = "Error occured when set Device Status";
            }
            _unitOfWork.Commit();
            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
            return true;
        }

        public bool RequestTimerStatus(bool set)
        {
            throw new NotImplementedException();
        }

        public bool UpdateConveyorStatus(string conveyorCode)
        {
            throw new NotImplementedException();
        }

        public void ProcessAwMessage(MessageFormAW aw, string deviceCode)
        {

            if (ConvertHelper.ToInteger(aw.Id) == 5000)
            {
                // this is status request
                // do nothing here
                return;
            }

            var tm47 = _unitOfWork.ProductWarehouseCommandRepository.GetByCommondNoAndSeqNo(aw.Sequence, aw.Id);
            if (tm47 == null)
            {
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName,
                    "Error message comes from AW!(Not in DB)");
                return;
            }
            var nStatus = tm47.F47_Status[0];
            var nRetryCount = tm47.F47_RetryCount;
            var message = "";
            var nRetCode = 0;
            var IDS_DBOPERATIONFAIL = "The database operation failed.";
            switch (ConvertHelper.ToInteger(aw.Command))
            {
                case 0:
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseProductScreenName,
                        "Error message comes from AW!(Error Command)");

                    break;
                case 10: // accepted command 
                    if (nStatus != '1' &&
                        nStatus != '4' &&
                        nStatus != '5')
                    {
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehouseProductScreenName,
                            "Error message comes from AW!(Status Error)");
                        return;
                    }
                    //m_pDlg->RemoveAcceptTimer();

                    switch (ConvertHelper.ToInteger(aw.Status))
                    {
                        case 0: // OK
                            //sprintf(szTmpStr, "%04.4d", TC_MID_CmdEnd);
                            //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                            // update status to 3(accepted)
                            nRetCode = SetMtrCmdSts(aw.Id, aw.Sequence, 3, aw.Status, false);
                            //m_pDlg -> ProcessDataBase();		// process next command
                            break;
                        case 5000: // Retry Unlimitly.
                            // Adde by Wu.Jing

                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseProductScreenName,
                                "Uketuke Error<5000> , Retry Unlimitly");
                            nRetCode = SetMtrCmdSts(aw.Id, aw.Sequence, 5, aw.Status, false);
                            // end of added
                            //m_pDlg->SetResendTimer();
                            break;
                        case 1: // Set AutoWare House to error status
                        case 2:
                        case 1001:
                        case 1002:
                        case 1003:
                        case 1004:
                            message = string.Format("Uketuke Error{0},Cancel this command",
                                ConvertHelper.ToInteger(aw.Status));

                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseProductScreenName,
                                message);
                            if (CancelProductWarehouseCommand(aw.Sequence, aw.Id))
                            {
                                // cancel operation success
                                //sprintf(szTmpStr, "%04.4d", TC_MID_CmdCancel);
                                //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                                // set the AutoWare House status = 2 (error), 

                                AwError(deviceCode);
                            }
                            else
                            {
                                // cancel operation failure.
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseProductScreenName,
                                    IDS_DBOPERATIONFAIL);


                                return;
                            }
                            break;
                        case 1005:
                            // cancel the command, and set aw status to offline
                            if (CancelProductWarehouseCommand(aw.Sequence, aw.Id))
                            {
                                // cancel operation success
                                //sprintf(szTmpStr, "%04.4d", TC_MID_CmdCancel);
                                //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                                // set the AutoWare House status = 1 (offline), 
                                AwOffLine(deviceCode);

                            }
                            else
                            {
                                // cancel operation failure.
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseProductScreenName,
                                    IDS_DBOPERATIONFAIL);
                                return;
                            }
                            break;
                        case 3: // cancel this command, process next command
                        case 5: // added by Crystal WU on 96/11/05
                        case 7:
                        case 9:
                        case 11:
                        case 12:
                        case 13:
                        case 14:
//			if( C3_CommandCancel(pMsg, nStrRtrType, 0, TRUE) ){
                            message = string.Format("Uketuke Error{0},Cancel this command",
                                aw.Status);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseProductScreenName,
                                message);

                            if (CancelProductWarehouseCommand(aw.Sequence, aw.Id))
                            {
                                //sprintf(szTmpStr, "%04.4d", TC_MID_CmdCancel);
                                //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                            }
                            else
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseProductScreenName, IDS_DBOPERATIONFAIL);
                                return;
                            }
                            //m_pDlg->SetResendTimer();
                            break;
                        case 6: // Retry three times and set the autoware house error.
                        case 15:
                            nRetCode = SetMtrCmdSts(aw.Id, aw.Sequence, 5, aw.Status, false, 1);
                            if (nRetryCount >= 3)
                            {
                                if (CancelProductWarehouseCommand(aw.Sequence, aw.Id))
                                {
                                    var messsage =
                                        string.Format(
                                            "Uketuke Error{0},already retry {1} times, so cancel this command",
                                            ConvertHelper.ToInteger(aw.Status), nRetryCount);
                                    _notificationService.SendNoteInformation(null,
                                        _notificationService.AutomatedWarehouseProductScreenName, messsage);


                                    //sprintf(szTmpStr, "%04.4d", TC_MID_CmdCancel);
                                    //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                    //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                                    AwError(deviceCode);
                                }
                                else
                                {
                                    _notificationService.SendNoteInformation(null,
                                        _notificationService.AutomatedWarehouseProductScreenName, IDS_DBOPERATIONFAIL);


                                    return;
                                }
                            }
                            else
                            {
                                message = string.Format("Uketuke Error{0},already retry {1} times",
                                    ConvertHelper.ToInteger(aw.Status),
                                    nRetryCount);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseProductScreenName, message);

                            }
                            break;
                        case 4: // Retry three times, cancel this command,process next command
//		case 5:				deleted by Crystal Wu on 96/11/05
                        case 8:
                        case 10:
                            nRetCode = SetMtrCmdSts(aw.Id, aw.Sequence, 5, aw.Status, false, 1);
                            if (nRetryCount >= 3)
                            {

                                if (CancelProductWarehouseCommand(aw.Sequence, aw.Id))
                                {
                                    message =
                                        string.Format(
                                            "Uketuke Error{0},already retry {1} times, so cancel this command",
                                            ConvertHelper.ToInteger(aw.Status), nRetryCount);
                                    _notificationService.SendNoteInformation(null,
                                        _notificationService.AutomatedWarehouseProductScreenName, message);

                                    //    sprintf(szTmpStr, "%04.4d", TC_MID_CmdCancel);
                                    //    memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                    //    m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                                    //}else{
                                    _notificationService.SendNoteInformation(null,
                                        _notificationService.AutomatedWarehouseProductScreenName, IDS_DBOPERATIONFAIL);


                                    return;
                                }
                                // m_pDlg->SetResendTimer();
                            }
                            else
                            {
                                message = string.Format("Uketuke Error{0},already retry {1} times",
                                    ConvertHelper.ToInteger(aw.Status),
                                    nRetryCount);

                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseProductScreenName,
                                    message);

                                //m_pDlg->SetAcceptTimer();
                            }
                            break;
                        default:

                            message = string.Format("unexpected errcode{0} from AW client",
                                aw.Status);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseProductScreenName, message);
                            break;
                    }
                    break;
                case 100: // command ended
                    if (nStatus != '3' &&
                        nStatus != '4' &&
                        nStatus != '5')
                    {
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehouseProductScreenName,
                            "Error message comes from AW!(Status Error)");
                        return;
                    }

                    switch (ConvertHelper.ToInteger(aw.Status))
                    {
                        case 0: // OK
                            if (EndProductWarehouseCommand(tm47.F47_CommandNo,tm47.F47_CmdSeqNo))
                            {
                                //sprintf(szTmpStr, "%04.4d", TC_MID_CmdEnd);
                                //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                            }
                            else
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseProductScreenName, IDS_DBOPERATIONFAIL);


                                return;
                            }
                            break;

                        case 60: // storage 2 times
                        case 61:
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseProductScreenName,
                                "Storage error:storage 2 times.");

                            if (CancelProductWarehouseCommand(aw.Sequence, aw.Id, 2))
                            {
                                //sprintf(szTmpStr, "%04.4d", TC_MID_ReStoraged);
                                //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                            }
                            else
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseProductScreenName, IDS_DBOPERATIONFAIL);


                                return;
                            }
                            break;

                        case 64: // empty retrieve
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseProductScreenName,
                                "Retrieve error:empty retrieve.");


                            if (CancelProductWarehouseCommand(aw.Sequence, aw.Id, 3))
                            {
                                //sprintf(szTmpStr, "%04.4d", TC_MID_EmptyRetrieve);
                                //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                                // added by Crystal WU on 96/11/06 
                                AwError(deviceCode);
                                // end of added
                            }
                            else
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseProductScreenName, IDS_DBOPERATIONFAIL);
                                return;
                            }
                            break;

                        default:
                            message = string.Format("Retrieve error ! errcode{0}",
                                aw.Status);

                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseProductScreenName, message);

                            if (CancelProductWarehouseCommand(aw.Sequence, aw.Id))
                            {
                                //sprintf(szTmpStr, "%04.4d", TC_MID_CmdCancel);
                                //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                            }
                            else
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseProductScreenName, IDS_DBOPERATIONFAIL);
                                return;
                            }
                            break;
                    }
                    break;
                case 1000: // cancel
                    // Added by Crystal Wu on 96/11/05 for Check Status when receive AW message
                    if (nStatus != '3' &&
                        nStatus != '4' &&
                        nStatus != '5')
                    {
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehouseProductScreenName,
                            "Error message comes from AW!(Status Error)");

                        return;
                    }
                    // end of added

                    switch (ConvertHelper.ToInteger(aw.Status))
                    {
                        case 64: // empty retrieve
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseProductScreenName,
                                "Retrieve error:empty retrieve.");

                            if (CancelProductWarehouseCommand(aw.Sequence, aw.Id, 3))
                            {
                                //sprintf(szTmpStr, "%04.4d", TC_MID_EmptyRetrieve);
                                //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                                // added by Crystal WU on 96/11/06 
                                AwError(deviceCode);
                                // end of added
                            }
                            else
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseProductScreenName, IDS_DBOPERATIONFAIL);


                                return;
                            }
                            break;
                        default:
                            if (CancelProductWarehouseCommand(aw.Sequence, aw.Id))
                            {
                                //sprintf(szTmpStr, "%04.4d", TC_MID_CmdCancel);
                                //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                            }
                            else
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseProductScreenName, IDS_DBOPERATIONFAIL);


                            }
                            break;
                    }
                    break;
            }
        }

        private void AwOffLine(string deviceCode)
        {
            var device = _unitOfWork.DeviceRepository.GetByDeviceCode(deviceCode);
            if (device!=null)
            {
                device.F14_DeviceStatus = "1";
                device.F14_UpdateDate = DateTime.Now;
                _unitOfWork.DeviceRepository.Update(device);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, "Autoware House is at error status..");
           
            }
            else
            {
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, "Set Device Status failed");
           
            }
           _unitOfWork.Commit();
            
        }

        public ResponseResult DeleteProductWarehouseCommand(string commandNo, string cmdSeqNo)
        {
            var preProduct =
                _unitOfWork.ProductWarehouseCommandRepository.GetMany(
                    i =>
                        i.F47_CommandNo.Trim().Equals(commandNo.Trim()) && i.F47_CmdSeqNo.Trim().Equals(cmdSeqNo.Trim()))
                    .FirstOrDefault();
            if (preProduct != null)
            {
                //	If [F47_Status] of the selected preProduct Warehouse Command record is 6, 7, 8, 9, A, B, C, D, E or F, the system will:
                if ((ConvertHelper.ToInteger(preProduct.F47_Status) >=
                     ConvertHelper.ToInteger(Constants.TC_CMDSTS.TC_CMDSTS_6)
                     &&
                     ConvertHelper.ToInteger(preProduct.F47_Status) <=
                     ConvertHelper.ToInteger(Constants.TC_CMDSTS.TC_CMDSTS_9))
                    || preProduct.F47_Status == Constants.TC_CMDSTS.TC_CMDSTS_A
                    || preProduct.F47_Status == Constants.TC_CMDSTS.TC_CMDSTS_B
                    || preProduct.F47_Status == Constants.TC_CMDSTS.TC_CMDSTS_C
                    || preProduct.F47_Status == Constants.TC_CMDSTS.TC_CMDSTS_D
                    || preProduct.F47_Status == Constants.TC_CMDSTS.TC_CMDSTS_E
                    || preProduct.F47_Status == Constants.TC_CMDSTS.TC_CMDSTS_F)
                {
                    //	Create a preProduct Warehouse History by duplicating the selected preProduct Warehouse Command.
                    _unitOfWork.ProductWarehouseCommandHistoryRepository.InsertWithTx47(preProduct);
                    _unitOfWork.ProductWarehouseCommandRepository.Delete(preProduct);
                    _unitOfWork.Commit();
                    return new ResponseResult(true);
                }
                else
                {
                    //If [F47_Status] of the selected preProduct Warehouse Command record is not 6, 7, 8, 9, A, B, C, D, E and F, 
                    //the system will display Error message using Error message template MSG 4.
                    return new ResponseResult(false, "MSG4");
                }

            }
            //	If there is any occurred Error, the system will display message “” in field [Edit Log].
            //	Trigger use case UC 32: View Product Warehouse Commands.
            return new ResponseResult(false, "Error occurred when make history");
        }

        public bool DeleteProductWarehouseHistories()
        {
            try
            {

                var lstTh66 = _unitOfWork.ProductWarehouseCommandHistoryRepository.GetAll();

                    foreach (var th66 in lstTh66)
                    {
                        _unitOfWork.ProductWarehouseCommandHistoryRepository.Delete(th66);
                    }
                  
                
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EndProductWarehouseCommand(string commandNo, string cmdSeqNo)
        {

            var nCmdId = ConvertHelper.ToInteger(commandNo);
            string lpConveyor;
            var lpLocation = "";
            int nShelfSts;
            var nStkFlg = -1;
            var nOsdPpdStkFlg = -1;
            var nTanaorosiFlg = -1;
            var nShelfTime = -1;
            var nStgRtrCls = -1; // for Set StgRtrHst
            int nRetCode;
            var pMsg = _unitOfWork.ProductWarehouseCommandRepository.GetByCommondNoAndSeqNo(commandNo, cmdSeqNo);
            var nStrRtrType = ConvertHelper.ToInteger(pMsg.F47_StrRtrType);
            if ((ConvertHelper.ToInteger(pMsg.F47_Status) >
                  ConvertHelper.ToInteger(Constants.TC_CMDSTS.TC_CMDSTS_5)))
            {
                return false;
            }
            string message;

            switch (nCmdId)
            {
                case 1000: // storage    command
                case 1001: // re-storage command 
                    lpConveyor = pMsg.F47_From;
                    lpLocation = pMsg.F47_To;
                    nTanaorosiFlg = 0;
                    nShelfTime = 0; // set storage time		
                    switch (nStrRtrType)
                    {
                        case 0: // Product

                            message = string.Format("SUCCESS: Product Storage. From: {0} To: {1} TermNo: {2}", pMsg.F47_From,
                                pMsg.F47_To,
                                pMsg.F47_TerminalNo);

                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            nShelfSts = 2;
                            nStkFlg = 3;
                            //		nShelfTime = 0;// set storage time
                            nStgRtrCls = 0; // storage
                            break;
                        case 1: // Warehouse pallet

                            message = string.Format("SUCCESS: Warehouse Pallet Storage. From: {0} To: {1} TermNo: {2}",
                                pMsg.F47_From,
                                pMsg.F47_To,
                                pMsg.F47_TerminalNo);

                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            nShelfSts = 1;
                            nStkFlg = -1;
                            break;
                        case 2: // Outside Preproduct

                            message = string.Format("SUCCESS: Outside Preproduct Storage. From: {0} To: {1} TermNo: {2}",
                                pMsg.F47_From,
                                pMsg.F47_To, pMsg.F47_TerminalNo);

                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            nShelfSts = 8;
                            nOsdPpdStkFlg = 3;
                            break;
                        case 3: // Out of sign Preproduct

                            message = string.Format(
                                "SUCCESS: Out of sign Preproduct Storage. From: {0} To: {1} TermNo: {2}",
                                pMsg.F47_From, pMsg.F47_To, pMsg.F47_TerminalNo);

                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            nShelfSts = 3;
                            nStkFlg = -1;
                            break;
                        case 4: // re-storage

                            message = string.Format("SUCCESS: Product Restorage. From: {0} To: {1} TermNo: {2}",
                                pMsg.F47_From,
                                pMsg.F47_To, pMsg.F47_TerminalNo);

                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            nShelfSts = 2;
                            nStkFlg = 3;
                            //set re-storage time and recover storage time
                            nShelfTime = 1;
                            nStgRtrCls = 0;
                            break;
                        default:

                            message = string.Format("Unknown type {0} when storage success", nStrRtrType);
                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                            return false;
                    }
                    break;

                case 2000: // retrieve command
                case 2001: // re-retrieve command (impossible)
                    lpConveyor = pMsg.F47_To;
                    lpLocation = pMsg.F47_From;
                    nShelfTime = 2; // set retrieval time	

                    switch (nStrRtrType)
                    {
                        case 0: // Product
                            message = string.Format("SUCCESS: Product Retrieve.  From: {0} To: {1} TermNo: {2}", pMsg.F47_From,
                                pMsg.F47_To, pMsg.F47_TerminalNo);
                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            nShelfSts = 0;
                            nStkFlg = 0;
                            //		nShelfTime = 2 ; // set retrieval time
                            nStgRtrCls = 1; // retrieval
                            break;
                        case 1: // warehouse's pallet

                            message = string.Format("SUCCESS: Pallet Retrieve. From: {0} To: {1} TermNo: {2}", pMsg.F47_From,
                                pMsg.F47_To, pMsg.F47_TerminalNo);

                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            nShelfSts = 0;
                            nStkFlg = -1;
                            break;
                        case 2: // Outside Preproduct
                            message = string.Format("SUCCESS: Outside Preproduct Retrieve. From: {0} To: {1} TermNo: {2}",
                                pMsg.F47_From,
                                pMsg.F47_To, pMsg.F47_TerminalNo);

                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            nShelfSts = 0;
                            nOsdPpdStkFlg = 0;
                            break;
                        case 3: // Out of sign Preproduct
                            message = string.Format("SUCCESS: Out of sign Preproduct Retrieve. From: {0} To: {1} TermNo: {2}",
                                pMsg.F47_From, pMsg.F47_To, pMsg.F47_TerminalNo);

                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            nShelfSts = 0;
                            nStkFlg = -1;
                            break;
                        default:
                            message = string.Format("Unknown  type {0} when retrieve succeed", nStrRtrType);

                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            return false;
                    }
                    break;

                case 3000: // Moving between conveyers
                    nShelfSts = -1;
                    nStkFlg = -1;
                    lpConveyor = pMsg.F47_From;
                    if (SetConveyorSts(lpConveyor, 0)<=0)
                    {
                        message = " Can’t set Conveyor Status";
                        _log.Info(message);
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehouseProductScreenName, message); 
                    }
                    
                    lpConveyor = pMsg.F47_To;

                    message = string.Format("SUCCESS: Move From: {0} To: {1} TermNo: {2}", pMsg.F47_From, pMsg.F47_To,
                        pMsg.F47_TerminalNo);

                    _log.Info(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                    break;

                //case 4000:
                //case 5000:
                //    // Don't need to do here
                //    return false;
                case 6000:

                    message = string.Format("SUCCESS: Tanaonosi Storage. From: {0} To: {1} TermNo: {2}", pMsg.F47_From, pMsg.F47_To,
                        pMsg.F47_TerminalNo);

                    _log.Info(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                    lpConveyor = pMsg.F47_From;
                    lpLocation = pMsg.F47_To;
                    nShelfSts = 2;
                    nStkFlg = 3;
                    nTanaorosiFlg = 1;
                    break;
                case 7000:

                    message = string.Format("SUCCESS: Tanaonosi Retrieve. From: {0} To: {1} TermNo: {2}", pMsg.F47_From, pMsg.F47_To,
                        pMsg.F47_TerminalNo);

                    _log.Info(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                    lpConveyor = pMsg.F47_To;
                    lpLocation = pMsg.F47_From;
                    nShelfSts = -1;
                    nStkFlg = -1;
                    break;
                default:
                    // not supported in this version
                    message = string.Format("Unknown ID {0} from Automatic Warehouse", nCmdId);
                    _log.Info(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                    return false;
            }

            nRetCode = SetMtrCmdSts(pMsg.F47_CommandNo, pMsg.F47_CmdSeqNo, 6, "0000", true);
            if (nRetCode <= 0)
            {
                message = string.Format("Set Status of CMD table failed. SEQ {0}", pMsg.F47_CmdSeqNo);
                _log.Error(message);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
            }

            // restore conveyor number and status
            if (nCmdId != 1001)
            {
                nRetCode = SetConveyorSts(lpConveyor, 0);
                if (nRetCode <= 0)
                {
                    message = " Can’t set Conveyor Status";
                    _log.Info(message);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseProductScreenName, message); 
                    message = string.Format("Recover Conveyor Status failed. Conveyor: {0}", lpConveyor);
                    _log.Error(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                }
            }
            // update shelf status of material
            if (nShelfSts >= 0)
            {
                nRetCode = SetShelfSts(lpLocation, nShelfSts, nTanaorosiFlg);
                if (nRetCode <= 0)
                {
                    message = string.Format("Set Shelf Status failed. LOCATION: {0}", lpLocation);
                    _log.Error(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                }
            }

            // update material stock flag
            if (nStkFlg >= 0)
            {
                nRetCode = SetStockFlag(pMsg.F47_PalletNo, nStkFlg);
                if (nRetCode <= 0)
                {
                    message = string.Format("Set Stock Flag failed. PALLETNO: {0}", !string.IsNullOrEmpty(pMsg.F47_PalletNo)?pMsg.F47_PalletNo:"null" );
                    _log.Error(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                }
            }

            if (nOsdPpdStkFlg >= 0)
            {
                nRetCode = SetOsdPpdStockFlag(pMsg.F47_PalletNo, nOsdPpdStkFlg);
                if (nRetCode <= 0)
                {
                    message = string.Format("Set Outside Preproduct Stock Flag failed. PALLETNO: {0}", !string.IsNullOrEmpty( pMsg.F47_PalletNo)?pMsg.F47_PalletNo:"null");
                    _log.Error(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                }
            }

            if (nShelfTime >= 0)
                SetShelfTime(pMsg.F47_PalletNo, lpLocation, nShelfTime);


            if (nStgRtrCls >= 0)
            {

                nRetCode = SetStgRtrHst(pMsg.F47_PalletNo,
                    pMsg.F47_TerminalNo,
                    pMsg.F47_From,
                    pMsg.F47_To,
                    nStgRtrCls,
                    nShelfTime
                    );
                if (nRetCode <= 0)
                {
                    message = "Set StrRtrHistory  failed";
                    _log.Error(message);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseProductScreenName, message);

                }
            }
            _unitOfWork.Commit();
            return true;

        }
        
        public bool CancelProductWarehouseCommand(string commandNo, string cmdSeqNo, int nHowCancel = 0, bool nConveyorErr = false)
        {
            var nCmdId = ConvertHelper.ToInteger(commandNo);
            string lpConveyor, lpLocation = "";
            int nShelfSts, nStkFlg = -1;
            var nOsdPpdStkFlg = -1;
            var nRetCode = 0;
            var pMsg = _unitOfWork.ProductWarehouseCommandRepository.GetByCommondNoAndSeqNo(commandNo, cmdSeqNo);
            var nStrRtrType = ConvertHelper.ToInteger(pMsg.F47_StrRtrType);

            string message;

            switch (nCmdId)
            {
                case 1000: // storage		command
                case 1001: // restorage	command
                    lpConveyor = pMsg.F47_From;
                    lpLocation = pMsg.F47_To;
                    switch (nStrRtrType)
                    {
                        case 0: // Product
                        case 4: // Re-storage

                            nStkFlg = 0;

                            message = string.Format("CANCEL: Product Storage. From: {0} To: {1}", pMsg.F47_From, pMsg.F47_To);
                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            if (nHowCancel == 2)
                            {
                                // 2 times storage
                                nShelfSts = 6;
                            }
                            else
                            {
                                nShelfSts = 0;
                            }
                            break;
                        case 1: // warehouse Pallet

                            message = string.Format("CANCEL: Pallet Storage. From: {0} To: {1}", pMsg.F47_From, pMsg.F47_To);
                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            nStkFlg = -1;
                            if (nHowCancel == 2)
                            {
                                nShelfSts = 6;
                            }
                            else
                            {
                                nShelfSts = 0;
                            }
                            break;
                        case 2: // Outside Preproduct

                            message = string.Format("CANCEL: Outside Preproduct Storage. From: {0} To: {1}", pMsg.F47_From,
                                pMsg.F47_To);
                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            if (nHowCancel == 2)
                            {
                                nShelfSts = 6;
                            }
                            else
                            {
                                nShelfSts = 0;
                            }
                            nOsdPpdStkFlg = 0;
                            break;
                        case 3: // Out of sign Preproduct

                            message = string.Format("CANCEL: Out of sign Preproduct Storage. From: {0} To: {1}",
                                pMsg.F47_From,
                                pMsg.F47_To);

                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            if (nHowCancel == 2)
                                nShelfSts = 6;
                            else
                                nShelfSts = 0;

                            nStkFlg = -1;
                            break;
                        default:

                            message = string.Format("Unknown type{0} when CANCEL", nStrRtrType);
                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            return false;
                    }
                    break;
                case 2000:
                case 2001:
                    lpConveyor = pMsg.F47_To;
                    lpLocation = pMsg.F47_From;
                    switch (nStrRtrType)
                    {
                        case 0: // Product

                            message = string.Format("CANCEL: Product Retrieve. From: {0} To: {1}", pMsg.F47_From,
                                pMsg.F47_To);

                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            nShelfSts = 2;
                            nStkFlg = 3;
                            break;
                        case 1: // warehouse Pallet

                            message = string.Format("CANCEL: Pallet Retrieve. From: {0} To: {1}", pMsg.F47_From, pMsg.F47_To);
                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            nShelfSts = 1;
                            nStkFlg = -1;
                            break;
                        case 2: // Outside-making Preproduct

                            message = string.Format("CANCEL: Outside Making Preproduct Retrieve. From: {0} To: {1}", pMsg.F47_From,
                                pMsg.F47_To);

                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            nShelfSts = 8;
                            nOsdPpdStkFlg = 3;
                            break;
                        case 3: // Out of sign Preproduct

                            message = string.Format("CANCEL: Outside Making Preproduct Retrieve. From: {0} To: {1}", pMsg.F47_From,
                                pMsg.F47_To);

                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                            nShelfSts = 3;
                            nStkFlg = -1;
                            break;
                        default:
                            message = string.Format("Unknown  type {0} when CANCEL", nStrRtrType);
                            _log.Info(message);
                            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                            return false;
                    }
                    break;
                case 3000: // Moving between conveyers
                    nShelfSts = -1;
                    nStkFlg = -1;
                    lpConveyor = pMsg.F47_From;

                    message = string.Format("CANCEL: Move. From: {0} To: {1}", pMsg.F47_From, pMsg.F47_To);
                    _log.Info(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                    if (nConveyorErr)
                    {
                        // Set the conveyor status to Error
                        nRetCode = SetConveyorSts(lpConveyor, 9);
                    }
                    else
                    {
                        // restore conveyor number and status
                        nRetCode = SetConveyorSts(lpConveyor, 0);
                    }
                    if (nRetCode<=0)
                    {
                        message = " Can’t set Conveyor Status";
                        _log.Info(message);
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehouseProductScreenName, message); 
                    }
                    lpConveyor = pMsg.F47_To;
                    break;

                case 4000:
                case 5000:
                    // don't know how to do here
                    return false;
                case 6000: // stock taking command	(storage )
                    lpConveyor = pMsg.F47_From;
                    lpLocation = pMsg.F47_To;

                    message = string.Format("CANCEL: Tanaonosi Storage. From: {0} To: {1}", pMsg.F47_From, pMsg.F47_To);
                    _log.Info(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                    if (nHowCancel == 2)
                    {
                        // 2 times storage
                        nShelfSts = 6;
                        nStkFlg = 0;
                    }
                    else
                    {
                        nShelfSts = 0;
                        nStkFlg = 0;
                    }
                    break;
                case 7000: // stock taking command (retrieve)
                    lpConveyor = pMsg.F47_To;
                    lpLocation = pMsg.F47_From;
                    nShelfSts = 2;
                    nStkFlg = 3;

                    message = string.Format("CANCEL: Tanaonosi Retrieve. From: {0} To: {1}", pMsg.F47_From, pMsg.F47_To);
                    _log.Info(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                    break;
                default:
                    // not supported in this version
                    message = string.Format("Unknown Cmdid {0} from Automatic Warehouse", nCmdId);
                    _log.Info(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                    return false;
            }

            switch (nHowCancel)
            {
                case 0: // normal
                    // update status to 7
                    nRetCode = SetMtrCmdSts(pMsg.F47_CommandNo, pMsg.F47_CmdSeqNo, 7, "0000", true);
                    break;
                case 1: // manual
                    // update status to 7
                    nRetCode = SetMtrCmdSts(pMsg.F47_CommandNo, pMsg.F47_CmdSeqNo, 7, "0000", true);
                    // nRetCode = C3_AMakeHistory(pMsg.F47_CommandNo, pMsg.F47_CmdSeqNo, &nErrCode);
                    break;
                case 2: // storage 2 times
                    nRetCode = SetMtrCmdSts(pMsg.F47_CommandNo, pMsg.F47_CmdSeqNo, 8, "0000", true);
                    break;
                case 3: // empty retrieve
                    nRetCode = SetMtrCmdSts(pMsg.F47_CommandNo, pMsg.F47_CmdSeqNo, 9, "0000", true);
                    // modified by Crystal WU on 96/11/01 for set ShelfStatus to prohibit when empty retrieve
                    //nShelfSts = 0;
                    nShelfSts = 6;
                    // end of modified
                    if ((nCmdId == 2000 || nCmdId == 2001 || nCmdId == 7000) && nStrRtrType == 0)
                    {
                        nStkFlg = 0;
                        nOsdPpdStkFlg = -1;
                    }
                    else if ((nCmdId == 2000 || nCmdId == 2001 || nCmdId == 7000) && nStrRtrType == 2)
                    {
                        nStkFlg = -1;
                        nOsdPpdStkFlg = 0;
                    }
                    else
                    {
                        nOsdPpdStkFlg = -1;
                        nStkFlg = -1;
                    }
                    break;
            }
            if (nRetCode <= 0)
            {
                message = string.Format("Set Status of CMD table failed. SEQ{0}", pMsg.F47_CmdSeqNo);
                _log.Error(message);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);


            }
            if (nConveyorErr)
            {
                // Set the conveyor status to Error
                nRetCode = SetConveyorSts(lpConveyor, 9);
            }
            else
            {
                // restore conveyor number and status
                if (nCmdId != 1001)
                {
                    nRetCode = SetConveyorSts(lpConveyor, 0);
                }
            }
            if (nRetCode <= 0)
            {
                message = " Can’t set Conveyor Status";
                _log.Info(message);
                _notificationService.SendNoteInformation(null,
                    _notificationService.AutomatedWarehouseProductScreenName, message); 
                message = string.Format("Recover Conveyor Status  failed. Conveyor: {0}", lpConveyor);
                _log.Error(message);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

            }
            // update shelf status of material
            if (nShelfSts >= 0)
            {

                nRetCode = SetShelfSts(lpLocation, nShelfSts, -1, nHowCancel);
                if (nRetCode <= 0)
                {
                    message = string.Format("Set Shelf Status failed. LOCATION: {0}", lpLocation);
                    _log.Info(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                }
            }

            // update material stock flag
            if (nStkFlg >= 0)
            {
                nRetCode = SetStockFlag(pMsg.F47_PalletNo, nStkFlg);
                if (nRetCode <= 0)
                {
                    message = string.Format("Set Stock Flag failed. PALLETNO: {0}", pMsg.F47_PalletNo);
                    _log.Info(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                }
            }

            if (nOsdPpdStkFlg >= 0)
            {
                nRetCode = SetOsdPpdStockFlag(pMsg.F47_PalletNo, nOsdPpdStkFlg);
                if (nRetCode <= 0)
                {
                    message = string.Format("Set OutsidePreproductStock Flag failed. PALLETNO: {0}",  string.IsNullOrEmpty( pMsg.F47_PalletNo)?pMsg.F47_PalletNo:"null");
                    _log.Info(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                }
            }
            
            return true;
        }


        public bool ProcessData(bool isAutomatedWarehouse,string deviceCode)
        {
            // Find device in repositories.
            var devices = _unitOfWork.DeviceRepository.GetAll();

            devices = devices.Where(x => deviceCode.Equals(x.F14_DeviceCode.Trim()));
            var device = devices.FirstOrDefault();

            if (device == null)
                return false;

            if (!Constants.F14_DeviceStatus.Online.ToString("D").Equals(device.F14_DeviceStatus))
            {
                var message = "Autoware House  is at offline or Error status…";
                _log.Error(message);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName,
                    message);

                return false;
            }

            var lstPdtWhsCmd = _unitOfWork.ProductWarehouseCommandRepository.GetAll();
            if (!isAutomatedWarehouse)
            {
                lstPdtWhsCmd = lstPdtWhsCmd.Where(i => i.F47_Status.Trim().Equals(Constants.F34_Status.status6) ||
                                                        i.F47_Status.Trim().Equals(Constants.F34_Status.status7) ||
                                                        i.F47_Status.Trim().Equals(Constants.F34_Status.status8) ||
                                                        i.F47_Status.Trim().Equals(Constants.F34_Status.status9))
                    .OrderByDescending(i => i.F47_Priority)
                    .ThenByDescending(i => i.F47_Status);
            }
            else
            {
                lstPdtWhsCmd = lstPdtWhsCmd.Where(i => i.F47_Status.Trim().Equals(Constants.F34_Status.status0) ||
                                                        i.F47_Status.Trim().Equals(Constants.F34_Status.status1) ||
                                                        i.F47_Status.Trim().Equals(Constants.F34_Status.status2) ||
                                                        i.F47_Status.Trim().Equals(Constants.F34_Status.status4) ||
                                                        i.F47_Status.Trim().Equals(Constants.F34_Status.status5))
                    .OrderByDescending(i => i.F47_Priority)
                    .ThenByDescending(i => i.F47_Status);
            }
            ProcessDataList(lstPdtWhsCmd, deviceCode);

            if (!isAutomatedWarehouse)
                MackHistory();

            return true;
        }
        public string GetDeviceStatus(string deviceCode)
        {
            var device = _unitOfWork.DeviceRepository.GetById(deviceCode);
            return device != null ? device.F14_DeviceStatus : "";
        }

        #region private

        private void MackHistory()
        {
            try
            {
                var lstPdtWhsCmd = _unitOfWork.ProductWarehouseCommandRepository.GetMany(i => i.F47_Status.Trim()
                                                       .Equals(Constants.F34_Status.statusA) ||
                                                       i.F47_Status.Trim().Equals(Constants.F34_Status.statusB) ||
                                                       i.F47_Status.Trim().Equals(Constants.F34_Status.statusC) ||
                                                       i.F47_Status.Trim().Equals(Constants.F34_Status.statusD) ||
                                                        i.F47_Status.Trim().Equals(Constants.F34_Status.statusE) ||
                                                             i.F47_Status.Trim().Equals(Constants.F34_Status.statusF))
                    .OrderByDescending(i => i.F47_Priority)
                    .ThenByDescending(i => i.F47_Status);
                foreach (var tx47PdtWhsCmd in lstPdtWhsCmd)
                {
                    _unitOfWork.ProductWarehouseCommandHistoryRepository.InsertWithTx47(tx47PdtWhsCmd);
                    _unitOfWork.ProductWarehouseCommandRepository.Delete(tx47PdtWhsCmd);
                }
                if (!lstPdtWhsCmd.Any())
                {
                    //var message = "Error occured when Make history";
                    //_notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseMaterialScreenName, message);
                    //_log.Error(message);
                }
                _unitOfWork.Commit();
            }
            catch (Exception exception)
            {
                var message = exception.Message;
                _log.Info(message);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

            }
        }

        private void ProcessDataList(IEnumerable<TX47_PdtWhsCmd> lstPdtWhsCmd, string deviceCode)
        {
            var nRetCode = 0;
            string lpConveyor = null;

            //SetResendTimer();
            foreach (var tx47PdtWhsCmd in lstPdtWhsCmd.ToList())
            {
               var pSeq = tx47PdtWhsCmd.F47_CmdSeqNo;
                var pCmd = ConvertHelper.ToInteger(tx47PdtWhsCmd.F47_CommandNo);


                var nStrRtrType = ConvertHelper.ToInteger(tx47PdtWhsCmd.F47_StrRtrType);
                var nFlag = -1;
                var nOutSideFlag = -1;
                
                string message;

                // Find warehouse item status.
                if (string.IsNullOrEmpty(tx47PdtWhsCmd.F47_Status) || tx47PdtWhsCmd.F47_Status.Length != 1)
                    continue;


                switch (tx47PdtWhsCmd.F47_Status[0])
                {
                    case '0':
                        int nStrRtrSts;
                        if (pCmd == 1000 || pCmd == 1001)
                        {
                            switch (nStrRtrType)
                            {
                                case 0:
                                    nFlag = 1;
                                    break;
                                case 1:
                                    break;
                                case 2:
                                    nOutSideFlag = 1;
                                    break;
                                case 3:
                                    break;
                            }
                            nStrRtrSts = 1;
                            lpConveyor = tx47PdtWhsCmd.F47_From;
                            // added by Crystal Wu for 2 times storage need not to check conveyor status
                            if (pCmd == 1001)
                            {
                                nStrRtrSts = -1;
                            }
                            // end of added

                        }
                        else if (pCmd == 2000 || pCmd == 2001)
                        {
                            nStrRtrSts = 2;
                            lpConveyor = tx47PdtWhsCmd.F47_To;
                            switch (nStrRtrType)
                            {
                                case 0:
                                    nFlag = 2;
                                    break;
                                case 1:
                                    break;
                                case 2:
                                    nOutSideFlag = 2;
                                    break;
                                case 3:
                                    break;
                            }
                        }
                        else if (pCmd == 3000)
                        {
                            // move between floors
                            nStrRtrSts = 3;
                            nFlag = -1; // not need to update shelf status
                            lpConveyor = tx47PdtWhsCmd.F47_From;
                            nRetCode = SetConveyorSts(lpConveyor, nStrRtrSts);
                            if (nRetCode <= 0)
                            {
                                message = " Can’t set Conveyor Status";
                                _log.Info(message);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseProductScreenName, message); 
                                message = string.Format("Conveyor{0} status can not permit to send message", lpConveyor);
                                _log.Info(message);
                                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                                break;
                            }
                            lpConveyor = tx47PdtWhsCmd.F47_To;
                        }
                        else if (pCmd == 6000)
                        {
                            nStrRtrSts = 1;
                            nFlag = -1;
                            lpConveyor = tx47PdtWhsCmd.F47_From;
                        }
                        else if (pCmd == 7000)
                        {
                            nStrRtrSts = 2;
                            nFlag = -1;
                            lpConveyor = tx47PdtWhsCmd.F47_To;
                        }
                        else
                        {
                            // not supported there
                            nStrRtrSts = -1; // 3 should be change to other value
                            nFlag = -1;
                        }
                        if (nStrRtrSts >= 0)
                        {
                            nRetCode = SetConveyorSts(lpConveyor, nStrRtrSts);
                            if (nRetCode <= 0)
                            {
                                message = " Can’t set Conveyor Status";
                                _log.Info(message);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseProductScreenName, message); 
                                message = string.Format("Conveyor{0}status can not permit to send message", lpConveyor);
                                _log.Info(message);
                                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                                if (pCmd == 3000)
                                {
                                    lpConveyor = tx47PdtWhsCmd.F47_From;
                                    nRetCode = SetConveyorSts(lpConveyor, 0);
                                    if (nRetCode <= 0)
                                    {
                                        message = "Recover conveyor status failed";
                                        _log.Info(message);
                                        _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                                    }
                                }
                                break;
                            }
                        }
                        nRetCode = _notificationService.SendFromC3ToAw(tx47PdtWhsCmd);
                        
                        if (nRetCode >= 0)
                        {
                            // send the message OK
                            SetMtrCmdSts(pCmd.ToString(), pSeq, 1, "0000", false, 1);
                           
                        }
                        else
                        {
                            // send the message NG
                            SetMtrCmdSts(pCmd.ToString(), pSeq, 2, "0000", false, 1);

                            if (nStrRtrSts >= 0)
                            {
                                nRetCode = SetConveyorSts(lpConveyor, 0);
                                if (nRetCode <= 0)
                                {
                                    message = " Can’t set Conveyor Status";
                                    _log.Info(message);
                                    _notificationService.SendNoteInformation(null,
                                        _notificationService.AutomatedWarehouseProductScreenName, message); 
                                    message = "Recover conevyor status failed";
                                    _log.Info(message);
                                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                                }
                            }
                            if (pCmd == 3000)
                            {
                                lpConveyor = tx47PdtWhsCmd.F47_From;
                                nRetCode = SetConveyorSts(lpConveyor, 0);
                                if (nRetCode <= 0)
                                {
                                    message = " Can’t set Conveyor Status";
                                    _log.Info(message);
                                    _notificationService.SendNoteInformation(null,
                                        _notificationService.AutomatedWarehouseProductScreenName, message); 
                                    message = "Recover conveyor status failed";
                                    _log.Info(message);
                                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                                }
                            }
                            //SetResendTimer();
                        }

                        // update material shelf status
                        if (nFlag >= 0)
                        {
                            SetStockFlag(tx47PdtWhsCmd.F47_PalletNo, nFlag);
                        }
                        if (nOutSideFlag >= 0)
                        {
                            SetOsdPpdStockFlag(tx47PdtWhsCmd.F47_PalletNo, nFlag);
                        }
                        return;
                    case '1': // send OK but no accept message
                    case '2': // send NG
                        if (tx47PdtWhsCmd.F47_RetryCount >= 3)
                        {
                            // 3 times
                            if (tx47PdtWhsCmd.F47_Status == "1")
                            {
                                // 3 times,receive no accept
                                //C3_SetDeviceSts(sAWDevice.GetBuffer(0), m_nAWstatus, &m_nErrCode );
                                AwError(deviceCode);
                                //m_pDlg -> ShowDeviceStatus();
                            }
                          
                            SetMtrCmdSts(pCmd.ToString(), pSeq, 1, "0000", false, -1);
                        }
                        else
                        {
                            nRetCode = _notificationService.SendFromC3ToAw(tx47PdtWhsCmd);
                            if (nRetCode >= 0)
                            {
                                // AW message send success
                                SetMtrCmdSts(pCmd.ToString(), pSeq, 1, "0000", false, 1);
                                //SetAcceptTimer();
                            }
                            else
                            {
                                // AW message send failure.
                                SetMtrCmdSts(pCmd.ToString(), pSeq, 2, "0000", false, 1);

                                // SetResendTimer();
                            }
                        }
                        return;
                    case '3':
                        // do nothing
                        break;
                    case '4':
                        nRetCode = _notificationService.SendFromC3ToAw(tx47PdtWhsCmd);
                        if (nRetCode >= 0)
                        {
                            SetMtrCmdSts(pCmd.ToString(), pSeq, 4, "0000", false, 1);

                            //SetAcceptTimer();
                            return;
                        }
                        else
                        {
                            nRetCode = SetMtrCmdSts(pCmd.ToString(), pSeq, 2, "0000", false, 1);

                            // SetResendTimer();
                        }
                        break;
                    case '5':
                        nRetCode = _notificationService.SendFromC3ToAw(tx47PdtWhsCmd);
                        if (nRetCode >= 0)
                        {
                            SetMtrCmdSts(pCmd.ToString(), pSeq, 5, "0000", false, 1);

                            // SetAcceptTimer();
                            return;
                        }
                        else
                        {
                            nRetCode = SetMtrCmdSts(pCmd.ToString(), pSeq, 2, "0000", false, 1);

                            //SetResendTimer();
                        }
                        break;
                    case '6':
                    case '7':
                    case '8':
                    case '9':

                        // send the message to A side
                        var szMid = "";
                        var sMsgToA = new MessageItem();

                        // prepare the message to A side


                        switch (tx47PdtWhsCmd.F47_Status[0])
                        {
                            case '6':
                                szMid = Constants.MessageId.TC_MID_CmdEnd.ToString("D");

                                break;
                            case '7':
                                szMid = Constants.MessageId.TC_MID_CmdCancel.ToString("D");
                                break;
                            case '8':
                                szMid = Constants.MessageId.TC_MID_ReStoraged.ToString("D");
                                break;
                            case '9':
                                szMid = Constants.MessageId.TC_MID_EmptyRetrieve.ToString("D");

                                break;
                        }
                        sMsgToA.Id = szMid;
                        sMsgToA.TerminalNo = tx47PdtWhsCmd.F47_TerminalNo;
                        sMsgToA.PictureNo = tx47PdtWhsCmd.F47_PictureNo;
                        sMsgToA.Size = 20;
                        sMsgToA.WarehouseItem = tx47PdtWhsCmd;

                        // send the message to A side to inform cancel operation.
                        _notificationService.SendMessageFromC3(new List<string>() { sMsgToA.TerminalNo }, sMsgToA);
                        break;
                    case 'A': // impossible.

                        break;
                    case 'B': // impossible.

                        // delete unused data from Stock table
                        if (pCmd == 1000 && (nStrRtrType == 0 || nStrRtrType == 2))
                        {
                            switch (nStrRtrType)
                            {
                                case 0:
                                    nRetCode = DeleteStock(tx47PdtWhsCmd.F47_PalletNo);
                                    break;
                                case 2:
                                    nRetCode = DeleteOsdPpdStock(tx47PdtWhsCmd.F47_PalletNo);
                                    break;
                            }
                            if (nRetCode <= 0)
                            {
                                message = "Delete stock table failed";
                                _log.Info(message);
                                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                            }

                        }
                        //added by Crystal Wu on 96/11/06 for 
                        // can not find a empty location for re-assign when 2 times storage
                        AwError(deviceCode);
                        return;
                    // end of added
                    case 'C':
                        break;
                    case 'D':

                        // delete unused data from Stock table
                        //nRetCode = C3_CommandCancel(&(tx47PdtWhsCmd -> cmd), tx47PdtWhsCmd -> nStrRtrType, 1);
                        if (pCmd == 1000 && (nStrRtrType == 0 || nStrRtrType == 2))
                        {
                            switch (nStrRtrType)
                            {
                                case 0:
                                    nRetCode = DeleteStock(tx47PdtWhsCmd.F47_PalletNo);
                                    break;
                                case 2:
                                    nRetCode = DeleteOsdPpdStock(tx47PdtWhsCmd.F47_PalletNo);
                                    break;
                            }
                            if (nRetCode <= 0)
                            {
                                message = "Delete stock table failed";
                                _log.Info(message);
                                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                            }
                        }
                        break;

                }

            }
        }

        private int DeleteOsdPpdStock(string lpPallet)
        {
            try
            {
                _unitOfWork.OutSidePrePdtStkRepository.Delete(i => i.F53_PalletNo.Trim().Equals(lpPallet.Trim()));
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                var message = "Error occured when Delete OsdPpd stock table";
                _log.Info(message);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                return -1;

            }

            return 0;
        }

        private int DeleteStock(string lpPallet)
        {
            try
            {
                _unitOfWork.ProductShelfStockRepository.Delete(i => i.F40_PalletNo.Trim().Equals(lpPallet.Trim()));
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                var message = "Error occured when Delete Stock table";
                _log.Info(message);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                return -1;
            }


            return 0;
        }

        private int SetMtrCmdSts(string lpCmdNo, string lpSeq, int nStatus, string pAbnormalCode, bool endData, int nRetry = 0)
        {

            var tx47PdtWhsCmd = _unitOfWork.ProductWarehouseCommandRepository.GetByCommondNoAndSeqNo(lpCmdNo, lpSeq);
            if (tx47PdtWhsCmd == null)
            {
                var message = "Select cmd table failed when set status";
                _log.Info(message);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                return -1;
            }
            var nFldRetryCount = tx47PdtWhsCmd.F47_RetryCount ?? 0;
            var nFldStatus = tx47PdtWhsCmd.F47_Status;

            if (nFldStatus == nStatus.ToString())
            {
                // same status
                nFldRetryCount += nRetry;
            }
            else
            {
                nFldRetryCount = nRetry;
            }
            try
            {
                if (nRetry < 0) nFldRetryCount = 0;
                tx47PdtWhsCmd.F47_Status = nStatus.ToString();
                tx47PdtWhsCmd.F47_AbnormalCode = pAbnormalCode;
                tx47PdtWhsCmd.F47_RetryCount = nFldRetryCount;
                tx47PdtWhsCmd.F47_UpdateDate = DateTime.Now;
                tx47PdtWhsCmd.F47_CommandNo = lpCmdNo;
                tx47PdtWhsCmd.F47_CmdSeqNo = lpSeq;
                if (endData)
                {
                    tx47PdtWhsCmd.F47_CommandEndDate = DateTime.Now;
                }
                else
                {
                    tx47PdtWhsCmd.F47_CommandSendDate = DateTime.Now;
                }

                _unitOfWork.ProductWarehouseCommandRepository.Update(tx47PdtWhsCmd);
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                var message = "update cmd table failed when set status";
                _log.Info(message);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                return -1;
            }
            return 1;
        }

        private int SetShelfSts(string lpLocation, int nStatus, int nTanaorosiFlg, int nReason = -1)
        {

            if (nStatus < 0) return 0;
            var lstPdtShfSts =
                _unitOfWork.ProductShelfStatusRepository.GetAll()
                    .ToList()
                    .Where(o => (o.F51_ShelfRow + o.F51_ShelfBay + o.F51_ShelfLevel) == lpLocation);
            try
            {
                if (!lstPdtShfSts.Any())
                {
                    var message = "Set Shelf Status failed ";
                    _log.Info(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                    return -1;
                }
                foreach (var pdtShfSts in lstPdtShfSts)
                {
                   
                        pdtShfSts.F51_ShelfStatus = nStatus.ToString();
                        pdtShfSts.F51_UpdateDate = DateTime.Now;
                        if (nStatus == 0 || nStatus == 6 || nStatus == 7)
                        {
                            if (nReason != 3)
                            {
                                pdtShfSts.F51_PalletNo = null;
                            }
                        }
                        else
                        {
                            pdtShfSts.F51_StockTakingFlag = nTanaorosiFlg.ToString();
                        }
                        _unitOfWork.ProductShelfStatusRepository.Update(pdtShfSts);
                        _unitOfWork.Commit();
                  
                    return 1;
                }
            }

            catch (Exception)
            {
                var message = "Set Shelf Status failed ";
                _log.Info(message);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                return -1;
            }
            return 0;
        }

        private int SetStockFlag(string lpPallet, int nFlag)
        {

            if (nFlag < 0) return 0;
            // update material stock flag
            var pdtShfStk =
                _unitOfWork.ProductShelfStockRepository.GetMany(i => i.F40_PalletNo.Trim().Equals(lpPallet.Trim()));
            try
            {
                foreach (var tx40PdtShfStk in pdtShfStk)
                {
                    tx40PdtShfStk.F40_StockFlag = nFlag.ToString();
                    tx40PdtShfStk.F40_UpdateDate = DateTime.Now;
                    _unitOfWork.ProductShelfStockRepository.Update(tx40PdtShfStk);
                }
                if (!pdtShfStk.Any())
                {
                    var message = "Error occured  when set stock flag";
                    _log.Info(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                    return -1;
                }
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                var message = "Error occured  when set stock flag";
                _log.Info(message);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                return -1;
            }
            return 1;
        }
        private int SetOsdPpdStockFlag(string lpPallet, int nFlg)
        {
            // set STOCKFLAG  int  OutsidePrepdtStk TABLE
            if (nFlg < 0)
            {
                return 0;
            }

            try
            {
                var outSidePrePdtStk = _unitOfWork.OutSidePrePdtStkRepository.GetById(lpPallet.Trim());
                if (outSidePrePdtStk!=null)
                {
                    outSidePrePdtStk.F53_StockFlag = nFlg.ToString();
                    outSidePrePdtStk.F53_UpdateDate = DateTime.Now;
                    _unitOfWork.OutSidePrePdtStkRepository.Update(outSidePrePdtStk);
                }
                else
                {
                    var message = "Error occured  when set stock flag";
                    _log.Info(message);
                    _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                    return -1;
                }
               
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                var message = "Error occured  when set stock flag";
                _log.Info(message);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                return -1;
            }
            return 1;
        }

        private void SetShelfTime(string lpPallet, string lpLocation, int nShelfTime)
        {

            var lstTX51PdtShfSts = _unitOfWork.ProductShelfStatusRepository.GetAll().ToList().Where(o=>(o.F51_ShelfRow+o.F51_ShelfBay+o.F51_ShelfLevel)==lpLocation);
            var tX57 = _unitOfWork.ProductShelfRepository.GetById(lpPallet.Trim());
            string message;
            try
            {
                foreach (var tX51PdtShfSts in lstTX51PdtShfSts)
                {


                    switch (nShelfTime)
                    {
                        case 0: //Storage
                            tX51PdtShfSts.F51_StorageDate = tX57.F57_StorageDate;
                            break;
                        case 1: //ReStorage
                            tX51PdtShfSts.F51_StorageDate = tX57.F57_StorageDate;
                            break;
                        case 2: // RetrievalDate
                            tX51PdtShfSts.F51_RetrievalDate = tX57.F57_RetievalDate;
                            break;
                        default:
                            return;
                    }
                    _unitOfWork.ProductShelfStatusRepository.Update(tX51PdtShfSts);
                }
                if (!lstTX51PdtShfSts.Any())
                {
                    message = "Error occured when set storage-retrieval time";
                    _log.Info(message);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseProductScreenName, message);

                }
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                message = "Error occured when set storage-retrieval time";
                _log.Info(message);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName,
                    message);

                return;
            }
            try
            {
                if (tX57 != null)
                {


                    switch (nShelfTime)
                    {
                        case 0: //Storage
                            tX57.F57_StorageDate = DateTime.Now;
                            break;
                        case 1: //ReStorage

                            tX57.F57_ReStorageDate = DateTime.Now;
                            break;
                        case 2: // RetrievalDate

                            tX57.F57_RetievalDate = DateTime.Now;
                            break;
                        default:
                            return;
                    }
                    _unitOfWork.ProductShelfRepository.Update(tX57);
                }
                else
                {
                    message = "Error occured  when set storage-retrieval time";
                    _log.Info(message);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseProductScreenName, message);
                }

                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                message = "Error occured when set storage-retrieval time";
                _log.Info(message);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName,
                    message);
            }
        }

        private int SetStgRtrHst(string lpPallet, string lpTermNo, string lpFrom, string lpTo, int nStgRtrCls,
            int nShelfTime)
        {
            // Set Material Storage-Retrieval Result
            DateTime stgRtrDate;
            var tx57PdtShf = _unitOfWork.ProductShelfRepository.GetById(lpPallet.Trim());
            string message;
            if (tx57PdtShf == null)
            {

                message = "Error occured<select>  when set storage-retrieval history";
                _log.Info(message);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                return -1;
            }

            switch (nStgRtrCls)
            {
                case 0:
                    if (tx57PdtShf.F57_StorageDate == null || !tx57PdtShf.F57_ReStorageDate.HasValue)
                    {
                        return -1;
                    }
                    if (nShelfTime == 0)
                    {
                        stgRtrDate = tx57PdtShf.F57_StorageDate.Value;

                    }
                    else
                    {
                        stgRtrDate = tx57PdtShf.F57_ReStorageDate.Value;
                    }
                    break;
                case 1:
                    if (!tx57PdtShf.F57_RetievalDate.HasValue)
                    {
                        return -1;
                    }
                    else
                    {
                        stgRtrDate = tx57PdtShf.F57_RetievalDate.Value;
                    }
                    break;
                default:
                    return -1;
            }
            var lstPdtShfStk =
                _unitOfWork.ProductShelfStockRepository.GetMany(i => i.F40_PalletNo.Trim().Equals(lpPallet.Trim()));

            if (!lstPdtShfStk.Any())
            {

                message = "Error occured<select>  when set storage-retrieval history";
                _log.Info(message);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);

                return -1;
            }
            try
            {
                foreach (var tx40PdtShfStk in lstPdtShfStk)
                {
                    var tH65PdtStgRtrHst = new TH65_PdtStgRtrHst()
                    {
                        F65_ProductCode = tx40PdtShfStk.F40_ProductCode,
                        F65_PrePdtLotNo = tx40PdtShfStk.F40_PrePdtLotNo,
                        F65_PalletNo = lpPallet,
                        F65_StgRtrDate = stgRtrDate,
                        F65_ProductLotNo = tx40PdtShfStk.F40_ProductLotNo,
                        F65_From = lpFrom,
                        F65_To = lpTo,
                        F65_StgRtrCls = nStgRtrCls.ToString(),
                        F65_TerminalNo = lpTermNo,
                        F65_Amount = tx40PdtShfStk.F40_Amount,
                        F65_AddDate = DateTime.Now,
                        F65_UpdateDate = DateTime.Now
                    };
                    _unitOfWork.ProductStorageRetrieveHistoryRepository.Add(tH65PdtStgRtrHst);
                }
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                message = "Error occured<insert>  when set storage-retrieval history";
                _log.Info(message);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
                return -1;
            }
            return 1;
        }

        private void AwError(string deviceCode)
        {
            int m_nAWstatus = 2;

            _unitOfWork.DeviceRepository.UpdateStatus(deviceCode, m_nAWstatus);
            _unitOfWork.Commit();

            var message = "Autoware House is at Error status..";
            _log.Info(message);
            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName, message);
        }

        #endregion
    }
}
