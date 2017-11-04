using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Domain.Models.EnvironmentManagement;
using Newtonsoft.Json;

namespace KCSG.Domain.Domains.EnvironmentManagement
{
    public class CreepingAndRollSpeedDataEditDomain : EnvironmentBaseDomain, ICreepingAndRollSpeedDataEditDomain
    {
        public CreepingAndRollSpeedDataEditDomain(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        public ResponseResult<CreepingAndRollSpeedDurationItem> Search(DateTime environmentDate,
            Constants.RollMachine machine, Constants.EnvMode mode)
        {
            var item = new CreepingAndRollSpeedDurationItem();
            for (int i = 1; i <= 3; i++)
            {
                var id = 0;
                if (machine == Constants.RollMachine.Roll_MC_18 && i != 3)
                {
                    id = i;
                }
                else if (machine == Constants.RollMachine.Roll_MC_12 && i != 3)
                {
                    id = i + 2;
                }
                else if (machine == Constants.RollMachine.Roll_MC_18 && i == 3)
                {
                    id = 5;
                }
                else
                {
                    id = 6;
                }
                var res = GetMespVal(Constants.EnvType.TYPE_RS, id, mode);
                if (!res.Item1)
                {
                    return new ResponseResult<CreepingAndRollSpeedDurationItem>(null, false);
                }
                var b_TEMPUSL_NULL = false;
                var b_HUMIDUSL_NULL = false;
                if (res.Item3.F80_T_Usl == null)
                {
                    res.Item3.F80_T_Usl = res.Item3.F80_T_Ucl;
                    b_TEMPUSL_NULL = true;
                }

                if (res.Item3.F80_H_Usl == null)
                {
                    res.Item3.F80_H_Usl = res.Item3.F80_H_Ucl;
                    b_HUMIDUSL_NULL = true;
                }

                var te80EnvMesp = res.Item3;
                var lstTable = new List<Graphtbl>();
                environmentDate = environmentDate.AddHours(8);
                //do
                //{
                //    var result = CalcData(Constants.EnvType.TYPE_RS, id, Constants.TypeOfTable.CALC_TE83, termDate,
                //        Constants.TypeOfTable.CALC_TE83, 1, mode, te80EnvMesp.F80_T_Ucl ?? 0, te80EnvMesp.F80_T_Lcl ?? 0,
                //        te80EnvMesp.F80_T_Usl ?? 0, te80EnvMesp.F80_T_Lsl ?? 0, te80EnvMesp.F80_T_Dis_Up ?? 0, te80EnvMesp.F80_H_Dis_Up ?? 0,
                //        te80EnvMesp.F80_T_Dis_Lo ?? 0, te80EnvMesp.F80_H_Dis_Lo ?? 0, te80EnvMesp.F80_H_Ucl ?? 0, te80EnvMesp.F80_H_Lcl ?? 0,
                //        te80EnvMesp.F80_H_Usl ?? 0, te80EnvMesp.F80_H_Lsl ?? 0, te80EnvMesp.F80_T_Cal_Up ?? 0, te80EnvMesp.F80_T_Cal_Lo ?? 0,
                //        te80EnvMesp.F80_H_Cal_Up ?? 0, te80EnvMesp.F80_H_Cal_Lo ?? 0, true, lstTable);
                //    if (!result)
                //    {
                //        break;
                //    }
                //    termDate = termDate.AddDays(1);
                //} while (termDate <= dt2);
                //termDate = dt2.AddDays(1);
                var resultEndDate = CalcData(Constants.EnvType.TYPE_RS, id, Constants.TypeOfTable.CALC_TE83,
                    environmentDate,
                    Constants.TypeOfTable.CALC_TE83, 1, mode, te80EnvMesp.F80_T_Ucl ?? 0, te80EnvMesp.F80_T_Lcl ?? 0,
                    te80EnvMesp.F80_T_Usl ?? 0, te80EnvMesp.F80_T_Lsl ?? 0, te80EnvMesp.F80_T_Dis_Up ?? 0,
                    te80EnvMesp.F80_H_Dis_Up ?? 0,
                    te80EnvMesp.F80_T_Dis_Lo ?? 0, te80EnvMesp.F80_H_Dis_Lo ?? 0, te80EnvMesp.F80_H_Ucl ?? 0,
                    te80EnvMesp.F80_H_Lcl ?? 0,
                    te80EnvMesp.F80_H_Usl ?? 0, te80EnvMesp.F80_H_Lsl ?? 0, te80EnvMesp.F80_T_Cal_Up ?? 0,
                    te80EnvMesp.F80_T_Cal_Lo ?? 0,
                    te80EnvMesp.F80_H_Cal_Up ?? 0, te80EnvMesp.F80_H_Cal_Lo ?? 0, true, lstTable);
                if (!resultEndDate)
                {
                    continue;
                }
                var endDate = environmentDate.AddDays(1);
                var mean = CalcMean(Constants.TypeOfTable.CALC_TE83, Constants.TypeOfTable.CALC_TE83,
                    id + (int) Constants.TypeOfTable.CALC_TE83, environmentDate, endDate, te80EnvMesp.F80_T_Cal_Up ?? 0,
                    te80EnvMesp.F80_T_Cal_Lo ?? 0, te80EnvMesp.F80_H_Cal_Up ?? 0, te80EnvMesp.F80_H_Cal_Lo ?? 0,
                    lstTable);
                if (mean.Item1)
                {
                    UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB, Constants.TypeOfTable.CALC_TE83
                        , environmentDate, mean.Item2,
                        Constants.EnvFieldNo.FLD_MEAN, lstTable);
                }
                else
                {
                    return new ResponseResult<CreepingAndRollSpeedDurationItem>(null, false);
                }
                var sigma = CalcSigma(Constants.TypeOfTable.CALC_BUFFER, Constants.TypeOfTable.CALC_BUFFER,
                    (int) Constants.TypeOfTable.CALC_TE83, environmentDate, endDate, te80EnvMesp.F80_T_Cal_Up ?? 0,
                    te80EnvMesp.F80_T_Cal_Lo ?? 0, te80EnvMesp.F80_H_Cal_Up ?? 0, te80EnvMesp.F80_H_Cal_Lo ?? 0,
                    lstTable);
                var max = GetMax(Constants.EnvType.TYPE_RS, Constants.TypeOfTable.CALC_TE83,
                    Constants.TypeOfTable.CALC_TE83, id, environmentDate, endDate, te80EnvMesp.F80_T_Dis_Up ?? 0,
                    te80EnvMesp.F80_H_Dis_Up ?? 0, lstTable);
                var min = GetMin(Constants.EnvType.TYPE_RS, Constants.TypeOfTable.CALC_TE83,
                    Constants.TypeOfTable.CALC_TE83, id, environmentDate, endDate, te80EnvMesp.F80_T_Dis_Lo ?? 0,
                    te80EnvMesp.F80_H_Dis_Lo ?? 0, lstTable);
                double? rl_upper;
                double? rl_lower;
                if (mode == Constants.EnvMode.ControlLine)
                {
                    rl_upper = te80EnvMesp.F80_T_Ucl;
                    rl_lower = te80EnvMesp.F80_T_Lcl;
                }
                else
                {
                    rl_upper = te80EnvMesp.F80_T_Usl;
                    rl_lower = te80EnvMesp.F80_T_Lsl;
                }
                UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB, Constants.TypeOfTable.CALC_TE83
                    , environmentDate, rl_upper ?? 0,
                    Constants.EnvFieldNo.FLD_MEAN, lstTable);
                UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB, Constants.TypeOfTable.CALC_TE83
                    , environmentDate, rl_lower ?? 0,
                    Constants.EnvFieldNo.FLD_MEAN, lstTable);
                double r_cp = 0;
                double r_cpk = 0;

