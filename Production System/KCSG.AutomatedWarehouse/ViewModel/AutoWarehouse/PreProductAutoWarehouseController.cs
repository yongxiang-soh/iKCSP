using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using GalaSoft.MvvmLight.Ioc;
using KCSG.AutomatedWarehouse.Enumeration;
using KCSG.AutomatedWarehouse.Interfaces;
using KCSG.AutomatedWarehouse.Model;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using log4net;

namespace KCSG.AutomatedWarehouse.ViewModel.AutoWarehouse
{
    public class PreProductAutoWarehouseController : WarehouseParentController, IWarehouseController
    {
        #region Properties

        #region Inversion of controls
        
        /// <summary>
        ///     Instance which handles converter business.
        /// </summary>
        private ConverterViewModel _converter;
        
        /// <summary>
        ///     Instance which handles converter business.
        /// </summary>
        public ConverterViewModel Converter
        {
            get { return _converter ?? (_converter = SimpleIoc.Default.GetInstance<ConverterViewModel>()); }
            set { _converter = value; }
        }

        #endregion

        /// <summary>
        ///     Instance which is for handling logging.
        /// </summary>
        private ILog _log;

        /// <summary>
        ///     Instance which is for handling logging.
        /// </summary>
        public ILog Log
        {
            get { return _log ?? (_log = LogManager.GetLogger(typeof(PreProductAutoWarehouseController))); }
            set { _log = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// ProceedIncommingCommand incoming terminal message.
        /// </summary>
        /// <param name="terminalMessage"></param>
        /// <param name="deviceCode"></param>
        public void ProceedIncommingCommand(TerminalMessage terminalMessage, string deviceCode)
        {
            // Command is status request.
            if (MessageCommandIndex.RequestStatus.Equals(terminalMessage.CommandIndex))
                return;

            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                // Find Pre-product command.
                var preProductWarehouseCommand = unitOfWork.PreProductWarehouseCommandRepository.GetMany(
                        i => i.F50_CmdSeqNo.Trim().Equals(terminalMessage.CommandSequence.Trim())
                        && (i.F50_CommandNo.Trim().Equals(terminalMessage.CommandIndex.Trim()) || i.F50_PictureNo.Trim().Equals("TCIP041F") || i.F50_PictureNo.Trim().Equals("TCIP042F") || i.F50_PictureNo.Trim().Equals("TCIP043F")))
                    .FirstOrDefault();

                string message;
                if (preProductWarehouseCommand == null)
                {
                    message = "Error message comes from AW!(Not in DB)";
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    return;
                }

                var nRetryCount = preProductWarehouseCommand.F50_RetryCount;

                var iPreProductCommandIndex = Converter.TextToInt(terminalMessage.Command, -1);
                var iPreProductCommandStatus = Converter.TextToInt(terminalMessage.Status, -1);

                var idsDboperationfail = "The database operation failed.";
                switch (iPreProductCommandIndex)
                {
                    case 0:
                        message = "Error message comes from AW!(Error Command)";
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                        break;
                    case 10: // accepted command 
                             // Added by Crystal Wu on 96/11/05 for Check Status when receive AW message
                             //if ((nStatus != '1') &&
                             //    (nStatus != '4') &&
                             //    (nStatus != '5'))
                             //{
                             //    message = "Error message comes from AW!(Status Error)";
                             //    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                        //    return;
                        //}
                        // end of added
                        switch (iPreProductCommandStatus)
                        {
                            case 0: // OK
                                    // update status to 3(accepted)
                                SetPrePdtCmdSts(preProductWarehouseCommand.F50_CommandNo,
                                    preProductWarehouseCommand.F50_CmdSeqNo, 3, terminalMessage.Status);
                                break;
                            case 5000: // Retry Unlimitly.
                                SetPrePdtCmdSts(preProductWarehouseCommand.F50_CommandNo,
                                    preProductWarehouseCommand.F50_CmdSeqNo, 5, terminalMessage.Status);

                                break;
                            case 1:
                            case 2:
                            case 1001:
                            case 1002:
                            case 1003:
                            case 1004: // cancel the command,set auto warehouse to error 
                                if (CancelpreProductWarehouseCommand(preProductWarehouseCommand.F50_CommandNo,
                                    preProductWarehouseCommand.F50_CmdSeqNo, 0))
                                    SetAutoWarehouseOffline(deviceCode);
                                else
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                break;
                            // added by Crystal WU on 96/11/05 for 1005 
                            case 1005:
                                // cancel the command, and set terminalMessage status to offline
                                if (
                                    !CancelpreProductWarehouseCommand(preProductWarehouseCommand.F50_CommandNo,
                                        preProductWarehouseCommand.F50_CmdSeqNo, 0))
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                break;
                            case 3:
                            case 5: // added by Crystal WU on 96/11/05
                            case 7:
                            case 9:
                            case 11:
                            case 12:
                            case 13:
                            case 14: // cancel the command, process next command

                                if (
                                    !CancelpreProductWarehouseCommand(preProductWarehouseCommand.F50_CommandNo,
                                        preProductWarehouseCommand.F50_CmdSeqNo, 0))
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                //m_pDlg->SetResendTimer();
                                break;
                            case 6: // Retry three times and cancel the command set the autoware house error.
                            case 15:
                                SetPrePdtCmdSts(preProductWarehouseCommand.F50_CommandNo,
                                    preProductWarehouseCommand.F50_CmdSeqNo, 5, terminalMessage.Status, 1);
                                if (nRetryCount >= 3)
                                    if (
                                        !CancelpreProductWarehouseCommand(preProductWarehouseCommand.F50_CommandNo,
                                            preProductWarehouseCommand.F50_CmdSeqNo, 0))
                                        Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                break;
                            case 4:

                            case 8:
                            case 10:
                                SetPrePdtCmdSts(preProductWarehouseCommand.F50_CommandNo,
                                    preProductWarehouseCommand.F50_CmdSeqNo, 5, terminalMessage.Status, 1);
                                if (nRetryCount >= 3)
                                    if (
                                        !CancelpreProductWarehouseCommand(preProductWarehouseCommand.F50_CommandNo,
                                            preProductWarehouseCommand.F50_CmdSeqNo, 0))
                                        Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                break;
                            default:

                                message = string.Format("Unknown Errcode{0} from Warehouse",
                                    terminalMessage.Status);
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                                break;
                        }
                        break;
                    case 100: // command ended
                    case 101: // middle command ended.
                              //// Added by Crystal Wu on 96/11/05 for Check Status when receive AW message
                              //if ((nStatus != '3') &&
                              //    (nStatus != '4') &&
                              //    (nStatus != '5'))
                              //{
                              //    Message.InitiateMessage(DateTime.Now, MessageType.Error,
                              //        "Error message comes from AW!(Status Error)");
                              //    return;
                              //}
                              // end of added
                        switch (iPreProductCommandStatus)
                        {
                            case 0: // OK
                                if (
                                    !EndPreProductWarehouseCommand(preProductWarehouseCommand.F50_CommandNo,
                                        preProductWarehouseCommand.F50_CmdSeqNo, terminalMessage.Command))
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                break;

                            case 60: // storage 2 times
                            case 61:
                                message = "Storage error:storage 2 times.";
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                if (
                                    !CancelpreProductWarehouseCommand(preProductWarehouseCommand.F50_CommandNo,
                                        preProductWarehouseCommand.F50_CmdSeqNo, 2))
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                break;
                            case 64: // empty retrieve

                                message = "Retrieve error:empty retrieve.";
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                if (CancelpreProductWarehouseCommand(preProductWarehouseCommand.F50_CommandNo,
                                    preProductWarehouseCommand.F50_CmdSeqNo, 3))
                                    SetAutoWarehouseOffline(deviceCode);
                                else
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                break;

                            default:

                                message = string.Format("Retrieve error ! errcode:{0}", terminalMessage.Status);
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                if (
                                    !CancelpreProductWarehouseCommand(preProductWarehouseCommand.F50_CommandNo,
                                        preProductWarehouseCommand.F50_CmdSeqNo, 0))
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                break;
                        }
                        break;
                    case 1000: // cancel
                               //// Added by Crystal Wu on 96/11/05 for Check Status when receive AW message
                               //if ((nStatus != '3') &&
                               //    (nStatus != '4') &&
                               //    (nStatus != '5'))
                               //{
                               //    message = "Error message comes from AW!(Status Error)";
                               //    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                               //    return;
                               //}
                               // end of added
                        switch (iPreProductCommandStatus)
                        {
                            case 64: // empty retrieve
                                message = "Retrieve error:empty retrieve.";
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                if (CancelpreProductWarehouseCommand(preProductWarehouseCommand.F50_CommandNo,
                                    preProductWarehouseCommand.F50_CmdSeqNo, 3))
                                    SetAutoWarehouseOffline(deviceCode);
                                else
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                break;
                            default:
                                if (
                                    !CancelpreProductWarehouseCommand(preProductWarehouseCommand.F50_CommandNo,
                                        preProductWarehouseCommand.F50_CmdSeqNo, 0))
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Find process lít
        /// </summary>
        /// <param name="preProductWarehouseCommands"></param>
        /// <param name="deviceCode"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public int ProceedMessages(IList<TX50_PrePdtWhsCmd> preProductWarehouseCommands, string deviceCode,
            ConnectionSetting endPoint)
        {
            // Operation failed message
            const string szIdsDboperationfail = "The database operation failed.";

            foreach (var preProductWarehouseCommand in preProductWarehouseCommands)
            {
                var lpConveyor = "";
                int iReturnCode;
                var pSeq = preProductWarehouseCommand.F50_CmdSeqNo;
                var szCommandNo = preProductWarehouseCommand.F50_CommandNo;
                var szCommandStatus = preProductWarehouseCommand.F50_Status;
                var iStrRtrType = Converter.TextToInt(preProductWarehouseCommand.F50_StrRtrType, -1);
                string szMessage;

                switch (szCommandStatus[0])
                {
                    case '0': // the command be created. not send.
                        int nStrRtrSts;
                        int nFlag;
                        if ("1000".Equals(szCommandNo) || "1001".Equals(szCommandNo))
                        {
                            if (iStrRtrType == 0)
                            {
                                nStrRtrSts = 1; // storage status		
                                nFlag = 1;
                            }
                            else
                            {
                                nStrRtrSts = 1; // storage status		
                                nFlag = -1;
                            }

                            lpConveyor = preProductWarehouseCommand.F50_From;
                            if ("1001".Equals(szCommandNo))
                                nStrRtrSts = -1;
                        }
                        else if ("2000".Equals(szCommandNo))
                        {
                            // retrieve
                            if (iStrRtrType == 0)
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
                            lpConveyor = preProductWarehouseCommand.F50_To;
                        }
                        else if ("6000".Equals(szCommandNo))
                        {
                            // tanaorosi storage
                            nStrRtrSts = -1;
                            nFlag = -1;
                            lpConveyor = preProductWarehouseCommand.F50_From;
                        }
                        else if ("7000".Equals(szCommandNo))
                        {
                            // tanaorosi retrieve
                            nStrRtrSts = 2;
                            nFlag = -1;
                            lpConveyor = preProductWarehouseCommand.F50_To;
                        }
                        else
                        {
                            // not supported there
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, "Unknown command in queue.");
                            Log.Error("Unknown command in queue.");
                            nStrRtrSts = -1;
                            nFlag = -1;
                        }
                        if (nStrRtrSts >= 0)
                        {
                            iReturnCode = SetConveyorStatus(lpConveyor, nStrRtrSts);
                            if (iReturnCode <= 0)
                            {
                                // conveyor is full or in not correct status
                                szMessage = $"Conveyor <{lpConveyor}> status can not permit to send message";
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, szMessage);
                                Log.Error(szMessage);
                                break;
                            }
                        }

                        iReturnCode = SendCommandToExternalTerminal(endPoint, preProductWarehouseCommand);
                        if (iReturnCode > 0)
                        {
                            // send the message OK
                            iReturnCode = SetPrePdtCmdSts(szCommandNo, pSeq, 1, "0000", 1, true);
                            if (iReturnCode <= 0)
                            {
                                szMessage = szIdsDboperationfail;
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, szMessage);
                                Log.Error(szMessage);
                            }
                        }
                        else
                        {
                            // send the message NG
                            iReturnCode = SetPrePdtCmdSts(szCommandNo, pSeq, 2, "0000", 1, true);
                            if (iReturnCode <= 0)
                            {
                                szMessage = szIdsDboperationfail;
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, szMessage);
                                Log.Error(szMessage);
                            }

                            if (preProductWarehouseCommand.F50_RetryCount >= 3)
                            {
                                //AwOffLine();
                            }

                            if (nStrRtrSts >= 0)
                            {
                                iReturnCode = SetConveyorStatus(lpConveyor, 0);
                                if (iReturnCode <= 0)
                                {
                                    szMessage = "Recover conveyor status failed";
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, szMessage);
                                    Log.Error(szMessage);
                                }
                            }
                        }

                        // update material shelf status
                        if (nFlag >= 0)
                        {
                            iReturnCode = SetStockFlag(preProductWarehouseCommand.F50_ContainerCode, nFlag);
                            if (iReturnCode > 0) return 0;

                            szMessage = "Recover conveyor status failed";
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, szMessage);
                            Log.Error(szMessage);
                        }
                        return 0;
                    case '1': // send OK but no accept message
                    case '2': // send NG
                        if (preProductWarehouseCommand.F50_RetryCount >= 3)
                        {
                            // 3 times
                            if (szCommandStatus[0] == '1')
                                SetAutoWarehouseOffline(deviceCode);
                            SetPrePdtCmdSts(szCommandNo, pSeq, 1, "0000", -1, true);
                        }
                        else
                        {
                            // less than 3 times 
                            iReturnCode = SendCommandToExternalTerminal(endPoint, preProductWarehouseCommand);
                            if (iReturnCode > 0)
                            {
                                // AW message send success
                                iReturnCode = SetPrePdtCmdSts(szCommandNo, pSeq, 1, "0000", 1, true);
                                if (iReturnCode <= 0)
                                {
                                    szMessage = szIdsDboperationfail;
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, szMessage);
                                    Log.Error(szMessage);
                                }

                                return 0;
                            }
                            // AW message send failure.
                            iReturnCode = SetPrePdtCmdSts(szCommandNo, pSeq, 2, "0000", 1, true);
                            if (iReturnCode <= 0)
                            {
                                szMessage = szIdsDboperationfail;
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, szMessage);
                                Log.Error(szMessage);
                            }
                            if (preProductWarehouseCommand.F50_RetryCount >= 3)
                            {
                                //	AwOffLine();
                            }
                        }
                        return 0;
                    case '3':
                        // do nothing
                        break;
                    case '4':
                        iReturnCode = SendCommandToExternalTerminal(endPoint, preProductWarehouseCommand);
                        if (iReturnCode > 0)
                        {
                            // send message OK.
                            iReturnCode = SetPrePdtCmdSts(szCommandNo, pSeq, 4, "0000", 1, true);
                            if (iReturnCode < 0)
                            {
                                szMessage = szIdsDboperationfail;
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, szMessage);
                                Log.Error(szMessage);
                            }
                            //	m_nTidAccept = SetTimer(TID_ACCEPT, m_nAcceptTime, NULL);
                            return 0;
                        }
                        // send message NG
                        iReturnCode = SetPrePdtCmdSts(szCommandNo, pSeq, 2, "0000", 1, true);
                        if (iReturnCode < 0)
                        {
                            szMessage = szIdsDboperationfail;
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, szMessage);
                            Log.Error(szMessage);
                        }

                        if (preProductWarehouseCommand.F50_RetryCount >= 3)
                        {
                            //	AwOffLine();
                        }
                        break;
                    case '5':
                        iReturnCode = SendCommandToExternalTerminal(endPoint, preProductWarehouseCommand);
                        if (iReturnCode > 0)
                        {
                            SetPrePdtCmdSts(szCommandNo, pSeq, 5, "0000", 1, true);
                            //	m_nTidAccept = SetTimer(TID_ACCEPT, m_nAcceptTime, NULL);
                            return 0;
                        }
                        SetPrePdtCmdSts(szCommandNo, pSeq, 2, "0000", 1, true);
                        if (preProductWarehouseCommand.F50_RetryCount >= 3)
                        {
                            //					AwOffLine();
                        }
                        break;
                }
                break;
            }
            return 0;
        }

        /// <summary>
        /// Set pre-product command status.
        /// </summary>
        /// <param name="lpCmdNo"></param>
        /// <param name="lpSeq"></param>
        /// <param name="nStatus"></param>
        /// <param name="pAbnormalCode"></param>
        /// <param name="nRetry"></param>
        /// <param name="lpDateFld"></param>
        /// <returns></returns>
        private int SetPrePdtCmdSts(string lpCmdNo, string lpSeq, int nStatus, string pAbnormalCode,
            int nRetry = 0, bool lpDateFld = false)
        {
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                // delete off empty command
                var preProduct = unitOfWork.PreProductWarehouseCommandRepository.GetMany(
                        i => i.F50_From.Trim().Equals(""))
                    .FirstOrDefault();

                if (preProduct != null)
                {
                    unitOfWork.PreProductWarehouseCommandRepository.Delete(preProduct);
                    unitOfWork.Commit();
                }

                var prePdtWhsCmd = unitOfWork.PreProductWarehouseCommandRepository.GetByCmdNoAndSeqNo(lpCmdNo, lpSeq);

                if (prePdtWhsCmd == null)
                {
                    var message = "Can’t get information from PrePdtWhsCmd table";
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
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

                    unitOfWork.PreProductWarehouseCommandRepository.Update(prePdtWhsCmd);
                    unitOfWork.Commit();
                }
                catch (Exception exception)
                {
                    Log.Error(exception.Message, exception);
                    Message.InitiateMessage(DateTime.Now, MessageType.Error,
                        "Can’t update information in PrePdtWhsCmd table");
                    return -1;
                }

                return 1;
            }
        }

        /// <summary>
        /// Cancel pre-product command
        /// </summary>
        /// <param name="commandNo"></param>
        /// <param name="cmdSeqNo"></param>
        /// <param name="nHowCancel"></param>
        /// <param name="nConveyorErr"></param>
        /// <returns></returns>
        public bool CancelpreProductWarehouseCommand(string commandNo, string cmdSeqNo, int nHowCancel,
            bool nConveyorErr = false)
        {
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var nCmdId = Converter.TextToInt(commandNo, -1);
                var nRetCode = 0;

                string lpConveyor, lpLocation;
                int nShelfSts, nStkFlg;
                string message;
                var preProduct = unitOfWork.PreProductWarehouseCommandRepository.GetByCmdNoAndSeqNo(commandNo,
                    cmdSeqNo);
                var nStrRtrType = Converter.TextToInt(preProduct.F50_StrRtrType, -1);
                var nCmdType = Converter.TextToInt(preProduct.F50_CommandType.Trim(), -1);
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
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                            nShelfSts = 6;
                            if (nStrRtrType == 0)
                            {
                                nStkFlg = 0;
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
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                    nShelfSts = 0;
                                    nStkFlg = 0;
                                    break;
                                case 1: // container 
                                    message = string.Format("CANCEL: Container Storage.  From: {0}To:{1} ",
                                        preProduct.F50_From, preProduct.F50_To);
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                    nShelfSts = 0;
                                    nStkFlg = -1;
                                    break;
                                default:
                                    message =
                                        string.Format(
                                            "Unknown Storage-Retrieve type {0} when Command be cancelled ",
                                            nStrRtrType);
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
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
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                nShelfSts = 3;
                                nStkFlg = 3;
                                break;
                            case 1: // container

                                if (nCmdType == 0)
                                {
                                    message = string.Format("CANCEL: Container Retrieve.  From: {0}To:{1}",
                                        preProduct.F50_From, preProduct.F50_To);
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                    nShelfSts = 1;
                                    nStkFlg = -1;
                                }
                                else if (nCmdType == 1)
                                {
                                    // command cancel, 
                                    message =
                                        string.Format("CANCEL: Container Retrieve Middle Command.  From: {0}To:{1}",
                                            preProduct.F50_From, preProduct.F50_To);
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
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
                            message =
                                string.Format("CANCEL: TANAONOSI Preproduct Two Times Storage.  From: {0}To:{1}",
                                    preProduct.F50_From, preProduct.F50_To);
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                        }
                        else
                        {
                            message = string.Format("CANCEL: TANAONOSI Preproduct Storage .  From: {0}To:{1}",
                                preProduct.F50_From, preProduct.F50_To);
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
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
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
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
                            message =
                                string.Format(
                                    "CANCEL: TANAONOSI Preproduct Retrieve Middle Command.  From: {0}To:{1}",
                                    preProduct.F50_From, preProduct.F50_To);
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    default:
                        // not supported in this version
                        return false;
                }

                switch (nHowCancel)
                {
                    case 0: // normal
                            // update status to 7
                        nRetCode = SetPrePdtCmdSts(preProduct.F50_CommandNo, preProduct.F50_CmdSeqNo, 7, "0000", 0,
                            true);
                        break;
                    case 1: // manual
                            // update status to 7
                        nRetCode = SetPrePdtCmdSts(preProduct.F50_CommandNo, preProduct.F50_CmdSeqNo, 7, "0000", 0,
                            true);
                        // nRetCode = C2_AMakeHistory(preProduct.F50_CommandNo, preProduct.F50_CmdSeqNo, &nErrCode);
                        break;
                    case 2: // storage 2 times
                        nRetCode = SetPrePdtCmdSts(preProduct.F50_CommandNo, preProduct.F50_CmdSeqNo, 8, "0000", 0,
                            true);
                        break;
                    case 3: // empty retrieve
                        nRetCode = SetPrePdtCmdSts(preProduct.F50_CommandNo, preProduct.F50_CmdSeqNo, 9, "0000", 0,
                            true);
                        // Set Shelf status to prohibit on 96/11/01
                        //nShelfSts = 0;
                        nShelfSts = 6;
                        // End of modified
                        if ((nCmdId == 2000 || nCmdId == 2001 || nCmdId == 7000) && nStrRtrType == 0)
                            nStkFlg = 0;
                        else
                            nStkFlg = -1;
                        break;
                }
                if (nRetCode <= 0)
                {
                    message = "Set Command Queue status failed";
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                }

                if (nConveyorErr)
                {
                    // Set the conveyor status to error
                    nRetCode = SetConveyorStatus(lpConveyor, 9);
                }
                else
                {
                    if (nCmdId != 1001)
                        nRetCode = SetConveyorStatus(lpConveyor, 0);
                }
                if (nRetCode <= 0)
                {
                    message = "Can’t set Conveyor Status";
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    message = string.Format("Set Conveyor {0} status failed", lpConveyor);
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                }


                // update shelf status of material
                if (nShelfSts >= 0)
                {
                    nRetCode = SetShelfSts(lpLocation, nShelfSts, 0, nHowCancel);
                    if (nRetCode <= 0)
                    {
                        message = string.Format("Set Shelf Status failed. LOCATION: {0}", lpLocation);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                }

                // update material stock flag
                if (nStkFlg >= 0)
                {
                    nRetCode = SetStockFlag(lpContainerCode, nStkFlg);
                    if (nRetCode <= 0)
                    {
                        message = string.Format("Set Stock Flag failed. CONTAINERCODE: {0}", lpContainerCode);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                }
                unitOfWork.Commit();
                return true;
            }
        }

        /// <summary>
        /// Set stock flag.
        /// </summary>
        /// <param name="lpContainerCode"></param>
        /// <param name="nFlag"></param>
        /// <returns></returns>
        public int SetStockFlag(string lpContainerCode, int nFlag)
        {
            if (nFlag < 0) return 0;
            using (var context = new KCSGDbContext())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    try
                    {
                        var prePdtShfStk =
                            unitOfWork.PreProductShelfStockRepository.GetMany(
                                i => i.F49_ContainerCode.Trim().Equals(lpContainerCode.Trim())).FirstOrDefault();
                        if (prePdtShfStk != null)
                        {
                            prePdtShfStk.F49_ShelfStatus = nFlag.ToString();
                            prePdtShfStk.F49_UpdateDate = DateTime.Now;
                            unitOfWork.PreProductShelfStockRepository.Update(prePdtShfStk);
                        }
                        else
                        {
                            var message = "Error occured when set stock flag";
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                            return -1;
                        }
                        unitOfWork.Commit();
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception.Message, exception);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, "Error occured when set stock flag");

                        return -1;
                    }

                    return 1;
                }
            }
        }

        public bool EndPreProductWarehouseCommand(string commandNo, string cmdSeqNo, string cmd)
        {
            using (var context = new KCSGDbContext())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var preproduct = unitOfWork.PreProductWarehouseCommandRepository.GetByCmdNoAndSeqNo(commandNo,
                        cmdSeqNo);
                    var nCmd = Converter.TextToInt(cmd, -1);

                    string lpConveyor;
                    string lpLocation;
                    int nShelfSts;
                    var nStkFlg = -1;
                    var nTanaorosiFlg = 0;
                    bool? pShelfTime = null;
                    var nStgRtrCls = -1;
                    var nStrRtrType = Converter.TextToInt(preproduct.F50_StrRtrType, 1000);

                    if (nStrRtrType > 5)
                        return false;

                    var iPreProductCommandIndex = Converter.TextToInt(commandNo, -1);
                    // added by Crystal Wu on 96/11/08 for need not to 
                    // set Conveyor Code to NULL when Dest Change
                    var nForbidReason = -1;
                    string message;

                    // end of added by Crystal Wu 
                    switch (iPreProductCommandIndex)
                    {
                        case 1000: // storage    command
                        case 1001: // re-storage command 
                            lpConveyor = preproduct.F50_From;
                            lpLocation = preproduct.F50_To;
                            switch (nStrRtrType)
                            {
                                case 0: // pre-product storage
                                    message = string.Format(
                                        "SUCCESS:  Preproduct Storage.  From: {0} To:{1} TermNo:{2}",
                                        preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                                    Message.InitiateMessage(DateTime.Now, MessageType.Success, message);
                                    pShelfTime = false;
                                    nStgRtrCls = 0;
                                    nShelfSts = 3; // storage
                                    nStkFlg = 3; // in  warehouse
                                    break;
                                case 1: // container storage
                                    message = string.Format("SUCCESS:  Container Storage.  From: {0} To:{1} TermNo:{2}",
                                        preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                                    Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                                    nShelfSts = 1;
                                    nStkFlg = -1;
                                    break;
                                default:
                                    message = string.Format("Unknown type {0} when succeed", preproduct.F50_StrRtrType);
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

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
                                        Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                                        return false;
                                    }
                                    if (nCmd == 100)
                                    {
                                        message =
                                            string.Format("SUCCESS: Preproduct Retrieve  From: {0} To:{1} TermNo:{2}",
                                                preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                                        Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                                        nShelfSts = 0;
                                        nStkFlg = 0;
                                        nStgRtrCls = 1;
                                        pShelfTime = true;
                                    }
                                    else
                                    {
                                        message = string.Format("Unknown cmdid{0} when retrieve success",
                                            preproduct.F50_CommandNo);
                                        Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                                        return false;
                                    }
                                    break;
                                case 1: // container retrieve
                                    if (nCmd == 101)
                                    {
                                        // middle command end 

                                        nShelfSts = -1;
                                        nStkFlg = -1;
                                        message =
                                            string.Format(
                                                "SUCCESS: Empty Container Retrieve  From: {0} To:{1} TermNo:{2}",
                                                preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                                        Message.InitiateMessage(DateTime.Now, MessageType.Success, message);
                                    }
                                    else if (nCmd == 100)
                                    {
                                        // container retrieve end
                                        message = string.Format("SUCCESS: Middle Command  From: {0} To:{1} TermNo:{2}",
                                            preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                                        Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

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
                                        Message.InitiateMessage(DateTime.Now, MessageType.Success, message);
                                        return false;
                                    }
                                    break;
                                default:
                                    message = string.Format("Unknown type {0} when retrieve succeed", nStrRtrType);
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                    return false;
                            }
                            break;

                        case 4000: // move-to change command 
                            // set the shelf to prohibit shelf
                            lpConveyor = preproduct.F50_From;
                            lpLocation = preproduct.F50_To;
                            if (nStrRtrType == 0)
                            {
                                message = string.Format("SUCCESS: Dest Change(Product)  From: {0} To:{1} TermNo:{2}",
                                    preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                                nShelfSts = 6; // preproduct
                                nStkFlg = -1;
                                nForbidReason = 3;
                            }
                            else
                            {
                                message = string.Format("SUCCESS: Dest Change(Container) From: {0} To:{1} TermNo:{2}",
                                    preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

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
                            Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                            break;
                        case 7000: // tanaorosi retrieve
                            lpConveyor = preproduct.F50_To;
                            lpLocation = preproduct.F50_From;
                            if (nStrRtrType == 0 && nCmd == 101)
                            {
                                // middle command end
                                message = string.Format("SUCCESS: Tanaonosi Retrieve  From: {0} To:{1} TermNo:{2}",
                                    preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                                nShelfSts = -1;
                                nStkFlg = -1;
                            }
                            else if (nStrRtrType == 0 && nCmd == 100)
                            {
                                // command end.
                                message = string.Format(
                                    "SUCCESS: Tanaonosi Middle Command  From: {0} To:{1} TermNo:{2}",
                                    preproduct.F50_From, preproduct.F50_To, preproduct.F50_TerminalNo);
                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                                nShelfSts = -1;
                            }
                            else
                            {
                                return false;
                            }
                            break;
                        default:
                            message = string.Format("Unknown id {0}  from AW Client", commandNo);
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                            return false;
                    }


                    // update status to 6
                    var nRetCode = SetPrePdtCmdSts(commandNo, preproduct.F50_CmdSeqNo, 6, "0000", 0, true);

                    if (nRetCode <= 0)
                    {
                        message = "Set Command Queue status failed";
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                    // restore conveyor number and status
                    if (iPreProductCommandIndex != 1001)
                    {
                        nRetCode = SetConveyorStatus(lpConveyor, 0);
                        if (nRetCode <= 0)
                        {
                            message = " Can’t set Conveyor Status";
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                            message = string.Format("Set Conveyor {0} status failed", lpConveyor);
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                        }
                    }
                    // update shelf status of material
                    if (nShelfSts >= 0)
                    {
                        nRetCode = SetShelfSts(lpLocation, nShelfSts, nTanaorosiFlg, nForbidReason);
                        if (nRetCode <= 0)
                        {
                            message = string.Format("Set Shelf Status failed. LOCATION: {0}", lpLocation);
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                        }
                    }

                    // update material stock flag
                    if (nStkFlg >= 0)
                    {
                        nRetCode = SetStockFlag(preproduct.F50_ContainerCode, nStkFlg);
                        if (nRetCode <= 0)
                        {
                            message = string.Format("Set Stock  Flag failed. CONTAINERCODE: {0}",
                                preproduct.F50_ContainerCode);
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                        }
                    }

                    if (pShelfTime.HasValue)
                    {
                        nRetCode = SetShelfTime(preproduct.F50_ContainerCode, pShelfTime.Value);
                        if (nRetCode < 0)
                        {
                            message = string.Format("Set Shelf Time  failed. LOCATION:{0}", lpLocation);
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
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
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                        }
                    }

                    return true;
                }
            }
        }

        private int SetStgRtrHst(string lpContainerCode, string lpTermNo, string lpFrom,
            string lpTo, int nStgRtrCls)
        {
            using (var context = new KCSGDbContext())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var prePdtShfs =
                        unitOfWork.PreProductShelfStockRepository.GetMany(
                            i => i.F49_ContainerCode.Trim().Equals(lpContainerCode.Trim())).FirstOrDefault();
                    if (prePdtShfs == null)
                    {
                        var message = "Error occured when set history of storage-retrieval";
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
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

                        unitOfWork.PreProductStorageRetrieveHistoryRepository.Add(prePdtStgRtrHst);
                        unitOfWork.Commit();
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception.Message, exception);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error,
                            "Error occured when set history of storage-retrieval");

                        return -1;
                    }
                    return 1;
                }
            }
        }

        private int SetShelfTime(string lpContainorCode, bool pShelfTime)
        {
            using (var context = new KCSGDbContext())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    try
                    {
                        var prePdtShfStk =
                            unitOfWork.PreProductShelfStockRepository.GetMany(
                                i => i.F49_ContainerCode.Trim().Equals(lpContainorCode.Trim())).FirstOrDefault();
                        if (prePdtShfStk != null)
                        {
                            if (pShelfTime)
                                prePdtShfStk.F49_RetrievalDate = DateTime.Now;
                            else
                                prePdtShfStk.F49_StorageDate = DateTime.Now;
                            unitOfWork.PreProductShelfStockRepository.Update(prePdtShfStk);
                        }
                        else
                        {
                            var message = "Error occured when set Shelf Time";
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                            return -1;
                        }
                        unitOfWork.Commit();
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception.Message, exception);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, "Error occured when set Shelf Time");
                        return -1;
                    }

                    return 1;
                }
            }
        }

        private int SetShelfSts(string lpLocation, int nStatus, int nTanaorosiFlg, int nReason = -1)
        {
            using (var context = new KCSGDbContext())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var nRetCode = 1;
                    if (nStatus < 0) return 0;
                    if (string.IsNullOrEmpty(lpLocation)) return 0;
                    var lstPrePdtShfSts =
                        unitOfWork.PreProductShelfStatusRepository.GetAll()
                            .ToList()
                            .Where(i => i.F37_ShelfRow + i.F37_ShelfBay + i.F37_ShelfLevel == lpLocation);

                    var tx37PrePdtShfStses = lstPrePdtShfSts as TX37_PrePdtShfSts[] ?? lstPrePdtShfSts.ToArray();
                    if (!tx37PrePdtShfStses.Any())
                    {
                        var message = "Error occured when set shelf status";
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                        return 0;
                    }


                    try
                    {
                        foreach (var prePdtShfSts in tx37PrePdtShfStses)
                            if (nStatus == 0 || nStatus == 6 || nStatus == 7)
                            {
                                if (nReason != 3)
                                {
                                    prePdtShfSts.F37_ContainerNo = null;
                                    prePdtShfSts.F37_ContainerCode = null;
                                }
                                prePdtShfSts.F37_ShelfStatus = nStatus.ToString();
                                prePdtShfSts.F37_UpdateDate = DateTime.Now;
                                unitOfWork.PreProductShelfStatusRepository.Update(prePdtShfSts);
                            }
                            else
                            {
                                if (nTanaorosiFlg == 1)
                                    prePdtShfSts.F37_StockTakingFlag =
                                        InventoryStockTakingFlag.InventoryChecked;

                                prePdtShfSts.F37_ShelfStatus = nStatus.ToString();
                                prePdtShfSts.F37_UpdateDate = DateTime.Now;
                                unitOfWork.PreProductShelfStatusRepository.Update(prePdtShfSts);
                            }
                        unitOfWork.Commit();
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception.Message, exception);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, "Error occured when set shelf status");
                        nRetCode = -1;
                    }
                    return nRetCode;
                }
            }
        }

        public int SetConveyorStatus(string lpConveyer, int nStatus)
        {
            using (var context = new KCSGDbContext())
            {
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var nRecordCount = 1;
                    var nRetCode = 1;
                    if (nStatus < 0)
                        return 0;

                    // restore conveyor number and status
                    try
                    {
                        if (nStatus == 1 || nStatus == 2)
                        {
                            // Storage or Retrieve send OK
                            var lstConveyor =
                                unitOfWork.ConveyorRepository.GetMany(
                                    i => i.F05_ConveyorCode.Trim().Equals(lpConveyer.Trim()));
                            foreach (var tm05Conveyor in lstConveyor)
                            {
                                var preProductStockRetrieveStatus = Converter.TextToInt(tm05Conveyor.F05_StrRtrSts, -1);
                                if (preProductStockRetrieveStatus >= 0 &&
                                    preProductStockRetrieveStatus <= nStatus &&
                                    tm05Conveyor.F05_BufferUsing < tm05Conveyor.F05_MaxBuffer)
                                {
                                    tm05Conveyor.F05_StrRtrSts = nStatus.ToString();
                                    tm05Conveyor.F05_BufferUsing = tm05Conveyor.F05_BufferUsing + 1;
                                    tm05Conveyor.F05_UpdateDate = DateTime.Now;
                                    unitOfWork.ConveyorRepository.Update(tm05Conveyor);
                                }
                            }
                        }
                        else if (nStatus == 3)
                        {
                            // Floor Movement send OK
                            var conveyor =
                                unitOfWork.ConveyorRepository.GetMany(
                                        i => i.F05_ConveyorCode == lpConveyer && i.F05_StrRtrSts == "0")
                                    .FirstOrDefault();
                            if (conveyor != null)
                            {
                                conveyor.F05_StrRtrSts = nStatus.ToString();
                                conveyor.F05_BufferUsing = conveyor.F05_BufferUsing + 1;
                                conveyor.F05_UpdateDate = DateTime.Now;
                            }
                        }
                        else if (nStatus == 0)
                        {
                            try
                            {
                                // Command End or Command Cancel

                                var conveyor =
                                    unitOfWork.ConveyorRepository.GetMany(
                                            i => i.F05_ConveyorCode == lpConveyer && i.F05_BufferUsing > 0)
                                        .FirstOrDefault();
                                if (conveyor != null)
                                {
                                    conveyor.F05_BufferUsing = conveyor.F05_BufferUsing - 1;
                                    if (conveyor.F05_BufferUsing == 0)
                                        conveyor.F05_StrRtrSts = "0";
                                    conveyor.F05_UpdateDate = DateTime.Now;
                                }
                            }
                            catch (Exception exception)
                            {
                                Log.Error(exception.Message, exception);
                                nRetCode = -1;
                            }


                            nRecordCount = nRetCode;
                            //var conveyor1 =
                            //    unitOfWork.ConveyorRepository.GetMany(
                            //        i => i.F05_ConveyorCode == lpConveyer && i.F05_BufferUsing == 0).FirstOrDefault();
                            //conveyor1.F05_StrRtrSts = "0";
                            //conveyor1.F05_UpdateDate = DateTime.Now;
                            //unitOfWork.ConveyorRepository.Update(conveyor1);
                        }
                        else
                        {
                            // If status == 9 then this conveyor is in error status
                            var conveyor1 =
                                unitOfWork.ConveyorRepository.GetById(lpConveyer.Trim());
                            conveyor1.F05_StrRtrSts = "0";
                            conveyor1.F05_UpdateDate = DateTime.Now;
                            unitOfWork.ConveyorRepository.Update(conveyor1);
                        }
                        unitOfWork.Commit();
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception.Message, exception);
                        nRetCode = -1;
                    }
                    if (nStatus == 0)
                        return nRecordCount;
                    return nRetCode;
                }
            }
        }

        /// <summary>
        ///     Build a terminal message and send to external terminal.
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="preProductWarehouseCommand"></param>
        /// <returns></returns>
        private int SendCommandToExternalTerminal(ConnectionSetting endPoint, TX50_PrePdtWhsCmd preProductWarehouseCommand)
        {
            #region Command refactor

            // added by Crystal Wu on 96/11/08 
            if ("7000".Equals(preProductWarehouseCommand.F50_CommandNo))
                preProductWarehouseCommand.F50_CommandNo = "2000";
            if ("6000".Equals(preProductWarehouseCommand.F50_CommandNo))
                preProductWarehouseCommand.F50_CommandNo = "1000";

            #endregion

            #region Command validity

            // Find terminal message in queue.
            var terminalMessage = FindCommandInQueue(preProductWarehouseCommand.F50_CmdSeqNo,
                preProductWarehouseCommand.F50_CommandNo);

            if (terminalMessage != null && !IsSendable(terminalMessage))
                return -1;

            if (terminalMessage == null)
            {
                terminalMessage = new TerminalMessage(preProductWarehouseCommand);
                Insert(terminalMessage);
            }

            #endregion

            #region Send command

            try
            {
                var tcpClient = new TcpClient();
                tcpClient.Connect(new IPEndPoint(IPAddress.Parse(endPoint.Address), endPoint.Port));
                using (var stream = tcpClient.GetStream())
                using (var streamWriter = new StreamWriter(stream))
                {
                    // Broadcast to external terminal.
                    streamWriter.AutoFlush = true;
                    streamWriter.WriteLine(terminalMessage.ToString());

                    var message = $"Sent {terminalMessage} to external terminal.";

                    Message.InitiateMessage(DateTime.Now, MessageType.Broadcast, "Sent", terminalMessage.ToString());
                    Log.Debug(message);
                }

                return 1;
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message, exception);
                return -1;
            }
            finally
            {
                // Update sent time.
                terminalMessage.Sent = DateTime.Now;
                terminalMessage.SentCount++;
            }

            #endregion
        }


        #endregion
    }
}