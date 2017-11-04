using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.Presentation;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.EnvironmentManagement
{
    
    public class EnvironmentBaseDomain : IEnvironmentBaseDomain
    {
        public readonly IUnitOfWork _unitOfWork;

        public EnvironmentBaseDomain(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public List<Te80_Env_Mesp> GetLocationItemByType(string type)
        {
            var locations = _unitOfWork.EnvMespRepository.GetAll().Where(x => x.F80_Type == type);
            return locations.ToList();
        }

        public Te80_Env_Mesp GetLocationItemByName(string type, int id, string name)
        {
            var location = _unitOfWork.EnvMespRepository.Get(x => x.F80_Type == type && x.F80_Id == id && x.F80_Name.Trim().Equals(name.Trim()));
            return location;
        }
        public Tuple<bool,string,Te80_Env_Mesp> GetMespVal(Constants.EnvType tblType, int id, Constants.EnvMode envmode)
        {
            // Get USL, LSL, UCL, LCL, Disp UP, Disp LO, CAL Up, CAL LO for both temperature and humidity
            var f80type = tblType.ToString("D");
            var te80EnvMesp =
                _unitOfWork.EnvMespRepository.GetMany(i => i.F80_Type.Equals(f80type) && i.F80_Id.Equals(id))
                    .FirstOrDefault();

            if (te80EnvMesp == null)
                //MessageBox("Select Error", "Record not found!", StopSign!);
                return new Tuple<bool, string, Te80_Env_Mesp>(false, "Record not found!",null);

            if (te80EnvMesp.F80_T_Usl == null && te80EnvMesp.F80_T_Lsl == null &&
                envmode == Constants.EnvMode.SpecLine && tblType == Constants.EnvType.TYPE_RM)
            {
                //reply = MessageBox("Environment Management", "Values for USL and LSL of temperature are not available! Continue ?", StopSign!, YesNo!);
                return new Tuple<bool, string, Te80_Env_Mesp>(false, "Values for USL and LSL of temperature are not available! Continue ?", null);
            }

            if (te80EnvMesp.F80_T_Ucl == null && te80EnvMesp.F80_T_Lcl == null &&
                envmode == Constants.EnvMode.ControlLine && tblType == Constants.EnvType.TYPE_RM)
            {
                //reply = MessageBox("Environment Management", " Values for UCL and LCL of temperature are not available! Continue ?", StopSign!, YesNo!);
                return new Tuple<bool, string, Te80_Env_Mesp>(false, "Values for UCL and LCL of temperature are not available! Continue ?", null);
            }

            if (te80EnvMesp.F80_T_Usl == null && te80EnvMesp.F80_T_Lsl == null &&
                envmode == Constants.EnvMode.SpecLine && tblType == Constants.EnvType.TYPE_RS)
            {
                //reply = MessageBox("Environment Management", "Values for USL and LSL of roll speed are not available! Continue ?", StopSign!, YesNo!);
                return new Tuple<bool, string, Te80_Env_Mesp>(false, "Values for USL and LSL of roll speed are not available! Continue ?", null);
            }

            if (te80EnvMesp.F80_T_Ucl == null && te80EnvMesp.F80_T_Lcl == null &&
                envmode == Constants.EnvMode.ControlLine && tblType == Constants.EnvType.TYPE_RS)
            {
                //reply = MessageBox("Environment Management", "Values for UCL and LCL of roll speed are not available! Continue ?", StopSign!, YesNo!);
                return new Tuple<bool, string, Te80_Env_Mesp>(false, "Values for UCL and LCL of roll speed are not available! Continue ?", null);
            }

            if (te80EnvMesp.F80_Humidity == "1")
            {
                if (te80EnvMesp.F80_T_Usl == null && te80EnvMesp.F80_T_Lsl == null &&
                    envmode == Constants.EnvMode.SpecLine)
                {
                    //reply = MessageBox("Environment Management", "Values for USL and LSL of humidity are not available! Continue ?", StopSign!, YesNo!);
                    return new Tuple<bool, string, Te80_Env_Mesp>(false, "Values for USL and LSL of humidity are not available! Continue ?", null);
                }

                if (te80EnvMesp.F80_T_Ucl == null && te80EnvMesp.F80_T_Lcl == null &&
                    envmode == Constants.EnvMode.ControlLine)
                {
                    //reply = MessageBox("Environment Management", "Values for UCL and LCL of humidity are not available! Continue ?", StopSign!, YesNo!);
                    return new Tuple<bool, string, Te80_Env_Mesp>(false, "Values for UCL and LCL of humidity are not available! Continue ?", null);
                }
            }
            return new Tuple<bool, string, Te80_Env_Mesp>(true, string.Empty, te80EnvMesp);
        }
        public Tuple<bool,string> CalcuTemper(Constants.TypeOfTable proc_type, Constants.TypeOfTable? iddb, int id, double rl_upper, double rl_lower, double rl_max, double rl_min, DateTime dt1, DateTime dt2, double t_dis_up, double t_dis_lo, Constants.EnvMode mod_dt, List<Graphtbl> data)
        {
            var lmaxout = rl_upper;
            var lminout = rl_lower;
            var lmax = rl_upper;
            var lmin = rl_lower;
            var envTempRepositories = _unitOfWork.EnvTempRepository.GetAll().Where(
                            x => x.F81_Id == id && x.F81_Env_Time >= dt1 && x.F81_Env_Time < dt2 && x.F81_Temp != 999.9);
            var calcAadjval = CalcAadjval(out lmaxout, out lminout, lmax, lmin, rl_max, rl_min, proc_type);
            if (proc_type == Constants.TypeOfTable.CALC_TE81_TEMP)
            {
                var f81temCount = envTempRepositories.Count();
                if (f81temCount == 0)
                    return new Tuple<bool, string>(false, "No record found on the specified day!");
                //var t = envTempRepositories.ToList();
                foreach (var item in envTempRepositories)
                {
                    if ((item.F81_Temp <= t_dis_up) && (item.F81_Temp >= t_dis_lo))
                    {
                        if  (calcAadjval == 0)
                        {
                            item.F81_Temp = CalcPoint(lmaxout, lminout, rl_max, rl_min, item.F81_Temp.Value);
                        }
                    }
                    UpdateBuffer(Constants.EnvActionBuffer.INSERT_DB, Constants.TypeOfTable.CALC_TE81_TEMP,
                        item.F81_Env_Time, item.F81_Temp.Value, Constants.EnvFieldNo.FLD_DATA, data);
                }
                return new Tuple<bool, string>(true, "");
            }
            else
            {
                foreach (var item in data)
                {
                    if (item.buf_type == iddb.Value)
                    {
                        if ((item.Dec_data <= t_dis_up) && (item.Dec_data >= t_dis_lo))
                        {
                            if  (calcAadjval == 0)
                            {
                                item.Dec_data = CalcPoint(lmaxout, lminout, rl_max, rl_min, item.Dec_data.Value);
                            }
                        }
                        item.Dec_data = Math.Round(item.Dec_data.Value, 5);
                        item.buf_type = Constants.TypeOfTable.CALC_TETMP_TEMP90;
                    }
                }
                return new Tuple<bool, string>(true, "");
            }
            
        }
        public Tuple<bool, string> CalRollspd(Constants.TypeOfTable id_disp,int id, double rl_upper, double rl_lower, double rl_max, double rl_min, DateTime dt1, DateTime dt2,double t_dis_up,double t_dis_lo,  Constants.EnvMode mod_dt, List<Graphtbl> data)
        {
            var lmaxout = rl_upper;
            var lminout = rl_lower;
            var lmax = rl_upper;
            var lmin = rl_lower;
            var calcAadjval = CalcAadjval(out lmaxout, out lminout, lmax, lmin, rl_max, rl_min, id_disp);
            if (id_disp == Constants.TypeOfTable.CALC_TE83)
            {
                var envTempRepositories = _unitOfWork.EnvElseRepository.GetAll().Where(
                    x => x.F83_Id == id && x.F83_Env_Time >= dt1 && x.F83_Env_Time < dt2 && x.F83_Value != -1);
                var f83temCount = envTempRepositories.Count();
                if (f83temCount == 0)
                    return new Tuple<bool, string>(false, "No record found on the specified day!");
                foreach (var item in envTempRepositories)
                {
                    if ((item.F83_Value <= t_dis_up) && (item.F83_Value >= t_dis_lo))
                    {
                        if (calcAadjval == 0)
                        {
                            item.F83_Value = CalcPoint(lmaxout, lminout, rl_max, rl_min, item.F83_Value.Value);
                        }
                    }
                    UpdateBuffer(Constants.EnvActionBuffer.INSERT_DB, id_disp,
                        item.F83_Env_Time, item.F83_Value.Value, Constants.EnvFieldNo.FLD_DATA, data);
                }
                return new Tuple<bool, string>(true, "");

            }
            else
            {
                foreach (var item in data)
                {
                    if (item.buf_type == Constants.TypeOfTable.CALC_TE83_90)
                    {
                        if ((item.Dec_data <= t_dis_up) && (item.Dec_data >= t_dis_lo))
                        {
                            if (calcAadjval == 0)
                            {
                                item.Dec_data = CalcPoint(lmaxout, lminout, rl_max, rl_min, item.Dec_data.Value);
                            }
                        }
                        item.Dec_data = Math.Round(item.Dec_data.Value, 5);
                        item.buf_type = Constants.TypeOfTable.CALC_TETMP_RLSPD90;
                    }
                }
                return new Tuple<bool, string>(true, "");
            }
        }
        public Tuple<bool, string> CalcuHumid(Constants.TypeOfTable proc_type, Constants.TypeOfTable? iddb, int id, double rl_upper, double rl_lower, double rl_max, double rl_min, DateTime dt1, DateTime dt2, 
            double h_dis_up, double h_dis_lo, Constants.EnvMode mod_dt, List<Graphtbl> data)
        {
            var lmaxout = rl_upper;
            var lminout = rl_lower;
            var lmax = rl_upper;
            var lmin = rl_lower;
            var calcAadjval = CalcAadjval(out lmaxout, out lminout, lmax, lmin, rl_max, rl_min, Constants.TypeOfTable.CALC_TE81_HUMID);
            if (proc_type == Constants.TypeOfTable.CALC_TE81_TEMP)
            {
                var envTempRepositories = _unitOfWork.EnvTempRepository.GetAll().Where(
                    x => x.F81_Id == id && x.F81_Env_Time >= dt1 && x.F81_Env_Time < dt2 && x.F81_Humidity != 999.9);
                var f81temCount = envTempRepositories.Count();
                if (f81temCount == 0)
                    return new Tuple<bool, string>(false, "No record found on the specified day!");
                foreach (var item in envTempRepositories)
                {
                    if ((item.F81_Humidity <= h_dis_up) && (item.F81_Humidity >= h_dis_lo))
                    {
                        if (calcAadjval == 0)
                        {
                            item.F81_Humidity = CalcPoint(lmaxout, lminout, rl_max, rl_min, item.F81_Humidity.Value);
                        }
                    }
                    UpdateBuffer(Constants.EnvActionBuffer.INSERT_DB, Constants.TypeOfTable.CALC_TE81_HUMID,
                        item.F81_Env_Time, item.F81_Humidity.Value, Constants.EnvFieldNo.FLD_DATA, data);
                }
                return new Tuple<bool, string>(true, "");

            }
            else
            {
                foreach (var item in data)
                {
                    if (item.buf_type == iddb.Value)
                    {
                        if ((item.Dec_data <= h_dis_up) && (item.Dec_data >= h_dis_lo))
                        {
                            if  (calcAadjval == 0)
                            {
                                item.Dec_data = CalcPoint(lmaxout, lminout, rl_max, rl_min, item.Dec_data.Value);
                            }
                        }
                        item.Dec_data = Math.Round(item.Dec_data.Value, 5);
                        item.buf_type = Constants.TypeOfTable.CALC_TETMP_HUMID90;
                    }
                }
                return new Tuple<bool, string>(true, "");
            }
        }
        public void UpdateBuffer(Constants.EnvActionBuffer action, Constants.TypeOfTable buf_type, DateTime dtmBuffer, double rl_data, Constants.EnvFieldNo field_no,List<Graphtbl> data)
        {
            var graphtbl = new Graphtbl();
            if (action == Constants.EnvActionBuffer.INSERT_DB)
            {
                switch (field_no)
                {
                    case Constants.EnvFieldNo.FLD_DATA:
                        graphtbl.Dec_data = Math.Round(rl_data, 5);
                        break;
                    case Constants.EnvFieldNo.FLD_MEAN:
                        graphtbl.Dec_mean = Math.Round(rl_data, 5);
                        break;
                    case Constants.EnvFieldNo.FLD_UPPER:
                        graphtbl.Dec_upper = Math.Round(rl_data, 5);
                        break;
                    case Constants.EnvFieldNo.FLD_LOWER:
                        graphtbl.Dec_lower = Math.Round(rl_data, 5);
                        break;
                }

                graphtbl.dt_dtm = dtmBuffer;
                graphtbl.buf_type = buf_type;
                data.Add(graphtbl);
            }
            if (action == Constants.EnvActionBuffer.UPDATE_DB)
            {
                foreach (var item  in data)
                {
                    if (item.buf_type == buf_type)
                    {
                        switch (field_no)
                        {
                            case Constants.EnvFieldNo.FLD_MEAN:
                                item.Dec_mean = Math.Round(rl_data, 5);
                                break;
                            case Constants.EnvFieldNo.FLD_UPPER:
                                item.Dec_upper = Math.Round(rl_data, 5);
                                break;
                            case Constants.EnvFieldNo.FLD_LOWER:
                                item.Dec_lower = Math.Round(rl_data, 5);
                                break;
                        }
                    }
                }
            }
        }
        private double CalcPoint(double upper, double lower, double max_pt, double min_pt, double calc_pt)
        {
            if (max_pt != min_pt)
            {
                return lower + ((calc_pt - min_pt)*(upper - lower)/(max_pt - min_pt));
            }
            else
            {
                return upper;
            }
        }
        private int CalcAadjval(out double Lmaxout,out double Lminout, double Lmax, double Lmin, double rmax, double rmin,
            Constants.TypeOfTable id_disp)
        {
            double Kmax;
            double Kmin;
            double K;
            double Vmax;
            double Vmin;
            Lmaxout = Lmax;
            Lminout = Lmin;
            if ((id_disp == Constants.TypeOfTable.CALC_TE81_TEMP) || (id_disp == Constants.TypeOfTable.CALC_TETMP_TEMP90) || (id_disp == Constants.TypeOfTable.CALC_TE83) ||
                    (id_disp == Constants.TypeOfTable.CALC_TETMP_RLSPD))
            {
                Kmax = Lmax - 1;
                Kmin = Lmin + 1;
                K = 0.1;
            }
            else
            {
                Kmax = Lmax - 2;
                Kmin = Lmin + 2;
                K = 0.05;
            }

            if (rmax <= Lmin || rmin >= Lmax)
            {
                if ((rmax - rmin) <= (Lmax - Lmin))
                {
                    Vmax = ((Lmax + Lmin)/2) + ((rmax - rmin)/2);
                    Vmin = ((Lmax + Lmin)/2) - ((rmax - rmin)/2);
                }
                else
                {
                    if (rmin > Lmax)
                    {
                        Vmax = Kmax + ((rmax - Kmax)*K);
                        if (Vmax > Lmax)
                        {
                            Vmax = Lmax;
                        }
                        Vmin = Kmin;
                    }
                    else {
                        Vmax = Kmax;
                        Vmin = Kmin - ((Kmin - rmin) * K);
                        if (Vmin < Lmin)
                        {
                            Vmin = Lmin;
                        }
                    }
                        
                }
                Lmaxout = Vmax;
                Lminout = Vmin;
                return 0;
            }

            if ((rmax <= Lmax) && (rmin >= Lmin))
            {
                Lmaxout = 0;
                Lminout = 0;
                return 1;
            }

            if ((rmax > Lmax) && (rmin < Lmin))
            {
                Vmax = Kmax + ((rmax - Kmax) * K);
                Vmin = Kmin - ((Kmin - rmin) * K);
                if (Vmax > Lmax)
                {
                    Vmax = Lmax;
                }
                if (Vmin < Lmin)
                {
                    Vmin = Lmin;
                }
                Lmaxout = Vmax;
                Lminout = Vmin;
                return 0;
            }

            if ((rmax > Lmax) && (rmin >= Lmin))
            {
                Vmax = Kmax + ((rmax - Kmax) * K);
                if (Vmax > Lmax)
                {
                    Vmax = Lmax;
                }
                Vmin = rmin;
                Lmaxout = Vmax;
                Lminout = Vmin;
                return 0;
            }

            if ((rmax <= Lmax) && (rmin < Lmin))
            {
                Vmax = rmax;
                Vmin = Kmin - ((Kmin - rmin) * K);
                if (Vmin < Lmin)
                {
                    Vmin = Lmin;
                }
                Lmaxout = Vmax;
                Lminout = Vmin;
                return 0;
            }

            if (rmax <= Lmin || rmin >= Lmax)
            {
                if ((rmax - rmin) <= (Lmax - Lmin))
                {
                    Vmax = ((Lmax + Lmin) / 2) + ((rmax - rmin) / 2);
                    Vmin = ((Lmax + Lmin) / 2) - ((rmax - rmin) / 2);
                }
                else
                {
                    if (rmin > Lmax)
                    {
                        Vmax = Kmax + ((rmax - Kmax) * K);
                        if (Vmax > Lmax)
                        {
                            Vmax = Lmax;
                        }
                        Vmin = Kmin;
                    }
                    else {
                        Vmax = Kmax;
                        Vmin = Kmin - ((Kmin - rmin) * K);
                        if (Vmin < Lmin)
                        {
                            Vmin = Lmin;
                        }
                    }
                        
                }
                Lmaxout = Vmax;
                Lminout = Vmin;
                return 0;
            }
            return 0;
        }
        public double GetMax(Constants.EnvType tblType, Constants.TypeOfTable fieldNo, Constants.TypeOfTable? iddb, int id, DateTime dt1, DateTime dt2, double t_dis_up, double h_dis_up, List<Graphtbl> data)
        {
            int res;
            double maxVal = 0;
            switch (tblType)
            {
                case Constants.EnvType.TYPE_RM:
                    if (fieldNo == Constants.TypeOfTable.CALC_TE81_TEMP)
                    {
                        maxVal = t_dis_up;
                        var datatem =
                           _unitOfWork.EnvTempRepository.GetMany(
                               i =>
                                   i.F81_Id.Equals(id) && i.F81_Env_Time >= dt1 && i.F81_Env_Time <= dt2 &&
                                   i.F81_Temp != 999.9);

                        if (datatem.Any())
                        {
                            if ((datatem.Max(x => x.F81_Temp.Value) < t_dis_up))
                                maxVal = datatem.Max(x => x.F81_Temp.Value);
                        }

                    }
                    else if (fieldNo == Constants.TypeOfTable.CALC_TE81_HUMID)
                    {
                        maxVal = h_dis_up;
                        var datatem =
                            _unitOfWork.EnvTempRepository.GetMany(
                                i =>
                                    i.F81_Id.Equals(id) && i.F81_Env_Time >= dt1 && i.F81_Env_Time < dt2 &&
                                    i.F81_Humidity != 999.9);
                        if (datatem.Any())
                        {
                            if (datatem.Max(x => x.F81_Humidity.Value) < h_dis_up) maxVal = datatem.Max(x => x.F81_Humidity.Value);
                        }
                            
                    }else if (fieldNo == Constants.TypeOfTable.CALC_TETMP_TEMP || fieldNo == Constants.TypeOfTable.CALC_TETMP_HUMID)
                    {
                        //TODO: 
                        //  Dynamic Table access
              //          trans_TmpTbl.AutoCommit = TRUE;
              //          s_sql = "SELECT MAX(" + S_FLD_DATA + ") FROM " + TMP_TBL + " WHERE " + S_FLD_ID + " = ? AND " + S_FLD_DTM + " >= ? AND " + &
              //                  S_FLD_DTM + " < ? ";

              //          PREPARE SQLSA FROM: s_sql USING trans_TmpTbl;
              //          DESCRIBE SQLSA INTO SQLDA;
              //          DECLARE cur_max DYNAMIC CURSOR FOR SQLSA;
              //          SetDynamicParm(SQLDA, 1, id);
              //          SetDynamicParm(SQLDA, 2, dt1);
              //          SetDynamicParm(SQLDA, 3, dt2);
              //          OPEN DYNAMIC cur_max USING DESCRIPTOR SQLDA;
              //          if trans_TmpTbl.sqlcode <> 0 then
              //              res = FAILURE;
              //          MessageBox("Dynamic SQL Error", trans_TmpTbl.sqlerrtext, StopSign!);
              //          rollback using trans_TmpTbl;
              //          return FAILURE;
              //          end if
              

              //          FETCH cur_max USING DESCRIPTOR SQLDA;
              //          if trans_TmpTbl.sqlcode <> 0 then
              //MessageBox("Dynamic SQL Error", trans_TmpTbl.sqlerrtext, StopSign!);
              //          CLOSE cur_max;
              //          rollback using trans_TmpTbl;
              //          return FAILURE;
              //          end if
              
              //          max_val = GetDynamicNumber(SQLDA, 1);
              //          if id = CALC_TE81_TEMP OR id = CALC_TE81_TEMP90  OR id = CALC_TETMP_TEMP90 then
              //if max_val > t_dis_up then
              //    max_val = t_dis_up;
              //          end if
              //      else
              //          if max_val > h_dis_up then
              //              max_val = h_dis_up;
              //          end if
          
              //      end if
          
              //      CLOSE cur_max;
              //          trans_TmpTbl.AutoCommit = FALSE;


                    }
                    else if (fieldNo == Constants.TypeOfTable.CALC_MGMT_LIMIT || fieldNo == Constants.TypeOfTable.CALC_BUFFER)
                    {
                        if (data.Count > 0)
                        {
                            maxVal = data.Where(x => x.buf_type == iddb && x.Dec_data.HasValue).Max(x => x.Dec_data.Value);
                            if (iddb == Constants.TypeOfTable.CALC_TE81_TEMP ||
                                iddb == Constants.TypeOfTable.CALC_TETMP_TEMP90 ||
                                iddb == Constants.TypeOfTable.CALC_MGMT_LIMIT_TEMP)
                            {
                                if (maxVal > t_dis_up)
                                {
                                    maxVal = t_dis_up;
                                }
                            }
                            else
                            {
                                if (maxVal > h_dis_up)
                                {
                                    maxVal = h_dis_up;
                                }
                            }
                        }
                        
                    }
                    break;
                case Constants.EnvType.TYPE_RS:
                    if (fieldNo == Constants.TypeOfTable.CALC_TE83)
                    {
                        maxVal = t_dis_up;
                        var datatem =
                           _unitOfWork.EnvElseRepository.GetAll().Where(
                               i =>
                                   i.F83_Id.Equals(id) && i.F83_Env_Time >= dt1 && i.F83_Env_Time <= dt2 &&
                                   i.F83_Value != -1).ToList();
                        if (datatem.Any())
                        {
                            if (datatem.Max(x => x.F83_Value.Value) < t_dis_up) maxVal = datatem.Max(x => x.F83_Value.Value);
                        }
                    }
                    else if (fieldNo == Constants.TypeOfTable.CALC_TETMP_RLSPD)
                    {
                        //TODO:
                        //  Dynamic Table access
              //          trans_TmpTbl.AutoCommit = TRUE;
              //          s_sql = "SELECT MAX(" + S_FLD_DATA + ") FROM " + TMP_TBL + " WHERE " + S_FLD_ID + " = ? AND " + S_FLD_DTM + " >= ? AND " + &
              //                  S_FLD_DTM + " < ? ";

              //          PREPARE SQLSA FROM: s_sql USING trans_TmpTbl;
              //          DESCRIBE SQLSA INTO SQLDA;
              //          DECLARE cur_max_spd DYNAMIC CURSOR FOR SQLSA;
              //          SetDynamicParm(SQLDA, 1, id);
              //          SetDynamicParm(SQLDA, 2, dt1);
              //          SetDynamicParm(SQLDA, 3, dt2);
              //          OPEN DYNAMIC cur_max_spd USING DESCRIPTOR SQLDA;
              //          if trans_TmpTbl.sqlcode <> 0 then
              //              res = FAILURE;
              //          MessageBox("Dynamic SQL Error", trans_TmpTbl.sqlerrtext, StopSign!);
              //          rollback using trans_TmpTbl;
              //          return FAILURE;
              //          end if
              

              //          FETCH cur_max_spd USING DESCRIPTOR SQLDA;
              //          if trans_TmpTbl.sqlcode <> 0 then
              //MessageBox("Dynamic SQL Error", trans_TmpTbl.sqlerrtext, StopSign!);
              //          CLOSE cur_max_spd;
              //          rollback using trans_TmpTbl;
              //          return FAILURE;
              //          end if
              
              //          max_val = GetDynamicNumber(SQLDA, 1);
              //          if max_val > t_dis_up then
              //              max_val = t_dis_up;
              //          end if
              
              //          CLOSE cur_max_spd;
              //          trans_TmpTbl.AutoCommit = FALSE;
                    }
                    else if (fieldNo == Constants.TypeOfTable.CALC_BUFFER)
                    {
                        if (data.Count > 0)
                        {
                            maxVal = data.Where(x => x.buf_type == iddb && x.Dec_data.HasValue).Max(x => x.Dec_data.Value);
                            if (maxVal > t_dis_up)
                            {
                                maxVal = t_dis_up;
                            }
                        }
                        
                    }
                    break;
                case Constants.EnvType.TYPE_LT:
                    if (fieldNo == Constants.TypeOfTable.CALC_LOT_AVG)
                    {
                        maxVal = data.Where(x => x.buf_type == iddb && x.Dec_mean.HasValue).Max(x => x.Dec_mean.Value);
                    }
                    if (fieldNo == Constants.TypeOfTable.CALC_LOT_RANGE)
                    {
                        maxVal = data.Where(x => x.buf_type == iddb && x.Dec_upper.HasValue).Max(x => x.Dec_upper.Value);
                    }
                    break;
            }
            return maxVal;
        }
        public double GetMin(Constants.EnvType tblType, Constants.TypeOfTable fieldNo, Constants.TypeOfTable? iddb, int id, DateTime dt1, DateTime dt2, double t_dis_lo, double h_dis_lo, List<Graphtbl> data)
        {
            double minVal = 0;
            switch (tblType)
            {
                case Constants.EnvType.TYPE_RM:
                    if (fieldNo == Constants.TypeOfTable.CALC_TE81_TEMP)
                    {
                        minVal = t_dis_lo;
                        var datatem =
                           _unitOfWork.EnvTempRepository.GetMany(
                               i =>
                                   i.F81_Id.Equals(id) && i.F81_Env_Time >= dt1 && i.F81_Env_Time <= dt2 &&
                                   i.F81_Temp != 999.9);
                        if (datatem.Any())
                        {
                            if (datatem.Min(x => x.F81_Temp.Value) > t_dis_lo) minVal = datatem.Min(x => x.F81_Temp.Value);
                        }

                    }
                    if (fieldNo == Constants.TypeOfTable.CALC_TE81_HUMID)
                    {
                        minVal = h_dis_lo;
                        var datatem =
                            _unitOfWork.EnvTempRepository.GetMany(
                                i =>
                                    i.F81_Id.Equals(id) && i.F81_Env_Time >= dt1 && i.F81_Env_Time < dt2 &&
                                    i.F81_Humidity != 999.9);
                        if (datatem.Any())
                        {
                            if (datatem.Min(x => x.F81_Humidity.Value) > h_dis_lo) minVal = datatem.Min(x => x.F81_Humidity.Value);
                        }
                    }
                    if (fieldNo == Constants.TypeOfTable.CALC_TETMP_TEMP || fieldNo == Constants.TypeOfTable.CALC_TETMP_HUMID)
                    {
                        //TODO: 
              //          trans_TmpTbl.AutoCommit = TRUE;
              //          s_sql = "SELECT MIN(" + S_FLD_DATA + ") FROM " + TMP_TBL + " WHERE " + S_FLD_ID + " = ? AND " + S_FLD_DTM + " >= ? AND " + &
              //                  S_FLD_DTM + " < ? ";

              //          PREPARE SQLSA FROM: s_sql USING trans_TmpTbl;
              //          DESCRIBE SQLSA INTO SQLDA;
              //          DECLARE cur_min DYNAMIC CURSOR FOR SQLSA;
              //          SetDynamicParm(SQLDA, 1, id);
              //          SetDynamicParm(SQLDA, 2, dt1);
              //          SetDynamicParm(SQLDA, 3, dt2);
              //          OPEN DYNAMIC cur_min USING DESCRIPTOR SQLDA;
              //          if trans_TmpTbl.sqlcode <> 0 then
              //              res = FAILURE;
              //          MessageBox("Dynamic SQL Error", trans_TmpTbl.sqlerrtext, StopSign!);
              //          rollback using trans_TmpTbl;
              //          return FAILURE;
              //          end if
              

              //          FETCH cur_min USING DESCRIPTOR SQLDA;
              //          if trans_TmpTbl.sqlcode <> 0 then
              //MessageBox("Dynamic SQL Error", trans_TmpTbl.sqlerrtext, StopSign!);
              //          CLOSE cur_min;
              //          rollback using trans_TmpTbl;
              //          return FAILURE;
              //          end if
              
              //          min_val = GetDynamicNumber(SQLDA, 1);
              //          if id = CALC_TE81_TEMP OR id = CALC_TE81_TEMP90 OR id = CALC_TETMP_TEMP90 then
              //if min_val < t_dis_lo then
              //    min_val = t_dis_lo;
              //          end if
              //      else
              //          if min_val < h_dis_lo then
              //              min_val = h_dis_lo;
              //          end if
          
              //      end if
          
              //      CLOSE cur_min;
              //          trans_TmpTbl.AutoCommit = FALSE;


                    }
                    if (fieldNo == Constants.TypeOfTable.CALC_MGMT_LIMIT || fieldNo == Constants.TypeOfTable.CALC_BUFFER)
                    {
                        if (data.Count > 0)
                        {
                            minVal = data.Where(x => x.Dec_data.HasValue && x.buf_type == iddb).Min(x => x.Dec_data.Value);
                            if (iddb == Constants.TypeOfTable.CALC_TE81_TEMP ||
                                iddb == Constants.TypeOfTable.CALC_MGMT_LIMIT_TEMP ||
                                iddb == Constants.TypeOfTable.CALC_TETMP_TEMP90 ||
                                iddb == Constants.TypeOfTable.CALC_TE81_TEMP90)
                            {
                                if (minVal < t_dis_lo) minVal = t_dis_lo;
                            }
                            else
                            {
                                if (minVal < h_dis_lo) minVal = h_dis_lo;
                            }
                        }
                    }
                    break;
                case Constants.EnvType.TYPE_RS:
                    if (fieldNo == Constants.TypeOfTable.CALC_TE83)
                    {
                        minVal = t_dis_lo;
                        var datatem =
                           _unitOfWork.EnvElseRepository.GetAll().Where(
                               i =>
                                   i.F83_Id.Equals(id) && i.F83_Env_Time >= dt1 && i.F83_Env_Time <= dt2 &&
                                   i.F83_Value != -1).ToList();
                        if (datatem.Any())
                        {
                            if (datatem.Min(x => x.F83_Value.Value) > t_dis_lo) minVal = datatem.Min(x => x.F83_Value.Value);
                        }

                    }
                    if (fieldNo == Constants.TypeOfTable.CALC_TETMP_RLSPD)
                    {
                        //TODO:
              //          trans_TmpTbl.AutoCommit = TRUE;
              //          s_sql = "SELECT MIN(" + S_FLD_DATA + ") FROM " + TMP_TBL + " WHERE " + S_FLD_ID + " = ? AND " + S_FLD_DTM + " >= ? AND " + &
              //                  S_FLD_DTM + " < ? ";

              //          PREPARE SQLSA FROM: s_sql USING trans_TmpTbl;
              //          DESCRIBE SQLSA INTO SQLDA;
              //          DECLARE cur_min_spd DYNAMIC CURSOR FOR SQLSA;
              //          SetDynamicParm(SQLDA, 1, id);
              //          SetDynamicParm(SQLDA, 2, dt1);
              //          SetDynamicParm(SQLDA, 3, dt2);
              //          OPEN DYNAMIC cur_min_spd USING DESCRIPTOR SQLDA;
              //          if trans_TmpTbl.sqlcode <> 0 then
              //              res = FAILURE;
              //          MessageBox("Dynamic SQL Error", trans_TmpTbl.sqlerrtext, StopSign!);
              //          rollback using trans_TmpTbl;
              //          return FAILURE;
              //          end if
              

              //          FETCH cur_min_spd USING DESCRIPTOR SQLDA;
              //          if trans_TmpTbl.sqlcode <> 0 then
              //MessageBox("Dynamic SQL Error", trans_TmpTbl.sqlerrtext, StopSign!);
              //          CLOSE cur_min_spd;
              //          rollback using trans_TmpTbl;
              //          return FAILURE;
              //          end if
              
              //          min_val = GetDynamicNumber(SQLDA, 1);
              //          if min_val < t_dis_lo then
              //              min_val = t_dis_lo;
              //          end if
              
              //          CLOSE cur_min_spd;
              //          trans_TmpTbl.AutoCommit = FALSE;

                    }
                    if (fieldNo == Constants.TypeOfTable.CALC_BUFFER)
                    {
                        if (data.Count > 0)
                        {
                            minVal = data.Where(x => x.Dec_data.HasValue && x.buf_type == iddb).Min(x => x.Dec_data.Value);
                            if (minVal < t_dis_lo)
                            {
                                minVal = t_dis_lo;
                            }
                        }
                        
                    }
                    break;
                case Constants.EnvType.TYPE_LT:
                    if (fieldNo == Constants.TypeOfTable.CALC_LOT_AVG)
                    {
                        minVal = data.Where(x => x.Dec_mean.HasValue && x.buf_type == iddb).Min(x => x.Dec_mean.Value);
                    }
                    if (fieldNo == Constants.TypeOfTable.CALC_LOT_RANGE)
                    {
                        minVal = data.Where(x => x.Dec_upper.HasValue && x.buf_type == iddb).Min(x => x.Dec_upper.Value);
                    }
                    break;
            }
            return minVal;
        }

        public void CallLimit(double r_mean, double r_sigma, out double r_ucl, out double r_lcl)
        {
            r_ucl = r_mean + (3 * (r_sigma));
            r_lcl = r_mean - (3 * (r_sigma));
        }
        public Tuple<bool, double> CalcMean(Constants.TypeOfTable calctype, Constants.TypeOfTable? iddb, int id, DateTime dt1, DateTime dt2, double t_cal_up, double t_cal_lo,double h_cal_up,double h_cal_lo, List<Graphtbl> data)
        {
            double rl_total;
            int cnt = 0;
            double cal_up;
            double cal_lo;
            if (calctype == Constants.TypeOfTable.CALC_TE81_TEMP || calctype == Constants.TypeOfTable.CALC_TE81_TEMP90)
            {
                var dataTem =
                           _unitOfWork.EnvTempRepository.GetMany(
                               i =>
                                   i.F81_Id.Equals(id) && i.F81_Env_Time >= dt1 && i.F81_Env_Time <= dt2 &&
                                   i.F81_Temp <= t_cal_up && i.F81_Temp >= t_cal_lo);
                if (dataTem.Any())
                {
                    return new Tuple<bool, double>(true, dataTem.Average(x => x.F81_Temp).Value);
                }
            }

            if (calctype == Constants.TypeOfTable.CALC_TE81_HUMID || calctype == Constants.TypeOfTable.CALC_TE81_HUMID90)
            {
                var dataTem =
                           _unitOfWork.EnvTempRepository.GetMany(
                               i =>
                                   i.F81_Id.Equals(id) && i.F81_Env_Time >= dt1 && i.F81_Env_Time <= dt2 &&
                                   i.F81_Humidity <= h_cal_up && i.F81_Humidity >= h_cal_lo);
                if (dataTem.Any())
                {
                    return new Tuple<bool, double>(true, dataTem.Average(x => x.F81_Humidity).Value);
                }
            }

            if (calctype == Constants.TypeOfTable.CALC_TE83 || calctype == Constants.TypeOfTable.CALC_TE83_90)
            {
                var dataTem =
                           _unitOfWork.EnvElseRepository.GetAll().Where(
                               i =>
                                   i.F83_Id.Equals(id) && i.F83_Env_Time >= dt1 && i.F83_Env_Time <= dt2 &&
                                   i.F83_Value <= t_cal_up && i.F83_Value >= t_cal_lo);
                if (dataTem.Any())
                {
                    return new Tuple<bool, double>(true, dataTem.Average(x => x.F83_Value).Value);
                }
            }

            if (calctype == Constants.TypeOfTable.CALC_TETMP_TEMP || calctype == Constants.TypeOfTable.CALC_TETMP_HUMID ||
                calctype == Constants.TypeOfTable.CALC_TETMP_RLSPD ||
                calctype == Constants.TypeOfTable.CALC_TETMP_TEMP90 ||
                calctype == Constants.TypeOfTable.CALC_TETMP_HUMID90)
            {
                if ((calctype == Constants.TypeOfTable.CALC_TETMP_TEMP ||
                     calctype == Constants.TypeOfTable.CALC_TETMP_RLSPD ||
                     calctype == Constants.TypeOfTable.CALC_TETMP_TEMP90))
                {
                    cal_up = t_cal_up;
                    cal_lo = t_cal_lo;

                }
                else
                {
                    cal_up = h_cal_up;
                    cal_lo = h_cal_lo;
                }
                //TODO: return avg
                //s_sql = "SELECT AVG(" + S_FLD_DATA + ") FROM " + TMP_TBL + " WHERE " + S_FLD_ID + " = ? AND " + S_FLD_DTM + " >= ? AND " + &
                //S_FLD_DTM + " < ? AND " + S_FLD_DATA + " <= ? AND " + S_FLD_DATA + " >= ? ";
                return new Tuple<bool, double>(true, 0);
            }
            if (calctype == Constants.TypeOfTable.CALC_LOT_AVG || calctype == Constants.TypeOfTable.CALC_LOT_RANGE)
            {
                rl_total = 0;
                cnt = 0;
                double r_avg = 0;
                foreach (var item in data)
                {
                    if (calctype == Constants.TypeOfTable.CALC_LOT_AVG && item.buf_type == iddb)
                    {
                        rl_total += item.Dec_mean.Value;
                        cnt = cnt + 1;
                    }
                    else if (calctype == Constants.TypeOfTable.CALC_LOT_RANGE && item.buf_type == iddb)
                    {
                        rl_total += item.Dec_upper.Value;
                        cnt = cnt + 1;
                    }
                }
                if (cnt > 0)
                    r_avg = rl_total / cnt;
                return new Tuple<bool, double>(true, r_avg);
            }
            if (calctype == Constants.TypeOfTable.CALC_MGMT_LIMIT || calctype == Constants.TypeOfTable.CALC_BUFFER)
            {
                rl_total = 0;
                cnt = 0;
                double r_avg = 0;
                if (iddb == Constants.TypeOfTable.CALC_MGMT_LIMIT_TEMP || iddb.Value == Constants.TypeOfTable.CALC_TE81_TEMP ||
                    iddb == Constants.TypeOfTable.CALC_TETMP_TEMP90 || iddb.Value == Constants.TypeOfTable.CALC_TE83 ||
                    iddb == Constants.TypeOfTable.CALC_TETMP_RLSPD90)
                {
                    cal_up = t_cal_up;
                    cal_lo = t_cal_lo;
                }
                else
                {
                    cal_up = h_cal_up;
                    cal_lo = h_cal_lo;
                }
                foreach (var item in data)
                {
                    if (item.buf_type == iddb && item.Dec_data.HasValue)
                    {
                        if (item.Dec_data.Value >= cal_lo && item.Dec_data.Value <= cal_up)
                        {
                            rl_total += item.Dec_data.Value;
                            cnt += 1;
                        }
                    }
                }
                if (cnt > 0)
                    r_avg = rl_total/cnt;
                else
                {
                    r_avg = 0;
                }
                return new Tuple<bool, double>(true, r_avg);
            }

            return new Tuple<bool, double>(false, 0);
       }
        public bool CalcData(Constants.EnvType tblType, int id, Constants.TypeOfTable? iddb, DateTime dt, Constants.TypeOfTable proc_type, int days_no, 
            Constants.EnvMode s_mode,double t_ucl,double t_lcl,double t_usl,double t_lsl,double t_dis_up,
            double h_dis_up,double t_dis_lo,double h_dis_lo,double h_ucl,double h_lcl,double h_usl, double h_lsl,double t_cal_up, double t_cal_lo, double h_cal_up, double h_cal_lo, bool b_DispHumidity, List<Graphtbl> data)
        {
            Constants.TypeOfTable fieldNo;
            double rl_ul;
            double rl_ll;
            double rl_max;
            double rl_min;
            double rl_data;
            var dt1 = dt;
            var dt2 = dt.AddDays(days_no);
            bool res = false;
            #region Constants.EnvType.TYPE_RM
            if (tblType == Constants.EnvType.TYPE_RM)
            {
                #region Constants.TypeOfTable CALC_TE81_TEMP || CALC_TETMP_TEMP90 
                if (proc_type == Constants.TypeOfTable.CALC_TE81_TEMP ||
                    proc_type == Constants.TypeOfTable.CALC_TETMP_TEMP90)
                {
                    if (proc_type == Constants.TypeOfTable.CALC_TE81_TEMP)
                    {
                        fieldNo = Constants.TypeOfTable.CALC_TE81_TEMP;
                    }
                    else
                    {
                        fieldNo = Constants.TypeOfTable.CALC_BUFFER;
                    }

                    if (s_mode.ToString("D").Equals(Constants.EnvMode.ControlLine.ToString("D")))
                    {
                        rl_ul = t_ucl;
                        rl_ll = t_lcl;
                    }
                    else
                    {
                        rl_ul = t_usl;
                        rl_ll = t_lsl;

                    }
                    
                    rl_max = GetMax(tblType, fieldNo, iddb, id, dt1, dt2,t_dis_up,h_dis_up,data);
                    rl_min = GetMin(tblType, proc_type, iddb, id, dt1, dt2, t_dis_lo, h_dis_lo,data);
                    var calcuTemper = CalcuTemper(proc_type, iddb, id, rl_ul, rl_ll, rl_max, rl_min, dt1, dt2, t_dis_up, t_dis_lo,  s_mode, data);
                    if (!calcuTemper.Item1)
                    {
                        return calcuTemper.Item1;
                    }
                    res = calcuTemper.Item1;
                    if (b_DispHumidity)
                    {
                        if (proc_type == Constants.TypeOfTable.CALC_TE81_TEMP)
                        {
                            fieldNo = Constants.TypeOfTable.CALC_TE81_HUMID;
                        }
                        else
                        {
                            fieldNo = Constants.TypeOfTable.CALC_BUFFER;
                            iddb = Constants.TypeOfTable.CALC_TE81_HUMID90;
                        }
                        if (s_mode.ToString("D").Equals(Constants.EnvMode.ControlLine.ToString("D")))
                        {
                            rl_ul = h_ucl;
                            rl_ll = h_lcl;
                        }
                        else
                        {
                            rl_ul = h_usl;
                            rl_ll = h_lsl;
                        }
                        rl_max = GetMax(tblType, fieldNo, iddb, id, dt1, dt2, t_dis_up, h_dis_up, data);
                        rl_min = GetMin(tblType, fieldNo, iddb, id, dt1, dt2, t_dis_lo, h_dis_lo, data);
                        var calcuHumid = CalcuHumid(proc_type, iddb, id, rl_ul, rl_ll, rl_max, rl_min, dt1, dt2, h_dis_up, h_dis_lo,  s_mode, data);
                        res = calcuHumid.Item1;
                    }
                    
                }
                #endregion
                #region Constants.TypeOfTable CALC_TE81_TEMP90 || CALC_MGMT_LIMIT

                if (proc_type == Constants.TypeOfTable.CALC_TE81_TEMP90 ||
                    proc_type == Constants.TypeOfTable.CALC_MGMT_LIMIT)
                {
                    // calc average for 1 day and update to db        
                    var calcaverage = CalcMean(Constants.TypeOfTable.CALC_TE81_TEMP90, iddb, id, dt1, dt2,t_cal_up,t_cal_lo,h_cal_up,h_cal_lo,data);
                    res = calcaverage.Item1;
                    if (calcaverage.Item1)
                    {
                        if (proc_type == Constants.TypeOfTable.CALC_TE81_TEMP90)
                        {
                            UpdateBuffer(Constants.EnvActionBuffer.INSERT_DB, Constants.TypeOfTable.CALC_TE81_TEMP90,
                                dt1, calcaverage.Item2, Constants.EnvFieldNo.FLD_DATA, data);
                        }
                        else if (proc_type == Constants.TypeOfTable.CALC_MGMT_LIMIT)
                        {
                            UpdateBuffer(Constants.EnvActionBuffer.INSERT_DB, Constants.TypeOfTable.CALC_MGMT_LIMIT_TEMP,
                                dt1, calcaverage.Item2, Constants.EnvFieldNo.FLD_DATA, data);
                        }
                    }
                    if (b_DispHumidity)
                    {
                        calcaverage = CalcMean(Constants.TypeOfTable.CALC_TE81_HUMID90, iddb, id, dt1, dt2, t_cal_up, t_cal_lo, h_cal_up, h_cal_lo, data);
                        res = calcaverage.Item1;
                        if (calcaverage.Item1)
                        {
                            if (proc_type == Constants.TypeOfTable.CALC_TE81_TEMP90)
                            {
                                UpdateBuffer(Constants.EnvActionBuffer.INSERT_DB, Constants.TypeOfTable.CALC_TE81_HUMID90,
                                    dt1, calcaverage.Item2, Constants.EnvFieldNo.FLD_DATA, data);
                            }
                            else if (proc_type == Constants.TypeOfTable.CALC_MGMT_LIMIT)
                            {
                                UpdateBuffer(Constants.EnvActionBuffer.INSERT_DB, Constants.TypeOfTable.CALC_MGMT_LIMIT_HUMID,
                                    dt1, calcaverage.Item2, Constants.EnvFieldNo.FLD_DATA, data);
                            }
                        }

                    }
                }
                
                #endregion
            }
            #endregion
            #region Constants.EnvType.TYPE_RS
            if (tblType == Constants.EnvType.TYPE_RS)
            {
                if (proc_type == Constants.TypeOfTable.CALC_TE83 || proc_type == Constants.TypeOfTable.CALC_TETMP_RLSPD)
                {
                    if (proc_type == Constants.TypeOfTable.CALC_TE83)
                    {
                        fieldNo =  proc_type;
                    }
                    else
                    {
                        iddb = Constants.TypeOfTable.CALC_TE83_90;
                        fieldNo = Constants.TypeOfTable.CALC_BUFFER;
                    }
                    if (s_mode.ToString("D").Equals(Constants.EnvMode.ControlLine.ToString("D")))
                    {
                        rl_ul = t_ucl;
                        rl_ll = t_lcl;
                    }
                    else
                    {
                        rl_ul = t_usl;
                        rl_ll = t_lsl;
                    }
                    rl_max = GetMax(tblType, fieldNo, iddb, id, dt1, dt2, t_dis_up, h_dis_up, data);
                    rl_min = GetMin(tblType, fieldNo, iddb, id, dt1, dt2, t_dis_lo, h_dis_lo, data);
                    var calRollspd = CalRollspd(proc_type, id, rl_ul, rl_ll, rl_max, rl_min, dt1, dt2, t_dis_up, t_dis_lo,  s_mode, data);
                    res = calRollspd.Item1;
                }else if (proc_type == Constants.TypeOfTable.CALC_TE83_90)
                {
                    var calcaverage = CalcMean(proc_type, iddb, id, dt1, dt2, t_cal_up, t_cal_lo,h_cal_up,h_cal_lo,data);
                    res = calcaverage.Item1;
                    if (calcaverage.Item1)
                    {
                        UpdateBuffer(Constants.EnvActionBuffer.INSERT_DB, Constants.TypeOfTable.CALC_TE83_90,
                            dt1, calcaverage.Item2, Constants.EnvFieldNo.FLD_DATA, data);
                    }
                    
                }
            }
            #endregion
            return res;
        }
     public Tuple<bool,double> CalcSigma(Constants.TypeOfTable calctype, Constants.TypeOfTable iddb, int id, DateTime dt1, DateTime dt2, double t_cal_up,
            double t_cal_lo,double h_cal_up,double h_cal_lo, List<Graphtbl> data)
        {
            int? cnt = null;
            double? sqr_total = 0;
            double total_sqr = 0;
            double cal_up;
            double cal_lo;

            if (calctype == Constants.TypeOfTable.CALC_TE81_TEMP)
            {
                var temdata =
                           _unitOfWork.EnvTempRepository.GetMany(
                               i =>
                                   i.F81_Id.Equals(id) && i.F81_Env_Time >= dt1 && i.F81_Env_Time <= dt2 &&
                                   i.F81_Temp <= t_cal_up && i.F81_Temp >= t_cal_lo);
                if (temdata.Any())
                {
                    cnt = temdata.Count();
                    sqr_total = temdata.Sum(x => Math.Pow(x.F81_Temp.Value,2));
                    total_sqr = Math.Pow(temdata.Sum(x => x.F81_Temp).Value,2);
                }
            }

            if (calctype == Constants.TypeOfTable.CALC_TE81_HUMID)
            {
                var temdata =
                           _unitOfWork.EnvTempRepository.GetMany(
                               i =>
                                   i.F81_Id.Equals(id) && i.F81_Env_Time >= dt1 && i.F81_Env_Time <= dt2 &&
                                   i.F81_Humidity <= h_cal_up && i.F81_Humidity >= h_cal_lo);
                if (temdata.Any())
                {
                    cnt = data.Count();
                    sqr_total = temdata.Sum(x => Math.Pow(x.F81_Humidity.Value, 2));
                    total_sqr = Math.Pow(temdata.Sum(x => x.F81_Humidity).Value, 2);
                }
            }

            if (calctype == Constants.TypeOfTable.CALC_TE83)
            {
                var temdata =
                           _unitOfWork.EnvElseRepository.GetAll().Where(
                               i =>
                                   i.F83_Id.Equals(id) && i.F83_Env_Time >= dt1 && i.F83_Env_Time <= dt2 &&
                                   i.F83_Value <= t_cal_up && i.F83_Value >= t_cal_lo);
                if (temdata.Any())
                {
                    cnt = data.Count();
                    sqr_total = temdata.Sum(x => Math.Pow(x.F83_Value.Value, 2));
                    total_sqr = Math.Pow(temdata.Sum(x => x.F83_Value).Value, 2);

                }
            }

            if (calctype == Constants.TypeOfTable.CALC_TETMP_TEMP || calctype == Constants.TypeOfTable.CALC_TETMP_HUMID ||
                calctype == Constants.TypeOfTable.CALC_TETMP_RLSPD)
            {
                if (calctype == Constants.TypeOfTable.CALC_TETMP_TEMP || calctype == Constants.TypeOfTable.CALC_TETMP_RLSPD)
                {
                    cal_up = t_cal_up;
                    cal_lo = t_cal_lo;

                }
                if (calctype == Constants.TypeOfTable.CALC_TETMP_HUMID)
                {
                    cal_up = h_cal_up;
                    cal_lo = h_cal_lo;
                }
                //TODO:
    //            trans_TmpTbl.AutoCommit = TRUE;
    //            s_sql = "SELECT Count(" + S_FLD_DATA + "), SUM(POWER(" + S_FLD_DATA + ",2)), POWER(SUM(" + S_FLD_DATA + "), 2) FROM " + TMP_TBL + &
    //                    " WHERE " + S_FLD_ID + " = ? AND " + S_FLD_DTM + " >= ? AND " + &
    //                    S_FLD_DTM + " < ? AND " + S_FLD_DATA + " <= ? AND " + S_FLD_DATA + " >= ? ";
    //            PREPARE SQLSA FROM: s_sql USING trans_TmpTbl;
    //            DESCRIBE SQLSA INTO SQLDA;
    //            DECLARE cur_sigma DYNAMIC CURSOR FOR SQLSA;
    //            SetDynamicParm(SQLDA, 1, id);
    //            SetDynamicParm(SQLDA, 2, dt1);
    //            SetDynamicParm(SQLDA, 3, dt2);
    //            SetDynamicParm(SQLDA, 4, cal_up);
    //            SetDynamicParm(SQLDA, 5, cal_lo);

    //            OPEN DYNAMIC cur_sigma USING DESCRIPTOR SQLDA;
    //            if trans_TmpTbl.sqlcode <> 0 then
    //    MessageBox("Dynamic SQL Error", trans_TmpTbl.sqlerrtext, StopSign!);
    //            rollback using trans_TmpTbl;
    //            return FAILURE;
    //            end if
            
    //            FETCH cur_sigma USING DESCRIPTOR SQLDA;
    //            if trans_TmpTbl.sqlcode <> 0 then
    //    MessageBox("Dynamic SQL Error", trans_TmpTbl.sqlerrtext, StopSign!);
    //            CLOSE cur_sigma;
    //            rollback using trans_TmpTbl;
    //            return FAILURE;
    //            end if
            
    //            cnt = GetDynamicNumber(SQLDA, 1);
    //            sqr_total = GetDynamicNumber(SQLDA, 2);
    //            total_sqr = GetDynamicNumber(SQLDA, 3);
    //            CLOSE cur_sigma;
    //            trans_TmpTbl.AutoCommit = FALSE;
    //            if cnt > 1 then
    //                r_tmp = ((sqr_total) - (total_sqr / cnt)) / (cnt - 1);
    //            r_sigma = sqrt(r_tmp);
    //else
    //    r_sigma = 0;
    //            end if
    //            return SUCCESS;

            }

            if (calctype == Constants.TypeOfTable.CALC_LOT_AVG || calctype == Constants.TypeOfTable.CALC_LOT_RANGE)
            {
                 //TODO:
                sqr_total = 0;
                total_sqr = 0;
                cnt = 0;
                foreach (var item in data)
                {
                    if (calctype == Constants.TypeOfTable.CALC_LOT_AVG &&
                        item.buf_type == iddb)
                    {
                        sqr_total += Math.Pow(item.Dec_mean.Value, 2);
                        total_sqr += item.Dec_mean.Value;
                        cnt = cnt + 1;
                        

                    }
                    if (calctype == Constants.TypeOfTable.CALC_LOT_RANGE &&
                        item.buf_type == iddb)
                    {
                        sqr_total += Math.Pow(item.Dec_upper.Value, 2);
                        total_sqr += item.Dec_upper.Value;
                        cnt = cnt + 1;

                    }
                }
                total_sqr = Math.Pow(total_sqr, 2);
                if (cnt > 1)
                {
                    var r_tmp = ((sqr_total) - (total_sqr/cnt))/(cnt - 1);
                    return new Tuple<bool, double>(true, Math.Sqrt(r_tmp.Value));
                }
                else
                {
                    return new Tuple<bool, double>(true, 0);
                }
            }

            if (calctype == Constants.TypeOfTable.CALC_MGMT_LIMIT || calctype == Constants.TypeOfTable.CALC_BUFFER)
            {
                sqr_total = 0;
                total_sqr = 0;
                cnt = 0;
                if (iddb == Constants.TypeOfTable.CALC_TE81_TEMP || iddb == Constants.TypeOfTable.CALC_TE81_TEMP90 ||
                    iddb == Constants.TypeOfTable.CALC_MGMT_LIMIT_TEMP || iddb == Constants.TypeOfTable.CALC_TE83 ||
                    iddb == Constants.TypeOfTable.CALC_TETMP_RLSPD90)
                {
                    cal_up = t_cal_up;
                    cal_lo = t_cal_lo;
                }
                else
                {
                    cal_up = h_cal_up;
                    cal_lo = h_cal_lo;
                }
                foreach (var item in data)
                {
                    if (item.Dec_data >= cal_lo && item.Dec_data <= cal_up && item.buf_type == iddb)
                    {
                        sqr_total += Math.Pow(item.Dec_data.Value, 2);
                        total_sqr += item.Dec_data.Value;
                        cnt = cnt + 1;
                    }
                }
                total_sqr = Math.Pow(total_sqr, 2);
            }


            if (!cnt.HasValue)
            {
                return new Tuple<bool, double>(false, 0);
            }
            if (cnt.Value > 1)
            {
                var r_tmp = ((sqr_total) - (total_sqr/cnt))/(cnt - 1);
                // Modified by Sum (24/12)
                if (r_tmp > 0)
                {
                    return new Tuple<bool, double>(true, Math.Sqrt(r_tmp.Value));
                }
                else
                    return new Tuple<bool, double>(true, 0);
            }
            else return new Tuple<bool, double>(true, 0);
        }
        public List<TemporaryTable> UpdateTemporaryTable(List<TemporaryTable>  data,Constants.EnvActionBuffer action, DateTime dt, Constants.TypeOfTable id_calc, Constants.EnvFieldNo fieldNo, double r_data)
        {
            var dc_data = Math.Round(r_data, 3);
            if (action == Constants.EnvActionBuffer.INSERT_DB)
            {
                var newData = new TemporaryTable()
                {
                    S_FLD_DTM = dt,
                    S_FLD_ID = id_calc,
                    S_FLD_DATA = dc_data
                };
                data.Add(newData);
            }
            if (action == Constants.EnvActionBuffer.UPDATE_DB)
            {
                var updata = data.FirstOrDefault(x => x.S_FLD_ID == id_calc);
                if (updata != null)
                {
                    if (fieldNo == Constants.EnvFieldNo.FLD_DATA)
                    {
                        updata.S_FLD_DATA = dc_data;
                    }
                    if (fieldNo == Constants.EnvFieldNo.FLD_LOWER)
                    {
                        updata.S_FLD_LOWER = dc_data;
                    }
                    if (fieldNo == Constants.EnvFieldNo.FLD_MEAN)
                    {
                        updata.S_FLD_MEAN = dc_data;
                    }
                    if (fieldNo == Constants.EnvFieldNo.FLD_UPPER)
                    {
                        updata.S_FLD_UPPER = dc_data;
                    }
                }
            }
            if (action == Constants.EnvActionBuffer.DELETE_DB)
            {
                data = null;
            }

            if (action == Constants.EnvActionBuffer.UPDATE_DB_REC)
            {
                var updata = data.FirstOrDefault(x => x.S_FLD_ID == id_calc);
                if (updata != null)
                {
                    if (fieldNo == Constants.EnvFieldNo.FLD_DATA)
                    {
                        updata.S_FLD_DATA = dc_data;
                    }
                    if (fieldNo == Constants.EnvFieldNo.FLD_LOWER)
                    {
                        updata.S_FLD_LOWER = dc_data;
                    }
                    if (fieldNo == Constants.EnvFieldNo.FLD_MEAN)
                    {
                        updata.S_FLD_MEAN = dc_data;
                    }
                    if (fieldNo == Constants.EnvFieldNo.FLD_UPPER)
                    {
                        updata.S_FLD_UPPER = dc_data;
                    }
                    updata.S_FLD_DTM = dt;
                }
            }
            return data;
        }
        public double CalcProdlot(string prod_code, Constants.TypeOfTable tmptbl_id, ref DateTime dtm_earliest, ref DateTime dtm_latest,
           ref List<Graphtbl> temperatureItems)
        {
            double? lot_max, lot_min;
            int lot_id, res = 0;
            var lstTe84 = _unitOfWork.EnvLotRepository.GetMany(i => i.F84_ProductCode == prod_code);
            if (!lstTe84.Any())
            {
                return -1;
            }
            var preLotNo = "";
            lot_id = 0;
            foreach (var te84 in lstTe84)
            {
                if (te84.F84_S_Time < dtm_earliest)
                {
                    dtm_earliest = te84.F84_S_Time;
                }
                if (te84.F84_S_Time > dtm_latest)
                {
                    dtm_latest = te84.F84_S_Time;
                }
                if (preLotNo == te84.F84_ProductLotNo)
                {
                    continue;
                }
                preLotNo = te84.F84_ProductLotNo;
                var graph_tbl = new Graphtbl();
                graph_tbl.Dec_mean =
                    lstTe84.Where(i => i.F84_ProductLotNo == te84.F84_ProductLotNo).Average(i => i.F84_Temp) ?? 0;
                lot_max = lstTe84.Where(i => i.F84_ProductLotNo == te84.F84_ProductLotNo).Max(i => i.F84_Temp);
                lot_min = lstTe84.Where(i => i.F84_ProductLotNo == te84.F84_ProductLotNo).Min(i => i.F84_Temp);
                graph_tbl.Dec_upper = (lot_max - lot_min) ?? 0;
                lot_id++;

                graph_tbl.Dec_data = lot_id;
                graph_tbl.dt_dtm = te84.F84_S_Time;
                graph_tbl.buf_type = tmptbl_id;

                graph_tbl.Lot = preLotNo;
                temperatureItems.Add(graph_tbl);

            }
            return lot_id;

        }

        public int CalcCp(Constants.TypeOfTable id_calc, double r_mean, double r_sigma, double r_range, double r_usl, double r_lsl,
            ref double r_cp, ref double r_cpk, bool b_TEMPUSL_NULL, bool b_HUMIDUSL_NULL, double t_lcl, double t_ucl,
            double h_lcl = 0.0, double h_ucl = 0.0)
        {
            if (r_sigma == 0)
            {
                r_cp = 0;
                r_cpk = 0;
                return 0;
            }
            double r_lcl, r_ucl, k;
            if (id_calc == Constants.TypeOfTable.CALC_TETMP_TEMP90 && b_TEMPUSL_NULL)
            {
                r_cp = (r_usl - r_mean) / (3 * r_sigma);
                r_lcl = t_lcl;
                r_ucl = t_ucl;
                k = Math.Abs(((r_usl + r_lcl) / 2) - r_mean) / ((r_ucl - r_lcl) / 2);
            }
            if (id_calc == Constants.TypeOfTable.CALC_TETMP_HUMID90 && b_HUMIDUSL_NULL)
            {
                r_cp = (r_usl - r_mean) / (3 * r_sigma);
                r_lcl = h_lcl;
                r_ucl = h_ucl;
                k = Math.Abs(((r_usl + r_lcl) / 2) - r_mean) / ((r_ucl - r_lcl) / 2);
            }
            if (id_calc == Constants.TypeOfTable.CALC_TETMP_RLSPD && b_TEMPUSL_NULL)
            {
                r_cp = (r_usl - r_mean) / (3 * r_sigma);
                r_lcl = t_lcl;
                r_ucl = t_ucl;
                k = Math.Abs(((r_usl + r_lcl) / 2) - r_mean) / ((r_ucl - r_lcl) / 2);
            }
            else
            {
                r_cp = r_range / (6 * r_sigma);
                k = Math.Abs(((r_usl + r_lsl) / 2) - r_mean) / ((r_usl - r_lsl) / 2);
            }
            r_cpk = (1 - k) * r_cp;
            return 0;
        }
        public Tuple<List<double>, List<double>, List<double>, List<double>,List<string>> GrapData(Constants.TypeOfTable iddb, List<Graphtbl> data)
        {
            var temdata = data.Where(x => x.buf_type == iddb).ToList();
            for (int i = 0; i < temdata.Count()-1; i++)
            {
                if (temdata[i].Dec_data.HasValue && !temdata[i + 1].Dec_data.HasValue)
                {
                    temdata[i + 1].Dec_data = temdata[i].Dec_data;
                }
                if (temdata[i].Dec_lower.HasValue && !temdata[i + 1].Dec_lower.HasValue)
                {
                    temdata[i + 1].Dec_lower = temdata[i].Dec_lower;
                }
                if (temdata[i].Dec_mean.HasValue && !temdata[i + 1].Dec_mean.HasValue)
                {
                    temdata[i + 1].Dec_mean = temdata[i].Dec_mean;
                }
                if (temdata[i].Dec_upper.HasValue && !temdata[i + 1].Dec_upper.HasValue)
                {
                    temdata[i + 1].Dec_upper = temdata[i].Dec_upper;
                }
            }
            if (iddb == Constants.TypeOfTable.CALC_TETMP_RLSPD90)
            {
                return
                new Tuple<List<double>, List<double>, List<double>, List<double>, List<string>>(
                    temdata.Select(x => x.Dec_data ?? 0).ToList(),
                    temdata.Select(x => x.Dec_mean ?? 0).ToList(),
                    temdata.Select(x => x.Dec_upper ?? 0).ToList(),
                    temdata.Select(x => x.Dec_lower ?? 0).ToList(),
                    temdata.Select(x => x.dt_dtm.ToString("dd/MM/yyyy")).ToList()
                    );
            }else return
                new Tuple<List<double>, List<double>, List<double>, List<double>, List<string>>(
                    temdata.Select(x => x.Dec_data ?? 0).ToList(), 
                    temdata.Select(x => x.Dec_mean ?? 0).ToList(),
                    temdata.Select(x => x.Dec_upper ?? 0).ToList(),
                    temdata.Select(x => x.Dec_lower ?? 0).ToList(), 
                    temdata.Select(x => x.dt_dtm.ToString("HH:mm")).ToList()
                    ); 
        }

        public void SetValueConfigFromTe80(Te80_Env_Mesp envMespData,
            out double t_ucl,
            out double t_lcl,
            out double t_usl,
            out double t_lsl,
            out double t_dis_up,
            out double h_dis_up,
            out double t_dis_lo,
            out double h_dis_lo,
            out double h_ucl,
            out double h_lcl,
            out double h_usl,
            out double h_lsl,
            out double t_cal_up,
            out double t_cal_lo,
            out double h_cal_up,
            out double h_cal_lo)
        {
            t_ucl = envMespData.F80_T_Ucl.HasValue ? envMespData.F80_T_Ucl.Value : 0;
            t_lcl = envMespData.F80_T_Lcl.HasValue ? envMespData.F80_T_Lcl.Value : 0;
            t_usl = envMespData.F80_T_Usl.HasValue ? envMespData.F80_T_Usl.Value : 0;
            t_lsl = envMespData.F80_T_Lsl.HasValue ? envMespData.F80_T_Lsl.Value : 0;
            t_dis_up = envMespData.F80_T_Dis_Up.HasValue ? envMespData.F80_T_Dis_Up.Value : 0;
            h_dis_up = envMespData.F80_H_Dis_Up.HasValue ? envMespData.F80_H_Dis_Up.Value : 0;
            t_dis_lo = envMespData.F80_T_Dis_Lo.HasValue ? envMespData.F80_T_Dis_Lo.Value : 0;
            h_dis_lo = envMespData.F80_H_Dis_Lo.HasValue ? envMespData.F80_H_Dis_Lo.Value : 0;
            h_ucl = envMespData.F80_H_Ucl.HasValue ? envMespData.F80_H_Ucl.Value : 0;
            h_lcl = envMespData.F80_H_Lcl.HasValue ? envMespData.F80_H_Lcl.Value : 0;
            h_usl = envMespData.F80_H_Usl.HasValue ? envMespData.F80_H_Usl.Value : 0;
            h_lsl = envMespData.F80_H_Lsl.HasValue ? envMespData.F80_H_Lsl.Value : 0;
            t_cal_up = envMespData.F80_T_Cal_Up.HasValue ? envMespData.F80_T_Cal_Up.Value : 0;
            t_cal_lo = envMespData.F80_T_Cal_Lo.HasValue ? envMespData.F80_T_Cal_Lo.Value : 0;
            h_cal_up = envMespData.F80_H_Cal_Up.HasValue ? envMespData.F80_H_Cal_Up.Value : 0;
            h_cal_lo = envMespData.F80_H_Cal_Lo.HasValue ? envMespData.F80_H_Cal_Lo.Value : 0;
        }


        public bool CalcAval(int id, DateTime dt1, DateTime dt2, List<Graphtbl> data)
        {
            double const1 = 0;
            double const2 = 0;
            double const3 = 0;
            double id_type;
            string s_ser1 = "Rotating with Load";
            string s_ser2 = "Empty Load";
            string s_ser3 = "Stop";
            string s_ser4 = "Not Avai";
            string col = string.Empty;
            int line;
            if (id == 1 || id == 3)
            {
                if (id == 3)
                {
                    id_type = id - 2;
                }
                else
                {
                    id_type = id;
                }
                line = 2;
                const1 = -0.1;
                const2 = -0.2;
                const3 = -0.3;

            }else if (id >= 10)
            {
                id = id - 10;
                id_type = 1.5;
                line = 3;
                const1 = -0.5;
                const2 = -1.0;
            }
            else
            {
                if (id > 3) {
                  id_type = id - 2;
                }else id_type = id;
                line = 2 ;
                const1 = -0.1;
                const2 = -0.2;
                const3 = -0.3;
                s_ser1 = s_ser1 + " " + id.ToString("##");
                s_ser2 = s_ser2 + " " + id.ToString("##");
                s_ser3 = s_ser3 + " " + id.ToString("##");
                s_ser4 = s_ser4 + " " + id.ToString("##");

            }
            var datatem =_unitOfWork.EnvAvalRepository.GetMany(
                                           i =>
                                               i.F82_Id.Equals(id) && i.F82_Env_Time >= dt1 && i.F82_Env_Time <= dt2);
            var t = datatem.ToList();
            double val = 0;
            string ser = string.Empty;
            foreach (var item in datatem)
            {
                if (item.F82_Status.Equals("O"))
                {
                    val = id_type;
                    ser = s_ser1;
                    col = "RGB(189,189,189)";
                }
                if (item.F82_Status.Equals("A"))
                {
                    val = id_type + const1;
                    ser = s_ser2;
                    col = "RGB(21, 115,168)";
                }
                if (item.F82_Status.Equals("S"))
                {
                    val = id_type + const2;
                    ser = s_ser3;
                    col = "RGB(255,0,0)";
                }
                if (item.F82_Status.Equals("E"))
                {
                    val = id_type + const3;
                    ser = s_ser4;
                    col = "RGB(255,0,255)";
                }
                data.Add(new Graphtbl()
                {
                    dt_dtm = item.F82_Env_Time,
                    time = item.F82_Env_Time.ToString("yyyy-MM-dd HH:mm"),
                    rot_status = item.F82_Status,
                    val = val,
                    Ser = ser,
                    col = col
                });
            }
            return true;
        }

        public double Countt82Status(int id, string status, DateTime dt1, DateTime dt2)
        {
            if (!string.IsNullOrEmpty(status))
            {
                var datatem = _unitOfWork.EnvAvalRepository.GetMany(
                    i =>    i.F82_Id == id &&
                            i.F82_Status.Equals(status) && i.F82_Env_Time >= dt1 && i.F82_Env_Time < dt2);
                return datatem.Count();
            }
            else
            {
                var datatem = _unitOfWork.EnvAvalRepository.GetMany(
                                           i =>
                                               i.F82_Id == id &&
                                               i.F82_Env_Time >= dt1 && i.F82_Env_Time < dt2);
                return datatem.Count();
            }
        }



        public void EditTe81EnvTempTable(string locationId, string environmentDate, string time, double temperature, double humidity)
        {
            DateTime newTime = DateTime.Parse(time,
                System.Globalization.CultureInfo.CurrentCulture);

            DateTime timeEnd = DateTime.Parse("08:00",
                System.Globalization.CultureInfo.CurrentCulture);
            DateTime timeStart = DateTime.Parse("00:00", 
                System.Globalization.CultureInfo.CurrentCulture);

            var stringF81EnvTime = environmentDate + ' ' + newTime.ToString("hh:mm tt");
            var f81EnvTime = ConvertHelper.ConvertToDateTimeFullHourMinute(stringF81EnvTime);

            if (newTime >= timeStart && newTime < timeEnd)
            {
                f81EnvTime = f81EnvTime.AddDays(1);
            }
            var envTimeToStart = f81EnvTime.AddSeconds(0);
            var envTimeToEnd = f81EnvTime.AddSeconds(60);

            var id = Int32.Parse(locationId);
            var f81EvnTemp =
                _unitOfWork.EnvTempRepository.Get(i => i.F81_Env_Time >= envTimeToStart && i.F81_Env_Time<envTimeToEnd && i.F81_Id.Equals(id));

            f81EvnTemp.F81_Temp = temperature;
            f81EvnTemp.F81_Humidity = humidity;

            _unitOfWork.EnvTempRepository.Update(f81EvnTemp);
            _unitOfWork.Commit();

        }


        public Tuple<int, Te81_Env_Temp> GetEnvTempData(string time, string environmentDate,string locationName)
        {
            var te80EnvMesp =
                _unitOfWork.EnvMespRepository.GetMany(
                    i => i.F80_Name.Trim().Equals(locationName.Trim()) && i.F80_Type.Equals("1")).FirstOrDefault();

            DateTime newTime = DateTime.Parse(time,
                System.Globalization.CultureInfo.CurrentCulture);

            DateTime timeEnd = DateTime.Parse("08:00",
                System.Globalization.CultureInfo.CurrentCulture);
            DateTime timeStart = DateTime.Parse("00:00",
                System.Globalization.CultureInfo.CurrentCulture);

            var stringF81EnvTime = environmentDate + ' ' + newTime.ToString("hh:mm tt");
            var f81EnvTime = ConvertHelper.ConvertToDateTimeFullHourMinute(stringF81EnvTime);

            if (newTime >= timeStart && newTime < timeEnd)
            {
                f81EnvTime = f81EnvTime.AddDays(1);
            }
            var envTimeToStart = f81EnvTime.AddSeconds(0);
            var envTimeToEnd = f81EnvTime.AddSeconds(60);

            var f81EnvTemp = _unitOfWork.EnvTempRepository.GetMany(i => i.F81_Env_Time < envTimeToEnd && i.F81_Env_Time >= envTimeToStart);
            if (!f81EnvTemp.Any())
                return new Tuple<int, Te81_Env_Temp>(Convert.ToInt32(te80EnvMesp.F80_Humidity),null);
            
            return new Tuple<int, Te81_Env_Temp>(Convert.ToInt32(te80EnvMesp.F80_Humidity), f81EnvTemp.FirstOrDefault());
        }

        public void UpdateTe80(Te80_Env_Mesp data)
        {
            var te80 = _unitOfWork.EnvMespRepository.Get(x => x.F80_Id == data.F80_Id);
            if (te80 != null)
            {
                te80.F80_T_Mean = data.F80_T_Mean;
                te80.F80_T_Sigma = data.F80_T_Sigma;
                te80.F80_T_Range = data.F80_T_Range;
                te80.F80_T_Ucl = data.F80_T_Ucl;
                te80.F80_T_Lcl = data.F80_T_Lcl;
                te80.F80_T_Cp = data.F80_T_Cp;
                te80.F80_T_Cpk = data.F80_T_Cpk;

                te80.F80_H_Mean = data.F80_H_Mean;
                te80.F80_H_Sigma = data.F80_H_Sigma;
                te80.F80_H_Range = data.F80_H_Range;
                te80.F80_H_Ucl = data.F80_H_Ucl;
                te80.F80_H_Lcl = data.F80_H_Lcl;
                te80.F80_H_Cp = data.F80_H_Cp;
                te80.F80_H_Cpk = data.F80_H_Cpk;

                te80.F80_D_From = data.F80_D_From;
                te80.F80_D_To = data.F80_D_To;
                _unitOfWork.EnvMespRepository.Update(te80);
                _unitOfWork.Commit();
            }
        }

    }
}  