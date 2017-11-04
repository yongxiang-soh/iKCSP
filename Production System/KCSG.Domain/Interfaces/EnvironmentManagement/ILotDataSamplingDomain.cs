using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.EnvironmentManagement
{
     public  interface ILotDataSamplingDomain
     {
         SelectList GetProduct();
         SelectList GetLotNo(string productCode,bool newCheckLot);

         ResponseResult<LotdataSamplingItem> Search(string productCode, DateTime date, Constants.EnvMode mode,
             string lotNo);

         ResponseResult Add(string productCode, string lotNo, DateTime date, TimeSpan time,double? temp);
         bool Delete(int id);
         ResponseResult<GridResponse<Te84_Env_Lot>> GetTable(string productCode, GridSettings settings);
         string GetDate(string lotNo);
         ResponseResult GetValueWithTime(string time, string productCode, string date);
         IList<Te84_Env_Lot> GetTe84EnvLot(string productCode, string productLotNo, string date);
     }
}
