using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.ProductionPlanning
{
    public class PreProductDomain : BaseDomain, IPreProductDomain
    {
       

        #region Constructor

        public PreProductDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
           
        }

        #endregion

        #region Methods

        public PreProductItem GetById(string id)
        {
            var entity = _unitOfWork.PreProductRepository.GetById(id);

            if (entity!=null&& (entity.F03_KneadingLine.Equals(((int)Constants.F39_KneadingLine.ConventionalC).ToString()) &&
                entity.F03_ColorClass == ((int)Constants.ColorClass.Black).ToString()))
            {
                entity.F03_KneadingLine = "2";
            }
            return Mapper.Map<PreProductItem>(entity);
        }

        public void Create(PreProductItem preProduct)
        {
            var entity = Mapper.Map<TM03_PreProduct>(preProduct);
            _unitOfWork.PreProductRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(PreProductItem preProduct)
        {
            var entity = Mapper.Map<TM03_PreProduct>(preProduct);
            _unitOfWork.PreProductRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public void Delete(string preProductCode)
        {
            if (!IfMaking(preProductCode)) {
                _unitOfWork.PrePdtMkpRepository.Delete(ppm => preProductCode.Trim().Equals(ppm.F02_PreProductCode.Trim()));
                _unitOfWork.PreProductRepository.Delete(pp => preProductCode.Trim().Equals(pp.F03_PreProductCode.Trim()));
                //Missing wf_mast_sndmsg???
                _unitOfWork.Commit();
            }
        }

        public bool CheckUnique(string preProductCode)
        {
            return _unitOfWork.PreProductRepository.GetAll().Any(m => m.F03_PreProductCode.Trim().Equals(preProductCode));
        }

        public  TX52_MtrMsrSndCmd InsertTX52(string kneadingLine, string preProductCode)
        {
            var mtrMsrSndCmd = new TX52_MtrMsrSndCmd();
            mtrMsrSndCmd.F52_TerminalNo = Constants.TerminalNo.A017;//TODO get TerminalNo for user
            mtrMsrSndCmd.F52_AddDate = DateTime.Now;
            if (kneadingLine.Equals(((int)Constants.F39_KneadingLine.Megabit).ToString()))
            {
                mtrMsrSndCmd.F52_MsrMacCls = ((int)Constants.F52_MsrMacCls.Mega).ToString();
            }
            else
            {
                mtrMsrSndCmd.F52_MsrMacCls = ((int)Constants.F52_MsrMacCls.Conv).ToString();
            }
            mtrMsrSndCmd.F52_CommandType = Constants.F52_CommandType.TX52_CmdType_PrePdtAdd;
            mtrMsrSndCmd.F52_Status = ((int)Constants.F55_Status.SentFromAToC).ToString();
            mtrMsrSndCmd.F52_Priority = ((int)Constants.Default.value).ToString();
            mtrMsrSndCmd.F52_MasterCode = preProductCode;
            mtrMsrSndCmd.F52_PictureNo = Constants.PictureNo.TCPP022F;
            mtrMsrSndCmd.F52_AbnormalCode = "";
            mtrMsrSndCmd.F52_UpdateDate = DateTime.Now;
            mtrMsrSndCmd.F52_UpdateCount = 0;
            _unitOfWork.MtrMsrSndCmdRepository.Add(mtrMsrSndCmd);
            return mtrMsrSndCmd;

        }
        public ResponseResult CreateOrUpdate(PreProductItem model)
        {
           
            if (model.IsCreate)
            {
                if (!string.IsNullOrEmpty(model.F03_PreProductCode))
                {
                    if (CheckUnique(model.F03_PreProductCode))
                    {
                        return new ResponseResult(false, Constants.Messages.Material_MSG004);
                    }
                }
                var entity = Mapper.Map<TM03_PreProduct>(model);
                entity = SetColorClass(entity);
               // entity = SetKneadingLine(entity);
                entity.F03_AddDate = DateTime.Now;
                entity.F03_UpdateDate = DateTime.Now;
                entity.F03_UpdateCount = 0;
                _unitOfWork.PreProductRepository.Add(entity);
                _unitOfWork.PrePdtMkpRepository.UpdateForSavePreProduct(entity,true);
                InsertTX52(entity.F03_KneadingLine, entity.F03_PreProductCode);

            }
            else
            {
                var entity = _unitOfWork.PreProductRepository.GetById(model.F03_PreProductCode);
                if (entity == null)
                {
                    return new ResponseResult(false, Constants.Messages.Material_MSG001);
                }
                Mapper.Map(model, entity);
                entity = SetColorClass(entity);
              //  entity = SetKneadingLine(entity);
                entity.F03_UpdateDate = DateTime.Now;
                entity.F03_UpdateCount += 1;
                _unitOfWork.PrePdtMkpRepository.UpdateForSavePreProduct(entity,false);
                _unitOfWork.PreProductRepository.Update(entity);
            }
            
            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        public ResponseResult<GridResponse<PreProductItem>> Search(string preProductCode, GridSettings gridSettings)
        {
            var result = _unitOfWork.PreProductRepository.GetAll();
            if (!string.IsNullOrEmpty(preProductCode))
            {
                result = result.Where(i => i.F03_PreProductCode.ToUpper().Contains(preProductCode.ToUpper()));
            }
            var itemCount = result.Count();
            OrderByAndPaging(ref  result, gridSettings);
            var lstResult = Mapper.Map<IEnumerable<TM03_PreProduct>, IEnumerable<PreProductItem>>(result);
            var resultModel = new GridResponse<PreProductItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<PreProductItem>>(resultModel, true);
        }

        /// <summary>
        /// Search pre-product records for printing.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        public ResponseResult<GridResponse<PrintPreProductItem>> SearchMaterialList(string request, GridSettings gridSettings)
        {

            var lstPreproduct = _unitOfWork.PreProductRepository.GetAll();
            if (!string.IsNullOrEmpty(request))
                lstPreproduct = lstPreproduct.Where(i => i.F03_PreProductCode.ToUpper().Contains(request.ToUpper()));

            var lstPreProductCode = lstPreproduct.Select(i => i.F03_PreProductCode);
            var lstPrePdtMkp =
                _unitOfWork.PrePdtMkpRepository.GetAll().Where(i => lstPreProductCode.Contains(i.F02_PreProductCode));
            var lstMaterial = _unitOfWork.MaterialRepository.GetAll();
            var itemCount = lstPrePdtMkp.Count();
            if (gridSettings != null)
            {
                lstPrePdtMkp = lstPrePdtMkp.OrderBy(i => i.F02_PreProductCode).ThenBy(i => i.F02_LayinPriority)
                    .ThenBy(i => i.F02_ThrawSeqNo)
                    .ThenBy(i => i.F02_PotSeqNo)
                    .ThenBy(i => i.F02_MsrSeqNo);
                lstPrePdtMkp =
                    lstPrePdtMkp.Skip((gridSettings.PageIndex - 1)*gridSettings.PageSize).Take(gridSettings.PageSize);
            }
            var lstResult = (from prePdtMkp in lstPrePdtMkp
                join preProduct in lstPreproduct on prePdtMkp.F02_PreProductCode equals preProduct.F03_PreProductCode
                join tm01Material in lstMaterial on prePdtMkp.F02_MaterialCode equals tm01Material.F01_MaterialCode
                select new
                {
                    preProduct,
                    tm01Material,
                    prePdtMkp
                }).ToList().Select(i => new PrintPreProductItem()
                {
                    MasterialName = i.tm01Material.F01_MaterialDsp,
                    MasterialCode = i.prePdtMkp.F02_MaterialCode,
                    Sum3F4F = i.prePdtMkp.F02_3FLayinAmount + i.prePdtMkp.F02_4FLayinAmount,
                    WSeq = i.prePdtMkp.F02_MsrSeqNo,
                    C_Pri = i.prePdtMkp.F02_LayinPriority,
                    PSeq = i.prePdtMkp.F02_PotSeqNo,
                    CSeq = i.prePdtMkp.F02_ThrawSeqNo,
                    LoadPosition = i.prePdtMkp.F02_LoadPosition,
                    Method = Enum.GetName(typeof (Constants.WeighingMethod), i.prePdtMkp.F02_WeighingMethod),
                    Additive =
                        Enum.GetName(typeof (Constants.Additive), ConvertHelper.ToInteger(i.prePdtMkp.F02_Addtive)),
                    MilingFlag1 =
                        EnumsHelper.GetDescription<Constants.Crushing>(
                            ConvertHelper.ToInteger(i.prePdtMkp.F02_MilingFlag1)),
                    MilingFlag2 =
                        EnumsHelper.GetDescription<Constants.Crushing>(
                            ConvertHelper.ToInteger(i.prePdtMkp.F02_MilingFlag2)),
                    F03_AllMtrAmtPerBth = i.preProduct.F03_AllMtrAmtPerBth,
                    F03_BatchLot = i.preProduct.F03_BatchLot,
                    F03_ColorClass = i.preProduct.F03_ColorClass,
                    F03_ContainerType = i.preProduct.F03_ContainerType,
                    F03_KneadingLine = i.preProduct.F03_KneadingLine,
                    F03_LotNoEnd = i.preProduct.F03_LotNoEnd,
                    F03_LowTmpClass = i.preProduct.F03_LowTmpClass,
                    MixDate1 =
                        i.preProduct.F03_MixDate1.HasValue ? i.preProduct.F03_MixDate1.Value.ToString("HH:mm:ss") : "",
                    MixDate2 =
                        i.preProduct.F03_MixDate2.HasValue ? i.preProduct.F03_MixDate2.Value.ToString("HH:mm:ss") : "",
                    F03_MixMode = i.preProduct.F03_MixMode,
                    MixDate3 =
                        i.preProduct.F03_MixDate3.HasValue ? i.preProduct.F03_MixDate3.Value.ToString("HH:mm:ss") : "",
                    F03_Point = i.preProduct.F03_Point,
                    F03_PreProductCode = i.preProduct.F03_PreProductCode,
                    F03_PreProductName = i.preProduct.F03_PreProductName,
                    F03_TmpRetTime = i.preProduct.F03_TmpRetTime,
                    F03_YieldRate = i.preProduct.F03_YieldRate

                });







            //var lstResult = new List<PrintPreProductItem>();
            //foreach (var tm02PrePdtMkp in lstPrePdtMkp)
            //{
            //    var tm03PreProduct =
            //        lstPreproduct.FirstOrDefault(i => i.F03_PreProductCode == tm02PrePdtMkp.F02_PreProductCode);
            //    var printPreProduct = Mapper.Map<TM03_PreProduct, PrintPreProductItem>(tm03PreProduct);
            //    printPreProduct.MasterialCode = tm02PrePdtMkp.F02_MaterialCode;
            //    var marterial = lstMaterial.FirstOrDefault(i => i.F01_MaterialCode == tm02PrePdtMkp.F02_MaterialCode);

            //    if (marterial != null)
            //        printPreProduct.MasterialName = marterial.F01_MaterialDsp;
            //    printPreProduct.MasterialCode = tm02PrePdtMkp.F02_MaterialCode;
            //    printPreProduct.Sum3F4F = tm02PrePdtMkp.F02_3FLayinAmount + tm02PrePdtMkp.F02_4FLayinAmount;
            //    printPreProduct.WSeq = tm02PrePdtMkp.F02_MsrSeqNo;
            //    printPreProduct.C_Pri = tm02PrePdtMkp.F02_ThrawSeqNo;
            //    printPreProduct.PSeq = tm02PrePdtMkp.F02_PotSeqNo;
            //    printPreProduct.CSeq = tm02PrePdtMkp.F02_LayinPriority;
            //    printPreProduct.LoadPosition = tm02PrePdtMkp.F02_LoadPosition;
            //    printPreProduct.Method = Enum.GetName(typeof(Constants.WeighingMethod), tm02PrePdtMkp.F02_WeighingMethod); 
            //    printPreProduct.Additive =Enum.GetName(typeof(Constants.Additive),ConvertHelper.ToInteger(tm02PrePdtMkp.F02_Addtive));
            //    printPreProduct.MilingFlag1 =EnumsHelper.GetDescription<Constants.Crushing>(ConvertHelper.ToInteger( tm02PrePdtMkp.F02_MilingFlag1));
            //    printPreProduct.MilingFlag2 = EnumsHelper.GetDescription<Constants.Crushing>(ConvertHelper.ToInteger(tm02PrePdtMkp.F02_MilingFlag2)); 
            //    lstResult.Add(printPreProduct);
            //}

            //  IQueryable<PrintPreProductItem> lstreuslt1 = lstResult.AsQueryable();

            var resultModel = new GridResponse<PrintPreProductItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<PrintPreProductItem>>(resultModel, true);
        }

        public ResponseResult<GridResponse<PrintPreProductItem>> MaterialListPrint(string request)
        {

            var lstPreproduct = _unitOfWork.PreProductRepository.GetAll();
            if (!string.IsNullOrEmpty(request))
                lstPreproduct = lstPreproduct.Where(i => i.F03_PreProductCode.ToUpper().Contains(request.ToUpper()));

            var lstPreProductCode = lstPreproduct.Select(i => i.F03_PreProductCode);
            var lstPrePdtMkp =
                _unitOfWork.PrePdtMkpRepository.GetAll().Where(i => lstPreProductCode.Contains(i.F02_PreProductCode));
            var lstMaterial = _unitOfWork.MaterialRepository.GetAll();
            var itemCount = lstPrePdtMkp.Count();
            lstPrePdtMkp = lstPrePdtMkp.OrderBy(i => i.F02_PreProductCode).ThenBy(i => i.F02_LayinPriority)
                .ThenBy(i => i.F02_ThrawSeqNo)
                .ThenBy(i => i.F02_PotSeqNo)
                .ThenBy(i => i.F02_MsrSeqNo);
            var lstResult = lstPreproduct.ToList().Select(i => new PrintPreProductItem()
            {
                MaterialPrintItems = new List<MaterialPrintItem>(
                    lstPrePdtMkp.Where(m => m.F02_PreProductCode == i.F03_PreProductCode)
                        .ToList().Select(o => new MaterialPrintItem()
                        {
                            MasterialName =
                                lstMaterial.FirstOrDefault(k => k.F01_MaterialCode == o.F02_MaterialCode)!=null?
                                    lstMaterial.FirstOrDefault(k => k.F01_MaterialCode == o.F02_MaterialCode).F01_MaterialDsp:"",
                            MasterialCode = o.F02_MaterialCode,
                            Sum3F4F = (o.F02_3FLayinAmount + o.F02_4FLayinAmount).ToString("N"),
                            WSeq = o.F02_MsrSeqNo,
                            C_Pri = o.F02_LayinPriority,
                            PSeq = o.F02_PotSeqNo,
                            CSeq = o.F02_ThrawSeqNo,
                            LoadPosition = o.F02_LoadPosition,
                            Method = Enum.GetName(typeof (Constants.WeighingMethod), o.F02_WeighingMethod),
                            Additive =
                                Enum.GetName(typeof (Constants.Additive), ConvertHelper.ToInteger(o.F02_Addtive)),
                            MilingFlag1 =
                                EnumsHelper.GetDescription<Constants.Crushing>(
                                    ConvertHelper.ToInteger(o.F02_MilingFlag1)),
                            MilingFlag2 =
                                EnumsHelper.GetDescription<Constants.Crushing>(
                                    ConvertHelper.ToInteger(o.F02_MilingFlag2)),
                        })),
                F03_AllMtrAmtPerBth = i.F03_AllMtrAmtPerBth,
                F03_BatchLot = i.F03_BatchLot,
                F03_ColorClass = i.F03_ColorClass,
                F03_ContainerType = i.F03_ContainerType,
                F03_KneadingLine = i.F03_KneadingLine,
                F03_LotNoEnd = i.F03_LotNoEnd,
                F03_LowTmpClass = i.F03_LowTmpClass,
                MixDate1 =
                    i.F03_MixDate1.HasValue ? i.F03_MixDate1.Value.ToString("HH:mm:ss") : "",
                MixDate2 =
                    i.F03_MixDate2.HasValue ? i.F03_MixDate2.Value.ToString("HH:mm:ss") : "",
                F03_MixMode = i.F03_MixMode,
                MixDate3 =
                    i.F03_MixDate3.HasValue ? i.F03_MixDate3.Value.ToString("HH:mm:ss") : "",
                F03_Point = i.F03_Point,
                F03_PreProductCode = i.F03_PreProductCode,
                F03_PreProductName = i.F03_PreProductName,
                F03_TmpRetTime = i.F03_TmpRetTime,
                YieldRate = i.F03_YieldRate.ToString("F")
            });


            var resultModel = new GridResponse<PrintPreProductItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<PrintPreProductItem>>(resultModel, true);
        }

        public TM03_PreProduct SetColorClass(TM03_PreProduct model)
        {
           
            if (model.F03_KneadingLine.Equals(((int)Constants.F39_KneadingLine.ConventionalC).ToString()))
            {
                model.F03_ColorClass = ((int)Constants.ColorClass.Color).ToString();
            }
            else
            {
                model.F03_ColorClass = ((int)Constants.ColorClass.Black).ToString();
            }
            if (model.F03_KneadingLine == "2")
            {
                model.F03_KneadingLine = "1";
            }
            return model;
        }

        public TM03_PreProduct SetKneadingLine(TM03_PreProduct model)
        {
            if (model.F03_KneadingLine.Equals(((int)Constants.F39_KneadingLine.Megabit).ToString()))
            {
                model.F03_KneadingLine = ((int)Constants.KndLine.Megabit).ToString();
            }
            else
            {
                model.F03_KneadingLine = ((int)Constants.KndLine.Conventional).ToString();
            }
            return model;
        }

        //need to move to common
        public bool IfMaking(string preProductCode)
        {
            var result = _unitOfWork.PdtPlnRepository.GetAll()
                .Where(p => p.F39_PreProductCode.Trim().Equals(preProductCode) && "2".Equals(p.F39_Status));
            if (result.Count() > 0)
                return true;
            return false;
        }

        #endregion


        public TM03_PreProduct GetPreProduct(string id)
        {
            var preProductItem =
                _unitOfWork.PreProductRepository.GetMany(i => i.F03_PreProductCode == id && i.F03_PreProductName != null).FirstOrDefault();
            return Mapper.Map<TM03_PreProduct>(preProductItem);
        }
    }
}
