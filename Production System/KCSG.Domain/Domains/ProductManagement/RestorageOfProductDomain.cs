using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.ProductManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductManagement;

namespace KCSG.Domain.Domains.ProductManagement
{
    public class RestorageOfProductDomain : BaseDomain, IRestorageOfProductDomain
    {
        #region Constructor

        private readonly INotificationService _notificationService;

        public RestorageOfProductDomain(IUnitOfWork unitOfWork, 
            INotificationService notificationService,
            IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load data in Restore Product 
        /// Refer Br62 - SRS Product management v1.1
        /// </summary>
        /// <param name="palletNo"></param>
        /// <returns></returns>
        public List<RestorageOfProductViewItem> GetListRestorageOfProduct(string palletNo)
        {
            var productShelfStocks = _unitOfWork.ProductShelfStockRepository.GetAll();
            var products = _unitOfWork.ProductRepository.GetAll();

            var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_NotStk;
            var lstResult = (from productShelfStock in productShelfStocks
                from product in products
                where (
                    productShelfStock.F40_PalletNo.Trim().Equals(palletNo.Trim()) &&
                    productShelfStock.F40_ProductCode.Trim().Equals(product.F09_ProductCode.Trim()) &&
                    productShelfStock.F40_StockFlag.Equals(stockFlag)
                    )
                select new RestorageOfProductViewItem
                {
                    F40_ProductCode = productShelfStock.F40_ProductCode,
                    ProductName = product.F09_ProductDesp,
                    F40_PrePdtLotNo = productShelfStock.F40_PrePdtLotNo,
                    F40_ProductLotNo = productShelfStock.F40_ProductLotNo,
                    PackUnit = Math.Round(product.F09_PackingUnit, 2),
                    F40_TabletingEndDate = productShelfStock.F40_TabletingEndDate,
                    F40_CertificationFlg = productShelfStock.F40_CertificationFlg,
                    F40_CertificationDate = productShelfStock.F40_CertificationDate,
                    F40_AddDate = productShelfStock.F40_AddDate,
                    F40_Amount = Math.Round(productShelfStock.F40_Amount),
                    F40_ShippedAmount = Math.Round(productShelfStock.F40_ShippedAmount, 2),
                    Remainder =
                        product.F09_PackingUnit != 0
                            ? (int) Math.Floor((productShelfStock.F40_Amount - productShelfStock.F40_ShippedAmount)/
                                               product.F09_PackingUnit)
                            : 0
                });
            return lstResult.ToList();
        }

        public bool StoreProduct(RestoreProductItem item, string terminalNo)
        {
            //•	Delete data from TX40_PDTSHFSTK, in which [F40_PalletNo] = [Pallet No] textbox value.
            _unitOfWork.ProductShelfStockRepository.Delete(i => i.F40_PalletNo.Trim().Equals(item.PalletNo.Trim()));

            //•	Delete data from TX53_OUTSIDEPREPDTSTK, in which [F53_PalletNo] = [Pallet No] textbox value.
            _unitOfWork.OutSidePrePdtStkRepository.Delete(i => i.F53_PalletNo.Trim().Equals(item.PalletNo.Trim()));
            _unitOfWork.Commit();

            //•	For each line of textboxes, system inserts new product details into TX40_PDTSHFSTK
            _unitOfWork.ProductShelfStockRepository.InsertProductShelfStock(item.PalletNo, item.PreProductLotNo1,
                item.ProductCode1, item.LotNo1, Constants.F40_StockFlag.TX40_StkFlg_Str,
                (int) Math.Ceiling(item.Remainder1), item.Fraction1, item.Total1,
                Convert.ToDateTime(item.EndDate1), string.Empty, 0, item.CertificationFlag1,
                Convert.ToDateTime(item.CertificationDate1)
                , Convert.ToDateTime(item.AddDate1));

            _unitOfWork.ProductShelfStockRepository.InsertProductShelfStock(item.PalletNo, item.PreProductLotNo2,
                item.ProductCode2, item.LotNo2, Constants.F40_StockFlag.TX40_StkFlg_Str,
                (int) Math.Ceiling(item.Remainder2), item.Fraction2, item.Total2,
                Convert.ToDateTime(item.EndDate2), string.Empty, 0, item.CertificationFlag2,
                Convert.ToDateTime(item.CertificationDate2)
                , Convert.ToDateTime(item.AddDate2));

            _unitOfWork.ProductShelfStockRepository.InsertProductShelfStock(item.PalletNo, item.PreProductLotNo3,
                item.ProductCode3, item.LotNo3, Constants.F40_StockFlag.TX40_StkFlg_Str,
                (int) Math.Ceiling(item.Remainder3), item.Fraction3, item.Total3,
                Convert.ToDateTime(item.EndDate3), string.Empty, 0, item.CertificationFlag3,
                Convert.ToDateTime(item.CertificationDate3)
                , Convert.ToDateTime(item.AddDate3));

            _unitOfWork.ProductShelfStockRepository.InsertProductShelfStock(item.PalletNo, item.PreProductLotNo4,
                item.ProductCode4, item.LotNo4, Constants.F40_StockFlag.TX40_StkFlg_Str,
                (int) Math.Ceiling(item.Remainder4), item.Fraction4, item.Total4,
                Convert.ToDateTime(item.EndDate4), string.Empty, 0, item.CertificationFlag4,
                Convert.ToDateTime(item.CertificationDate4)
                , Convert.ToDateTime(item.AddDate4));

            _unitOfWork.ProductShelfStockRepository.InsertProductShelfStock(item.PalletNo, item.PreProductLotNo5,
                item.ProductCode5, item.LotNo5, Constants.F40_StockFlag.TX40_StkFlg_Str,
                (int) Math.Ceiling(item.Remainder5), item.Fraction5, item.Total5,
                Convert.ToDateTime(item.EndDate5), string.Empty, 0, item.CertificationFlag5,
                Convert.ToDateTime(item.CertificationDate5)
                , Convert.ToDateTime(item.AddDate5));

            //Save change in db
            _unitOfWork.Commit();

            //o	System will assign space shelf
            var lstProductCode =
                new List<string>(new String[]
                {
                    item.ProductCode1, item.ProductCode2, item.ProductCode3,
                    item.ProductCode4,
                    item.ProductCode5
                });
            var lstProduct =
                _unitOfWork.ProductRepository.GetMany(
                    i => lstProductCode.Contains(i.F09_ProductCode) && i.F09_LowTmpCls.Equals("0"));
            var shelfNo = "";
            var shelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_Epy;
            var shelfType = Constants.F51_ShelfType.Normal.ToString("D");
            if (!lstProduct.Any())
            {
                var productShelfStatuses =
                    _unitOfWork.ProductShelfStatusRepository.GetMany(
                        i =>
                            i.F51_ShelfStatus.Equals(shelfStatus) && i.F51_ShelfType.Equals(shelfType) &&
                            i.F51_LowTmpShfAgnOrd.HasValue).OrderBy(i => i.F51_LowTmpShfAgnOrd);
                foreach (var productShelfStatus in productShelfStatuses)
                {
                    productShelfStatus.F51_ShelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvStr;
                    productShelfStatus.F51_StockTakingFlag = Constants.F51_StockTakingFlag.TX51_StkTkgFlg_InvNotChk;
                    productShelfStatus.F51_PalletNo = item.PalletNo;
                    productShelfStatus.F51_TerminalNo = terminalNo;
                    productShelfStatus.F51_UpdateDate = DateTime.Now;

                    _unitOfWork.ProductShelfStatusRepository.Update(productShelfStatus);

                    shelfNo = productShelfStatus.F51_ShelfRow + productShelfStatus.F51_ShelfBay +
                              productShelfStatus.F51_ShelfLevel;

                    break;
                }
            }
            else
            {
                var productShelfStatuses =
                    _unitOfWork.ProductShelfStatusRepository.GetMany(
                        i =>
                            i.F51_ShelfStatus.Equals(shelfStatus) && i.F51_ShelfType.Equals(shelfType) &&
                            i.F51_LowTmpShfAgnOrd.HasValue).OrderBy(i => i.F51_CmdShfAgnOrd);
                foreach (var productShelfStatus in productShelfStatuses)
                {
                    productShelfStatus.F51_ShelfStatus = Constants.F51_ShelfStatus.TX51_ShfSts_RsvStr;
                    productShelfStatus.F51_StockTakingFlag = Constants.F51_StockTakingFlag.TX51_StkTkgFlg_InvNotChk;
                    productShelfStatus.F51_PalletNo = item.PalletNo;
                    productShelfStatus.F51_TerminalNo = terminalNo;
                    productShelfStatus.F51_UpdateDate = DateTime.Now;

                    _unitOfWork.ProductShelfStatusRepository.Update(productShelfStatus);

                    shelfNo = productShelfStatus.F51_ShelfRow + productShelfStatus.F51_ShelfBay +
                              productShelfStatus.F51_ShelfLevel;

                    break;
                }

            }
            if (string.IsNullOrEmpty(shelfNo))
            {
                return false;
            }

            //System will make out re-storage command
            //Insert Or Update tx48_nomanage
            var isNoManage = true;
            var serialNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                Constants.GetColumnInNoManager.Pdtwhscmdno, 0, 0, 0, 0, 1).ToString("D4");

            //•	Insert data to Product Warehouse TX47_PDTWHSCMD:
            var from = GetConveyorCode(terminalNo);
            var items = _unitOfWork.ProductWarehouseCommandRepository.InsertProductWarehouseCommand(
                Constants.F47_CommandNo.Storage.ToString("D"), serialNo,
                Constants.CommandType.CmdType_0, Constants.F47_StrRtrType.ProductRestorage.ToString("D"),
                Constants.F47_Status.AnInstruction.ToString("D"), item.PalletNo, from, shelfNo, terminalNo,
                Constants.PictureNo.TCPR071F);

            _unitOfWork.Commit();

            _notificationService.SendMessageToC3("TCPR071F",
                _notificationService.FormatThirdCommunicationMessageResponse(items));
            return true;
        }

