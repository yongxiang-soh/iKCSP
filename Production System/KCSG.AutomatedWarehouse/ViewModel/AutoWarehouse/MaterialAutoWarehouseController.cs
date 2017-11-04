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

namespace KCSG.AutomatedWarehouse.ViewModel.AutoWarehouse
{
    public class MaterialAutoWarehouseController : WarehouseParentController, IWarehouseController
    {
        #region Methods

        /// <summary>
        ///     Analyze terminal messages.
        /// </summary>
        /// <param name="terminalMessage"></param>
        /// <param name="deviceCode"></param>
        public void ProceedIncommingCommand(TerminalMessage terminalMessage, string deviceCode)
        {
            // Status request.
            if (terminalMessage.CommandIndex == MessageCommandIndex.RequestStatus)
                return;

            // Message about database operation failure.
            var idsDboperationfail = "The database operation failed.";

            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var materialWarehouseCommand = unitOfWork.MaterialWarehouseCommandRepository.GetMany(
                        i => i.F34_CmdSeqNo.Trim().Equals(terminalMessage.CommandSequence.Trim())
                        && (i.F34_CommandNo.Trim().Equals(terminalMessage.CommandIndex.Trim()) || i.F34_PictureNo.Trim().Equals("TCRM082F") || i.F34_PictureNo.Trim().Equals("TCRM081F")))
                    .FirstOrDefault();

                // 2017-05-15: Make this command be acknowledged.
                var enqueuedTerminalMessage =
                    CommandsQueue.FirstOrDefault(
                        x =>
                            x.CommandSequence == terminalMessage.CommandSequence &&
                            x.CommandIndex == terminalMessage.CommandIndex);

                // Material warehouse command is invalid.
                if (materialWarehouseCommand == null)
                {
                    Message.InitiateMessage(DateTime.Now, MessageType.Error,
                        "Material warehouse command is not found.");
                    return;
                }

                var iCommandSequence = Converter.TextToInt(terminalMessage.Command, -1);

                // Message about database operation fail.
                var nRetryCount = materialWarehouseCommand.F34_RetryCount;
                switch (iCommandSequence)
                {
                    case 0:
                        Log.Debug("Error message comes from AW!(Error Command)");
                        Message.InitiateMessage(DateTime.Now, MessageType.Error,
                            "Error message comes from AW!(Error Command)");
                        break;
                    case 10: // accepted command
                        // Added by Crystal Wu on 96/11/05 for Check Status when receive AW message
                        //if ((nStatus != '1') &&
                        //    (nStatus != '4') &&
                        //    (nStatus != '5'))
                        //{
                        //    Message.InitiateMessage(DateTime.Now, MessageType.Error,
                        //        "Error message comes from AW!(Status Error)");
                        //    return;
                        //}


                        // Find material status from message.
                        var iMaterialStatus = Converter.TextToInt(terminalMessage.Status, -1);

                        // 2017-05-15: Make command be acknowledged.
                        if (enqueuedTerminalMessage != null)
                            enqueuedTerminalMessage.IsAck = true;

                        // end of added
                        //m_pDlg->RemoveAcceptTimer();
                        switch (iMaterialStatus)
                        {
                            case 0: // OK
                                //sprintf(szTmpStr, "%04.4d", TC_MID_CmdEnd);
                                //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                // update status to 3(accepted)
                                SetMaterialCommandStatus(materialWarehouseCommand.F34_CommandNo,
                                    materialWarehouseCommand.F34_CmdSeqNo, 3, terminalMessage.Status);
                                //m_pDlg -> ProcessDataBase();		// process next command
                                break;
                            case 5000: // Retry Unlimitly.
                                // Adde by Wu.Jing
                                SetMaterialCommandStatus(materialWarehouseCommand.F34_CommandNo,
                                    materialWarehouseCommand.F34_CmdSeqNo, 5, terminalMessage.Status);
                                // end of added
                                //m_pDlg->SetResendTimer();
                                break;
                            case 1: // Set AutoWare House to error status
                            case 2:
                            case 1001:
                            case 1002:
                            case 1003:
                            case 1004:
                                if (CancelMaterialCommand(materialWarehouseCommand.F34_CommandNo,
                                    materialWarehouseCommand.F34_CmdSeqNo))
                                {
                                    SetMaterialAutowarehouseFailure(deviceCode);
                                }
                                else
                                {
                                    Log.Debug(idsDboperationfail);
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                }
                                break;
                            // added by Crystal WU on 96/11/05 for 1005 
                            case 1005:
                                // cancel the command, and set aw status to offline
                                if (CancelMaterialCommand(materialWarehouseCommand.F34_CommandNo,
                                    materialWarehouseCommand.F34_CmdSeqNo))
                                {
                                    SetMaterialAutowarehouseFailure(deviceCode);
                                }
                                else
                                {
                                    Log.Debug(idsDboperationfail);
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                }
                                break;
                            // end of added
                            case 3: // cancel this command, process next command
                            case 5: // added by Crystal WU on 96/11/05
                            case 7:
                            case 9:
                            case 11:
                            case 12:
                            case 13:
                            case 14:

                                if (CancelMaterialCommand(materialWarehouseCommand.F34_CommandNo,
                                    materialWarehouseCommand.F34_CmdSeqNo))
                                {
                                    //sprintf(szTmpStr, "%04.4d", TC_MID_CmdCancel);
                                    //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                    //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                                }
                                else
                                {
                                    Log.Debug(idsDboperationfail);
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                }

                                break;
                            case 6: // Retry three times and set the autoware house error.
                            case 15:
                                SetMaterialCommandStatus(materialWarehouseCommand.F34_CommandNo,
                                    materialWarehouseCommand.F34_CmdSeqNo, 5, terminalMessage.Status, 1);
                                if (nRetryCount >= 3)
                                    if (CancelMaterialCommand(materialWarehouseCommand.F34_CommandNo,
                                        materialWarehouseCommand.F34_CmdSeqNo))
                                        SetMaterialAutowarehouseFailure(deviceCode);
                                    else
                                        Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                break;
                            case 4: // Retry three times, cancel this command,process next command
                            //		case 5:       deleted by Crystal Wu on 96/11/05
                            case 8:
                            case 10:
                                SetMaterialCommandStatus(materialWarehouseCommand.F34_CommandNo,
                                    materialWarehouseCommand.F34_CmdSeqNo, 5, terminalMessage.Status, 1);
                                if (nRetryCount >= 3)
                                    if (CancelMaterialCommand(materialWarehouseCommand.F34_CommandNo,
                                        materialWarehouseCommand.F34_CmdSeqNo))
                                    {
                                        //sprintf(szTmpStr, "%04.4d", TC_MID_CmdCancel);
                                        //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                        //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                                    }
                                    else
                                    {
                                        Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                    }
                                break;
                            default:
                                var message = string.Format("Unexpected error code {0} from AW client",
                                    terminalMessage.Status);
                                Log.Debug(message);
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                                break;
                        }
                        break;
                    case 100: // command ended
                        // Added by Crystal Wu on 96/11/05 for Check Status when receive AW message
                        //if ((nStatus != '3') &&
                        //    (nStatus != '4') &&
                        //    (nStatus != '5'))
                        //{
                        //    Message.InitiateMessage(DateTime.Now, MessageType.Error,
                        //        "Error message comes from AW!(Status Error)");
                        //    return;
                        //}

                        // 2017-05-15: Make command be acknowledged.
                        if (enqueuedTerminalMessage != null)
                            enqueuedTerminalMessage.IsAck = true;

                        int.TryParse(terminalMessage.Status, out iMaterialStatus);
                        // end of added
                        switch (iMaterialStatus)
                        {
                            case 0: // OK
                                if (EndMaterialWarehouseCommand(materialWarehouseCommand.F34_CommandNo,
                                    materialWarehouseCommand.F34_CmdSeqNo))
                                {
                                    //sprintf(szTmpStr, "%04.4d", TC_MID_CmdEnd);
                                    //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                    //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                                }
                                else
                                {
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                }
                                break;

                            case 60: // storage 2 times
                            case 61:
                                Message.InitiateMessage(DateTime.Now, MessageType.Error,
                                    "Storage error:storage 2 times.");
                                Log.Debug("Storage error:storage 2 times.");
                                if (CancelMaterialCommand(materialWarehouseCommand.F34_CommandNo,
                                    materialWarehouseCommand.F34_CmdSeqNo, 2))
                                {
                                    //sprintf(szTmpStr, "%04.4d", TC_MID_ReStoraged);
                                    //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                    //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                                }
                                else
                                {
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                }
                                break;

                            case 64: // empty retrieve
                                Message.InitiateMessage(DateTime.Now, MessageType.Error,
                                    "Retrieve error:empty retrieve.");
                                Log.Debug("Retrieve error:empty retrieve.");
                                if (CancelMaterialCommand(materialWarehouseCommand.F34_CommandNo,
                                    materialWarehouseCommand.F34_CmdSeqNo, 3))
                                    SetMaterialAutowarehouseFailure(deviceCode);
                                else
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                break;

                            default:
                                var message = string.Format("Error ! errcode{0}", terminalMessage.Status);
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                Log.Debug(message);
                                if (CancelMaterialCommand(materialWarehouseCommand.F34_CommandNo,
                                    materialWarehouseCommand.F34_CmdSeqNo))
                                {
                                    //sprintf(szTmpStr, "%04.4d", TC_MID_CmdCancel);
                                    //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                    //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                                }
                                else
                                {
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                }
                                break;
                        }
                        break;
                    case 1000: // cancel
                        // Added by Crystal Wu on 96/11/05 for Check Status when receive AW message
                        //if ((nStatus != '3') &&
                        //    (nStatus != '4') &&
                        //    (nStatus != '5'))
                        //{
                        //    Message.InitiateMessage(DateTime.Now, MessageType.Error,
                        //        "Error message comes from AW!(Status Error)");
                        //    return;
                        //}

                        int.TryParse(terminalMessage.Status, out iMaterialStatus);
                        // end of added
                        switch (iMaterialStatus)
                        {
                            case 64: // empty retrieve
                                Message.InitiateMessage(DateTime.Now, MessageType.Error,
                                    "Retrieve error:empty retrieve.");
                                if (CancelMaterialCommand(materialWarehouseCommand.F34_CommandNo,
                                    materialWarehouseCommand.F34_CmdSeqNo, 3))
                                    SetMaterialAutowarehouseFailure(deviceCode);
                                else
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                break;
                            default:
                                if (CancelMaterialCommand(materialWarehouseCommand.F34_CommandNo,
                                    materialWarehouseCommand.F34_CmdSeqNo))
                                {
                                    //sprintf(szTmpStr, "%04.4d", TC_MID_CmdCancel);
                                    //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                    //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                                }
                                else
                                {
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                }
                                break;
                        }
                        break;

                    default:
                        Log.Error("Error message comes from AW!(Error Command)");
                        Message.InitiateMessage(DateTime.Now, MessageType.Error,
                            "Error message comes from AW!(Error Command)");
                        break;
                }
            }
        }

        /// <summary>
        ///     Process database records list.
        /// </summary>
        /// <param name="materialWarehouseCommands"></param>
        /// <param name="deviceCode"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public int ProcessDataBase(IList<TX34_MtrWhsCmd> materialWarehouseCommands, string deviceCode,
            ConnectionSetting endPoint)
        {
            int nStrRtrSts, nRetCode = -1;
            int nFlag;
            var lpConveyor = "";
            string pSeq, pCmd;
            var nStrRtrType = -1;

            foreach (var materialWarehouseCommand in materialWarehouseCommands)
            {
                pSeq = materialWarehouseCommand.F34_CmdSeqNo;
                pCmd = materialWarehouseCommand.F34_CommandNo;
                nStrRtrType = Converter.TextToInt(materialWarehouseCommand.F34_StrRtrType, -1);

                switch (materialWarehouseCommand.F34_Status[0])
                {
                    case '0':
                        if ("1000".Equals(pCmd) || "1001".Equals(pCmd))
                        {
                            nStrRtrSts = 1;
                            if (nStrRtrType == 0)
                                nFlag = 1; // Material 
                            else
                                nFlag = -1; // Empty Pallet , Suppiler Pallet.
                            lpConveyor = materialWarehouseCommand.F34_From;
                            // added by Crystal Wu for 2 times storage need not to check conveyor status
                            if ("1001".Equals(pCmd))
                                nStrRtrSts = -1;
                            // end of added
                        }
                        else if ("2000".Equals(pCmd) || "2001".Equals(pCmd))
                        {
                            nStrRtrSts = 2;
                            if (nStrRtrType == 0)
                                nFlag = 2; // Material
                            else
                                nFlag = -1;
                            lpConveyor = materialWarehouseCommand.F34_To;
                        }
                        else if ("3000".Equals(pCmd))
                        {
                            // move between floors
                            nStrRtrSts = 3;
                            nFlag = -1; // not need to update shelf status
                            lpConveyor = materialWarehouseCommand.F34_From;
                            nRetCode = SetConveyorStatus(lpConveyor, nStrRtrSts);
                            if (nRetCode <= 0)
                            {
                                //char tmpbuf[256];
                                //sprintf(tmpbuf, "Conveyor<%-6.6s> status can not permit to send message",
                                //        lpConveyor);
                                //m_pDlg->ShowMessage(tmpbuf);
                                Message.InitiateMessage(DateTime.Now, MessageType.Error,
                                    $"Conveyor <{lpConveyor}> - 1 -  status can not permit to send message");
                                break;
                            }
                            lpConveyor = materialWarehouseCommand.F34_To;
                        }
                        else if ("6000".Equals(pCmd))
                        {
                            nStrRtrSts = -1;
                            nFlag = -1;
                            lpConveyor = materialWarehouseCommand.F34_From;
                        }
                        else if ("7000".Equals(pCmd))
                        {
                            nStrRtrSts = 2;
                            nFlag = -1;
                            lpConveyor = materialWarehouseCommand.F34_To;
                        }
                        else
                        {
                            // not supported there
                            nStrRtrSts = -1; // 3 should be change to other value
                            nFlag = -1;
                        }

                        if (nStrRtrSts >= 0)
                        {
                            nRetCode = SetConveyorStatus(lpConveyor, nStrRtrSts);
                            if (nRetCode <= 0)
                            {
                                //char tmpbuf[256];
                                //sprintf(tmpbuf, "Conveyor<%-6.6s> status can not permit to send message",
                                //        lpConveyor);
                                //m_pDlg->ShowMessage(tmpbuf);
                                Message.InitiateMessage(DateTime.Now, MessageType.Error,
                                    $"Conveyor {lpConveyor} - 2 - status can not permit to send message");
                                if ("3000".Equals(pCmd))
                                {
                                    lpConveyor = materialWarehouseCommand.F34_From;
                                    nRetCode = SetConveyorStatus(lpConveyor, 0);
                                    if (nRetCode <= 0)
                                        Message.InitiateMessage(DateTime.Now, MessageType.Error,
                                            $"Recover conveyor <{lpConveyor}> status failed");
                                }

                                break;
                            }
                        }


                        nRetCode = SendCommandToExternalTerminal(endPoint, materialWarehouseCommand);

                        if (nRetCode > 0)
                        {
                            // send the message OK
                            nRetCode = SetMaterialCommandStatus(pCmd, pSeq, 1, "0000", 1, true);
                        }
                        else
                        {
                            // send the message NG
                            nRetCode = SetMaterialCommandStatus(pCmd, pSeq, 2, "0000", 1, true);
                            if (C1_CheckRetryEnd(materialWarehouseCommand.F34_RetryCount))
                                SetMaterialAutowarehouseFailure(deviceCode);
                            if (nStrRtrSts >= 0)
                            {
                                nRetCode = SetConveyorStatus(lpConveyor, 0);
                                if (nRetCode <= 0)
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error,
                                        "Recover conveyor status failed");
                            }
                            if ("3000".Equals(pCmd))
                            {
                                lpConveyor = materialWarehouseCommand.F34_From;
                                nRetCode = SetConveyorStatus(lpConveyor, 0);
                                if (nRetCode <= 0)
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error,
                                        $"Recover conveyor <{lpConveyor}> status failed");
                            }
                        }

                        // update material shelf status
                        if (nFlag >= 0)
                            nRetCode = SetStockFlag(materialWarehouseCommand.F34_PalletNo, nFlag);
                        return 0;
                    case '1': // send OK but no accept message
                    case '2': // send NG
                        if (materialWarehouseCommand.F34_RetryCount >= 3)
                        {
                            // 3 times
                            if (materialWarehouseCommand.F34_Status[0] == '1')
                                SetMaterialAutowarehouseFailure(deviceCode);
                            nRetCode = SetMaterialCommandStatus(pCmd, pSeq, 1, "0000", -1, true);
                        }
                        else
                        {
                            nRetCode = SendCommandToExternalTerminal(endPoint, materialWarehouseCommand);
                            if (nRetCode > 0)
                            {
                                // AW message send success
                                nRetCode = SetMaterialCommandStatus(pCmd, pSeq, 1, "0000", 1, true);
                                return 0;
                            }
                            // AW message send failure.
                            nRetCode = SetMaterialCommandStatus(pCmd, pSeq, 2, "0000", 1, true);
                            if (C1_CheckRetryEnd(materialWarehouseCommand.F34_RetryCount))
                                SetMaterialAutowarehouseFailure(deviceCode);
                        }

                        return 0;
                    case '3':
                        // do nothing
                        break;
                    case '4':
                        nRetCode = SendCommandToExternalTerminal(endPoint, materialWarehouseCommand);
                        if (nRetCode > 0)
                        {
                            nRetCode = SetMaterialCommandStatus(pCmd, pSeq, 4, "0000", 1, true);
                            //	m_nTidAccept = SetTimer(TID_ACCEPT, m_nAcceptTime, NULL);
                            return 0;
                        }
                        nRetCode = SetMaterialCommandStatus(pCmd, pSeq, 2, "0000", 1, true);
                        if (C1_CheckRetryEnd(materialWarehouseCommand.F34_RetryCount))
                            SetMaterialAutowarehouseFailure(deviceCode);
                        break;
                    case '5':
                        nRetCode = SendCommandToExternalTerminal(endPoint, materialWarehouseCommand);
                        if (nRetCode > 0)
                        {
                            nRetCode = SetMaterialCommandStatus(pCmd, pSeq, 5, "0000", 1, true);
                            //m_nTidAccept = SetTimer(TID_ACCEPT, m_nAcceptTime, NULL);
                            //SetAcceptTimer();
                            return 0;
                        }
                        nRetCode = SetMaterialCommandStatus(pCmd, pSeq, 2, "0000", 1, true);
                        if (C1_CheckRetryEnd(materialWarehouseCommand.F34_RetryCount))
                            SetMaterialAutowarehouseFailure(deviceCode);
                        break;
                    //case '6':
                    //case '7':
                    //case '8':
                    //case '9':
                    //    // send the message to A side
                    //    char szTermNo[10];
                    //    char szPicNo[10];
                    //    char szMid[10];
                    //    char szSize[10];
                    //    TC_Msg_S1Comm sMsgToA;

                    //    // prepare the message to A side
                    //    memset(&sMsgToA, 0, sizeof(sMsgToA));
                    //    switch (pDataList->sStatus[0])
                    //    {
                    //        case '6':
                    //            sprintf(szMid, "%-4.4d", TC_MID_CmdEnd);
                    //            break;
                    //        case '7':
                    //            sprintf(szMid, "%-4.4d", TC_MID_CmdCancel);
                    //            break;
                    //        case '8':
                    //            sprintf(szMid, "%-4.4d", TC_MID_ReStoraged);
                    //            break;
                    //        case '9':
                    //            sprintf(szMid, "%-4.4d", TC_MID_EmptyRetrieve);
                    //            break;
                    //    }

                    //    memcpy(sMsgToA.head.mid, szMid, 4);
                    //    sprintf(szTermNo, "%-4.4s", pDataList->sTermNo);
                    //    memcpy(sMsgToA.head.term_no, szTermNo, 4);
                    //    sprintf(szPicNo, "%-8.8s", pDataList->sPicNo);
                    //    memcpy(sMsgToA.head.pic_no, szPicNo, 8);
                    //    sprintf(szSize, "%04.4d", sizeof(sMsgToA));
                    //    memcpy(sMsgToA.head.size, szSize, 4);

                    //    // send the message to A side to inform cancel operation.
                    //    SendMessageToA(pDataList->sTermNo, &sMsgToA, sizeof(sMsgToA));

                    //    break;
                    //case 'A':   // impossible.
                    //            //	nRetCode = C1_CommandEnd(&(pDataList -> cmd), pDataList -> nStrRtrType, pDataList -> sTermNo,1);
                    //    break;

                    //case 'B':   // impossible.
                    //            //	nRetCode = C1_CommandCancel(&(pDataList -> cmd), pDataList -> nStrRtrType, 1);
                    //    if ((strncmp(pCmd, "1000", 4) == 0) && (nStrRtrType == 0))
                    //    {
                    //        nRetCode = C1_DelStock(pDataList->cmd.pallet, &m_nErrCode);
                    //    }
                    //    //added by Crystal Wu on 96/11/06 for 
                    //    // can not find a empty location for re-assign when 2 times storage
                    //    AwError();
                    //    return 0;
                    //    // end of added
                    //    break;


                    //case 'C':
                    //    break;
                    //case 'D':
                    //    if ((strncmp(pCmd, "1000", 4) == 0) && (nStrRtrType == 0))
                    //    {
                    //        nRetCode = C1_DelStock(pDataList->cmd.pallet, &m_nErrCode);
                    //    }
                    //    break;
                    //case 'E':
                    //    break;
                    //case 'F':
                    //    // do nothing
                    //    break;
                    default:
                        break;
                }
            }
            return 0;
        }

        /// <summary>
        ///     Set material command status base on specific conditions.
        /// </summary>
        /// <param name="lpCmdNo"></param>
        /// <param name="lpSeq"></param>
        /// <param name="nStatus"></param>
        /// <param name="pAbnormalCode"></param>
        /// <param name="nRetry"></param>
        /// <param name="updateCommandEndDate"></param>
        /// <returns></returns>
        private int SetMaterialCommandStatus(
            string lpCmdNo, string lpSeq, int nStatus, string pAbnormalCode, int nRetry = 0,
            bool updateCommandEndDate = false)
        {
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var materialWarehouseCommand = unitOfWork.MaterialWarehouseCommandRepository.GetMtrWhsCmdByKey(lpCmdNo,
                    lpSeq);
                if (materialWarehouseCommand == null)
                {
                    Log.Error("Error occured when set Cmd Status");
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, "Error occured when set Cmd Status");
                    return -1;
                }

                var nFldRetryCount = materialWarehouseCommand.F34_RetryCount ?? 0;
                var iMaterialStatus = 0;
                int.TryParse(materialWarehouseCommand.F34_Status, out iMaterialStatus);

                if (iMaterialStatus == nStatus)
                    nFldRetryCount += nRetry;
                else
                    nFldRetryCount = nRetry;
                if (nRetry < 0) nFldRetryCount = 0;
                materialWarehouseCommand.F34_Status = nStatus.ToString();
                materialWarehouseCommand.F34_AbnormalCode = pAbnormalCode;
                materialWarehouseCommand.F34_RetryCount = nFldRetryCount;
                materialWarehouseCommand.F34_UpdateDate = DateTime.Now;
                if (updateCommandEndDate)
                    materialWarehouseCommand.F34_CommandEndDate = DateTime.Now;
                else
                    materialWarehouseCommand.F34_CommandSendDate = DateTime.Now;
                try
                {
                    unitOfWork.MaterialWarehouseCommandRepository.Update(materialWarehouseCommand);
                    unitOfWork.Commit();
                }
                catch (Exception exception)
                {
                    Log.Error(exception.Message, exception);
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, "Error occured when set Cmd Status");
                    return -1;
                }
                return 1;
            }
        }

