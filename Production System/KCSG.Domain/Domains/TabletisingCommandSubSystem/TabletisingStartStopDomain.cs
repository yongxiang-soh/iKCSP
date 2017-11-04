using System;
using System.Collections.Generic;
using System.Linq;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Interfaces.TabletisingCommandSubSystem;
using KCSG.Domain.Models;
using KCSG.Domain.Models.TabletisingCommondSubSystem;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.TabletisingCommandSubSystem
{
    public class TabletisingStartStopDomain :BaseDomain, ITabletisingStartStopDomain
    {
        private readonly ICommonDomain _commonDomain;
        private readonly IUnitOfWork _unitOfWork;

        #region Constructor

        public TabletisingStartStopDomain(
            IUnitOfWork iUnitOfWork,
            ICommonDomain commonDomain, IConfigurationService configurationService)
            : base(iUnitOfWork, configurationService)
        {
            _unitOfWork = iUnitOfWork;
            _commonDomain = commonDomain;
        }

        #endregion

        public ResponseResult<GridResponse<TabletisingStartStopControlItem>> SearchCriteria(GridSettings gridSettings,
            string terminalNo)
        {
            //find as_line is value of [f06_tabletline] on “tm06_terminal” where tm06_terminal.f06_terminalno = Terminal No. of the current terminal.
            var terminal = _unitOfWork.TerminalRepository.Get(i => i.F06_TerminalNo.Trim().Equals(terminalNo));
            var asLine = terminal.F06_TabletLine;

            //find record in TX41_TbtCmd where f41_status different 7 or 8 and f41_tabletLine equals asLine
            var tabletCommands =
                _unitOfWork.TabletCommandRepository.GetAll()
                    .Where(
                        i =>
                            (i.F41_Status != Constants.F41_Status.Tableted) &&
                            (i.F41_Status != Constants.F41_Status.Stored) &&
                            i.F41_TabletLine.Trim().Equals(asLine.Trim()) &&
                            (i.F41_RtrEndCntAmt > 0));

            //get all record in TM03_PreProduct
            var preProducts = _unitOfWork.PreProductRepository.GetAll();

            //get all record in TX49_PrePdtShfStk
            var preProdcutShelfStocks = _unitOfWork.PreProductShelfStockRepository.GetAll();

            //get all record  in TX53_OutSidePrePdtStk
            var outSidePreProductStocks = _unitOfWork.OutSidePrePdtStkRepository.GetAll();

            //get all record in TX57_PdtShf
            var productShelfs = _unitOfWork.ProductShelfRepository.GetAll();

            var lstResult = new List<TabletisingStartStopControlItem>();

            #region Get List item for tm03_preproduct and tx49_prepdtshfstk and tx41_tbtcmd

            var results = from tabletCommand in tabletCommands
                from preProduct in preProducts
                from preProdcutShelfStock in preProdcutShelfStocks
                where tabletCommand.F41_PreproductCode.Equals(preProduct.F03_PreProductCode) &&
                      tabletCommand.F41_PrePdtLotNo.Equals(preProdcutShelfStock.F49_PreProductLotNo) &&
                      preProduct.F03_PreProductCode.Equals(preProdcutShelfStock.F49_PreProductCode)
                orderby new
                {
                    tabletCommand.F41_KndCmdNo,
                    tabletCommand.F41_PrePdtLotNo,
                    preProdcutShelfStock.F49_ContainerCode
                }
                select new
                {
                    tabletCommand.F41_KndCmdNo,
                    tabletCommand.F41_PreproductCode,
                    tabletCommand.F41_PrePdtLotNo,
                    tabletCommand.F41_Status,
                    tabletCommand.F41_TabletLine,
                    preProduct.F03_PreProductName,
                    preProduct.F03_TmpRetTime,
                    preProdcutShelfStock.F49_ContainerCode,
                    preProdcutShelfStock.F49_Amount,
                    preProdcutShelfStock.F49_RetrievalDate
                };

            foreach (var item in results)
            {
                var tabletisingStartStopContolItem = new TabletisingStartStopControlItem();

                tabletisingStartStopContolItem.F41_KndCmdNo = item.F41_KndCmdNo;
                tabletisingStartStopContolItem.F41_PreproductCode = item.F41_PreproductCode;
                tabletisingStartStopContolItem.F41_PrePdtLotNo = item.F41_PrePdtLotNo;
                tabletisingStartStopContolItem.F41_Status = item.F41_Status;
                tabletisingStartStopContolItem.F41_TabletLine = item.F41_TabletLine;
                tabletisingStartStopContolItem.PreProductName = item.F03_PreProductName;
                tabletisingStartStopContolItem.TmpReturnTime = item.F03_TmpRetTime;

                tabletisingStartStopContolItem.F49_ContainerCode = item.F49_ContainerCode;
                tabletisingStartStopContolItem.Quantity = item.F49_Amount;
                tabletisingStartStopContolItem.RetrievalDate = item.F49_RetrievalDate;

                if (item.F49_RetrievalDate != null)
                {
                    var remainDateTime = DateTime.Now - item.F49_RetrievalDate;

                    tabletisingStartStopContolItem.PassedTime = remainDateTime.Value.Days + " day " +
                                                                remainDateTime.Value.Hours + " : " +
                                                                remainDateTime.Value.Minutes + " : " +
                                                                remainDateTime.Value.Seconds;
                }
                lstResult.Add(tabletisingStartStopContolItem);
            }

            #endregion

            #region Get List item for tm03_preproduct and tx53_outsideprepdtstk and tx41_tbtcmd and tx57_pdtshf

            var result2 = from tabletCommand in tabletCommands
                from preProduct in preProducts
                from outSidePreProductStock in outSidePreProductStocks
                from productShelf in productShelfs
                where tabletCommand.F41_PreproductCode.Equals(outSidePreProductStock.F53_OutSidePrePdtCode) &&
                      tabletCommand.F41_PrePdtLotNo.Equals(outSidePreProductStock.F53_OutSidePrePdtLotNo) &&
                      outSidePreProductStock.F53_PalletNo.Equals(productShelf.F57_PalletNo) &&
                      tabletCommand.F41_PreproductCode.Equals(preProduct.F03_PreProductCode)
                orderby new
                {
                    tabletCommand.F41_KndCmdNo,
                    tabletCommand.F41_PrePdtLotNo,
                    outSidePreProductStock.F53_PalletNo
                }
                select new
                {
                    tabletCommand.F41_KndCmdNo,
                    tabletCommand.F41_PreproductCode,
                    tabletCommand.F41_PrePdtLotNo,
                    tabletCommand.F41_Status,
                    tabletCommand.F41_TabletLine,
                    preProduct.F03_PreProductName,
                    preProduct.F03_TmpRetTime,
                    outSidePreProductStock.F53_PalletNo,
                    outSidePreProductStock.F53_Amount,
                    productShelf.F57_RetievalDate
                };

            foreach (var item2 in result2)
            {
                var tabletisingStartStopContolItem = new TabletisingStartStopControlItem();

                tabletisingStartStopContolItem.F41_KndCmdNo = item2.F41_KndCmdNo;
                tabletisingStartStopContolItem.F41_PreproductCode = item2.F41_PreproductCode;
                tabletisingStartStopContolItem.F41_PrePdtLotNo = item2.F41_PrePdtLotNo;
                tabletisingStartStopContolItem.F41_Status = item2.F41_Status;
                tabletisingStartStopContolItem.F41_TabletLine = item2.F41_TabletLine;
                tabletisingStartStopContolItem.PreProductName = item2.F03_PreProductName;
                tabletisingStartStopContolItem.TmpReturnTime = item2.F03_TmpRetTime;

                tabletisingStartStopContolItem.F49_ContainerCode = item2.F53_PalletNo;
                tabletisingStartStopContolItem.Quantity = item2.F53_Amount;
                tabletisingStartStopContolItem.RetrievalDate = item2.F57_RetievalDate;
                if (item2.F57_RetievalDate != null)
                {
                    var remainDateTime = DateTime.Now - item2.F57_RetievalDate;

                    tabletisingStartStopContolItem.PassedTime = remainDateTime.Value.Days + " day " +
                                                                remainDateTime.Value.Hours + " : " +
                                                                remainDateTime.Value.Minutes + " : " +
                                                                remainDateTime.Value.Seconds;
                }
                lstResult.Add(tabletisingStartStopContolItem);
            }

            var itemCount = lstResult.Count;
            var queryable = lstResult.AsQueryable();
            OrderByAndPaging(ref queryable, gridSettings);
            #endregion

            var resultModel = new GridResponse<TabletisingStartStopControlItem>(queryable, itemCount);
            return new ResponseResult<GridResponse<TabletisingStartStopControlItem>>(resultModel, true);
        }


        public ResponseResult<GridResponse<TabletisingStarStopSelectItem>> Selected(string cmdno, string lotno,
            GridSettings gridSettings)
        {
            if ((cmdno == null) || (lotno == null))
                return new ResponseResult<GridResponse<TabletisingStarStopSelectItem>>(null, false);
            var result =
                _unitOfWork.TabletProductRepository.GetMany(
                    i => i.F56_KndCmdNo.Trim().Equals(cmdno) && i.F56_PrePdtLotNo.Trim().Equals(lotno));

            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var lstProductCode = result.Select(i => i.F56_ProductCode);
            var lstProduct =
                _unitOfWork.ProductRepository.GetAll().Where(i => lstProductCode.Contains(i.F09_ProductCode));
            var lstResult = new List<TabletisingStarStopSelectItem>();

            foreach (var item in result)
            {
                var tabletisingSelectItem = new TabletisingStarStopSelectItem();
                var productItem = lstProduct.FirstOrDefault(i => i.F09_ProductCode.Equals(item.F56_ProductCode));

                tabletisingSelectItem.F56_ProductCode = item.F56_ProductCode;
                tabletisingSelectItem.F56_ProductLotNo = item.F56_ProductLotNo;
                tabletisingSelectItem.F56_TbtCmdAmt = item.F56_TbtCmdAmt;
                tabletisingSelectItem.F56_TbtCmdEndPackAmt = item.F56_TbtCmdEndPackAmt;
                tabletisingSelectItem.F56_TbtCmdEndFrtAmt = item.F56_TbtCmdEndFrtAmt;
                tabletisingSelectItem.F56_Status = item.F56_Status;
                tabletisingSelectItem.F56_PrePdtLotNo = item.F56_PrePdtLotNo;

                if (productItem != null)
                {
                    tabletisingSelectItem.ProductName = productItem.F09_ProductDesp;
                    tabletisingSelectItem.PackUnit = productItem.F09_PackingUnit;
                }
                lstResult.Add(tabletisingSelectItem);
            }
            var resultModel = new GridResponse<TabletisingStarStopSelectItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<TabletisingStarStopSelectItem>>(resultModel, true);
        }


        public bool Start(string commandNo, string lotNo, string preProductCode, string productCode)
        {
            try
            {
                //get record in TX56_TbtPdt
                var tableProduct =
                    _unitOfWork.TabletProductRepository.Get(
                        i =>
                            i.F56_KndCmdNo.Trim().Equals(commandNo) && i.F56_PrePdtLotNo.Trim().Equals(lotNo) &&
                            i.F56_ProductCode.Trim().Equals(productCode));
                if (tableProduct != null)
                {
                    tableProduct.F56_Status = Constants.F56_Status.Tableting;
                    _unitOfWork.TabletProductRepository.Update(tableProduct);
                }

                var firtChar = commandNo[0];
                var tx41TbtCmd =
                    _unitOfWork.TabletCommandRepository.GetMany(
                            i =>
                                i.F41_PreproductCode.Trim().Equals(preProductCode) &&
                                i.F41_PrePdtLotNo.Trim().Equals(lotNo) &&
                                i.F41_KndCmdNo.Trim().Equals(commandNo))
                        .FirstOrDefault();
                if (tx41TbtCmd != null)
                {
                    if (firtChar == 'X')
                        tx41TbtCmd.F41_Status = Constants.F41_Status.ContainerSet;
                    tx41TbtCmd.F41_TbtBgnDate = DateTime.Now;
                    _unitOfWork.Commit();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool End(EndTabletisingItem model)
        {
            var tbtpdt =
                _unitOfWork.TabletProductRepository.GetMany(
                    i =>
                        i.F56_PrePdtLotNo.Trim().Equals(model.PreProductLotNo.Trim()) &&
                        i.F56_ProductCode.Trim().Equals(model.ProductCode.Trim()) &&
                        i.F56_KndCmdNo.Trim().Equals(model.CommandNo.Trim()) &&
                        i.F56_Status.Equals(Constants.F56_Status.Tableting)).FirstOrDefault();
            if (tbtpdt != null)
            {
                tbtpdt.F56_TbtCmdEndFrtAmt = model.Fraction;
                tbtpdt.F56_TbtCmdEndPackAmt = model.Package;
                tbtpdt.F56_TbtCmdEndAmt = model.Package*model.PackingUnit+model.Fraction;
                tbtpdt.F56_TbtEndDate = DateTime.Now;
                tbtpdt.F56_Status = Constants.F56_Status.TabletingOver;
                _unitOfWork.TabletProductRepository.Update(tbtpdt);

                //Update TX41_TbtCmd tabletising command status
                var result =
                _unitOfWork.TabletCommandRepository.Get(
                    i => i.F41_KndCmdNo.Trim().Equals(model.CommandNo.Trim()) && 
                    i.F41_PrePdtLotNo.Trim().Equals(model.PreProductLotNo.Trim()));
              
                    result.F41_Status = Constants.F41_Status.Tableted;
                    result.F41_UpdateDate = DateTime.Now;
                    result.F41_TbtEndDate = DateTime.Now;
                _unitOfWork.TabletCommandRepository.Update(result);
            }

            var lstpckmtr =
                _unitOfWork.PckMtrRepository.GetMany(i => i.F11_ProductCode.Trim() == model.ProductCode.Trim());
            var checkNotEnough = false;
            foreach (var tm11PckMtr in lstpckmtr)
            {
                var amount = tm11PckMtr.F11_Amount*model.Package;
                var tx46supmatstk =
                    _unitOfWork.SupMaterialStockRepository.GetMany(
                            i => i.F46_SubMaterialCode.Trim().Equals(tm11PckMtr.F11_SubMaterialCode.Trim()))
                        .FirstOrDefault();
                if (tx46supmatstk != null)
                    if (amount >= tx46supmatstk.F46_Amount)
                    {
                        amount = tx46supmatstk.F46_Amount;
                        tx46supmatstk.F46_Amount = 0;
                        _unitOfWork.SupMaterialStockRepository.Update(tx46supmatstk);
                        checkNotEnough = true;
                    }
                    else
                    {
                        tx46supmatstk.F46_Amount = tx46supmatstk.F46_Amount - amount;
                        _unitOfWork.SupMaterialStockRepository.Update(tx46supmatstk);
                    }
            }
            _unitOfWork.Commit();
            return checkNotEnough;
        }

        public bool ValidationForEndButton(string cmdNo)
        {
            var result = _unitOfWork.TabletCommandRepository.GetMany(i => i.F41_KndCmdNo.Equals(cmdNo)).FirstOrDefault();
            var totalAmount = result.F41_TblCntAmt;
            var outoverAmount = result.F41_RtrEndCntAmt;

            return totalAmount > outoverAmount ? true : false;
        }


        public bool ContainerSet(string cmdNo, string lotNo)
        {
            var result =
                _unitOfWork.TabletCommandRepository.Get(
                    i => i.F41_KndCmdNo.Trim().Equals(cmdNo.Trim()) && i.F41_PrePdtLotNo.Trim().Equals(lotNo.Trim()));
            if (result.F41_RtrEndCntAmt > result.F41_ChgCntAmt)
            {
                result.F41_Status = Constants.F41_Status.ContainerSetWait;
                result.F41_UpdateDate = DateTime.Now;

                _unitOfWork.TabletCommandRepository.Update(result);
                _unitOfWork.Commit();
                return true;
            }
            return false;
        }

        public ResponseResult TimeJob(string cmdNo, string productCode, string lotNo, string lowerLotNo)
        {
            var result =
                _unitOfWork.TabletCommandRepository.GetMany(
                        i => i.F41_KndCmdNo.Trim().Equals(cmdNo.Trim()) && i.F41_PrePdtLotNo.Trim().Equals(lotNo.Trim()))
                    .FirstOrDefault();
            result.F41_Status = Constants.F41_Status.Default;

            _unitOfWork.TabletCommandRepository.Update(result);
            _unitOfWork.Commit();

            var tabletProductItem =
                _unitOfWork.TabletProductRepository.GetMany(
                    i =>
                        i.F56_KndCmdNo.Trim().Equals(cmdNo.Trim()) &&
                        i.F56_ProductLotNo.Trim().Equals(lowerLotNo.Trim()) &&
                        i.F56_ProductCode.Trim().Equals(productCode.Trim()) &&
                        (i.F56_Status.Equals(Constants.F56_Status.NotTablet) ||
                         i.F56_Status.Equals(Constants.F56_Status.Tableting))).FirstOrDefault();
            if (tabletProductItem != null)
                if (result.F41_RtrEndCntAmt < result.F41_ChgCntAmt)
                    return new ResponseResult(true, "editable");
                else
                    return new ResponseResult(true, "noteditable");
            return new ResponseResult(false);
        }


        public bool CheckedStatus(string cmdNo, string lotNo)
        {
            var result =
                _unitOfWork.TabletProductRepository.GetAll().Any(i =>
                    i.F56_KndCmdNo.Trim().Equals(cmdNo.Trim()) && i.F56_PrePdtLotNo.Trim().Equals(lotNo.Trim()) &&
                    ((i.F56_Status == Constants.F56_Status.NotTablet) ||
                     (i.F56_Status == Constants.F56_Status.Tableting) ||
                     (i.F56_Status == Constants.F56_Status.Change)));

            if (result || (cmdNo[0] == 'X'))
                return true;
            return false;
        }

        //    if (kndCmdItem != null)
        //    tbtCmdItem.F41_PreproductCode = preProductCode;
        //    tbtCmdItem.F41_PrePdtLotNo = lotNo;
        //    tbtCmdItem.F41_KndCmdNo = kneadingNo;


        //    var tbtCmdItem = new TX41_TbtCmd();

        //    var kndCmdItem = _unitOfWork.KneadingCommandRepository.GetMany(i=>i.F42_KndCmdNo.Equals(kneadingNo) && i.F42_PreProductCode.Equals(preProductCode)).FirstOrDefault();
        //    _unitOfWork.TabletCommandRepository.Delete(i=>i.F41_KndCmdNo.Equals(kneadingNo) && i.F41_PreproductCode.Equals(lotNo));
        //    _unitOfWork.TabletProductRepository.Delete(i=>i.F56_KndCmdNo.Equals(kneadingNo) && i.F56_PrePdtLotNo.Equals(lotNo));
        //{


        //public ResponseResult Create(string kneadingNo, string lotNo,string preProductCode)


        //Creating tabletising command
        //    {
        //        tbtCmdItem.F41_TblCntAmt = kndCmdItem.F42_StgCtnAmt;

        //    }
        //    tbtCmdItem.F41_RtrEndCntAmt = 0;
        //    tbtCmdItem.F41_ChgCntAmt = 0;
        //    tbtCmdItem.F41_AddDate = DateTime.Now;
        //    tbtCmdItem.F41_UpdateDate = DateTime.Now;
        //    tbtCmdItem.F41_UpdateCount = 0;
        //    _unitOfWork.TabletCommandRepository.Add(tbtCmdItem);


        //}
    }
}