using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Core.Resources;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;

namespace KCSG.Domain.Domains
{
    public class BaseDomain : ICommonDomain
    {
        public IUnitOfWork _unitOfWork;

        private readonly IConfigurationService _configurationService;

        public BaseDomain(IUnitOfWork uniOfWord, IConfigurationService configurationService)
        {
            _unitOfWork = uniOfWord;
            _configurationService = configurationService;
        }
        public BaseDomain(IUnitOfWork uniOfWord)
        {
            _unitOfWork = uniOfWord;
        }

        protected static void OrderByAndPaging<T>(ref IQueryable<T> records, GridSettings gridSetting)
        {
            if (string.IsNullOrEmpty(gridSetting.SortField))
                return;

            string methodName = gridSetting.SortOrder == SortOrder.Asc ? "OrderBy" : "OrderByDescending";
            ParameterExpression parameter = Expression.Parameter(records.ElementType, "p");

            MemberExpression memberAccess = Expression.Property(parameter, gridSetting.SortField);

            LambdaExpression orderByLambda = Expression.Lambda(memberAccess, parameter);

            MethodCallExpression result = Expression.Call(
                typeof (Queryable),
                methodName,
                new[] {records.ElementType, memberAccess.Type},
                records.Expression,
                Expression.Quote(orderByLambda));

            records = records.Provider.CreateQuery<T>(result);
            records = records.Skip((gridSetting.PageIndex - 1)*gridSetting.PageSize).Take(gridSetting.PageSize);
        }

       

        public int CreateOrUpdateTX48(ref bool isNoManage)
        {
            var noManage = _unitOfWork.NoManageRepository.Get(n => n.F48_SystemId.Equals("00000"));
            if (noManage != null)
            {
                //	Increase [f48_mtrwhscmdno] by 1.
                noManage.F48_MtrWhsCmdNo += 1;
                ////	If [f48_mtrwhscmdno] is greater than 9999, the system will:
                if (noManage.F48_MtrWhsCmdNo > 9999)
                {
                    //	Update [f48_mtrwhscmdno] as 1.
                    noManage.F48_MtrWhsCmdNo = 1;
                }
                //	Update [f48_updatedate] as current date and time.
                noManage.F48_UpdateDate = DateTime.Now;
                _unitOfWork.NoManageRepository.Update(noManage);
                isNoManage = true;
            }
            else
            {
                //insert tx48
                noManage = new TX48_NoManage();
                noManage.F48_SystemId = "00000";
                noManage.F48_MegaKndCmdNo = 0;
                noManage.F48_GnrKndCmdNo = 0;
                noManage.F48_MtrWhsCmdNo = 1;
                noManage.F48_PrePdtWhsCmdNo = 0;
                noManage.F48_PrePdtWhsCmdNo = 0;
                noManage.F48_KndCmdBookNo = 0;
                noManage.F48_AddDate = DateTime.Now;
                noManage.F48_UpdateDate = DateTime.Now;
                noManage.F48_KneadSheefNo = 0;
                noManage.F48_OutKndCmdNo = 0;
                noManage.F48_CnrKndCmdNo = 0;
                _unitOfWork.NoManageRepository.Add(noManage);
                isNoManage = false;
            }
            _unitOfWork.Commit();
            return noManage.F48_MtrWhsCmdNo;
        }


