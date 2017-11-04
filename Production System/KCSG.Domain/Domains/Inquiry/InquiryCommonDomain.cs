using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Drawing.Charts;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.Inquiry;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.Inquiry.ByWareHouseLocation;

namespace KCSG.Domain.Domains.Inquiry
{
    public class InquiryCommonDomain : BaseDomain,IInquiryCommonDomain
    {
        #region Constructor

        public InquiryCommonDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion
        public string GetLabelByStatus(Constants.InquirySearchConditionWarehouseLocation type, string status)
        {
            if (type == Constants.InquirySearchConditionWarehouseLocation.Material)
            {
                switch (status)
                {
                    case "0": //Empty Shelf
                        return "Ep";
                    case "1": //warehouse pallet
                        return "Wp";
                    case "2": //Supplier Pallet
                        return "Sp";
                    case "3": //Material
                        return "Mt";
                    case "4": //Reserved for Storage
                        return "Rs";
                    case "5": //Reserved for Retrieval
                        return "Rr";
                    case "6": //Prohibited
                        return "Fb";
                    case "7": //Physically  Prohibited
                        return "Pf";
                    default:
                        return "";
                }

            }

            if (type == Constants.InquirySearchConditionWarehouseLocation.PreProduct)
            {
                switch (status)
                {
                    case "0": //Empty Shelf
                        return "Ep";
                    case "1": //Empty Container
                        return "Ec";
                    case "3": //Stock
                        return "St";
                    case "4": //Reserved for Storage
                        return "Rs";
                    case "5": //Reserved for Retrieval
                        return "Rr";
                    case "6": //Prohibited
                        return "Fb";
                    case "7": //Physically  Prohibited
                        return "Pf";
                    default:
                        return "";
                }

            }

            if (type == Constants.InquirySearchConditionWarehouseLocation.Product)
            {
                switch (status)
                {
                    case "0": //Empty Shelf
                        return "Ep";
                    case "1": //Empty Pallet
                        return "Wp";
                    case "2": //Product Stock
                        return "St";
                    case "3": //Out of Sign Pre-Product Stock
                        return "Os";
                    case "4": //Reserved for Storage
                        return "Rs";
                    case "5": //Reserved for Retrieval
                        return "Rr";
                    case "6": //Prohibited
                        return "Fb";
                    case "7": //Physically  Prohibited
                        return "Pf";
                    case "8": //External Pre-product
                        return "Ex";
                    default:
                        return "";
                }
            }
            return "";
        }

        public string GetShelfStatus(Constants.InquirySearchConditionWarehouseLocation type,string row, string bay, string level)
        {
            var status = "";
            if (type == Constants.InquirySearchConditionWarehouseLocation.Material)
            {
                var shelfStatus = _unitOfWork.MaterialShelfStatusRepository.Get(
                    x =>
                        x.F31_ShelfRow.Trim().Equals(row) && x.F31_ShelfBay.Trim().Equals(bay) &&
                        x.F31_ShelfLevel.Trim().Equals(level));
                if (shelfStatus != null) status = shelfStatus.F31_ShelfStatus;
            }

            if (type == Constants.InquirySearchConditionWarehouseLocation.PreProduct)
            {
                var shelfStatus = _unitOfWork.PreProductShelfStatusRepository.Get(
                    x =>
                        x.F37_ShelfRow.Trim().Equals(row) && x.F37_ShelfBay.Trim().Equals(bay) &&
                        x.F37_ShelfLevel.Trim().Equals(level));
                if (shelfStatus != null) status = shelfStatus.F37_ShelfStatus;
            }

            if (type == Constants.InquirySearchConditionWarehouseLocation.Product)
            {
                var shelfStatus = _unitOfWork.ProductShelfStatusRepository.Get(
                    x =>
                        x.F51_ShelfRow.Trim().Equals(row) && x.F51_ShelfBay.Trim().Equals(bay) &&
                        x.F51_ShelfLevel.Trim().Equals(level));
                if (shelfStatus != null) status = shelfStatus.F51_ShelfStatus;
            }

            return status;
        }

