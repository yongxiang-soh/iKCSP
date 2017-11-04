using System;using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels.AvailabilityMixingRollMachine;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels.EnvironmentDataDuration;
using KCSG.Web.Attributes;
using Newtonsoft.Json;

namespace KCSG.Web.Areas.EnvironmentManagement.Controllers
{
     [MvcAuthorize("TCEN021F")]
    public class AvailabilityMixingRollMachineController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        private IEnvironmentBaseDomain _environmentBaseDomain;

        #endregion

        public AvailabilityMixingRollMachineController(IEnvironmentBaseDomain environmentBaseDomain)
        {
            this._environmentBaseDomain = environmentBaseDomain;
        }
        //
        // GET: /EnvironmentManagement/EnvironmentDataDuration/
       
        public ActionResult Index()
        {
            var today = DateTime.Now;
            var model = new AvailabilityMixingRollMachineViewModel()
            {
                EndDate = today.AddDays(-1).ToString("dd/MM/yyyy"),
                StartDate = today.AddDays(-89).ToString("dd/MM/yyyy"),
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult Search(AvailabilityMixingRollMachineViewModel model)
        {
            var dt1 = DateTime.ParseExact(model.StartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var dt2 = DateTime.ParseExact(model.EndDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            if (dt2 > DateTime.Now)
            {
                return Json(new
                {
                    Success = false,
                    ErrorCode = -2,
                    Message = "The End Date is invalid."
                }, JsonRequestBehavior.AllowGet);
            }

            if ((dt2 - dt1).Days >= 90)
            {
                return Json(new
                {
                    Success = false,
                    ErrorCode = -2,
                    Message = "The different of dates is more than 90 days!"
                }, JsonRequestBehavior.AllowGet);
            }

            if (dt2 < dt1)
            {
                return Json(new
                {
                    Success = false,
                    ErrorCode = -2,
                    Message = "The Start date is larger than the End date!"
                }, JsonRequestBehavior.AllowGet);
            }
            
            if (ModelState.IsValid)
            {
                var dtm1 = dt1.AddHours(8);
                var dtm2 = dt1.AddDays(1).AddHours(8);
                var grapdata = new List<Graphtbl>();
                _environmentBaseDomain.CalcAval(1, dtm1, dtm2, grapdata);
                _environmentBaseDomain.CalcAval(2, dtm1, dtm2, grapdata);
                dtm2 = dt2.AddDays(1).AddHours(8);
                var count1 = _environmentBaseDomain.Countt82Status(1,"O", dtm1, dtm2);
                double total = 0;
                var total1 = _environmentBaseDomain.Countt82Status(1,string.Empty, dtm1, dtm2);
                var avai1tex = "0.00%";
                if (total1 != 0)
                {
                    avai1tex = (count1 / total1).ToString("####0.00%");
                    total += count1 / total1;
                }
                var count2 = _environmentBaseDomain.Countt82Status(2, "O", dtm1, dtm2);
                var total2 = _environmentBaseDomain.Countt82Status(2, string.Empty, dtm1, dtm2);
                var avai2tex = "0.00%";
                if (total2 != 0)
                {
                    avai2tex = (count2 / total2).ToString("####0.00%");
                    total += count2/total2;
                }
                var sersGraph = grapdata.GroupBy(x => x.Ser).Select(grp => grp.ToList()).ToList();
                return Json(new
                {
                    Success = true,
                    grapdata = JsonConvert.SerializeObject(sersGraph),
                    avai1tex = avai1tex,
                    avai2tex = avai2tex,
                    avaitotaltext = total.ToString("####0.00%"),
                }, JsonRequestBehavior.AllowGet);
                
            }
            return Json(new
            {
                Success = false,
                ErrorCode = -2,
                Message = "Test"
            }, JsonRequestBehavior.AllowGet);

        }
    }
}