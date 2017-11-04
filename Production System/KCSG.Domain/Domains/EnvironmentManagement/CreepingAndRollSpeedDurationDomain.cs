using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Html;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Domain.Models.EnvironmentManagement;

namespace KCSG.Domain.Domains.EnvironmentManagement
{
    public class CreepingAndRollSpeedDurationDomain:EnvironmentBaseDomain,ICreepingAndRollSpeedDurationDomain
    {
        public CreepingAndRollSpeedDurationDomain(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public ResponseResult<CreepingAndRollSpeedDurationItem> Search(DateTime startDate, DateTime endDate,
            Constants.RollMachine machine, Constants.EnvMode mode)
        {
            var nlbl = 0;
            if ((endDate - startDate).TotalDays < 10)
                nlbl = 0;
            else if ((endDate - startDate).TotalDays < 20)
            {
                nlbl = 4;

            }
            else
            {
                nlbl = (endDate - startDate).TotalDays < 50 ? 8 : 10;
            }
            var item = new CreepingAndRollSpeedDurationItem();
            for (int i = 1; i <= 3; i++)
            {
                var id = 0;
                if (machine == Constants.RollMachine.Roll_MC_18&&i!=3   )
                {
                    id = i;
                }
                else if (machine == Constants.RollMachine.Roll_MC_12  && i!=3   )
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
                    return new ResponseResult<CreepingAndRollSpeedDurationItem>(null,false);
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
                var termDate = startDate;
                do
                {
                    var result = CalcData(Constants.EnvType.TYPE_RS, id, null, termDate,
                        Constants.TypeOfTable.CALC_TE83_90, 1, mode, te80EnvMesp.F80_T_Ucl??0, te80EnvMesp.F80_T_Lcl??0,
                        te80EnvMesp.F80_T_Usl??0, te80EnvMesp.F80_T_Lsl??0, te80EnvMesp.F80_T_Dis_Up??0, te80EnvMesp.F80_H_Dis_Up??0,
                        te80EnvMesp.F80_T_Dis_Lo??0, te80EnvMesp.F80_H_Dis_Lo??0, te80EnvMesp.F80_H_Ucl??0, te80EnvMesp.F80_H_Lcl??0,
                        te80EnvMesp.F80_H_Usl??0, te80EnvMesp.F80_H_Lsl??0, te80EnvMesp.F80_T_Cal_Up??0, te80EnvMesp.F80_T_Cal_Lo??0,
                        te80EnvMesp.F80_H_Cal_Up??0, te80EnvMesp.F80_H_Cal_Lo??0, true, lstTable);
                    if (!result)
                    {
                        break;
                    }
                    termDate = termDate.AddDays(1);
                } while (termDate <= endDate);
                termDate = endDate.AddDays(1);
                var resultEndDate = CalcData(Constants.EnvType.TYPE_RS, id, Constants.TypeOfTable.CALC_TETMP_RLSPD, startDate,
                       Constants.TypeOfTable.CALC_TETMP_RLSPD, (termDate - startDate).Days, mode, te80EnvMesp.F80_T_Ucl ?? 0, te80EnvMesp.F80_T_Lcl ?? 0,
                       te80EnvMesp.F80_T_Usl ?? 0, te80EnvMesp.F80_T_Lsl ?? 0, te80EnvMesp.F80_T_Dis_Up ?? 0, te80EnvMesp.F80_H_Dis_Up ?? 0,
                       te80EnvMesp.F80_T_Dis_Lo ?? 0, te80EnvMesp.F80_H_Dis_Lo ?? 0, te80EnvMesp.F80_H_Ucl ?? 0, te80EnvMesp.F80_H_Lcl ?? 0,
                       te80EnvMesp.F80_H_Usl ?? 0, te80EnvMesp.F80_H_Lsl ?? 0, te80EnvMesp.F80_T_Cal_Up ?? 0, te80EnvMesp.F80_T_Cal_Lo ?? 0,
                       te80EnvMesp.F80_H_Cal_Up ?? 0, te80EnvMesp.F80_H_Cal_Lo ?? 0, true, lstTable);
                if (!resultEndDate)
                {
                    continue;
                }
                var dt1 = startDate.AddHours(8);
                var dt2 = endDate.AddHours(8);
                var mean = CalcMean(Constants.TypeOfTable.CALC_BUFFER, Constants.TypeOfTable.CALC_TETMP_RLSPD90,
                    id, dt1, dt2, te80EnvMesp.F80_T_Cal_Up??0,
                    te80EnvMesp.F80_T_Cal_Lo??0, te80EnvMesp.F80_H_Cal_Up??0, te80EnvMesp.F80_H_Cal_Lo??0, lstTable);
                if (mean.Item1)
                {
                  
                  UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB, Constants.TypeOfTable.CALC_TETMP_RLSPD90
                        , dt1, mean.Item2,
                        Constants.EnvFieldNo.FLD_MEAN, lstTable);
                }
                else
                {
                    return new ResponseResult<CreepingAndRollSpeedDurationItem>(null, false);   
                }
                var sigma = CalcSigma(Constants.TypeOfTable.CALC_BUFFER, Constants.TypeOfTable.CALC_BUFFER,
                    (int) Constants.TypeOfTable.CALC_TETMP_RLSPD90, dt1, dt2, te80EnvMesp.F80_T_Cal_Up??0,
                    te80EnvMesp.F80_T_Cal_Lo??0, te80EnvMesp.F80_H_Cal_Up??0, te80EnvMesp.F80_H_Cal_Lo??0, lstTable);
                var max = GetMax(Constants.EnvType.TYPE_RS, Constants.TypeOfTable.CALC_BUFFER,
                    Constants.TypeOfTable.CALC_TETMP_RLSPD90, id, dt1, dt2, te80EnvMesp.F80_T_Dis_Up??0,
                    te80EnvMesp.F80_H_Dis_Up??0, lstTable);
                var min = GetMin(Constants.EnvType.TYPE_RS, Constants.TypeOfTable.CALC_BUFFER,
                    Constants.TypeOfTable.CALC_TETMP_RLSPD90, id, dt1, dt2, te80EnvMesp.F80_T_Dis_Lo??0,
                    te80EnvMesp.F80_H_Dis_Lo??0, lstTable);
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
                
                UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB, Constants.TypeOfTable.CALC_TETMP_RLSPD90
                       , dt1, rl_lower ?? 0,
                       Constants.EnvFieldNo.FLD_LOWER, lstTable);
                UpdateBuffer(Constants.EnvActionBuffer.UPDATE_DB, Constants.TypeOfTable.CALC_TETMP_RLSPD90
                       , dt1, rl_upper ?? 0,
                       Constants.EnvFieldNo.FLD_UPPER, lstTable);
                double r_cp = 0;
                double r_cpk = 0;
                CalcCp(Constants.TypeOfTable.CALC_TETMP_RLSPD, mean.Item2, sigma.Item2, max - min, te80EnvMesp.F80_T_Usl??0,
                    te80EnvMesp.F80_T_Lsl??0,ref r_cp,ref r_cpk, b_TEMPUSL_NULL, b_HUMIDUSL_NULL, te80EnvMesp.F80_T_Lcl??0, te80EnvMesp.F80_T_Ucl??0,
                    te80EnvMesp.F80_H_Lcl??0, te80EnvMesp.F80_H_Ucl??0);
                var dataChart = GrapData(Constants.TypeOfTable.CALC_TETMP_RLSPD90, lstTable);
                switch (id)
                {
                    case 1:
                    case 3:
                        item.LeftModel = new ChartModel()
                        {
                            ChartName = machine.ToString() + ": Left Creeping",
                            Cp = r_cp.ToString("f"),Cpk = r_cpk.ToString("F"),
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
                        item.RollModel = new ChartModel()
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
    }
}
