using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.Office.Word;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.ProductManagement
{
    public class StorageOfProductDomain : BaseDomain, IStorageOfProductDomain
    {
        private readonly INotificationService _notificationService;

        public StorageOfProductDomain(IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
        }

        public ResponseResult<GridResponse<StorageOfProductItem>> SearchCriteria(GridSettings gridSettings)
        {
            var products = _unitOfWork.ProductRepository.GetAll();

            var tabletStatusstatus = Constants.F56_Status.TabletingOver;
            var tableproducts = _unitOfWork.TabletProductRepository.GetAll();


            var results = from tableproduct in tableproducts
                from product in products
                where (
                    tableproduct.F56_ProductCode.Trim().Equals(product.F09_ProductCode.Trim()) &&
                    tableproduct.F56_TbtCmdEndAmt > tableproduct.F56_StorageAmt &&
                    tableproduct.F56_Status.Equals(tabletStatusstatus)
                    )
                select new StorageOfProductItem
                {
                    F56_KndCmdNo = tableproduct.F56_KndCmdNo,
                    F56_ProductLotNo = tableproduct.F56_ProductLotNo,
                    F56_ProductCode = tableproduct.F56_ProductCode,
                    F56_PrePdtLotNo = tableproduct.F56_PrePdtLotNo,
                    F56_TbtCmdEndAmt = tableproduct.F56_TbtCmdEndAmt,
                    F56_StorageAmt = tableproduct.F56_StorageAmt,
                    F56_CertificationFlag = tableproduct.F56_CertificationFlag,
                    ProductName = product.F09_ProductDesp,
                    PackingUnit = product.F09_PackingUnit
                };


            var itemCount = results.Count();

            if (gridSettings != null)
                OrderByAndPaging(ref results, gridSettings);

            var resultModel = new GridResponse<StorageOfProductItem>(results, itemCount);
            return new ResponseResult<GridResponse<StorageOfProductItem>>(resultModel, true);
        }

        public ResponseResult<GridResponse<OutOfPlanProductItem>> ShowData(GridSettings gridSettings)
        {
            var outPlanProducts = _unitOfWork.OutOfPlanProductRepository.GetAll();
            var products = _unitOfWork.ProductRepository.GetAll();

            var f58Status = Constants.F58_Status.StorageComplete.ToString("D");
            var result = from outPlanProduct in outPlanProducts
                from product in products
                where (
                    outPlanProduct.F58_ProductCode.Trim().Equals(product.F09_ProductCode.Trim()) &&
                    outPlanProduct.F58_TbtCmdEndAmt > outPlanProduct.F58_StorageAmt &&
                    outPlanProduct.F58_Status.Equals(f58Status)
                    )
                orderby new
                {
                    outPlanProduct.F58_ProductCode,
                    outPlanProduct.F58_PrePdtLotNo
                }
                select new OutOfPlanProductItem()
                {
                    F58_ProductCode = outPlanProduct.F58_ProductCode,
                    ProductName = product.F09_ProductDesp,
                    F58_PrePdtLotNo = outPlanProduct.F58_PrePdtLotNo,
                    F58_ProductLotNo = outPlanProduct.F58_ProductLotNo,
                    F58_TbtCmdEndAmt = outPlanProduct.F58_TbtCmdEndAmt,
                    F58_StorageAmt = outPlanProduct.F58_StorageAmt,
                    F58_CertificationFlag = outPlanProduct.F58_CertificationFlag,
                    PackingUnit = product.F09_PackingUnit,
                };


            var itemCount = result.Count();

            if (gridSettings != null)
                OrderByAndPaging(ref result, gridSettings);

            var resultModel = new GridResponse<OutOfPlanProductItem>(result, itemCount);
            return new ResponseResult<GridResponse<OutOfPlanProductItem>>(resultModel, true);
        }

        public List<SelectingItem> GetSelected(string lstValue, int status)
        {
            var results = new List<SelectingItem>();
            var s = lstValue.TrimEnd('#').Split('#');


            if (status == 0)
            {
                for (int j = 0; j < s.Length; j++)
                {
                    var commandNo = s[j].Split(',')[0];
                    var productCode = s[j].Split(',')[1];
                    var preProductLotNo = s[j].Split(',')[2];
                    var tableProduct =
                        _unitOfWork.TabletProductRepository.Get(
                            i =>
                                i.F56_KndCmdNo.Trim().Equals(commandNo.Trim()) &&
                                i.F56_ProductCode.Trim().Equals(productCode.Trim()) &&
                                i.F56_PrePdtLotNo.Trim().Equals(preProductLotNo.Trim()));

                    var product =
                        _unitOfWork.ProductRepository.Get(i => i.F09_ProductCode.Trim().Equals(productCode));
                    var selectItem = new SelectingItem();
                    selectItem.CommandNo = tableProduct.F56_KndCmdNo;
                    selectItem.ProductCode = tableProduct.F56_ProductCode;
                    selectItem.PreProductLotNo = tableProduct.F56_PrePdtLotNo;
                    selectItem.ProductName = product.F09_ProductDesp;
                    selectItem.LotNo = tableProduct.F56_ProductLotNo;
                    selectItem.PackUnit = product.F09_PackingUnit;
                    selectItem.PackQty = (tableProduct.F56_TbtCmdEndAmt - tableProduct.F56_StorageAmt)/
                                         product.F09_PackingUnit;
                    //selectItem.Fraction = (tableProduct.F56_TbtCmdEndAmt - tableProduct.F56_StorageAmt) -
                    //                      selectItem.PackQty*product.F09_PackingUnit;
                    selectItem.Fraction = tableProduct.F56_TbtCmdEndFrtAmt;
                    selectItem.TabletingEndDate = tableProduct.F56_TbtEndDate.ToString();
                    results.Add(selectItem);
                }
            }
            else
            {
                for (int j = 0; j < s.Length; j++)
                {
                    var productCode = s[j].Split(',')[0];
                    var preProductLotNo = s[j].Split(',')[1];

                    var outOfPlanProduct =
                        _unitOfWork.OutOfPlanProductRepository.Get(
                            i =>
                                i.F58_ProductCode.Trim().Equals(productCode.Trim()) &&
                                i.F58_PrePdtLotNo.Trim().Equals(preProductLotNo.Trim()));

                    var product =
                        _unitOfWork.ProductRepository.Get(i => i.F09_ProductCode.Trim().Equals(productCode));
                    var selectItem = new SelectingItem();
                    selectItem.ProductCode = outOfPlanProduct.F58_ProductCode;
                    selectItem.PreProductLotNo = outOfPlanProduct.F58_PrePdtLotNo;
                    selectItem.ProductName = product.F09_ProductDesp;
                    selectItem.LotNo = outOfPlanProduct.F58_ProductLotNo;
                    selectItem.PackUnit = product.F09_PackingUnit;
                    selectItem.PackQty = (outOfPlanProduct.F58_TbtCmdEndAmt - outOfPlanProduct.F58_StorageAmt)/
                                         product.F09_PackingUnit;
                    selectItem.Fraction = (outOfPlanProduct.F58_TbtCmdEndAmt - outOfPlanProduct.F58_StorageAmt) -
                                          selectItem.PackQty*product.F09_PackingUnit;
                    selectItem.TabletingEndDate = outOfPlanProduct.F58_TbtEndDate.ToString();
                    results.Add(selectItem);
                }
            }
            return results;
        }

        /// <summary>
        /// Checked the pallet no
        /// refer br3- srs product management sub system 1.1 
        /// </summary>
        /// <param name="palletNo"></param>
        /// <returns>MSG11</returns>
        public bool CheckedPalletNo(string palletNo)
        {
            var stockFlag1 = Constants.F40_StockFlag.TX40_StkFlg_Stk;
            var stockFlag2 = Constants.F40_StockFlag.TX40_StkFlg_Str;
            var stockFlag3 = Constants.F40_StockFlag.TX40_StkFlg_Rtr;
            var productShelfStocks =
                _unitOfWork.ProductShelfStockRepository.GetMany(
                    i =>
                        i.F40_PalletNo.Trim().Equals(palletNo) &&
                        (i.F40_StockFlag.Equals(stockFlag1) || i.F40_StockFlag.Equals(stockFlag2) ||
                         i.F40_StockFlag.Equals(stockFlag3)));

            return productShelfStocks.Any();
        }

        /// <summary>
        /// Check total amount Of tx40
        /// refer br3 - srs product management sub system 1.1 
        /// </summary>
        /// <param name="palletNo"></param>
        /// <returns>show message 12</returns>
        public double GetTotalAmountOfTX40(string palletNo)
        {
            var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_NotStk;

            var productShelfStocks =
                _unitOfWork.ProductShelfStockRepository.GetMany(
                    i => i.F40_PalletNo.Trim().Equals(palletNo) && i.F40_StockFlag.Equals(stockFlag));

            double totalAmount = 0;

            foreach (var productShelfStock in productShelfStocks)
            {
                totalAmount += productShelfStock.F40_Amount;
            }

            return totalAmount;
        }

        /// <summary>
        /// Check record number of TX53_OUTSIDEPREPDTSTK
        /// refer br3 - srs product management sub system 1.1 
        /// </summary>
        /// <param name="palletNo"></param>
        /// <returns>false-MSG11</returns>
        public bool CheckOutSidePrePdtStk(string palletNo)
        {
            var stockFlag1 = Constants.F53_StokcFlag.TX53_StkFlg_Stk;
            var stockFlag2 = Constants.F53_StokcFlag.TX53_StkFlg_Str;
            var stockFlag3 = Constants.F53_StokcFlag.TX53_StkFlg_Rtr;
            var outSidePreproductProductStock =
                _unitOfWork.OutSidePrePdtStkRepository.GetMany(
                    i =>
                        i.F53_PalletNo.Trim().Equals(palletNo) &&
                        (i.F53_StockFlag.Equals(stockFlag1) || i.F53_StockFlag.Equals(stockFlag2) ||
                         i.F53_StockFlag.Equals(stockFlag3)));
            return outSidePreproductProductStock.Any();
        }

        /// <summary>
        ///Insert,Update And Delete Data
        /// </summary>
        /// <returns></returns>
        public ResponseResult UpdaDateCreateAndDelete(StoreProductItem item, string terminalNo)
        {
            #region Delete old details on the pallet

            //Delete data from “TX40_PdtShfStk” table, where [F40_PalletNo] = Pallet No textbox value
            _unitOfWork.ProductShelfStockRepository.Delete(i => i.F40_PalletNo.Trim().Equals(item.PalletNo.Trim()));

            //Delete data from “TX53_OutSidePrePdtStk” table, where [F53_PalletNo] = Pallet No textbox value.
            _unitOfWork.OutSidePrePdtStkRepository.Delete(i => i.F53_PalletNo.Trim().Equals(item.PalletNo.Trim()));

            //Delete data from “TX57_Pdtshf” table, where [F57_PalletNo] = Pallet No textbox value.
            _unitOfWork.ProductShelfRepository.Delete(i => i.F57_PalletNo.Trim().Equals(item.PalletNo.Trim()));
            _unitOfWork.Commit();
            #endregion

            #region Insert And Update Datas

           
            //	Insert new data into “TX57_PdtShf” table
            var outFlg = item.OutOfSpec
                ? Constants.StorageOfProductStatus.OutOfPlan.ToString("D")
                : Constants.StorageOfProductStatus.Normal.ToString("D");
            if (!string.IsNullOrEmpty(item.PalletNo))
            {
                _unitOfWork.ProductShelfRepository.InsertProductShelf(item.PalletNo.Trim(), outFlg, 0);
            }


            //-	Suppose System Date is the current system date.
            DateTime systemDate = DateTime.Now;
            //If the “Normal” radio button is checked, retrieve System Date from [f56_tbtenddate] in “TX56_tbtpdt” table, where [f56_Productcode] = [Product Code] and [f56_prepdtlotno] = [Prepdt LotNo]
            //If the “Out of Plan” radio button is checked, retrieve System Date from [f58_tbtenddate] 
            if (Constants.StorageOfProductStatus.Normal.Equals(item.StorageOfProductStatus))
            {
                UpdateTableProduct(item.ProductCode1, item.PreProductLotNo1,
                    item.PackQty1, item.PackUnit1, item.OutOfSpec, item.Fraction1);
                UpdateTableProduct(item.ProductCode2, item.PreProductLotNo2,
                    item.PackQty2, item.PackUnit2, item.OutOfSpec, item.Fraction2);
                UpdateTableProduct(item.ProductCode3, item.PreProductLotNo3,
                    item.PackQty3, item.PackUnit3, item.OutOfSpec, item.Fraction3);
                UpdateTableProduct(item.ProductCode4, item.PreProductLotNo4,
                    item.PackQty4, item.PackUnit4, item.OutOfSpec, item.Fraction4);
                UpdateTableProduct(item.ProductCode5, item.PreProductLotNo5,
                    item.PackQty5, item.PackUnit1, item.OutOfSpec, item.Fraction5);
            }
            else
            {
                UpdateOutOfPlanProduct(item.ProductCode1, item.PreProductLotNo1,
                    item.PackQty1, item.PackUnit1, item.OutOfSpec, item.Fraction1);
                UpdateOutOfPlanProduct(item.ProductCode2, item.PreProductLotNo2,
                    item.PackQty2, item.PackUnit2, item.OutOfSpec, item.Fraction2);
                UpdateOutOfPlanProduct(item.ProductCode3, item.PreProductLotNo3,
                    item.PackQty3, item.PackUnit3, item.OutOfSpec, item.Fraction3);
                UpdateOutOfPlanProduct(item.ProductCode4, item.PreProductLotNo4,
                    item.PackQty4, item.PackUnit4, item.OutOfSpec, item.Fraction4);
                UpdateOutOfPlanProduct(item.ProductCode5, item.PreProductLotNo5,
                    item.PackQty5, item.PackUnit5, item.OutOfSpec, item.Fraction5);
            }

            //Insert new data into “TX40_PdtShfStk”:
            var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_Str;
            #region Get System Date

            #endregion

            var certificationflg = item.OutOfSpec
                ? Constants.F40_CertificationFlag.TX40_CrtfctnFlg_OutofSpec.ToString("D")
                : Constants.F40_CertificationFlag.TX40_CrtfctnFlg_Normal.ToString("D");

            _unitOfWork.ProductShelfStockRepository.InsertProductShelfStock(item.PalletNo,
                item.PreProductLotNo1, item.ProductCode1,
                item.LotNo1, stockFlag, item.PackQty1, item.Fraction1, item.PackQty1*item.PackUnit1 + item.Fraction1,
                item.TabletingEndDate1,
                certificationflg);
            _unitOfWork.ProductShelfStockRepository.InsertProductShelfStock(item.PalletNo,
                item.PreProductLotNo2, item.ProductCode2,
                item.LotNo2, stockFlag, item.PackQty2, item.Fraction2, item.PackQty2*item.PackUnit2 + item.Fraction2,
                item.TabletingEndDate2,
                certificationflg);
            _unitOfWork.ProductShelfStockRepository.InsertProductShelfStock(item.PalletNo,
                item.PreProductLotNo3, item.ProductCode3,
                item.LotNo3, stockFlag, item.PackQty3, item.Fraction3, item.PackQty3*item.PackUnit3 + item.Fraction3,
                item.TabletingEndDate3,
                certificationflg);
            _unitOfWork.ProductShelfStockRepository.InsertProductShelfStock(item.PalletNo,
                item.PreProductLotNo4, item.ProductCode4,
                item.LotNo4, stockFlag, item.PackQty4, item.Fraction4, item.PackQty4*item.PackUnit4 + item.Fraction4,
                item.TabletingEndDate4,
                certificationflg);
            _unitOfWork.ProductShelfStockRepository.InsertProductShelfStock(item.PalletNo,
                item.PreProductLotNo5, item.ProductCode5,
                item.LotNo5, stockFlag, item.PackQty5, item.Fraction5, item.PackQty5*item.PackUnit5 + item.Fraction5,
                item.TabletingEndDate5,
                certificationflg);
            //Update data in TX51_PdtShfSts table
            var lstProductCode =
                new List<string>(new String[]
                {item.ProductCode1, item.ProductCode2, item.ProductCode3, item.ProductCode4, item.ProductCode5});
            var lowTemp = Constants.TX37LowTmpCls.TX37_LowTmpCls_Low.ToString("D");
            var normal = Constants.TX37LowTmpCls.TX37_LowTmpCls_Nml.ToString("D");
            var emptyShelf = Constants.F51_ShelfStatus.TX51_ShfSts_Epy;
            var normalShelf = Constants.F51_ShelfType.Normal.ToString("D");
            var isChecked = false;
            
           // var serialNo = 0;

            var f47From = GetConveyorCode(terminalNo);
            var f47To = "";
            for (int j = 0; j < lstProductCode.Count; j++)
            {
                if (!string.IsNullOrEmpty(lstProductCode[j]))
                {
                    var productCode = lstProductCode[j];
                    var product =
                        _unitOfWork.ProductRepository.Get(
                            i =>
                                i.F09_ProductCode.Trim().Equals(productCode.Trim()) &&
                                (i.F09_LowTmpCls.Equals(lowTemp) || i.F09_LowTmpCls.Equals(normal)));

                    if (product != null)
                    {
                        var productShelfStatuses =
                            _unitOfWork.ProductShelfStatusRepository.GetMany(
                                i =>
                                    i.F51_ShelfStatus.Equals(emptyShelf) && i.F51_ShelfType.Equals(normalShelf));


                        if (product.F09_LowTmpCls == lowTemp)
                        {
                            productShelfStatuses =
                                productShelfStatuses.Where(
                                    i =>
                                        i.F51_LowTmpShfAgnOrd != null).OrderBy(i => i.F51_LowTmpShfAgnOrd);
                        }
                        else
                        {
                            productShelfStatuses =
                                productShelfStatuses.Where(
                                    i =>
                                        i.F51_CmdShfAgnOrd != null).OrderBy(i => i.F51_CmdShfAgnOrd);
                        }
                        foreach (var productShelfStatus in productShelfStatuses)
                        {
                            productShelfStatus.F51_ShelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvStr;
                            productShelfStatus.F51_StockTakingFlag =
                                Constants.F51_StockTakingFlag.TX51_StkTkgFlg_InvNotChk;
                            productShelfStatus.F51_PalletNo = item.PalletNo;
                            productShelfStatus.F51_TerminalNo = terminalNo;
                            productShelfStatus.F51_UpdateDate = DateTime.Now;
                            _unitOfWork.ProductShelfStatusRepository.Update(productShelfStatus);

                            isChecked = true;

                            f47To = productShelfStatus.F51_ShelfRow + productShelfStatus.F51_ShelfBay +
                                    productShelfStatus.F51_ShelfLevel;
                            break;
                        }
                    }
                }
            }
            //Insert or update tx48
                var isNoManage = true;
                var serialNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                    Constants.GetColumnInNoManager.Pdtwhscmdno, 0, 0, 0, 0, 1);

            var F47CmdSeqNo = serialNo.ToString("D4");

            //Insert Tx47
            _unitOfWork.ProductWarehouseCommandRepository.InsertProductWarehouseCommand(
                Constants.F47_CommandNo.Storage.ToString("D"), F47CmdSeqNo,
                Constants.CommandType.CmdType_0, Constants.F47_StrRtrType.Product.ToString("D"),
                Constants.F47_Status.AnInstruction.ToString("D"), item.PalletNo, f47From, f47To,
                terminalNo,
                Constants.PictureNo.TCPR011F);

            var message = "0001" + terminalNo.PadLeft(4) + Constants.PictureNo.TCPR011F.PadLeft(8) +
                          "0066" +
                          F47CmdSeqNo + "1000" +
                          Constants.F50_CommandType.CmdType_0.ToString("D").PadLeft(4) +
                          Constants.F47_Status.AnInstruction.ToString("D").PadLeft(4) + f47From + f47To +
                          item.PalletNo.PadLeft(4);
            _notificationService.SendMessageToC3("TCPR011F", message);

            #endregion

            _unitOfWork.Commit();
            if (!isChecked)
            {
                return new ResponseResult(true, "There is no empty location available in the warehouse now!");
            }
            return new ResponseResult(true);
        }

        public void UpdateTableProduct(string productCode, string preProductLotNo, double packQty,
            double packUnit, bool outOfSpace,
            double fraction)
        {
            if (!string.IsNullOrEmpty(productCode) &&
                !string.IsNullOrEmpty(preProductLotNo))
            {
                var tabletProducts =
                    _unitOfWork.TabletProductRepository.GetMany(
                        i =>
                            i.F56_ProductCode.Trim().Equals(productCode.Trim()) &&
                            i.F56_PrePdtLotNo.Trim().Equals(preProductLotNo.Trim()));

                var certificationflg = outOfSpace
                    ? Constants.F40_CertificationFlag.TX40_CrtfctnFlg_OutofSpec.ToString("D")
                    : Constants.F40_CertificationFlag.TX40_CrtfctnFlg_Normal.ToString("D");

                //If “Normal” radio button is checked, then update data in “TX56_TbtPdt” table, where [f56_productcode] = Product Code and [f56_prepdtlotno] = Prepdt LotNo of selected line
                foreach (var tabletProduct in tabletProducts)
                {
                    var storageamt = tabletProduct.F56_StorageAmt + packQty*packUnit + fraction;
                    _unitOfWork.TabletProductRepository.UpdateTabletProduct(tabletProduct, certificationflg, storageamt);
                }
                _unitOfWork.Commit();
            }
        }

        public void UpdateOutOfPlanProduct(string productCode, string preProductLotNo, double packQty, double packUnit,
            bool outOfSpace,
            double fraction)
        {
            if (!string.IsNullOrEmpty(productCode) &&
                !string.IsNullOrEmpty(preProductLotNo))
            {
                var outOfPlanProduct =
                    _unitOfWork.OutOfPlanProductRepository.GetMany(
                        i =>
                            i.F58_ProductCode.Trim().Equals(productCode.Trim()) &&
                            i.F58_PrePdtLotNo.Trim().Equals(preProductLotNo.Trim())).FirstOrDefault();
                var certificationflg = !outOfSpace
                    ? Constants.F40_CertificationFlag.TX40_CrtfctnFlg_OutofSpec.ToString("D")
                    : Constants.F40_CertificationFlag.TX40_CrtfctnFlg_Normal.ToString("D");

                //If “Out of Plan” radio button is unchecked, update “tx58_OutPlanpdt”
                outOfPlanProduct.F58_StorageAmt += packQty*packUnit + fraction;
                outOfPlanProduct.F58_CertificationFlag = certificationflg;
                outOfPlanProduct.F58_UpdateDate = DateTime.Now;

                _unitOfWork.OutOfPlanProductRepository.Update(outOfPlanProduct);
            }
        }

        /// <summary>
        /// Responding Reply From C3 Rules:
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="item"></param>
        public List<ThirdCommunicationResponse> ProcessDataReceiveMessageForC3(string terminalNo, StoreProductItem item)
        {
            var items = new List<ThirdCommunicationResponse>();

            var lstTx47 =
                _unitOfWork.ProductWarehouseCommandRepository.GetMany(
                    i => i.F47_TerminalNo.Trim().Equals(terminalNo.Trim()) && i.F47_PictureNo.Trim().Equals(Constants.PictureNo.TCPR011F)
                         && (i.F47_Status == "6" || i.F47_Status == "7" || i.F47_Status == "8"))
                    .OrderBy(i => i.F47_AddDate);
            //OPEN c1 ;
            foreach (var tx47 in lstTx47)
            {
                var tx47Item = Mapper.Map<ThirdCommunicationResponse>(tx47);
                tx47Item.OldStatus = tx47.F47_Status;
                tx47Item.ProductCode = "";
                if (!string.IsNullOrEmpty(item.ProductCode1))
                {
                    tx47Item.ProductCode = tx47Item.ProductCode + item.ProductCode1;
                }
                if (!string.IsNullOrEmpty(item.ProductCode2))
                {
                    if (string.IsNullOrEmpty(tx47Item.ProductCode))
                    {

                        tx47Item.ProductCode = tx47Item.ProductCode + item.ProductCode2;
                    }
                    else
                    {
                        tx47Item.ProductCode = tx47Item.ProductCode + ", " + item.ProductCode2;
                    }
                }
                if (!string.IsNullOrEmpty(item.ProductCode3))
                {
                    if (string.IsNullOrEmpty(tx47Item.ProductCode))
                    {

                        tx47Item.ProductCode = tx47Item.ProductCode + item.ProductCode3;
                    }
                    else
                    {
                        tx47Item.ProductCode = tx47Item.ProductCode + ", " + item.ProductCode3;
                    }
                }
                if (!string.IsNullOrEmpty(item.ProductCode4))
                {
                    if (string.IsNullOrEmpty(tx47Item.ProductCode))
                    {

                        tx47Item.ProductCode = tx47Item.ProductCode + item.ProductCode4;
                    }
                    else
                    {
                        tx47Item.ProductCode = tx47Item.ProductCode + ", " + item.ProductCode4;
                    }
                }
                if (!string.IsNullOrEmpty(item.ProductCode5))
                {
                    if (string.IsNullOrEmpty(tx47Item.ProductCode))
                    {

                        tx47Item.ProductCode = tx47Item.ProductCode + item.ProductCode5;
                    }
                    else
                    {
                        tx47Item.ProductCode = tx47Item.ProductCode + ", " + item.ProductCode5;
                    }
                }


                var newStatus = "";
                var ls_serialno = "";
                var row = "";
                var bay = "";
                var level = "";
                switch (tx47.F47_Status[0])
                {
                    case '6':
                        newStatus = "C"; 
                        TabletStorageEnd(tx47.F47_PalletNo);
                        break;
                    case '7':
                        newStatus = "D";
                        CancelCommand(tx47.F47_PalletNo);
                        break;
                    case '8':
                        newStatus = "E";
                        if (!AssignSpaceShelf(tx47.F47_PalletNo, Getlowtmpflag(tx47.F47_PalletNo, item), ref row,
                                ref bay, ref level, terminalNo))
                        {
                            newStatus = "B";
                            CancelCommand(tx47.F47_PalletNo);
                        }
                        else
                        {
                            if (insertcommand(Constants.F47_CommandNo.TwoTimes.ToString("d"), ref ls_serialno,
                                item.PalletNo, row, bay, level, terminalNo))
                            {
                                //               f_tcsendmsgtoc3(TX47_CmdNo_TwoTimes, ls_serialno)
                            }
                        }
                        break;
                }
                tx47.F47_Status = newStatus;
                tx47.F47_UpdateDate = DateTime.Now;
                _unitOfWork.ProductWarehouseCommandRepository.Update(tx47);
                items.Add(tx47Item);
            }

            _unitOfWork.Commit();

            return items;
        }

        public bool insertcommand(string as_cmdno, ref string as_serialno, string as_palletno, string as_row,
            string as_bay, string as_level, string terminalNo)
        {
            string ls_shelfno = as_row + as_bay + as_level;
            var nomanager = true;
            as_serialno = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref nomanager,
                Constants.GetColumnInNoManager.PrePdtWhsCmdNo, 0, 0, 0, 0, 1).ToString("D4");


            var tx47 = new TX47_PdtWhsCmd()
            {
                F47_CommandNo = as_cmdno,
                F47_CmdSeqNo = as_serialno,
                F47_CommandType = Constants.CmdType.cmdType,
                F47_Status = Constants.F47_StrRtrType.Product.ToString("D"),
                F47_Priority = 0,
                F47_PalletNo = as_palletno,
                F47_StrRtrType =  "0",
                F47_From = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminalNo).F05_ConveyorCode,
                F47_To = ls_shelfno,
                F47_CommandSendDate = null,
                F47_TerminalNo = terminalNo,
                F47_PictureNo = Constants.PictureNo.TCPR011F,
                F47_AbnormalCode = null,
                F47_RetryCount = 0,
                F47_AddDate = DateTime.Now,
                F47_UpdateDate = DateTime.Now,
            };
            _unitOfWork.ProductWarehouseCommandRepository.Add(tx47);
            return true;
        }

        #region private method

        public bool AssignSpaceShelf(string as_palletno, string ach_lowtmpcls, ref string as_row,
            ref string as_bay, ref string as_level, string terminalNo)
        {
            try
            {
                var TX51_ShfSts_Epy = Constants.TX51SheflStatus.TX51_ShfSts_Epy.ToString("D");
                var TX51_ShelfType_Nml = Constants.F51_ShelfType.Normal.ToString("D");
                var lstTx51 =
                    _unitOfWork.ProductShelfStatusRepository.GetMany(
                        i => i.F51_ShelfStatus == TX51_ShfSts_Epy && i.F51_ShelfType == TX51_ShelfType_Nml);

                lstTx51 = ach_lowtmpcls == Constants.Temperature.Low.ToString("d")
                    ? lstTx51.Where(i => i.F51_LowTmpShfAgnOrd.HasValue).OrderBy(i => i.F51_LowTmpShfAgnOrd)
                    : lstTx51.Where(i => i.F51_CmdShfAgnOrd.HasValue).OrderBy(i => i.F51_CmdShfAgnOrd);
                if (!lstTx51.Any())
                {
                    return false;
                }
                foreach (var tx51 in lstTx51)
                {
                    as_row = tx51.F51_ShelfRow;
                    as_level = tx51.F51_ShelfLevel;
                    as_bay = tx51.F51_ShelfBay;
                    tx51.F51_ShelfStatus = Constants.TX51SheflStatus.TX51_ShfSts_RsvStr.ToString("D");
                    tx51.F51_StockTakingFlag = Constants.TX51StockTakingFlag.TX51_StkTkgFlg_InvNotChk.ToString("D");
                    tx51.F51_PalletNo = as_palletno;
                    tx51.F51_TerminalNo = terminalNo;
                    tx51.F51_UpdateDate = DateTime.Now;
                    _unitOfWork.ProductShelfStatusRepository.Update(tx51);
                    break;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string Getlowtmpflag(string as_palletno, StoreProductItem item)
        {
            if (string.IsNullOrEmpty(as_palletno))
            {
                for (int i = 0; i < 5; i++)
                {
                    var productCode = "";
                    switch (i)
                    {
                        case 1:
                            productCode = item.ProductCode1;
                            break;
                        case 2:
                            productCode = item.ProductCode2;
                            break;
                        case 3:
                            productCode = item.ProductCode3;
                            break;
                        case 4:
                            productCode = item.ProductCode4;
                            break;
                        case 5:
                            productCode = item.ProductCode5;
                            break;
                    }
                    var tm09 =
                        _unitOfWork.ProductRepository.GetMany(o => o.F09_ProductCode == productCode).FirstOrDefault();
                    if (tm09.F09_LowTmpCls == Constants.Temperature.Low.ToString("D"))
                    {
                        return Constants.Temperature.Low.ToString("D");
                    }
                }
                return Constants.Temperature.Normal.ToString("D");
            }
            var lstPreproductCode =
                _unitOfWork.ProductShelfStockRepository.GetMany(
                    i => i.F40_PalletNo.Trim().Equals(as_palletno.Trim())).Select(i => i.F40_ProductCode.Trim());
            var lstTm09 =
                _unitOfWork.ProductRepository.GetMany(i => lstPreproductCode.Contains(i.F09_ProductCode.Trim()));
            foreach (var tm09 in lstTm09)
            {
                if (tm09.F09_LowTmpCls == Constants.Temperature.Low.ToString("D"))
                {
                    return Constants.Temperature.Low.ToString("D");
                }
            }
            return Constants.Temperature.Normal.ToString("D");
        }

        public void CancelCommand(string as_palletno)
        {
            String ls_pcode, ls_pplot;
            Double lr_amount;
            var tx57 =
                _unitOfWork.ProductShelfRepository.GetMany(i => i.F57_PalletNo.Trim().Equals(as_palletno.Trim()))
                    .FirstOrDefault();
            if (tx57 != null)
            {
                var lstTx40 =
                    _unitOfWork.ProductShelfStockRepository.GetMany(
                        i => i.F40_PalletNo.Trim().Equals(as_palletno.Trim()));
                foreach (var tx40 in lstTx40)
                {
                    if (tx57.F57_OutFlg == Constants.F57_OutFlg.Plan.ToString("D"))
                    {
                        var lstTx56 =
                            _unitOfWork.TabletProductRepository.GetMany(
                                i =>
                                    i.F56_ProductCode.Equals(tx40.F40_ProductCode) &&
                                    i.F56_PrePdtLotNo.Equals(tx40.F40_PrePdtLotNo));
                        foreach (var tx56 in lstTx56)
                        {
                            tx56.F56_StorageAmt -= tx40.F40_Amount;
                            _unitOfWork.TabletProductRepository.Update(tx56);
                        }
                    }
                    else
                    {
                        var lstTx58 =
                            _unitOfWork.OutOfPlanProductRepository.GetMany(
                                i =>
                                    i.F58_ProductCode.Equals(tx40.F40_ProductCode) &&
                                    i.F58_PrePdtLotNo.Equals(tx40.F40_PrePdtLotNo));
                        foreach (var tx58 in lstTx58)
                        {
                            tx58.F58_StorageAmt -= tx40.F40_Amount;
                            _unitOfWork.OutOfPlanProductRepository.Update(tx58);
                        }
                    }
                }
            }
        }

        private void TabletStorageEnd(string as_palletno)
        {
             var tx57 =  _unitOfWork.ProductShelfRepository.Get(i => i.F57_PalletNo.Trim().Equals(as_palletno.Trim()));
            var lsttx40 = _unitOfWork.ProductShelfStockRepository.GetByPalletNo(as_palletno);
            foreach (var tx40 in lsttx40)
            {
                if (tx57.F57_OutFlg == Constants.F57_OutFlg.Plan.ToString("D"))
                {
                    var lstTx56 =
                        _unitOfWork.TabletProductRepository.GetMany(
                            i =>
                                i.F56_ProductCode == tx40.F40_ProductCode && i.F56_PrePdtLotNo == tx40.F40_PrePdtLotNo);
                    var lstUpdateTx56 = lstTx56.ToList().Where(i=>
                               (i.F56_TbtCmdEndAmt-i.F56_StorageAmt) >-0.01 &&  (i.F56_TbtCmdEndAmt-i.F56_StorageAmt) < 0.01);
                    foreach (var tx56 in lstUpdateTx56)
                    {
                        tx56.F56_Status = Constants.F56_Status.StorageOver;
                        _unitOfWork.TabletProductRepository.Update(tx56);
                    }
                    var lstKndNo = lstTx56.Select(i => i.F56_KndCmdNo);
                    var counttx56 =
                        _unitOfWork.TabletProductRepository.GetMany(
                            i => lstKndNo.Contains(i.F56_KndCmdNo) && i.F56_Status != Constants.F56_Status.StorageOver).Count();
                    if (counttx56 == 0)
                    {
                        var lstTx41 =
                            _unitOfWork.TabletCommandRepository.GetMany(
                                i => lstKndNo.Contains(i.F41_KndCmdNo) && i.F41_PrePdtLotNo == tx40.F40_PrePdtLotNo);
                        foreach (var tx41 in lstTx41)
                        {
                            tx41.F41_Status = Constants.TX41_Status.Stored.ToString("D");
                            _unitOfWork.TabletCommandRepository.Update(tx41);
                        }
                    }
                }
                else
                {
                    var lstTx58 =
                        _unitOfWork.OutOfPlanProductRepository.GetMany(
                            i => i.F58_ProductCode == tx40.F40_ProductCode && i.F58_PrePdtLotNo == tx40.F40_PrePdtLotNo).ToList()
                            .Where(
                                i => 
                                    (i.F58_TbtCmdEndAmt - i.F58_StorageAmt) > -0.01 &&
                                    (i.F58_TbtCmdEndAmt - i.F58_StorageAmt) < 0.01);
                    foreach (var tx58 in lstTx58)
                    {
                        tx58.F58_Status = "4";
                        _unitOfWork.OutOfPlanProductRepository.Update(tx58);
                    }
                }
            }
            
        }
        #endregion
    }
}