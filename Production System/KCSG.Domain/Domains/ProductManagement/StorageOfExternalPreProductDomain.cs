using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;

namespace KCSG.Domain.Domains.ProductManagement
{
    public class StorageOfExternalPreProductDomain : BaseDomain, IStorageOfExternalPreProductDomain
    {
        #region Properties

        /// <summary>
        /// Service which handles notifications.
        /// </summary>
        private readonly INotificationService _notificationService;
        private readonly IConfigurationService _configurationService;
        #endregion

        #region Constructor

        public StorageOfExternalPreProductDomain(IUnitOfWork unitOfWork, INotificationService notificationService, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
            _configurationService = configurationService;
        }

        #endregion

        public IQueryable<TX42_KndCmd> GetKneadingComnmand(string preProductCode, string lotNo)
        {
            var kneadingCommand =
                _unitOfWork.KneadingCommandRepository.GetMany(
                    i => i.F42_PreProductCode.Trim().Equals(preProductCode) && i.F42_PrePdtLotNo.Trim().Equals(lotNo));

            return kneadingCommand;
        }

        public IQueryable<TX53_OutSidePrePdtStk> GetOutSidePrePdtStks(string palletNo)
        {
            var outSidePreProductStock =
                _unitOfWork.OutSidePrePdtStkRepository.GetMany(i => i.F53_PalletNo.Trim().Equals(palletNo));

            return outSidePreProductStock;
        }