        public IList<ThirdCommunicationResponse> RespondingReplyFromC3(string terminalNo,RestoreProductItem item)
        {
            var lstStatus = new List<string>() {"6", "7", "8"};
            var lstTx47 =
                _unitOfWork.ProductWarehouseCommandRepository.GetByTerminalNoAndPictureNoAndStatus(terminalNo,
                    Constants.PictureNo.TCPR071F, lstStatus);

            var items = new List<ThirdCommunicationResponse>();

            var outsidePreProductStocks = _unitOfWork.OutSidePrePdtStkRepository.GetAll();

            foreach (var tx47 in lstTx47)
            {

                var outsidePreProductStock =
                    outsidePreProductStocks.FirstOrDefault(x => x.F53_PalletNo.Equals(tx47.F47_PalletNo));

                var newStatus = "";
                if (string.IsNullOrEmpty(tx47.F47_Status) || tx47.F47_Status.Length != 1)
                    continue;

                var map = Mapper.Map<ThirdCommunicationResponse>(tx47);
                map.OldStatus = tx47.F47_Status;
                map.ProductCode = outsidePreProductStock == null ? "" : outsidePreProductStock.F53_PalletNo;

                switch (tx47.F47_Status[0])
                {
                    case '6': //Command End

                        newStatus = "C";

                        break;
                    case '7': //Command Cancel
                        newStatus = "D";

                        break;

                    case '8': //Command Error
                        newStatus = "E";
                        var row = "";
                        var bay = "";
                        var level = "";
                        if (!AssignSpaceShelf(tx47.F47_PalletNo, Getlowtmpflag(tx47.F47_PalletNo, item), ref row, ref bay, ref level, terminalNo))
                        {
                            newStatus = "B";
                        }
                        else
                        {
                            InsertCommand(Constants.F47_CommandNo.TwoTimes.ToString("d"),terminalNo,tx47.F47_To,row,bay,level,tx47.F47_PalletNo);
                        }
                        break;
                }

               
                tx47.F47_Status = newStatus;
                tx47.F47_UpdateDate = DateTime.Now;
                map.ProductWarehouseItem = tx47;

                items.Add(map);
                //_unitOfWork.ProductWarehouseCommandRepository.Update(tx47);

            }

            _unitOfWork.Commit();

            return items;
        }

