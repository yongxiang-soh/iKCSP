using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels;
using KCSG.Web.Attributes;

namespace KCSG.Web.Areas.EnvironmentManagement.Controllers
{
    [MvcAuthorize("TCEN031F")]
    public class CleanlinessDataInputController : KCSG.Web.Controllers.BaseController
    {
        private readonly ICleanlinessDataInputDomain _cleanlinessDataInputDomain;
          #region Properties

        private IEnvironmentBaseDomain _environmentBaseDomain;
        private IExportReportDomain _exportReportDomain;
        #endregion

        public CleanlinessDataInputController(IEnvironmentBaseDomain environmentBaseDomain, IExportReportDomain exportReportDomain,ICleanlinessDataInputDomain cleanlinessDataInputDomain)
        {
            _cleanlinessDataInputDomain = cleanlinessDataInputDomain;
            _exportReportDomain = exportReportDomain;
            _environmentBaseDomain = environmentBaseDomain;
        }
        //
        // GET: EnvironmentManagement/CleanlinessDataInput
        public ActionResult Index()
        {
            var today = DateTime.Now.ToString("dd/MM/yyyy");

            var model = new CleanlinessDataInputModel()
            {
                SearchCriteriaModel = new SearchCriteriaModel() { Location = "Line", StartDate = today,EndDate = today},
               
                DateDiameter1 = today.Remove(5),
                DateDiameter2 = today.Remove(5),
            };
            ViewBag.ListLocation = EnumsHelper.GetListItemsWithDescription<Constants.EnvLine>();
            return View(model);
        }
       [HttpPost]
        public ActionResult Print(double?[][] val1m05, double?[][] val1m5, double?[][] val5cm03, double?[][] val5cm05, double?[][] val5cm1, double?[][] val5cm5,string inputDate)
        {
           
               var dMean = 0.0;
                   var dSigma = 0.0;
               var dCp = 0.0;
               var conv1 = new Conv1();
               conv1.Datas = new double?[2, 10];
                conv1.Cp= new double[2];
                conv1.Mean= new double[2];
                conv1.Sigma = new double[2];
               for (int i = 0; i < 10; i++)
               {
                   conv1.Datas[0,i] = val1m05[0][i];
                   conv1.Datas[1,i] =val1m5[0][i];
               }

               for (int i = 1; i <= 2; i++)
               {
                   _cleanlinessDataInputDomain.CalculaAll(i, ref dMean, ref dSigma, ref dCp, val1m05, val1m5, val5cm03,
                       val5cm05, val5cm1, val5cm5);
                   conv1.Cp[i-1] = dCp;
                   conv1.Mean[i-1] = dMean;
                   conv1.Sigma[i-1] = dSigma;
               }  
               var mega1 = new Mega1();
               mega1.Datas = new double?[2, 10];
               mega1.Cp = new double[2];
               mega1.Mean = new double[2];
               mega1.Sigma = new double[2];
               for (int i = 0; i < 10; i++)
               {
                   mega1.Datas[0,i] = val1m05[1][i];
                   mega1.Datas[1,i] = val1m5[1][i];
               }

               for (int i = 3; i <= 4; i++)
               {
                   _cleanlinessDataInputDomain.CalculaAll(i, ref dMean, ref dSigma, ref dCp, val1m05, val1m5, val5cm03,
                       val5cm05, val5cm1, val5cm5);
                   mega1.Cp[i-3] = dCp;
                   mega1.Mean[i - 3] = dMean;
                   mega1.Sigma[i - 3] = dSigma;
               }
               var conv2 = new Conv2();
               conv2.Datas = new double?[4, 10];
               conv2.Cp = new double[4];
               conv2.Mean = new double[4];
               conv2.Sigma = new double[4];
               for (int i = 0; i < 10; i++)
               {
                   conv2.Datas[0,i] = val5cm03[0][i];
                   conv2.Datas[1,i] = val5cm05[0][i];
                   conv2.Datas[2,i] = val5cm1[0][i];
                   conv2.Datas[3,i] = val5cm5[0][i];
               }

               for (int i = 5; i <= 8; i++)
               {
                   _cleanlinessDataInputDomain.CalculaAll(i, ref dMean, ref dSigma, ref dCp, val1m05, val1m5, val5cm03,
                       val5cm05, val5cm1, val5cm5);
                   conv2.Cp[i-5] = dCp;
                   conv2.Mean[i-5] = dMean;
                   conv2.Sigma[i-5] = dSigma;
               }
               var mega2 = new Mega2();
               mega2.Datas = new double?[4, 10];
               mega2.Cp = new double[4];
               mega2.Mean = new double[4];
               mega2.Sigma = new double[4];
               for (int i = 0; i < 10; i++)
               {
                   mega2.Datas[0,i] = val5cm03[1][i];
                   mega2.Datas[1,i] = val5cm05[1][i];
                   mega2.Datas[2,i] = val5cm1[1][i];
                   mega2.Datas[3,i] = val5cm5[1][i];
               }

               for (int i = 9; i <= 12; i++)
               {
                   _cleanlinessDataInputDomain.CalculaAll(i, ref dMean, ref dSigma, ref dCp, val1m05, val1m5, val5cm03,
                       val5cm05, val5cm1, val5cm5);
                   mega2.Cp[i-9] = dCp;
                   mega2.Mean[i-9] = dMean;
                   mega2.Sigma[i-9] = dSigma;
               }
             
                 var model = new CleanlinessDataInputModel()
                    {
                        Company = WebConfigurationManager.AppSettings["CompanyName"],
                        DateReport = inputDate,
                        Conv1 = conv1,
                        Conv2 = conv2,
                        Mega1 = mega1 ,
                        Mega2 = mega2 
                    };
                    // Find the template file.
          //  var view = PartialView("CleanlinessDataInput/_partialExportCleanlinessDataInput", model);
            return PartialView("CleanlinessDataInput/_partialExportCleanlinessDataInput", model);
        }
        public ActionResult GetDataWindow1(SearchCriteriaModel model,GridSettings gridSettings)
        {
            var inputDate =!string.IsNullOrEmpty(model.StartDate)? ConvertHelper.ConvertToDateTimeFull(model.StartDate):DateTime.Now;
            var dataDate = !string.IsNullOrEmpty(model.EndDate)?ConvertHelper.ConvertToDateTimeFull(model.EndDate):DateTime.Now;
            var result = _cleanlinessDataInputDomain.SearchDataWindow1(inputDate,dataDate, model.Location, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDataWindow2(SearchCriteriaModel model, GridSettings gridSettings)
        {
            var inputDate = !string.IsNullOrEmpty(model.StartDate) ? ConvertHelper.ConvertToDateTimeFull(model.StartDate) : DateTime.Now;
            var dataDate = !string.IsNullOrEmpty(model.EndDate) ? ConvertHelper.ConvertToDateTimeFull(model.EndDate) : DateTime.Now;
            var result = _cleanlinessDataInputDomain.SearchDataWindow1(inputDate, dataDate, model.Location, gridSettings);
            if (!result.IsSuccess)
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(result.Data, JsonRequestBehavior.AllowGet);
        }
        
    }
}