        public void Clearshelf(Constants.InquirySearchConditionWarehouseLocation type, string row, string bay, string level,bool commit)
        {
            try
            {
                if (type == Constants.InquirySearchConditionWarehouseLocation.Material)
                {
                    var shelfStatus = _unitOfWork.MaterialShelfStatusRepository.Get(
                        x =>
                            x.F31_ShelfRow.Trim().Equals(row) && x.F31_ShelfBay.Trim().Equals(bay) &&
                            x.F31_ShelfLevel.Trim().Equals(level));
                    if (shelfStatus != null)
                    {
                        if (shelfStatus.F31_PalletNo != null) {
                            var palletNo = shelfStatus.F31_PalletNo.Trim();
                            if (shelfStatus.F31_ShelfStatus.Trim() != ((int)Constants.TX31SheflStatus.TX31_MtrShfSts_PhyPbt).ToString())
                            {
                                if (shelfStatus.F31_ShelfStatus.Trim() != ((int)Constants.TX31SheflStatus.TX31_MtrShfSts_Epy).ToString() &&
                                    shelfStatus.F31_ShelfStatus.Trim() != ((int)Constants.TX31SheflStatus.TX31_MtrShfSts_WhsPlt).ToString() &&
                                    shelfStatus.F31_ShelfStatus.Trim() != ((int)Constants.TX31SheflStatus.TX31_MtrShfSts_SplPlt).ToString())
                                {
                                    _unitOfWork.MaterialShelfStockRepository.Delete(x => x.F33_PalletNo.Trim() == palletNo);
                                }
                            }
                            _unitOfWork.MaterialShelfRepository.Delete(x => x.F32_PalletNo.Trim() == palletNo);
                        }
                        shelfStatus.F31_ShelfStatus = ((int)Constants.TX31SheflStatus.TX31_MtrShfSts_Epy).ToString();
                        shelfStatus.F31_StockTakingFlag = ((int)Constants.T31StockTakingFlag.TX31_StkTkgFlg_InvNotChk).ToString();
                        shelfStatus.F31_TerminalNo = null;
                        shelfStatus.F31_PalletNo = null;
                        shelfStatus.F31_SupplierCode = null;
                        shelfStatus.F31_LoadAmount = 0;
                        shelfStatus.F31_Amount = 0;
                        shelfStatus.F31_StorageDate = null;
                        shelfStatus.F31_RetrievalDate = null;
                        _unitOfWork.MaterialShelfStatusRepository.Update(shelfStatus);
                        //_unitOfWork.Commit();
                    }
                }

                if (type == Constants.InquirySearchConditionWarehouseLocation.PreProduct)
                {
                    var shelfStatus = _unitOfWork.PreProductShelfStatusRepository.Get(
                        x =>
                            x.F37_ShelfRow.Trim().Equals(row) && x.F37_ShelfBay.Trim().Equals(bay) &&
                            x.F37_ShelfLevel.Trim().Equals(level));
                    if (shelfStatus != null)
                    {
                        if (shelfStatus.F37_ShelfStatus.Trim() != ((int)Constants.TX37SheflStatus.TX37_ShfSts_PhyPbt).ToString())
                        {
                            if (shelfStatus.F37_ContainerCode != null)
                            {
                                if (shelfStatus.F37_ShelfStatus.Trim() != ((int)Constants.TX37SheflStatus.TX37_ShfSts_Epy).ToString() &&
                                shelfStatus.F37_ShelfStatus.Trim() != ((int)Constants.TX37SheflStatus.TX37_ShfSts_EpyCtn).ToString())
                                {
                                    _unitOfWork.PreProductShelfStockRepository.Delete(x => x.F49_ContainerCode.Trim() == shelfStatus.F37_ContainerCode.Trim());
                                }
                                _unitOfWork.MaterialShelfRepository.Delete(x => x.F32_PalletNo.Trim() == shelfStatus.F37_ContainerCode.Trim());
                            }
                            
                            shelfStatus.F37_ShelfStatus = ((int)Constants.TX37SheflStatus.TX37_ShfSts_Epy).ToString();
                            shelfStatus.F37_ContainerCode = null;
                            shelfStatus.F37_ContainerNo = null;
                            shelfStatus.F37_StockTakingFlag = ((int)Constants.TX37StockTakingFlag.TX37_StkTkgFlg_InvNotChk).ToString();
                            shelfStatus.F37_TerminalNo = null;
                            shelfStatus.F37_StorageDate = null;
                            shelfStatus.F37_RetrievalDate = null;

                            _unitOfWork.PreProductShelfStatusRepository.Update(shelfStatus);
                            
                            //_unitOfWork.Commit();
                        }


                    }
                }

                if (type == Constants.InquirySearchConditionWarehouseLocation.Product)
                {
                    var shelfStatus = _unitOfWork.ProductShelfStatusRepository.Get(
                        x =>
                            x.F51_ShelfRow.Trim().Equals(row) && x.F51_ShelfBay.Trim().Equals(bay) &&
                            x.F51_ShelfLevel.Trim().Equals(level));
                    if (shelfStatus != null)
                    {
                        
                        if (shelfStatus.F51_ShelfStatus.Trim() != ((int)Constants.TX51SheflStatus.TX51_ShfSts_PhyPbt).ToString())
                        {
                            if (shelfStatus.F51_PalletNo != null) {
                                var palletNo = shelfStatus.F51_PalletNo.Trim();
                                if (shelfStatus.F51_ShelfStatus.Trim() != ((int)Constants.TX51SheflStatus.TX51_ShfSts_Epy).ToString() &&
                                    shelfStatus.F51_ShelfStatus.Trim() != ((int)Constants.TX51SheflStatus.TX51_ShfSts_WhsPlt).ToString())
                                {
                                    _unitOfWork.ProductShelfStockRepository.Delete(x => x.F40_PalletNo.Trim() == shelfStatus.F51_PalletNo.Trim());
                                    _unitOfWork.OutSidePrePdtStkRepository.Delete(x => x.F53_PalletNo.Trim() == shelfStatus.F51_PalletNo.Trim());
                                    _unitOfWork.ProductShelfRepository.Delete(x => x.F57_PalletNo.Trim() == palletNo);
                                }
                                
                            }
                            shelfStatus.F51_ShelfStatus = ((int)Constants.TX51SheflStatus.TX51_ShfSts_Epy).ToString();
                            shelfStatus.F51_PalletNo = null;
                            shelfStatus.F51_TerminalNo = null;
                            shelfStatus.F51_StorageDate = null;
                            shelfStatus.F51_RetrievalDate = null;
                            shelfStatus.F51_LoadAmount = 0;
                            shelfStatus.F51_StockTakingFlag = ((int)Constants.TX51StockTakingFlag.TX51_StkTkgFlg_InvChk).ToString();
                            _unitOfWork.ProductShelfStatusRepository.Update(shelfStatus);
                            //_unitOfWork.Commit();
                        }


                    }
                }
                if (commit)
                {
                    _unitOfWork.Commit();
                }
            }
            catch (Exception) { }
            

        }

        public void Setshelf(Constants.InquirySearchConditionWarehouseLocation type, string row, string bay, string level, string status, bool commit)
        {
            if (type == Constants.InquirySearchConditionWarehouseLocation.Material)
            {
                var shelfStatus = _unitOfWork.MaterialShelfStatusRepository.Get(
                    x =>
                        x.F31_ShelfRow.Trim().Equals(row) && x.F31_ShelfBay.Trim().Equals(bay) &&
                        x.F31_ShelfLevel.Trim().Equals(level));
                if (shelfStatus != null)
                {
                    shelfStatus.F31_ShelfStatus = status;
                    _unitOfWork.MaterialShelfStatusRepository.Update(shelfStatus);
                    //_unitOfWork.Commit();
                }
            }

            if (type == Constants.InquirySearchConditionWarehouseLocation.PreProduct)
            {
                var shelfStatus = _unitOfWork.PreProductShelfStatusRepository.Get(
                    x =>
                        x.F37_ShelfRow.Trim().Equals(row) && x.F37_ShelfBay.Trim().Equals(bay) &&
                        x.F37_ShelfLevel.Trim().Equals(level));
                if (shelfStatus != null)
                {
                    shelfStatus.F37_ShelfStatus = status;
                    _unitOfWork.PreProductShelfStatusRepository.Update(shelfStatus);
                    //_unitOfWork.Commit();


                }
            }

            if (type == Constants.InquirySearchConditionWarehouseLocation.Product)
            {
                var shelfStatus = _unitOfWork.ProductShelfStatusRepository.Get(
                    x =>
                        x.F51_ShelfRow.Trim().Equals(row) && x.F51_ShelfBay.Trim().Equals(bay) &&
                        x.F51_ShelfLevel.Trim().Equals(level));
                if (shelfStatus != null)
                {
                    shelfStatus.F51_ShelfStatus = status;
                    _unitOfWork.ProductShelfStatusRepository.Update(shelfStatus);
                    //_unitOfWork.Commit();
                }
            }
            if (commit)
            {
                _unitOfWork.Commit();
            }
        }

        public string GetShelftypeTx51(string row, string bay, string level)
        {
            var shelfStatus = _unitOfWork.ProductShelfStatusRepository.Get(
                    x =>
                        x.F51_ShelfRow.Trim().Equals(row) && x.F51_ShelfLevel.Trim().Equals(level) && x.F51_ShelfBay.Trim().Equals(bay));
            if (shelfStatus != null)
            {
                return shelfStatus.F51_ShelfType.Trim();
            }
            return string.Empty;
        }
        
        public List<TX51_PdtShfSts> GetAllPdtShfStsByShelfType(string shelftype)
        {
            return _unitOfWork.ProductShelfStatusRepository.GetAll().Where(x => x.F51_ShelfType.Trim().Equals(shelftype)).ToList();
        }
        #region f032f

        /// <summary>
        /// get Data for Sub screen TCFC032F
        /// </summary>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <returns></returns>