        /// <summary>
        ///     Cancel terminal command by using specific conditions.
        /// </summary>
        /// <param name="commandNo"></param>
        /// <param name="cmdSeqNo"></param>
        /// <param name="nHowCancel"></param>
        /// <param name="nConveyorErr"></param>
        /// <returns></returns>
        private bool CancelMaterialCommand(string commandNo, string cmdSeqNo, int nHowCancel = 0,
            bool nConveyorErr = false)
        {
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                // Find material warehouse command from database.
                var materialWarehouseCommand = unitOfWork.MaterialWarehouseCommandRepository.GetMtrWhsCmdByKey(
                    commandNo,
                    cmdSeqNo);
                if (materialWarehouseCommand == null)
                    return false;

                var iStorageRetrieveType = 0;
                int.TryParse(materialWarehouseCommand.F34_StrRtrType, out iStorageRetrieveType);

                string lpConveyor, lpLocation = "";
                int nShelfSts, nStkFlg;
                var nRetCode = 0;

                int iMaterialWarehouseCommandStatus;
                int.TryParse(materialWarehouseCommand.F34_Status, out iMaterialWarehouseCommandStatus);

                if (iMaterialWarehouseCommandStatus > 5)
                    return false;

                string message;
                var iCommandNo = 0;
                int.TryParse(commandNo, out iCommandNo);

                switch (iCommandNo)
                {
                    case 1000: // storage		command
                    case 1001: // restorage	command
                        lpConveyor = materialWarehouseCommand.F34_From;
                        lpLocation = materialWarehouseCommand.F34_To;
                        if (nHowCancel == 2)
                        {
                            message = string.Format("CANCEL: Storage 2 times.  From: {0} To: {1}",
                                materialWarehouseCommand.F34_From,
                                materialWarehouseCommand.F34_To);
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                            Log.Error(message);
                            // 2 times storage
                            nShelfSts = 6;
                            if (iStorageRetrieveType == 0)
                                nStkFlg = 0;
                            else
                                nStkFlg = -1;
                        }
                        else
                        {
                            switch (iStorageRetrieveType)
                            {
                                case 0: // material

                                    message = string.Format("CANCEL: Material Storage  From: {0} To: {1}",
                                        materialWarehouseCommand.F34_From, materialWarehouseCommand.F34_To);
                                    Log.Error(message);
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                                    nShelfSts = 0;
                                    nStkFlg = 0;
                                    break;
                                case 1: // empty pallet
                                case 2: // provider's pallet

                                    message = string.Format("CANCEL: Pallet Storage  From: {0} To: {1}",
                                        materialWarehouseCommand.F34_From, materialWarehouseCommand.F34_To);
                                    Log.Error(message);
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                    nShelfSts = 0;
                                    nStkFlg = -1;
                                    break;
                                default:
                                    return false;
                            }
                        }
                        break;
                    case 2000:
                    case 2001:
                        lpConveyor = materialWarehouseCommand.F34_To;
                        lpLocation = materialWarehouseCommand.F34_From;
                        switch (iStorageRetrieveType)
                        {
                            case 0: // material
                                message = string.Format("CANCEL: Material Retrieve  From: {0} To: {1}",
                                    materialWarehouseCommand.F34_From, materialWarehouseCommand.F34_To);
                                Log.Error(message);
                                // Display message to UI.
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                                nShelfSts = 3;
                                nStkFlg = 3;
                                break;
                            case 1: // empty pallet
                                message = string.Format("CANCEL: Empty Pallet Retrieve  From: {0} To: {1}",
                                    materialWarehouseCommand.F34_From, materialWarehouseCommand.F34_To);
                                Log.Error(message);
                                // Display message to UI.
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                                nShelfSts = 1;
                                nStkFlg = -1;
                                break;
                            case 2: // provider's pallet retrieve (front)

                                message = string.Format("CANCEL: Provider's Pallet Retrieve  From: {0} To: {1}",
                                    materialWarehouseCommand.F34_From, materialWarehouseCommand.F34_To);
                                Log.Error(message);
                                // Display message to UI.
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                                nShelfSts = 2;
                                nStkFlg = -1;
                                break;
                            case 3: // provider's pallet retrieve (side)
                                message = string.Format("CANCEL: Provider's Pallet Retrieve  From: {0} To: {1}",
                                    materialWarehouseCommand.F34_From, materialWarehouseCommand.F34_To);

                                // Display message to UI.
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                Log.Error(message);
                                nShelfSts = -1;
                                nStkFlg = -1;
                                break;
                            default:
                                return false;
                        }
                        break;
                    case 3000: // Moving between conveyers
                        nShelfSts = -1;
                        nStkFlg = -1;
                        lpConveyor = materialWarehouseCommand.F34_From;
                        if (nConveyorErr)
                            nRetCode = SetConveyorStatus(lpConveyor, 9);
                        else
                            nRetCode = SetConveyorStatus(lpConveyor, 0);
                        lpConveyor = materialWarehouseCommand.F34_To;

                        message = string.Format("CANCEL: Material Move  From: {0} To: {1}",
                            materialWarehouseCommand.F34_From,
                            materialWarehouseCommand.F34_To);

                        // Display message to UI.
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                        Log.Error(message);
                        break;

                    case 4000:
                    case 5000:
                        // don't know how to do here
                        return false;
                    case 6000: // stock taking command	(storage )
                        lpConveyor = materialWarehouseCommand.F34_From;
                        lpLocation = materialWarehouseCommand.F34_To;
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

                        message = string.Format("CANCEL: TANAONOSI Storage  From: {0} To: {1}",
                            materialWarehouseCommand.F34_From,
                            materialWarehouseCommand.F34_To);

                        // Display message to UI.
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                        Log.Error(message);
                        break;
                    case 7000: // stock taking command (retrieve)
                        lpConveyor = materialWarehouseCommand.F34_To;
                        lpLocation = materialWarehouseCommand.F34_From;
                        nShelfSts = 3;
                        nStkFlg = 3;

                        message = string.Format("CANCEL: TANAONOSI  Retrieve  From: {0} To: {1}",
                            materialWarehouseCommand.F34_From,
                            materialWarehouseCommand.F34_To);

                        // Display message to UI.
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                        Log.Error(message);
                        break;
                    default:
                        // not supported in this version
                        return false;
                }

                switch (nHowCancel)
                {
                    case 0: // normal
                        // update status to 7
                        nRetCode = SetMaterialCommandStatus(materialWarehouseCommand.F34_CommandNo,
                            materialWarehouseCommand.F34_CmdSeqNo, 7,
                            "0000", 0, true);
                        break;
                    case 1: // manual
                        // update status to 7
                        nRetCode = SetMaterialCommandStatus(materialWarehouseCommand.F34_CommandNo,
                            materialWarehouseCommand.F34_CmdSeqNo, 7,
                            "0000", 0, true);
                        // nRetCode = C1_AMakeHistory(materialWhcmd->id, materialWhcmd->ser, &nErrCode);
                        break;
                    case 2: // storage 2 times
                        nRetCode = SetMaterialCommandStatus(materialWarehouseCommand.F34_CommandNo,
                            materialWarehouseCommand.F34_CmdSeqNo, 8,
                            "0000", 0, true);
                        break;
                    case 3: // empty retrieve
                        nRetCode = SetMaterialCommandStatus(materialWarehouseCommand.F34_CommandNo,
                            materialWarehouseCommand.F34_CmdSeqNo, 9,
                            "0000", 0, true);
                        // modified by Crystal WU on 96/11/01 set status to prohibit
                        //nShelfSts = 0;
                        nShelfSts = 6;
                        // end of modified
                        if ((materialWarehouseCommand.F34_CommandNo == "2000" ||
                             materialWarehouseCommand.F34_CommandNo == "2001" ||
                             materialWarehouseCommand.F34_CommandNo == "7000") &&
                            iStorageRetrieveType == 0)
                            nStkFlg = 0;
                        else
                            nStkFlg = -1;
                        break;
                    default:
                        break;
                }
                if (nRetCode <= 0)
                {
                    message = string.Format("Set Material Command Status Failed,Message ID: {0}, SEQ: {1}",
                        materialWarehouseCommand.F34_CommandNo, materialWarehouseCommand.F34_CmdSeqNo);
                    Log.Error(message);
                    // Display message to UI.
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                }

                if (nConveyorErr)
                {
                    // Set the conveyor status to error
                    nRetCode = SetConveyorStatus(lpConveyor, 9);
                }
                else
                {
                    // restore conveyor number and status
                    // TODO: Comment this at 2017-07-17
                    //if (materialWarehouseCommand.F34_CommandNo != "1001")
                    nRetCode = SetConveyorStatus(lpConveyor, 0);
                }
                if (nRetCode <= 0)
                {
                    message = string.Format("Recover Conveyor status failed. {0}", lpConveyor);
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    Log.Error(message);
                }
                // update shelf status of material
                if (nShelfSts >= 0)
                {
                    nRetCode = SetMaterialShelfStatus(lpLocation, nShelfSts, -1, nHowCancel);
                    if (nRetCode <= 0)
                    {
                        message = string.Format("Set Shelf Status failed,LOCATION: {0}", lpLocation);
                        Log.Error(message);
                        // Display message to UI.
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                }

                // update material stock flag
                if (nStkFlg >= 0)
                {
                    nRetCode = SetStockFlag(materialWarehouseCommand.F34_PalletNo, nStkFlg);

                    if (nRetCode <= 0)
                    {
                        message = string.Format("Set Stock Flag failed,PALLET: {0}",
                            materialWarehouseCommand.F34_PalletNo);
                        Log.Error(message);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                }

                unitOfWork.Commit();
                return true;
            }
        }

        /// <summary>
        ///     Update material shelf status by using specific conditions.
        /// </summary>
        /// <param name="lpLocation"></param>
        /// <param name="nStatus"></param>
        /// <param name="nTanaorosiFlg"></param>
        /// <param name="nReason"></param>
        /// <returns></returns>
        private int SetMaterialShelfStatus(string lpLocation, int nStatus, int nTanaorosiFlg,
            int nReason = -1)
        {
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var nRetCode = 1;
                if (nStatus < 0) return 0;
                var lstStockTakingPre =
                    unitOfWork.MaterialShelfStatusRepository.GetAll()
                        .ToList()
                        .Where(i => i.F31_ShelfRow + i.F31_ShelfBay + i.F31_ShelfLevel == lpLocation);

                if (nStatus == 0 || nStatus == 6 || nStatus == 7)
                {
                    foreach (var stockTakingPre in lstStockTakingPre)
                    {
                        stockTakingPre.F31_ShelfStatus = nStatus.ToString();
                        stockTakingPre.F31_UpdateDate = DateTime.Now;
                        //if (nReason != 3)
                        //    stockTakingPre.F31_PalletNo = string.Empty;
                        unitOfWork.MaterialShelfStatusRepository.Update(stockTakingPre);
                    }

                    if (!lstStockTakingPre.Any())
                    {
                        var message = "Error occured when set shelf status";
                        // Display message to UI.
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                        nRetCode = -1;
                    }
                }
                else
                {
                    foreach (var stockTakingPre in lstStockTakingPre)
                    {
                        stockTakingPre.F31_ShelfStatus = nStatus.ToString();
                        stockTakingPre.F31_UpdateDate = DateTime.Now;
                        if (nTanaorosiFlg >= 0)
                            stockTakingPre.F31_StockTakingFlag = nTanaorosiFlg.ToString();
                    }

                    if (!lstStockTakingPre.Any())
                    {
                        var message = "Error occured when set shelf status";
                        // Display message to UI.
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                        nRetCode = -1;
                    }
                }

                unitOfWork.Commit();
                return nRetCode;
            }
        }