        public TX34_MtrWhsCmd InsertTX34(Constants.F34_CommandNo commandNo, int f48_MtrWhsCmdNo, string cmdType,
            string lsStatus, string status, string palletNo, string from, string to, string terminalNo, string pictureNo)
        {
            
            var commandNo1 = commandNo.ToString("D");
            var cmdSeqNo = f48_MtrWhsCmdNo.ToString("D4");
            var materialWarehouse =
                _unitOfWork.MaterialWarehouseCommandRepository.GetMany(
                    i =>
                        i.F34_CommandNo.Trim().Equals(commandNo1) &&
                        i.F34_CmdSeqNo.Trim().Equals(cmdSeqNo));

            var tx34Mtrwhscmd = new TX34_MtrWhsCmd();

            if (!materialWarehouse.Any())
            {
                
                tx34Mtrwhscmd.F34_CommandNo = commandNo1;
                //tx34Mtrwhscmd.F34_CmdSeqNo = string.Format("{0}", f48_MtrWhsCmdNo + 1);
                tx34Mtrwhscmd.F34_CmdSeqNo = cmdSeqNo;
                tx34Mtrwhscmd.F34_CommandType = cmdType;
                tx34Mtrwhscmd.F34_StrRtrType = lsStatus;
                tx34Mtrwhscmd.F34_Status = status;
                tx34Mtrwhscmd.F34_Priority = 0;
                tx34Mtrwhscmd.F34_PalletNo = palletNo;
                tx34Mtrwhscmd.F34_From = from;
                tx34Mtrwhscmd.F34_To = to;
                tx34Mtrwhscmd.F34_CommandSendDate = null;
                tx34Mtrwhscmd.F34_CommandEndDate = null;
                tx34Mtrwhscmd.F34_TerminalNo = terminalNo;
                tx34Mtrwhscmd.F34_PictureNo = pictureNo;
                tx34Mtrwhscmd.F34_AbnormalCode = "";
                tx34Mtrwhscmd.F34_RetryCount = 0;
                tx34Mtrwhscmd.F34_AddDate = DateTime.Now;
                tx34Mtrwhscmd.F34_UpdateDate = DateTime.Now;
                tx34Mtrwhscmd.F34_UpdateCount = 0;

                // Add record into database.
                _unitOfWork.MaterialWarehouseCommandRepository.AddOrUpdate(tx34Mtrwhscmd);
                //_unitOfWork.Commit();
            }
            //_unitOfWork.Commit();
            return tx34Mtrwhscmd;
        }

        //Get conveyorCode for TM05 table with terminalNo as current terminalNo
        public string GetConveyorCode(string terminalNo)
        {
            var conveyorItem = _unitOfWork.ConveyorRepository.Get(i => i.F05_TerminalNo.Trim().Equals(terminalNo));
            if (conveyorItem == null)
                throw new Exception("Invalid conveyor item");
            return conveyorItem.F05_ConveyorCode;
        }

        /// <summary>
        /// From terminal no, find the related conveyor code.
        /// </summary>
        /// <returns></returns>
        public async Task<TM05_Conveyor> FindConveyorCodeAsync(string terminalNo)
        {
            var errorStatus = Constants.F05_StrRtrSts.Error.ToString("D");
            var conveyor =
                await
                    _unitOfWork.ConveyorRepository.GetAll()
                        .FirstOrDefaultAsync(
                            i => i.F05_TerminalNo.Trim().Equals(terminalNo) && i.F05_StrRtrSts.Trim() != errorStatus);
            return conveyor;
        }

        /// <summary>
        /// From terminal no, find the related conveyor code.
        /// </summary>
        /// <returns></returns>
        public TM05_Conveyor FindConveyor(string terminalNo)
        {
            var conveyor =
                _unitOfWork.ConveyorRepository.GetAll().FirstOrDefault(i => i.F05_TerminalNo.Trim().Equals(terminalNo));
            return conveyor;
        }

        /// <summary>
        /// Check exists and status of [tm05_conveyor]  record whose  [f05_terminalno] is current terminalNo
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public bool CheckStatusAndNumberRecordOfConveyor(string terminalNo)
        {
            //find [tm05_conveyor]  record whose  [f05_terminalno] is current application terminal or [f05_strrtrsts] is “9” (Error)
            var conveyors =
                _unitOfWork.ConveyorRepository.GetMany(i => i.F05_TerminalNo.Trim().Equals(terminalNo.Trim()));

            var status = Constants.F05_StrRtrSts.Error.ToString("D");

            var consveyorError = conveyors.Where(i => i.F05_StrRtrSts.Equals(status));
            //If there is no [tm05_conveyor] record whose [f05_terminalno] is current application terminal or [f05_strrtrsts] is “9” (Error) return false
            if (!conveyors.Any() || consveyorError.Any())
                return false;
            return true;
        }