        public RawMaterialShelftStatusItem GetRawMaterialShelftStatusItem(string row, string bay, string level)
        {
            var tx31s = _unitOfWork.MaterialShelfStatusRepository.GetAll();
            var tx32s = _unitOfWork.MaterialShelfRepository.GetAll();
            var tx33s = _unitOfWork.MaterialShelfStockRepository.GetAll();
            var tm01s = _unitOfWork.MaterialRepository.GetAll();
            var result = (from tx31 in tx31s
                          join tx32 in tx32s on tx31.F31_PalletNo.Trim() equals tx32.F32_PalletNo.Trim()
                          join tx33 in tx33s on tx32.F32_PalletNo.Trim() equals tx33.F33_PalletNo.Trim()
                          join tm01 in tm01s on tx33.F33_MaterialCode.Trim() equals tm01.F01_MaterialCode.Trim()
                          where tx31.F31_ShelfRow.Trim().Equals(row) && tx31.F31_ShelfBay.Trim().Equals(bay) && tx31.F31_ShelfLevel.Trim().Equals(level)
                          select new RawMaterialShelftStatusItem
                          {
                              ShelfStatus = tx31.F31_ShelfStatus,
                              BailmentClassification = tm01.F01_EntrustedClass,
                              MaterialCode = tm01.F01_MaterialCode.Trim(),
                              MaterialName = tm01.F01_MaterialDsp.Trim(),
                              PalletNo = tx31.F31_PalletNo.Trim(),
                              PrcordNo = tx32.F32_PrcOrdNo.Trim(),
                              PrtdvrNo = tx32.F32_PrtDvrNo.Trim(),
                              StorageDate = tx32.F32_StorageDate.HasValue ? tx32.F32_StorageDate.Value : DateTime.Now,
                          }).Distinct().FirstOrDefault();
            if (result != null)
            {
                if (result.BailmentClassification == Constants.Bailment.Normal.ToString("D"))
                {
                    result.BailmentClassification = "Norm";
                } else if (result.BailmentClassification == Constants.Bailment.Bailment.ToString("D"))
                {
                    result.BailmentClassification = "Bail";
                }
                else
                {
                    result.BailmentClassification = "";
                }
                if (!string.IsNullOrEmpty(result.PrcordNo) && !string.IsNullOrEmpty(result.PrtdvrNo))
                {
                    result.AcceptanceClassification = GetAcceptanceClassification(result.PrcordNo, result.PrtdvrNo);
                }
                
            }
            return result;
        }

        /// <summary>
        /// get AcceptanceClassification for Sub screen TCFC032F
        /// </summary>
        /// <param name="prcordNo"></param>
        /// <param name="prtdvrNo"></param>
        /// <returns></returns>
        public string GetAcceptanceClassification(string prcordNo, string prtdvrNo)
        {
            try
            {
                var result = _unitOfWork.ReceptionRepository.Get(
                x => x.F30_PrcOrdNo.Trim().Equals(prcordNo) && x.F30_PrtDvrNo.Trim().Equals(prtdvrNo));
                if (result != null)
                {
                    if (result.F30_AcceptClass.Trim().Equals("1")) return "Done";
                    if (result.F30_AcceptClass.Trim().Equals("0")) return "Yet";
                    if (result.F30_AcceptClass.Trim().Equals("2")) return "Reject";
                }
            }
            catch (Exception) { }
            return string.Empty;
        }

        public void UpdateSubscreentcf032f(RawMaterialShelftStatusItem model, string termino)
        {
            Clearshelf(Constants.InquirySearchConditionWarehouseLocation.Material, model.Row, model.Bay, model.Level,false);
            // insert record into tx_32
            var tx32 = new TX32_MtrShf
            {
                F32_PalletNo = model.PalletNo,
                F32_PrcOrdNo = !string.IsNullOrEmpty(model.PrcordNo) ? model.PrcordNo : null,
                F32_PrtDvrNo = !string.IsNullOrEmpty(model.PrtdvrNo) ? model.PrtdvrNo : null,
                F32_MegaMsrMacSndEndFlg = ((int)Constants.TX32_MSndEndFlg.NotSend).ToString(),
                F32_GnrlMsrMacSndEndFlg = ((int)Constants.TX32_CSndEndFlg.NotSend).ToString(),
                F32_StorageDate = model.StorageDate,
                F32_ReStorageDate = null,
                F32_RetrievalDate = null,
                F32_AddDate = DateTime.Now,
                F32_UpdateDate = DateTime.Now,
                F32_UpdateCount = null
            };
            _unitOfWork.MaterialShelfRepository.Add(tx32);

            //Update tx31
            var tx31 =
                _unitOfWork.MaterialShelfStatusRepository.Get(
                    x =>
                        x.F31_ShelfRow.Trim().Equals(model.Row) && x.F31_ShelfBay.Trim().Equals(model.Bay) &&
                        x.F31_ShelfLevel.Trim().Equals(model.Level));
            if (tx31 != null)
            {
                tx31.F31_ShelfStatus = ((int)Constants.TX31SheflStatus.TX31_MtrShfSts_Mtr).ToString();
                tx31.F31_PalletNo = model.PalletNo;
                tx31.F31_Amount = model.Quantity1 + model.Quantity2 + model.Quantity3 + model.Quantity4 +
                                  model.Quantity5;
                tx31.F31_TerminalNo = termino;
                tx31.F31_UpdateDate = DateTime.Now;
                _unitOfWork.MaterialShelfStatusRepository.Update(tx31);
            }

            // insert into tx33
            if (!string.IsNullOrEmpty(model.MaterialLotNo1))
            {
                var tx33_1 = new TX33_MtrShfStk
                {
                    F33_PalletNo = model.PalletNo,
                    F33_MaterialCode = model.MaterialCode,
                    F33_MaterialLotNo = model.MaterialLotNo1,
                    F33_Amount = model.Quantity1,
                    F33_StockFlag = ((int)Constants.TX33_MtrShfStk.Stocked).ToString(),
                    F33_AddDate = DateTime.Now,
                    F33_UpdateDate = DateTime.Now
                };
                _unitOfWork.MaterialShelfStockRepository.Add(tx33_1);
            }

            if (!string.IsNullOrEmpty(model.MaterialLotNo2))
            {
                var tx33_2 = new TX33_MtrShfStk
                {
                    F33_PalletNo = model.PalletNo,
                    F33_MaterialCode = model.MaterialCode,
                    F33_MaterialLotNo = model.MaterialLotNo2,
                    F33_Amount = model.Quantity2,
                    F33_StockFlag = ((int)Constants.TX33_MtrShfStk.Stocked).ToString(),
                    F33_AddDate = DateTime.Now,
                    F33_UpdateDate = DateTime.Now
                };
                _unitOfWork.MaterialShelfStockRepository.Add(tx33_2);
            }

            if (!string.IsNullOrEmpty(model.MaterialLotNo3))
            {
                var tx33_3 = new TX33_MtrShfStk
                {
                    F33_PalletNo = model.PalletNo,
                    F33_MaterialCode = model.MaterialCode,
                    F33_MaterialLotNo = model.MaterialLotNo3,
                    F33_Amount = model.Quantity3,
                    F33_StockFlag = ((int)Constants.TX33_MtrShfStk.Stocked).ToString(),
                    F33_AddDate = DateTime.Now,
                    F33_UpdateDate = DateTime.Now
                };
                _unitOfWork.MaterialShelfStockRepository.Add(tx33_3);
            }

            if (!string.IsNullOrEmpty(model.MaterialLotNo4))
            {
                var tx33_4 = new TX33_MtrShfStk
                {
                    F33_PalletNo = model.PalletNo,
                    F33_MaterialCode = model.MaterialCode,
                    F33_MaterialLotNo = model.MaterialLotNo4,
                    F33_Amount = model.Quantity4,
                    F33_StockFlag = ((int)Constants.TX33_MtrShfStk.Stocked).ToString(),
                    F33_AddDate = DateTime.Now,
                    F33_UpdateDate = DateTime.Now
                };
                _unitOfWork.MaterialShelfStockRepository.Add(tx33_4);
            }

            if (!string.IsNullOrEmpty(model.MaterialLotNo5))
            {
                var tx33_5 = new TX33_MtrShfStk
                {
                    F33_PalletNo = model.PalletNo,
                    F33_MaterialCode = model.MaterialCode,
                    F33_MaterialLotNo = model.MaterialLotNo5,
                    F33_Amount = model.Quantity5,
                    F33_StockFlag = ((int)Constants.TX33_MtrShfStk.Stocked).ToString(),
                    F33_AddDate = DateTime.Now,
                    F33_UpdateDate = DateTime.Now
                };
                _unitOfWork.MaterialShelfStockRepository.Add(tx33_5);
            }
            //Commit
            _unitOfWork.Commit();
        }

