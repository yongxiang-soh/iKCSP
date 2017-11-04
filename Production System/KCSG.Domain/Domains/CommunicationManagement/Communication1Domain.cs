using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.CommunicationManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.CommunicationManagement;
using KCSG.jsGrid.MVC;
using log4net;

namespace KCSG.Domain.Domains.CommunicationManagement
{
    public class Communication1Domain : BaseDomain, ICommunication1Domain
    {
        /// <summary>
        ///     Find history by using specific conditions.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="terminal"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        public ResponseResult<GridResponse<HistoryItem>> GetHistory(DateTime? date, string terminal,
            GridSettings gridSettings)
        {
            var result = _unitOfWork.MaterialWarehouseCommandHistoryRepository.GetAll();
            if (!string.IsNullOrEmpty(terminal))
                result = result.Where(i => i.F60_TerminalNo.Trim().Equals(terminal.Trim()));
            if (date.HasValue)
                result = result.Where(i => i.F60_AddDate >= date);
            var itemCount = result.Count();
            var lstResult = result.Select(i => new HistoryItem
            {
                AbnormalCode = i.F60_AbnormalCode,
                AddDate = i.F60_AddDate,
                CommandNo = i.F60_CommandNo,
                CommandSeqNo = i.F60_CmdSeqNo,
                CommandType = i.F60_CommandType, //Enum.GetName(typeof(Constants.TX34_CmdType), i.F60_CommandType),
                From = i.F60_From,
                PalletNo = i.F60_PalletNo,
                Priority = i.F60_Priority,
                To = i.F60_To,
                CommandDate = i.F60_CommandSendDate
            });
            if (gridSettings != null)
                OrderByAndPaging(ref lstResult, gridSettings);
            var resultModel = new GridResponse<HistoryItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<HistoryItem>>(resultModel, true);
        }

        public ResponseResult<GridResponse<QueueItem>> GetQueue(DateTime? date, string terminal,
            GridSettings gridSettings)
        {
            var result = _unitOfWork.MaterialWarehouseCommandRepository.GetAll();
            if (!string.IsNullOrEmpty(terminal))
                result = result.Where(i => i.F34_TerminalNo.Trim().Equals(terminal.Trim()));
            if (date.HasValue)
                result = result.Where(i => i.F34_AddDate >= date);
            var itemCount = result.Count();
            var lstResult = result.Select(i => new QueueItem
            {
                AbnormalCode = i.F34_AbnormalCode,
                AddDate = i.F34_AddDate,
                CommandEndDate = i.F34_CommandEndDate,
                CommandNo = i.F34_CommandNo,
                CommandSendDate = i.F34_CommandSendDate,
                CommandSeqNo = i.F34_CmdSeqNo,
                CommandType = i.F34_CommandType, //Enum.GetName(typeof(Constants.TX34_CmdType), i.F34_CommandType),
                From = i.F34_From,
                PalletNo = i.F34_PalletNo,
                PictureNo = i.F34_PictureNo,
                Priority = i.F34_Priority,
                RetryCount = i.F34_RetryCount,
                Status = i.F34_Status,
                StrRtrTypeMaterial = i.F34_StrRtrType,
                TerminalNo = i.F34_TerminalNo,
                To = i.F34_To,
                UpdateDate = i.F34_UpdateDate
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
                //	Update field [F34_RetryCount] of Material Warehouse Command records whose [F34_Status] is 1, 2, 4 or 5 as 0.
                var lstTx34 =
                    _unitOfWork.MaterialWarehouseCommandRepository.GetMany(
                        i => i.F34_Status == Constants.F34_Status.status2 ||
                             i.F34_Status == Constants.F34_Status.status1 ||
                             i.F34_Status == Constants.F34_Status.status4 ||
                             i.F34_Status == Constants.F34_Status.status5);
                foreach (var tx34MtrWhsCmd in lstTx34)
                {
                    tx34MtrWhsCmd.F34_RetryCount = 0;
                    _unitOfWork.MaterialWarehouseCommandRepository.Update(tx34MtrWhsCmd);
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
            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseMaterialScreenName,
                message);
            return true;
        }

        public ResponseResult DeleteMaterialWarehouseCommand(string commandNo, string cmdSeqNo)
        {
            using (var contet = new KCSGConnectionString())
            {
                var material =
                    contet.TX34_MtrWhsCmd.FirstOrDefault(i => i.F34_CommandNo.Trim().Equals(commandNo.Trim()) &&
                                                              i.F34_CmdSeqNo.Trim().Equals(cmdSeqNo.Trim()));
                if (material != null)
                    if (ConvertHelper.ToInteger(material.F34_Status) >=
                        ConvertHelper.ToInteger(Constants.TC_CMDSTS.TC_CMDSTS_6)
                        &&
                        ConvertHelper.ToInteger(material.F34_Status) <=
                        ConvertHelper.ToInteger(Constants.TC_CMDSTS.TC_CMDSTS_9)
                        || material.F34_Status == Constants.TC_CMDSTS.TC_CMDSTS_A
                        || material.F34_Status == Constants.TC_CMDSTS.TC_CMDSTS_B
                        || material.F34_Status == Constants.TC_CMDSTS.TC_CMDSTS_C
                        || material.F34_Status == Constants.TC_CMDSTS.TC_CMDSTS_D
                        || material.F34_Status == Constants.TC_CMDSTS.TC_CMDSTS_E
                        || material.F34_Status == Constants.TC_CMDSTS.TC_CMDSTS_F)
                    {
                        //	Create a Material Warehouse History by duplicating the selected Material Warehouse Command.

                        var history = _unitOfWork.MaterialWarehouseCommandHistoryRepository.AddByMaterialCmd(material);
                        contet.TH60_MtrWhsCmdHst.AddOrUpdate(history);
                        contet.TX34_MtrWhsCmd.Remove(material);
                        contet.SaveChanges();
                        return new ResponseResult(true);
                    }
                    else
                    {
                        //If [F34_Status] of the selected Material Warehouse Command record is not 6, 7, 8, 9, A, B, C, D, E and F, 
                        //the system will display error message using error message template MSG 4.
                        return new ResponseResult(false, "MSG4");
                    }
                //	If there is any occurred error, the system will display message “” in field [Edit Log].
                //	Trigger use case UC 32: View Product Warehouse Commands.

                return new ResponseResult(false, "Error occurred when make history");
            }
        }


        public bool DeleteMaterialWarehouseHistories()
        {
            try
            {
                var lstTh60 = _unitOfWork.MaterialWarehouseCommandHistoryRepository.GetAll();

                foreach (var th60MtrWhsCmdHst in lstTh60)
                    _unitOfWork.MaterialWarehouseCommandHistoryRepository.Delete(th60MtrWhsCmdHst);

                _unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EndMaterialWarehouseCommand(string commandNo, string cmdSeqNo)
        {
            var nShelfSts = 0;
            var nStkFlg = 0;
            var nShelfTime = -1;
            var nStgRtrCls = -1;
            var nTanaorosiFlg = -1;
            string lpConveyor;
            var lpLocation = "";
            int nRetCode;
            var material =
                _unitOfWork.MaterialWarehouseCommandRepository.GetMany(
                        i =>
                            i.F34_CommandNo.Trim().Equals(commandNo.Trim()) &&
                            i.F34_CmdSeqNo.Trim().Equals(cmdSeqNo.Trim()))
                    .FirstOrDefault();

            if (material == null)
                return false;
            if (ConvertHelper.ToInteger(material.F34_Status) >
                ConvertHelper.ToInteger(Constants.TC_CMDSTS.TC_CMDSTS_5))
                return false;
            string message;

            switch (ConvertHelper.ParseEnum<Constants.F34_CommandNo>(material.F34_CommandNo))
            {
                case Constants.F34_CommandNo.Storage: // storage    command
                case Constants.F34_CommandNo.TwoTimesIn:
                    // re-storage command 
                    lpConveyor = material.F34_From;
                    lpLocation = material.F34_To;
                    nTanaorosiFlg = 0;

                    switch (ConvertHelper.ParseEnum<Constants.TX34_StrRtrType>(material.F34_StrRtrType))
                    {
                        case Constants.TX34_StrRtrType.Material: // material
                            nShelfSts = 3;
                            nStkFlg = 3;
                            nShelfTime = 0; // set storage time
                            nStgRtrCls = 0; // storage

                            message = string.Format("SUCCESS: {0} Storage. From: {1} To: {2} TermNo: {3}", "Material",
                                material.F34_From, material.F34_To, material.F34_TerminalNo);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseMaterialScreenName, message);

                            break;
                        case Constants.TX34_StrRtrType.WarehousePallet: // empty pallet
                            nShelfSts = 1;
                            nStkFlg = -1;

                            message = string.Format("SUCCESS: {0} Storage. From: {1} To: {2} TermNo: {3}",
                                "Empty Pallet", material.F34_From, material.F34_To, material.F34_TerminalNo);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseMaterialScreenName, message);

                            break;
                        case Constants.TX34_StrRtrType.SupplierPallet: // provider's pallet

                            message = string.Format("SUCCESS: {0} Storage. From: {1} To: {2} TermNo: {3}",
                                "Provider's Pallet", material.F34_From, material.F34_To, material.F34_TerminalNo);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseMaterialScreenName, message);

                            nShelfSts = 2;
                            nStkFlg = -1;
                            break;
                        case Constants.TX34_StrRtrType.SupplierPalletSideIn: // retrieve from supplier pallet
                            break;
                        case Constants.TX34_StrRtrType.MaterialReStorage: // re-storage
                            message = string.Format("SUCCESS: {0}Storage. From: {1} To: {2} TermNo: {3}", "Material Re-",
                                material.F34_From, material.F34_To, material.F34_TerminalNo);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseMaterialScreenName, message);

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

                case Constants.F34_CommandNo.Retrieval: // retrieve command
                case Constants.F34_CommandNo.ReRetrieve: // re-retrieve command (impossible)
                    lpConveyor = material.F34_To;
                    lpLocation = material.F34_From;

                    switch (ConvertHelper.ParseEnum<Constants.TX34_StrRtrType>(material.F34_StrRtrType))
                    {
                        case Constants.TX34_StrRtrType.Material:
                            // material
                            message = string.Format("SUCCESS: {0} Retrieve. From: {1} To: {2} TermNo: {3}", "Material",
                                material.F34_From, material.F34_To, material.F34_TerminalNo);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseMaterialScreenName, message);

                            nShelfSts = 0;
                            nStkFlg = 0;
                            nShelfTime = 2; // set retrieval time
                            nStgRtrCls = 1; // retrieval
                            break;
                        case Constants.TX34_StrRtrType.WarehousePallet: // empty pallet
                        case Constants.TX34_StrRtrType.SupplierPallet: // provider's pallet		  (front)

                            message = string.Format("SUCCESS: {0} Storage. From: {1} To: {2} TermNo: {3}", "Pallet",
                                material.F34_From, material.F34_To, material.F34_TerminalNo);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseMaterialScreenName, message);

                            nShelfSts = 0;
                            nStkFlg = -1;
                            break;
                        case Constants.TX34_StrRtrType.SupplierPalletSideIn: // provider's pallet retrieve (side)

                            message = string.Format("SUCCESS: {0} Retrieve. From: {1} To: {2} TermNo: {3}", "Pallet",
                                material.F34_From, material.F34_To, material.F34_TerminalNo);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseMaterialScreenName, message);
                            
                            nShelfSts = -1;
                            nStkFlg = -1;
                            break;
                        default:
                            return false;
                    }
                    break;

