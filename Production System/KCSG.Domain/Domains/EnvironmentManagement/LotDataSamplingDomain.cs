using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.EnvironmentManagement
{
    public class LotDataSamplingDomain:EnvironmentBaseDomain,ILotDataSamplingDomain
    {

        public LotDataSamplingDomain(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            
        }
        public SelectList GetProduct()
        {
            return new SelectList(_unitOfWork.ProductRepository.GetAll(), "F09_ProductCode", "F09_ProductDesp");
        }

        public SelectList GetLotNo(string productCode, bool newCheckLot)
        {
            var lstLotNoNotUsed =
                _unitOfWork.EnvLotRepository.GetMany(i => i.F84_ProductCode.Trim().Equals(productCode.Trim())).Select(i=>i.F84_ProductLotNo);
            if (newCheckLot)
            {
                var lstLotNoUsed =
               _unitOfWork.ProductStorageRetrieveHistoryRepository.GetMany(
                   i => i.F65_ProductCode.Trim().Equals(productCode.Trim()))
                   .Where(i => !lstLotNoNotUsed.Contains(i.F65_ProductLotNo));


                return new SelectList(lstLotNoUsed, "F65_ProductLotNo", "F65_ProductLotNo");
            }

            return new SelectList(lstLotNoNotUsed);
        }

        public ResponseResult<LotdataSamplingItem> Search(string productCode, DateTime date, Constants.EnvMode mode, string lotNo)
        {
            var lstTable = new List<Graphtbl>();
            var lstTe85 = _unitOfWork.EnvProdRepository.GetMany(i => i.F85_Code.Trim().Equals(productCode.Trim())).Select(i=>i.F85_Id);
            var te80 = _unitOfWork.EnvMespRepository.GetMany(i => lstTe85.Contains(i.F80_Id)&&i.F80_Type == "1").FirstOrDefault();
            if (te80==null)
            {
                return new ResponseResult<LotdataSamplingItem>(null,false);
            }
            var dt1 = date.AddHours(8);
            var dt2 = date.AddDays(1).AddHours(8);
            var res = GetMespVal(Constants.EnvType.TYPE_RM, te80.F80_Id, mode);
            if (!res.Item1)
            {
                return new ResponseResult<LotdataSamplingItem>(null, false, res.Item2);
             }
            te80 = res.Item3;
            if (!CalcData(Constants.EnvType.TYPE_RM, te80.F80_Id,null, date, Constants.TypeOfTable.CALC_TE81_TEMP, 1, mode,
                 te80.F80_T_Ucl ?? 0, te80.F80_T_Lcl ?? 0, te80.F80_T_Usl ?? 0,
                te80.F80_T_Lsl ?? 0,
                te80.F80_T_Dis_Up ?? 0, te80.F80_H_Dis_Up ?? 0, te80.F80_T_Dis_Lo ?? 0, te80.F80_H_Dis_Lo ?? 0,
                te80.F80_H_Ucl ?? 0,
                te80.F80_H_Lcl ?? 0, te80.F80_H_Usl ?? 0, te80.F80_H_Lsl ?? 0, te80.F80_T_Cal_Up ?? 0,
                te80.F80_T_Cal_Lo ?? 0, te80.F80_H_Cal_Up ?? 0,
                te80.F80_H_Cal_Lo ?? 0, false, lstTable))
            {
                return new ResponseResult<LotdataSamplingItem>(null, false, res.Item2);
            }
          var  result = CalcMean(Constants.TypeOfTable.CALC_BUFFER, Constants.TypeOfTable.CALC_TE81_TEMP,
                (int) Constants.TypeOfTable.CALC_TE81_TEMP, dt1, dt2, te80.F80_T_Cal_Up ?? 0, te80.F80_T_Cal_Lo ?? 0,
                te80.F80_H_Cal_Up ?? 0, te80.F80_H_Cal_Lo ?? 0, lstTable);
            if (result.Item1)
            {
                UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB, Constants.TypeOfTable.CALC_TE81_TEMP, dt1,
                    result.Item2, Constants.EnvFieldNo.FLD_MEAN, lstTable);
            }
            else
            {
                return new ResponseResult<LotdataSamplingItem>(null, false); 
            }
           var sigma =  CalcSigma(Constants.TypeOfTable.CALC_BUFFER, Constants.TypeOfTable.CALC_TE81_TEMP,
                (int) Constants.TypeOfTable.CALC_TE81_TEMP, dt1, dt2, te80.F80_T_Cal_Up ?? 0, te80.F80_T_Cal_Lo ?? 0,
                te80.F80_H_Cal_Up??0, te80.F80_H_Cal_Lo??0, lstTable).Item2;
            var max = GetMax(Constants.EnvType.TYPE_RM, Constants.TypeOfTable.CALC_BUFFER,
                Constants.TypeOfTable.CALC_TE81_TEMP, (int) Constants.TypeOfTable.CALC_TE81_TEMP, dt1, dt2,
                te80.F80_T_Dis_Up ?? 0, te80.F80_H_Dis_Up ?? 0, lstTable);
            var min = GetMin(Constants.EnvType.TYPE_RM, Constants.TypeOfTable.CALC_BUFFER,
                Constants.TypeOfTable.CALC_TE81_TEMP, (int) Constants.TypeOfTable.CALC_TE81_TEMP, dt1, dt2,
                te80.F80_T_Dis_Lo??0, te80.F80_H_Dis_Lo??0, lstTable);
            if (mode == Constants.EnvMode.ControlLine)
            {
                UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB, Constants.TypeOfTable.CALC_TE81_TEMP, dt1,
                    te80.F80_T_Ucl??0, Constants.EnvFieldNo.FLD_MEAN, lstTable);
                UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB, Constants.TypeOfTable.CALC_TE81_TEMP, dt1,
                  te80.F80_T_Lcl??0, Constants.EnvFieldNo.FLD_MEAN, lstTable);
            }
            else
            {
                UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB, Constants.TypeOfTable.CALC_TE81_TEMP, dt1,
                  te80.F80_T_Usl??0, Constants.EnvFieldNo.FLD_MEAN, lstTable); 
                UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB, Constants.TypeOfTable.CALC_TE81_TEMP, dt1,
                   te80.F80_T_Lsl ?? 0, Constants.EnvFieldNo.FLD_MEAN, lstTable);
            }
           var dataChart =  GrapData(Constants.TypeOfTable.CALC_TE81_TEMP, lstTable);
            var item = new LotdataSamplingItem
            {
                ChartName = te80.F80_Name,
                HighTemp = Math.Round(max,2),
                LowTemp = Math.Round(min,2),
                RangeTemp = Math.Round(max - min,2),
                SigmaTemp = Math.Round(sigma,2),
                MeanTemp = Math.Round(result.Item2,2),
                Dec_data = dataChart.Item1,
                Dec_lower = dataChart.Item2,
                Dec_mean = dataChart.Item3,
                Dec_upper = dataChart.Item4,
                dt_dtm = dataChart.Item5,
            };
            return new ResponseResult<LotdataSamplingItem>(item,true);
        }

        public ResponseResult Add(string productCode, string lotNo, DateTime date, TimeSpan time,double? temp)
        {
            var countte84 =
                _unitOfWork.EnvLotRepository.GetMany(i => i.F84_ProductCode.Trim().Equals(productCode.Trim())).Select(i=>i.F84_ProductLotNo).Distinct().Count();
            if (countte84 >= 200)
            {
                return new ResponseResult(false);
            }

            var startDate = date.AddHours(8);
            var endDate = date.AddDays(1).AddHours(8);

            if (time >= new TimeSpan(0, 0, 0) && time < new TimeSpan(8, 0, 0))
            {
                date = date.AddDays(1);
            }
            var newDateTime = date.Add(time);

            var te85 = _unitOfWork.EnvProdRepository.Get(i => i.F85_Code.Trim().Equals(productCode.Trim()));
            //var dec_temp = _unitOfWork.EnvTempRepository.Get(i => i.F81_Id == te85.F85_Id&& i.F81_Env_Time == date).F81_Temp;
            var lstTe84 = _unitOfWork.EnvLotRepository.GetMany(i => i.F84_ProductCode.Trim().Equals(productCode.Trim()));
            var nLot =lstTe84.Select(i => i.F84_ProductLotNo)
                    .Distinct()
                    .Count();
            if (nLot >= 200 ){
                return  new ResponseResult(false,"Number of lots has exceeded 200 records!");
           }
            var te84EnvLots =
                _unitOfWork.EnvLotRepository.GetMany(
                    i =>
                        i.F84_ProductCode.Trim().Equals(productCode.Trim()) && i.F84_ProductLotNo.Trim().Equals(lotNo) &&
                        i.F84_S_Time >= startDate && i.F84_S_Time <= endDate);
            if(te84EnvLots.Count()>=10)
                return new ResponseResult(false, "Number of lot sampling has exceeded 10 records!");
            var te84 = new Te84_Env_Lot()
            {
                F84_ProductCode = productCode,
                F84_ProductLotNo = lotNo,
                F84_S_Time = newDateTime,
                F84_Temp = temp
            };
            
            _unitOfWork.EnvLotRepository.Add(te84);
            _unitOfWork.Commit();

            var newLotNo = lstTe84.Select(i => i.F84_ProductLotNo)
                    .Distinct()
                    .Count();
            te85.F85_No_Lot = newLotNo;
            _unitOfWork.EnvProdRepository.Update(te85);
            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        public bool Delete(int id)
        {
            var te84 = _unitOfWork.EnvLotRepository.Get(i => i.F84_Id == id);
            _unitOfWork.EnvLotRepository.Delete(te84);

            _unitOfWork.Commit();
            var countLotno = _unitOfWork.EnvLotRepository.GetMany(i => i.F84_ProductCode == te84.F84_ProductCode).Select(i=>i.F84_ProductLotNo).Distinct().Count();
            var te85 = _unitOfWork.EnvProdRepository.Get(i => i.F85_Code==te84.F84_ProductCode);
            te85.F85_No_Lot = countLotno;
            _unitOfWork.EnvProdRepository.Update(te85);
            _unitOfWork.Commit();
            return true;
        }

        public ResponseResult<GridResponse<Te84_Env_Lot>> GetTable(string productCode,GridSettings settings)
        {
           var result = _unitOfWork.EnvLotRepository.GetMany(i => i.F84_ProductCode.Trim().Equals(productCode.Trim()));
            var itemCount = result.Count();
            result = result.OrderBy(i => i.F84_Id);
            result = result.Skip((settings.PageIndex - 1) * settings.PageSize).Take(settings.PageSize);
            var resultModel = new GridResponse<Te84_Env_Lot>(result, itemCount);
            return new ResponseResult<GridResponse<Te84_Env_Lot>>(resultModel, true);
            
        }

        public string GetDate(string lotNo)
        {
            var th65 =
                _unitOfWork.ProductStorageRetrieveHistoryRepository.GetMany(
                    i => i.F65_ProductLotNo.Trim().Equals(lotNo.Trim())).FirstOrDefault();
            return th65 != null ? th65.F65_StgRtrDate.ToString("dd/MM/yyyy") : "";
        }

        public ResponseResult GetValueWithTime(string time,string productCode,string date)
        {

            DateTime newTime = DateTime.Parse(time,
                System.Globalization.CultureInfo.CurrentCulture);
            DateTime timeEnd = DateTime.Parse("08:00",
                System.Globalization.CultureInfo.CurrentCulture);
            DateTime timeStart = DateTime.Parse("00:00",
                System.Globalization.CultureInfo.CurrentCulture);

            var stringF81EnvTime = date + ' ' + newTime.ToString("hh:mm tt");

            var f81EnvTime = ConvertHelper.ConvertToDateTimeFullHourMinute(stringF81EnvTime);

            if (newTime >= timeStart && newTime < timeEnd)
            {
                f81EnvTime = f81EnvTime.AddDays(1);
            }
            var envTimeToStart = f81EnvTime.AddSeconds(0);
            var envTimeToEnd = f81EnvTime.AddSeconds(60);


            var te85 = _unitOfWork.EnvProdRepository.Get(i => i.F85_Code.Trim().Equals(productCode.Trim()));
            var dec_temp = _unitOfWork.EnvTempRepository.Get(i => i.F81_Id == te85.F85_Id && i.F81_Env_Time >= envTimeToStart && i.F81_Env_Time < envTimeToEnd);
            if (dec_temp==null)
            {
                return new ResponseResult(false);
            }
            return new ResponseResult(true,dec_temp.F81_Temp.ToString());
           
        }

        public IList<Te84_Env_Lot> GetTe84EnvLot(string productCode,string productLotNo,string date)
        {
            var startDate = ConvertHelper.ConvertToDateTimeFull(date).AddHours(8);
            var endTime = startDate.AddDays(1);
            var te84EnvLots =
                _unitOfWork.EnvLotRepository.GetMany(
                    i =>
                        i.F84_ProductCode.Trim().Equals(productCode.Trim()) &&
                        i.F84_ProductLotNo.Trim().Equals(productLotNo.Trim()) && i.F84_S_Time >= startDate &&
                        i.F84_S_Time <= endTime);
            return te84EnvLots.ToList();
        }
    }
}