        public Tuple<bool,string> CanUpdate(string palletNoNew,string palletNoCurrent, string materialCode, string prcordNo, string prtdvrNo)
        {
            if (palletNoNew != palletNoCurrent && !string.IsNullOrEmpty(palletNoCurrent))
            {
                var tx32 = _unitOfWork.MaterialShelfRepository.Get(x => x.F32_PalletNo.Trim().Equals(palletNoNew));
                if (tx32 != null)
                {
                    return new Tuple<bool, string>(false, "This pallet is being used !");
                }
            }
            
            if (!string.IsNullOrWhiteSpace(prcordNo))
            {
                var tx30 = _unitOfWork.ReceptionRepository.Get(
                x => x.F30_PrcOrdNo.Trim().Equals(prcordNo) && x.F30_PrtDvrNo.Trim().Equals(prtdvrNo));
                if (tx30 != null)
                {
                    if (!tx30.F30_MaterialCode.Trim().Equals(materialCode.Trim()))
                    {
                        return new Tuple<bool, string>(false, "There is no data for this P.O.No..and Partial Delivery for this material in database !");
                    }
                }else return new Tuple<bool, string>(false, "There is no data for this P.O.No..and Partial Delivery for this material in database !");
            }
            return new Tuple<bool, string>(true, string.Empty);
        }

        public Tuple<bool, string> PalletNo032fExit(string palletno)
        {
            var tx32 = _unitOfWork.MaterialShelfRepository.Get(x => x.F32_PalletNo.Trim().Equals(palletno));
            if (tx32 != null)
            {
                return new Tuple<bool, string>(false, "This pallet is being used !");
            }
            return new Tuple<bool, string>(true, string.Empty);
        }
        public Tuple<bool, string> MaterialCode032fExit(string prcordNo,string prtdvrNo, string material)
        {
            var tx30 = _unitOfWork.ReceptionRepository.Get(
                x => x.F30_PrcOrdNo.Trim().Equals(prcordNo) && x.F30_PrtDvrNo.Trim().Equals(prtdvrNo));
            if (tx30 == null || !tx30.F30_MaterialCode.Trim().Equals(material.Trim()))
            {
                return new Tuple<bool, string>(false, "There is no data for this P.O.No..and Partial Delivery for this material in database !");
            }
            return new Tuple<bool, string>(true, string.Empty);
        }
        #endregion
        #region f034f
        public PreProductShelfStatusItem GetPreProductShelftStatusItem(string row, string bay, string level)
        {
            var tm08s = _unitOfWork.ContainerRepository.GetAll();
            var tm03s = _unitOfWork.PreProductRepository.GetAll();
            var tx37s = _unitOfWork.PreProductShelfStatusRepository.GetAll();
            var tx49s = _unitOfWork.PreProductShelfStockRepository.GetAll();
            var result = (from tx37 in tx37s
                          join tm08 in tm08s on  tx37.F37_ContainerType.Trim() equals tm08.F08_ContainerType.Trim()
                          join tx49 in tx49s on tx37.F37_ContainerCode.Trim() equals tx49.F49_ContainerCode.Trim()
                          join tm03 in tm03s on tx49.F49_PreProductCode.Trim() equals tm03.F03_PreProductCode.Trim()
                          where tx37.F37_ShelfRow.Trim().Equals(row) && tx37.F37_ShelfBay.Trim().Equals(bay) && tx37.F37_ShelfLevel.Trim().Equals(level)
                          select new PreProductShelfStatusItem
                          {
                              ShelfStatus = tx37.F37_ShelfStatus,
                              ContainerCode = tx37.F37_ContainerCode.Trim(),
                              KneadingCommandNo = tx49.F49_KndCmdNo.Trim(),
                              ContainerNo = tx49.F49_ContainerNo.Trim(),
                              ContainerSeqNo = tx49.F49_ContainerSeqNo,
                              PreProductCode = tx49.F49_PreProductCode.Trim(),
                              PreProductName = tm03.F03_PreProductName.Trim(),
                              ContainerType = tm08.F08_ContainerType.Trim(),
                              ContainerName = tm08.F08_ContainerName.Trim(),
                              Amount = tx49.F49_Amount,
                              PreProductLotNo = tx49.F49_PreProductLotNo.Trim(),
                              StorageDate = tx49.F49_StorageDate.HasValue ? tx49.F49_StorageDate.Value : DateTime.Now,
                          }).Distinct().FirstOrDefault();
            return result;
        }
        public Tuple<bool, string> CanUpdate034f(int seqNo)
        {
            var tx49 = _unitOfWork.PreProductShelfStockRepository.Get(x => x.F49_ContainerSeqNo.Equals(seqNo));
            if (tx49 != null)
            {
                return new Tuple<bool, string>(false, "This Container Seq No is already existed !");
            }
            return new Tuple<bool, string>(true, string.Empty);
        }
        public void UpdateSubscreentcf034f(PreProductShelfStatusItem model)
        {
            Clearshelf(Constants.InquirySearchConditionWarehouseLocation.PreProduct, model.Row, model.Bay, model.Level,false);
            // insert record into tx_49
            var tx49 = new TX49_PrePdtShfStk()
            {
                F49_ContainerCode =  model.ContainerCode,
                F49_KndCmdNo = model.KneadingCommandNo,
                F49_ContainerNo = model.ContainerNo,
                F49_PreProductCode = model.PreProductCode,
                F49_PreProductLotNo = model.PreProductLotNo,
                F49_Amount = model.Amount,
                F49_ShelfStatus = Constants.TX49SheflStatus.TX49_StkFlg_Stk.ToString("D"),
                F49_StorageDate = model.StorageDate,
                F49_ContainerSeqNo = model.ContainerSeqNo,
                F49_AddDate = DateTime.Now,
                F49_UpdateDate = DateTime.Now,
                F49_UpdateCount = 0
            };
            _unitOfWork.PreProductShelfStockRepository.Add(tx49);

            //Update tx37
            var tx37 =
                _unitOfWork.PreProductShelfStatusRepository.Get(
                    x =>
                        x.F37_ShelfRow.Trim().Equals(model.Row) && x.F37_ShelfBay.Trim().Equals(model.Bay) &&
                        x.F37_ShelfLevel.Trim().Equals(model.Level));
            if (tx37 != null)
            {
                tx37.F37_ContainerCode = model.ContainerCode;
                tx37.F37_ContainerNo = model.ContainerNo;
                tx37.F37_ShelfStatus = Constants.TX37SheflStatus.TX37_ShfSts_Stk.ToString("D");
                tx37.F37_TerminalNo = null;
                tx37.F37_ContainerType = model.ContainerType;
                tx37.F37_StockTakingFlag = Constants.TX37StockTakingFlag.TX37_StkTkgFlg_InvNotChk.ToString("D");
                tx37.F37_StorageDate = model.StorageDate;
                tx37.F37_UpdateCount = 0;
                _unitOfWork.PreProductShelfStatusRepository.Update(tx37);
            }
            _unitOfWork.Commit();
            //Update tx42
            var tx49_1 = _unitOfWork.PreProductShelfStockRepository.GetAll().Where(x=>x.F49_KndCmdNo.Trim().Equals(model.KneadingCommandNo.Trim()) && x.F49_PreProductLotNo.Trim().Equals(model.PreProductLotNo));
            //var t = tx49_1.ToList();
            var f49amount = tx49_1.ToList().Select(x => x.F49_Amount).Sum();
            var f49Count = tx49_1.Count();
            var tx42 =
                _unitOfWork.KneadingCommandRepository.Get(
                    x =>
                        x.F42_KndCmdNo.Trim().Equals(model.KneadingCommandNo) &&
                        x.F42_PrePdtLotNo.Trim().Equals(model.PreProductLotNo) && x.F42_Status.Trim().Equals("4"));

            if (tx42 != null)
            {
                tx42.F42_ThrowAmount = f49amount;
                tx42.F42_StgCtnAmt = f49Count;
                _unitOfWork.KneadingCommandRepository.Update(tx42);
            }
            //Commit
            _unitOfWork.Commit();
        }