        /// <summary>
        /// De-assigning Product
        /// Refer BR 66 - SRS Product management v1.1
        /// </summary>
        /// <param name="palletNo"></param>
        public void DeAssignProduct(string palletNo)
        {
            //	Delete data from TX40_PDTSHFSTK, where [f40_palletno] = [Pallet No] textbox value.
            _unitOfWork.ProductShelfStockRepository.Delete(i => i.F40_PalletNo.Trim().Equals(palletNo));

            //	Delete data from TX57_PDTSHF, where [f57_palletno] = [Pallet No] textbox value.
            _unitOfWork.ProductShelfRepository.Delete(i => i.F57_PalletNo.Trim().Equals(palletNo));

            //Save change
            _unitOfWork.Commit();
        }

        #endregion

        private bool AssignSpaceShelf(string palletNo, string lowtmpcls, ref string row, ref string bay,
            ref string level, string terminalNo)
        {
            var lb_assignOK = false;
            var nml = Constants.TX51ShelfType.TX51_ShelfType_Nml.ToString("D");
            var lstTx51 =
                _unitOfWork.ProductShelfStatusRepository.GetMany(
                    i =>
                        i.F51_ShelfStatus == Constants.F51_ShelfStatus.TX51_ShfSts_Epy &&
                        i.F51_ShelfType == nml
                    );
            if (lowtmpcls == Constants.TX37LowTmpCls.TX37_LowTmpCls_Low.ToString("D"))
            {
                lstTx51 = lstTx51.Where(i => i.F51_LowTmpShfAgnOrd.HasValue).OrderBy(i => i.F51_LowTmpShfAgnOrd);
            }
            else
            {
                lstTx51 = lstTx51.Where(i => i.F51_CmdShfAgnOrd.HasValue).OrderBy(i => i.F51_CmdShfAgnOrd);
            }

            lb_assignOK = lstTx51.Any();
            foreach (var tx51 in lstTx51)
            {
                row = tx51.F51_ShelfRow;
                bay = tx51.F51_ShelfBay;
                level = tx51.F51_ShelfLevel;
                tx51.F51_ShelfStatus = Constants.TX51SheflStatus.TX51_ShfSts_RsvStr.ToString("d");
                tx51.F51_StockTakingFlag = Constants.TX51StockTakingFlag.TX51_StkTkgFlg_InvNotChk.ToString("D");
                tx51.F51_PalletNo = palletNo;
                tx51.F51_TerminalNo = terminalNo;
                tx51.F51_UpdateDate = DateTime.Now;
                _unitOfWork.ProductShelfStatusRepository.Update(tx51);

                break;
            }
            return lb_assignOK;
        }
        private string Getlowtmpflag (string palletNo,RestoreProductItem item)
        {

            if (!string.IsNullOrEmpty(palletNo))
            {
             return   _unitOfWork.ProductRepository.GetMany(
                    i =>
                        (i.F09_PreProductCode.Trim().Equals(item.ProductCode1) ||
                        i.F09_PreProductCode.Trim().Equals(item.ProductCode2) ||
                        i.F09_PreProductCode.Trim().Equals(item.ProductCode3) ||
                        i.F09_PreProductCode.Trim().Equals(item.ProductCode4) ||
                        i.F09_PreProductCode.Trim().Equals(item.ProductCode5))&&i.F09_LowTmpCls =="0").Any()?"0":"1";
            }
            else
            {
                var tx40 = _unitOfWork.ProductShelfStockRepository.Get(i => i.F40_PalletNo.Trim() == palletNo.Trim());
                return _unitOfWork.ProductRepository.Get(i => i.F09_ProductCode == tx40.F40_ProductCode).F09_LowTmpCls;
            }
        }

        private void InsertCommand(string as_cmdno,string terminerNo , string as_serialno, string as_row, string as_bay,
            string as_level, string as_palletno)
        {
            var conveyor = _unitOfWork.ConveyorRepository.GetbyTerminerNo(terminerNo);
            _unitOfWork.ProductWarehouseCommandRepository.InsertProductWarehouseCommand(as_cmdno, as_serialno,
                Constants.CommandType.CmdType_0, Constants.F47_StrRtrType.ProductRestorage.ToString("d"), "0", as_palletno,
                conveyor.F05_ConveyorCode, as_row + as_bay + as_level, terminerNo, Constants.PictureNo.TCPR071F);
        }
    }
}