                CalcCp(Constants.TypeOfTable.CALC_TETMP_RLSPD, mean.Item2, sigma.Item2, max - min,
                    te80EnvMesp.F80_T_Usl ?? 0,
                    te80EnvMesp.F80_T_Lsl ?? 0, ref r_cp, ref r_cpk, b_TEMPUSL_NULL, b_HUMIDUSL_NULL, te80EnvMesp.F80_T_Lcl ?? 0,
                    te80EnvMesp.F80_T_Ucl ?? 0,
                    te80EnvMesp.F80_H_Lcl ?? 0, te80EnvMesp.F80_H_Ucl ?? 0);
                var dataChart = GrapData(Constants.TypeOfTable.CALC_TE83, lstTable);
                switch (id)
                {
                    case 1:
                    case 3:
                        item.LeftModel = new ChartModel()
                        {
                            ChartName = machine.ToString() + ": Left Creeping",
                            Cp = r_cp.ToString("f"),
                            Cpk = r_cpk.ToString("F"),
                            Data1 = dataChart.Item1,
                            Data2 = dataChart.Item2,
                            Data3 = dataChart.Item3,
                            Data4 = dataChart.Item4,
                            lstTime = dataChart.Item5,
                            High = max.ToString("f"),
                            Low = min.ToString("f"),
                            Range = (max - min).ToString("f"),
                            LCL = rl_lower.Value.ToString("f"),
                            Mean = mean.Item2.ToString("f"),
                            Sigma = sigma.Item2.ToString("f"),
                            UCL = rl_upper.Value.ToString("f")
                        };

                        break;
                    case 2:
                    case 4:
                        item.RightModel = new ChartModel()
                        {
                            ChartName = machine.ToString() + ": Right Creeping",
                            Cp = r_cp.ToString("f"),
                            Cpk = r_cpk.ToString("F"),
                            Data1 = dataChart.Item1,
                            Data2 = dataChart.Item2,
                            Data3 = dataChart.Item3,
                            Data4 = dataChart.Item4,
                            lstTime = dataChart.Item5,
                            High = max.ToString("f"),
                            Low = min.ToString("f"),
                            Range = (max - min).ToString("f"),
                            LCL = rl_lower.Value.ToString("f"),
                            Mean = mean.Item2.ToString("f"),
                            Sigma = sigma.Item2.ToString("f"),
                            UCL = rl_upper.Value.ToString("f")
                        };
                        break;
                    case 5:
                    case 6:
                        item.RightModel = new ChartModel()
                        {
                            ChartName = machine.ToString() + ": Roll Speed",
                            Cp = r_cp.ToString("f"),
                            Cpk = r_cpk.ToString("F"),
                            Data1 = dataChart.Item1,
                            Data2 = dataChart.Item2,
                            Data3 = dataChart.Item3,
                            Data4 = dataChart.Item4,
                            lstTime = dataChart.Item5,
                            High = max.ToString("f"),
                            Low = min.ToString("f"),
                            Range = (max - min).ToString("f"),
                            LCL = rl_lower.Value.ToString("f"),
                            Mean = mean.Item2.ToString("f"),
                            Sigma = sigma.Item2.ToString("f"),
                            UCL = rl_upper.Value.ToString("f")
                        };
                        break;
                }
            }
            return new ResponseResult<CreepingAndRollSpeedDurationItem>(item, true);
        }


        public void Edit(int id1, int id2, int id3, string environmentDate, string time, double leftCreeping,
            double rightCreeping, double rollSpeed)
        {
            var newTime = DateTime.Parse(time,
                System.Globalization.CultureInfo.CurrentCulture);

            var timeEnd = DateTime.Parse("08:00",
                System.Globalization.CultureInfo.CurrentCulture);
            var timeStart = DateTime.Parse("00:00",
                System.Globalization.CultureInfo.CurrentCulture);

            var stringF83EnvTime = environmentDate + ' ' + newTime.ToString("hh:mm tt");
            var f83EnvTime = ConvertHelper.ConvertToDateTimeFullHourMinute(stringF83EnvTime);
            if (newTime >= timeStart && newTime < timeEnd)
            {
                f83EnvTime = f83EnvTime.AddDays(1);
            }

            var envTimeStart = f83EnvTime.AddSeconds(0);
            var envTimeEnd = f83EnvTime.AddSeconds(60);

            //update F83_value equal leftCreeping
            var te83EnvEls1 =
                _unitOfWork.EnvElseRepository.GetMany(
                    i => i.F83_Env_Time >= envTimeStart && i.F83_Env_Time < envTimeEnd && i.F83_Id.Equals(id1));
            if (te83EnvEls1.Any())
            {
                var item1 = te83EnvEls1.FirstOrDefault();
                item1.F83_Value = leftCreeping;
                _unitOfWork.EnvElseRepository.Update(item1);
            }


            //update f83_value equal rightCreeping
            var te83EnvEls2 =
                _unitOfWork.EnvElseRepository.GetMany(
                    i => i.F83_Env_Time >= envTimeStart && i.F83_Env_Time < envTimeEnd && i.F83_Id.Equals(id2));

            if (te83EnvEls2.Any())
            {
                var item2 = te83EnvEls2.FirstOrDefault();
                item2.F83_Value = rightCreeping;
                _unitOfWork.EnvElseRepository.Update(item2);
            }

            //update f83_value equal roll speed
            var te83EnvEls3 =
                _unitOfWork.EnvElseRepository.GetMany(
                    i => i.F83_Env_Time >= envTimeStart && i.F83_Env_Time < envTimeEnd && i.F83_Id.Equals(id3));
            if (te83EnvEls3.Any())
            {
                var item3 = te83EnvEls3.FirstOrDefault();
                item3.F83_Value = rollSpeed;
                _unitOfWork.EnvElseRepository.Update(item3);
            }

            _unitOfWork.Commit();
        }


        public Tuple<Te83_Env_Else, Te83_Env_Else, Te83_Env_Else> ChangeValueOfTime(string environmentDate, string time,
            int id1, int id2, int id3)
        {
            var newTime = DateTime.Parse(time,
                System.Globalization.CultureInfo.CurrentCulture);

            var timeEnd = DateTime.Parse("08:00",
                System.Globalization.CultureInfo.CurrentCulture);
            var timeStart = DateTime.Parse("00:00",
                System.Globalization.CultureInfo.CurrentCulture);

            var stringF83EnvTime = environmentDate + ' ' + newTime.ToString("hh:mm tt");
            var f83EnvTime = ConvertHelper.ConvertToDateTimeFullHourMinute(stringF83EnvTime);
            if (newTime >= timeStart && newTime < timeEnd)
            {
                f83EnvTime = f83EnvTime.AddDays(1);
            }

            var envTimeStart = f83EnvTime.AddSeconds(0);
            var envTimeEnd = f83EnvTime.AddSeconds(60);

            var te83EnvElse =
                _unitOfWork.EnvElseRepository.GetMany(
                    i =>
                        i.F83_Env_Time >= envTimeStart && i.F83_Env_Time < envTimeEnd &&
                        (i.F83_Id.Equals(id1) || i.F83_Id.Equals(id2) || i.F83_Id.Equals(id3)));
            if (!te83EnvElse.Any())
            {
                return new Tuple<Te83_Env_Else, Te83_Env_Else, Te83_Env_Else>(null, null, null);
            }

            //get Te83_Env_Else record with id1
            var getTe83EnvElse1 =
                _unitOfWork.EnvElseRepository.GetMany(
                    i => i.F83_Env_Time >= envTimeStart && i.F83_Env_Time < envTimeEnd && i.F83_Id.Equals(id1))
                    .FirstOrDefault();
            //get Te83_Env_Else record with id2
            var getTe83EnvElse2 =
                _unitOfWork.EnvElseRepository.GetMany(
                    i => i.F83_Env_Time >= envTimeStart && i.F83_Env_Time < envTimeEnd && i.F83_Id.Equals(id2))
                    .FirstOrDefault();
            //get Te83_Env_Else record with id3
            var getTe83EnvElse3 =
                _unitOfWork.EnvElseRepository.GetMany(
                    i => i.F83_Env_Time >= envTimeStart && i.F83_Env_Time < envTimeEnd && i.F83_Id.Equals(id3))
                    .FirstOrDefault();

            return new Tuple<Te83_Env_Else, Te83_Env_Else, Te83_Env_Else>(getTe83EnvElse1, getTe83EnvElse2,
                getTe83EnvElse3);
        }
    }
}