        public string Getkndcmd(string precode, string prelot)
        {
            var tx42 =
                _unitOfWork.KneadingCommandRepository.Get(
                    x => x.F42_PrePdtLotNo.Trim().Equals(prelot) && x.F42_PreProductCode.Trim().Equals(precode));
            if (tx42 != null)
            {
                return tx42.F42_KndCmdNo.Trim();
            }
            else
            {
                return string.Empty;
            }
        }
        #endregion
        #region f038f
        public InquiryBySupplierPalletItem GetInquiryBySupplierPalletItem(string row, string bay, string level)
        {
            var tx31s = _unitOfWork.MaterialShelfStatusRepository.GetAll();
            var tm04s = _unitOfWork.SupplierRepossitories.GetAll();
            var result = (from tx31 in tx31s
                          join tm04 in tm04s on tx31.F31_SupplierCode.Trim() equals tm04.F04_SupplierCode.Trim()
                          where tx31.F31_ShelfRow.Trim().Equals(row) && tx31.F31_ShelfBay.Trim().Equals(bay) && tx31.F31_ShelfLevel.Trim().Equals(level)
                          select new InquiryBySupplierPalletItem
                          {
                              ShelfStatus = tx31.F31_ShelfStatus,
                              SupplierCode = tx31.F31_SupplierCode,
                              SupplierName = tm04.F04_SupplierName,
                              MaxPallet = tm04.F04_MaxLoadAmount,
                              StockedPallet = tx31.F31_LoadAmount.HasValue ? tx31.F31_LoadAmount.Value : 0
                          }).FirstOrDefault();
            return result;
        }

        public void UpdateSubscreentcf038f(InquiryBySupplierPalletItem model)
        {
            Clearshelf(Constants.InquirySearchConditionWarehouseLocation.Material, model.Row, model.Bay, model.Level,false);

            //Update Tx31
            var tx31 =
                _unitOfWork.MaterialShelfStatusRepository.Get(
                    x =>
                        x.F31_ShelfRow.Trim().Equals(model.Row) && x.F31_ShelfBay.Trim().Equals(model.Bay) &&
                        x.F31_ShelfLevel.Trim().Equals(model.Level));
            if (tx31 != null)
            {
                tx31.F31_ShelfStatus = Constants.TX31SheflStatus.TX31_MtrShfSts_SplPlt.ToString("D");
                tx31.F31_TerminalNo = null;
                tx31.F31_PalletNo = null;
                tx31.F31_SupplierCode = model.SupplierCode;
                tx31.F31_LoadAmount = model.StockedPallet;
                tx31.F31_StorageDate = DateTime.Now;
                tx31.F31_UpdateDate = DateTime.Now;
                _unitOfWork.MaterialShelfStatusRepository.Update(tx31);
            }
            //Commit
            _unitOfWork.Commit();
        }

        #endregion
        #region f03Af

        public InquiryByPreProductShelfStatusEmptyItem GetInquiryByPreProductShelfStatusEmptyItem(string row, string bay, string level)
        {
            var tm08s = _unitOfWork.ContainerRepository.GetAll();
            var tx37s = _unitOfWork.PreProductShelfStatusRepository.GetAll();
            var result = (from tx37 in tx37s
                          join tm08 in tm08s on tx37.F37_ContainerType.Trim() equals tm08.F08_ContainerType.Trim()
                          where tx37.F37_ShelfRow.Trim().Equals(row) && tx37.F37_ShelfBay.Trim().Equals(bay) && tx37.F37_ShelfLevel.Trim().Equals(level)
                          select new InquiryByPreProductShelfStatusEmptyItem
                          {
                              ShelfStatus = tx37.F37_ShelfStatus.Trim(),
                              ContainerType = tm08.F08_ContainerType.Trim(),
                              ContainerName = tm08.F08_ContainerName.Trim(),
                              ContainerNo = tx37.F37_ContainerNo.Trim()
                          }).FirstOrDefault();
            return result;
        }

        public void UpdateSubscreentcf03Af(InquiryByPreProductShelfStatusEmptyItem model)
        {
            Clearshelf(Constants.InquirySearchConditionWarehouseLocation.PreProduct, model.Row, model.Bay, model.Level,false);
            var tx37 =
                _unitOfWork.PreProductShelfStatusRepository.Get(
                    x =>
                        x.F37_ShelfRow.Trim().Equals(model.Row) && x.F37_ShelfBay.Trim().Equals(model.Bay) &&
                        x.F37_ShelfLevel.Trim().Equals(model.Level));
            if (tx37 != null)
            {
                var containerNo = "000" + model.ContainerNo.ToString();
                containerNo = containerNo.Substring(containerNo.Length - 3);
                tx37.F37_ShelfStatus = ((int)Constants.TX37SheflStatus.TX37_ShfSts_EpyCtn).ToString();
                tx37.F37_ContainerNo = containerNo;
                tx37.F37_ContainerType = model.ContainerType;
                tx37.F37_TerminalNo = null;
                _unitOfWork.PreProductShelfStatusRepository.Update(tx37);
            }
            //Commit
            _unitOfWork.Commit();
        }



