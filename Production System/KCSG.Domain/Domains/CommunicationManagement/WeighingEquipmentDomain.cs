using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.CommunicationManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains
{
    public class WeighingEquipmentDomain : BaseDomain, IWeighingEquipmentDomain
    {
        private INotificationService _notificationService;

        public WeighingEquipmentDomain(IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
        }

        public ResponseResult<GridResponse<WeighingEquipmentItem>> SearchByMaterial(
            WeighingEquipmentViewSearchModel model, GridSettings gridSettings)
        {

            var lstTx52 =
                _unitOfWork.MtrMsrSndCmdRepository.GetMany(i => i.F52_CommandType == "04" || i.F52_CommandType == "14");
            var lstTx68 =
                _unitOfWork.MtrMsrSndCmdHstRepository.GetMany(
                    i => i.F68_CommandType == "04" || i.F68_CommandType == "14");
            if (!string.IsNullOrEmpty(model.CodeNo))
            {
                lstTx52 = lstTx52.Where(i => i.F52_MasterCode.Trim().Contains(model.CodeNo));
            }
            if (!string.IsNullOrEmpty(model.Date))
            {
                var date = ConvertHelper.ConvertToDateTimeFull(model.Date);
                lstTx52 = lstTx52.Where(i => i.F52_AddDate == date);
            }
            if (model.LN != 0)
            {
                lstTx52 = model.LN == 1 ? lstTx52.Where(i => i.F52_MsrMacCls == "1") : lstTx52.Where(i => i.F52_MsrMacCls == "2");
            }
            if (model.SelectData == Constants.SelectData.DataOnQueue)
            {

                lstTx52 = lstTx52.Where(i => i.F52_Status == "0" || i.F52_Status == "1" || i.F52_Status == "3");
            }
            else if (model.SelectData == Constants.SelectData.SentData)
            {
                lstTx52 = lstTx52.Where(i => i.F52_Status == "2");
            }
            var lstResult = new List<WeighingEquipmentItem>();
            lstResult.AddRange(lstTx52.Select(
                  i =>
                      new WeighingEquipmentItem
                      {
                          Terminal = i.F52_TerminalNo,
                          Class = i.F52_MsrMacCls,
                          Status = i.F52_Status,
                          MasterCode = i.F52_MasterCode,
                          ErrorCode = i.F52_AbnormalCode,
                          Date = i.F52_AddDate
                      }));
            lstResult.AddRange(
                      lstTx68.Select(
                          i =>
                              new WeighingEquipmentItem()
                              {
                                  Terminal = i.F68_TerminalNo,
                                  Class = i.F68_MsrMacCls,
                                  Status = i.F68_Status,
                                  MasterCode = i.F68_MasterCode,
                                  ErrorCode = i.F68_AbnormalCode,
                                  Date = i.F68_AddDate
                              }));
            var itemCount = lstResult.Count();
            if (gridSettings != null)
            {
                var weighingEquipmentItems = lstResult.AsQueryable();
                OrderByAndPaging(ref weighingEquipmentItems, gridSettings);
                lstResult = weighingEquipmentItems.ToList();
            }


            var resultModel = new GridResponse<WeighingEquipmentItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<WeighingEquipmentItem>>(resultModel, true);

        }

        public ResponseResult<GridResponse<WeighingEquipmentItem>> SearchByPreproduct(
            WeighingEquipmentViewSearchModel model, GridSettings gridSettings)
        {
            var lstTx52 =
                _unitOfWork.MtrMsrSndCmdRepository.GetMany(i => i.F52_CommandType == "03" || i.F52_CommandType == "13");
            var lstTx68 =
                _unitOfWork.MtrMsrSndCmdHstRepository.GetMany(
                    i => i.F68_CommandType == "03" || i.F68_CommandType == "13");
            if (!string.IsNullOrEmpty(model.CodeNo))
            {
                lstTx52 = lstTx52.Where(i => i.F52_MasterCode.Trim().Contains(model.CodeNo));
            }
            if (!string.IsNullOrEmpty(model.Date))
            {
                var date = ConvertHelper.ConvertToDateTimeFull(model.Date);
                lstTx52 = lstTx52.Where(i => i.F52_AddDate == date);
            }
            if (model.LN != 0)
            {
                lstTx52 = model.LN == 1 ? lstTx52.Where(i => i.F52_MsrMacCls == "1") : lstTx52.Where(i => i.F52_MsrMacCls == "2");
            }
            if (model.SelectData == Constants.SelectData.DataOnQueue)
            {

                lstTx52 = lstTx52.Where(i => i.F52_Status == "0" || i.F52_Status == "1" || i.F52_Status == "3");
            }
            else if (model.SelectData == Constants.SelectData.SentData)
            {
                lstTx52 = lstTx52.Where(i => i.F52_Status == "2");
            }
            var lstResult = new List<WeighingEquipmentItem>();
            lstResult.AddRange(lstTx52.Select(
                    i =>
                        new WeighingEquipmentItem
                        {
                            Terminal = i.F52_TerminalNo,
                            Class = i.F52_MsrMacCls,
                            Status = i.F52_Status,
                            MasterCode = i.F52_MasterCode,
                            ErrorCode = i.F52_AbnormalCode,
                            Date = i.F52_AddDate
                        }));
            lstResult.AddRange(
                        lstTx68.Select(
                            i =>
                                new WeighingEquipmentItem()
                                {
                                    Terminal = i.F68_TerminalNo,
                                    Class = i.F68_MsrMacCls,
                                    Status = i.F68_Status,
                                    MasterCode = i.F68_MasterCode,
                                    ErrorCode = i.F68_AbnormalCode,
                                    Date = i.F68_AddDate
                                }));
            var itemCount = lstResult.Count();
            if (gridSettings != null)
            {
                var weighingEquipmentItems = lstResult.AsQueryable();
                OrderByAndPaging(ref weighingEquipmentItems, gridSettings);
                lstResult = weighingEquipmentItems.ToList();
            }


            var resultModel = new GridResponse<WeighingEquipmentItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<WeighingEquipmentItem>>(resultModel, true);
        }

        public ResponseResult<GridResponse<WeighingEquipmentItem>> SearchByRetrieval(
            WeighingEquipmentViewSearchModel model, GridSettings gridSettings)
        {
            var lstTx54 =
                _unitOfWork.MtrRtrMsrSndCmdRepository.GetAll();
            var lstTx69 =
                _unitOfWork.MtrRtrMsrSndCmdHstRepository.GetAll();
            if (!string.IsNullOrEmpty(model.CodeNo))
            {
                lstTx54 = lstTx54.Where(i => i.F54_MaterialCode.Trim().Contains(model.CodeNo));
            }
            if (!string.IsNullOrEmpty(model.Date))
            {
                var date = ConvertHelper.ConvertToDateTimeFull(model.Date);
                lstTx54 = lstTx54.Where(i => i.F54_MtrRtrDate == date);
            }
            if (model.LN != 0)
            {

                lstTx54 = model.LN == 1
                    ? lstTx54.Where(i => i.F54_MsrMacClass == "1")
                    : lstTx54.Where(i => i.F54_MsrMacClass == "2");
            }
            if (model.SelectData == Constants.SelectData.DataOnQueue)
            {

                lstTx54 = lstTx54.Where(i => i.F54_Status == "0" || i.F54_Status == "1" || i.F54_Status == "3");
            }
            else if (model.SelectData == Constants.SelectData.SentData)
            {
                lstTx54 = lstTx54.Where(i => i.F54_Status == "2");
            }
            var lstResult = new List<WeighingEquipmentItem>();
            lstResult.AddRange(lstTx54.Select(
                    i =>
                        new WeighingEquipmentItem
                        {
                            MaterialCode = i.F54_MaterialCode,
                            PalletNo = i.F54_PalletNo,
                            Class = i.F54_MsrMacClass,
                            SendFlag = i.F54_Status,
                            AbnormalCode = i.F54_AbnormalCode,
                            Date = i.F54_MtrRtrDate
                        }));
            lstResult.AddRange(
                        lstTx69.Select(
                            i =>
                                new WeighingEquipmentItem()
                                {
                                    MaterialCode = i.F69_MaterialCode,
                                    PalletNo = i.F69_PalletNo,
                                    Class = i.F69_MsrMacClass,
                                    SendFlag = i.F69_Status,
                                    AbnormalCode = i.F69_AbnormalCode,
                                    Date = i.F69_MtrRtrDate
                                }));
            var itemCount = lstResult.Count();
            //if (gridSettings != null)
            //{

            //    var weighingEquipmentItems = lstResult.AsQueryable();
            //    OrderByAndPaging(ref weighingEquipmentItems, gridSettings);
            //    lstResult = weighingEquipmentItems.ToList();
            //}

            var pageIndex = gridSettings.PageIndex - 1;
            lstResult =
                lstResult.OrderBy(x => x.MaterialCode)
                    .Skip(pageIndex * gridSettings.PageSize)
                    .Take(gridSettings.PageSize).ToList();
            var resultModel = new GridResponse<WeighingEquipmentItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<WeighingEquipmentItem>>(resultModel, true);
        }

        public ResponseResult<GridResponse<WeighingEquipmentItem>> SearchByKndCommand(
            WeighingEquipmentViewSearchModel model, GridSettings gridSettings)
        {
            // Find kneading msr snd.
            var kndCmdMsrSnds =
                _unitOfWork.KndCmdMsrSndRepository.GetAll();

            // Find all kneading commands.
            var kneadingCommands = _unitOfWork.KneadingCommandRepository.GetAll();

            // Find command history.
            var kndCmdMsrSndHsts =
                _unitOfWork.KndCmdMsrSndHstRepository.GetAll();

            // Find kneading command msr snds.
            if (!string.IsNullOrEmpty(model.CodeNo))
                kndCmdMsrSnds = kndCmdMsrSnds.Where(i => i.F55_PrePdtLotNo.Trim().Contains(model.CodeNo));

            if (!string.IsNullOrEmpty(model.Date))
            {
                var date = ConvertHelper.ConvertToDateTimeFull(model.Date);
                kneadingCommands = kneadingCommands.Where(i => i.F42_KndEptBgnDate == date);
            }

            if (model.LN != 0)
            {

                kndCmdMsrSnds = model.LN == 1
                    ? kndCmdMsrSnds.Where(i => i.F55_MsrSndCls == "1")
                    : kndCmdMsrSnds.Where(i => i.F55_MsrSndCls == "2");

            }
            if (model.SelectData == Constants.SelectData.DataOnQueue)
            {
                kndCmdMsrSnds =
                    kndCmdMsrSnds.Where(i => i.F55_Status == "0" || i.F55_Status == "1" || i.F55_Status == "3");

                kndCmdMsrSndHsts = kndCmdMsrSndHsts.Where(i => i.F62_Status == "0" || i.F62_Status == "1" || i.F62_Status == "3");
            }

            else if (model.SelectData == Constants.SelectData.SentData)
            {
                kndCmdMsrSnds = kndCmdMsrSnds.Where(i => i.F55_Status == "2");
                kndCmdMsrSndHsts = kndCmdMsrSndHsts.Where(i => i.F62_Status == "2");
            }


            // Find kneading commands list.
            var commandsList = from kneadingCommand in kneadingCommands
                               join kndCmdMsrSnd in kndCmdMsrSnds
                               on kneadingCommand.F42_KndCmdNo equals kndCmdMsrSnd.F55_KndCmdNo
                               where kneadingCommand.F42_PrePdtLotNo.Trim().Equals(kndCmdMsrSnd.F55_PrePdtLotNo.Trim())
                               select new
                               {
                                   kneadingCommand,
                                   kndCmdMsrSnd
                               };


            //var masterCommandsList = from th62 in kndCmdMsrSndHsts
            //                     join tx42 in kneadingCommands on
            //                         new
            //                         {
            //                             kndNo = th62.F62_KndCmdNo,
            //                             prepreductCode = th62.F62_PrePdtCode,
            //                             prePdtLotNo = th62.F62_PrePdtLotNo
            //                         } equals
            //                         new
            //                         {
            //                             kndNo = tx42.F42_KndCmdNo,
            //                             prepreductCode = tx42.F42_PreProductCode,
            //                             prePdtLotNo = tx42.F42_PrePdtLotNo
            //                         }
            //                     select new { tx42, th62 };

            var masterCommandsList = from kndCmdMsrSndHst in kndCmdMsrSndHsts
                                     from kneadingCommand in kneadingCommands
                                     where kndCmdMsrSndHst.F62_KndCmdNo.Equals(kneadingCommand.F42_KndCmdNo)
                                     && kndCmdMsrSndHst.F62_PrePdtCode.Equals(kneadingCommand.F42_PreProductCode)
                                     && kndCmdMsrSndHst.F62_PrePdtLotNo.Equals(kneadingCommand.F42_PrePdtLotNo)
                                     select new
                                     {
                                         kneadingCommand,
                                         kndCmdMsrSndHst
                                     };


            var lstResult = new List<WeighingEquipmentItem>();
            lstResult.AddRange(commandsList.Select(
                  i =>
                      new WeighingEquipmentItem
                      {
                          CommandNo = i.kndCmdMsrSnd.F55_KndCmdNo,
                          PreproductLotNo = i.kndCmdMsrSnd.F55_PrePdtLotNo,
                          Class = i.kndCmdMsrSnd.F55_MsrSndCls,
                          PreproductCode = i.kndCmdMsrSnd.F55_PrePdtCode,
                          AbnormalCode = i.kndCmdMsrSnd.F55_AbnormalCode,
                          Status = i.kndCmdMsrSnd.F55_Status,
                          Date = i.kneadingCommand.F42_KndEptBgnDate
                      }));

            lstResult.AddRange(
                      masterCommandsList.Select(
                          i =>
                              new WeighingEquipmentItem()
                              {
                                  CommandNo = i.kneadingCommand.F42_KndCmdNo,
                                  PreproductLotNo = i.kneadingCommand.F42_PrePdtLotNo,
                                  Class = i.kndCmdMsrSndHst.F62_MsrSndCls,
                                  PreproductCode = i.kneadingCommand.F42_PreProductCode,
                                  AbnormalCode = i.kndCmdMsrSndHst.F62_AbnormalCode,
                                  Status = i.kndCmdMsrSndHst.F62_Status,
                                  Date = i.kneadingCommand.F42_KndEptBgnDate
                              }));


            var itemCount = lstResult.Count();
            if (gridSettings != null)
            {
                var weighingEquipmentItems = lstResult.AsQueryable();
                OrderByAndPaging(ref weighingEquipmentItems, gridSettings);
                lstResult = weighingEquipmentItems.ToList();
            }

            var resultModel = new GridResponse<WeighingEquipmentItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<WeighingEquipmentItem>>(resultModel, true);
        }


        public ResponseResult<GridResponse<WeighingEquipmentItem>> SearchByKndResult(
            WeighingEquipmentViewSearchModel model, GridSettings gridSettings)
        {
            var lstTx43 =
                _unitOfWork.KneadingRecordRepository.GetAll();
            var lstTx42 =
                _unitOfWork.KneadingCommandRepository.GetAll();
            var lstTm03 = _unitOfWork.PreProductRepository.GetAll();
            if (!string.IsNullOrEmpty(model.CodeNo))
            {
                lstTx43 = lstTx43.Where(i => i.F43_PrePdtLotNo.Trim().Contains(model.CodeNo));
            }
            if (!string.IsNullOrEmpty(model.Date))
            {
                var date = ConvertHelper.ConvertToDateTimeFull(model.Date);
                lstTx43 = lstTx43.Where(i => i.F43_BthBgnDate == date);
            }
            if (model.LN != 0)
            {

                lstTm03 = model.LN == 1
                    ? lstTm03.Where(i => i.F03_KneadingLine == "1")
                    : lstTm03.Where(i => i.F03_KneadingLine == "2");
            }

            //var lstResult = from preProduct in lstTm03
            //    from kneadingCommand in lstTx42
            //    from kneadingRecord in lstTx43
            //    where preProduct.F03_PreProductCode.Equals(kneadingCommand.F42_PreProductCode)
            //          && kneadingCommand.F42_KndCmdNo.Equals(kneadingRecord.F43_KndCmdNo)
            //    select new WeighingEquipmentItem
            //    {
            //        CommandNo = kneadingRecord.F43_KndCmdNo,
            //        PreproductLotNo = kneadingRecord.F43_PrePdtLotNo,
            //        MaterialCode = preProduct.F03_PreProductCode,
            //        MaterialLotNo = kneadingRecord.F43_MaterialLotNo,
            //        KneadingLine = preProduct.F03_KneadingLine,
            //        StartDate = kneadingRecord.F43_BthEndDate,
            //        EndDate = kneadingRecord.F43_BthBgnDate,
            //        Sequence = kneadingRecord.F43_BatchSeqNo,
            //        ChargedQty = kneadingRecord.F43_LayinginAmount,
            //        Date = kneadingCommand.F42_KndEptBgnDate
            //    };

            var lstResult =
                lstTm03.Join(lstTx42, tm03PreProduct => tm03PreProduct.F03_PreProductCode,
                    tx42 => tx42.F42_PreProductCode, (tm03PreProduct, tx42) => new { tm03PreProduct, tx42 })
                    .Join(lstTx43, @t => @t.tx42.F42_KndCmdNo, tx43 => tx43.F43_KndCmdNo, (@t, tx43) => new { @t, tx43 })
                    .Select(i => new { i.t, i.tx43 })
                    .Select(
                        i =>
                            new WeighingEquipmentItem()
                            {
                                Date = i.t.tx42.F42_KndEptBgnDate,
                                CommandNo = i.tx43.F43_KndCmdNo,
                                PreproductLotNo = i.tx43.F43_PrePdtLotNo,
                                MaterialCode = i.t.tm03PreProduct.F03_PreProductCode,
                                MaterialLotNo = i.tx43.F43_MaterialLotNo,
                                KneadingLine = i.t.tm03PreProduct.F03_KneadingLine,
                                StartDate = i.tx43.F43_BthEndDate,
                                EndDate = i.tx43.F43_BthBgnDate,
                                Sequence = i.tx43.F43_BatchSeqNo,
                                ChargedQty = i.tx43.F43_LayinginAmount
                            });
            var itemCount = lstResult.Count();
            if (gridSettings != null)
            {
                OrderByAndPaging(ref lstResult, gridSettings);
            }

            var resultModel = new GridResponse<WeighingEquipmentItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<WeighingEquipmentItem>>(resultModel, true);
        }

        public bool DeleteDataOnQueue(WeighingEquipmentViewSearchModel searchModel)
        {
            var lstSelectRe = searchModel.SelectedRec;
            switch (searchModel.ViewSelect)
            {
                case Constants.ViewSelectedC4.MasterialMaster:
                case Constants.ViewSelectedC4.PreproductMaster:
                    var lstResult = _unitOfWork.MtrMsrSndCmdRepository.GetMany(
                        i => i.F52_TerminalNo == searchModel.TerminalNo)
                        .ToList()
                        .Where(i => lstSelectRe.Contains(i.F52_AddDate));
                    foreach (var tx52 in lstResult)
                    {
                        _unitOfWork.MtrMsrSndCmdRepository.Delete(tx52);
                    }
                    break;
                case Constants.ViewSelectedC4.Retrieval:
                    _unitOfWork.MtrRtrMsrSndCmdRepository.Delete(i => lstSelectRe.Contains(i.F54_MtrRtrDate));
                    break;
                case Constants.ViewSelectedC4.KneadingCommand:
                    _unitOfWork.KndCmdMsrSndRepository.Delete(
                        i => i.F55_KndCmdNo == searchModel.KndCmdNo && i.F55_PrePdtLotNo == searchModel.PrePdtLotNo);
                    _unitOfWork.KneadingCommandRepository.Delete(i => i.F42_KndCmdNo == searchModel.KndCmdNo);
                    break;
            }
            _unitOfWork.Commit();
            return true;
        }

        public bool SendPreproductMaster(string terminalNo, string deviceCode)
        {
            return SendMessage(terminalNo, 1, deviceCode);
        }

        public bool SendMaterialMaster(string terminalNo, string deviceCode)
        {
            return SendMessage(terminalNo, 2, deviceCode);
        }

        public bool CheckDevice()
        {
            return
                _unitOfWork.DeviceRepository.GetMany(
                    i => i.F14_DeviceCode == Constants.TerminalNo.C004 && i.F14_UsePermitClass == "1")
                    .Any();
        }



        public ResponseResult CheckDeviceStatus()
        {
            var text = DateTime.Now.ToString("yyyy/MM/dd") +
                       "File/DB Command No. ## Close communication (Switch to Off-line mode)";
            var tm14 = _unitOfWork.DeviceRepository.Get(i => i.F14_DeviceCode == "C004");
            if (tm14.F14_DeviceStatus == Constants.F14_DeviceStatus.Online.ToString("D"))
            {

            }
            else
            {

            }
            return new ResponseResult(true);
        }

        private int UpdateError()
        {
            var lstTx52 = _unitOfWork.MtrMsrSndCmdRepository.GetMany(i => i.F52_Status == "1");
            foreach (var tx52 in lstTx52)
            {
                tx52.F52_Status = "0";
                _unitOfWork.MtrMsrSndCmdRepository.Update(tx52);
            }
            var lstTx54 = _unitOfWork.MtrRtrMsrSndCmdRepository.GetMany(i => i.F54_Status == "1");
            foreach (var tx54 in lstTx54)
            {
                tx54.F54_Status = "0";
                _unitOfWork.MtrRtrMsrSndCmdRepository.Update(tx54);
            }
            var lstTx55 = _unitOfWork.KndCmdMsrSndRepository.GetMany(i => i.F55_Status == "1");
            foreach (var tx55 in lstTx55)
            {
                tx55.F55_Status = "0";
                _unitOfWork.KndCmdMsrSndRepository.Update(tx55);
            }
            try
            {
                _unitOfWork.Commit();
                return 0;
            }
            catch (Exception)
            {

                return -1;
            }

        }

        private bool OnlinePrc(int onState, string deviceCode)
        {
            if (onState == 1)
            {
                var devicestatus = "";
                var mode = "";

                UpdateDevice(2, ref devicestatus, ref mode, "C004");
                devicestatus = "0";
                mode = "0";
                var res = UpdateDevice(1, ref devicestatus, ref mode, "C004");
                if (!res) return false;
                var State = 2; // Polling State
                var PrevState = 6; // Online state
                // Poll the mc
                // ProcessPolling(1, err, ercd);
                return true;
            }
            if (onState == 2)
            {
                // Came back from weighing equipment
                UpdateError();
                var cmd = "";
                var ret = CheckSend(ref cmd, 3, deviceCode, "C004");
                var PrevState = 0;
                if (ret == false)
                {
                    // modified 15/7 -- to check DB table again
                    ret = CheckSend(ref cmd, 1, deviceCode, "C004"); // modified 21/10 
                    if (ret == false)
                    {
                        onState = 0;
                        StartTimer(60, 3);
                    }
                    //else
                    //    ProtoState = 1;
                }
                return true;
            }
            return true;
        }

        private int ProcessPolling(int processaction, string err, string errcode, string terminalNumber)
        {
            //            int res, msglen, cmd, READ_HEADER = 1, READ_END = 2, READ_OK = 3, j, i, act, POLL = 2, ncomd = 2, msg_id,
            //     NOTHING = 3, ERROR = 4, SENDALL = 5, SENDOK = 12, UPDATEDB = 13, FINISH = 14, SENDERR = 15, SENDNAC = 16;
            //string temp, comd, sdt, kndcmd, ppdlot, bthcomp, sts, mode;
            //string sndchr, mcno, cmd1;
            //DateTime dt;

            ////blob blobbuf;
            //string ercd, tmno;

            //act = processaction;

            //tmno = terminalNumber;
            //            if (tmno == "C004")
            //            {
            //                mcno = "A";

            //            }
            //            else
            //            {
            //                mcno = "B";
            //            }		

            //if (act == 1 ){			// Send the polling
            //      State = POLL;								// Polling State
            //      res = Poll(mcno, cmd);
            //      // Update DB								// Modified by Sum (23/10)
            //      if (State == 2 ){
            //          UpdateCommand(kneadingcommno, ppdlotno, "00", sdt, "4", "00", dt, 1);
            //      }
            //      StartTimer(tmTO, idTO);					// 1 = COMM_TO (8/7)
            //      PollState = 1;
            //}

            //if act = ERROR then							// Receive NG (check error code)
            //    // Check Error Code
            //    ercd = errCode;
            //    if (ercd == "41")  then  // modifed 10/7 -- to check DB
            //        res = Checktables(cmd, 4);
            //        if res = 0  then    		// added 10/7  -- if no record ==> to select mode
            //            ResendNO = 0;
            //            State = 0;				//      Back to 0 state
            //            mode[1] = "1";
            //            UpdateDevice(1, devicestatus, mode, devicecode);
            //            st_3.text = "Selecting";
            //            OnlinePrc(2);					// Continue with on-line process
            //            return 0;
            //        end if
            //    end if                          // Receive other codes
            //     // Continue to Poll         
            //    State = POLL;						
            //    ResendNo = ResendNo + 1;
            //    if ResendNo > 2 then				// X3 times
            //        ResendNo=0;
            //        logevent(2, "Resend x3 error! ", 1);
            //        // End Conversation
            //        UpdateDB(ncomd, "3",  errCode, "00");

            //            State = -1;					// Error state (newly defined)
            //            // Update Device
            //            st_5.text  = "Error";
            //            UpdateDevice(1, devicestatus, mode, devicecode);
            ////        StartTimer(10, 2);        
            //            timer(0, this);
            //            Polldata.textdata = "";
            //            Pollstate = 0;				// Reset
            //            offlineprc(2);
            //            return 0;
            //        end if
            //        res = Poll(mcno, cmd);
            //        StartTimer(tmTO, idTO);					// 1 = COMM_TO
            //        PollState = 1;
            //    end if
            ////end if

            //kndcmd = mid(PollData.textdata, 2, 6);
            //ppdlot = mid(PollData.textdata, 20, 10);
            //bthcomp = mid(PollData.textdata, 30, 2);
            //// Send NG
            //if act = SENDERR then					// BCC Error, Send NG (Reset all parameters)
            //    ResendNo = ResendNo + 1;
            //    if ResendNo > 2 then				// X3 times
            //        ResendNo=0;
            //        logevent(2, "Resend x3 error! ", 1);
            //        // End Conversation
            //        UpdateDB(ncomd, "3",  errCode, "00");
            //        // Send back to TCP/IP
            //        sndchr[1] = char(02);   
            //        blobbuf = blob(sndchr);
            ////    
            //        State = -1;					// Error state (newly defined)
            //        // Update Device
            //        st_5.text  = "Error";
            //        UpdateDevice(1, devicestatus, mode, devicecode);
            ////        StartTimer(10, 2);        
            //        timer(0, this);
            //        Polldata.textdata = "";
            //        Pollstate = 0;				// Reset
            //        offlineprc(2);
            //        return 0;
            //    end if
            //    err[1] = "2";						// MOdified on 8/7/96 6.30 pm   
            //    acknowledge(err, errCode, 2);
            //    Polldata.textdata = "";
            //    PollState = 1;					// Reset
            //    StartTimer(tmTO, idTO);		// add a timer (9/7/96)
            //    return 0;							// added on 9/7/96 -- 1.00pm
            //end if

            //// Send OK (send OK)
            //if act = SENDOK OR act = UPDATEDB OR act = FINISH then
            //    err = "1";								// MOdified on 8/7/96 -- 9.00pm
            //    errCode = "00";
            //    acknowledge(err, errCode, 2);
            //    ResendNo = 0;
            //    PollState = 0;
            //end if

            //// Update DB
            //if act = UPDATEDB OR act = FINISH then
            //    // Update Database and inform Ax
            //    kndcmd = mid(PollData.textdata, 2, 6);
            //    ppdlot = mid(PollData.textdata, 20, 10);
            //    bthcomp = mid(PollData.textdata, 30, 2);
            //    if act = FINISH then
            //        i = 3;
            //        sts[1] = '2';
            //    else
            //        i = 2;
            //        sts[1] = '4';
            //    end if
            //    res = UpdateCommand(kndcmd, ppdlot, bthcomp, mid(PollData.textdata, 33, 12), sts, "00", dt, i);
            //    if res = -1 then
            //        logevent(1, "Write to Table Tx42_KndCmd/Tx55_KndCmdMsrSnd Failed! \n (Record :" + PollData.mcno + PollData.command + PollData.textdata, 2);
            //        return -1;
            //    end if
            //    res = UpdateResults(kndcmd, ppdlot, bthcomp, PollData.textdata);
            //    if res = -1 then
            //        logevent(1, "Write to Table Tx43_KndRcd Failed! \n (Record :" + PollData.mcno + PollData.command + PollData.textdata, 2);
            //        // Modified to continue processing event though error (7/7/96)
            ////        return -1;
            //    end if
            //    if act = FINISH then
            //        // Modified by Sum (28/10)
            //        UpdateProduction(mid(PollData.textdata, 8, 12), dt, 3);
            //        // Send to Axx 			-- modified by Sum (21/10)
            //        msg_id = 1003;
            //        Send_Tcp(msg_id, kndcmd, ppdlot);
            //    end if
            //end if



            //// Check DB Table for Cmd 05 (Special Case) -- modified 7/7/96 12:30pm -- for checking Command 05
            //if  ( act = UPDATEDB OR act = SENDOK OR act = FINISH ) then
            //    // Check RTR command queue
            //    PollData.textdata = "";
            //    Data = "";
            //    // Change text
            //    if Check_Send(comd, 2) = TRUE then
            //        ProtoState = READ_HEADER; // Read Header
            //        if PrevState = 0 then		//  Modified 7/7/96 8.30pm -- for previous state
            //            PrevState = 2;			// Previous polling state
            //        else 
            //            PrevState = 3;			// modified 8/7/96 8.00pm -- for previous on-line state
            //        end if
            //        logevent(2, "Switch to Selecting Mode for Sending Command 5", 1);
            //        mode[1] = "1";
            //        st_3.text = "Selecting";
            //        UpdateDevice(1, devicestatus, mode, devicecode);
            //        return 0;
            //    end if
            //end if

            //if act  = SENDOK then				// modified 10/7/96 -- to start timer and poll again
            //    State = 2;
            //    StartTimer(tmPOLL, idPL);		// Polling TO (2);
            //    PollData.textdata = "";
            //    PollState  = 0;
            //end if

            //// modified 10/7/96 -- to poll again for next data (both on-line process or normal process)
            //if act = FINISH OR act = UPDATEDB then
            //    // Polling
            //    State = 2;
            //    data = "";
            //    PollData.textdata = "";
            //    res = Poll(mcno, cmd);				// Continue to poll until ??
            //    StartTimer(tmTO, idTO);					// 1 = COMM_TO
            //    PollState = 1;
            //    return 0;     
            //end if



            //    if PrevState =  6 then
            //        // Polling
            //        State = 0;
            //        mode[1] = "1";
            //        UpdateDevice(1, devicestatus, mode, devicecode);
            //        st_3.text = "Selecting";
            //        OnlinePrc(2);						// COntinue with on-line processing
            //    end if
            //end if


            return 0;
        }

        #region private method

        private bool SendMessage(string terminalNo, int comd, string devicecode)
        {
            var mccls = "2";
            var sndflg1 = "2";
            if (terminalNo == Constants.TerminalNo.C004)
            {
                mccls = "1";
                sndflg1 = "1";
            }
            var cmd = "04";
            var priority = "2";
            if (comd == 1)
            {

                var lstMaterial =
                    _unitOfWork.MaterialRepository.GetMany(
                        i => i.F01_MsrMacSndFlg == mccls || i.F01_MsrMacSndFlg == sndflg1);
                if (!lstMaterial.Any())
                {
                    return false;
                }
                foreach (var tm01 in lstMaterial)
                {
                    _unitOfWork.MtrMsrSndCmdRepository.insert(terminalNo, mccls, cmd, "0", priority,
                        Constants.PictureNo.TCTR011F, tm01.F01_MaterialCode, "00");
                }
            }
            else
            {
                cmd = "03";
                priority = "1";
                var lstPreProduct = _unitOfWork.PreProductRepository.GetAll();
                if (!lstPreProduct.Any())
                {
                    return false;
                }
                foreach (var tm03 in lstPreProduct)
                {
                    _unitOfWork.MtrMsrSndCmdRepository.insert(terminalNo, mccls, cmd, "0", priority,
                        Constants.PictureNo.TCTR011F, tm03.F03_PreProductCode, "00");
                }
            }
            bool res = CheckSend(ref cmd, 1, devicecode, terminalNo);
            if (res == false)
            {
                StartTimer(60, 3);
            }


            return true;
        }

        private void StartTimer(int timeperiod, int toaction)
        {

            var process = false;
            var currtime = DateTime.Now;
            var timeout = currtime.AddMinutes(timeperiod).TimeOfDay;
            // Modified by Sum (5/3) -- to check for next day
            if (timeout.Hours == 23 && timeout.Minutes == 59 && timeout.Seconds == 59)
            {
                if (timeout.Add(-currtime.TimeOfDay).TotalMilliseconds < timeperiod)
                {
                    timeout = new TimeSpan(0, 0, 0).Add(new TimeSpan(timeperiod - (currtime.Add(-timeout).Ticks + 1)));
                }
            }
            //todo đéo biết làm  gì ở đây

        }

        private bool CheckSend(ref string comd, int chktp, string devicecode, string terminalNumber)
        {
            string status = "";
            string mode = "";
            DateTime stdate = new DateTime();
            UpdateDevice(2, ref status, ref mode, devicecode);
            var state = 1;
            var terminalNo = "";
            var mcls = "";
            var anycode = "";
            var palletNo = "";
            var ppdlotno = "";
            var kneadingcommno = "";
            var recdate = new DateTime();
            var code = "";
            var ret = CheckTables(ref state, ref kneadingcommno, ref ppdlotno, ref palletNo, ref anycode, ref comd,
                ref mcls, chktp, ref terminalNo, terminalNo, ref recdate);
            if (ret == 0 || ret == -1) return false;
            if (ret == 2)
            {
                StartTimer(20, 2); // Start Polling mode; -- 8/7
                return true;
            }
            if (status == "1")
            {
                // Update to DB
                switch (ConvertHelper.ToInteger(comd))
                {
                    case 1:
                        UpdateCommand(kneadingcommno, ppdlotno, "00", "", "4", "00", ref stdate, 1);
                        break;
                    case 3:
                    case 14:
                    case 13:
                        UpdateMaterial("4", "00", recdate, terminalNo, mcls);
                        break;
                    case 5:
                        UpdateRetrieval(recdate, palletNo, code, "4", "00", mcls);
                        break;
                }

                //st_5.text = "Off-line";
                //logevent(1, "Now is in offline state", 0);
                return false;

            } // Offline
            code = anycode;
            MessageC4Item msg = new MessageC4Item();
            int len = 0;
            ret = Prepare_Msg(ref msg, ref comd, ref len, code, kneadingcommno, recdate, palletNo, ppdlotno, mcls);
            if (ret == 0)
            {
                var ResendNo = 0;
                Send_Select(msg, len);
                stdate = DateTime.Now;
                var sdt = stdate.ToString("YYYYMMDDhhmm");
                StartTimer(20, 1);
                switch (ConvertHelper.ToInteger(comd))
                {
                    case 1:
                        if (UpdateCommand(kneadingcommno, ppdlotno, "00", sdt, "1", "00", ref stdate, 4) == 0)
                        {
                            UpdateProduction(msg.textdata.Substring(7, 12), stdate, 2);
                        }
                        break;
                    case 3:
                    case 4:
                    case 13:
                    case 14:
                    case 5:
                        if (ConvertHelper.ToInteger(comd) != 5)
                        {
                            UpdateMaterial("1", "00", recdate, terminalNo, mcls);
                        }
                        else
                        {
                            UpdateRetrieval(recdate, palletNo, code, "1", "00", mcls);
                        }
                        break;

                }

            }
            return true;
        }

        private int UpdateProduction(string prepdtcode, DateTime productiondate, int pdt_sts)
        {
            var tx39 =
                _unitOfWork.PdtPlnRepository.Get(
                    i => i.F39_PreProductCode == prepdtcode && i.F39_KndEptBgnDate == productiondate);
            if (tx39 == null) return -1;
            var amt = tx39.F39_EndLotAmont;
            if (pdt_sts == 3)
            {
                amt += 1;
            }
            var newStatus = "";
            if (amt >= tx39.F39_PrePdtLotAmt)
            {
                if (tx39.F39_Status == "6")
                {
                    newStatus = "3";
                }
                else
                {
                    newStatus = Convert.ToChar(48 + pdt_sts).ToString();
                }

            }
            else if (tx39.F39_Status == "6")
            {
                newStatus = "5";
            }
            tx39.F39_EndLotAmont = amt;
            tx39.F39_Status = newStatus;
            tx39.F39_UpdateCount += 1;
            tx39.F39_UpdateDate = DateTime.Now;
            _unitOfWork.PdtPlnRepository.Update(tx39);
            return 0;
        }

        private int Send_Select(MessageC4Item msg, int msgsize)
        {
            string send = new string(' ', msgsize);

            send = msg.stx + msg.mtype + msg.mcno + msg.command + msg.textdata + msg.bcc + msg.etx;

            var res = 0; //todo comm_obj.write_comm(Comm_ID, msgSize, send);
            //if (res == -1 )
            //    //MessageBox("Error", "Error in writing to communication port", StopSign!);

            //        var  writeinfo = send;
            ////logevent(2, writeinfo, 1);
            ////ProtoState = 1;

            return res;

        }

        private bool UpdateDevice(int rwtype, ref string status, ref string mode, string devicecd)
        {
            var device = _unitOfWork.DeviceRepository.GetById(devicecd);
            if (rwtype == 1) // Write
            {
                device.F14_DeviceMode = mode;
                device.F14_DeviceStatus = status;
                _unitOfWork.DeviceRepository.Update(device);
            }
            else
            {
                mode = device.F14_DeviceMode;
                status = device.F14_DeviceStatus;
            }
            return true;
        }

        private int CheckTables(ref int State, ref string kneadingcommno, ref string ppdlotno, ref string palletNo,
            ref string anycode, ref string comd, ref string mcls, int tblchk, ref string terminalNo,
            string terminalNumber, ref DateTime recdate)
        {
            var line = "2";
            if (terminalNumber == Constants.TerminalNo.C004)
            {
                line = "1";
            }
            var lineBoth = "3";
            var staus = "0";
            var status = "";
            if (tblchk == 1 || tblchk == 3)
            {
                if (tblchk == 3)
                    staus = "3";
                var tx52 =
                    _unitOfWork.MtrMsrSndCmdRepository.GetMany(
                        i => i.F52_Status == staus && (i.F52_MsrMacCls == line || i.F52_MsrMacCls == lineBoth))
                        .OrderBy(i => i.F52_AddDate)
                        .ThenBy(i => i.F52_Priority)
                        .FirstOrDefault();
                if (tx52 != null)
                {
                    comd = tx52.F52_CommandType;
                    mcls = tx52.F52_MsrMacCls;
                    terminalNo = tx52.F52_TerminalNo;
                    anycode = tx52.F52_MasterCode;
                    recdate = tx52.F52_AddDate;
                }
                else
                {
                    return -1;
                }
            }
            if (tblchk == 1 || tblchk == 2 || tblchk == 3)
            {
                comd = "05";
                if (tblchk == 3)
                    staus = "3";
                var tx54 =
                    _unitOfWork.MtrRtrMsrSndCmdRepository.GetMany(
                        i => i.F54_Status == staus && (i.F54_MsrMacClass == line || i.F54_MsrMacClass == lineBoth))
                        .OrderBy(i => i.F54_AddDate)
                        .ThenBy(i => i.F54_Priority)
                        .FirstOrDefault();
                if (tx54 != null)
                {
                    palletNo = tx54.F54_PalletNo;
                    mcls = tx54.F54_MsrMacClass;
                    anycode = tx54.F54_MaterialCode;
                    status = tx54.F54_Status;
                    recdate = tx54.F54_AddDate;
                }
                else
                {
                    return -1;
                }
            }
            if (tblchk == 1 || tblchk == 3 || tblchk == 4)
            {
                comd = "01";
                if (tblchk == 3)
                    staus = "3";
                if (tblchk == 4)
                {
                    staus = "";
                }

                var tx55 =
                    _unitOfWork.KndCmdMsrSndRepository.GetMany(
                        i => i.F55_Status == "4" || i.F55_Status == staus && i.F55_MsrSndCls == line)
                        .OrderBy(i => i.F55_Status)
                        .ThenBy(i => i.F55_AddDate).ThenBy(i => i.F55_Priority)
                        .FirstOrDefault();
                if (tx55 != null)
                {
                    kneadingcommno = tx55.F55_KndCmdNo;
                    ppdlotno = tx55.F55_PrePdtLotNo;
                    mcls = tx55.F55_MsrSndCls;
                    status = tx55.F55_Status;
                    recdate = tx55.F55_AddDate;
                }
                else
                {
                    return -1;
                }
            }
            if (status == "4")
            {
                comd = "02";
                State = 2; // Goto polling mode
                return 2;
            }
            return 1;
        }

        private int UpdateCommand(string kneadingno, string prepdtlotno, string batchcomp, string kneadingdate,
            string status, string errorcode, ref DateTime kndbgndate, int seltype)
        {
            int batchend, update_cnt;
            string errText, bthcpt;


            batchend = ConvertHelper.ToInteger(batchcomp);

            // Modified by Sum (28/10)
            //if( seltype == 3 || seltype == 4 || seltype == 5)
            //{
            var tx42 =
                _unitOfWork.KneadingCommandRepository.GetMany(
                    i => i.F42_KndCmdNo == kneadingno && i.F42_PrePdtLotNo == prepdtlotno).FirstOrDefault();
            //}
            kndbgndate = tx42.F42_KndBgnDate.Value;
            if (seltype >= 1)
            {
                // Update the kndcmdmsrsnd table only (Status, abnormal code);

                var tx55 =
                    _unitOfWork.KndCmdMsrSndRepository.GetMany(
                        i => i.F55_KndCmdNo == kneadingno && i.F55_PrePdtLotNo == prepdtlotno).FirstOrDefault();
                if (tx55 != null)
                {
                    tx55.F55_Status = status;
                    tx55.F55_AbnormalCode = errorcode;
                    tx55.F55_UpdateDate = DateTime.Now;
                    _unitOfWork.KndCmdMsrSndRepository.Update(tx55);
                }
                else
                {
                    errText = "Material & Preproduct master record not found!";
                    return -1;
                }

                // Modified by Sum (1/11)
                bthcpt = batchcomp;
                if (bthcpt == "00" && seltype == 5)
                {
                    // Update Tx42 (KndBgnDate

                    if (tx42 != null)
                    {
                        tx42.F42_UpdateCount = tx42.F42_UpdateCount + 1;
                        tx42.F42_KndBgnDate = DateTime.Now;
                        tx42.F42_Status = "1";
                        tx42.F42_BatchEndAmount = batchend;
                        tx42.F42_UpdateDate = DateTime.Now;
                        _unitOfWork.KneadingCommandRepository.Update(tx42);
                    }
                    else
                    {
                        errText = "Material & Preproduct master record not found!";
                        return -1;
                    }
                }
                // Modified by Sum (21/11)
                if (bthcpt == "00" && seltype == 4)
                {
                    // Update Tx42 (Status = '6' / Command begin)
                    if (tx42 != null)
                    {
                        tx42.F42_UpdateCount = tx42.F42_UpdateCount + 1;
                        tx42.F42_Status = "6";
                        tx42.F42_UpdateDate = DateTime.Now;
                        _unitOfWork.KneadingCommandRepository.Update(tx42);
                    }
                    else
                    {
                        errText = "Material & Preproduct master record not found!";
                        return -1;
                    }
                }
            }
            if (seltype >= 2)
            {
                // Update the kndcmdmsrsnd table (status, abnormal code) and kndcmd table (batchend, enddate, status, updatedate)
                if (seltype == 2)
                {
                    // Update batchend, status
                    if (tx42 != null)
                    {
                        tx42.F42_UpdateCount = tx42.F42_UpdateCount + 1;
                        tx42.F42_Status = "1";
                        tx42.F42_BatchEndAmount = batchend;
                        tx42.F42_UpdateDate = DateTime.Now;
                        _unitOfWork.KneadingCommandRepository.Update(tx42);
                    }
                    else
                    {
                        errText = "Material & Preproduct master record not found!";
                        return -1;
                    }
                }
                if (seltype == 3)
                {
                    // Update Batch end, kndenddate, status 

                    if (tx42 != null)
                    {
                        tx42.F42_UpdateCount = tx42.F42_UpdateCount + 1;
                        tx42.F42_Status = "2";
                        tx42.F42_BatchEndAmount = batchend;
                        tx42.F42_UpdateDate = DateTime.Now;
                        tx42.F42_KndEndDate = DateTime.Now;
                        _unitOfWork.KneadingCommandRepository.Update(tx42);
                    }
                    else
                    {
                        errText = "Material & Preproduct master record not found!";
                        return -1;
                    }

                }
            }



            if (seltype == 3 && status == "2")
            {
                var tx55 =
                    _unitOfWork.KndCmdMsrSndRepository.GetMany(
                        i => i.F55_KndCmdNo == kneadingno && i.F55_PrePdtLotNo == prepdtlotno).FirstOrDefault();
                if (tx55 != null)
                {
                    _unitOfWork.KndCmdMsrSndRepository.Delete(tx55);
                }
                else
                {
                    errText = "Kneading Command Send record not found!";
                    //  MessageBox("Pre-product", errText, StopSign!);
                }
                var th62 = new TH62_KndCmdMsrSndHst()
                {
                    F62_KndCmdNo = kneadingno,
                    F62_PrePdtLotNo = tx55.F55_PrePdtLotNo,
                    F62_PrePdtCode = tx55.F55_PrePdtCode,
                    F62_Status = status,
                    F62_Priority = tx55.F55_Priority,
                    F62_MsrSndCls = tx55.F55_MsrSndCls,
                    F62_TerminalNo = tx55.F55_TerminalNo,
                    F62_PictureNo = tx55.F55_PictureNo,
                    F62_AbnormalCode = errorcode,
                };
                _unitOfWork.KndCmdMsrSndHstRepository.Add(th62);

            }
            return 0;
        }

        private int UpdateMaterial(string status, string errorcode, DateTime recorddate, string terminalno,
            string mcclass)
        {

            var lstTx52 =
                _unitOfWork.MtrMsrSndCmdRepository.GetMany(
                    i => i.F52_AddDate == recorddate && i.F52_TerminalNo == terminalno && i.F52_MsrMacCls == mcclass);
            if (!lstTx52.Any())
            {

                var errText = "Material & Preproduct master record not found!";
                return -1;
                // MessageBox("Pre-product", errText, StopSign!);
            }
            foreach (var tx52 in lstTx52)
            {
                tx52.F52_Status = status;
                tx52.F52_AbnormalCode = errorcode;
                tx52.F52_UpdateDate = DateTime.Now;
                _unitOfWork.MtrMsrSndCmdRepository.Update(tx52);
            }
            // Update to History Record
            if (status == "2")
            {
                foreach (var tx52 in lstTx52)
                {
                    var th68 = new TH68_MtrMsrSndCmdHst()
                    {
                        F68_Status = status,
                        F68_AbnormalCode = errorcode,
                        F68_AddDate = recorddate,
                        F68_TerminalNo = tx52.F52_TerminalNo,
                        F68_MsrMacCls = mcclass,
                        F68_Priority = tx52.F52_Priority,
                        F68_CommandType = tx52.F52_CommandType,
                        F68_MasterCode = tx52.F52_MasterCode,
                        F68_PictureNo = tx52.F52_PictureNo,
                    };
                    _unitOfWork.MtrMsrSndCmdHstRepository.Add(th68);
                    _unitOfWork.MtrMsrSndCmdRepository.Delete(tx52);
                }

            }

            return 0;
        }

        private int UpdateRetrieval(DateTime retrievaldate, string palleteno, string rawmaterialcode, string status,
            string errorcode, string mcclass)
        {
            var errText = "";
            var tx54 =
                _unitOfWork.MtrRtrMsrSndCmdRepository.Get(
                    i =>
                        i.F54_MtrRtrDate == retrievaldate && i.F54_PalletNo == palleteno &&
                        i.F54_MaterialCode == rawmaterialcode && i.F54_MsrMacClass == mcclass);
            if (tx54 != null)
            {
                tx54.F54_Status = status;
                tx54.F54_AbnormalCode = errorcode;
                tx54.F54_UpdateDate = DateTime.Now;
                _unitOfWork.MtrRtrMsrSndCmdRepository.Update(tx54);
            }
            else
            {
                errText = "Material retrieval master record not found!";
                return -1;
            }
            if (status == "2")
            {
                if (tx54 != null)
                {
                    _unitOfWork.MtrRtrMsrSndCmdRepository.Delete(tx54);
                    var th69 = new TH69_MtrRtrMsrSndCmdHst()
                    {
                        F69_MtrRtrDate = retrievaldate,
                        F69_PalletNo = tx54.F54_PalletNo,
                        F69_MaterialCode = rawmaterialcode,
                        F69_MsrMacClass = mcclass,
                        F69_TerminalNo = tx54.F54_TerminalNo,
                        F69_PictureNo = tx54.F54_PictureNo,
                        F69_Status = status,
                        F69_Priority = tx54.F54_Priority,
                        F69_MtrLotNo1 = tx54.F54_MtrLotNo1,
                        F69_MtrLotNo2 = tx54.F54_MtrLotNo2,
                        F69_MtrLotNo3 = tx54.F54_MtrLotNo3,
                        F69_MtrLotNo4 = tx54.F54_MtrLotNo4,
                        F69_MtrLotNo5 = tx54.F54_MtrLotNo5,
                        F69_Amount1 = tx54.F54_Amount1,
                        F69_Amount2 = tx54.F54_Amount2,
                        F69_Amount3 = tx54.F54_Amount3,
                        F69_Amount4 = tx54.F54_Amount4,
                        F69_Amount5 = tx54.F54_Amount5,
                        F69_AbnormalCode = errorcode
                    };
                    _unitOfWork.MtrRtrMsrSndCmdHstRepository.Add(th69);
                    _unitOfWork.MtrRtrMsrSndCmdRepository.Delete(tx54);
                }
                else
                {
                    errText = "Retrieval Command Send record not found!";
                }
            }
            return 0;
        }

        private int Prepare_Msg(ref MessageC4Item message, ref string comd, ref int len, string code, string mixno,
            DateTime recdate, string palletNo,
            string lotno, string mcls)
        {
            message.stx = "2";
            message.etx = "3";
            message.mtype = "SL";

            if (mcls == "1")
            {
                message.mcno = "A";
            }
            else if (mcls == "2")
            {
                message.mcno = "B";
            }
            else
            {
                message.mcno = "F";
            }

            message.command = comd;
            var cmd = ConvertHelper.ToInteger(comd);
            string msg = "";
            var ret = 0;
            switch (cmd)
            {
                case 1:
                    ret = Get_Cmd01(ref mixno, ref lotno, ref msg);
                    len = 64;
                    break;
                case 3:
                    len = 1024;
                    ret = PreproductMaster(ref code, ref msg);
                    break;
                case 4:
                    len = 64;
                    ret = RawMaterialMaster(ref code, ref msg);
                    break;
                case 5:
                    ret = Get_Cmd05(code, ref msg, recdate, palletNo);
                    len = 192;
                    break;
                case 13:
                    msg = new string(Convert.ToChar(code), 1003);
                    len = 1024;
                    ret = 0;
                    break;
                case 14:
                    msg = new string(Convert.ToChar(code), 43);
                    len = 64;
                    ret = 0;
                    break;
                default:
                    ret = -1;
                    break;

            }
            return ret;
        }

        private int Get_Cmd05(string rawmatcode, ref string text, DateTime retrievaldate, string palletno)
        {
            var tx54 =
                _unitOfWork.MtrRtrMsrSndCmdRepository.Get(
                    i =>
                        i.F54_MtrRtrDate == retrievaldate && i.F54_MaterialCode == rawmatcode &&
                        i.F54_MtrRtrDate == retrievaldate && i.F54_PalletNo == palletno);
            if (tx54 == null) return -1;
            var loc = "1";
            if (tx54.F54_TerminalNo.ToLower().Trim() == Constants.TerminalNo.A016 ||
                tx54.F54_TerminalNo.ToLower() == Constants.TerminalNo.A017)
                loc = "0";

            text = rawmatcode + loc + retrievaldate.ToString("yyyyMMddHH:mm");
            text = text + tx54.F54_MtrLotNo1 + tx54.F54_Amount1 + tx54.F54_MtrLotNo2 + tx54.F54_Amount2 +
                   tx54.F54_MtrLotNo3 + tx54.F54_Amount3 + tx54.F54_MtrLotNo4 + tx54.F54_Amount4 + tx54.F54_MtrLotNo5 +
                   tx54.F54_Amount5;
            return 0;
        }

        private int Get_Cmd01(ref string mixno, ref string lotno, ref string text)
        {
            string errText;
            string kndNo = mixno;
            string lotno1 = lotno;
            var tx55 =
                _unitOfWork.KndCmdMsrSndRepository.Get(i => i.F55_KndCmdNo == kndNo && i.F55_PrePdtLotNo == lotno1);
            if (tx55 != null)
            {
                string preproductcode = tx55.F55_PrePdtCode;
                text = mixno + preproductcode + lotno;
            }
            else
            {
                errText = "Kneading command record not found!";
                return -1;
            }

            return 0;
        }

        private int PreproductMaster(ref string preproductcode, ref string s_text)
        {
            var tm03 = _unitOfWork.PreProductRepository.GetById(preproductcode);
            if (tm03 == null) return -1;
            s_text = tm03.F03_PreProductCode + tm03.F03_PreProductName + string.Format("{0}00", tm03.F03_BatchLot) +
                     tm03.F03_ColorClass;
            var lstTm02 = _unitOfWork.PrePdtMkpRepository.GetMany(i => i.F02_PreProductCode == tm03.F03_PreProductCode);
            foreach (var tm02 in lstTm02)
            {
                s_text += tm02.F02_MaterialCode.Trim() + tm02.F02_3FLayinAmount + tm02.F02_4FLayinAmount +
                          tm02.F02_ThrawSeqNo + tm02.F02_PotSeqNo + tm02.F02_MsrSeqNo;
            }
            return 0;
        }

        private int RawMaterialMaster(ref string rawmatcode, ref string text)
        {
            var tm01 = _unitOfWork.MaterialRepository.GetById(rawmatcode);
            if (tm01 == null) return -1;
            text = tm01.F01_MaterialCode + tm01.F01_MaterialDsp + tm01.F01_Unit + tm01.F01_LiquidClass;
            return 0;
        }

        #endregion

        private int UpdateDb(int comd, string sts, string errcode, string bthcomp)
        {
            DateTime dt = new DateTime();
            string er;
            var sdt = DateTime.Now.ToString("yyyMMddhhmm");
            switch (comd)
            {
                case 1:
                    //     UpdateCommand(kneadingcommno, ppdlotno, bthcomp, sdt, sts, errcode, ref dt, 1);
                    break;
                case 3:
                case 4:
                case 13:
                case 14:
                    //  UpdateMaterial(sts, errCode, recdate, termno, mcls);
                    break;
                case 5:
                    //    UpdateRetrieval(recdate, palletno, anycode, sts, errCode, 1, mcls);
                    //    if (errCode = "00")
                    //{
                    //    // added 9/7/96   -- for flag  set to 1
                    //    // Update the Pallet table
                    //    UpdateMtrShf(palletno, mcls, sts);
                    //}
                    break;
            }



            return 0;
        }

        private int UpdateMtrShf(string palletno, string mcclass, string sts)
        {
            //Update the tx32_mtrshf
            var tx32 = _unitOfWork.MaterialShelfRepository.Get(i => i.F32_PalletNo == palletno);
            var update_cnt = tx32.F32_UpdateCount;
            var flg = "1";
            update_cnt = update_cnt + 1;
            if (mcclass == "1")
            {
                tx32.F32_GnrlMsrMacSndEndFlg = flg;
            }
            else if (mcclass == "2")
            {
                // Mega line (2)
                tx32.F32_MegaMsrMacSndEndFlg = flg;
            }
            else
            {
                tx32.F32_GnrlMsrMacSndEndFlg = flg;
                tx32.F32_MegaMsrMacSndEndFlg = flg;

            }
            tx32.F32_UpdateDate = DateTime.Now;
            tx32.F32_UpdateCount = update_cnt;
            _unitOfWork.MaterialShelfRepository.Update(tx32);
            try
            {
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                return -1;

            }
            return 0;
        }

        private int poll(ref string mcno, ref string command, int State)
        {
            int POLL_STATE = 2, msglen;
            Basicfmt msg = new Basicfmt();

            string writeinfo;

            if (State == POLL_STATE)
            {
                // to poll for data
                msg.stx = "  ";
                msg.mtype = "PL";
                msg.mcno = mcno;
                msg.command = "02";
                var bcc = msg.bcc;
                Calc_BCC(ref msg, ref bcc);
                msg.textdata = "";
                msg.etx = "   ";
                // Log Event
                var send = msg.stx + msg.mtype + msg.mcno + msg.command + msg.bcc + msg.etx;
                writeinfo = send;
                // LogEvent(2, "Polling ("+writeinfo+")", 1);
                // Send polling message
                // msglen = send.Length;
                _notificationService.SendMessageToComPort(send);

                //Resend = msg;
                //Resendlen = msglen;
                //ProtoState = 1;
            }
            else
            {
                return -1;
            }


            return 0;
        }

        public int Calc_BCC(ref Basicfmt message, ref string bcc)
        {
            int i, msglen, res;
            // Call external function
            var send = message.mtype + message.mcno + message.command + message.textdata;
            msglen = send.Length;
            //res = CalcBCC(send, msglen);
            //bcc[1] = res/256;
            //bcc[2] = char(Mod(res, 256));
            return 0;
        }


    }

    public class Basicfmt
    {
        public string stx { get; set; }
        public string mtype { get; set; }
        public string mcno { get; set; }
        public string command { get; set; }
        public string textdata { get; set; }
        public string bcc { get; set; }
        public string etx { get; set; }

    }


}
