using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;
using log4net;

namespace KCSG.Domain.Domains.ProductionPlanning
{
    public class PrePdtMkpDomain : IPrePdtMkpDomain
    {
        #region UnitOrWork Declaration

        private readonly IUnitOfWork _unitOfWork;

        private readonly ILog _log;

        #endregion

        #region Constructor

        public PrePdtMkpDomain(IUnitOfWork unitOfWork, ILog log)
        {
            _unitOfWork = unitOfWork;
            _log = log;
        }

        public PrePdtMkpDomain(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        #endregion

        #region Methods

        public PrePdtMkpItem GetById(string preProdCode, string matCode)
        {
            var entity = _unitOfWork.PrePdtMkpRepository.Get(m => (m.F02_PreProductCode.Equals(preProdCode)
                                                                   && m.F02_MaterialCode.Equals(matCode)
                                                                  ));
            return Mapper.Map<PrePdtMkpItem>(entity);
        }

        public ResponseResult<GridResponse<PrePdtMkpMatItem>> SearchByPreProductCode(string preProductCode, GridSettings gridSettings)
        {
            var prePdtMkp = _unitOfWork.PrePdtMkpRepository.GetMany(d => true);
            var material = _unitOfWork.MaterialRepository.GetMany(d => true);
            IEnumerable<PrePdtMkpMatItem> result = (from ppm in prePdtMkp
                    join m in material
                    on ppm.F02_MaterialCode equals m.F01_MaterialCode
                    where
                    String.IsNullOrEmpty(preProductCode) || ppm.F02_PreProductCode.Trim().Equals(preProductCode.Trim())
                    select new PrePdtMkpMatItem
                    {
                        F02_PreProductCode = ppm.F02_PreProductCode,
                        F02_MaterialCode = ppm.F02_MaterialCode,
                        F02_ThrawSeqNo = ppm.F02_ThrawSeqNo,
                        F02_3FLayinAmount = ppm.F02_3FLayinAmount,
                        F02_4FLayinAmount = ppm.F02_4FLayinAmount,
                        F02_LayinPriority = ppm.F02_LayinPriority,
                        F02_WeighingMethod = ppm.F02_WeighingMethod,
                        F02_MilingFlag1 = ppm.F02_MilingFlag1,
                        F02_MilingFlag2 = ppm.F02_MilingFlag2,
                        F02_LoadPosition = ppm.F02_LoadPosition,
                        F02_PotSeqNo = ppm.F02_PotSeqNo,
                        F02_MsrSeqNo = ppm.F02_MsrSeqNo,
                        F02_Addtive = ppm.F02_Addtive,
                        F02_AddDate = ppm.F02_AddDate,
                        F02_UpdateDate = ppm.F02_UpdateDate,
                        F02_UpdateCount = ppm.F02_UpdateCount,
                        F01_MaterialDsp = m.F01_MaterialDsp
                    }).Distinct()
                .OrderBy(x => x.F02_LayinPriority)
                .ThenBy(x => x.F02_ThrawSeqNo)
                .ThenBy(x => x.F02_PotSeqNo)
                .ThenBy(x => x.F02_MsrSeqNo);
            
            var itemCount = result.Count();
            var result1 = result.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize);
            var resultModel = new GridResponse<PrePdtMkpMatItem>(result1, itemCount);
            return new ResponseResult<GridResponse<PrePdtMkpMatItem>>(resultModel, true);
        }

        public void Create(TM02_PrePdtMkp prePdtMkp)
        {
            var entity = Mapper.Map<TM02_PrePdtMkp>(prePdtMkp);
            _unitOfWork.PrePdtMkpRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(TM02_PrePdtMkp prePdtMkp)
        {
            var entity = Mapper.Map<TM02_PrePdtMkp>(prePdtMkp);
            _unitOfWork.PrePdtMkpRepository.Update(entity);
            _unitOfWork.Commit();
        }

        public List<TM02_PrePdtMkp> GetAllByPreProduct(string preProdCode)
        {
            return _unitOfWork.PrePdtMkpRepository.GetMany(m => m.F02_PreProductCode.Equals(preProdCode)).ToList();
        }

        public void Delete(string preProductCode, string materialCode, string thrawSeqNo)
        {
            _unitOfWork.PrePdtMkpRepository.Delete(s => (s.F02_PreProductCode.Trim() == preProductCode
                && s.F02_MaterialCode.Trim() == materialCode
                && s.F02_ThrawSeqNo.Trim() == thrawSeqNo));
            _unitOfWork.Commit();
        }

        public void Delete(string preProductCode)
        {
           
                _unitOfWork.PrePdtMkpRepository.Delete(s => s.F02_PreProductCode.Trim() == preProductCode.Trim()&& (s.F02_IsDraft == true || s.F02_IsDraft==null));
                _unitOfWork.Commit();
           
           
        }
        public bool CheckUnique(string preProdCode, string matCode)
        {
            return _unitOfWork.PrePdtMkpRepository.GetAll().Any(m => (m.F02_PreProductCode.Trim().Equals(preProdCode.Trim())
                                                                      && (m.F02_MaterialCode.Trim().Equals(matCode.Trim())
                                                                       )));
        }

        public ResponseResult CreateOrUpdate(PrePdtMkpItem model)
        {
            try
            {
                if (model.IsCreate)
                {
                    var entity = Mapper.Map<TM02_PrePdtMkp>(model);
                    entity.F02_AddDate = DateTime.Now;
                    entity.F02_UpdateDate = DateTime.Now;
                    entity.F02_IsDraft = true;
                    _unitOfWork.PrePdtMkpRepository.Add(entity);
                }
                else
                {

                   var entity = _unitOfWork.PrePdtMkpRepository.GetAll().FirstOrDefault(
                            m => m.F02_PreProductCode == model.F02_PreProductCode
                                 && m.F02_MaterialCode == model.F02_MaterialCode);

                        if (entity == null)
                        {
                            return new ResponseResult(false, Constants.Messages.Material_MSG001);
                        }
                        var addDate = entity.F02_AddDate;
                        Mapper.Map(model, entity);
                        entity.F02_AddDate = addDate;
                        entity.F02_ThrawSeqNo = entity.F02_ThrawSeqNo.PadRight(2);
                        entity.F02_UpdateDate = DateTime.Now;
                        entity.F02_UpdateCount += 1;
                        _unitOfWork.PrePdtMkpRepository.Update(entity);
                   

                }
                _unitOfWork.Commit();
                return new ResponseResult(true);
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return new ResponseResult(false);
            }
        }

        public bool PotSeqNo(string f02PreProductCode, string f02_potseqno)
        {
            return
                _unitOfWork.PrePdtMkpRepository.GetAll()
                    .Any(
                        i =>
                            i.F02_PreProductCode.Trim().Equals(f02PreProductCode.Trim()) &&
                            i.F02_PotSeqNo.Trim().Equals(f02_potseqno.Trim()));
        }

        public int CountByPreproductCode(string preProdCode)
        {
            return
                _unitOfWork.PrePdtMkpRepository.GetMany(i => i.F02_PreProductCode.Trim().Equals(preProdCode.Trim()))
                    .Count();
        }

        #endregion
    }
}
