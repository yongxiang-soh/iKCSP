using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
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

    public class PdtPlnDomain :BaseDomain, IPdtPlnDomain
    {
       

        #region Constructor

        public PdtPlnDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            
        }

        #endregion

        #region Methods
        public PdtPlnItem GetById(DateTime date, string preProdCode)
        {
            var startDate = date.AddDays(-1);
            var endDate = date.AddDays(1);
            var entity = _unitOfWork.PdtPlnRepository.GetMany(m => m.F39_PreProductCode.Trim().Equals(preProdCode.Trim()) && m.F39_KndEptBgnDate>startDate&&m.F39_KndEptBgnDate<endDate).FirstOrDefault();
            var tm03Entity = _unitOfWork.PreProductRepository.GetById(preProdCode.Trim());
            var result = Mapper.Map<PdtPlnItem>(entity);
            result.PreProduct = tm03Entity;
            result.F39_PreProductName = tm03Entity != null ? tm03Entity.F03_PreProductName : "";
            return result;
        }

        public void Create(PdtPlnItem pdtPln)
        {
            var entity = Mapper.Map<TX39_PdtPln>(pdtPln);
            _unitOfWork.PdtPlnRepository.Add(entity);
            _unitOfWork.Commit();
        }

        public void Update(PdtPlnItem pdtPln)
        {
            var entity = Mapper.Map<TX39_PdtPln>(pdtPln);
            _unitOfWork.PdtPlnRepository.Update(entity);
            _unitOfWork.Commit();
        }

        //phan nay can them dieu kien filter ngay thang
        public bool Delete(DateTime date, string code)
        {
           // var statusYet = Constants.Status.Yet.ToString("D");
            var pdtPlnItem =
                _unitOfWork.PdtPlnRepository.GetMany(
                    s =>
                        s.F39_PreProductCode.Trim().Equals(code.Trim()) && s.F39_KndEptBgnDate.Equals(date)).FirstOrDefault();
            if (pdtPlnItem != null)
            {
                _unitOfWork.PdtPlnRepository.Delete(pdtPlnItem);
                _unitOfWork.Commit();
                return true;
            }
            else
            {
                return false;
            }
            

        //_unitOfWork.PdtPlnRepository.Delete(s => s.F39_PreProductCode.Trim().Equals(code.Trim()) && s.F39_KndEptBgnDate.Equals(date) && s.F39_Status != statusYet);
            
            
        //    _unitOfWork.Commit();
        }
        public ResponseResult<GridResponse<PdtPlnItem>> SearchCriteria(string date,Enum line, GridSettings gridSettings)
        {
            string kndLine;
            var result = _unitOfWork.PdtPlnRepository.GetAll();
            if (!string.IsNullOrEmpty(date))
            {
                date = "01/" + date;
                var mydate = ConvertHelper.ConvertToDateTimeFull(date);
                var endDate = mydate.AddMonths(1);
                result = result.Where(i => i.F39_KndEptBgnDate >= mydate && i.F39_KndEptBgnDate < endDate);
            }
            
            if (line!=null)
            {
                kndLine = Constants.KndLine.Megabit.Equals(line) ? "0" : "1";
                result = result.Where(i =>i.F39_KneadingLine.Equals(kndLine)); 
            }
            var itemCount = result.Count();
            OrderByAndPaging(ref result, gridSettings);
            //result = result.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize);
            var lstResult = Mapper.Map<IEnumerable<TX39_PdtPln>, IEnumerable<PdtPlnItem>>(result.ToList());
            var resultModel = new GridResponse<PdtPlnItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<PdtPlnItem>>(resultModel, true);
        }

        public bool CheckUnique3Reco(DateTime dateTime, string kndLine)
        {
            return
                _unitOfWork.PdtPlnRepository.GetAll()
                    .Count(i => i.F39_KndEptBgnDate == dateTime && i.F39_KneadingLine == kndLine) >=5;

        }

        public bool CheckUnique(string preProdCode, DateTime prodDate)
        {
            return
                _unitOfWork.PdtPlnRepository.GetAll()
                    .Any(
                        m =>
                            m.F39_PreProductCode.Equals(preProdCode.Trim()) &&
                            m.F39_KndEptBgnDate.Year == prodDate.Year
                            &&
                            m.F39_KndEptBgnDate.Month == prodDate.Month
                            &&
                            m.F39_KndEptBgnDate.Day == prodDate.Day);
        }

        public ResponseResult CreateOrUpdate(PdtPlnItem model)
        {
            
            if (model.IsCreate)
            {
                if (!string.IsNullOrEmpty(model.F39_PreProductCode))
                {
                    if (CheckUnique(model.F39_PreProductCode, model.F39_KndEptBgnDate))
                    {
                        return new ResponseResult(false, Constants.Messages.Material_MSG004);
                    }
                }
                
                var entity = Mapper.Map<TX39_PdtPln>(model);
                entity.F39_KndCmdNo = " ";
                entity.F39_KneadingLine = model.KndLine == (int) Constants.KndLine.Megabit
                    ? Constants.KndLine.Megabit.ToString("D")
                    : Constants.KndLine.Conventional.ToString("D");
                entity.F39_StartLotNo = "";
                entity.F39_EndLotAmont = 0;
                entity.F39_ColorClass = GetKndLine(entity.F39_PreProductCode);
                entity.F39_AddDate = DateTime.Now;
                entity.F39_UpdateDate = DateTime.Now;
                entity.F39_UpdateCount = 0;
                entity.F39_Status = Constants.F39_Status.NotCommanded.ToString("D");
                _unitOfWork.PdtPlnRepository.Add(entity);
            }
            else
            {
                //var entity = _unitOfWork.PdtPlnRepository.GetById(model.F39_PreProductCode);
                var entity = _unitOfWork.PdtPlnRepository.Get(m => m.F39_PreProductCode.Trim().Equals(model.F39_PreProductCode.Trim()) && m.F39_KndEptBgnDate.Equals(model.F39_KndEptBgnDate));
                if (entity == null)
                {
                    return new ResponseResult(false, Constants.Messages.Material_MSG001);
                }

                Constants.F39_Status status;
                if (!Enum.TryParse(model.F39_Status, true, out status))
                    status = Constants.F39_Status.NotCommanded;

                entity.F39_Status = status.ToString("D");
                entity.F39_PrePdtLotAmt = model.F39_PrePdtLotAmt;
                entity.F39_UpdateCount = entity.F39_UpdateCount + 1;
                _unitOfWork.PdtPlnRepository.Update(entity);
            }
            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        private string GetKndLine(string preProductCode)
        {
            var preProduct = _unitOfWork.PreProductRepository.GetById(preProductCode.Trim());
            if (preProduct.F03_KneadingLine == Constants.F39_KneadingLine.Megabit.ToString("D")||preProduct.F03_KneadingLine == Constants.F39_KneadingLine.ConventionalB.ToString("D"))
            {
                return "0";
            }
            else
            {
               return "1";
            }
            
        }
        #endregion
    }
}