        /// <summary>
        /// If Remaining Amount less 0.005 show comfirm message
        /// </summary>
        /// <param name="palletNo"></param>
        /// <returns></returns>
        public double GetRemainingAmount(string palletNo)
        {
            try
            {
                var productShelfStock =
                    _unitOfWork.ProductShelfStockRepository.GetMany(i => i.F40_PalletNo.Trim().Equals(palletNo))
                        .FirstOrDefault();
                if (productShelfStock == null)
                    return 0;
                var remainingAmount = productShelfStock.F40_Amount - productShelfStock.F40_ShippedAmount;
                return remainingAmount;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// Check Kneading class and status 
        /// refer BR69 - Srs product management sub system v1.1
        /// </summary>
        /// <param name="preProductCode"></param>
        /// <param name="lotNo"></param>
        /// <returns></returns>
        public bool CheckKneadingClassAndStatus(string preProductCode, string lotNo)
        {
            var kneadingCommands =
                _unitOfWork.KneadingCommandRepository.GetMany(
                    i =>
                        i.F42_PreProductCode.Trim().Equals(preProductCode) && i.F42_PrePdtLotNo.Trim().Equals(lotNo.Trim()));

            if (!kneadingCommands.Any())
                return true;

            foreach (var kneadingCommand in kneadingCommands)
            {
                if (kneadingCommand.F42_OutSideClass != Constants.F42_OutSideClass.OutsideProduct.ToString("D"))
                    return false;

                if (kneadingCommand.F42_Status.Trim() != Constants.F42_Status.TX42_Sts_Cmp)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// check the pallet no
        /// refer BR69 - Srs product management sub system v1.1
        /// </summary>
        /// <param name="palletNo"></param>
        /// <returns></returns>
        public bool CheckPalletNo(string palletNo)
        {
            var stockFlag1 = Constants.F40_StockFlag.TX40_StkFlg_Str;
            var stockFlag2 = Constants.F40_StockFlag.TX40_StkFlg_Rtr;
            var stockFlag3 = Constants.F40_StockFlag.TX40_StkFlg_Stk;

            var productShelfStocks =
                _unitOfWork.ProductShelfStockRepository.GetMany(
                    i =>
                        i.F40_PalletNo.Trim().Equals(palletNo) && (i.F40_StockFlag.Equals(stockFlag1) ||
                                                                   i.F40_StockFlag.Equals(stockFlag2) ||
                                                                   i.F40_StockFlag.Equals(stockFlag3)));
            if (productShelfStocks.Any())
                return false;
            return true;
        }

        /// <summary>
        /// Get list record of product shelf stock table(tbl tx40)
        /// </summary>
        /// <param name="palletNo"></param>
        /// <returns></returns>
        public IQueryable<TX40_PdtShfStk> GetProductShelfStock(string palletNo)
        {
            var productShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();
            try
            {
                productShelfStocks =
                    productShelfStocks.Where(i => i.F40_PalletNo.Trim().Equals(palletNo));
            }
            catch (Exception e)
            {
                //stop
            }
            return productShelfStocks;
        }

        //public ResponseResult CheckRemainingAmount(string palletNo)
        //{
        //    var productShelfStocks = GetProductShelfStock(palletNo);
        //    foreach (var productShelfStock in productShelfStocks)
        //    {
        //        if()
        //    }
        //}

        /// <summary>
        /// Delete TX40_PdtShfStk where F40_PalletNo is Pallet No textbox value
        /// </summary>
        /// <param name="palletNo"></param>
        public void DeteteProductShelfStock(string palletNo)
        {
            try
            {
                var result = GetProductShelfStock(palletNo);
                foreach (var item in result)
                {
                    _unitOfWork.ProductShelfStockRepository.Delete(item);
                }
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                //
            }
        }


        /// <summary>
        /// Check Out Side PreProduct Stock Status 
        /// refer BR69 - Srs product management sub system v1.1
        /// </summary>
        /// <param name="palletNo"></param>
        /// <returns></returns>
        public bool CheckOutSidePreProductStockStatus(string palletNo)
        {
            var outSidePreProductStock =
                _unitOfWork.OutSidePrePdtStkRepository.Get(i => i.F53_PalletNo.Trim().Equals(palletNo));
            //o	If no record found, continue the use case
            if (outSidePreProductStock == null)
                return true;

            //If Stock Flag = “Not In Stock” (or 0), get Tabletising Status from [f41_status] in TX41_TBTCMD in which: [f41_kndcmdno] = Command No 1 above and [f41_prepdtlotno] = Outside Pre-product Lotno above
            if (outSidePreProductStock.F53_StockFlag == Constants.F53_StokcFlag.TX53_StkFlg_NotStk)
            {
                var tableCommand =
                    _unitOfWork.TabletCommandRepository.Get(
                        i =>
                            i.F41_KndCmdNo.Trim().Equals(outSidePreProductStock.F53_KndCmdNo.Trim()) &&
                            i.F41_PrePdtLotNo.Trim().Equals(outSidePreProductStock.F53_OutSidePrePdtLotNo.Trim()));

                if(tableCommand==null)
                    return false;
                if (tableCommand.F41_Status == Constants.F41_Status.Tableted ||
                    tableCommand.F41_Status == Constants.F41_Status.Stored)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Store External Pre-product
        /// Refer Br70 - srs product management v1.1
        /// </summary>
        /// <param name="lotNo"></param>
        /// <param name="preProductCode"></param>
        /// <param name="quantity"></param>
        /// <param name="palletNo"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public ResponseResult<string> StoringExternalPreProduct(string lotNo, string preProductCode, double quantity,
            string palletNo, string terminalNo, string deviceCode)
        {
            var lstKneadingCommand = GetKneadingComnmand(preProductCode, lotNo);

            var moth = DateTime.Now.Month < 10
                ? ("0" + DateTime.Now.Month.ToString())
                : DateTime.Now.Month.ToString();

            //	Suppose Kneading Command No is the temporary variable
            var kneadingCommandNo = "001";
            var commandNo4 = "";
            //	System will make out or renew Kneading Command 
            if (!lstKneadingCommand.Any())
            {
                //Find record of TX48_NOMANAGE where [f48_systemid] = “00000”.
                var nomanage = _unitOfWork.NoManageRepository.Get(i => i.F48_SystemId.Trim().Equals("00000"));

                //Insert Or Update data into TX48_NOMANAGE
                if (nomanage != null)
                {
                    nomanage.F48_OutKndCmdNo = nomanage.F48_OutKndCmdNo > 999 ? 1 : nomanage.F48_OutKndCmdNo + 1;
                    nomanage.F48_UpdateDate = DateTime.Now;

                    _unitOfWork.NoManageRepository.Update(nomanage);

                    kneadingCommandNo = "000" + nomanage.F48_OutKndCmdNo.ToString();
                    kneadingCommandNo = kneadingCommandNo.Substring(kneadingCommandNo.Length - 3);
                }
                else
                {
                    _unitOfWork.NoManageRepository.InsertNomanage(0, 0, 0, 1, 0);
                    kneadingCommandNo = "001";
                }

                //Command No 4 is in format of "X" + Current Month + Kneading Command No above. (If Current Month < 10, add "0" before Current Month)

                 commandNo4 = "X" + moth + kneadingCommandNo;

                //Insert data to TX42_KNDCMD
                _unitOfWork.KneadingCommandRepository.InsertKneadingCommand(commandNo4, lotNo, preProductCode,
                    Constants.F42_OutSideClass.OutsideProduct.ToString("D"), Constants.F42_Status.TX42_Sts_Cmp,
                    quantity);
            }
            else
            {
                foreach (var kneadingCommand in lstKneadingCommand)
                {
                    kneadingCommand.F42_ThrowAmount += quantity;
                    kneadingCommand.F42_StgCtnAmt = kneadingCommand.F42_StgCtnAmt + 1;
                    kneadingCommand.F42_UpdateDate = DateTime.Now;
                    commandNo4 = kneadingCommand.F42_KndCmdNo;
                    _unitOfWork.KneadingCommandRepository.Update(kneadingCommand);
                }
            }

            //System will make out external pre-product stock data
            var outSidePreProductStock = GetOutSidePrePdtStks(palletNo).FirstOrDefault();
            if (outSidePreProductStock == null)
            {
                var productShelf = _unitOfWork.ProductShelfRepository.Get(i => i.F57_PalletNo.Trim().Equals(palletNo));

                if (productShelf == null)
                    _unitOfWork.ProductShelfRepository.InsertProductShelf(palletNo, "0", 0);

                /*
                 * [f53_palletseqno]  in TX53_OUTSIDEPREPDTSTK}, in which: 
                 * [f53_outsideprepdtcode] = Pre-product Code textbox value, 
                 * [f53_outsideprepdtlotno] = Lot No textbox value, 
                 * [f53_kndcmdno] = Command No 4 above
               */
               
                var outSidePreProductStocks =
                    _unitOfWork.OutSidePrePdtStkRepository.GetMany(
                        i =>
                            i.F53_OutSidePrePdtCode.Trim().Equals(preProductCode) &&
                            i.F53_OutSidePrePdtLotNo.Trim().Equals(lotNo) && i.F53_KndCmdNo.Trim().Equals(commandNo4))
                        .OrderByDescending(i => i.F53_PalletSeqNo)
                        .FirstOrDefault();
                var palletSeqNo = 1;
                if (outSidePreProductStocks != null)
                {
                    if (outSidePreProductStocks.F53_PalletSeqNo > 0)
                        palletSeqNo = outSidePreProductStocks.F53_PalletSeqNo + 1;
                }

                //Insert  insert data to TX53_OUTSIDEPREPDTSTK
                var outSidePreProductStockItem = new TX53_OutSidePrePdtStk();
                outSidePreProductStockItem.F53_PalletNo = palletNo;
                outSidePreProductStockItem.F53_OutSidePrePdtCode = preProductCode;
                outSidePreProductStockItem.F53_OutSidePrePdtLotNo = lotNo;
                outSidePreProductStockItem.F53_KndCmdNo = commandNo4;
                outSidePreProductStockItem.F53_PalletSeqNo = palletSeqNo;
                outSidePreProductStockItem.F53_Amount = quantity;
                outSidePreProductStockItem.F53_StockFlag = Constants.F53_StokcFlag.TX53_StkFlg_Str;
                outSidePreProductStockItem.F53_AddDate = DateTime.Now;
                outSidePreProductStockItem.F53_UpdateDate = DateTime.Now;
                _unitOfWork.OutSidePrePdtStkRepository.Add(outSidePreProductStockItem);
            }
            else
            {
                outSidePreProductStock.F53_OutSidePrePdtCode = preProductCode;
                outSidePreProductStock.F53_OutSidePrePdtLotNo = lotNo;
                outSidePreProductStock.F53_KndCmdNo = commandNo4;
                outSidePreProductStock.F53_Amount = quantity;
                outSidePreProductStock.F53_StockFlag = Constants.F53_StokcFlag.TX53_StkFlg_NotStk;
                outSidePreProductStock.F53_UpdateDate = DateTime.Now;

                _unitOfWork.OutSidePrePdtStkRepository.Update(outSidePreProductStock);
            }
            _unitOfWork.Commit();

            #region Check conveyor status and device status

            var isCheckedConveyor = CheckStatusAndNumberRecordOfConveyor(terminalNo);
            if (!isCheckedConveyor)
                return new ResponseResult<string>("",false, "MSG8");

            var isCheckedDeviceCode = CheckStatusOfDeviceRecordForProductManagement(deviceCode);
            if (!isCheckedDeviceCode)
                return new ResponseResult<string>("", false, "MSG9");
                

            #endregion

            //o	Suppose Low Temp Class is temporary variable, which is retrieved from [f03_lowtmpclass] in TM03_PREPRODUCT, in which [f03_preproductcode] = Pre-product Code textbox value
            var lowTempClass =
                _unitOfWork.PreProductRepository.Get(i => i.F03_PreProductCode.Trim().Equals(preProductCode))
                    .F03_LowTmpClass;
            var row = "";
            var bay = "";
            var level = "";
            var f51Status = Constants.F51_ShelfStatus.TX51_ShfSts_Epy;
            var f51ShelfType = Constants.F51_ShelfType.Normal.ToString("D");
            var shelfNo = "";
            var productShelfStatuses =
                _unitOfWork.ProductShelfStatusRepository.GetMany(
                    i =>
                        i.F51_ShelfStatus.Equals(f51Status) && i.F51_ShelfType.Equals(f51ShelfType));
            if (lowTempClass == "0")
            {
                productShelfStatuses =
                    productShelfStatuses.Where(i => i.F51_LowTmpShfAgnOrd.HasValue).OrderBy(i => i.F51_LowTmpShfAgnOrd);
                foreach (var productShelfStatus in productShelfStatuses)
                {
                    productShelfStatus.F51_ShelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvStr;
                    productShelfStatus.F51_StockTakingFlag =
                        Constants.F51_StockTakingFlag.TX51_StkTkgFlg_InvNotChk;
                    productShelfStatus.F51_PalletNo = palletNo;
                    productShelfStatus.F51_TerminalNo = terminalNo;

                    _unitOfWork.ProductShelfStatusRepository.Update(productShelfStatus);

                    shelfNo = productShelfStatus.F51_ShelfRow +
                              productShelfStatus.F51_ShelfBay +
                              productShelfStatus.F51_ShelfLevel;
                    break;
                }
            }
            else
            {
                productShelfStatuses =
                    productShelfStatuses.Where(i => i.F51_CmdShfAgnOrd.HasValue).OrderBy(i => i.F51_CmdShfAgnOrd);
                foreach (var productShelfStatus in productShelfStatuses)
                {
                    productShelfStatus.F51_ShelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvStr;
                    productShelfStatus.F51_StockTakingFlag =
                        Constants.F51_StockTakingFlag.TX51_StkTkgFlg_InvNotChk;
                    productShelfStatus.F51_PalletNo = palletNo;
                    productShelfStatus.F51_TerminalNo = terminalNo;

                    _unitOfWork.ProductShelfStatusRepository.Update(productShelfStatus);

                    shelfNo = productShelfStatus.F51_ShelfRow +
                              productShelfStatus.F51_ShelfBay +
                              productShelfStatus.F51_ShelfLevel;
                    break;
                }
            }
            if (String.IsNullOrEmpty(shelfNo))
            {
                return new ResponseResult<string>("",false, "MSG19");
            }

            //	System will make out Storage command by doing as follow:
            var nomanageItem = _unitOfWork.NoManageRepository.Get(i => i.F48_SystemId.Trim().Equals("00000"));

            var serialNo = "0001";

            //Insert Or Update data in TX48_NOMANAGE
            if (nomanageItem != null)
            {
                _unitOfWork.NoManageRepository.UpdateNomanage(nomanageItem);
                serialNo = "0000" + nomanageItem.F48_PdtWhsCmdNo.ToString();
                serialNo = serialNo.Substring(serialNo.Length - 4);
            }
            else
            {
                _unitOfWork.NoManageRepository.InsertNomanage(0, 0, 0, 0, 1);
            }

            //o	Insert data to TX47_PDTWHSCMD
            var from = GetConveyorCode(terminalNo);
            var item = _unitOfWork.ProductWarehouseCommandRepository.InsertProductWarehouseCommand(
                Constants.F47_CommandNo.Storage.ToString("D"), serialNo,
                Constants.CommandType.CmdType_0,
                Constants.F47_StrRtrType.ExternalPreProduct.ToString("D"),
                Constants.F47_Status.AnInstruction.ToString("D"), palletNo, from, shelfNo, terminalNo,
                Constants.PictureNo.TCPR091F);


            _notificationService.SendMessageToC3("TCPR091F",
                _notificationService.FormatThirdCommunicationMessageResponse(item));
            _unitOfWork.Commit();
            return new ResponseResult<string>(commandNo4,true);
        }

        /// <summary>
        /// Complete Kneading Command To Be Stored
        /// Refer BR73 - srs product management v1.1
        /// </summary>
        /// <param name="lotNo"></param>
        /// <param name="preProductCode"></param>
        /// <returns></returns>
        public bool CompletingKneadingCommand(string lotNo, string preProductCode)
        {
            try
            {
                var outSideClass = Constants.F42_OutSideClass.OutsideProduct.ToString("D");
                var status = Constants.F42_Status.TX42_Sts_Cmp;
                var kneadingCommands =
                    _unitOfWork.KneadingCommandRepository.GetMany(
                        i =>
                            i.F42_PrePdtLotNo.Trim().Equals(lotNo) && i.F42_PreProductCode.Trim().Equals(preProductCode) &&
                            i.F42_OutSideClass.Equals(outSideClass) && i.F42_Status.Equals(status));
                if (kneadingCommands.Count() == 0)
                {
                    return false;
                }
                foreach (var kneadingCommand in kneadingCommands)
                {
                    var kneadingCommandItem =
                        _unitOfWork.KneadingCommandRepository.Get(
                            i =>
                                i.F42_KndCmdNo.Trim().Equals(kneadingCommand.F42_KndCmdNo.Trim()) &&
                                i.F42_PrePdtLotNo.Trim().Equals(lotNo) && i.F42_Status.Equals(status));
                    //	Set [f42_status] = “Storage complete” (or 4).
                    kneadingCommandItem.F42_Status = Constants.F42_Status.TX42_Sts_Stored;
                    _unitOfWork.KneadingCommandRepository.Update(kneadingCommandItem);
                }
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<ThirdCommunicationResponse> RespondingReplyFromC3(string terminalNo, string lotNo,
            string preProductCode,string is_kndcmdno)
        {
            var ls_ShelfNo = ""; //shelf no.
            var lstStatus = new List<string>() {"6", "7", "8"};
            var lstTx47 =
                _unitOfWork.ProductWarehouseCommandRepository.GetByTerminalNoAndPictureNoAndStatus(terminalNo,
                    Constants.PictureNo.TCPR091F, lstStatus);

            var items = new List<ThirdCommunicationResponse>();

            foreach (var tx47 in lstTx47)
            {
                var lstPreproductionCode =
                            _unitOfWork.OutSidePrePdtStkRepository.GetMany(i => i.F53_PalletNo == tx47.F47_PalletNo)
                                .Select(i => i.F53_OutSidePrePdtCode.Trim());

                var outSidePrePdtStk = lstPreproductionCode.FirstOrDefault();

                var item = Mapper.Map<ThirdCommunicationResponse>(tx47);
                item.OldStatus = tx47.F47_Status;
                //item.ProductCode = outSidePrePdtStk;
                item.ProductCode = preProductCode;

                var newStatus = "";
                switch (tx47.F47_Status[0])
                {
                    case '6': //Command End
                        newStatus = "C";
                        break;
                    case '7': //Command Cancel
                        newStatus = "D";
                        resetkndcmd(tx47.F47_CommandNo, tx47.F47_CmdSeqNo, lotNo, preProductCode, is_kndcmdno);
                        break;
                    case '8': //Command Cancel
                        
                        var tm03 =
                            _unitOfWork.PreProductRepository.GetMany(
                                i => lstPreproductionCode.Contains(i.F03_PreProductCode.Trim())).FirstOrDefault();
                        if (!wf_assignspaceshelf(ref ls_ShelfNo, tx47.F47_PalletNo, tm03.F03_LowTmpClass, terminalNo))
                        {
                            resetkndcmd(tx47.F47_CommandNo, tx47.F47_CmdSeqNo, lotNo, preProductCode, is_kndcmdno);
                            newStatus = "B";
                        }
                        else
                        {
                            insertcommand(tx47.F47_To, Constants.F47_CommandNo.TwoTimes.ToString("D"), terminalNo,
                                tx47.F47_PalletNo);
                         
                            newStatus = "E";
                        }
                        break;
                }

                items.Add(item);
                _unitOfWork.ProductWarehouseCommandRepository.Update(tx47);

                // Broadcast notification to C3.
                _notificationService.SendMessageToC3("TCPR051F",
                    _notificationService.FormatThirdCommunicationMessageResponse(tx47));
                tx47.F47_Status = newStatus;
                tx47.F47_UpdateDate = DateTime.Now;
                _unitOfWork.ProductWarehouseCommandRepository.Update(tx47);
               
            }
            try
            {

                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
            }
            return items;
        }

        private bool resetkndcmd(string cmdNo, string cmdSeqNo, string lotNo, string preProductCode, string is_kndcmdno)
        {
            var lr_amount = 0;
            int li_num = 0;
            //string is_kndcmdno = "";
            //_unitOfWork.NoManageRepository.GetkndNo("2", ref is_kndcmdno, ref li_num);
            //is_kndcmdno = "X" + DateTime.Now.Month.ToString("d2") + is_kndcmdno;
            var tx42 =
                _unitOfWork.KneadingCommandRepository.GetMany(
                    i =>
                        i.F42_PrePdtLotNo.Trim().Equals(lotNo.Trim()) &&
                        i.F42_PreProductCode.Trim().Equals(preProductCode.Trim())).FirstOrDefault();
            if (tx42 == null)
            {
                _unitOfWork.KneadingCommandRepository.Delete(
                    i =>
                        i.F42_KndCmdNo.Trim().Equals(is_kndcmdno.Trim()) &&
                        i.F42_PrePdtLotNo.Trim().Equals(lotNo.Trim()));
            }
            else
            {
                var ii_stgctnamt = tx42.F42_StgCtnAmt- 1;
                if (ii_stgctnamt == 0)
                {
                    _unitOfWork.KneadingCommandRepository.Delete(
                        i =>
                            i.F42_KndCmdNo.Trim().Equals(is_kndcmdno.Trim()) &&
                            i.F42_PrePdtLotNo.Trim().Equals(lotNo.Trim()));
                }
                else
                {
                    var paletno =
                        _unitOfWork.ProductWarehouseCommandRepository.GetByCommondNoAndSeqNo(cmdNo, cmdSeqNo)
                            .F47_PalletNo;
                    var tx53 =
                        _unitOfWork.OutSidePrePdtStkRepository.GetMany(i => i.F53_PalletNo.Trim().Equals(paletno.Trim()))
                            .FirstOrDefault();
                    if (tx53 == null)
                    {
                        //todo linhnd20 show message : "This record was modified by others, please retry!"
                        return false;
                    }
                    var ir_throwamount = tx42.F42_ThrowAmount - tx53.F53_Amount;
                    var tx42update =
    _unitOfWork.KneadingCommandRepository.GetMany(
        i =>
                        i.F42_KndCmdNo.Trim().Equals(is_kndcmdno.Trim()) &&
                        i.F42_PrePdtLotNo.Trim().Equals(lotNo.Trim())).FirstOrDefault();
                    tx42update.F42_ThrowAmount = ir_throwamount;
                    tx42update.F42_StgCtnAmt = ii_stgctnamt;
                    try
                    {

                        _unitOfWork.KneadingCommandRepository.Update(tx42update);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            
            return true;
        }

        public bool wf_assignspaceshelf(ref string as_shelfno, string as_palletno, string ach_lowtmp, string terminalNo)
        {
            if (!CheckStockConveyor(terminalNo, 2).IsSuccess)
            {
                return false;
            }
            try
            {
                var lstTx51 =
                    _unitOfWork.ProductShelfStatusRepository.GetMany(
                        i => i.F51_ShelfStatus == "0" && i.F51_ShelfType == "0");

                if (ach_lowtmp == Constants.Temperature.Low.ToString("D"))
                {
                    lstTx51 = lstTx51.Where(i => i.F51_LowTmpShfAgnOrd.HasValue).OrderBy(i => i.F51_LowTmpShfAgnOrd);
                }
                else
                {
                    lstTx51 = lstTx51.Where(i => i.F51_CmdShfAgnOrd.HasValue).OrderBy(i => i.F51_CmdShfAgnOrd);
                }
                foreach (var tx51 in lstTx51)
                {
                    tx51.F51_ShelfStatus = Constants.TX51SheflStatus.TX51_ShfSts_RsvStr.ToString("D");
                    tx51.F51_StockTakingFlag = Constants.F51_StockTakingFlag.TX51_StkTkgFlg_InvNotChk;
                    tx51.F51_PalletNo = as_palletno;
                    tx51.F51_TerminalNo = terminalNo;
                    tx51.F51_UpdateDate = DateTime.Now;
                    _unitOfWork.ProductShelfStatusRepository.Update(tx51);
                    as_shelfno = tx51.F51_ShelfRow + tx51.F51_ShelfBay + tx51.F51_ShelfLevel;
                    break;
                }
            }
            catch (Exception ex)
            {
                return false;
            }


            return true;
        }

        private bool insertcommand(string as_shelfno, string as_type, string terminalNo, string palletNo)
        {
            var isNoManage = true;
            var serialNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                Constants.GetColumnInNoManager.Pdtwhscmdno, 0, 0, 0, 0, 1).ToString("D4");
            var gs_cnvcode = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo).F05_ConveyorCode;
            var txt47 = new TX47_PdtWhsCmd()
            {
                F47_CommandNo = as_type,
                F47_CmdSeqNo = serialNo,
                F47_CommandType = Constants.CmdType.cmdType,
                F47_StrRtrType = Constants.F47_StrRtrType.ExternalPreProduct.ToString("D"),
                F47_Status = "0",
                F47_Priority = 0,
                F47_PalletNo = palletNo,
                F47_From = gs_cnvcode,
                F47_To = as_shelfno,
                F47_TerminalNo = terminalNo,
                F47_PictureNo = Constants.PictureNo.TCPR091F,
                F47_RetryCount = 0,
                F47_AddDate = DateTime.Now,
                F47_UpdateDate = DateTime.Now
            };
            _unitOfWork.ProductWarehouseCommandRepository.Add(txt47);
            
            return true;
        }
    }
}