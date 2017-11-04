using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.EnvironmentManagement
{
    public interface IEnvironmentBaseDomain
    {
        List<Te80_Env_Mesp> GetLocationItemByType(string type);
        Te80_Env_Mesp GetLocationItemByName(string type, int id, string name);
        Tuple<bool, string, Te80_Env_Mesp> GetMespVal(Constants.EnvType tblType, int id, Constants.EnvMode mode);

        List<TemporaryTable> UpdateTemporaryTable(List<TemporaryTable> data, Constants.EnvActionBuffer action,
            DateTime dt, Constants.TypeOfTable id_calc, Constants.EnvFieldNo fieldNo, double r_data);

        void UpdateBuffer(Constants.EnvActionBuffer action, Constants.TypeOfTable buf_type, DateTime dtmBuffer,
            double rl_data, Constants.EnvFieldNo field_no, List<Graphtbl> data);

        Tuple<bool, string> CalcuTemper(Constants.TypeOfTable proc_type, Constants.TypeOfTable? iddb, int id, double rl_upper,
            double rl_lower, double rl_max, double rl_min, DateTime dt1, DateTime dt2, double t_dis_up, double t_dis_lo,
             Constants.EnvMode mod_dt, List<Graphtbl> data);

        Tuple<bool, string> CalcuHumid(Constants.TypeOfTable proc_type, Constants.TypeOfTable? iddb, int id,
            double rl_upper, double rl_lower, double rl_max, double rl_min, DateTime dt1, DateTime dt2,
            double h_dis_up, double h_dis_lo,  Constants.EnvMode mod_dt, List<Graphtbl> data);

        double GetMax(Constants.EnvType tblType, Constants.TypeOfTable fieldNo, Constants.TypeOfTable? iddb, int id, DateTime dt1, DateTime dt2,
            double t_dis_up, double h_dis_up, List<Graphtbl> data);

        void CallLimit(double r_mean, double r_sigma, out double r_ucl, out double r_lcl);

        double GetMin(Constants.EnvType tblType, Constants.TypeOfTable fieldNo, Constants.TypeOfTable? iddb, int id, DateTime dt1, DateTime dt2,
            double t_dis_lo, double h_dis_lo, List<Graphtbl> data);

        Tuple<bool, double> CalcMean(Constants.TypeOfTable calctype, Constants.TypeOfTable? iddb, int id, DateTime dt1,
            DateTime dt2, double t_cal_up, double t_cal_lo, double h_cal_up, double h_cal_lo, List<Graphtbl> data);

        Tuple<bool, double> CalcSigma(Constants.TypeOfTable calctype, Constants.TypeOfTable iddb, int id, DateTime dt1, DateTime dt2,
            double t_cal_up,
            double t_cal_lo, double h_cal_up, double h_cal_lo, List<Graphtbl> data);

        bool CalcData(Constants.EnvType tblType, int id, Constants.TypeOfTable? bufferType, DateTime dt, Constants.TypeOfTable proc_type,
            int days_no,
            Constants.EnvMode s_mode, 
            double t_ucl, double t_lcl, double t_usl, double t_lsl,double t_dis_up,
            double h_dis_up, double t_dis_lo, double h_dis_lo, double h_ucl, double h_lcl, double h_usl, double h_lsl,
            double t_cal_up, double t_cal_lo, double h_cal_up, double h_cal_lo, bool b_DispHumidity, List<Graphtbl> data);

        Tuple<List<double>, List<double>, List<double>, List<double>, List<string>> GrapData(Constants.TypeOfTable iddb, List<Graphtbl> data);

        void SetValueConfigFromTe80(Te80_Env_Mesp envMespData, out double t_ucl, out double t_lcl, out double t_usl, out double t_lsl,
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
            out double h_cal_lo);

        int CalcCp(Constants.TypeOfTable id_calc, double r_mean, double r_sigma, double r_range, double r_usl,
            double r_lsl,
            ref double r_cp, ref double r_cpk, bool b_TEMPUSL_NULL, bool b_HUMIDUSL_NULL, double t_lcl, double t_ucl,
            double h_lcl = 0.0, double h_ucl = 0.0);

        bool CalcAval(int id, DateTime dt1, DateTime dt2, List<Graphtbl> data);
        double Countt82Status(int id,string status, DateTime dt1, DateTime dt2);

        void EditTe81EnvTempTable(string locationId, string environmentDate, string time, double temperature, double humidity);
        Tuple<int, Te81_Env_Temp> GetEnvTempData(string time, string environmentDate, string locationName);

        void UpdateTe80(Te80_Env_Mesp data);
    }
}