        public bool CheckHoliday(DateTime day)
        {
            var result = false;
            var holiday = _unitOfWork.CalenderRepository.GetMany(i => i.F07_Date == day).FirstOrDefault();
            if (holiday != null)
            {
                if (holiday.F07_HolidayFlag == Constants.F07_HolidayFlag.TM07_HldyFlg_Hldy ||
                    holiday.F07_SunSatDayFlag == Constants.F07_HolidayFlag.TM07_SunSatDayFlag_Yes)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// Check status and usepermitclass of [tm14_device]
        /// </summary>
        /// <returns></returns>
        public bool CheckStatusOfDeviceRecord(string deviceCode)
        {
            //var deviceCode = Constants.DeviceCode.ATW001;

            var offlineStatus = Constants.F14_DeviceStatus.Offline.ToString("D");

            var errorStatus = Constants.F14_DeviceStatus.Error.ToString("D");

            var usepermitClass = Constants.F14_UsePermitClass.Prohibited.ToString("D");

            //find no [tm14_device] record whose [f14_devicecode] is “ATW001” or [f14_devicestatus] is “1” (Offline), [f14_devicestatus] is “2” (Error) or [f14_usepermitclass] is “1” (Prohibited)
            var devices =
                _unitOfWork.DeviceRepository.GetMany(
                    i =>
                        i.F14_DeviceCode.Trim().Equals(deviceCode) || i.F14_DeviceStatus.Equals(offlineStatus) ||
                        i.F14_DeviceStatus.Equals(errorStatus) || i.F14_UsePermitClass.Equals(usepermitClass));

            return devices.Any();
        }

        /// <summary>
        /// Check status or permit of record in tm14_device of product management 
        /// Refer SRS product management
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <returns></returns>
        public bool CheckStatusOfDeviceRecordForProductManagement(string deviceCode)
        {
            if (!string.IsNullOrEmpty(deviceCode))
            {
                var errorStatus = Constants.F14_DeviceStatus.Error.ToString("D");
                var offlineStatus = Constants.F14_DeviceStatus.Offline.ToString("D");
                var permit = Constants.F14_UsePermitClass.Prohibited.ToString("D");
                //find all device record in tm14_device with deviceCode
                var devices =
                    _unitOfWork.DeviceRepository.GetMany(i => i.F14_DeviceCode.Trim().Equals(deviceCode.Trim()));

                if (!devices.Any())
                    return false;

                devices =
                    devices.Where(
                        i =>
                            i.F14_DeviceStatus.Equals(errorStatus) || i.F14_DeviceStatus.Equals(offlineStatus) ||
                            i.F14_UsePermitClass.Equals(permit));

                if (devices.Any())
                    return false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Find device by its availability.
        /// </summary>
        /// <returns></returns>
        public async Task<TM14_Device> FindDeviceAvailabilityAsync()
        {
            var offlineStatus = Constants.F14_DeviceStatus.Offline.ToString("D");
            var errorStatus = Constants.F14_DeviceStatus.Error.ToString("D");
            var usepermitClass = Constants.F14_UsePermitClass.Prohibited.ToString("D");

            //find no [tm14_device] record whose [f14_devicecode] is “ATW001” or [f14_devicestatus] is “1” (Offline), [f14_devicestatus] is “2” (Error) or [f14_usepermitclass] is “1” (Prohibited)
            var devices =
                _unitOfWork.DeviceRepository.GetMany(
                    i =>
                        i.F14_DeviceCode.Trim().Equals(Constants.DeviceCode.ATW001) ||
                        i.F14_DeviceStatus.Equals(offlineStatus) ||
                        i.F14_DeviceStatus.Equals(errorStatus) || i.F14_UsePermitClass.Equals(usepermitClass));

            return await devices.FirstOrDefaultAsync();
        }

        public async Task<TM14_Device> FindDeviceAvailabilityAsync(string productDeviceCode)
        {
            //var offlineStatus = Constants.F14_DeviceStatus.Offline.ToString("D");
            //var errorStatus = Constants.F14_DeviceStatus.Error.ToString("D");
            //var usepermitClass = Constants.F14_UsePermitClass.Prohibited.ToString("D");

            var devices =
                _unitOfWork.DeviceRepository.GetMany(
                    i =>
                        i.F14_DeviceCode.Trim().Equals(productDeviceCode));

            return await devices.FirstOrDefaultAsync();
        }

        //use communication
        public int SetConveyorSts(string lpConveyer, int nStatus)
        {
            int nRecordCount = 1;
            int nRetCode = 1;
            if (nStatus < 0) return 0;

            // restore conveyor number and status
            try
            {
                if (nStatus == 1 || nStatus == 2)
                {
                    // Storage or Retrieve send OK
                    var lstConveyor =
                        _unitOfWork.ConveyorRepository.GetMany(i => i.F05_ConveyorCode.Trim().Equals(lpConveyer.Trim()));
                    foreach (var tm05Conveyor in lstConveyor)
                    {
                        if (ConvertHelper.ToInteger(tm05Conveyor.F05_StrRtrSts) >= 0 &&
                            ConvertHelper.ToInteger(tm05Conveyor.F05_StrRtrSts) <= nStatus &&
                            tm05Conveyor.F05_BufferUsing < tm05Conveyor.F05_MaxBuffer)
                        {
                            tm05Conveyor.F05_StrRtrSts = nStatus.ToString();
                            tm05Conveyor.F05_BufferUsing = tm05Conveyor.F05_BufferUsing + 1;
                            tm05Conveyor.F05_UpdateDate = DateTime.Now;
                            _unitOfWork.ConveyorRepository.Update(tm05Conveyor);
                        }
                    }
                }
                else if (nStatus == 3)
                {
                    // Floor Movement send OK
                    var conveyor =
                        _unitOfWork.ConveyorRepository.GetMany(
                            i => i.F05_ConveyorCode == lpConveyer && i.F05_StrRtrSts == "0").FirstOrDefault();
                    conveyor.F05_StrRtrSts = nStatus.ToString();
                    conveyor.F05_BufferUsing = conveyor.F05_BufferUsing + 1;
                    conveyor.F05_UpdateDate = DateTime.Now;
                    _unitOfWork.ConveyorRepository.Update(conveyor);
                }
                else if (nStatus == 0)
                {
                    try
                    {
                        // Command End or Command Cancel

                        var conveyor =
                            _unitOfWork.ConveyorRepository.GetMany(
                                i => i.F05_ConveyorCode == lpConveyer && i.F05_BufferUsing > 0).FirstOrDefault();
                        conveyor.F05_BufferUsing = conveyor.F05_BufferUsing - 1;
                        if (conveyor.F05_BufferUsing == 0)
                        {
                            conveyor.F05_StrRtrSts = "0";
                        }
                        conveyor.F05_UpdateDate = DateTime.Now;
                        _unitOfWork.ConveyorRepository.Update(conveyor);
                    }
                    catch (Exception)
                    {
                      
                        nRetCode = -1;
                    }


                    nRecordCount = nRetCode;
                    //var conveyor1 =
                    //    _unitOfWork.ConveyorRepository.GetMany(
                    //        i => i.F05_ConveyorCode == lpConveyer && i.F05_BufferUsing == 0).FirstOrDefault();
                    //conveyor1.F05_StrRtrSts = "0";
                    //conveyor1.F05_UpdateDate = DateTime.Now;
                    //_unitOfWork.ConveyorRepository.Update(conveyor1);
                }
                else
                {
                    // If status == 9 then this conveyor is in error status
                    var conveyor1 =
                        _unitOfWork.ConveyorRepository.GetById(lpConveyer.Trim());
                    conveyor1.F05_StrRtrSts = "0";
                    conveyor1.F05_UpdateDate = DateTime.Now;
                    _unitOfWork.ConveyorRepository.Update(conveyor1);
                }
                _unitOfWork.Commit();
            }
            catch (Exception)
            {
                //todo Can’t set Conveyor Status
                nRetCode = -1;
            }
            if (nStatus == 0)
            {
                return nRecordCount;
            }
            else
            {
                return nRetCode;
            }
        }

        /// <summary>
        /// Insert a record into no manage table and calculate the seq no.
        /// </summary>
        /// <returns></returns>
        public async Task<int> InsertIntoNoManageAsync()
        {
            // Find all no manage records.
            var noManages = _unitOfWork.NoManageRepository.GetAll();

            // Find the no manage whose system id is "00000"
            var noManage = await noManages.FirstOrDefaultAsync(x => "00000".Equals(x.F48_SystemId.Trim()));

            if (noManage != null)
            {
                //	Increase [f48_mtrwhscmdno] by 1.
                noManage.F48_PdtWhsCmdNo += 1;
                //	If [f48_mtrwhscmdno] is greater than 9999, the system will:
                if (noManage.F48_PdtWhsCmdNo > 9999)
                {
                    //	Update [f48_mtrwhscmdno] as 1.
                    noManage.F48_PdtWhsCmdNo = 1;
                }
                //	Update [f48_updatedate] as current date and time.
                noManage.F48_UpdateDate = DateTime.Now;
                _unitOfWork.NoManageRepository.Update(noManage);
            }
            else
            {
                //insert tx48
                noManage = new TX48_NoManage();
                noManage.F48_SystemId = "00000";
                noManage.F48_MegaKndCmdNo = 0;
                noManage.F48_GnrKndCmdNo = 0;
                noManage.F48_MtrWhsCmdNo = 0;
                noManage.F48_PrePdtWhsCmdNo = 0;
                noManage.F48_PdtWhsCmdNo = 1;
                noManage.F48_KndCmdBookNo = 0;
                noManage.F48_AddDate = DateTime.Now;
                noManage.F48_UpdateDate = DateTime.Now;
                noManage.F48_KneadSheefNo = 0;
                noManage.F48_OutKndCmdNo = 0;
                noManage.F48_CnrKndCmdNo = 0;
                _unitOfWork.NoManageRepository.Add(noManage);
            }

            await _unitOfWork.CommitAsync();
            return noManage.F48_PdtWhsCmdNo;
        }

        //public ResponseResult CheckingStatusOfConveyorAndPreproductWarehouse(string terminalNo)
        //{
        //    var conveyor = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo);
        //    if (conveyor == null)
        //    {
        //        return new ResponseResult(false, "MSG13");
        //    }
        //    var deviceCode = "";
        //    switch (conveyor.F05_StrRtrSts)
        //    {
        //        case "0":
        //            deviceCode = WebConfigurationManager.AppSettings["MaterialDeviceCode"];
        //            break;
        //        case "1":
        //            deviceCode = WebConfigurationManager.AppSettings["PreProductDeviceCode"];
        //            break;
        //        case "2":
        //            deviceCode = WebConfigurationManager.AppSettings["ProductDeviceCode"];

        //            break;
        //        case "9":
        //            return new ResponseResult(false, "MSG13");
        //    }
        //    var device = _unitOfWork.DeviceRepository.GetById(deviceCode);
        //    if (device == null)
        //    {
        //        return new ResponseResult(false, "MSG14");
        //    }
        //    if (device.F14_DeviceStatus != Constants.F14_DeviceStatus.Online.ToString("d") ||
        //        device.F14_UsePermitClass == Constants.F14_UsePermitClass.Prohibited.ToString("d"))
        //    {
        //        return new ResponseResult(false, "MSG14");
        //    }
        //    return new ResponseResult(true);
        //}

        public ResponseResult CheckStockConveyor(string terminalNo, int stockType)
        {
            var conveyor = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo);
            if (conveyor == null)
            {
                return new ResponseResult(false, "MSG13");
            }
            var deviceCode = "";
            switch (stockType)
            {
                case 0:
                    deviceCode = _configurationService.MaterialDeviceCode;
                    break;
                case 1:
                    deviceCode = _configurationService.PreProductDeviceCode;
                    break;
                case 2:
                    deviceCode = _configurationService.ProductDeviceCode;
                    break;
            }
            var device = _unitOfWork.DeviceRepository.GetById(deviceCode);
            if (device == null)
            {
                return new ResponseResult(false, "MSG14");
            }
            if (device.F14_DeviceStatus != Constants.F14_DeviceStatus.Online.ToString("d") ||
                device.F14_UsePermitClass == Constants.F14_UsePermitClass.Prohibited.ToString("d"))
            {
                return new ResponseResult(false, "MSG14");
            }
            return new ResponseResult(true);
        }

        #region References

        /// <summary>
        /// (Reference xxii - SRS 1.1 Sign off)
        /// </summary>
        /// <returns></returns>
        public async Task<TX53_OutSidePrePdtStk> FindProductPalletNoAsync(string commandNo, string preProductCode,
            string preProductLotNo)
        {
            //o	Get Command No, Pre-product Code and Pre-product Lot No of selected row.
            //o	System will get pallet no for the pre-product by doing as follow:
            /*
             * •	Suppose Pallet No and Updated Date are temporary variables which are retrieved respectively from [f53_palletno] and [f53_updatedate] in TX53_OUTSIDEPREPDTSTK, in which:
             * 	[f53_outsideprepdtcode] = Pre-product Code of selected row,
             * 	AND [f53_outsideprepdtlotno] = Pre-product Lot No of selected row,
             * 	AND [f53_kndcmdno] = Command No of selected row,
             * 	AND [f53_stockflag] = “In Stock” (or 3), 
             * 	Ascending order by [f53_palletseqno].
             */
            var outsidePreProductStocks = _unitOfWork.OutSidePrePdtStkRepository.GetAll();
            outsidePreProductStocks =
                outsidePreProductStocks.Where(x => x.F53_OutSidePrePdtCode.Trim().Equals(preProductCode)
                                                   && x.F53_OutSidePrePdtLotNo.Trim().Equals(preProductLotNo)
                                                   && x.F53_KndCmdNo.Trim().Equals(commandNo)
                                                   &&
                                                   Constants.F53_StokcFlag.TX53_StkFlg_Stk.Equals(x.F53_StockFlag.Trim()))
                    .OrderBy(x => x.F53_PalletSeqNo);

            return await outsidePreProductStocks.FirstOrDefaultAsync();
        }

        /// <summary>
        /// (Reference xxiv - SRS 1.1 Sign off)
        /// </summary>
        /// <returns></returns>
        public async Task<int> UpdateProductShelfStockAsync(string row, string bay, string level, string status,
            DateTime updateDate)
        {
            /*
             * Update TX51_PDTSHFSTS, in which: 
             * 	[f51_shelfrow] = Row above,
             * 	AND [f51_shelfbay] = Bay above, 
             * 	AND [f51_shelflevel] = Level above,
             * 	AND [f51_shelfstatus] = “External Pre-Products Stocked” (or 8),
             * 	AND [f51_updatedate] = Updated Date 1 above.
             */
            var productShelfStocks = _unitOfWork.ProductShelfStatusRepository.GetAll();
            productShelfStocks = productShelfStocks.Where(x =>
                x.F51_ShelfRow.Trim().Equals(row)
                && x.F51_ShelfBay.Trim().Equals(bay)
                && x.F51_ShelfLevel.Trim().Equals(level)
                && Constants.F51_ShelfStatus.TX51_ShfSts_ExtPrePdt.Equals(x.F51_ShelfStatus.Trim())
                && x.F51_UpdateDate == updateDate);

            // No records found
            var totalRecords = await productShelfStocks.CountAsync();
            if (totalRecords < 1)
                return 0;

            foreach (var productShelfStock in productShelfStocks)
            {
                // TODO: Q&A 28 - 25/11/2016
            }

            await _unitOfWork.CommitAsync();
            return totalRecords;
        }

        /// <summary>
        /// find list material ware house command
        /// </summary>
        /// <returns></returns>
        public IQueryable<TX34_MtrWhsCmd> GetListMaterialWarehouseCommand()
        {
            var pictureNo1 = Constants.PictureNo.TCRM031F;
            var pictureNo2 = Constants.PictureNo.TCRM051F;

            var status1 = Constants.F34_Status.status6;
            var status2 = Constants.F34_Status.status7;
            var status3 = Constants.F34_Status.status8;
            var status4 = Constants.F34_Status.status9;

            //find all material warehouse command 
            var materialWarehouseCommands =
                _unitOfWork.MaterialWarehouseCommandRepository.GetMany(
                    i =>
                        (i.F34_PictureNo.Trim().Equals(pictureNo1) || i.F34_PictureNo.Trim().Equals(pictureNo2)) &&
                        (i.F34_Status.Equals(status1) || i.F34_Status.Equals(status2) || i.F34_Status.Equals(status3) ||
                         i.F34_Status.Equals(status4))).OrderBy(i => i.F34_AddDate);

            return materialWarehouseCommands;
        }

        /// <summary>
        /// Find product row, bay, level from shelf no.
        /// </summary>
        /// <param name="shelfNo"></param>
        /// <returns></returns>
        public ProductShelfNoItem FindProductShelfInformation(string shelfNo)
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

            // Slashed shelf no regex.
            var slashedShelfNoRegex = new Regex(@"^\d{2}\-\d{2}\-\d{2}$");
            var nonSlashedShelfNoRegex = new Regex(@"^\d{6}$");

            var productShelfNoItem = new ProductShelfNoItem();

            // Format of shelf no is : 
            if (slashedShelfNoRegex.IsMatch(shelfNo))
            {
                var infos = shelfNo.Split('-');
                if (infos.Length != 3)
                    throw new Exception(HttpMessages.InvalidShelfNo);

                //•	Set Row = 2 first characters of [Shelf No],
                productShelfNoItem.Row = infos[0];

                //•	Set Bay = 2 middle characters of [Shelf No] starts from the 4th position,
                productShelfNoItem.Bay = infos[1];

                //•	Set Level = 2 last characters of [Shelf No].
                productShelfNoItem.Level = infos[2];

                return productShelfNoItem;
            }

            if (nonSlashedShelfNoRegex.IsMatch(shelfNo))
            {
                //o	Otherwise:
                //•	Set Row = 2 first characters of [Shelf No],
                productShelfNoItem.Row = shelfNo.Substring(0, 2);
                //•	Set Bay = 2 middle characters of [Shelf No] starts from the 3rd position,
                productShelfNoItem.Bay = shelfNo.Substring(3, 2);
                //•	Set Level = 2 last characters of [Shelf No].
                productShelfNoItem.Level = shelfNo.Substring(4, 2);

                return productShelfNoItem;
            }


            return null;
        }

        //BR 8 Checking and update data for Conveyor and Pre-product Warehouse 
       

        #endregion
    }
}