        #endregion
        #region f036f
        public InquiryByProductShelfStatusItem GetInquiryByProductShelfStatusItem(string row, string bay, string level)
        {
            var tx51s = _unitOfWork.ProductShelfStatusRepository.GetAll();
            var tx57s = _unitOfWork.ProductShelfRepository.GetAll();
            var result = (from tx51 in tx51s
                          join tx57 in tx57s on tx51.F51_PalletNo.Trim() equals tx57.F57_PalletNo.Trim()
                          where tx51.F51_ShelfRow.Trim().Equals(row) && tx51.F51_ShelfBay.Trim().Equals(bay) && tx51.F51_ShelfLevel.Trim().Equals(level)
                          select new InquiryByProductShelfStatusItem
                          {
                              ShelfStatus = tx51.F51_ShelfStatus.Trim(),
                              PalletNo = tx51.F51_PalletNo.Trim(),
                              StorageDate = tx57.F57_StorageDate.HasValue ? tx57.F57_StorageDate.Value : DateTime.Now,
                              ProductClassification = tx57.F57_OutFlg.Trim()
                          }).Distinct().FirstOrDefault();
            if (result != null)
            {
                var tx40s = _unitOfWork.ProductShelfStockRepository.GetAll();
                var tm09s = _unitOfWork.ProductRepository.GetAll();
                var c1s = (from tx40 in tx40s
                           join tm09 in tm09s on tx40.F40_ProductCode.Trim() equals tm09.F09_ProductCode.Trim()
                           where tx40.F40_PalletNo.Trim().Equals(result.PalletNo)
                           select new
                           {
                               tx40.F40_PrePdtLotNo,
                               tx40.F40_ProductCode,
                               tx40.F40_ProductLotNo,
                               tx40.F40_PackageAmount,
                               tx40.F40_Fraction,
                               tm09.F09_PackingUnit
                           }).ToList();
                if (c1s.Count >= 1)
                {
                    result.ProductCode1 = c1s[0].F40_ProductCode.Trim();
                    result.PreProductLotNo1 = c1s[0].F40_PrePdtLotNo.Trim();
                    result.ProductLotNo1 = c1s[0].F40_ProductLotNo.Trim();
                    result.PackQty1 = c1s[0].F40_PackageAmount;
                    result.Fraction1 = c1s[0].F40_Fraction;
                    result.Quantity1 = c1s[0].F40_Fraction + c1s[0].F09_PackingUnit * c1s[0].F40_PackageAmount;
                }

                if (c1s.Count >= 2)
                {
                    result.ProductCode2 = c1s[1].F40_ProductCode.Trim();
                    result.PreProductLotNo2 = c1s[1].F40_PrePdtLotNo.Trim();
                    result.ProductLotNo2 = c1s[1].F40_ProductLotNo.Trim();
                    result.PackQty2 = c1s[1].F40_PackageAmount;
                    result.Fraction2 = c1s[1].F40_Fraction;
                    result.Quantity2 = c1s[1].F40_Fraction + c1s[1].F09_PackingUnit * c1s[1].F40_PackageAmount;
                }

                if (c1s.Count >= 3)
                {
                    result.ProductCode3 = c1s[2].F40_ProductCode.Trim();
                    result.PreProductLotNo3 = c1s[2].F40_PrePdtLotNo.Trim();
                    result.ProductLotNo3 = c1s[2].F40_ProductLotNo.Trim();
                    result.PackQty3 = c1s[2].F40_PackageAmount;
                    result.Fraction3 = c1s[2].F40_Fraction;
                    result.Quantity3 = c1s[2].F40_Fraction + c1s[2].F09_PackingUnit*c1s[2].F40_PackageAmount;
                }

                if (c1s.Count >= 4)
                {
                    result.ProductCode4 = c1s[3].F40_ProductCode.Trim();
                    result.PreProductLotNo4 = c1s[3].F40_PrePdtLotNo.Trim();
                    result.ProductLotNo4 = c1s[3].F40_ProductLotNo.Trim();
                    result.PackQty4 = c1s[3].F40_PackageAmount;
                    result.Fraction4 = c1s[3].F40_Fraction;
                    result.Quantity4 = c1s[3].F40_Fraction + c1s[3].F09_PackingUnit*c1s[3].F40_PackageAmount;
                }

                if (c1s.Count >= 5)
                {
                    result.ProductCode5 = c1s[4].F40_ProductCode.Trim();
                    result.PreProductLotNo5 = c1s[4].F40_PrePdtLotNo.Trim();
                    result.ProductLotNo5 = c1s[4].F40_ProductLotNo.Trim();
                    result.PackQty5 = c1s[4].F40_PackageAmount;
                    result.Fraction5 = c1s[4].F40_Fraction;
                    result.Quantity5 = c1s[4].F40_Fraction + c1s[4].F09_PackingUnit*c1s[4].F40_PackageAmount;
                }
            }
            return result;
        }
        public string GetProductLotNo036f(string productcode, string preProductLotNo, int productClassification)
        {
            if (productClassification == 0)
            {
                var tx56 =
                    _unitOfWork.TabletProductRepository.Get(
                        x =>
                            x.F56_ProductCode.Trim().Equals(productcode) &&
                            x.F56_PrePdtLotNo.Trim().Equals(preProductLotNo));
                if (tx56 != null)
                {
                    return tx56.F56_ProductLotNo.Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
            else if (productClassification == 1)
            {
                var tx58 =
                    _unitOfWork.OutOfPlanProductRepository.Get(
                        x =>
                            x.F58_ProductCode.Trim().Equals(productcode) &&
                            x.F58_PrePdtLotNo.Trim().Equals(preProductLotNo));
                if (tx58 != null)
                {
                    return tx58.F58_ProductLotNo.Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
            return string.Empty;

        }
        public double GetPackingUnit036f(string productcode)
        {
            var tx09 =
                _unitOfWork.ProductRepository.Get(
                    x =>
                            x.F09_ProductCode.Trim().Equals(productcode));
            if (tx09 != null)
            {
                return tx09.F09_PackingUnit;
            }
            else
            {
                return 0;
            }
        }
        public void UpdateSubscreentcf036f(InquiryByProductShelfStatusItem model)
        {
            //TODO implement agian update functionwith case [Product Classification] = “Out of Plan”. Sai business khi GetExInfor turn null
            Clearshelf(Constants.InquirySearchConditionWarehouseLocation.Product, model.Row, model.Bay, model.Level,false);
            // insert record into tx57
            var tx57new = new TX57_PdtShf
            {
                F57_PalletNo = model.PalletNo,
                F57_StorageDate = model.StorageDate,
                F57_ReStorageDate = null,
                F57_RetievalDate = null,
                F57_OutFlg = model.ProductClassification,
                F57_AddDate = DateTime.Now,
                F57_UpdateDate = DateTime.Now,
                F57_UpdateCount = 0
            };
            _unitOfWork.ProductShelfRepository.Add(tx57new);

            // update tx51
            var tx51 =
                _unitOfWork.ProductShelfStatusRepository.Get(
                    x =>
                        x.F51_ShelfRow.Trim().Equals(model.Row) && x.F51_ShelfBay.Trim().Equals(model.Bay) &&
                        x.F51_ShelfLevel.Trim().Equals(model.Level));
            if (tx51 != null)
            {
                tx51.F51_ShelfStatus = "2"; //TX51_ShfSts_Pdt
                tx51.F51_LoadAmount = 0;
                tx51.F51_StockTakingFlag = "0";//TX51_StkTkgFlg_InvNotChk
                tx51.F51_PalletNo = model.PalletNo;
                tx51.F51_TerminalNo = null;
                tx51.F51_StorageDate = DateTime.Now;
                _unitOfWork.ProductShelfStatusRepository.Update(tx51);
            }
            // inset into tx40

            
            if (!string.IsNullOrEmpty(model.ProductCode1) && !string.IsNullOrEmpty(model.PreProductLotNo1))
            {
                var exInfo = GetExInfor(model.PreProductLotNo1, model.ProductCode1, model.ProductClassification);
                if (exInfo != null)
                {
                    var tx40_1 = new TX40_PdtShfStk
                    {
                        F40_PalletNo = model.PalletNo,
                        F40_PrePdtLotNo = model.PreProductLotNo1,
                        F40_ProductCode = model.ProductCode1,
                        F40_ProductLotNo = model.ProductLotNo1,
                        F40_StockFlag = "3",
                        F40_PackageAmount = model.PackQty1,
                        F40_Fraction = model.Fraction1,
                        F40_Amount = model.Quantity1,
                        F40_TabletingEndDate = exInfo.Item1.Value,
                        F40_ShippedAmount = 0,
                        F40_ShipCommandNo = null,
                        F40_AssignAmount = null,
                        F40_CertificationFlg = exInfo.Item2,
                        F40_CertificationDate = exInfo.Item3.Value,
                        F40_AddDate = DateTime.Now,
                        F40_UpdateDate = DateTime.Now,
                        F40_UpdateCount = 0
                    };
                    _unitOfWork.ProductShelfStockRepository.Add(tx40_1);
                }
                
            }


            if (!string.IsNullOrEmpty(model.ProductCode2) && !string.IsNullOrEmpty(model.PreProductLotNo2))
            {
                var exInfo = GetExInfor(model.PreProductLotNo2, model.ProductCode2, model.ProductClassification);
                if (exInfo != null)
                {
                    var tx40_2 = new TX40_PdtShfStk
                    {
                        F40_PalletNo = model.PalletNo,
                        F40_PrePdtLotNo = model.PreProductLotNo2,
                        F40_ProductCode = model.ProductCode2,
                        F40_ProductLotNo = model.ProductLotNo2,
                        F40_StockFlag = "3",
                        F40_PackageAmount = model.PackQty2,
                        F40_Fraction = model.Fraction2,
                        F40_Amount = model.Quantity2,
                        F40_TabletingEndDate = exInfo.Item1.Value,
                        F40_ShippedAmount = 0,
                        F40_ShipCommandNo = null,
                        F40_AssignAmount = null,
                        F40_CertificationFlg = exInfo.Item2,
                        F40_CertificationDate = exInfo.Item3.Value,
                        F40_AddDate = DateTime.Now,
                        F40_UpdateDate = DateTime.Now,
                        F40_UpdateCount = 0
                    };
                    _unitOfWork.ProductShelfStockRepository.Add(tx40_2);
                }

            }

            if (!string.IsNullOrEmpty(model.ProductCode3) && !string.IsNullOrEmpty(model.PreProductLotNo3))
            {
                var exInfo = GetExInfor(model.PreProductLotNo3, model.ProductCode3, model.ProductClassification);
                if (exInfo != null)
                {
                    var tx40_3 = new TX40_PdtShfStk
                    {
                        F40_PalletNo = model.PalletNo,
                        F40_PrePdtLotNo = model.PreProductLotNo3,
                        F40_ProductCode = model.ProductCode3,
                        F40_ProductLotNo = model.ProductLotNo3,
                        F40_StockFlag = "3",
                        F40_PackageAmount = model.PackQty3,
                        F40_Fraction = model.Fraction3,
                        F40_Amount = model.Quantity3,
                        F40_TabletingEndDate = exInfo.Item1.Value,
                        F40_ShippedAmount = 0,
                        F40_ShipCommandNo = null,
                        F40_AssignAmount = null,
                        F40_CertificationFlg = exInfo.Item2,
                        F40_CertificationDate = exInfo.Item3.Value,
                        F40_AddDate = DateTime.Now,
                        F40_UpdateDate = DateTime.Now,
                        F40_UpdateCount = 0
                    };
                    _unitOfWork.ProductShelfStockRepository.Add(tx40_3);
                }
            }

            if (!string.IsNullOrEmpty(model.ProductCode4) && !string.IsNullOrEmpty(model.PreProductLotNo4))
                {
                    var exInfo = GetExInfor(model.PreProductLotNo4, model.ProductCode4, model.ProductClassification);
                    if (exInfo!=null)
                    {
                        var tx40_4 = new TX40_PdtShfStk
                        {
                            F40_PalletNo = model.PalletNo,
                            F40_PrePdtLotNo = model.PreProductLotNo4,
                            F40_ProductCode = model.ProductCode4,
                            F40_ProductLotNo = model.ProductLotNo4,
                            F40_StockFlag = "3",
                            F40_PackageAmount = model.PackQty4,
                            F40_Fraction = model.Fraction4,
                            F40_Amount = model.Quantity4,
                            F40_TabletingEndDate = exInfo.Item1.Value,
                            F40_ShippedAmount = 0,
                            F40_ShipCommandNo = null,
                            F40_AssignAmount = null,
                            F40_CertificationFlg = exInfo.Item2,
                            F40_CertificationDate = exInfo.Item3.Value,
                            F40_AddDate = DateTime.Now,
                            F40_UpdateDate = DateTime.Now,
                            F40_UpdateCount = 0
                        };
                        _unitOfWork.ProductShelfStockRepository.Add(tx40_4);
                    }

                }

            if (!string.IsNullOrEmpty(model.ProductCode5) && !string.IsNullOrEmpty(model.PreProductLotNo5))
            {
                var exInfo = GetExInfor(model.PreProductLotNo5, model.ProductCode5, model.ProductClassification);
                if (exInfo != null)
                {
                    var tx40_5 = new TX40_PdtShfStk
                    {
                        F40_PalletNo = model.PalletNo,
                        F40_PrePdtLotNo = model.PreProductLotNo5,
                        F40_ProductCode = model.ProductCode5,
                        F40_ProductLotNo = model.ProductLotNo5,
                        F40_StockFlag = "3",
                        F40_PackageAmount = model.PackQty5,
                        F40_Fraction = model.Fraction5,
                        F40_Amount = model.Quantity5,
                        F40_TabletingEndDate = exInfo.Item1.Value,
                        F40_ShippedAmount = 0,
                        F40_ShipCommandNo = null,
                        F40_AssignAmount = null,
                        F40_CertificationFlg = exInfo.Item2,
                        F40_CertificationDate = exInfo.Item3.Value,
                        F40_AddDate = DateTime.Now,
                        F40_UpdateDate = DateTime.Now,
                        F40_UpdateCount = 0
                    };
                    _unitOfWork.ProductShelfStockRepository.Add(tx40_5);
                }

            }

            //Commit
            _unitOfWork.Commit();
        }

        public bool ProductLotNoNotbeentabletised(string productcode, string preProductLotNo,int productClassification)
        {
            if (productClassification == 0)
            {
                var tx56 =
                    _unitOfWork.TabletProductRepository.Get(
                        x =>
                            x.F56_ProductCode.Trim().Equals(productcode) &&
                            x.F56_PrePdtLotNo.Trim().Equals(preProductLotNo) && (x.F56_Status.Equals("3") || x.F56_Status.Equals("4")));
                if (tx56 != null)
                {
                    return true;
                }
            }
            else if (productClassification == 1)
            {
                var tx58 =
                    _unitOfWork.OutOfPlanProductRepository.Get(
                        x =>
                            x.F58_ProductCode.Trim().Equals(productcode) &&
                            x.F58_PrePdtLotNo.Trim().Equals(preProductLotNo) && (x.F58_Status.Equals("3") || x.F58_Status.Equals("4")));
                if (tx58 != null)
                {
                    return true;
                }
            }
            return false;
        }
        public bool PalletNoExit(string palletNo)
        {
            var tx57s = _unitOfWork.ProductShelfRepository.GetAll().Where(x => x.F57_PalletNo.Trim().Equals(palletNo));
            return tx57s.Any();
        }

        private Tuple<DateTime?, string, DateTime?> GetExInfor(string prelot, string pcode, string productClassification)
        {
            if (productClassification.Equals("0"))
            {
                var tx56 =
                    _unitOfWork.TabletProductRepository.GetAll()
                        .Where(
                            x =>
                                x.F56_PrePdtLotNo.Trim().Equals(prelot) &&
                                x.F56_ProductCode.Trim().Equals(pcode) &&
                                (x.F56_Status.Trim().Equals("3") || x.F56_Status.Trim().Equals("4")))
                        .OrderByDescending(x => x.F56_AddDate)
                        .FirstOrDefault();
                if (tx56 != null)
                {
                    return new Tuple<DateTime?, string, DateTime?>(tx56.F56_TbtEndDate, tx56.F56_CertificationFlag.Trim(), tx56.F56_CertificationDate);
                }
            }
            else
            {
                var tx58 =
                    _unitOfWork.OutOfPlanProductRepository.Get(x =>
                                x.F58_ProductCode.Trim().Equals(pcode) &&
                                x.F58_PrePdtLotNo.Trim().Equals(prelot));
                if (tx58 != null)
                {
                    return new Tuple<DateTime?, string, DateTime?>(tx58.F58_TbtEndDate, tx58.F58_CertificationFlag.Trim(), tx58.F58_CertificationDate);
                }
            }
            return null;
        }
        #endregion
        #region f037f
        public InquiryByProductShelfStatusExternalPreProductItem GetProductShelftStatusExternalPreProductItem(string row, string bay, string level)
        {
            var tx51s = _unitOfWork.ProductShelfStatusRepository.GetAll();
            var tx57s = _unitOfWork.ProductShelfRepository.GetAll();
            var tm03s = _unitOfWork.PreProductRepository.GetAll();
            var tx53s = _unitOfWork.OutSidePrePdtStkRepository.GetAll();
            var result = (from tx51 in tx51s
                join tx57 in tx57s on tx51.F51_PalletNo.Trim() equals tx57.F57_PalletNo.Trim()
                join tx53 in tx53s on tx57.F57_PalletNo.Trim() equals tx53.F53_PalletNo.Trim()
                join tm03 in tm03s on tx53.F53_OutSidePrePdtCode.Trim() equals tm03.F03_PreProductCode.Trim()
                where
                tx51.F51_ShelfRow.Trim().Equals(row) && tx51.F51_ShelfBay.Trim().Equals(bay) &&
                tx51.F51_ShelfLevel.Trim().Equals(level)
                select new InquiryByProductShelfStatusExternalPreProductItem
                {
                    ShelfStatus = tx51.F51_ShelfStatus.Trim(),
                    PalletNo = tx51.F51_PalletNo.Trim(),
                    StorageDate = tx57.F57_StorageDate.HasValue ? tx57.F57_StorageDate.Value : DateTime.Now,
                    PreProductCode = tx53.F53_OutSidePrePdtCode.Trim(),
                    PreProductLotNo = tx53.F53_OutSidePrePdtLotNo.Trim(),
                    KneadingCommandNo = tx53.F53_KndCmdNo.Trim(),
                    PalletSeqNo = tx53.F53_PalletSeqNo,
                    PreProductName = tm03.F03_PreProductName.Trim(),
                    Amount = tx53.F53_Amount
                }).FirstOrDefault();
            return result;

        }
        public void UpdateSubscreentcf037f(InquiryByProductShelfStatusExternalPreProductItem model)
        {
            Clearshelf(Constants.InquirySearchConditionWarehouseLocation.Product, model.Row, model.Bay, model.Level,false);
            // insert tx57
            var tx57new = new TX57_PdtShf
            {
                F57_PalletNo = model.PalletNo,
                F57_StorageDate = model.StorageDate,
                F57_OutFlg = "1", //TX57_OutFlg_Out
                F57_AddDate = DateTime.Now,
                F57_UpdateDate = DateTime.Now,
                F57_UpdateCount = 0
            };
            _unitOfWork.ProductShelfRepository.Add(tx57new);

            // Insert Tx53
            var tx53 = new TX53_OutSidePrePdtStk
            {
                F53_PalletNo = model.PalletNo,
                F53_OutSidePrePdtCode = model.PreProductCode,
                F53_OutSidePrePdtLotNo = model.PreProductLotNo,
                F53_KndCmdNo = model.KneadingCommandNo,
                F53_PalletSeqNo = model.PalletSeqNo,
                F53_Amount = model.Amount,
                F53_StockFlag = "3", //TX53_StkFlg_Stk
                F53_AddDate = DateTime.Now,
                F53_UpdateDate = DateTime.Now,
                F53_UpdateCount = 0
            };
            _unitOfWork.OutSidePrePdtStkRepository.Add(tx53);
            // update tx51
            var tx51 =
                _unitOfWork.ProductShelfStatusRepository.Get(
                    x =>
                        x.F51_ShelfRow.Trim().Equals(model.Row) && x.F51_ShelfBay.Trim().Equals(model.Bay) &&
                        x.F51_ShelfLevel.Trim().Equals(model.Level));
            if (tx51 != null)
            {
                tx51.F51_ShelfStatus = "8"; //TX51_ShfSts_ExtPrePdt
                tx51.F51_PalletNo = model.PalletNo;
                tx51.F51_TerminalNo = null;
                tx51.F51_StorageDate = DateTime.Now;
                _unitOfWork.ProductShelfStatusRepository.Update(tx51);
            }
            //Commit
            _unitOfWork.Commit();

        }

        public bool PreProductIsexternal(string precode, string lotno)
        {
            var tx42 =
                _unitOfWork.KneadingCommandRepository.Get(
                    x =>
                    x.F42_OutSideClass.Trim().Equals("1") && //TX42_OutSideCls_ExtPrepdt
                        x.F42_PreProductCode.Trim().Equals(precode) &&
                        x.F42_PrePdtLotNo.Trim().Equals(lotno));
            return tx42 != null;
        }

        public bool PalletSeqNoExit(int sepno)
        {
            var tx53 = _unitOfWork.OutSidePrePdtStkRepository.Get(x => x.F53_PalletSeqNo == sepno);
            return tx53 != null;
        }
        #endregion
        #region f03BF

        public InquiryByProductShelfStatusEmptyItem GetInquiryByProductShelfStatusEmptyItem(string row, string bay, string level)
        {
            var tx51 = _unitOfWork.ProductShelfStatusRepository.Get(x=>x.F51_ShelfRow.Trim().Equals(row) && x.F51_ShelfBay.Trim().Equals(bay) && x.F51_ShelfLevel.Trim().Equals(level));

            if (tx51 != null)
            {
                return new InquiryByProductShelfStatusEmptyItem
                {
                    PalletLoadAmout = tx51.F51_LoadAmount,
                    ShelfStatus = tx51.F51_ShelfStatus.Trim(),
                };
            }
            return null;
        }

        public void UpdateSubscreentcf03Bf(InquiryByProductShelfStatusEmptyItem model)
        {
            Clearshelf(Constants.InquirySearchConditionWarehouseLocation.Product, model.Row, model.Bay, model.Level,false);
            //update tx_51
            var tx51 =
                _unitOfWork.ProductShelfStatusRepository.Get(
                    x =>
                        x.F51_ShelfRow.Trim().Equals(model.Row) && x.F51_ShelfBay.Trim().Equals(model.Bay) &&
                        x.F51_ShelfLevel.Trim().Equals(model.Level));
            if (tx51 != null)
            {
                tx51.F51_ShelfStatus = "1"; //TX51_ShfSts_WhsPlt
                tx51.F51_TerminalNo = null;
                tx51.F51_LoadAmount = model.PalletLoadAmout;
                tx51.F51_StorageDate = DateTime.Now;
                _unitOfWork.ProductShelfStatusRepository.Update(tx51);
            }
            //Commit
            _unitOfWork.Commit();
        }

        public void Clearshelf(Constants.InquirySearchConditionWarehouseLocation type, string row, string bay, string level)
        {
            throw new NotImplementedException();
        }

        public void Setshelf(Constants.InquirySearchConditionWarehouseLocation type, string row, string bay, string level, string status)
        {
            throw new NotImplementedException();
        }



        #endregion

    }
}