                case Constants.F34_CommandNo.Move: // Moving between conveyers

                    message = string.Format("SUCCESS: Material Move From: {0} To: {1} TermNo: {2}", material.F34_From,
                        material.F34_To, material.F34_TerminalNo);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName, message);
                    
                    nShelfSts = -1;
                    nStkFlg = -1;
                    lpConveyor = material.F34_From;
                    SetConveyorSts(lpConveyor, 0);
                    lpConveyor = material.F34_To;
                    break;


                case Constants.F34_CommandNo.StockTakingIn:

                    message = string.Format("SUCCESS: TANAONOSI Material Storage. From: {0} To: {1} TermNo: {2}",
                        material.F34_From, material.F34_To, material.F34_TerminalNo);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName, message);
                    
                    lpConveyor = material.F34_From;
                    lpLocation = material.F34_To;
                    nShelfSts = 3;
                    nStkFlg = 3;
                    nTanaorosiFlg = 1;
                    break;

                case Constants.F34_CommandNo.StockTakingOff:

                    message = string.Format("SUCCESS: TANAONOSI Material Retrieve. From: {0} To: {1} TermNo: {2}",
                        material.F34_From, material.F34_To, material.F34_TerminalNo);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName, message);
                    
                    lpConveyor = material.F34_To;
                    lpLocation = material.F34_From;
                    nShelfSts = -1;
                    nStkFlg = -1;
                    break;
                default:
                    // not supported in this version
                    return false;
            }
            // manaual end the command.
            // update status to 6
            nRetCode = SetMtrCmdSts(material.F34_CommandNo, material.F34_CmdSeqNo, 6, "0000", 0, true);
            if (nRetCode <= 0)
            {
                message = string.Format("Set Material Command Status Failed,Message ID: {0}, SEQ: {1}",
                    material.F34_CommandNo, material.F34_CmdSeqNo);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseMaterialScreenName,
                    message);
            }
            // restore conveyor number and status
            if (material.F34_CommandNo != Constants.F34_CommandNo.TwoTimesIn.ToString("d"))
            {
                nRetCode = SetConveyorSts(lpConveyor, 0);
                if (nRetCode <= 0)
                {
                    message = string.Format("Recover Conveyor status failed. {0}", lpConveyor);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName, message);
                }
            }
            // update shelf status of material
            if (nShelfSts >= 0)
            {
                nRetCode = SetShelfSts(lpLocation, nShelfSts, nTanaorosiFlg);
                if (nRetCode <= 0)
                {
                    message = string.Format("Set Shelf Status failed,LOCATION: {0}", lpLocation);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName, message);
                }
            }

            // update material stock flag
            if (nStkFlg >= 0)
            {
                nRetCode = SetStockFlag(material.F34_PalletNo, nStkFlg);
                if (nRetCode <= 0)
                {
                    message = string.Format("Set Stock Flag failed,PALLET: {0}", material.F34_PalletNo);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName, message);
                }
            }

            if (nShelfTime >= 0)
            {
                nRetCode = SetShelfTime(material.F34_PalletNo, nShelfTime, lpLocation);
                if (nRetCode <= 0)
                {
                    message = string.Format("Set Shelf Time failed,PALLET: {0} LOCATION: {1}", material.F34_PalletNo,
                        lpLocation);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName, message);
                }
            }

            if (nStgRtrCls >= 0)
            {
                nRetCode = SetStgRtrHst(material.F34_PalletNo,
                    material.F34_TerminalNo,
                    material.F34_From,
                    material.F34_To,
                    nStgRtrCls,
                    nShelfTime);
                if (nRetCode < 0)
                {
                    message = string.Format("Set Storage-Retrieve History failed.PALLET: {0}", material.F34_PalletNo);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName, message);
                }
            }
            _unitOfWork.Commit();
            return true;
        }

        public bool CancelMaterialWarehouseCommand(string commandNo, string cmdSeqNo)
        {
            return CommandCancel(commandNo, cmdSeqNo, 1);
        }

        public bool ProcessData(bool isAutomatedWarehouse, string deviceCode)
        {
            //if(m_nTidAccept){	// Now is at waiting command accept message
            //    // just return
            //    return;
            //}

            var device = _unitOfWork.DeviceRepository.GetById(deviceCode);
            var lstPdtWhsCmd = _unitOfWork.MaterialWarehouseCommandRepository.GetAll()
                .OrderByDescending(i => i.F34_Priority)
                .ThenByDescending(i => i.F34_Status);
            var check = Constants.F14_DeviceStatus.Online.ToString("D").Equals(device.F14_DeviceStatus);
            if (!check)
            {
                var message = "Autoware House is at offline or error status..";
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseMaterialScreenName,
                    message);

                if (!isAutomatedWarehouse)
                    lstPdtWhsCmd = lstPdtWhsCmd.Where(i => i.F34_Status.Trim().Equals(Constants.F34_Status.status6) ||
                                                           i.F34_Status.Trim().Equals(Constants.F34_Status.status7) ||
                                                           i.F34_Status.Trim().Equals(Constants.F34_Status.status8) ||
                                                           i.F34_Status.Trim().Equals(Constants.F34_Status.status9))
                        .OrderByDescending(i => i.F34_Priority)
                        .ThenByDescending(i => i.F34_Status);
                else
                    lstPdtWhsCmd = lstPdtWhsCmd.Where(i => i.F34_Status.Trim().Equals(Constants.F34_Status.status0) ||
                                                           i.F34_Status.Trim().Equals(Constants.F34_Status.status1) ||
                                                           i.F34_Status.Trim().Equals(Constants.F34_Status.status2) ||
                                                           i.F34_Status.Trim().Equals(Constants.F34_Status.status4) ||
                                                           i.F34_Status.Trim().Equals(Constants.F34_Status.status5))
                        .OrderByDescending(i => i.F34_Priority)
                        .ThenByDescending(i => i.F34_Status);
            }


            ProcessDataList(lstPdtWhsCmd, deviceCode);

            if (!isAutomatedWarehouse)
                MakeHistory();

            return true;
        }

        public string GetDeviceStatus(string deviceCode)
        {
            var device = _unitOfWork.DeviceRepository.GetById(deviceCode);
            return device != null ? device.F14_DeviceStatus : "";
        }

        #region respone message form AW

        public void ProcessAwMessage(MessageFormAW aw, string deviceCode)
        {
            if (aw.Id == "5000")
                return;
            var tm34 = _unitOfWork.MaterialWarehouseCommandRepository.GetMany(
                    i =>
                        i.F34_CommandNo.Trim().Equals(aw.Id.Trim()) &&
                        i.F34_CmdSeqNo.Trim().Equals(aw.Sequence.Trim()))
                .FirstOrDefault();
            var IDS_DBOPERATIONFAIL = "The database operation failed.";
            var nStatus = tm34.F34_Status[0];
            var nRetCode = 0;
            var nRetryCount = tm34.F34_RetryCount;
            switch (ConvertHelper.ToInteger(aw.Sequence))
            {
                case 0:
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName,
                        "Error message comes from AW!(Error Command)");

                    break;
                case 10: // accepted command
                    // Added by Crystal Wu on 96/11/05 for Check Status when receive AW message
                    if (nStatus != '1' &&
                        nStatus != '4' &&
                        nStatus != '5')
                    {
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehouseMaterialScreenName,
                            "Error message comes from AW!(Status Error)");
                        return;
                    }
                    // end of added
                    //m_pDlg->RemoveAcceptTimer();
                    switch (ConvertHelper.ToInteger(aw.Status))
                    {
                        case 0: // OK
                            //sprintf(szTmpStr, "%04.4d", TC_MID_CmdEnd);
                            //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                            // update status to 3(accepted)
                            nRetCode = SetMtrCmdSts(aw.Id, aw.Sequence, 3, aw.Status);
                            //m_pDlg -> ProcessDataBase();		// process next command
                            break;
                        case 5000: // Retry Unlimitly.
                            // Adde by Wu.Jing
                            nRetCode = SetMtrCmdSts(aw.Id, aw.Sequence, 5, aw.Status);
                            // end of added
                            //m_pDlg->SetResendTimer();
                            break;
                        case 1: // Set AutoWare House to error status
                        case 2:
                        case 1001:
                        case 1002:
                        case 1003:
                        case 1004:
                            if (CommandCancel(aw.Id, aw.Command)) AwError(deviceCode);
                            else
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseMaterialScreenName,
                                    IDS_DBOPERATIONFAIL);
                            break;
                        // added by Crystal WU on 96/11/05 for 1005 
                        case 1005:
                            // cancel the command, and set aw status to offline
                            if (CommandCancel(aw.Id, aw.Command)) AwOffLine(deviceCode);
                            else
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseMaterialScreenName,
                                    IDS_DBOPERATIONFAIL);
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

                            if (CommandCancel(aw.Id, aw.Command))
                            {
                                //sprintf(szTmpStr, "%04.4d", TC_MID_CmdCancel);
                                //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                            }
                            else
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseMaterialScreenName,
                                    IDS_DBOPERATIONFAIL);
                            }

                            break;
                        case 6: // Retry three times and set the autoware house error.
                        case 15:
                            nRetCode = SetMtrCmdSts(aw.Id, aw.Sequence, 5, aw.Status, 1);
                            if (nRetryCount >= 3)
                                if (CommandCancel(aw.Id, aw.Command)) AwError(deviceCode);
                                else
                                    _notificationService.SendNoteInformation(null,
                                        _notificationService.AutomatedWarehouseMaterialScreenName,
                                        IDS_DBOPERATIONFAIL);
                            break;
                        case 4: // Retry three times, cancel this command,process next command