        /// <summary>
        ///     Update material stock flag by using specific conditions.
        /// </summary>
        /// <param name="lpPallet"></param>
        /// <param name="nFlag"></param>
        /// <returns></returns>
        public int SetStockFlag(string lpPallet, int nFlag)
        {
            if (nFlag < 0) return 0;
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var lstMtrShfStk =
                    unitOfWork.MaterialShelfStockRepository.GetMany(i => i.F33_PalletNo.Trim().Equals(lpPallet.Trim()));

                if (!lstMtrShfStk.Any())
                {
                    var message = "Error occured when set stock flag";
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    return -1;
                }

                foreach (var tx33MtrShfStk in lstMtrShfStk)
                {
                    tx33MtrShfStk.F33_StockFlag = nFlag.ToString();
                    tx33MtrShfStk.F33_UpdateDate = DateTime.Now;
                }
                
                unitOfWork.Commit();
                return 1;
            }
        }

        /// <summary>
        ///     End material warehouse command.
        /// </summary>
        /// <param name="commandNo"></param>
        /// <param name="cmdSeqNo"></param>
        /// <returns></returns>
        private bool EndMaterialWarehouseCommand(string commandNo, string cmdSeqNo)
        {
            var nShelfSts = 0;
            var nStkFlg = 0;
            var nShelfTime = -1;
            var nStgRtrCls = -1;
            var nTanaorosiFlg = -1;
            var lpLocation = "";

            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var materialWarehouseCommand =
                    unitOfWork.MaterialWarehouseCommandRepository.GetMany(
                            i =>
                                i.F34_CommandNo.Trim().Equals(commandNo.Trim()) &&
                                i.F34_CmdSeqNo.Trim().Equals(cmdSeqNo.Trim()))
                        .FirstOrDefault();

                if (materialWarehouseCommand == null)
                    return false;

                var iMaterialStatus = 0;
                int.TryParse(materialWarehouseCommand.F34_Status, out iMaterialStatus);

                if (iMaterialStatus > 5)
                    return false;

                string message;
                int iMaterialCommandNo;
                int iMaterialStorageRetrieveType;

                int.TryParse(materialWarehouseCommand.F34_CommandNo, out iMaterialCommandNo);
                int.TryParse(materialWarehouseCommand.F34_StrRtrType, out iMaterialStorageRetrieveType);

                string lpConveyor;
                switch ((MaterialCommand)iMaterialCommandNo)
                {
                    case MaterialCommand.Storage: // storage    command
                    case MaterialCommand.TwoTimesIn:
                        // re-storage command 
                        lpConveyor = materialWarehouseCommand.F34_From;
                        lpLocation = materialWarehouseCommand.F34_To;
                        nTanaorosiFlg = 0;

                        switch ((MaterialStorageRetrieveType)iMaterialStorageRetrieveType)
                        {
                            case MaterialStorageRetrieveType.Material: // material
                                nShelfSts = 3;
                                nStkFlg = 3;
                                nShelfTime = 0; // set storage time
                                nStgRtrCls = 0; // storage

                                message = string.Format("SUCCESS: {0} Storage. From: {1} To: {2} TermNo: {3}",
                                    "Material",
                                    materialWarehouseCommand.F34_From, materialWarehouseCommand.F34_To,
                                    materialWarehouseCommand.F34_TerminalNo);
                                Log.Debug(message);
                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);
                                break;
                            case MaterialStorageRetrieveType.WarehousePallet: // empty pallet
                                nShelfSts = 1;
                                nStkFlg = -1;

                                message = string.Format("SUCCESS: {0} Storage. From: {1} To: {2} TermNo: {3}",
                                    "Empty Pallet", materialWarehouseCommand.F34_From, materialWarehouseCommand.F34_To,
                                    materialWarehouseCommand.F34_TerminalNo);
                                Log.Debug(message);
                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                                break;
                            case MaterialStorageRetrieveType.SupplierPallet: // provider's pallet

                                message = string.Format("SUCCESS: {0} Storage. From: {1} To: {2} TermNo: {3}",
                                    "Provider's Pallet", materialWarehouseCommand.F34_From,
                                    materialWarehouseCommand.F34_To,
                                    materialWarehouseCommand.F34_TerminalNo);
                                Log.Debug(message);
                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);
                                nShelfSts = 2;
                                nStkFlg = -1;
                                break;
                            case MaterialStorageRetrieveType.SupplierPalletSideIn: // retrieve from supplier pallet
                                break;
                            case MaterialStorageRetrieveType.MaterialReStorage: // re-storage
                                message = string.Format("SUCCESS: {0}Storage. From: {1} To: {2} TermNo: {3}",
                                    "Material Re-",
                                    materialWarehouseCommand.F34_From, materialWarehouseCommand.F34_To,
                                    materialWarehouseCommand.F34_TerminalNo);
                                Log.Debug(message);
                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                                nShelfSts = 3;
                                nStkFlg = 3;
                                //set re-storage time and recover storage time
                                nShelfTime = 1;
                                nStgRtrCls = 0; // storage
                                break;
                            default:
                                return false;
                        }
                        break;

                    case MaterialCommand.Retrieval: // retrieve command
                    case MaterialCommand.ReRetrieve: // re-retrieve command (impossible)
                        lpConveyor = materialWarehouseCommand.F34_To;
                        lpLocation = materialWarehouseCommand.F34_From;

                        switch ((MaterialStorageRetrieveType)iMaterialStorageRetrieveType)
                        {
                            case MaterialStorageRetrieveType.Material:
                                // material
                                message = string.Format("SUCCESS: {0} Retrieve. From: {1} To: {2} TermNo: {3}",
                                    "Material",
                                    materialWarehouseCommand.F34_From, materialWarehouseCommand.F34_To,
                                    materialWarehouseCommand.F34_TerminalNo);
                                Log.Debug(message);
                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                                nShelfSts = 0;
                                nStkFlg = 0;
                                nShelfTime = 2; // set retrieval time
                                nStgRtrCls = 1; // retrieval
                                break;
                            case MaterialStorageRetrieveType.WarehousePallet: // empty pallet
                            case MaterialStorageRetrieveType.SupplierPallet: // provider's pallet		  (front)

                                message = string.Format("SUCCESS: {0} Storage. From: {1} To: {2} TermNo: {3}", "Pallet",
                                    materialWarehouseCommand.F34_From, materialWarehouseCommand.F34_To,
                                    materialWarehouseCommand.F34_TerminalNo);
                                Log.Debug(message);
                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                                nShelfSts = 0;
                                nStkFlg = -1;
                                break;
                            case MaterialStorageRetrieveType.SupplierPalletSideIn: // provider's pallet retrieve (side)

                                message = string.Format("SUCCESS: {0} Retrieve. From: {1} To: {2} TermNo: {3}", "Pallet",
                                    materialWarehouseCommand.F34_From, materialWarehouseCommand.F34_To,
                                    materialWarehouseCommand.F34_TerminalNo);
                                Log.Debug(message);
                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                                nShelfSts = -1;
                                nStkFlg = -1;
                                break;
                            default:
                                return false;
                        }
                        break;

                    case MaterialCommand.Move: // Moving between conveyers

                        message = string.Format("SUCCESS: Material Move From: {0} To: {1} TermNo: {2}",
                            materialWarehouseCommand.F34_From,
                            materialWarehouseCommand.F34_To, materialWarehouseCommand.F34_TerminalNo);
                        Log.Debug(message);
                        Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                        nShelfSts = -1;
                        nStkFlg = -1;
                        lpConveyor = materialWarehouseCommand.F34_From;
                        SetConveyorStatus(lpConveyor, 0);
                        lpConveyor = materialWarehouseCommand.F34_To;
                        break;


                    case MaterialCommand.StockTakingIn:

                        message = string.Format("SUCCESS: TANAONOSI Material Storage. From: {0} To: {1} TermNo: {2}",
                            materialWarehouseCommand.F34_From, materialWarehouseCommand.F34_To,
                            materialWarehouseCommand.F34_TerminalNo);
                        Log.Debug(message);
                        Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                        lpConveyor = materialWarehouseCommand.F34_From;
                        lpLocation = materialWarehouseCommand.F34_To;
                        nShelfSts = 3;
                        nStkFlg = 3;
                        nTanaorosiFlg = 1;
                        break;

                    case MaterialCommand.StockTakingOff:

                        message = string.Format("SUCCESS: TANAONOSI Material Retrieve. From: {0} To: {1} TermNo: {2}",
                            materialWarehouseCommand.F34_From, materialWarehouseCommand.F34_To,
                            materialWarehouseCommand.F34_TerminalNo);
                        Log.Debug(message);
                        Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                        lpConveyor = materialWarehouseCommand.F34_To;
                        lpLocation = materialWarehouseCommand.F34_From;
                        nShelfSts = -1;
                        nStkFlg = -1;
                        break;
                    default:
                        // not supported in this version
                        return false;
                }
                // manaual end the command.
                // update status to 6
                var nRetCode = SetMaterialCommandStatus(materialWarehouseCommand.F34_CommandNo,
                    materialWarehouseCommand.F34_CmdSeqNo, 6, "0000", 0, true);
                if (nRetCode <= 0)
                {
                    message = string.Format("Set Material Command Status Failed,Message ID: {0}, SEQ: {1}",
                        materialWarehouseCommand.F34_CommandNo, materialWarehouseCommand.F34_CmdSeqNo);
                    Log.Debug(message);
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                }
                // restore conveyor number and status
                if (materialWarehouseCommand.F34_CommandNo != MaterialCommand.TwoTimesIn.ToString("d"))
                {
                    nRetCode = SetConveyorStatus(lpConveyor, 0);
                    if (nRetCode <= 0)
                    {
                        message = string.Format("Recover Conveyor status failed. {0}", lpConveyor);
                        Log.Debug(message);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                }
                // update shelf status of material
                if (nShelfSts >= 0)
                {
                    nRetCode = SetMaterialShelfStatus(lpLocation, nShelfSts, nTanaorosiFlg);
                    if (nRetCode <= 0)
                    {
                        message = string.Format("Set Shelf Status failed,LOCATION: {0}", lpLocation);
                        Log.Debug(message);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                }

                // update material stock flag
                if (nStkFlg >= 0)
                {
                    nRetCode = SetStockFlag(materialWarehouseCommand.F34_PalletNo, nStkFlg);
                    if (nRetCode <= 0)
                    {
                        message = string.Format("Set Stock Flag failed,PALLET: {0}",
                            materialWarehouseCommand.F34_PalletNo);
                        Log.Debug(message);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                }

                if (nShelfTime >= 0)
                {
                    nRetCode = SetMaterialShelfTime(materialWarehouseCommand.F34_PalletNo, nShelfTime,
                        lpLocation);
                    if (nRetCode <= 0)
                    {
                        message = string.Format("Set Shelf Time failed,PALLET: {0} LOCATION: {1}",
                            materialWarehouseCommand.F34_PalletNo,
                            lpLocation);
                        Log.Debug(message);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                }

                if (nStgRtrCls >= 0)
                {
                    nRetCode = SetMaterialStorageRetrieveHistory(materialWarehouseCommand.F34_PalletNo,
                        materialWarehouseCommand.F34_TerminalNo,
                        materialWarehouseCommand.F34_From,
                        materialWarehouseCommand.F34_To,
                        nStgRtrCls,
                        nShelfTime);

                    if (nRetCode >= 0) return true;
                    message = string.Format("Set Storage-Retrieve History failed.PALLET: {0}",
                        materialWarehouseCommand.F34_PalletNo);
                    Log.Debug(message);
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                }

                unitOfWork.Commit();
                return true;
            }
        }

        /// <summary>
        ///     Set storage retrieve history.
        /// </summary>
        /// <param name="lpPallet"></param>
        /// <param name="lpTermNo"></param>
        /// <param name="lpFrom"></param>
        /// <param name="lpTo"></param>
        /// <param name="nStgRtrCls"></param>
        /// <param name="nShelfTime"></param>
        /// <returns></returns>
        private int SetMaterialStorageRetrieveHistory(string lpPallet, string lpTermNo,
            string lpFrom, string lpTo,
            int nStgRtrCls,
            int nShelfTime)
        {
            // Set Material Storage-Retrieval Result
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var tX32MtrShf =
                    unitOfWork.MaterialShelfRepository.GetMany(i => i.F32_PalletNo.Trim().Equals(lpPallet.Trim()))
                        .FirstOrDefault();
                if (tX32MtrShf == null)
                    return -1;
                DateTime stgRtrDate;
                var prcOrdNo = !string.IsNullOrEmpty(tX32MtrShf.F32_PrcOrdNo) ? tX32MtrShf.F32_PrcOrdNo : "";
                var prtDvrNo = !string.IsNullOrEmpty(prcOrdNo) ? tX32MtrShf.F32_PrtDvrNo : "";
                switch (nStgRtrCls)
                {
                    case 0:
                        if (!tX32MtrShf.F32_StorageDate.HasValue)
                            return -1;

                        stgRtrDate = nShelfTime == 1
                            ? tX32MtrShf.F32_ReStorageDate.Value
                            : tX32MtrShf.F32_StorageDate.Value;

                        break;
                    case 1:
                        if (!tX32MtrShf.F32_ReStorageDate.HasValue)
                            return -1;
                        stgRtrDate = tX32MtrShf.F32_ReStorageDate.Value;
                        break;
                    default:
                        return -1;
                }

                IQueryable<TX33_MtrShfStk> lstMtrShfStk =
                    unitOfWork.MaterialShelfStockRepository.GetMany(i => i.F33_PalletNo.Trim().Equals(lpPallet.Trim()))
                        .OrderBy(i => i.F33_AddDate);
                if (!lstMtrShfStk.Any())
                {
                    var message = "Error occured when set history of Storage - Retrieve";
                    Log.Debug(message);
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    return -1;
                }
                lstMtrShfStk = lstMtrShfStk.Count() > 5 ? lstMtrShfStk.Skip(1).Take(5) : lstMtrShfStk;
                try
                {
                    foreach (var tx33MtrShfStk in lstMtrShfStk)
                    {
                        var mtrStgRtrHst = new TH61_MtrStgRtrHst();
                        mtrStgRtrHst.F61_MaterialCode = tx33MtrShfStk.F33_MaterialCode;
                        mtrStgRtrHst.F61_MaterialLotNo = tx33MtrShfStk.F33_MaterialLotNo;
                        mtrStgRtrHst.F61_PalletNo = tx33MtrShfStk.F33_PalletNo;
                        mtrStgRtrHst.F61_StgRtrDate = stgRtrDate;

                        prcOrdNo = prcOrdNo.Trim();
                        if (prcOrdNo.Length > 7)
                            prcOrdNo = prcOrdNo.Substring(0, 7);
                        mtrStgRtrHst.F61_PrcPdtNo = prcOrdNo;
                        mtrStgRtrHst.F61_PrtDvrNo = prtDvrNo;
                        mtrStgRtrHst.F61_StgRtrCls = nStgRtrCls.ToString();
                        mtrStgRtrHst.F61_To = lpTo;
                        mtrStgRtrHst.F61_From = lpFrom;
                        mtrStgRtrHst.F61_TerminalNo = lpTermNo;
                        mtrStgRtrHst.F61_Amount = tx33MtrShfStk.F33_Amount;
                        mtrStgRtrHst.F61_AddDate = DateTime.Now;
                        mtrStgRtrHst.F61_UpdateDate = DateTime.Now;
                        unitOfWork.MaterialStorageRetrieveHistoryRepository.Add(mtrStgRtrHst);
                    }

                    unitOfWork.Commit();
                }
                catch (Exception exception)
                {
                    Log.Error(exception.Message, exception);
                    Message.InitiateMessage(DateTime.Now, MessageType.Error,
                        "Error occured when set history of Storage - Retrieve");
                    return -1;
                }
                return 1;
            }
        }

        /// <summary>
        ///     Set material shelf time.
        /// </summary>
        /// <param name="lpPallet"></param>
        /// <param name="nShelfTime"></param>
        /// <param name="lpLocation"></param>
        /// <returns></returns>
        private int SetMaterialShelfTime(string lpPallet, int nShelfTime, string lpLocation)
        {
            try
            {
                using (var context = new KCSGDbContext())
                using (var unitOfWork = new UnitOfWork(context))
                {
                    var tx32MtrShf =
                        unitOfWork.MaterialShelfRepository.GetMany(i => i.F32_PalletNo.Trim().Equals(lpPallet.Trim()))
                            .FirstOrDefault();

                    if (tx32MtrShf != null)
                    {
                        var lstMtrshfSts =
                            unitOfWork.MaterialShelfStatusRepository.GetAll()
                                .ToList()
                                .Where(i => i.F31_ShelfRow + i.F31_ShelfBay + i.F31_ShelfLevel == lpLocation);
                        foreach (var tx31MtrShfStk in lstMtrshfSts)
                        {
                            switch (nShelfTime)
                            {
                                case 0:
                                    tx31MtrShfStk.F31_StorageDate = tx32MtrShf.F32_StorageDate;
                                    break;
                                case 1:
                                    tx31MtrShfStk.F31_StorageDate = tx32MtrShf.F32_StorageDate;

                                    break;
                                case 2:

                                    tx31MtrShfStk.F31_RetrievalDate = tx32MtrShf.F32_RetrievalDate;

                                    break;
                                default:
                                    return -1;
                            }
                            unitOfWork.MaterialShelfStatusRepository.Update(tx31MtrShfStk);
                        }
                        switch (nShelfTime)
                        {
                            case 0:
                                tx32MtrShf.F32_StorageDate = DateTime.Now;
                                break;
                            case 1:
                                tx32MtrShf.F32_ReStorageDate = DateTime.Now;
                                break;
                            case 2:
                                tx32MtrShf.F32_RetrievalDate = DateTime.Now;
                                break;
                        }

                        unitOfWork.MaterialShelfRepository.Update(tx32MtrShf);
                    }
                    else
                    {
                        var message = "Error occured when set shelf time";
                        Log.Debug(message);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                        return -1;
                    }

                    unitOfWork.Commit();
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception.Message, exception);
                Message.InitiateMessage(DateTime.Now, MessageType.Error, "Error occured when set shelf time");
                return -1;
            }
            return 1;
        }

        /// <summary>
        ///     Find material warehouse and turn it into error status.
        /// </summary>
        /// <param name="deviceCode"></param>
        private void SetMaterialAutowarehouseFailure(string deviceCode)
        {
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var tm14 = unitOfWork.DeviceRepository.GetByDeviceCode(deviceCode);
                tm14.F14_DeviceStatus = "2";
                tm14.F14_UpdateDate = DateTime.Now;
                unitOfWork.DeviceRepository.Update(tm14);
                Message.InitiateMessage(DateTime.Now, MessageType.Error, "Autoware House is at error status..");
                unitOfWork.Commit();
            }
        }

        /// <summary>
        ///     Update conveyor status by using specific condition.
        /// </summary>
        /// <param name="conveyorCode"></param>
        /// <param name="nStatus"></param>
        /// <returns></returns>
        public int SetConveyorStatus(string conveyorCode, int nStatus)
        {
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var nRecordCount = 1;
                if (nStatus < 0) return 0;
                // restore conveyor number and status
                if (nStatus == 1 || nStatus == 2)
                {
                    // Storage or Retrieve send OK
                    var conveyor =
                        unitOfWork.ConveyorRepository.GetMany(i =>
                                i.F05_ConveyorCode.Trim().Equals(conveyorCode.Trim()) &&
                                i.F05_BufferUsing < i.F05_MaxBuffer).ToList()
                            .FirstOrDefault(
                                i =>
                                    Convert.ToInt16(i.F05_StrRtrSts) >= 0 &&
                                    Convert.ToInt16(i.F05_StrRtrSts) <= nStatus);

                    if (conveyor == null)
                    {
                        Log.Debug($"Cannot find conveyor {conveyorCode} - {nStatus} (1)");
                        return -1;
                    }
                    conveyor.F05_StrRtrSts = nStatus.ToString();

                    // Removed at Jul 25 2017: conveyor.F05_BufferUsing = conveyor.F05_BufferUsing + 1;
                    conveyor.F05_BufferUsing = 1;
                    conveyor.F05_UpdateDate = DateTime.Now;
                    unitOfWork.ConveyorRepository.Update(conveyor);

                    unitOfWork.Commit();
                }
                else if (nStatus == 3)
                {
                    // Floor Movement send OK
                    var conveyor =
                        unitOfWork.ConveyorRepository.GetMany(
                            i =>
                                i.F05_ConveyorCode.Trim().Equals(conveyorCode.Trim()) &&
                                i.F05_BufferUsing < i.F05_MaxBuffer &&
                                i.F05_StrRtrSts == "0").FirstOrDefault();

                    if (conveyor != null)
                    {
                        // Removed at Jul 25 2017: conveyor.F05_BufferUsing = conveyor.F05_BufferUsing + 1;
                        conveyor.F05_BufferUsing = 1;
                        conveyor.F05_UpdateDate = DateTime.Now;
                        conveyor.F05_StrRtrSts = nStatus.ToString();
                        unitOfWork.ConveyorRepository.Update(conveyor);

                        unitOfWork.Commit();
                    }
                }
                else if (nStatus == 0)
                {
                    // Command End or Command Cancel
                    try
                    {
                        var conveyor =
                            unitOfWork.ConveyorRepository.GetMany(
                                    i => i.F05_ConveyorCode.Trim().Equals(conveyorCode) && i.F05_BufferUsing > 0)
                                .FirstOrDefault();

                        if (conveyor != null)
                        {
                            // Removed at Jul 25 2017: conveyor.F05_BufferUsing = conveyor.F05_BufferUsing - 1;
                            conveyor.F05_BufferUsing = 0;
                            conveyor.F05_UpdateDate = DateTime.Now;
                            if (conveyor.F05_BufferUsing == 0)
                                conveyor.F05_StrRtrSts = "0";

                            unitOfWork.Commit();
                        }
                        else
                        {
                            Log.Debug($"Cannot find conveyor {conveyorCode} - {nStatus} (2)");
                            nRecordCount = -1;
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception.Message, exception);
                        nRecordCount = -1;
                    }
                }
                else
                {
                    try
                    {
                        // If status == 9 then this conveyor is in error status
                        var conveyor =
                            unitOfWork.ConveyorRepository.GetMany(
                                i => i.F05_ConveyorCode.Trim().Equals(conveyorCode)).FirstOrDefault();
                        if (conveyor != null)
                        {
                            conveyor.F05_StrRtrSts = "9";
                            conveyor.F05_UpdateDate = DateTime.Now;

                            unitOfWork.Commit();
                        }
                        else
                        {
                            Log.Debug($"Cannot find conveyor {conveyorCode} - {nStatus} (3)");
                            nRecordCount = -1;
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error(exception.Message, exception);
                        nRecordCount = -1;
                    }
                }


                if (nRecordCount == -1)
                {
                    var message = "Error occured when set Conveyor Status";
                    Log.Debug(message);
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    return -1;
                }

                if (nStatus == 0)
                {
                    Log.Debug($"Record count = {nRecordCount}");
                    return nRecordCount;
                }
                return 1;
            }
        }

        private bool C1_CheckRetryEnd(int? iRetryCount)
        {
            return iRetryCount.HasValue && iRetryCount.Value >= 3;
        }

        /// <summary>
        ///     Build a terminal message and send to external terminal.
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="materialWarehouseCommand"></param>
        /// <returns></returns>
        private int SendCommandToExternalTerminal(ConnectionSetting endPoint, TX34_MtrWhsCmd materialWarehouseCommand)
        {
            #region Command refactor

            // added by Crystal Wu on 96/11/08 
            if ("7000".Equals(materialWarehouseCommand.F34_CommandNo))
                materialWarehouseCommand.F34_CommandNo = "2000";
            if ("6000".Equals(materialWarehouseCommand.F34_CommandNo))
                materialWarehouseCommand.F34_CommandNo = "1000";

            #endregion

            #region Command validity

            // Find terminal message in queue.
            var terminalMessage = FindCommandInQueue(materialWarehouseCommand.F34_CmdSeqNo,
                materialWarehouseCommand.F34_CommandNo);

            if (terminalMessage != null && !IsSendable(terminalMessage))
                return -1;

            if (terminalMessage == null)
            {
                terminalMessage = new TerminalMessage(materialWarehouseCommand);
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
                    Message.InitiateMessage(DateTime.Now, MessageType.Information, "Sent", terminalMessage.ToString());
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

        #region Properties

        #region Inversion of controls

        /// <summary>
        ///     Instance handles application converter business.
        /// </summary>
        private ConverterViewModel _converter;

        /// <summary>
        ///     Instance handles application converter business.
        /// </summary>
        public ConverterViewModel Converter
        {
            get { return _converter ?? (_converter = SimpleIoc.Default.GetInstance<ConverterViewModel>()); }
            set { _converter = value; }
        }

        #endregion

        #endregion
    }
}