using System;
using System.Collections.Generic;
using System.Linq;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.ProductManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains
{
    public class CommonSearchDomain : BaseDomain, ICommonSearchDomain
    {
        #region Constructor

        public CommonSearchDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Supplier Code Select

        public ResponseResult<GridResponse<CommonSearchItem>> GetSupplierCodes(string code, GridSettings gridSettings)
        {
            var result = _unitOfWork.SupplierRepossitories.GetAll();
            if (!string.IsNullOrEmpty(code))
                result =
                    result.Where(
                        i =>
                            i.F04_SupplierCode.ToUpper().Contains(code.ToUpper()) ||
                            i.F04_SupplierName.ToUpper().Contains(code.ToUpper()));
            // Sort and paging
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var resultLst = result.ToList().Select(s => new CommonSearchItem
            {
                F04_SupplierCode = s.F04_SupplierCode,
                F04_SupplierName = s.F04_SupplierName,
                F04_MaxLoadAmount = s.F04_MaxLoadAmount
            });

            //var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());
            var resultModel = new GridResponse<CommonSearchItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
            //return result;
        }

        #endregion

        #region Product Code Select

        public ResponseResult<GridResponse<CommonSearchItem>> GetProductCode(string code, GridSettings gridSettings)
        {
            var result = _unitOfWork.ProductRepository.GetAll();
            if (!string.IsNullOrEmpty(code))
                result =
                    result.Where(
                        i =>
                            i.F09_ProductCode.ToUpper().Contains(code.ToUpper()) ||
                            i.F09_ProductDesp.ToUpper().Contains(code.ToUpper()));
            // Sort and paging
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var resultLst = result.ToList().Select(s => new CommonSearchItem
            {
                F09_ProductCode = s.F09_ProductCode,
                F09_ProductDesp = s.F09_ProductDesp
            });

            //var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());
            var resultModel = new GridResponse<CommonSearchItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
            //return result;
        }

        /// <summary>
        /// Find products list which is for 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        public ResponseResult<GridResponse<CommonSearchItem>> FindProductLabelList(string code, GridSettings gridSettings)
        {
            var products = _unitOfWork.ProductRepository.GetAll();
            var tbtproducts = _unitOfWork.TabletProductRepository.GetAll();
            var result = (from product in products
                          from tbtproduct in tbtproducts
                          where tbtproduct.F56_Status.Equals("3")
                          where tbtproduct.F56_ProductCode.Equals(product.F09_ProductCode)
                          select new CommonSearchItem
                          {
                              F56_ProductCode = tbtproduct.F56_ProductCode,
                              F56_KndCmdNo = tbtproduct.F56_KndCmdNo,
                              F09_TabletSize = product.F09_TabletSize,
                              F09_TabletSize2 = product.F09_TabletSize2,
                              F09_TabletType = product.F09_TabletType,
                              F09_Label = product.F09_Label,
                              F09_ProductDesp = product.F09_ProductDesp,
                              F09_ValidPeriod = product.F09_ValidPeriod
                          }).Distinct();

            //var products = _unitOfWork.ProductRepository.GetAll();
            //if (!string.IsNullOrEmpty(code))
            //{
            //    var upperCasedCode = code.ToUpper();
            //    products =
            //        products.Where(
            //            i =>
            //                i.F09_ProductCode.ToUpper().Contains(upperCasedCode) ||
            //                i.F09_ProductDesp.ToUpper().Contains(upperCasedCode) ||
            //                i.F09_Label.ToUpper().Contains(upperCasedCode));
            //}

            //// Find all kneading command in database.
            //var kneadingCommands = _unitOfWork.KneadingCommandRepository.GetAll();

            //// Find list of items which can be selected in modal dialog.
            //var result = (from product in products
            //              from kneadingCommand in kneadingCommands
            //              where product.F09_PreProductCode.Trim().Equals(kneadingCommand.F42_PreProductCode.Trim())
            //              select new CommonSearchItem
            //              {
            //                  F09_ProductCode = product.F09_ProductCode, 
            //                  F42_KneadingCommand = kneadingCommand.F42_KndCmdNo
            //                  //F09_TabletSize = product.F09_TabletSize,
            //                  //F09_TabletSize2 = product.F09_TabletSize2,
            //                  //F09_TabletType = product.F09_TabletType,
            //                  //F09_Label = product.F09_Label,
            //                  //F09_ProductDesp = product.F09_ProductDesp,
            //                  //F09_ValidPeriod = product.F09_ValidPeriod
            //              }).Distinct();

            // Sort and paging
            var itemCount = result.Count();

            // Do pagination.
            OrderByAndPaging(ref result, gridSettings);

            //var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());


            var resultModel = new GridResponse<CommonSearchItem>(result, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
            //return result;
        }
        #endregion

        #region Material Code Select

        public ResponseResult<GridResponse<CommonSearchItem>> GetMaterialCode(string code, GridSettings gridSettings)
        {
            var result = _unitOfWork.MaterialRepository.GetAll();
            if (!string.IsNullOrEmpty(code))
                result =
                    result.Where(
                        i =>
                            i.F01_MaterialCode.ToUpper().Contains(code.ToUpper()) ||
                            i.F01_MaterialDsp.ToUpper().Contains(code.ToUpper()));
            // Sort and paging
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var resultLst = result.ToList().Select(s => new CommonSearchItem
            {
                F01_MaterialCode = s.F01_MaterialCode,
                F01_MaterialDsp = s.F01_MaterialDsp,
                F01_Unit = s.F01_Unit,
                F01_RtrPosCls = s.F01_RtrPosCls,
                F01_EntrustedClass = s.F01_EntrustedClass,
                F01_PackingUnit = s.F01_PackingUnit,

                F01_LiquidClass = EnumsHelper.GetDescription<Constants.Liquid>(ConvertHelper.ToInteger(s.F01_LiquidClass))
            });

            //var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());
            var resultModel = new GridResponse<CommonSearchItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
            //return result;
        }

        #endregion

        #region Preproduct Code Select

        public ResponseResult<GridResponse<CommonSearchItem>> GetPreproductCode(string code, GridSettings gridSettings)
        {
            var result = _unitOfWork.PreProductRepository.GetAll();
            if (!string.IsNullOrEmpty(code))
                result =
                    result.Where(
                        i =>
                            i.F03_PreProductCode.ToUpper().Contains(code.ToUpper()) ||
                            i.F03_PreProductName.ToUpper().Contains(code.ToUpper()));
            // Sort and paging
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var resultLst = result.ToList().Select(s => new CommonSearchItem
            {
                F03_PreproductCode = s.F03_PreProductCode,
                F03_PreproductName = s.F03_PreProductName
            });

            //var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());
            var resultModel = new GridResponse<CommonSearchItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
            //return result;
        }

        #endregion

        #region Supplimentary Material Code Select

        public ResponseResult<GridResponse<CommonSearchItem>> GetSupMatCode(string code, GridSettings gridSettings)
        {
            var result = _unitOfWork.SubMaterialRepository.GetAll();
            if (!string.IsNullOrEmpty(code))
                result =
                    result.Where(
                        i =>
                            i.F15_SubMaterialCode.ToUpper().Contains(code.ToUpper()) ||
                            i.F15_MaterialDsp.ToUpper().Contains(code.ToUpper()));
            // Sort and paging
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var resultLst = result.ToList().Select(s => new CommonSearchItem
            {
                F15_submaterialcode = s.F15_SubMaterialCode,
                F15_materialdsp = s.F15_MaterialDsp,
                F01_Unit = s.F15_Unit
            });

            //var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());
            var resultModel = new GridResponse<CommonSearchItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
            //return result;
        }

        #endregion

        #region End User Code Select

        public ResponseResult<GridResponse<CommonSearchItem>> GetEndUserCode(string code, GridSettings gridSettings)
        {
            var result = _unitOfWork.EndUserRepository.GetAll();
            if (!string.IsNullOrEmpty(code))
                result =
                    result.Where(
                        i =>
                            i.F10_EndUserCode.ToUpper().Contains(code.ToUpper()) ||
                            i.F10_EndUserName.ToUpper().Contains(code.ToUpper()));

            // Sort and paging
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var resultLst = result.ToList().Select(s => new CommonSearchItem
            {
                F10_EndUserCode = s.F10_EndUserCode,
                F10_EndUserName = s.F10_EndUserName
            });

            var resultModel = new GridResponse<CommonSearchItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
            //return result;
        }

        #endregion

        #region Shipping No Select

        public ResponseResult<GridResponse<CommonSearchItem>> GetShippingNo(string code, GridSettings gridSettings)
        {
            var result = _unitOfWork.ShippingPlanRepository.GetAll();
            if (!string.IsNullOrEmpty(code))
                result = result.Where(i => i.F44_ShipCommandNo.ToUpper().Contains(code.ToUpper()));
            // Sort and paging
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var resultLst = result.ToList().Select(s => new CommonSearchItem
            {
                ShippingNo = s.F44_ShipCommandNo,
                ProductCode = s.F44_ProductCode,
                EndUserName =
                    _unitOfWork.EndUserRepository.Get(e => e.F10_EndUserCode.Equals(s.F44_EndUserCode)).F10_EndUserName
            });

            //var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());

            var resultModel = new GridResponse<CommonSearchItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
            //return result;
        }

        #endregion

        #region Product Lot No Select

        public ResponseResult<GridResponse<CommonSearchItem>> GetProductLotNo(string code, GridSettings gridSettings)
        {
            var result = _unitOfWork.ProductShelfStockRepository.GetAll();
            if (!string.IsNullOrEmpty(code))
                result = result.Where(i => i.F40_ProductLotNo.ToUpper().Contains(code.ToUpper()));
            // Sort and paging
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var resultLst = result.ToList().Select(s => new CommonSearchItem
            {
                F40_ProductLotNo = s.F40_ProductLotNo
            });
           
            //var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());

            var resultModel = new GridResponse<CommonSearchItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
            //return result;
        }

        #endregion
        #region Product Lot No Select with productCode
        public ResponseResult<GridResponse<CommonSearchItem>> GetProductLotNoWithProductCode(string code, GridSettings gridSettings, string productCode = null)
        {
            var result = _unitOfWork.ProductShelfStockRepository.GetAll().Where(i=>i.F40_StockFlag.Trim()=="3");
            if (!string.IsNullOrEmpty(productCode))
                result = result.Where(i => i.F40_ProductCode.Trim().Equals(productCode.Trim()));
            if (!string.IsNullOrEmpty(code))
                result = result.Where(i => i.F40_ProductLotNo.ToUpper().Contains(code.ToUpper()));
            // Sort and paging
           
            OrderByAndPaging(ref result, gridSettings);
            var resultLst = result.ToList().Select(i=>i.F40_ProductLotNo).Distinct().Select(s => new CommonSearchItem
            {
                F40_ProductLotNo = s
            });
            var itemCount = resultLst.Count();
            //var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());

            var resultModel = new GridResponse<CommonSearchItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
            //return result;
        }
        #endregion

        #region P.O.NO Select

        public ResponseResult<GridResponse<CommonSearchItem>> GetPONo(string materialCode, string code, GridSettings gridSettings)
        {
            var receptions = _unitOfWork.ReceptionRepository.GetAll().Where(i=> i.F30_StoragedAmount < i.F30_ExpectAmount);
            var materials = _unitOfWork.MaterialRepository.GetAll();
            if (!string.IsNullOrEmpty(materialCode))
                receptions =
                    receptions.Where(
                        i =>
                            i.F30_MaterialCode.Trim().Equals(materialCode.Trim()));
            if (!string.IsNullOrEmpty(code))
                receptions =
                    receptions.Where(
                        i =>
                            i.F30_PrcOrdNo.ToUpper().Contains(code.ToUpper()) ||
                            i.F30_PrtDvrNo.ToUpper().Contains(code.ToUpper()));

            var result = from reception in receptions
                         join material in materials on reception.F30_MaterialCode equals material.F01_MaterialCode
                         //where
                         //(
                         //    reception.F30_MaterialCode = material.F01_MaterialCode
                         //)
                         select new
                         {
                             material.F01_MaterialCode,
                             material.F01_MaterialDsp,
                             material.F01_PackingUnit,
                             reception.F30_PrcOrdNo,
                             reception.F30_PrtDvrNo
                         };
            // Sort and paging
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var resultLst = result.ToList().Select(s => new CommonSearchItem
            {
                F30_PrcOrdNo = s.F30_PrcOrdNo,
                F30_PrtDvrNo = s.F30_PrtDvrNo,
                F33_MaterialCode = s.F01_MaterialCode,
                MaterialName = s.F01_MaterialDsp,
                F01_PackingUnit = s.F01_PackingUnit
            });

            //var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());

            var resultModel = new GridResponse<CommonSearchItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
            //return result;
        }

        #endregion

        #region Container Type Select

        public ResponseResult<GridResponse<CommonSearchItem>> GetContainerType(string code, GridSettings gridSettings)
        {
            var result = _unitOfWork.ContainerRepository.GetAll();
            if (!string.IsNullOrEmpty(code))
                result =
                    result.Where(
                        i =>
                            i.F08_ContainerType.ToUpper().Contains(code.ToUpper()) ||
                            i.F08_ContainerName.ToUpper().Contains(code.ToUpper()));

            // Sort and paging
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var resultLst = result.ToList().Select(s => new CommonSearchItem
            {
                F08_ContainerType = s.F08_ContainerType,
                F08_ContainerName = s.F08_ContainerName
            });
            //var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());

            var resultModel = new GridResponse<CommonSearchItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
            //return result;
        }

        #endregion

        #region Material Pallet No Select

        public ResponseResult<GridResponse<CommonSearchItem>> GetMaterialPalletNo(string code, GridSettings gridSettings)
        {
            var result = _unitOfWork.MaterialShelfRepository.GetAll();
            if (!string.IsNullOrEmpty(code))
                result = result.Where(i => i.F32_PalletNo.ToUpper().Contains(code.ToUpper()));
            // Sort and paging
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var resultLst = result.ToList().Select(s => new CommonSearchItem
            {
                F32_PalletNo = s.F32_PalletNo
            });

            //var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());

            var resultModel = new GridResponse<CommonSearchItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
            //return result;
        }

        #endregion

        #region Tableting Line Select

        public ResponseResult<GridResponse<CommonSearchItem>> GetTabletingLine(string code, GridSettings gridSettings)
        {
            var result = _unitOfWork.DeviceRepository.GetAll().Where(i => i.F14_DeviceCode.ToLower().StartsWith("tab"));
            if (!string.IsNullOrEmpty(code))
                result = result.Where(i => i.F14_DeviceCode.ToUpper().Contains(code.ToUpper()));
            // Sort and paging
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var resultLst = result.ToList().Select(s => new CommonSearchItem
            {
                F14_DeviceCode = s.F14_DeviceCode,
                F14_DeviceName = s.F14_DeviceName
            });

            //var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());

            var resultModel = new GridResponse<CommonSearchItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
            //return result;
        }

        #endregion

        #region Shelf No Select

        public ResponseResult<GridResponse<CommonSearchItem>> GetShelfNo(string code, GridSettings gridSettings)
        {
            gridSettings.SortField = "F37_ShelfStatus";
            var result = _unitOfWork.PreProductShelfStatusRepository.GetAll();
            if (!string.IsNullOrEmpty(code))
                result =
                    result.Where(
                        i =>
                            i.F37_ShelfStatus.Contains(code) ||
                            i.F37_ShelfBay.Contains(code) ||
                            i.F37_ShelfLevel.Contains(code));

            //result.Where(
            //    i => i.F37_ShelfStatus.Contains(string.Format("{0}", Constants.F37_ShelfStatus.EmptyShelf))
            //         && i.F37_LowTmpCls.Equals(string.Format("{0}", Constants.F37_lowtmpcls.One)));
            // Sort and paging
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var resultLst = result.ToList().Select(s => new CommonSearchItem
            {
                ShelfNo = s.F37_ShelfRow + '-' + s.F37_ShelfBay + '-' + s.F37_ShelfLevel
            });

            //var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());

            var resultModel = new GridResponse<CommonSearchItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
            //return result;
        }

        #endregion

        #region Product Pallet No Select

        public ResponseResult<GridResponse<CommonSearchItem>> GetProductPalletNo(string code, GridSettings gridSettings)
        {
            var stockFlag = Constants.F40_StockFlag.TX40_StkFlg_NotStk;
            var result = _unitOfWork.ProductShelfStockRepository.GetMany(i => i.F40_StockFlag.Equals(stockFlag));

            if (!string.IsNullOrEmpty(code))
                result =
                    result.Where(
                        i => i.F40_PalletNo.Trim().Equals(code));

            // Sort and paging
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            var resultLst = result.ToList().Select(s => new CommonSearchItem
            {
                F40_PalletNo = s.F40_PalletNo
            });

            //var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());

            var resultModel = new GridResponse<CommonSearchItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
            //return result;
        }
        #endregion

        #region Out of Plan Product

        public ResponseResult<GridResponse<CommonSearchItem>> GetOutOfPlanProduct(string code, GridSettings gridSettings)
        {
            var outPlanProducts = _unitOfWork.OutOfPlanProductRepository.GetAll();
            var products = _unitOfWork.ProductRepository.GetAll();
            var result = from outPlanProduct in outPlanProducts
                         join product in products on outPlanProduct.F58_ProductCode.Trim() equals product.F09_ProductCode.Trim()
                         where outPlanProduct.F58_StorageAmt.Equals(0)
                         orderby new
                         {
                             outPlanProduct.F58_PrePdtLotNo,
                             outPlanProduct.F58_ProductCode
                         }
                         select new
                         {
                             product.F09_ProductCode,
                             product.F09_ProductDesp,
                             outPlanProduct.F58_PrePdtLotNo,
                             outPlanProduct.F58_ProductLotNo,
                             outPlanProduct.F58_TbtCmdEndPackAmt, //Pack Qty
                             outPlanProduct.F58_TbtCmdEndFrtAmt, //Fraction
                             outPlanProduct.F58_TbtEndDate //Tabletising 
                         };

            if (!string.IsNullOrEmpty(code))
                result = result.Where(x => x.F09_ProductCode.Trim().Equals(code));
            // Sort and paging
            var itemCount = result.Count();
            if (gridSettings != null)
                OrderByAndPaging(ref result, gridSettings);
            var resultLst = result.ToList().Select(s => new CommonSearchItem
            {
                F09_ProductCode = s.F09_ProductCode,
                F09_ProductDesp = s.F09_ProductDesp,
                F58_PrePdtLotNo = s.F58_PrePdtLotNo,
                F58_ProductLotNo = s.F58_ProductLotNo,
                F58_TbtCmdEndPackAmt = s.F58_TbtCmdEndPackAmt,
                F58_TbtCmdEndFrtAmt = s.F58_TbtCmdEndFrtAmt,
                F58_TbtEndDate = s.F58_TbtEndDate
            });
            //var lstResult = Mapper.Map<IEnumerable<TM01_Material>, IEnumerable<MaterialItem>>(result.ToList());
            var resultModel = new GridResponse<CommonSearchItem>(resultLst, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
            //return result;
        }
        #endregion



        public ResponseResult<GridResponse<CommonSearchItem>> GetPalletNoWithStockFlag(string code, GridSettings gridSettings)
        {
            var stockFlag = Constants.F33_StockFlag.TX33_StkFlg_NotStk;
            var materialShelfStocks = _unitOfWork.MaterialShelfStockRepository.GetMany(i => i.F33_StockFlag.Equals(stockFlag));
            var materials = _unitOfWork.MaterialRepository.GetAll();
            //var productShelfStatuses = _unitOfWork.ProductShelfStatusRepository.GetAll();

            if (!string.IsNullOrEmpty(code))
                materialShelfStocks =
                    materialShelfStocks.Where(
                        i => i.F33_PalletNo.Trim().Contains(code.Trim()));

            var result = from materialShelfStock in materialShelfStocks
                         from material in materials
                             //from productShelfStatus in productShelfStatuses
                         where (
                    materialShelfStock.F33_MaterialCode.Equals(material.F01_MaterialCode.Trim())
                    //&& materialShelfStock.F33_PalletNo.Equals(productShelfStatus.F51_PalletNo)
                    )
                         select new CommonSearchItem
                         {
                             //ShelfNo = productShelfStatus.F51_ShelfRow + "-" + productShelfStatus.F51_ShelfBay + "-" + productShelfStatus.F51_ShelfLevel,
                             F33_PalletNo = materialShelfStock.F33_PalletNo,
                             F33_MaterialCode = materialShelfStock.F33_MaterialCode,
                             MaterialName = material.F01_MaterialDsp,
                             F01_PackingUnit = material.F01_PackingUnit,
                             F01_Unit = material.F01_Unit
                         };

            // Sort and paging
            var itemCount = result.Count();

            //OrderByAndPaging(ref result, gridSettings);
            // Find real page index.
            var realPageIndex = gridSettings.PageIndex - 1;
            if (realPageIndex < 0)
                realPageIndex = 0;

            // Pagination.
            result = result
                .OrderBy(x => x.MaterialName)
                .Skip(realPageIndex * gridSettings.PageSize)
                .Take(gridSettings.PageSize);

            var resultModel = new GridResponse<CommonSearchItem>(result, itemCount);
            return new ResponseResult<GridResponse<CommonSearchItem>>(resultModel, true);
        }
    }
}