//		case 5:       deleted by Crystal Wu on 96/11/05
                        case 8:
                        case 10:
                            nRetCode = SetMtrCmdSts(aw.Id, aw.Sequence, 5, aw.Status, 1);
                            if (nRetryCount >= 3)
                                if (CommandCancel(aw.Id, aw.Command))
                                {
                                    //sprintf(szTmpStr, "%04.4d", TC_MID_CmdCancel);
                                    //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                    //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                                }
                                else
                                {
                                    _notificationService.SendNoteInformation(null,
                                        _notificationService.AutomatedWarehouseMaterialScreenName,
                                        IDS_DBOPERATIONFAIL);
                                }
                            break;
                        default:
                            var message = string.Format("unexpected errcode {0} from AW client", aw.Status);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseMaterialScreenName,
                                message);

                            break;
                    }
                    break;
                case 100: // command ended
                    // Added by Crystal Wu on 96/11/05 for Check Status when receive AW message
                    if (nStatus != '3' &&
                        nStatus != '4' &&
                        nStatus != '5')
                    {
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehouseMaterialScreenName,
                            "Error message comes from AW!(Status Error)");
                        return;
                    }
                    // end of added
                    switch (ConvertHelper.ToInteger(aw.Status))
                    {
                        case 0: // OK
                            if (EndMaterialWarehouseCommand(aw.Id, aw.Command))
                            {
                                //sprintf(szTmpStr, "%04.4d", TC_MID_CmdEnd);
                                //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                            }
                            else
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseMaterialScreenName,
                                    IDS_DBOPERATIONFAIL);
                            }
                            break;

                        case 60: // storage 2 times
                        case 61:
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseMaterialScreenName,
                                "Storage error:storage 2 times.");

                            if (CommandCancel(aw.Id, aw.Command, 2))
                            {
                                //sprintf(szTmpStr, "%04.4d", TC_MID_ReStoraged);
                                //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                            }
                            else
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseMaterialScreenName,
                                    IDS_DBOPERATIONFAIL);
                            }
                            break;

                        case 64: // empty retrieve
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseMaterialScreenName,
                                "Retrieve error:empty retrieve.");
                            if (CommandCancel(aw.Id, aw.Command, 3)) AwError(deviceCode);
                            else
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseMaterialScreenName,
                                    IDS_DBOPERATIONFAIL);
                            break;

                        default:
                            var message = string.Format("Error ! errcode{0}", aw.Status);

                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseMaterialScreenName,
                                message);

                            if (CommandCancel(aw.Id, aw.Command))
                            {
                                //sprintf(szTmpStr, "%04.4d", TC_MID_CmdCancel);
                                //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                            }
                            else
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseMaterialScreenName,
                                    IDS_DBOPERATIONFAIL);
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
                            _notificationService.AutomatedWarehouseMaterialScreenName,
                            "Error message comes from AW!(Status Error)");

                        return;
                    }
                    // end of added
                    switch (ConvertHelper.ToInteger(aw.Status))
                    {
                        case 64: // empty retrieve
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseMaterialScreenName,
                                "Retrieve error:empty retrieve.");

                            if (CommandCancel(aw.Id, aw.Command, 3)) AwError(deviceCode);
                            else
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseMaterialScreenName,
                                    IDS_DBOPERATIONFAIL);
                            break;
                        default:
                            if (CommandCancel(aw.Id, aw.Command))
                            {
                                //sprintf(szTmpStr, "%04.4d", TC_MID_CmdCancel);
                                //memcpy(sMsgToA.head.mid, szTmpStr, 4);
                                //m_pDlg -> SendMessageToA(strTermNo, &sMsgToA, sizeof(sMsgToA));
                            }
                            else
                            {
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseMaterialScreenName,
                                    IDS_DBOPERATIONFAIL);
                            }
                            break;
                    }
                    break;

                default:
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName,
                        "Error message comes from AW!(Error Command)");

                    break;
            }
        }

        #endregion

        private bool CommandCancel(string commandNo, string cmdSeqNo, int nHowCancel = 0,
            bool nConveyorErr = false)
        {
            var materialWhcmd = _unitOfWork.MaterialWarehouseCommandRepository.GetMtrWhsCmdByKey(commandNo, cmdSeqNo);
            var nStrRtrType = ConvertHelper.ToInteger(materialWhcmd.F34_StrRtrType);
            string lpConveyor, lpLocation = "";
            int nShelfSts, nStkFlg;
            var nRetCode = 0;
            if (materialWhcmd == null)
                return false;
            if (ConvertHelper.ToInteger(materialWhcmd.F34_Status) >
                ConvertHelper.ToInteger(Constants.TC_CMDSTS.TC_CMDSTS_5))
                return false;
            string message;
            switch (ConvertHelper.ToInteger(commandNo))
            {
                case 1000: // storage		command
                case 1001: // restorage	command
                    lpConveyor = materialWhcmd.F34_From;
                    lpLocation = materialWhcmd.F34_To;
                    if (nHowCancel == 2)
                    {
                        message = string.Format("CANCEL: Storage 2 times.  From: {0} To: {1}", materialWhcmd.F34_From,
                            materialWhcmd.F34_To);
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehouseMaterialScreenName, message);

                        // 2 times storage
                        nShelfSts = 6;
                        if (nStrRtrType == 0)
                            nStkFlg = 0;
                        else
                            nStkFlg = -1;
                    }
                    else
                    {
                        switch (nStrRtrType)
                        {
                            case 0: // material

                                message = string.Format("CANCEL: Material Storage  From: {0} To: {1}",
                                    materialWhcmd.F34_From, materialWhcmd.F34_To);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseMaterialScreenName, message);

                                nShelfSts = 0;
                                nStkFlg = 0;
                                break;
                            case 1: // empty pallet
                            case 2: // provider's pallet

                                message = string.Format("CANCEL: Pallet Storage  From: {0} To: {1}",
                                    materialWhcmd.F34_From, materialWhcmd.F34_To);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseMaterialScreenName, message);

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
                    lpConveyor = materialWhcmd.F34_To;
                    lpLocation = materialWhcmd.F34_From;
                    switch (nStrRtrType)
                    {
                        case 0: // material
                            message = string.Format("CANCEL: Material Retrieve  From: {0} To: {1}",
                                materialWhcmd.F34_From, materialWhcmd.F34_To);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseMaterialScreenName, message);

                            nShelfSts = 3;
                            nStkFlg = 3;
                            break;
                        case 1: // empty pallet
                            message = string.Format("CANCEL: Empty Pallet Retrieve  From: {0} To: {1}",
                                materialWhcmd.F34_From, materialWhcmd.F34_To);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseMaterialScreenName, message);

                            nShelfSts = 1;
                            nStkFlg = -1;
                            break;
                        case 2: // provider's pallet retrieve (front)

                            message = string.Format("CANCEL: Provider's Pallet Retrieve  From: {0} To: {1}",
                                materialWhcmd.F34_From, materialWhcmd.F34_To);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseMaterialScreenName, message);

                            nShelfSts = 2;
                            nStkFlg = -1;
                            break;
                        case 3: // provider's pallet retrieve (side)
                            message = string.Format("CANCEL: Provider's Pallet Retrieve  From: {0} To: {1}",
                                materialWhcmd.F34_From, materialWhcmd.F34_To);
                            _notificationService.SendNoteInformation(null,
                                _notificationService.AutomatedWarehouseMaterialScreenName, message);

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
                    lpConveyor = materialWhcmd.F34_From;
                    if (nConveyorErr)
                        nRetCode = SetConveyorSts(lpConveyor, 9);
                    else
                        nRetCode = SetConveyorSts(lpConveyor, 0);
                    lpConveyor = materialWhcmd.F34_To;

                    message = string.Format("CANCEL: Material Move  From: {0} To: {1}", materialWhcmd.F34_From,
                        materialWhcmd.F34_To);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName, message);

                    break;

                case 4000:
                case 5000:
                    // don't know how to do here
                    return false;
                case 6000: // stock taking command	(storage )
                    lpConveyor = materialWhcmd.F34_From;
                    lpLocation = materialWhcmd.F34_To;
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

                    message = string.Format("CANCEL: TANAONOSI Storage  From: {0} To: {1}", materialWhcmd.F34_From,
                        materialWhcmd.F34_To);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName, message);

                    break;
                case 7000: // stock taking command (retrieve)
                    lpConveyor = materialWhcmd.F34_To;
                    lpLocation = materialWhcmd.F34_From;
                    nShelfSts = 3;
                    nStkFlg = 3;

                    message = string.Format("CANCEL: TANAONOSI  Retrieve  From: {0} To: {1}", materialWhcmd.F34_From,
                        materialWhcmd.F34_To);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName, message);

                    break;
                default:
                    // not supported in this version
                    return false;
            }

            switch (nHowCancel)
            {
                case 0: // normal
                    // update status to 7
                    nRetCode = SetMtrCmdSts(materialWhcmd.F34_CommandNo, materialWhcmd.F34_CmdSeqNo, 7,
                        "0000", 0, true);
                    break;
                case 1: // manual
                    // update status to 7
                    nRetCode = SetMtrCmdSts(materialWhcmd.F34_CommandNo, materialWhcmd.F34_CmdSeqNo, 7,
                        "0000", 0, true);
                    // nRetCode = C1_AMakeHistory(materialWhcmd->id, materialWhcmd->ser, &nErrCode);
                    break;
                case 2: // storage 2 times
                    nRetCode = SetMtrCmdSts(materialWhcmd.F34_CommandNo, materialWhcmd.F34_CmdSeqNo, 8,
                        "0000", 0, true);
                    break;
                case 3: // empty retrieve
                    nRetCode = SetMtrCmdSts(materialWhcmd.F34_CommandNo, materialWhcmd.F34_CmdSeqNo, 9,
                        "0000", 0, true);
                    // modified by Crystal WU on 96/11/01 set status to prohibit
                    //nShelfSts = 0;
                    nShelfSts = 6;
                    // end of modified
                    if ((materialWhcmd.F34_CommandNo == Constants.F34_CommandNo.Retrieval.ToString("D") ||
                         materialWhcmd.F34_CommandNo == Constants.F34_CommandNo.ReRetrieve.ToString("D") ||
                         materialWhcmd.F34_CommandNo == Constants.F34_CommandNo.StockTakingOff.ToString("D")) &&
                        nStrRtrType == 0)
                        nStkFlg = 0;
                    else
                        nStkFlg = -1;
                    break;
            }
            if (nRetCode <= 0)
            {
                message = string.Format("Set Material Command Status Failed,Message ID: {0}, SEQ: {1}",
                    materialWhcmd.F34_CommandNo, materialWhcmd.F34_CmdSeqNo);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseMaterialScreenName,
                    message);
            }

            if (nConveyorErr)
            {
                // Set the conveyor status to error
                nRetCode = SetConveyorSts(lpConveyor, 9);
            }
            else
            {
                // restore conveyor number and status
                if (materialWhcmd.F34_CommandNo != Constants.F34_CommandNo.TwoTimesIn.ToString("D"))
                    nRetCode = SetConveyorSts(lpConveyor, 0);
            }
            if (nRetCode <= 0)
            {
                message = string.Format("Recover Conveyor status failed. {0}", lpConveyor);
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseMaterialScreenName,
                    message);
            }

            // update shelf status of material
            if (nShelfSts >= 0)
            {
                nRetCode = SetShelfSts(lpLocation, nShelfSts, -1, nHowCancel);
                if (nRetCode <= 0)
                {
                    message = string.Format("Set Shelf Status failed,LOCATION: {0}", lpLocation);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName, message);
                }
            }

            // update material stock flag
            if (nStkFlg >= 0)
            {
                nRetCode = SetStockFlag(materialWhcmd.F34_PalletNo, nStkFlg);

                if (nRetCode <= 0)
                {
                    message = string.Format("Set Stock Flag failed,PALLET: {0}", materialWhcmd.F34_PalletNo);
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName, message);
                }
            }
            _unitOfWork.Commit();
            return true;
        }

        private void ProcessDataList(IEnumerable<TX34_MtrWhsCmd> lstTx34, string deviceCode)
        {
            int nStrRtrSts, nRetCode = 0;
            int nFlag;
            var lpConveyor = "";
            int pCmd;
            int nStrRtrType;

            string message;
            //  SetResendTimer();
            foreach (var tx34 in lstTx34.ToList())
            {
                var pSeq = tx34.F34_CmdSeqNo;
                pCmd = ConvertHelper.ToInteger(tx34.F34_CommandNo);
                nStrRtrType = ConvertHelper.ToInteger(tx34.F34_StrRtrType);

                // Status is empty.
                if (string.IsNullOrEmpty(tx34.F34_Status))
                    continue;

                // Trim the status.
                var status = tx34.F34_Status.Trim();

                switch (status[0])
                {
                    case '0':
                        if (pCmd == 1000 || pCmd == 1001)
                        {
                            nStrRtrSts = 1;
                            if (nStrRtrType == 0)
                                nFlag = 1; // Material 
                            else
                                nFlag = -1; // Empty Pallet , Suppiler Pallet.
                            lpConveyor = tx34.F34_From;
                            // added by Crystal Wu for 2 times storage need not to check conveyor status
                            if (pCmd == 1001)
                                nStrRtrSts = -1;
                            // end of added
                        }
                        else if (pCmd == 2000 || pCmd == 2001)
                        {
                            nStrRtrSts = 2;
                            if (nStrRtrType == 0)
                                nFlag = 2; // Material
                            else
                                nFlag = -1;
                            lpConveyor = tx34.F34_To;
                        }
                        else if (pCmd == 3000)
                        {
                            // move between floors
                            nStrRtrSts = 3;
                            nFlag = -1; // not need to update shelf status
                            lpConveyor = tx34.F34_From;
                            nRetCode = SetConveyorSts(lpConveyor, nStrRtrSts);
                            if (nRetCode <= 0)
                            {
                                message = string.Format("Conveyor {0} status can not permit to send message",
                                    lpConveyor);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseMaterialScreenName, message);

                                break;
                            }
                            lpConveyor = tx34.F34_To;
                        }
                        else if (pCmd == 6000)
                        {
                            nStrRtrSts = -1;
                            nFlag = -1;
                            lpConveyor = tx34.F34_From;
                        }
                        else if (pCmd == 7000)
                        {
                            nStrRtrSts = 2;
                            nFlag = -1;
                            lpConveyor = tx34.F34_To;
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
                                message = string.Format("Conveyor {0} status can not permit to send message",
                                    lpConveyor);
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseMaterialScreenName, message);

                                if (pCmd == 3000)
                                {
                                    lpConveyor = tx34.F34_From;
                                    nRetCode = SetConveyorSts(lpConveyor, 0);
                                    if (nRetCode <= 0)
                                        string.Format("Recover conveyor {0} status failed",
                                            lpConveyor);
                                }

                                break;
                            }
                        }

                        nRetCode = _notificationService.SendFromC1ToAw(tx34);
                        if (nRetCode >= 0)
                        {
                            // send the message OK
                            SetMtrCmdSts(pCmd.ToString(), pSeq, 1, "0000", 1);
                            // SetAcceptTimer();
                        }
                        else
                        {
                            // send the message NG
                            SetMtrCmdSts(pCmd.ToString(), pSeq, 2, "0000", 1);

                            if (nStrRtrSts >= 0)
                            {
                                nRetCode = SetConveyorSts(lpConveyor, 0);
                                if (nRetCode <= 0)
                                {
                                    message = "Recover conveyor status failed";
                                    _notificationService.SendNoteInformation(null,
                                        _notificationService.AutomatedWarehouseMaterialScreenName, message);
                                }
                            }
                            if (pCmd == 3000)
                            {
                                lpConveyor = tx34.F34_From;
                                nRetCode = SetConveyorSts(lpConveyor, 0);
                                if (nRetCode <= 0)
                                {
                                    message = "Recover conveyor status failed";
                                    _notificationService.SendNoteInformation(null,
                                        _notificationService.AutomatedWarehouseMaterialScreenName, message);
                                }
                            }

                            //SetResendTimer();
                        }

                        // update material shelf status
                        if (nFlag >= 0)
                            SetStockFlag(tx34.F34_PalletNo, nFlag);
                        return;
                    case '1': // send OK but no accept message
                    case '2': // send NG
                        if (tx34.F34_RetryCount >= 3)
                        {
                            // 3 times
                            if (tx34.F34_Status == "1")
                            {
                                // 3 times,receive no accept
                                var m_nAWstatus = 2;
                                _unitOfWork.DeviceRepository.UpdateStatus(deviceCode, m_nAWstatus);

                                message = "Autoware House is at error status..";
                                _notificationService.SendNoteInformation(null,
                                    _notificationService.AutomatedWarehouseMaterialScreenName, message);
                            }
                            SetMtrCmdSts(pCmd.ToString(), pSeq, 1, "0000", -1);
                        }
                        else
                        {
                            nRetCode = _notificationService.SendFromC1ToAw(tx34);
                            if (nRetCode >= 0)
                            {
                                // AW message send success
                                SetMtrCmdSts(pCmd.ToString(), pSeq, 1, "0000", 1);
                                //  SetAcceptTimer();
                                return;
                            }
                            // AW message send failure.
                            SetMtrCmdSts(pCmd.ToString(), pSeq, 2, "0000", 1);
                            // SetResendTimer();
                        }
                        return;
                    case '3':
                        // do nothing
                        break;
                    case '4':
                        nRetCode = _notificationService.SendFromC1ToAw(tx34);
                        if (nRetCode >= 0)
                        {
                            SetMtrCmdSts(pCmd.ToString(), pSeq, 4, "0000", 1);
                            //	m_nTidAccept = SetTimer(TID_ACCEPT, m_nAcceptTime, NULL);
                            //SetAcceptTimer();
                            return;
                        }
                        nRetCode = SetMtrCmdSts(pCmd.ToString(), pSeq, 2, "0000", 1);

                        // SetResendTimer();
                        break;
                    case '5':
                        nRetCode = _notificationService.SendFromC1ToAw(tx34);
                        if (nRetCode >= 0)
                        {
                            SetMtrCmdSts(pCmd.ToString(), pSeq, 5, "0000", 1);
                            // SetAcceptTimer();
                            return;
                        }
                        nRetCode = SetMtrCmdSts(pCmd.ToString(), pSeq, 2, "0000", 1);

                        // SetResendTimer();
                        break;
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        // send the message to A side
                        var sMsgToA = new MessageItem();
                        switch (tx34.F34_Status)
                        {
                            case "6":
                                sMsgToA.Id = Constants.MessageId.TC_MID_CmdEnd.ToString("D");
                                break;
                            case "7":
                                sMsgToA.Id = Constants.MessageId.TC_MID_CmdCancel.ToString("d");

                                break;
                            case "8":
                                sMsgToA.Id = Constants.MessageId.TC_MID_ReStoraged.ToString("D");
                                break;
                            case "9":
                                sMsgToA.Id = Constants.MessageId.TC_MID_EmptyRetrieve.ToString("D");

                                break;
                        }
                        sMsgToA.TerminalNo = tx34.F34_TerminalNo;
                        sMsgToA.PictureNo = tx34.F34_PictureNo;

                        // send the message to A side to inform cancel operation.
                        _notificationService.SendMessageFromC1(new[] {sMsgToA.TerminalNo}, sMsgToA);
                        break;
                    case 'A': // impossible.
                        //	nRetCode = C1_CommandEnd(&(tx34 -> cmd), tx34 -> nStrRtrType, tx34 -> sTermNo,1);
                        break;

                    case 'B': // impossible.
                        //	nRetCode = C1_CommandCancel(&(tx34 -> cmd), tx34 -> nStrRtrType, 1);
                        if (pCmd == 1000 && nStrRtrType == 0)
                            DelStock(tx34.F34_PalletNo);
                        //added by Crystal Wu on 96/11/06 for 
                        // can not find a empty location for re-assign when 2 times storage
                        var nAWstatus = 2;
                        _unitOfWork.DeviceRepository.UpdateStatus("00000", nAWstatus);

                        message = "Autoware House is at error status..";
                        _notificationService.SendNoteInformation(null,
                            _notificationService.AutomatedWarehouseMaterialScreenName, message);

                        return;
                    // end of added
                    case 'C':
                        break;
                    case 'D':
                        if (pCmd == 1000 && nStrRtrType == 0)
                            nRetCode = DelStock(tx34.F34_PalletNo);
                        break;
                }
            }
            _unitOfWork.Commit();
        }

        #region Constructor

        /// <summary>
        ///     Service which broadcast notification from domain to client.
        /// </summary>
        private readonly INotificationService _notificationService;

        /// <summary>
        ///     Domain which handles common business.
        /// </summary>
        private readonly ICommonDomain _commonDomain;
        
        private readonly IConfigurationService _configurationService;

        /// <summary>
        ///     Initiate domain with IoC.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="notificationService"></param>
        /// <param name="commonDomain"></param>
        /// <param name="log"></param>
        public Communication1Domain(IUnitOfWork unitOfWork,
            INotificationService notificationService,
            ICommonDomain commonDomain,
            ILog log)
            : base(unitOfWork)
        {
            _notificationService = notificationService;
            _commonDomain = commonDomain;
        }

        #endregion

        #region private method

        private int DelStock(string lpPallet)
        {
            // Delete unused data from STOCK table
            try
            {
                _unitOfWork.MaterialShelfStockRepository.Delete(i => i.F33_PalletNo.Trim().Equals(lpPallet.Trim()));
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                //to do "Error occured when delete stock table"
                return -1;
            }


            return 0;
        }

        private void MakeHistory()
        {
            try
            {
                var lstPdtWhsCmd = _unitOfWork.MaterialWarehouseCommandRepository
                    .GetMany(i => i.F34_Status.Trim().Equals(Constants.F34_Status.statusA) ||
                                  i.F34_Status.Trim().Equals(Constants.F34_Status.statusB) ||
                                  i.F34_Status.Trim().Equals(Constants.F34_Status.statusC) ||
                                  i.F34_Status.Trim().Equals(Constants.F34_Status.statusD) ||
                                  i.F34_Status.Trim().Equals(Constants.F34_Status.statusE) ||
                                  i.F34_Status.Trim().Equals(Constants.F34_Status.statusF));
                foreach (var tx34 in lstPdtWhsCmd)
                {
                    _unitOfWork.MaterialWarehouseCommandHistoryRepository.AddByMaterialCmd(tx34);
                    _unitOfWork.MaterialWarehouseCommandRepository.Delete(tx34);
                }
                if (!lstPdtWhsCmd.Any())
                {
                    //var message = "Error occured when Make history";
                    //_notificationService.SendNoteInformation(null,
                    //    _notificationService.AutomatedWarehouseMaterialScreenName, message);
                }
                _unitOfWork.Commit();
            }
            catch (Exception exception)
            {
                var message = exception.Message;
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseMaterialScreenName,
                    message);
            }
        }

        private int SetConveyorSts(string conveyorCode, int nStatus)
        {
            var nRecordCount = 1;
            if (nStatus < 0) return 0;
            // restore conveyor number and status
            if (nStatus == 1 || nStatus == 2)
            {
                // Storage or Retrieve send OK
                var conveyor =
                    _unitOfWork.ConveyorRepository.GetMany(i =>
                            i.F05_ConveyorCode.Trim().Equals(conveyorCode.Trim()) &&
                            i.F05_BufferUsing < i.F05_MaxBuffer).ToList()
                        .FirstOrDefault(
                            i =>
                                Convert.ToInt16(i.F05_StrRtrSts) >= 0 &&
                                Convert.ToInt16(i.F05_StrRtrSts) <= nStatus);

                if (conveyor == null)
                    return -1;
                conveyor.F05_StrRtrSts = nStatus.ToString();
                conveyor.F05_BufferUsing = conveyor.F05_BufferUsing + 1;
                conveyor.F05_UpdateDate = DateTime.Now;
                _unitOfWork.ConveyorRepository.Update(conveyor);
            }
            else if (nStatus == 3)
            {
                // Floor Movement send OK
                var conveyor =
                    _unitOfWork.ConveyorRepository.GetMany(
                        i =>
                            i.F05_ConveyorCode.Trim().Equals(conveyorCode.Trim()) &&
                            i.F05_BufferUsing < i.F05_MaxBuffer &&
                            i.F05_StrRtrSts == "0").FirstOrDefault();

                if (conveyor != null)
                {
                    conveyor.F05_BufferUsing = conveyor.F05_BufferUsing + 1;
                    conveyor.F05_UpdateDate = DateTime.Now;
                    conveyor.F05_StrRtrSts = nStatus.ToString();
                    _unitOfWork.ConveyorRepository.Update(conveyor);
                }
            }
            else if (nStatus == 0)
            {
                // Command End or Command Cancel
                try
                {
                    var conveyor =
                        _unitOfWork.ConveyorRepository.GetMany(
                                i => i.F05_ConveyorCode.Trim().Equals(conveyorCode) && i.F05_BufferUsing > 0)
                            .FirstOrDefault();

                    if (conveyor != null)
                    {
                        conveyor.F05_BufferUsing = conveyor.F05_BufferUsing - 1;
                        conveyor.F05_UpdateDate = DateTime.Now;
                        if (conveyor.F05_BufferUsing == 0)
                            conveyor.F05_StrRtrSts = Constants.F05_StrRtrSts.NotUse.ToString("D");
                        _unitOfWork.ConveyorRepository.Update(conveyor);
                    }
                    else
                    {
                        nRecordCount = -1;
                    }
                }
                catch (Exception)
                {
                    nRecordCount = -1;
                }
            }
            else
            {
                try
                {
                    // If status == 9 then this conveyor is in error status
                    var conveyor =
                        _unitOfWork.ConveyorRepository.GetMany(
                            i => i.F05_ConveyorCode.Trim().Equals(conveyorCode)).FirstOrDefault();
                    if (conveyor != null)
                    {
                        conveyor.F05_StrRtrSts = Constants.F05_StrRtrSts.Error.ToString("D");
                        conveyor.F05_UpdateDate = DateTime.Now;

                        _unitOfWork.ConveyorRepository.Update(conveyor);
                    }
                    else
                    {
                        nRecordCount = -1;
                    }
                }
                catch (Exception)
                {
                    nRecordCount = -1;
                }
            }
            _unitOfWork.Commit();
            if (nRecordCount == -1)
            {
                var message = "Error occured when set Conveyor Status";
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseMaterialScreenName,
                    message);
                return -1;
            }
            if (nStatus == 0)
                return nRecordCount;
            return 1;
        }

        private int SetMtrCmdSts(string lpCmdNo, string lpSeq, int nStatus, string pAbnormalCode, int nRetry = 0,
            bool updateCommandEndDate = false)
        {
            var mtrWhsCmd = _unitOfWork.MaterialWarehouseCommandRepository.GetMtrWhsCmdByKey(lpCmdNo, lpSeq);
            if (mtrWhsCmd == null)
            {
                var message = "Error occured when set Cmd Status";
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseMaterialScreenName,
                    message);
                return -1;
            }

            var nFldRetryCount = mtrWhsCmd.F34_RetryCount ?? 0;
            var nFldStatus = ConvertHelper.ToInteger(mtrWhsCmd.F34_Status);

            if (nFldStatus == nStatus)
                nFldRetryCount += nRetry;
            else
                nFldRetryCount = nRetry;
            if (nRetry < 0) nFldRetryCount = 0;
            mtrWhsCmd.F34_Status = nStatus.ToString();
            mtrWhsCmd.F34_AbnormalCode = pAbnormalCode;
            mtrWhsCmd.F34_RetryCount = nFldRetryCount;

            mtrWhsCmd.F34_UpdateDate = DateTime.Now;
            if (updateCommandEndDate)
                mtrWhsCmd.F34_CommandEndDate = DateTime.Now;
            else
                mtrWhsCmd.F34_CommandSendDate = DateTime.Now;
            try
            {
                _unitOfWork.MaterialWarehouseCommandRepository.Update(mtrWhsCmd);
                _unitOfWork.Commit();
            }
            catch (Exception exception)
            {
                var message = "Error occured when set Cmd Status";
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseMaterialScreenName,
                    message);
                return -1;
            }

            return 1;
        }

        private int SetShelfSts(string lpLocation, int nStatus, int nTanaorosiFlg, int nReason = -1)
        {
            var nRetCode = 1;
            if (nStatus < 0) return 0;
            var lstStockTakingPre =
                _unitOfWork.MaterialShelfStatusRepository.GetAll()
                    .ToList()
                    .Where(i => i.F31_ShelfRow + i.F31_ShelfBay + i.F31_ShelfLevel == lpLocation);

            if (nStatus == 0 || nStatus == 6 || nStatus == 7)
            {
                foreach (var stockTakingPre in lstStockTakingPre)
                {
                    stockTakingPre.F31_ShelfStatus = nStatus.ToString();
                    stockTakingPre.F31_UpdateDate = DateTime.Now;
                    if (nReason != 3)
                        stockTakingPre.F31_PalletNo = string.Empty;
                    _unitOfWork.MaterialShelfStatusRepository.Update(stockTakingPre);
                }

                if (!lstStockTakingPre.Any())
                {
                    var message = "Error occured when set shelf status";
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName, message);

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

                    _unitOfWork.MaterialShelfStatusRepository.Update(stockTakingPre);
                }

                if (!lstStockTakingPre.Any())
                {
                    var message = "Error occured when set shelf status";
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName, message);
                    nRetCode = -1;
                }
            }
            _unitOfWork.Commit();
            return nRetCode;
        }

        private int SetStockFlag(string lpPallet, int nFlag)
        {
            if (nFlag < 0) return 0;
            var lstMtrShfStk =
                _unitOfWork.MaterialShelfStockRepository.GetMany(i => i.F33_PalletNo.Trim().Equals(lpPallet.Trim()));
            foreach (var tx33MtrShfStk in lstMtrShfStk)
            {
                tx33MtrShfStk.F33_StockFlag = nFlag.ToString();
                tx33MtrShfStk.F33_UpdateDate = DateTime.Now;
                _unitOfWork.MaterialShelfStockRepository.Update(tx33MtrShfStk);
            }
            if (!lstMtrShfStk.Any())
            {
                var message = "Error occured when set stock flag";
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseMaterialScreenName,
                    message);
                return -1;
            }

            _unitOfWork.Commit();
            return 1;
        }

        private int SetShelfTime(string lpPallet, int nShelfTime, string lpLocation)
        {
            try
            {
                var tx32MtrShf =
                    _unitOfWork.MaterialShelfRepository.GetMany(i => i.F32_PalletNo.Trim().Equals(lpPallet.Trim()))
                        .FirstOrDefault();

                if (tx32MtrShf != null)
                {
                    var lstMtrshfSts =
                        _unitOfWork.MaterialShelfStatusRepository.GetAll()
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
                        _unitOfWork.MaterialShelfStatusRepository.Update(tx31MtrShfStk);
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

                    _unitOfWork.MaterialShelfRepository.Update(tx32MtrShf);
                }
                else
                {
                    var message = "Error occured when set shelf time";
                    _notificationService.SendNoteInformation(null,
                        _notificationService.AutomatedWarehouseMaterialScreenName, message);
                    return -1;
                }
                _unitOfWork.Commit();
            }
            catch (Exception exception)
            {
                var message = "Error occured when set shelf time";
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseMaterialScreenName,
                    message);

                return -1;
            }
            return 1;
        }

        private int SetStgRtrHst(string lpPallet, string lpTermNo, string lpFrom, string lpTo, int nStgRtrCls,
            int nShelfTime)
        {
            // Set Material Storage-Retrieval Result

            var tX32MtrShf =
                _unitOfWork.MaterialShelfRepository.GetMany(i => i.F32_PalletNo.Trim().Equals(lpPallet.Trim()))
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

                    stgRtrDate = nShelfTime == 1 ? tX32MtrShf.F32_ReStorageDate.Value : tX32MtrShf.F32_StorageDate.Value;

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
                _unitOfWork.MaterialShelfStockRepository.GetMany(i => i.F33_PalletNo.Trim().Equals(lpPallet.Trim()))
                    .OrderBy(i => i.F33_AddDate);
            if (!lstMtrShfStk.Any())
            {
                var message = "Error occured when set history of Storage - Retrieve";
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseMaterialScreenName,
                    message);
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
                    mtrStgRtrHst.F61_PrcPdtNo = prcOrdNo;
                    mtrStgRtrHst.F61_PrtDvrNo = prtDvrNo;
                    mtrStgRtrHst.F61_StgRtrCls = nStgRtrCls.ToString();
                    mtrStgRtrHst.F61_To = lpTo;
                    mtrStgRtrHst.F61_From = lpFrom;
                    mtrStgRtrHst.F61_TerminalNo = lpTermNo;
                    mtrStgRtrHst.F61_Amount = tx33MtrShfStk.F33_Amount;
                    mtrStgRtrHst.F61_AddDate = DateTime.Now;
                    mtrStgRtrHst.F61_UpdateDate = DateTime.Now;
                    _unitOfWork.MaterialStorageRetrieveHistoryRepository.Add(mtrStgRtrHst);
                }


                _unitOfWork.Commit();
            }
            catch (Exception exception)
            {
                var message = "Error occured when set history of Storage - Retrieve";
                _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseMaterialScreenName,
                    message);
                return -1;
            }
            return 1;
        }

        private void AwError(string deviceCode)
        {
            var tm14 = _unitOfWork.DeviceRepository.GetByDeviceCode(deviceCode);
            tm14.F14_DeviceStatus = "2";
            tm14.F14_UpdateDate = DateTime.Now;
            _unitOfWork.DeviceRepository.Update(tm14);
            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseMaterialScreenName,
                "Autoware House is at error status..");
            _unitOfWork.Commit();
        }

        private void AwOffLine(string deviceCode)
        {
            var tm14 = _unitOfWork.DeviceRepository.GetByDeviceCode(deviceCode);
            tm14.F14_DeviceStatus = "1";
            tm14.F14_UpdateDate = DateTime.Now;
            _unitOfWork.DeviceRepository.Update(tm14);
            _notificationService.SendNoteInformation(null, _notificationService.AutomatedWarehouseMaterialScreenName,
                "Autoware House is at offline status..");
            _unitOfWork.Commit();
        }
    }

    #endregion
}