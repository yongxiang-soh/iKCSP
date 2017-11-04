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
    public class ProductAutoWarehouseController : WarehouseParentController, IWarehouseController
    {
        #region Properties

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

        #region Methods

        /// <summary>
        ///     ProceedIncommingCommand analyzation on messages received from external terminal.
        /// </summary>
        /// <param name="terminalMessage"></param>
        /// <param name="deviceCode"></param>
        public void ProceedIncommingCommand(TerminalMessage terminalMessage, string deviceCode)
        {
            // Status request response should be ignored.
            if (MessageCommandIndex.RequestStatus.Equals(terminalMessage.CommandIndex))
                return;

            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {

                var pendingProductCommand = unitOfWork.ProductWarehouseCommandRepository.GetMany(
                        i => i.F47_CmdSeqNo.Trim().Equals(terminalMessage.CommandSequence.Trim())
                        && (i.F47_CommandNo.Trim().Equals(terminalMessage.CommandIndex.Trim()) || i.F47_PictureNo.Trim().Equals("TCPR061F") || i.F47_PictureNo.Trim().Equals("TCPR062F")))
                    .FirstOrDefault();

                if (pendingProductCommand == null)
                {
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, "Error message comes from AW!(Not in DB)");
                    return;
                }

                var nRetryCount = pendingProductCommand.F47_RetryCount;
                string message;
                var idsDboperationfail = "The database operation failed.";
                var terminalCommand = Converter.TextToInt(terminalMessage.Command, 0);
                var terminalCommandStatus = Converter.TextToInt(terminalMessage.Status, -1);

                switch (terminalCommand)
                {
                    case 0:
                        Message.InitiateMessage(DateTime.Now, MessageType.Error,
                            "Error message comes from AW!(Error Command)");
                        break;
                    case 10: // accepted command 
                        //if (nStatus != '1' &&
                        //    nStatus != '4' &&
                        //    nStatus != '5')
                        //{
                        //    Message.InitiateMessage(DateTime.Now, MessageType.Error, "Error message comes from AW!(Status Error)");
                        //    return;
                        //}

                        switch (terminalCommandStatus)
                        {
                            case 0: // OK
                                //sprintf(szTmpStr, "%04.4d", TC_MID_CmdEnd);
                                //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                // update status to 3(accepted)
                                SetProductCommandStatus(pendingProductCommand.F47_CommandNo, terminalMessage.CommandSequence, 3,
                                    terminalMessage.Status, false);
                                //m_pDlg -> ProcessDataBase();		// process next command
                                break;
                            case 5000: // Retry Unlimitly.
                                // Adde by Wu.Jing
                                Message.InitiateMessage(DateTime.Now, MessageType.Error,
                                    "Uketuke Error<5000> , Retry Unlimitly");
                                SetProductCommandStatus(pendingProductCommand.F47_CommandNo, terminalMessage.CommandSequence, 5,
                                    terminalMessage.Status, false);
                                // end of added
                                //m_pDlg->SetResendTimer();
                                break;
                            case 1: // Set AutoWare House to error status
                            case 2:
                            case 1001:
                            case 1002:
                            case 1003:
                            case 1004:
                                message = string.Format("Uketuke Error{0},Cancel this command", terminalCommandStatus);
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                                if (CancelProductWarehouseCommand(terminalMessage.CommandSequence,
                                    terminalMessage.CommandIndex))
                                    SetProductAutoWarehouseFailure(deviceCode);
                                else
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                break;
                            case 1005:
                                // cancel the command, and set terminalMessage status to offline
                                if (CancelProductWarehouseCommand(terminalMessage.CommandSequence,
                                    terminalMessage.CommandIndex))
                                    SetAutoWarehouseOffline(deviceCode);
                                else
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
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
                                message = $"Uketuke Error{terminalMessage.Status},Cancel this command";
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                if (CancelProductWarehouseCommand(terminalMessage.CommandSequence,
                                    terminalMessage.CommandIndex))
                                {
                                    //sprintf(szTmpStr, "%04.4d", TC_MID_CmdCancel);
                                    //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                    //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                                }
                                else
                                {
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                }
                                //m_pDlg->SetResendTimer();
                                break;
                            case 6: // Retry three times and set the autoware house error.
                            case 15:
                                SetProductCommandStatus(pendingProductCommand.F47_CommandNo, terminalMessage.CommandSequence, 5,
                                    terminalMessage.Status, false, 1);
                                if (nRetryCount >= 3)
                                    if (CancelProductWarehouseCommand(terminalMessage.CommandSequence,
                                        terminalMessage.CommandIndex))
                                    {
                                        var messsage =
                                            string.Format(
                                                "Uketuke Error{0},already retry {1} times, so cancel this command",
                                                terminalCommandStatus, nRetryCount);
                                        Message.InitiateMessage(DateTime.Now, MessageType.Error, messsage);

                                        //sprintf(szTmpStr, "%04.4d", TC_MID_CmdCancel);
                                        //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                        //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                                        SetProductAutoWarehouseFailure(deviceCode);
                                    }
                                    else
                                    {
                                        Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                    }
                                else
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error,
                                        $"Uketuke Error{terminalCommandStatus},already retry {nRetryCount} times");
                                break;
                            case 4: // Retry three times, cancel this command,process next command
                            //		case 5:				deleted by Crystal Wu on 96/11/05
                            case 8:
                            case 10:
                                SetProductCommandStatus(pendingProductCommand.F47_CommandNo, terminalMessage.CommandSequence, 5,
                                    terminalMessage.Status, false, 1);
                                if (nRetryCount >= 3)
                                {
                                    if (CancelProductWarehouseCommand(terminalMessage.CommandSequence,
                                        terminalMessage.CommandIndex))
                                    {
                                        message =
                                            $"Uketuke Error{terminalCommandStatus},already retry {nRetryCount} times, so cancel this command";
                                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                    }
                                }
                                else
                                {
                                    message = $"Uketuke Error{terminalCommandStatus},already retry {nRetryCount} times";
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                }
                                break;
                            default:

                                message = $"unexpected errcode{terminalMessage.Status} from AW client";
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                break;
                        }
                        break;
                    case 100: // command ended
                        //if (nStatus != '3' &&
                        //    nStatus != '4' &&
                        //    nStatus != '5')
                        //{
                        //    Message.InitiateMessage(DateTime.Now, MessageType.Error, "Error message comes from AW!(Status Error)");
                        //    return;
                        //}

                        switch (terminalCommandStatus)
                        {
                            case 0: // OK
                                if (EndProductWarehouseCommand(pendingProductCommand.F47_CommandNo,
                                    pendingProductCommand.F47_CmdSeqNo))
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

                                if (CancelProductWarehouseCommand(terminalMessage.CommandSequence,
                                    terminalMessage.CommandIndex, 2))
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

                                if (CancelProductWarehouseCommand(terminalMessage.CommandSequence,
                                    terminalMessage.CommandIndex, 3))
                                    SetProductAutoWarehouseFailure(deviceCode);
                                else
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                break;

                            default:
                                message = string.Format("Retrieve error ! errcode{0}", terminalMessage.Status);
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                if (CancelProductWarehouseCommand(terminalMessage.CommandSequence,
                                    terminalMessage.CommandIndex))
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
                        //if (nStatus != '3' &&
                        //    nStatus != '4' &&
                        //    nStatus != '5')
                        //{
                        //    Message.InitiateMessage(DateTime.Now, MessageType.Error, "Error message comes from AW!(Status Error)");
                        //    return;
                        //}
                        // end of added

                        switch (terminalCommandStatus)
                        {
                            case 64: // empty retrieve
                                Message.InitiateMessage(DateTime.Now, MessageType.Error,
                                    "Retrieve error:empty retrieve.");

                                if (CancelProductWarehouseCommand(terminalMessage.CommandSequence,
                                    terminalMessage.CommandIndex, 3))
                                    SetProductAutoWarehouseFailure(deviceCode);
                                else
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, idsDboperationfail);
                                break;
                            default:
                                if (CancelProductWarehouseCommand(terminalMessage.CommandSequence,
                                    terminalMessage.CommandIndex))
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
                }
            }
        }

        /// <summary>
        ///     ProceedIncommingCommand data list.
        /// </summary>
        /// <param name="productWarehouseCommands"></param>
        /// <param name="deviceCode"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public int ProcessDataList(IList<TX47_PdtWhsCmd> productWarehouseCommands, string deviceCode,
            ConnectionSetting endPoint)
        {
            foreach (var productWarehouseCommand in productWarehouseCommands)
            {
                var szProductSequence = productWarehouseCommand.F47_CmdSeqNo;
                var szProductCommandNo = productWarehouseCommand.F47_CommandNo;
                var iStrRtrType = Converter.TextToInt(productWarehouseCommand.F47_StrRtrType, -1);
                var szProductCommandStatus = productWarehouseCommand.F47_Status;
                var szConveyor = "";

                var nFlag = -1;
                var nOutSideFlag = -1;

                int iReturnCode;
                switch (szProductCommandStatus[0])
                {
                    case '0':
                        int iStrRtsStatus;
                        string szMessage;
                        if ("1000".Equals(szProductCommandNo) || "1001".Equals(szProductCommandNo))
                        {
                            switch (iStrRtrType)
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
                            iStrRtsStatus = 1;
                            szConveyor = productWarehouseCommand.F47_From;
                            // added by Crystal Wu for 2 times storage need not to check conveyor status
                            if ("1001".Equals(szProductCommandNo))
                                iStrRtsStatus = -1;
                            // end of added
                        }
                        else if ("2000".Equals(szProductCommandNo) || "2001".Equals(szProductCommandNo))
                        {
                            iStrRtsStatus = 2;
                            szConveyor = productWarehouseCommand.F47_To;
                            switch (iStrRtrType)
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
                        else if ("3000".Equals(szProductCommandNo))
                        {
                            // move between floors
                            iStrRtsStatus = 3;
                            nFlag = -1; // not need to update shelf status
                            szConveyor = productWarehouseCommand.F47_From;
                            iReturnCode = SetConveyorStatus(szConveyor, iStrRtsStatus);
                            if (iReturnCode <= 0)
                            {
                                szMessage = $"Conveyor <{szConveyor}> status can not permit to send message";
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, szMessage);

                                break;
                            }

                            szConveyor = productWarehouseCommand.F47_To;
                        }
                        else if ("6000".Equals(szProductCommandNo))
                        {
                            iStrRtsStatus = 1;
                            nFlag = -1;
                            szConveyor = productWarehouseCommand.F47_From;
                        }
                        else if ("7000".Equals(szProductCommandNo))
                        {
                            iStrRtsStatus = 2;
                            nFlag = -1;
                            szConveyor = productWarehouseCommand.F47_To;
                        }
                        else
                        {
                            // not supported there
                            iStrRtsStatus = -1; // 3 should be change to other value
                            nFlag = -1;
                        }
                        if (iStrRtsStatus >= 0)
                        {
                            iReturnCode = SetConveyorStatus(szConveyor, iStrRtsStatus);
                            if (iReturnCode <= 0)
                            {
                                szMessage = string.Format("Conveyor <{0}> status can not permit to send message",
                                    szConveyor);
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, szMessage);

                                if ("3000".Equals(szProductCommandNo))
                                {
                                    szConveyor = productWarehouseCommand.F47_From;
                                    iReturnCode = SetConveyorStatus(szConveyor, 0);
                                    if (iReturnCode <= 0)
                                    {
                                        szMessage = "Recover conveyor status failed";
                                        Message.InitiateMessage(DateTime.Now, MessageType.Error, szMessage);
                                    }
                                }
                                break;
                            }
                        }

                        iReturnCode = SendCommandToExternalTerminal(endPoint, productWarehouseCommand);

                        if (iReturnCode > 0)
                        {
                            // send the message OK
                            SetProductCommandStatus(szProductCommandNo, szProductSequence, 1, "0000", true, 1);
                        }
                        else
                        {
                            // send the message NG
                            SetProductCommandStatus(szProductCommandNo, szProductSequence, 2, "0000", true, 1);
                            if (productWarehouseCommand.F47_RetryCount >= 3)
                            {
                                // AwOffLine();
                            }
                            if (iStrRtsStatus >= 0)
                            {
                                iReturnCode = SetConveyorStatus(szConveyor, 0);
                                if (iReturnCode <= 0)
                                {
                                    szMessage = "Recover conveyor status failed";
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, szMessage);
                                }
                            }
                            if ("3000".Equals(szProductCommandNo))
                            {
                                iReturnCode = SetConveyorStatus(szConveyor, 0);
                                if (iReturnCode <= 0)
                                {
                                    szMessage = "Recover conveyor status failed";
                                    Message.InitiateMessage(DateTime.Now, MessageType.Error, szMessage);
                                }
                            }
                        }

                        // update material shelf status
                        if (nFlag >= 0)
                            SetStockFlag(productWarehouseCommand.F47_PalletNo, nFlag);
                        if (nOutSideFlag >= 0)
                            SetOsdProductStockFlag(productWarehouseCommand.F47_PalletNo, nFlag);
                        return 0;
                    case '1': // send OK but no accept message
                    case '2': // send NG
                        if (productWarehouseCommand.F47_RetryCount >= 3)
                        {
                            // 3 times
                            if (szProductCommandStatus[0] == '1')
                                SetProductAutoWarehouseFailure(deviceCode);

                            SetProductCommandStatus(szProductCommandNo, szProductSequence, 1, "0000", true, 1);
                        }
                        else
                        {
                            iReturnCode = SendCommandToExternalTerminal(endPoint, productWarehouseCommand);

                            if (iReturnCode > 0)
                            {
                                // AW message send success
                                SetProductCommandStatus(szProductCommandNo, szProductSequence, 1, "0000", true, 1);
                            }
                            else
                            {
                                // AW message send failure.
                                SetProductCommandStatus(szProductCommandNo, szProductSequence, 2, "0000", true, 1);
                                if (productWarehouseCommand.F47_RetryCount >= 3)
                                {
                                    //						AwOffLine();
                                }
                            }
                        }
                        return 0;
                    case '3':
                        // do nothing
                        break;
                    case '4':

                        iReturnCode = SendCommandToExternalTerminal(endPoint, productWarehouseCommand);

                        if (iReturnCode > 0)
                        {
                            SetProductCommandStatus(szProductCommandNo, szProductSequence, 4, "0000", true, 1);
                            //			m_nTidAccept = SetTimer(TID_ACCEPT, m_nAcceptTime, NULL);
                            return 0;
                        }
                        SetProductCommandStatus(szProductCommandNo, szProductSequence, 2, "0000", true, 1);
                        if (productWarehouseCommand.F47_RetryCount >= 3)
                        {
                            //					AwOffLine();
                        }
                        break;
                    case '5':

                        iReturnCode = SendCommandToExternalTerminal(endPoint, productWarehouseCommand);

                        if (iReturnCode > 0)
                        {
                            SetProductCommandStatus(szProductCommandNo, szProductSequence, 5, "0000", true, 1);
                            //		m_nTidAccept = SetTimer(TID_ACCEPT, m_nAcceptTime, NULL);
                            return 0;
                        }
                        SetProductCommandStatus(szProductCommandNo, szProductSequence, 2, "0000", true, 1);
                        if (productWarehouseCommand.F47_RetryCount >= 3)
                        {
                            //					AwOffLine();
                        }
                        break;
                }
            }


            return 0;
        }

        /// <summary>
        ///     Terminate product warehouse command.
        /// </summary>
        /// <param name="commandNo"></param>
        /// <param name="cmdSeqNo"></param>
        /// <returns></returns>
        private bool EndProductWarehouseCommand(string commandNo, string cmdSeqNo)
        {
            var terminalCommandIndex = Converter.TextToInt(commandNo, -1);

            string lpConveyor;
            var lpLocation = "";
            int nShelfSts;
            var nStkFlg = -1;
            var nOsdPpdStkFlg = -1;
            var nTanaorosiFlg = -1;
            var nShelfTime = -1;
            var nStgRtrCls = -1; // for Set StgRtrHst
            int nRetCode;

            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var productCommand = unitOfWork.ProductWarehouseCommandRepository.GetByCommondNoAndSeqNo(cmdSeqNo,
                    commandNo);
                var iProductCommandStatus = Converter.TextToInt(productCommand.F47_Status, 1000);
                var iProductStorageRetrieveIndex = Converter.TextToInt(productCommand.F47_StrRtrType, -1);

                if (iProductCommandStatus > 5)
                    return false;

                string message;

                switch (terminalCommandIndex)
                {
                    case 1000: // storage    command
                    case 1001: // re-storage command 
                        lpConveyor = productCommand.F47_From;
                        lpLocation = productCommand.F47_To;
                        nTanaorosiFlg = 0;
                        nShelfTime = 0; // set storage time		
                        switch (iProductStorageRetrieveIndex)
                        {
                            case 0: // Product

                                message = string.Format("SUCCESS: Product Storage. From: {0} To: {1} TermNo: {2}",
                                    productCommand.F47_From,
                                    productCommand.F47_To,
                                    productCommand.F47_TerminalNo);

                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);
                                nShelfSts = 2;
                                nStkFlg = 3;
                                //		nShelfTime = 0;// set storage time
                                nStgRtrCls = 0; // storage
                                break;
                            case 1: // Warehouse pallet

                                message =
                                    string.Format("SUCCESS: Warehouse Pallet Storage. From: {0} To: {1} TermNo: {2}",
                                        productCommand.F47_From,
                                        productCommand.F47_To,
                                        productCommand.F47_TerminalNo);

                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);
                                nShelfSts = 1;
                                nStkFlg = -1;
                                break;
                            case 2: // Outside Preproduct

                                message =
                                    string.Format("SUCCESS: Outside Preproduct Storage. From: {0} To: {1} TermNo: {2}",
                                        productCommand.F47_From,
                                        productCommand.F47_To, productCommand.F47_TerminalNo);

                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);
                                nShelfSts = 8;
                                nOsdPpdStkFlg = 3;
                                break;
                            case 3: // Out of sign Preproduct

                                message = string.Format(
                                    "SUCCESS: Out of sign Preproduct Storage. From: {0} To: {1} TermNo: {2}",
                                    productCommand.F47_From, productCommand.F47_To, productCommand.F47_TerminalNo);

                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);
                                nShelfSts = 3;
                                nStkFlg = -1;
                                break;
                            case 4: // re-storage

                                message = string.Format("SUCCESS: Product Restorage. From: {0} To: {1} TermNo: {2}",
                                    productCommand.F47_From,
                                    productCommand.F47_To, productCommand.F47_TerminalNo);

                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);
                                nShelfSts = 2;
                                nStkFlg = 3;
                                //set re-storage time and recover storage time
                                nShelfTime = 1;
                                nStgRtrCls = 0;
                                break;
                            default:

                                message = string.Format("Unknown type {0} when storage success",
                                    iProductStorageRetrieveIndex);
                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);
                                return false;
                        }
                        break;

                    case 2000: // retrieve command
                    case 2001: // re-retrieve command (impossible)
                        lpConveyor = productCommand.F47_To;
                        lpLocation = productCommand.F47_From;
                        nShelfTime = 2; // set retrieval time	

                        switch (iProductStorageRetrieveIndex)
                        {
                            case 0: // Product
                                message = string.Format("SUCCESS: Product Retrieve.  From: {0} To: {1} TermNo: {2}",
                                    productCommand.F47_From,
                                    productCommand.F47_To, productCommand.F47_TerminalNo);
                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                                nShelfSts = 0;
                                nStkFlg = 0;
                                //		nShelfTime = 2 ; // set retrieval time
                                nStgRtrCls = 1; // retrieval
                                break;
                            case 1: // warehouse's pallet

                                message =
                                    $"SUCCESS: Pallet Retrieve. From: {productCommand.F47_From} To: {productCommand.F47_To} TermNo: {productCommand.F47_TerminalNo}";
                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                                nShelfSts = 0;
                                nStkFlg = -1;
                                break;
                            case 2: // Outside Preproduct
                                message =
                                    $"SUCCESS: Outside Preproduct Retrieve. From: {productCommand.F47_From} To: {productCommand.F47_To} TermNo: {productCommand.F47_TerminalNo}";
                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                                nShelfSts = 0;
                                nOsdPpdStkFlg = 0;
                                break;
                            case 3: // Out of sign Preproduct
                                message =
                                    $"SUCCESS: Out of sign Preproduct Retrieve. From: {productCommand.F47_From} To: {productCommand.F47_To} TermNo: {productCommand.F47_TerminalNo}";
                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                                nShelfSts = 0;
                                nStkFlg = -1;
                                break;
                            default:
                                message = string.Format("Unknown  type {0} when retrieve succeed",
                                    iProductStorageRetrieveIndex);
                                Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                                return false;
                        }
                        break;

                    case 3000: // Moving between conveyers
                        nShelfSts = -1;
                        nStkFlg = -1;
                        lpConveyor = productCommand.F47_From;
                        if (SetConveyorStatus(lpConveyor, 0) <= 0)
                        {
                            message = " Can’t set Conveyor Status";
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                        }

                        lpConveyor = productCommand.F47_To;

                        message = string.Format("SUCCESS: Move From: {0} To: {1} TermNo: {2}", productCommand.F47_From,
                            productCommand.F47_To,
                            productCommand.F47_TerminalNo);

                        Message.InitiateMessage(DateTime.Now, MessageType.Success, message);
                        break;

                    //case 4000:
                    //case 5000:
                    //    // Don't need to do here
                    //    return false;
                    case 6000:

                        message = string.Format("SUCCESS: Tanaonosi Storage. From: {0} To: {1} TermNo: {2}",
                            productCommand.F47_From, productCommand.F47_To,
                            productCommand.F47_TerminalNo);

                        Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                        lpConveyor = productCommand.F47_From;
                        lpLocation = productCommand.F47_To;
                        nShelfSts = 2;
                        nStkFlg = 3;
                        nTanaorosiFlg = 1;
                        break;
                    case 7000:

                        message = string.Format("SUCCESS: Tanaonosi Retrieve. From: {0} To: {1} TermNo: {2}",
                            productCommand.F47_From, productCommand.F47_To,
                            productCommand.F47_TerminalNo);

                        Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                        lpConveyor = productCommand.F47_To;
                        lpLocation = productCommand.F47_From;
                        nShelfSts = -1;
                        nStkFlg = -1;
                        break;
                    default:
                        // not supported in this version
                        message = string.Format("Unknown ID {0} from Automatic Warehouse", terminalCommandIndex);
                        Message.InitiateMessage(DateTime.Now, MessageType.Success, message);

                        return false;
                }

                nRetCode = SetProductCommandStatus(productCommand.F47_CommandNo, productCommand.F47_CmdSeqNo, 6, "0000",
                    true);
                if (nRetCode <= 0)
                {
                    message = string.Format("Set Status of CMD table failed. SEQ {0}", productCommand.F47_CmdSeqNo);
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                }

                // restore conveyor number and status
                if (terminalCommandIndex != 1001)
                {
                    nRetCode = SetConveyorStatus(lpConveyor, 0);
                    if (nRetCode <= 0)
                    {
                        message = " Can’t set Conveyor Status";
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                        message = string.Format("Recover Conveyor Status failed. Conveyor: {0}", lpConveyor);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                }
                // update shelf status of material
                if (nShelfSts >= 0)
                {
                    if ("TCPR121F".Equals(productCommand.F47_PictureNo) && nShelfSts == 2)
                        nShelfSts = 1;

                    nRetCode = SetProductShelfStatus(lpLocation, nShelfSts, nTanaorosiFlg);
                    if (nRetCode <= 0)
                    {
                        message = string.Format("Set Shelf Status failed. LOCATION: {0}", lpLocation);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                }

                // update material stock flag
                if (nStkFlg >= 0)
                {
                    nRetCode = SetStockFlag(productCommand.F47_PalletNo, nStkFlg);
                    if (nRetCode <= 0)
                    {
                        message = string.Format("Set Stock Flag failed. PALLETNO: {0}",
                            !string.IsNullOrEmpty(productCommand.F47_PalletNo) ? productCommand.F47_PalletNo : "null");
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                }

                if (nOsdPpdStkFlg >= 0)
                {
                    nRetCode = SetOsdProductStockFlag(productCommand.F47_PalletNo, nOsdPpdStkFlg);
                    if (nRetCode <= 0)
                    {
                        message = string.Format("Set Outside Preproduct Stock Flag failed. PALLETNO: {0}",
                            !string.IsNullOrEmpty(productCommand.F47_PalletNo) ? productCommand.F47_PalletNo : "null");
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                }

                if (nShelfTime >= 0)
                    SetProductShelfTime(productCommand.F47_PalletNo, lpLocation, nShelfTime);


                if (nStgRtrCls >= 0)
                {
                    nRetCode = SetProductStorageRetrieveHistory(productCommand.F47_PalletNo,
                        productCommand.F47_TerminalNo,
                        productCommand.F47_From,
                        productCommand.F47_To,
                        nStgRtrCls,
                        nShelfTime
                    );
                    if (nRetCode <= 0)
                    {
                        message = "Set StrRtrHistory  failed";
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                }

                unitOfWork.Commit();
            }
            return true;
        }

        /// <summary>
        ///     Set product warehouse command shelf time.
        /// </summary>
        /// <param name="lpPallet"></param>
        /// <param name="lpLocation"></param>
        /// <param name="nShelfTime"></param>
        private void SetProductShelfTime(string lpPallet, string lpLocation, int nShelfTime)
        {
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var lstTx51PdtShfSts =
                    unitOfWork.ProductShelfStatusRepository.GetAll()
                        .ToList()
                        .Where(o => o.F51_ShelfRow + o.F51_ShelfBay + o.F51_ShelfLevel == lpLocation);
                var tX57 = unitOfWork.ProductShelfRepository.GetById(lpPallet.Trim());
                string message;
                try
                {
                    var tx51PdtShfStses = lstTx51PdtShfSts as TX51_PdtShfSts[] ?? lstTx51PdtShfSts.ToArray();
                    foreach (var tX51PdtShfSts in tx51PdtShfStses)
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
                        unitOfWork.ProductShelfStatusRepository.Update(tX51PdtShfSts);
                    }
                    if (!tx51PdtShfStses.Any())
                    {
                        message = "Error occured when set storage-retrieval time";
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    message = "Error occured when set storage-retrieval time";
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
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
                        unitOfWork.ProductShelfRepository.Update(tX57);
                    }
                    else
                    {
                        message = "Error occured  when set storage-retrieval time";
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }

                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    message = "Error occured when set storage-retrieval time";
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                }
            }
        }

        /// <summary>
        ///     Set product storage/retrieve history.
        /// </summary>
        /// <param name="lpPallet"></param>
        /// <param name="lpTermNo"></param>
        /// <param name="lpFrom"></param>
        /// <param name="lpTo"></param>
        /// <param name="nStgRtrCls"></param>
        /// <param name="nShelfTime"></param>
        /// <returns></returns>
        private int SetProductStorageRetrieveHistory(string lpPallet, string lpTermNo, string lpFrom, string lpTo,
            int nStgRtrCls,
            int nShelfTime)
        {
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                // Set Material Storage-Retrieval Result
                DateTime stgRtrDate;
                var tx57PdtShf = unitOfWork.ProductShelfRepository.GetById(lpPallet.Trim());
                string message;
                if (tx57PdtShf == null)
                {
                    message = "Error occured<select>  when set storage-retrieval history";
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    return -1;
                }

                switch (nStgRtrCls)
                {
                    case 0:
                        if (tx57PdtShf.F57_StorageDate == null || !tx57PdtShf.F57_ReStorageDate.HasValue)
                            return -1;
                        if (nShelfTime == 0)
                            stgRtrDate = tx57PdtShf.F57_StorageDate.Value;
                        else
                            stgRtrDate = tx57PdtShf.F57_ReStorageDate.Value;
                        break;
                    case 1:
                        if (!tx57PdtShf.F57_RetievalDate.HasValue)
                            return -1;
                        stgRtrDate = tx57PdtShf.F57_RetievalDate.Value;
                        break;
                    default:
                        return -1;
                }
                var lstPdtShfStk =
                    unitOfWork.ProductShelfStockRepository.GetMany(i => i.F40_PalletNo.Trim().Equals(lpPallet.Trim()));

                if (!lstPdtShfStk.Any())
                {
                    message = "Error occured<select>  when set storage-retrieval history";
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                    return -1;
                }
                try
                {
                    foreach (var tx40PdtShfStk in lstPdtShfStk)
                    {
                        var tH65PdtStgRtrHst = new TH65_PdtStgRtrHst
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
                        unitOfWork.ProductStorageRetrieveHistoryRepository.Add(tH65PdtStgRtrHst);
                    }
                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    message = "Error occured<insert>  when set storage-retrieval history";
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    return -1;
                }
                return 1;
            }
        }

        /// <summary>
        ///     Set product shelf status
        /// </summary>
        /// <param name="lpLocation"></param>
        /// <param name="nStatus"></param>
        /// <param name="nTanaorosiFlg"></param>
        /// <param name="nReason"></param>
        /// <returns></returns>
        private int SetProductShelfStatus(string lpLocation, int nStatus, int nTanaorosiFlg, int nReason = -1)
        {
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                if (nStatus < 0) return 0;
                var lstPdtShfSts =
                    unitOfWork.ProductShelfStatusRepository.GetAll()
                        .ToList()
                        .Where(o => o.F51_ShelfRow + o.F51_ShelfBay + o.F51_ShelfLevel == lpLocation);
                try
                {
                    var tx51PdtShfStses = lstPdtShfSts as TX51_PdtShfSts[] ?? lstPdtShfSts.ToArray();
                    if (!tx51PdtShfStses.Any())
                    {
                        var message = "Set Shelf Status failed ";
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                        return -1;
                    }
                    foreach (var pdtShfSts in tx51PdtShfStses)
                    {
                        pdtShfSts.F51_ShelfStatus = nStatus.ToString();
                        pdtShfSts.F51_UpdateDate = DateTime.Now;
                        if (nStatus == 0 || nStatus == 6 || nStatus == 7)
                        {
                            if (nReason != 3)
                                pdtShfSts.F51_PalletNo = null;
                        }
                        else
                        {
                            pdtShfSts.F51_StockTakingFlag = nTanaorosiFlg.ToString();
                        }
                        unitOfWork.ProductShelfStatusRepository.Update(pdtShfSts);
                        unitOfWork.Commit();

                        return 1;
                    }
                }

                catch (Exception)
                {
                    var message = "Set Shelf Status failed ";
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    return -1;
                }
                return 0;
            }
        }

        /// <summary>
        ///     Set osd product stock flag.
        /// </summary>
        /// <param name="lpPallet"></param>
        /// <param name="nFlg"></param>
        /// <returns></returns>
        private int SetOsdProductStockFlag(string lpPallet, int nFlg)
        {
            // set STOCKFLAG  int  OutsidePrepdtStk TABLE
            if (nFlg < 0)
                return 0;
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                try
                {
                    var outSidePrePdtStk = unitOfWork.OutSidePrePdtStkRepository.GetById(lpPallet.Trim());
                    if (outSidePrePdtStk != null)
                    {
                        outSidePrePdtStk.F53_StockFlag = nFlg.ToString();
                        outSidePrePdtStk.F53_UpdateDate = DateTime.Now;
                        unitOfWork.OutSidePrePdtStkRepository.Update(outSidePrePdtStk);
                    }
                    else
                    {
                        var message = "Error occured  when set stock flag";
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                        return -1;
                    }

                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    var message = "Error occured  when set stock flag";
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    return -1;
                }
            }

            return 1;
        }

        /// <summary>
        ///     Set product stock flag.
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
                // update material stock flag
                var pdtShfStk =
                    unitOfWork.ProductShelfStockRepository.GetMany(i => i.F40_PalletNo.Trim().Equals(lpPallet.Trim()));
                try
                {
                    foreach (var tx40PdtShfStk in pdtShfStk)
                    {
                        tx40PdtShfStk.F40_StockFlag = nFlag.ToString();
                        tx40PdtShfStk.F40_UpdateDate = DateTime.Now;
                        unitOfWork.ProductShelfStockRepository.Update(tx40PdtShfStk);
                    }
                    if (!pdtShfStk.Any())
                    {
                        var message = "Error occured  when set stock flag";
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                        return -1;
                    }
                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    var message = "Error occured  when set stock flag";
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    return -1;
                }
                return 1;
            }
        }

        /// <summary>
        ///     Set product command status.
        /// </summary>
        /// <param name="lpCmdNo"></param>
        /// <param name="lpSeq"></param>
        /// <param name="nStatus"></param>
        /// <param name="pAbnormalCode"></param>
        /// <param name="endData"></param>
        /// <param name="nRetry"></param>
        /// <returns></returns>
        private int SetProductCommandStatus(string lpCmdNo, string lpSeq, int nStatus, string pAbnormalCode,
            bool endData, int nRetry = 0)
        {
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var tx47PdtWhsCmd = unitOfWork.ProductWarehouseCommandRepository.GetByCommondNoAndSeqNo(lpSeq, lpCmdNo);
                if (tx47PdtWhsCmd == null)
                {
                    var message = "Select cmd table failed when set status";
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    return -1;
                }
                var nFldRetryCount = tx47PdtWhsCmd.F47_RetryCount ?? 0;
                var nFldStatus = tx47PdtWhsCmd.F47_Status;

                if (nFldStatus == nStatus.ToString())
                    nFldRetryCount += nRetry;
                else
                    nFldRetryCount = nRetry;
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
                        tx47PdtWhsCmd.F47_CommandEndDate = DateTime.Now;
                    else
                        tx47PdtWhsCmd.F47_CommandSendDate = DateTime.Now;

                    unitOfWork.ProductWarehouseCommandRepository.Update(tx47PdtWhsCmd);
                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    var message = "update cmd table failed when set status";
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    return -1;
                }
                return 1;
            }
        }

        /// <summary>
        ///     Cancel product warehouse command.
        /// </summary>
        /// <param name="commandNo"></param>
        /// <param name="cmdSeqNo"></param>
        /// <param name="nHowCancel"></param>
        /// <param name="nConveyorErr"></param>
        /// <returns></returns>
        private bool CancelProductWarehouseCommand(string commandNo, string cmdSeqNo, int nHowCancel = 0,
            bool nConveyorErr = false)
        {
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                var commandIndex = Converter.TextToInt(commandNo, -1);
                string lpConveyor, lpLocation = "";
                int nShelfSts, nStkFlg = -1;
                var nOsdPpdStkFlg = -1;
                var nRetCode = 0;
                var pMsg = unitOfWork.ProductWarehouseCommandRepository.GetByCommondNoAndSeqNo(cmdSeqNo, commandNo);
                var nStrRtrType = Converter.TextToInt(pMsg.F47_StrRtrType, -1);

                string message;

                switch (commandIndex)
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

                                message = string.Format("CANCEL: Product Storage. From: {0} To: {1}", pMsg.F47_From,
                                    pMsg.F47_To);
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                if (nHowCancel == 2)
                                    nShelfSts = 6;
                                else
                                    nShelfSts = 0;
                                break;
                            case 1: // warehouse Pallet

                                message = string.Format("CANCEL: Pallet Storage. From: {0} To: {1}", pMsg.F47_From,
                                    pMsg.F47_To);
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                                nStkFlg = -1;
                                if (nHowCancel == 2)
                                    nShelfSts = 6;
                                else
                                    nShelfSts = 0;
                                break;
                            case 2: // Outside Preproduct

                                message = string.Format("CANCEL: Outside Preproduct Storage. From: {0} To: {1}",
                                    pMsg.F47_From,
                                    pMsg.F47_To);
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                                if (nHowCancel == 2)
                                    nShelfSts = 6;
                                else
                                    nShelfSts = 0;
                                nOsdPpdStkFlg = 0;
                                break;
                            case 3: // Out of sign Preproduct

                                message = string.Format("CANCEL: Out of sign Preproduct Storage. From: {0} To: {1}",
                                    pMsg.F47_From,
                                    pMsg.F47_To);

                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                if (nHowCancel == 2)
                                    nShelfSts = 6;
                                else
                                    nShelfSts = 0;

                                nStkFlg = -1;
                                break;
                            default:

                                message = string.Format("Unknown type{0} when CANCEL", nStrRtrType);
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

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
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                nShelfSts = 2;
                                nStkFlg = 3;
                                break;
                            case 1: // warehouse Pallet

                                message = string.Format("CANCEL: Pallet Retrieve. From: {0} To: {1}", pMsg.F47_From,
                                    pMsg.F47_To);
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                                nShelfSts = 1;
                                nStkFlg = -1;
                                break;
                            case 2: // Outside-making Preproduct

                                message = string.Format(
                                    "CANCEL: Outside Making Preproduct Retrieve. From: {0} To: {1}", pMsg.F47_From,
                                    pMsg.F47_To);
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                                nShelfSts = 8;
                                nOsdPpdStkFlg = 3;
                                break;
                            case 3: // Out of sign Preproduct

                                message = string.Format(
                                    "CANCEL: Outside Making Preproduct Retrieve. From: {0} To: {1}", pMsg.F47_From,
                                    pMsg.F47_To);
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                                nShelfSts = 3;
                                nStkFlg = -1;
                                break;
                            default:
                                message = string.Format("Unknown  type {0} when CANCEL", nStrRtrType);
                                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                                return false;
                        }
                        break;
                    case 3000: // Moving between conveyers
                        nShelfSts = -1;
                        nStkFlg = -1;
                        lpConveyor = pMsg.F47_From;

                        message = string.Format("CANCEL: Move. From: {0} To: {1}", pMsg.F47_From, pMsg.F47_To);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                        if (nConveyorErr)
                            nRetCode = SetConveyorStatus(lpConveyor, 9);
                        else
                            nRetCode = SetConveyorStatus(lpConveyor, 0);
                        if (nRetCode <= 0)
                        {
                            message = " Can’t set Conveyor Status";
                            Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
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

                        message = string.Format("CANCEL: Tanaonosi Storage. From: {0} To: {1}", pMsg.F47_From,
                            pMsg.F47_To);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
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

                        message = string.Format("CANCEL: Tanaonosi Retrieve. From: {0} To: {1}", pMsg.F47_From,
                            pMsg.F47_To);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                        break;
                    default:
                        // not supported in this version
                        message = string.Format("Unknown Cmdid {0} from Automatic Warehouse", commandIndex);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);

                        return false;
                }

                switch (nHowCancel)
                {
                    case 0: // normal
                        // update status to 7
                        nRetCode = SetProductCommandStatus(pMsg.F47_CommandNo, pMsg.F47_CmdSeqNo, 7, "0000", true);
                        break;
                    case 1: // manual
                        // update status to 7
                        nRetCode = SetProductCommandStatus(pMsg.F47_CommandNo, pMsg.F47_CmdSeqNo, 7, "0000", true);
                        // nRetCode = C3_AMakeHistory(pMsg.F47_CommandNo, pMsg.F47_CmdSeqNo, &nErrCode);
                        break;
                    case 2: // storage 2 times
                        nRetCode = SetProductCommandStatus(pMsg.F47_CommandNo, pMsg.F47_CmdSeqNo, 8, "0000", true);
                        break;
                    case 3: // empty retrieve
                        nRetCode = SetProductCommandStatus(pMsg.F47_CommandNo, pMsg.F47_CmdSeqNo, 9, "0000", true);
                        // modified by Crystal WU on 96/11/01 for set ShelfStatus to prohibit when empty retrieve
                        //nShelfSts = 0;
                        nShelfSts = 6;
                        // end of modified
                        if ((commandIndex == 2000 || commandIndex == 2001 || commandIndex == 7000) && nStrRtrType == 0)
                        {
                            nStkFlg = 0;
                            nOsdPpdStkFlg = -1;
                        }
                        else if ((commandIndex == 2000 || commandIndex == 2001 || commandIndex == 7000) && nStrRtrType == 2)
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
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                }
                if (nConveyorErr)
                {
                    // Set the conveyor status to Error
                    nRetCode = SetConveyorStatus(lpConveyor, 0);
                }
                else
                {
                    // restore conveyor number and status
                    if (commandIndex != 1001)
                        nRetCode = SetConveyorStatus(lpConveyor, 0);
                }
                if (nRetCode <= 0)
                {
                    message = " Can’t set Conveyor Status";
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    message = string.Format("Recover Conveyor Status  failed. Conveyor: {0}", lpConveyor);
                    Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                }
                // update shelf status of material
                if (nShelfSts >= 0)
                {
                    nRetCode = SetProductShelfStatus(lpLocation, nShelfSts, -1, nHowCancel);
                    if (nRetCode <= 0)
                    {
                        message = string.Format("Set Shelf Status failed. LOCATION: {0}", lpLocation);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                }

                // update material stock flag
                if (nStkFlg >= 0)
                {
                    nRetCode = SetStockFlag(pMsg.F47_PalletNo, nStkFlg);
                    if (nRetCode <= 0)
                    {
                        message = string.Format("Set Stock Flag failed. PALLETNO: {0}", pMsg.F47_PalletNo);
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                }

                if (nOsdPpdStkFlg >= 0)
                {
                    nRetCode = SetOsdProductStockFlag(pMsg.F47_PalletNo, nOsdPpdStkFlg);
                    if (nRetCode <= 0)
                    {
                        message = string.Format("Set OutsidePreproductStock Flag failed. PALLETNO: {0}",
                            string.IsNullOrEmpty(pMsg.F47_PalletNo) ? pMsg.F47_PalletNo : "null");
                        Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
                    }
                }

                return true;
            }
        }

        /// <summary>
        ///     Set product auto warehouse to be failed.
        /// </summary>
        /// <param name="deviceCode"></param>
        private void SetProductAutoWarehouseFailure(string deviceCode)
        {
            var status = 2;
            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                unitOfWork.DeviceRepository.UpdateStatus(deviceCode, status);
                unitOfWork.Commit();

                var message = "Autoware House is at Error status..";
                Message.InitiateMessage(DateTime.Now, MessageType.Error, message);
            }
        }

        /// <summary>
        ///     Set conveyor status.
        /// </summary>
        /// <param name="lpConveyer"></param>
        /// <param name="nStatus"></param>
        /// <returns></returns>
        public int SetConveyorStatus(string lpConveyer, int nStatus)
        {
            var nRecordCount = 1;
            var nRetCode = 1;
            if (nStatus < 0) return 0;

            // Conveyor instance.
            TM05_Conveyor conveyor = null;

            using (var context = new KCSGDbContext())
            using (var unitOfWork = new UnitOfWork(context))
            {
                // restore conveyor number and status
                try
                {
                    switch (nStatus)
                    {
                        case 1:
                        case 2:
                        {
                            // Storage or Retrieve send OK
                            var lstConveyor =
                                unitOfWork.ConveyorRepository.GetMany(
                                    i => i.F05_ConveyorCode.Trim().Equals(lpConveyer.Trim()));
                            foreach (var tm05Conveyor in lstConveyor)
                            {
                                var iStorageRetrieveStatus = Converter.TextToInt(tm05Conveyor.F05_StrRtrSts, -1);
                                if (iStorageRetrieveStatus >= 0 &&
                                    iStorageRetrieveStatus <= nStatus &&
                                    tm05Conveyor.F05_BufferUsing < tm05Conveyor.F05_MaxBuffer)
                                {
                                    tm05Conveyor.F05_StrRtrSts = nStatus.ToString();
                                    tm05Conveyor.F05_BufferUsing = tm05Conveyor.F05_BufferUsing + 1;
                                    tm05Conveyor.F05_UpdateDate = DateTime.Now;
                                    unitOfWork.ConveyorRepository.Update(tm05Conveyor);
                                }
                            }
                            break;
                        }
                        case 3:
                            // Floor Movement send OK
                            conveyor =
                                unitOfWork.ConveyorRepository.GetMany(
                                    i => i.F05_ConveyorCode == lpConveyer && i.F05_StrRtrSts == "0").FirstOrDefault();
                            if (conveyor != null)
                            {
                                conveyor.F05_StrRtrSts = nStatus.ToString();
                                conveyor.F05_BufferUsing = conveyor.F05_BufferUsing + 1;
                                conveyor.F05_UpdateDate = DateTime.Now;
                                unitOfWork.ConveyorRepository.Update(conveyor);
                            }
                            break;
                        case 0:
                            try
                            {
                                // Command End or Command Cancel

                                conveyor =
                                    unitOfWork.ConveyorRepository.GetMany(
                                        i => i.F05_ConveyorCode == lpConveyer && i.F05_BufferUsing > 0).FirstOrDefault();
                                if (conveyor != null)
                                {
                                    conveyor.F05_BufferUsing = conveyor.F05_BufferUsing - 1;
                                    if (conveyor.F05_BufferUsing == 0)
                                        conveyor.F05_StrRtrSts = "0";
                                    conveyor.F05_UpdateDate = DateTime.Now;
                                    unitOfWork.ConveyorRepository.Update(conveyor);
                                }
                            }
                            catch (Exception)
                            {
                                nRetCode = -1;
                            }
                            break;
                        default:
                            // If status == 9 then this conveyor is in error status
                            var conveyor1 =
                                unitOfWork.ConveyorRepository.GetById(lpConveyer.Trim());
                            conveyor1.F05_StrRtrSts = "0";
                            conveyor1.F05_UpdateDate = DateTime.Now;
                            unitOfWork.ConveyorRepository.Update(conveyor1);
                            break;
                    }

                    unitOfWork.Commit();
                }
                catch (Exception)
                {
                    //todo Can’t set Conveyor Status
                    nRetCode = -1;
                }

                switch (nStatus)
                {
                    case 0:
                        return nRecordCount;
                    default:
                        return nRetCode;
                }
            }
        }

        /// <summary>
        ///     Build a terminal message and send to external terminal.
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="productWarehouseCommand"></param>
        /// <returns></returns>
        private int SendCommandToExternalTerminal(ConnectionSetting endPoint, TX47_PdtWhsCmd productWarehouseCommand)
        {
            #region Command refactor

            // added by Crystal Wu on 96/11/08 
            if ("7000".Equals(productWarehouseCommand.F47_CommandNo))
                productWarehouseCommand.F47_CommandNo = "2000";
            if ("6000".Equals(productWarehouseCommand.F47_CommandNo))
                productWarehouseCommand.F47_CommandNo = "1000";

            #endregion

            #region Command validity

            // Find terminal message in queue.
            var terminalMessage = FindCommandInQueue(productWarehouseCommand.F47_CmdSeqNo,
                productWarehouseCommand.F47_CommandNo);

            // Terminal message is valid but not sendable.
            if (terminalMessage != null && !IsSendable(terminalMessage))
                return -1;

            if (terminalMessage == null)
            {
                terminalMessage = new TerminalMessage(productWarehouseCommand);
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