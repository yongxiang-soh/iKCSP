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
    public class Communication2Domain : BaseDomain, ICommunication2Domain
    {
        #region Constructor

        /// <summary>
        ///     Initiate domain with IoC.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="notificationService"></param>
        /// <param name="configurationService"></param>
        public Communication2Domain(IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Instance which is for notification handling.
        /// </summary>
        private readonly INotificationService _notificationService;

        #endregion

        #region Methods

        public ResponseResult<GridResponse<HistoryItem>> GetHistory(DateTime? date, string terminal,
            GridSettings gridSettings)
        {
            var result = _unitOfWork.PreProductWarehouseCommandHistoryRepository.GetAll();
            if (!string.IsNullOrEmpty(terminal))
                result = result.Where(i => i.F63_TerminalNo.Trim().Equals(terminal.Trim()));
            if (date.HasValue)
                result = result.Where(i => i.F63_AddDate >= date);
            var itemCount = result.Count();
            var lstResult = result.Select(i => new HistoryItem
            {
                AbnormalCode = i.F63_AbnormalCode,
                AddDate = i.F63_AddDate,
                CommandNo = i.F63_CommandNo,
                CommandSeqNo = i.F63_CmdSeqNo,
                CommandType = i.F63_CommandType, //Enum.GetName(typeof(Constants.TX34_CmdType), i.F63_CommandType),
                From = i.F63_From,
                PalletNo = i.F63_ContainerNo,
                Priority = i.F63_Priority,
                To = i.F63_To,
                CommandDate = i.F63_CommandSendDate
            });
            if (gridSettings != null)
                OrderByAndPaging(ref lstResult, gridSettings);


            var resultModel = new GridResponse<HistoryItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<HistoryItem>>(resultModel, true);
        }

        public ResponseResult<GridResponse<QueueItem>> GetQueue(DateTime? date, string terminal,
            GridSettings gridSettings)
        {
            var result = _unitOfWork.PreProductWarehouseCommandRepository.GetAll();
            if (!string.IsNullOrEmpty(terminal))
                result = result.Where(i => i.F50_TerminalNo.Trim().Equals(terminal.Trim()));
            if (date.HasValue)
                result = result.Where(i => i.F50_AddDate >= date);
            var itemCount = result.Count();
            var lstResult = result.ToList().Select(i => new QueueItem
            {
                AbnormalCode = i.F50_AbnormalCode,
                AddDate = i.F50_AddDate,
                CommandEndDate = i.F50_CommandEndDate,
                CommandNo = i.F50_CommandNo,
                CommandSendDate = i.F50_CommandSendDate,
                CommandSeqNo = i.F50_CmdSeqNo,
                CommandType = i.F50_CommandType, //Enum.GetName(typeof(Constants.TX34_CmdType), i.F50_CommandType),
                PalletNo = i.F50_ContainerNo,
                From = i.F50_From,
                PictureNo = i.F50_PictureNo,
                Priority = i.F50_Priority,
                RetryCount = i.F50_RetryCount,
                Status = i.F50_Status,
                StrRtrTypePreProduct = i.F50_StrRtrType,
                TerminalNo = i.F50_TerminalNo,
                To = i.F50_To,
                UpdateDate = i.F50_UpdateDate
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
                //	Update field [F50_RetryCount] of Preproduct Warehouse Command records whose [F50_Status] is 1, 2, 4 or 5 as 0.
                var lstTx50 =
                    _unitOfWork.PreProductWarehouseCommandRepository.GetMany(
                        i => (i.F50_Status == Constants.F34_Status.status2) ||
                             (i.F50_Status == Constants.F34_Status.status1) ||
                             (i.F50_Status == Constants.F34_Status.status4) ||
                             (i.F50_Status == Constants.F34_Status.status5));

                foreach (var tx50PreproWhsCmd in lstTx50)
                {
                    tx50PreproWhsCmd.F50_RetryCount = 0;
                    _unitOfWork.PreProductWarehouseCommandRepository.Update(tx50PreproWhsCmd);
                }
            }

            var message = "Auto-Warehouse is at " + (online ? "online" : "offline") + " status...";
            var devices = _unitOfWork.DeviceRepository.GetById(deviceCode);
            if (devices != null)
            {
                devices.F14_DeviceStatus = online
                    ? Constants.F14_DeviceStatus.Online.ToString("D")
                    : Constants.F14_DeviceStatus.Offline.ToString("D");
                devices.F14_UpdateDate = DateTime.Now;
                _unitOfWork.DeviceRepository.Update(devices);
            }
            else
            {
                message = "Error occured when set Device Status";
            }
            _unitOfWork.Commit();
            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehousePreProductScreenName,
                message);
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
            var tm50 = _unitOfWork.PreProductWarehouseCommandRepository.GetByCmdNoAndSeqNo(aw.Id, aw.Sequence);

            string message = "";
            if (tm50 == null)
            {
                message = "Error message comes from AW!(Not in DB)";
                _notificationService.SendNoteInformation(null,
                    _notificationService.AutomatedWarehousePreProductScreenName,
                    message);

                return;
            }
            var nStatus = tm50.F50_Status[0];
            var nRetryCount = tm50.F50_RetryCount;
            int nCount = 0;
            var nRetCode = 0;

            var IDS_DBOPERATIONFAIL = "The database operation failed.";
            switch (ConvertHelper.ToInteger(aw.Command))
            {
                case 0:
                    message = "Error message comes from AW!(Error Command)";
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehousePreProductScreenName,
                        message);


                    break;
                case 10: // accepted command 
                    // Added by Crystal Wu on 96/11/05 for Check Status when receive AW message
                    if (nStatus != '1' &&
                        nStatus != '4' &&
                        nStatus != '5')
                    {
                        message = "Error message comes from AW!(Status Error)";
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehousePreProductScreenName,
                            message);

                        return;
                    }
                    // end of added
                    switch (ConvertHelper.ToInteger(aw.Status))
                    {
                        case 0: // OK
                            // update status to 3(accepted)
                            nRetCode = SetPrePdtCmdSts(tm50.F50_CommandNo, tm50.F50_CmdSeqNo, 3, aw.Status);
                            break;
                        case 5000: // Retry Unlimitly.
                            nRetCode = SetPrePdtCmdSts(tm50.F50_CommandNo, tm50.F50_CmdSeqNo, 5, aw.Status);

                            break;
                        case 1:
                        case 2:
                        case 1001:
                        case 1002:
                        case 1003:
                        case 1004: // cancel the command,set auto warehouse to error 
                            if (CancelpreProductWarehouseCommand(tm50.F50_CommandNo, tm50.F50_CmdSeqNo, 0))
                            {
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
                                    _notificationService.AutomatedWarehousePreProductScreenName,
                                    IDS_DBOPERATIONFAIL);

                                return;
                            }
                            break;
                        // added by Crystal WU on 96/11/05 for 1005 
                        case 1005:
                            // cancel the command, and set aw status to offline
                            if (!CancelpreProductWarehouseCommand(tm50.F50_CommandNo, tm50.F50_CmdSeqNo, 0))
                            {
                                // cancel operation failure.
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName,
                                    IDS_DBOPERATIONFAIL);

                                return;
                            }
                            break;
                        case 3:
                        case 5: // added by Crystal WU on 96/11/05
                        case 7:
                        case 9:
                        case 11:
                        case 12:
                        case 13:
                        case 14: // cancel the command, process next command

                            if (!CancelpreProductWarehouseCommand(tm50.F50_CommandNo, tm50.F50_CmdSeqNo, 0))
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName,
                                    IDS_DBOPERATIONFAIL);

                                return;
                            }
                            //m_pDlg->SetResendTimer();
                            break;
                        case 6: // Retry three times and cancel the command set the autoware house error.
                        case 15:
                            nRetCode = SetPrePdtCmdSts(tm50.F50_CommandNo, tm50.F50_CmdSeqNo, 5, aw.Status, 1);
                            if (nRetryCount >= 3)
                            {
                                if (!CancelpreProductWarehouseCommand(tm50.F50_CommandNo, tm50.F50_CmdSeqNo, 0))
                                {
                                    _notificationService.SendNoteInformation(null,
                                        _notificationService.AutomatedWarehousePreProductScreenName,
                                        IDS_DBOPERATIONFAIL);

                                    return;
                                }
                            }
                            else
                            {
                                //m_pDlg->SetAcceptTimer();
                            }
                            break;
                        case 4:

                        case 8:
                        case 10:
                            nRetCode = SetPrePdtCmdSts(tm50.F50_CommandNo, tm50.F50_CmdSeqNo, 5, aw.Status, 1);
                            if (nRetryCount >= 3)
                            {
                                if (!CancelpreProductWarehouseCommand(tm50.F50_CommandNo, tm50.F50_CmdSeqNo, 0))
                                {
                                    _notificationService.SendNoteInformation(null,
                                        _notificationService.AutomatedWarehousePreProductScreenName,
                                        IDS_DBOPERATIONFAIL);

                                    return;
                                }
                            }
                            break;
                        default:

                            message = string.Format("Unknown Errcode{0} from Warehouse",
                                aw.Status);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehousePreProductScreenName,
                                message);

                            break;
                    }
                    break;
                case 100: // command ended
                case 101: // middle command ended.
                    // Added by Crystal Wu on 96/11/05 for Check Status when receive AW message
                    if (nStatus != '3' &&
                        nStatus != '4' &&
                        nStatus != '5')
                    {
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehousePreProductScreenName,
                            "Error message comes from AW!(Status Error)");

                        return;
                    }
                    // end of added
                    switch (ConvertHelper.ToInteger(aw.Status))
                    {
                        case 0: // OK
                            if (!EndPreProductWarehouseCommand(tm50.F50_CommandNo, tm50.F50_CmdSeqNo, aw.Command))
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName,
                                    IDS_DBOPERATIONFAIL);

                                return;
                            }
                            break;

                        case 60: // storage 2 times
                        case 61:
                            message = "Storage error:storage 2 times.";
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehousePreProductScreenName,
                                message);
                            if (!CancelpreProductWarehouseCommand(tm50.F50_CommandNo, tm50.F50_CmdSeqNo, 2))
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName,
                                    IDS_DBOPERATIONFAIL);

                                return;
                            }
                            break;
                        case 64: // empty retrieve

                            message = "Retrieve error:empty retrieve.";
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehousePreProductScreenName,
                                message);
                            if (CancelpreProductWarehouseCommand(tm50.F50_CommandNo, tm50.F50_CmdSeqNo, 3))
                            {
                                AwError(deviceCode);
                            }
                            else
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName,
                                    IDS_DBOPERATIONFAIL);

                                return;
                            }
                            break;

                        default:

                            message = string.Format("Retrieve error ! errcode:{0}", aw.Status);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehousePreProductScreenName,
                                message);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehousePreProductScreenName,
                                message);
                            if (!CancelpreProductWarehouseCommand(tm50.F50_CommandNo, tm50.F50_CmdSeqNo, 0))
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName,
                                    IDS_DBOPERATIONFAIL);

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
                        message = "Error message comes from AW!(Status Error)";
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehousePreProductScreenName,
                            message);
                        return;
                    }
                    // end of added
                    switch (ConvertHelper.ToInteger(aw.Status))
                    {
                        case 64: // empty retrieve
                            message = "Retrieve error:empty retrieve.";
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehousePreProductScreenName,
                                message);
                            if (CancelpreProductWarehouseCommand(tm50.F50_CommandNo, tm50.F50_CmdSeqNo, 3))
                            {
                                AwError(deviceCode);
                            }
                            else
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName,
                                    IDS_DBOPERATIONFAIL);

                                return;
                            }
                            break;
                        default:
                            if (!CancelpreProductWarehouseCommand(tm50.F50_CommandNo, tm50.F50_CmdSeqNo, 0, false))
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName,
                                    IDS_DBOPERATIONFAIL);
                            }
                            break;
                    }
                    break;
            }
        }

        private void AwOffLine(string deviceCode)
        {
            var device = _unitOfWork.DeviceRepository.GetByDeviceCode(deviceCode);
            if (device != null)
            {
                device.F14_DeviceStatus = "1";
                device.F14_UpdateDate = DateTime.Now;
                _unitOfWork.DeviceRepository.Update(device);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName,
                    "Autoware House is at error status..");
            }
            else
            {
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseProductScreenName,
                    "Set Device Status failed");
            }
            _unitOfWork.Commit();
        }

        public ResponseResult DeletePreProductWarehouseCommand(string commandNo, string cmdSeqNo)
        {
            var preProduct =
                _unitOfWork.PreProductWarehouseCommandRepository.GetMany(
                        i =>
                            i.F50_CommandNo.Trim().Equals(commandNo.Trim()) &&
                            i.F50_CmdSeqNo.Trim().Equals(cmdSeqNo.Trim()))
                    .FirstOrDefault();
            if (preProduct != null)
                if (((ConvertHelper.ToInteger(preProduct.F50_Status) >=
                      ConvertHelper.ToInteger(Constants.TC_CMDSTS.TC_CMDSTS_6))
                     &&
                     (ConvertHelper.ToInteger(preProduct.F50_Status) <=
                      ConvertHelper.ToInteger(Constants.TC_CMDSTS.TC_CMDSTS_9)))
                    || (preProduct.F50_Status == Constants.TC_CMDSTS.TC_CMDSTS_A)
                    || (preProduct.F50_Status == Constants.TC_CMDSTS.TC_CMDSTS_B)
                    || (preProduct.F50_Status == Constants.TC_CMDSTS.TC_CMDSTS_C)
                    || (preProduct.F50_Status == Constants.TC_CMDSTS.TC_CMDSTS_D)
                    || (preProduct.F50_Status == Constants.TC_CMDSTS.TC_CMDSTS_E)
                    || (preProduct.F50_Status == Constants.TC_CMDSTS.TC_CMDSTS_F))
                {
                    //	Create a preProduct Warehouse History by duplicating the selected preProduct Warehouse Command.

                    _unitOfWork.PreProductWarehouseCommandHistoryRepository.AddByPreproductCmd(preProduct);
                    _unitOfWork.PreProductWarehouseCommandRepository.Delete(preProduct);
                    _unitOfWork.Commit();
                    return new ResponseResult(true);
                }
                else
                {
                    //If [F50_Status] of the selected preProduct Warehouse Command record is not 6, 7, 8, 9, A, B, C, D, E and F, 
                    //the system will display error message using error message template MSG 4.
                    return new ResponseResult(false, "MSG4");
                }
            //	If there is any occurred error, the system will display message “” in field [Edit Log].
            //	Trigger use case UC 32: View Product Warehouse Commands.
            return new ResponseResult(false, "Error occurred when make history");
        }

        public bool DeletepreProductWarehouseHistories()
        {
            try
            {
                var lstTh63 = _unitOfWork.PreProductWarehouseCommandHistoryRepository.GetAll();

                foreach (var th63 in lstTh63)
                {
                    _unitOfWork.PreProductWarehouseCommandHistoryRepository.Delete(th63);
                }


                _unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EndPreProductWarehouseCommand(string commandNo, string cmdSeqNo, string cmd)
        {
            var preproduct = _unitOfWork.PreProductWarehouseCommandRepository.GetByCmdNoAndSeqNo(commandNo, cmdSeqNo);
            var nCmd = ConvertHelper.ToInteger(cmd);

            string lpConveyor;
            string lpLocation;
            int nShelfSts;
            var nStkFlg = -1;
            var nTanaorosiFlg = 0;
            bool? pShelfTime = null;
            var nStgRtrCls = -1;
            var nStrRtrType = ConvertHelper.ToInteger(preproduct.F50_StrRtrType);

            if ((ConvertHelper.ToInteger(preproduct.F50_Status) >
                 ConvertHelper.ToInteger(Constants.TC_CMDSTS.TC_CMDSTS_5)))
            {
                return false;
            }
            // added by Crystal Wu on 96/11/08 for need not to 
            // set Conveyor Code to NULL when Dest Change
            var nForbidReason = -1;

            string message;

            // end of added by Crystal Wu 
            switch (ConvertHelper.ToInteger(commandNo))
            {
                case 1000: // storage    command
                case 1001: // re-storage command 
                    lpConveyor = preproduct.F50_From;
                    lpLocation = preproduct.F50_To;
                    switch (nStrRtrType)
                    {
                        case 0: // pre-product storage
                            message = string.Format("SUCCESS:  Preproduct Storage.  From: {0} To:{1} TermNo:{2}",
                                preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehousePreProductScreenName, message);
                            pShelfTime = false;
                            nStgRtrCls = 0;
                            nShelfSts = 3; // storage
                            nStkFlg = 3; // in  warehouse
                            break;
                        case 1: // container storage
                            message = string.Format("SUCCESS:  Container Storage.  From: {0} To:{1} TermNo:{2}",
                                preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehousePreProductScreenName, message);

                            nShelfSts = 1;
                            nStkFlg = -1;
                            break;
                        default:
                            message = string.Format("Unknown type {0} when succeed", preproduct.F50_StrRtrType);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehousePreProductScreenName, message);

                            return false;
                    }
                    break;

                case 2000: // retrieve command
                case 2001: // re-retrieve command (impossible)
                    lpConveyor = preproduct.F50_To;
                    lpLocation = preproduct.F50_From;
                    switch (nStrRtrType)
                    {
                        case 0: // pre-product retrieve
                            if (nCmd == 101)
                            {
                                // "impossible" HXH said.
                                message = string.Format("Unknown cmdid {0} when retrieve success",
                                    preproduct.F50_CommandNo);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);

                                return false;
                            }
                            if (nCmd == 100)
                            {
                                message = string.Format("SUCCESS: Preproduct Retrieve  From: {0} To:{1} TermNo:{2}",
                                    preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);

                                nShelfSts = 0;
                                nStkFlg = 0;
                                nStgRtrCls = 1;
                                pShelfTime = true;
                            }
                            else
                            {
                                message = string.Format("Unknown cmdid{0} when retrieve success",
                                    preproduct.F50_CommandNo);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);

                                return false;
                            }
                            break;
                        case 1: // container retrieve
                            if (nCmd == 101)
                            {
                                // middle command end 

                                nShelfSts = -1;
                                nStkFlg = -1;
                                message = string.Format(
                                    "SUCCESS: Empty Container Retrieve  From: {0} To:{1} TermNo:{2}",
                                    preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);
                            }
                            else if (nCmd == 100)
                            {
                                // container retrieve end
                                message = string.Format("SUCCESS: Middle Command  From: {0} To:{1} TermNo:{2}",
                                    preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);

                                nShelfSts = 0;
                                nStkFlg = -1;
                            }
                            else if (nCmd == 1000)
                            {
                                // command cancel, impossible actually
                                nShelfSts = 1;
                                nStkFlg = -1;
                            }
                            else
                            {
                                message = string.Format("Unknown commandid {0} when retrieve success",
                                    preproduct.F50_CommandNo);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);
                                return false;
                            }
                            break;
                        default:
                            message = string.Format("Unknown type {0} when retrieve succeed", nStrRtrType);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehousePreProductScreenName, message);
                            return false;
                    }
                    break;

                case 4000: // move-to change command 
                    // set the shelf to prohibit shelf
                    lpConveyor = preproduct.F50_From;
                    lpLocation = preproduct.F50_To;
                    if (ConvertHelper.ToInteger(preproduct.F50_StrRtrType) == 0)
                    {
                        message = string.Format("SUCCESS: Dest Change(Product)  From: {0} To:{1} TermNo:{2}",
                            preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehousePreProductScreenName, message);

                        nShelfSts = 6; // preproduct
                        nStkFlg = -1;
                        nForbidReason = 3;
                    }
                    else
                    {
                        message = string.Format("SUCCESS: Dest Change(Container) From: {0} To:{1} TermNo:{2}",
                            preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehousePreProductScreenName, message);

                        nShelfSts = 6; // container
                        nStkFlg = -1;
                        nForbidReason = 3;
                    }
                    break;
                case 5000:
                    lpConveyor = preproduct.F50_From;
                    lpLocation = preproduct.F50_To;
                    nShelfSts = 6;
                    nStkFlg = 0;
                    break;
                case 6000: // tanaorosi storage.
                    lpConveyor = preproduct.F50_From;
                    lpLocation = preproduct.F50_To;
                    nShelfSts = 3;
                    nStkFlg = 3;
                    nTanaorosiFlg = 1;

                    message = string.Format("SUCCESS: Tanaonosi Storage  From: {0} To:{1} TermNo:{2}",
                        preproduct.F50_From,
                        preproduct.F50_To, preproduct.F50_TerminalNo);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehousePreProductScreenName, message);

                    break;
                case 7000: // tanaorosi retrieve
                    lpConveyor = preproduct.F50_To;
                    lpLocation = preproduct.F50_From;
                    if ((nStrRtrType == 0) && (nCmd == 101))
                    {
                        // middle command end
                        message = string.Format("SUCCESS: Tanaonosi Retrieve  From: {0} To:{1} TermNo:{2}",
                            preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehousePreProductScreenName, message);

                        nShelfSts = -1;
                        nStkFlg = -1;
                    }
                    else if ((nStrRtrType == 0) && (nCmd == 100))
                    {
                        // command end.
                        message = string.Format("SUCCESS: Tanaonosi Middle Command  From: {0} To:{1} TermNo:{2}",
                            preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehousePreProductScreenName, message);

                        nShelfSts = -1;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                default:
                    message = string.Format("Unknown id {0}  from AW Client", commandNo);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehousePreProductScreenName, message);
                    return false;
            }


            // update status to 6
            var nRetCode = SetPrePdtCmdSts(commandNo, preproduct.F50_CmdSeqNo, 6, "0000", 0, true);

            if (nRetCode <= 0)
            {
                message = "Set Command Queue status failed";
                _notificationService.SendNoteInformation(null,
                    _notificationService.AutomatedWarehousePreProductScreenName,
                    message);
            }
            // restore conveyor number and status
            if (ConvertHelper.ToInteger(commandNo) != 1001)
            {
                nRetCode = SetConveyorSts(lpConveyor, 0);
                if (nRetCode <= 0)
                {
                    message = " Can’t set Conveyor Status";
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehousePreProductScreenName, message);
                    message = string.Format("Set Conveyor {0} status failed", lpConveyor);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehousePreProductScreenName, message);
                }
            }
            // update shelf status of material
            if (nShelfSts >= 0)
            {
                nRetCode = SetShelfSts(lpLocation, nShelfSts, nTanaorosiFlg, nForbidReason);
                if (nRetCode <= 0)
                {
                    message = string.Format("Set Shelf Status failed. LOCATION: {0}", lpLocation);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehousePreProductScreenName, message);
                }
            }

            // update material stock flag
            if (nStkFlg >= 0)
            {
                nRetCode = SetStockFlag(preproduct.F50_ContainerCode, nStkFlg);
                if (nRetCode <= 0)
                {
                    message = string.Format("Set Stock  Flag failed. CONTAINERCODE: {0}", preproduct.F50_ContainerCode);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehousePreProductScreenName, message);
                }
            }

            if (pShelfTime.HasValue)
            {
                nRetCode = SetShelfTime(preproduct.F50_ContainerCode, pShelfTime.Value);
                if (nRetCode < 0)
                {
                    message = string.Format("Set Shelf Time  failed. LOCATION:{0}", lpLocation);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehousePreProductScreenName,
                        message);
                }
            }
            if (nStgRtrCls >= 0)
            {
                nRetCode = SetStgRtrHst(preproduct.F50_ContainerCode, preproduct.F50_TerminalNo,
                    preproduct.F50_From,
                    preproduct.F50_To,
                    nStgRtrCls);
                if (nRetCode < 0)
                {
                    message = string.Format("Set Storage-Retrieve History failed. LOCATION: {0}", lpLocation);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehousePreProductScreenName, message);
                }
            }

            return true;
        }

        public bool CancelpreProductWarehouseCommand(string commandNo, string cmdSeqNo, int nHowCancel,
            bool nConveyorErr = false)
        {
            var nCmdId = ConvertHelper.ToInteger(commandNo);

            var nRetCode = 0;

            string lpConveyor, lpLocation;
            int nShelfSts, nStkFlg;
            int nErrCode;
            int nDelShfStk = 0;
            var message = "";
            var preProduct = _unitOfWork.PreProductWarehouseCommandRepository.GetByCmdNoAndSeqNo(commandNo, cmdSeqNo);
            var nStrRtrType = ConvertHelper.ToInteger(preProduct.F50_StrRtrType);
            var nCmdType = ConvertHelper.ToInteger(preProduct.F50_CommandType.Trim());
            var lpContainerCode = preProduct.F50_ContainerCode;
            switch (nCmdId)
            {
                case 1000: // storage		command
                case 1001: // restorage	command
                case 4000: // to changed   command
                    lpConveyor = preProduct.F50_From;
                    lpLocation = preProduct.F50_To;
                    if (nHowCancel == 2)
                    {
                        // 2 times storage
                        message = string.Format("CANCEL: Two Times Storage.  From: {0}To:{1} ",
                            preProduct.F50_From, preProduct.F50_To);
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehousePreProductScreenName, message);
                        nShelfSts = 6;
                        if (nStrRtrType == 0)
                        {
                            nStkFlg = 0;
                            nDelShfStk = 1;
                        }
                        else
                        {
                            nStkFlg = -1;
                        }
                    }
                    else
                    {
                        switch (nStrRtrType)
                        {
                            case 0: // pre-product
                                message = string.Format("CANCEL: Preproduct Storage.  From: {0}To:{1} ",
                                    preProduct.F50_From, preProduct.F50_To);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);
                                nShelfSts = 0;
                                nStkFlg = 0;
                                nDelShfStk = 1;
                                break;
                            case 1: // container 
                                message = string.Format("CANCEL: Container Storage.  From: {0}To:{1} ",
                                    preProduct.F50_From, preProduct.F50_To);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);
                                nShelfSts = 0;
                                nStkFlg = -1;
                                break;
                            default:
                                message = string.Format("Unknown Storage-Retrieve type {0} when Command be cancelled ",
                                    nStrRtrType);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);
                                return false;
                        }
                    }
                    break;
                case 2000:
                case 2001:
                    lpConveyor = preProduct.F50_To;
                    lpLocation = preProduct.F50_From;
                    switch (nStrRtrType)
                    {
                        case 0: // pre-product
                            message = string.Format("CANCEL: Preproduct Retrieve.  From: {0}To:{1}",
                                preProduct.F50_From, preProduct.F50_To);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehousePreProductScreenName, message);
                            nShelfSts = 3;
                            nStkFlg = 3;
                            break;
                        case 1: // container

                            if (nCmdType == 0)
                            {
                                message = string.Format("CANCEL: Container Retrieve.  From: {0}To:{1}",
                                    preProduct.F50_From, preProduct.F50_To);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);
                                nShelfSts = 1;
                                nStkFlg = -1;
                            }
                            else if (nCmdType == 1)
                            {
                                // command cancel, 
                                message = string.Format("CANCEL: Container Retrieve Middle Command.  From: {0}To:{1}",
                                    preProduct.F50_From, preProduct.F50_To);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);
                                nShelfSts = 0;
                                nStkFlg = -1;
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        default:
                            return false;
                    }
                    break;
                case 5000:
                    // don't know how to do here
                    return false;
                    break;
                case 6000: // stock taking command	(storage )

                    lpConveyor = preProduct.F50_From;
                    lpLocation = preProduct.F50_To;
                    nShelfSts = -1;
                    nStkFlg = -1;
                    // end of modified 
                    if (nHowCancel == 2)
                    {
                        nShelfSts = 6;
                        nStkFlg = 0;
                        message = string.Format("CANCEL: TANAONOSI Preproduct Two Times Storage.  From: {0}To:{1}",
                            preProduct.F50_From, preProduct.F50_To);
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehousePreProductScreenName, message);
                    }
                    else
                    {
                        message = string.Format("CANCEL: TANAONOSI Preproduct Storage .  From: {0}To:{1}",
                            preProduct.F50_From, preProduct.F50_To);
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehousePreProductScreenName, message);
                    }
                    break;
                case 7000: // stock taking command (retrieve)
                    lpConveyor = preProduct.F50_To;
                    lpLocation = preProduct.F50_From;
                    if (nCmdType == 0)
                    {
                        nShelfSts = 3;
                        nStkFlg = 3;
                        message = string.Format("CANCEL: TANAONOSI Preproduct Retrieve.  From: {0}To:{1}",
                            preProduct.F50_From, preProduct.F50_To);
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehousePreProductScreenName, message);
                    }
                    else if (nCmdType == 1)
                    {
                        // modified by Crystal WU on 96/11/08 for TANAONOSI restorage
                        //	nShelfSts = 0;
                        //	nStkFlg = 0;
                        //	nDelShfStk = 1;
                        nShelfSts = -1;
                        nStkFlg = -1;
                        // end of modified
                        message = string.Format(
                            "CANCEL: TANAONOSI Preproduct Retrieve Middle Command.  From: {0}To:{1}",
                            preProduct.F50_From, preProduct.F50_To);
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehousePreProductScreenName, message);
                    }
                    else
                    {
                        return false;
                    }
                    break;
                default:
                    // not supported in this version
                    return false;
                    break;
            }

            switch (nHowCancel)
            {
                case 0: // normal
                    // update status to 7
                    nRetCode = SetPrePdtCmdSts(preProduct.F50_CommandNo, preProduct.F50_CmdSeqNo, 7, "0000", 0, true);
                    break;
                case 1: // manual
                    // update status to 7
                    nRetCode = SetPrePdtCmdSts(preProduct.F50_CommandNo, preProduct.F50_CmdSeqNo, 7, "0000", 0, true);
                    // nRetCode = C2_AMakeHistory(preProduct.F50_CommandNo, preProduct.F50_CmdSeqNo, &nErrCode);
                    break;
                case 2: // storage 2 times
                    nRetCode = SetPrePdtCmdSts(preProduct.F50_CommandNo, preProduct.F50_CmdSeqNo, 8, "0000", 0, true);
                    break;
                case 3: // empty retrieve
                    nRetCode = SetPrePdtCmdSts(preProduct.F50_CommandNo, preProduct.F50_CmdSeqNo, 9, "0000", 0, true);
                    // Set Shelf status to prohibit on 96/11/01
                    //nShelfSts = 0;
                    nShelfSts = 6;
                    // End of modified
                    if ((nCmdId == 2000 || nCmdId == 2001 || nCmdId == 7000) && nStrRtrType == 0)
                    {
                        nStkFlg = 0;
                    }
                    else
                    {
                        nStkFlg = -1;
                    }
                    break;
            }
            if (nRetCode <= 0)
            {
                message = "Set Command Queue status failed";
                _notificationService.SendNoteInformation(null,
                    _notificationService.AutomatedWarehousePreProductScreenName, message);
            }

            if (nConveyorErr)
            {
                // Set the conveyor status to error
                nRetCode = SetConveyorSts(lpConveyor, 9);
            }
            else
            {
                if (nCmdId != 1001)
                {
                    // restore conveyor number and status
                    nRetCode = SetConveyorSts(lpConveyor, 0);
                }
            }
            if (nRetCode <= 0)
            {
                message = "Can’t set Conveyor Status";
                _notificationService.SendNoteInformation(null,
                    _notificationService.AutomatedWarehousePreProductScreenName, message);
                message = string.Format("Set Conveyor {0} status failed", lpConveyor);
                _notificationService.SendNoteInformation(null,
                    _notificationService.AutomatedWarehousePreProductScreenName, message);
            }


            // update shelf status of material
            if (nShelfSts >= 0)
            {
                nRetCode = SetShelfSts(lpLocation, nShelfSts, 0, nHowCancel);
                if (nRetCode <= 0)
                {
                    message = string.Format("Set Shelf Status failed. LOCATION: {0}", lpLocation);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehousePreProductScreenName, message);
                }
            }

            // update material stock flag
            if (nStkFlg >= 0)
            {
                nRetCode = SetStockFlag(lpContainerCode, nStkFlg);
                if (nRetCode <= 0)
                {
                    message = string.Format("Set Stock Flag failed. CONTAINERCODE: {0}", lpContainerCode);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehousePreProductScreenName, message);
                }
            }
            _unitOfWork.Commit();
            return true;
        }


        public bool ProcessData(string deviceCode)
        {
            var device = _unitOfWork.DeviceRepository.GetById(deviceCode);
            var lstPdtWhsCmd = _unitOfWork.PreProductWarehouseCommandRepository.GetAll()
                .OrderByDescending(i => i.F50_Priority)
                .ThenByDescending(i => i.F50_Status);
            var check = Constants.F14_DeviceStatus.Online.ToString("D").Equals(device.F14_DeviceStatus);
            if (check)
            {
                lstPdtWhsCmd = lstPdtWhsCmd.Where(i => i.F50_Status.Trim().Equals(Constants.F34_Status.status6) ||
                                                       i.F50_Status.Trim().Equals(Constants.F34_Status.status7) ||
                                                       i.F50_Status.Trim().Equals(Constants.F34_Status.status8) ||
                                                       i.F50_Status.Trim().Equals(Constants.F34_Status.status9))
                    .OrderByDescending(i => i.F50_Priority)
                    .ThenByDescending(i => i.F50_Status);
                ProcessDataList(lstPdtWhsCmd, deviceCode);
                MakeHistory();
            }
            else
            {
                var message = "Autoware House is at offline or error status..";
                _notificationService.SendNoteInformation(null,
                    _notificationService.AutomatedWarehousePreProductScreenName,
                    message);
            }


            return true;
        }

        public string GetDeviceStatus(string deviceCode)
        {
            var device = _unitOfWork.DeviceRepository.GetById(deviceCode);
            return device != null ? device.F14_DeviceStatus : "";
        }


        private void MakeHistory()
        {
            try
            {
                var lstPdtWhsCmd = _unitOfWork.PreProductWarehouseCommandRepository
                    .GetMany(i => i.F50_Status.Trim().Equals(Constants.F34_Status.statusA) ||
                                  i.F50_Status.Trim().Equals(Constants.F34_Status.statusB) ||
                                  i.F50_Status.Trim().Equals(Constants.F34_Status.statusC) ||
                                  i.F50_Status.Trim().Equals(Constants.F34_Status.statusD) ||
                                  i.F50_Status.Trim().Equals(Constants.F34_Status.statusE) ||
                                  i.F50_Status.Trim().Equals(Constants.F34_Status.statusF));
                foreach (var tx50PrePdtWhsCmd in lstPdtWhsCmd)
                {
                    _unitOfWork.PreProductWarehouseCommandHistoryRepository.AddByPreproductCmd(tx50PrePdtWhsCmd);
                    _unitOfWork.PreProductWarehouseCommandRepository.Delete(tx50PrePdtWhsCmd);
                }
                if (!lstPdtWhsCmd.Any())
                {
                    //var message = "Error occured when Make history";
                    //_notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehousePreProductScreenName,
                    //    message);
                    //message = "Error occured when delete queue";
                    //_notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehousePreProductScreenName,
                    //    message);
                }
                _unitOfWork.Commit();
            }
            catch (Exception exception)
            {
                var message = exception.Message;
                _notificationService.SendNoteInformation(null,
                    _notificationService.AutomatedWarehousePreProductScreenName,
                    message);
            }
        }

        private int SetPrePdtCmdSts(string lpCmdNo, string lpSeq, int nStatus, string pAbnormalCode,
            int nRetry = 0, bool lpDateFld = false)
        {
            var prePdtWhsCmd = _unitOfWork.PreProductWarehouseCommandRepository.GetByCmdNoAndSeqNo(lpCmdNo, lpSeq);
            if (prePdtWhsCmd == null)
            {
                var message = "Can’t get information from PrePdtWhsCmd table";
                _notificationService.SendNoteInformation(null,
                    _notificationService.AutomatedWarehousePreProductScreenName,
                    message);

                return -1;
            }

            var nFldRetryCount = prePdtWhsCmd.F50_RetryCount;
            var nFldStatus = prePdtWhsCmd.F50_Status;

            if (nFldStatus == nStatus.ToString())
                nFldRetryCount += nRetry;
            else
                nFldRetryCount = nRetry;
            if (nRetry < 0) nFldRetryCount = 0;
            try
            {
                prePdtWhsCmd.F50_Status = nStatus.ToString();
                prePdtWhsCmd.F50_AbnormalCode = pAbnormalCode;
                prePdtWhsCmd.F50_RetryCount = nFldRetryCount;
                prePdtWhsCmd.F50_UpdateDate = DateTime.Now;

                if (lpDateFld)
                    prePdtWhsCmd.F50_CommandEndDate = DateTime.Now;
                else
                    prePdtWhsCmd.F50_CommandSendDate = DateTime.Now;

                _unitOfWork.PreProductWarehouseCommandRepository.Update(prePdtWhsCmd);
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                var message = "Can’t update information in PrePdtWhsCmd table";
                _notificationService.SendNoteInformation(null,
                    _notificationService.AutomatedWarehousePreProductScreenName,
                    message);

                return -1;
            }

            return 1;
        }


        private int SetShelfSts(string lpLocation, int nStatus, int nTanaorosiFlg, int nReason = -1)
        {
            var nRetCode = 1;
            if (nStatus < 0) return 0;
            if (string.IsNullOrEmpty(lpLocation)) return 0;
            var lstPrePdtShfSts =
                _unitOfWork.PreProductShelfStatusRepository.GetAll()
                    .ToList()
                    .Where(i => (i.F37_ShelfRow + i.F37_ShelfBay + i.F37_ShelfLevel) == lpLocation);

            if (!lstPrePdtShfSts.Any())
            {
                var message = "Error occured when set shelf status";
                _notificationService.SendNoteInformation(null,
                    _notificationService.AutomatedWarehousePreProductScreenName,
                    message);
                return 0;
            }


            try
            {
                foreach (var prePdtShfSts in lstPrePdtShfSts)
                {
                    if ((nStatus == 0) || (nStatus == 6) || (nStatus == 7))
                    {
                        if (nReason != 3)
                        {
                            prePdtShfSts.F37_ContainerNo = null;
                            prePdtShfSts.F37_ContainerCode = null;
                        }
                        prePdtShfSts.F37_ShelfStatus = nStatus.ToString();
                        prePdtShfSts.F37_UpdateDate = DateTime.Now;
                        _unitOfWork.PreProductShelfStatusRepository.Update(prePdtShfSts);
                    }
                    else
                    {
                        if (nTanaorosiFlg == 1)
                            prePdtShfSts.F37_StockTakingFlag =
                                Constants.F37_StockTakingFlag.InventoryChecked.ToString("D");

                        prePdtShfSts.F37_ShelfStatus = nStatus.ToString();
                        prePdtShfSts.F37_UpdateDate = DateTime.Now;
                        _unitOfWork.PreProductShelfStatusRepository.Update(prePdtShfSts);
                    }
                }
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                var message = "Error occured when set shelf status";
                _notificationService.SendNoteInformation(null,
                    _notificationService.AutomatedWarehousePreProductScreenName,
                    message);

                nRetCode = -1;
            }
            return nRetCode;
        }

        private int SetStockFlag(string lpContainerCode, int nFlag)
        {
            if (nFlag < 0) return 0;
            // update material stock flag
            try
            {
                var prePdtShfStk =
                    _unitOfWork.PreProductShelfStockRepository.GetMany(
                        i => i.F49_ContainerCode.Trim().Equals(lpContainerCode.Trim())).FirstOrDefault();
                if (prePdtShfStk != null)
                {
                    prePdtShfStk.F49_ShelfStatus = nFlag.ToString();
                    prePdtShfStk.F49_UpdateDate = DateTime.Now;
                    _unitOfWork.PreProductShelfStockRepository.Update(prePdtShfStk);
                }
                else
                {
                    var message = "Error occured when set stock flag";
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehousePreProductScreenName,
                        message);
                    return -1;
                }
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                var message = "Error occured when set stock flag";
                _notificationService.SendNoteInformation(null,
                    _notificationService.AutomatedWarehousePreProductScreenName,
                    message);

                return -1;
            }

            return 1;
        }

        private int SetShelfTime(string lpContainorCode, bool pShelfTime)
        {
            // set shelf Storage or Retieval  Date

            try
            {
                var prePdtShfStk =
                    _unitOfWork.PreProductShelfStockRepository.GetMany(
                        i => i.F49_ContainerCode.Trim().Equals(lpContainorCode.Trim())).FirstOrDefault();
                if (prePdtShfStk != null)
                {
                    if (pShelfTime)
                        prePdtShfStk.F49_RetrievalDate = DateTime.Now;
                    else
                        prePdtShfStk.F49_StorageDate = DateTime.Now;
                    _unitOfWork.PreProductShelfStockRepository.Update(prePdtShfStk);
                }
                else
                {
                    var message = "Error occured when set Shelf Time";
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehousePreProductScreenName,
                        message);

                    return -1;
                }
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                var message = "Error occured when set Shelf Time";
                _notificationService.SendNoteInformation(null,
                    _notificationService.AutomatedWarehousePreProductScreenName,
                    message);

                return -1;
            }

            return 1;
        }

        private int SetStgRtrHst(string lpContainerCode, string lpTermNo, string lpFrom,
            string lpTo, int nStgRtrCls)
        {
            var prePdtShfs =
                _unitOfWork.PreProductShelfStockRepository.GetMany(
                    i => i.F49_ContainerCode.Trim().Equals(lpContainerCode.Trim())).FirstOrDefault();
            if (prePdtShfs == null)
            {
                var message = "Error occured when set history of storage-retrieval";
                _notificationService.SendNoteInformation(null,
                    _notificationService.AutomatedWarehousePreProductScreenName,
                    message);
                return -1;
            }
            try
            {
                DateTime? shelfDate;
                switch (nStgRtrCls)
                {
                    case 0:
                        shelfDate = prePdtShfs.F49_StorageDate;
                        break;
                    case 1:
                        shelfDate = prePdtShfs.F49_RetrievalDate;
                        break;
                    default:
                        return -1;
                }

                var prePdtStgRtrHst = new TH64_PrePdtStgRtrHst();


                prePdtStgRtrHst.F64_PreProductCode = prePdtShfs.F49_PreProductCode;
                prePdtStgRtrHst.F64_PreProductLotNo = prePdtShfs.F49_PreProductLotNo;
                prePdtStgRtrHst.F64_ContainerCode = prePdtShfs.F49_ContainerCode;
                if (shelfDate != null)
                    prePdtStgRtrHst.F64_StgRtrDate = shelfDate.Value;
                prePdtStgRtrHst.F64_ContainerNo = prePdtShfs.F49_ContainerNo;
                prePdtStgRtrHst.F64_ContainerSeqNo = prePdtShfs.F49_ContainerSeqNo;
                prePdtStgRtrHst.F64_From = lpFrom;
                prePdtStgRtrHst.F64_To = lpTo;
                prePdtStgRtrHst.F64_StgRtrCls = nStgRtrCls.ToString();
                prePdtStgRtrHst.F64_TerminalNo = lpTermNo;
                prePdtStgRtrHst.F64_Amount = prePdtShfs.F49_Amount;
                prePdtStgRtrHst.F64_AddDate = DateTime.Now;
                prePdtStgRtrHst.F64_UpdateDate = DateTime.Now;

                _unitOfWork.PreProductStorageRetrieveHistoryRepository.Add(prePdtStgRtrHst);
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                var message = "Error occured when set history of storage-retrieval";
                _notificationService.SendNoteInformation(null,
                    _notificationService.AutomatedWarehousePreProductScreenName,
                    message);

                return -1;
            }
            return 1;
        }

        private void ProcessDataList(IEnumerable<TX50_PrePdtWhsCmd> lstPdtWhsCmds, string deviceCode)
        {
            int nStrRtrSts, nRetCode = 0;
            int nFlag;
            var lpConveyor = "";
            int pCmd;

            string message;
            foreach (var tx50PrePdtWhsCmd in lstPdtWhsCmds.ToList())
            {
                var nStrRtrType = ConvertHelper.ToInteger(tx50PrePdtWhsCmd.F50_StrRtrType);

                var pSeq = tx50PrePdtWhsCmd.F50_CmdSeqNo;
                pCmd = ConvertHelper.ToInteger(tx50PrePdtWhsCmd.F50_CommandNo);
                var nRetryCount = tx50PrePdtWhsCmd.F50_RetryCount;
                switch (tx50PrePdtWhsCmd.F50_Status)
                {
                    case "0": // the command be created. not send.
                        if ((pCmd == 1000) || (pCmd == 1001))
                        {
                            if (nStrRtrType == 0)
                            {
                                nStrRtrSts = 1; // storage status		
                                nFlag = 1;
                            }
                            else
                            {
                                nStrRtrSts = 1; // storage status		
                                nFlag = -1;
                            }
                            lpConveyor = tx50PrePdtWhsCmd.F50_From;
                            if (pCmd == 1001)
                                nStrRtrSts = -1;
                        }
                        else if (pCmd == 2000)
                        {
                            // retrieve
                            if (nStrRtrType == 0)
                            {
                                // pre-pdt retrieve
                                nStrRtrSts = 2;
                                nFlag = 2;
                            }
                            else
                            {
                                // empty container retrieve
                                nStrRtrSts = 2;
                                nFlag = -1;
                            }
                            lpConveyor = tx50PrePdtWhsCmd.F50_To;
                        }
                        else if (pCmd == 6000)
                        {
                            // tanaorosi storage
                            nStrRtrSts = -1;
                            nFlag = -1;
                            lpConveyor = tx50PrePdtWhsCmd.F50_From;
                        }
                        else if (pCmd == 7000)
                        {
                            // tanaorosi retrieve
                            nStrRtrSts = 2;
                            nFlag = -1;
                            lpConveyor = tx50PrePdtWhsCmd.F50_To;
                        }
                        else
                        {
                            // not supported there
                            message = "Unknown command in queue.";
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehousePreProductScreenName, message);

                            nStrRtrSts = -1;
                            nFlag = -1;
                        }
                        if (nStrRtrSts >= 0)
                        {
                            nRetCode = SetConveyorSts(lpConveyor, nStrRtrSts);
                            if (nRetCode <= 0)
                            {
                                message = " Can’t set Conveyor Status";
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);
                                // conveyor is full or in not correct status
                                message = string.Format("Conveyor{0}status can not permit to send message", lpConveyor);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);
                                break;
                            }
                        }
                        nRetCode = _notificationService.SendFromC2ToAw(tx50PrePdtWhsCmd);
                        if (nRetCode >= 0)
                        {
                            // send the message OK
                            nRetCode = SetPrePdtCmdSts(pCmd.ToString(), pSeq, 1, "0000", 1);
                            if (nRetCode <= 0)
                            {
                                message = "The database operation failed.";
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);
                            }
                            //  SetAcceptTimer();
                        }
                        else
                        {
                            // send the message NG
                            nRetCode = SetPrePdtCmdSts(pCmd.ToString(), pSeq, 2, "0000", 1);
                            if (nRetCode <= 0)
                            {
                                message = "The database operation failed.";
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);
                            }

                            if (nStrRtrSts >= 0)
                            {
                                nRetCode = SetConveyorSts(lpConveyor, 0);
                                if (nRetCode <= 0)
                                {
                                    message = " Can’t set Conveyor Status";
                                    _notificationService.SendNoteInformation(null,
                                        _notificationService.AutomatedWarehousePreProductScreenName, message);
                                    message = "Recover conveyor status failed";
                                    _notificationService.SendNoteInformation(null,
                                        _notificationService.AutomatedWarehousePreProductScreenName, message);
                                }
                            }
                            // SetResendTimer();
                        }

                        // update material shelf status
                        if (nFlag >= 0)
                        {
                            nRetCode = SetStockFlag(tx50PrePdtWhsCmd.F50_ContainerCode, nFlag);
                            if (nRetCode <= 0)
                            {
                                message = "The database operation failed.";
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);
                            }
                        }
                        return;
                    case "1": // send OK but no accept message
                    case "2": // send NG
                        if (nRetryCount >= 3)
                        {
                            // 3 times
                            if (tx50PrePdtWhsCmd.F50_Status == "1")
                                AwError(deviceCode);
                            SetPrePdtCmdSts(pCmd.ToString(), pSeq, 1, "0000", -1);
                        }
                        else
                        {
                            // less than 3 times 
                            nRetCode = _notificationService.SendFromC2ToAw(tx50PrePdtWhsCmd);
                            if (nRetCode >= 0)
                            {
                                // AW message send success
                                nRetCode = SetPrePdtCmdSts(pCmd.ToString(), pSeq, 1, "0000", 1);
                                if (nRetCode <= 0)
                                {
                                    message = "The database operation failed.";
                                    _notificationService.SendNoteInformation(null,
                                        _notificationService.AutomatedWarehousePreProductScreenName, message);
                                }
                                // SetAcceptTimer();
                                return;
                            }
                            // AW message send failure.
                            nRetCode = SetPrePdtCmdSts(pCmd.ToString(), pSeq, 2, "0000", 1);
                            if (nRetCode <= 0)
                            {
                                message = "The database operation failed.";
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);
                            }

                            //  SetResendTimer();
                        }
                        return;
                    case "3":
                        // do nothing
                        break;
                    case "4":
                        nRetCode = _notificationService.SendFromC2ToAw(tx50PrePdtWhsCmd);
                        if (nRetCode >= 0)
                        {
                            // send message OK.
                            nRetCode = SetPrePdtCmdSts(pCmd.ToString(), pSeq, 4, "0000", 1);
                            if (nRetCode < 0)
                            {
                                message = "The database operation failed.";
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);
                            }
                            //	m_nTidAccept = SetTimer(TID_ACCEPT, m_nAcceptTime, NULL);
                            // SetAcceptTimer();
                            return;
                        }

                        // send message NG
                        nRetCode = SetPrePdtCmdSts(pCmd.ToString(), pSeq, 2, "0000", 1);
                        if (nRetCode < 0)
                        {
                            message = "The database operation failed.";
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehousePreProductScreenName, message);
                        }

                        // SetResendTimer();
                        break;
                    case "5":
                        nRetCode = _notificationService.SendFromC2ToAw(tx50PrePdtWhsCmd);
                        if (nRetCode >= 0)
                        {
                            SetPrePdtCmdSts(pCmd.ToString(), pSeq, 5, "0000", 1);
                            //	m_nTidAccept = SetTimer(TID_ACCEPT, m_nAcceptTime, NULL);
                            //SetAcceptTimer();
                            return;
                        }
                        nRetCode = SetPrePdtCmdSts(pCmd.ToString(), pSeq, 2, "0000", 1);

                        // SetResendTimer();
                        break;
                    case "6":
                    case "7":
                    case "8":
                    case "9":
                        var szMid = "";
                        var sMsgToA = new MessageItem();

                        // prepare the message to A side

                        switch (tx50PrePdtWhsCmd.F50_Status)
                        {
                            case "6":
                                szMid = Constants.MessageId.TC_MID_CmdEnd.ToString("D");
                                break;
                            case "7":
                                szMid = Constants.MessageId.TC_MID_CmdCancel.ToString("D");

                                break;
                            case "8":
                                szMid = Constants.MessageId.TC_MID_ReStoraged.ToString("D");
                                break;
                            case "9":
                                szMid = Constants.MessageId.TC_MID_EmptyRetrieve.ToString("D");
                                break;
                        }
                        sMsgToA.Id = szMid;
                        sMsgToA.TerminalNo = tx50PrePdtWhsCmd.F50_TerminalNo;
                        sMsgToA.PictureNo = tx50PrePdtWhsCmd.F50_PictureNo;

                        // send the message to A side to inform cancel operation.
                        _notificationService.SendMessageFromC2(new[] {sMsgToA.TerminalNo}, sMsgToA);

                        break;

                    case "A": // impossible.
                        //#if 0	
                        //            nRetCode = C2_CommandEnd(&(tx50PrePdtWhsCmd -> cmd), tx50PrePdtWhsCmd -> nStrRtrType, tx50PrePdtWhsCmd -> sTermNo,tx50PrePdtWhsCmd ->sContainerCode, 1);
                        //            if(nRetCode < 0 ) {
                        //              //todo 	ShowMessage(The database operation failed.);
                        //            }
                        //#endif
                        break;

                    case "B": // impossible.
                        //#if 0			 
                        //            nCmdType = ConvertHelper.ToInteger(tx50PrePdtWhsCmd -> cmd.cmd,4);

                        //            nRetCode = C2_CommandCancel(&(tx50PrePdtWhsCmd -> cmd), tx50PrePdtWhsCmd -> nStrRtrType, tx50PrePdtWhsCmd ->sContainerCode ,nCmdType,1);
                        //            if(nRetCode < 0 ) {
                        //                  //todo ShowMessage(The database operation failed.);
                        //            }
                        //#endif
                        if ((pCmd == 1000) && (nStrRtrType == 0))
                        {
                            nRetCode = DelShfStk(tx50PrePdtWhsCmd.F50_ContainerCode);
                            if (nRetCode <= 0)
                            {
                                message = "Delete shelf stock table failed";
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);
                            }
                        }
                        //added by Crystal Wu on 96/11/06 for 
                        // can not find a empty location for re-assign when 2 times storage
                        AwError(deviceCode);
                        return;

                    case "C":
                        break;
                    case "D":
                        if ((pCmd == 1000) && (nStrRtrType == 0))
                        {
                            nRetCode = DelShfStk(tx50PrePdtWhsCmd.F50_ContainerCode);
                            if (nRetCode <= 0)
                            {
                                message = "Delete shelf stock table failed";
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehousePreProductScreenName, message);
                            }
                        }
                        break;
                }
            }
        }


        private void AwError(string deviceCode)
        {
            var m_nAWstatus = 1;
            _unitOfWork.DeviceRepository.UpdateStatus(deviceCode, m_nAWstatus);
            _unitOfWork.Commit();

            var message = "Autoware House is at error status..";
            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehousePreProductScreenName,
                message);
        }

        private int DelShfStk(string lpContainerCode)
        {
            try
            {
                var lstPreshelfStock =
                    _unitOfWork.PreProductShelfStockRepository.GetMany(i => i.F49_ContainerCode.Trim()
                        .Equals(lpContainerCode.Trim()));
                if (!lstPreshelfStock.Any())
                {
                    var message = "Error occured when delete unused data from TX49";
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehousePreProductScreenName,
                        message);


                    return -1;
                }
                _unitOfWork.PreProductShelfStockRepository.Delete(
                    i => i.F49_ContainerCode.Trim().Equals(lpContainerCode.Trim()));
                _unitOfWork.Commit();
            }
            catch (Exception exception)
            {
                var message = "Error occured when delete unused data from TX49";
                _notificationService.SendNoteInformation(null,
                    _notificationService.AutomatedWarehousePreProductScreenName,
                    message);

                return -1;
            }


            return 0;
        }

        #endregion
    }
}