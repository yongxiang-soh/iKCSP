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
     [MvcAuthorize("TCEN022F")]
    public class AvailabilityTabletisingMachineController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        private IEnvironmentBaseDomain _environmentBaseDomain;

        #endregion

        public AvailabilityTabletisingMachineController(IEnvironmentBaseDomain environmentBaseDomain)
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
                string avai3 = String.Empty;
                string avai4 = String.Empty;
                string avai5 = String.Empty;
                string avai6 = String.Empty;
                string avai7 = String.Empty;
                string avai8 = String.Empty;
                string avai9 = String.Empty;
                double sumAvai = 0;
                var grapdata = new List<Graphtbl>();
                for (var id = 3; id <= 9; id++)
                {
                    var dtm1 = dt1.AddHours(8);
                    var dtm2 = dt1.AddDays(1).AddHours(8);
                    _environmentBaseDomain.CalcAval(id, dtm1, dtm2, grapdata);
                    dtm2 = dt2.AddDays(1).AddHours(8);
                    var count = _environmentBaseDomain.Countt82Status(id, "O", dtm1, dtm2);
                    var total = _environmentBaseDomain.Countt82Status(id, string.Empty, dtm1, dtm2);
                    string avai = "0.00%";
                    if (total != 0)
                    {
                        avai = (count/total).ToString("####0.00%");
                        sumAvai += count/total;
                        
                    }
                    switch (id)
                    {
                        case 3:
                            avai3 = avai;
                            break;
                        case 4:
                            avai4 = avai;
                            break;
                        case 5:
                            avai5 = avai;
                            break;
                        case 6:
                            avai6 = avai;
                            break;
                        case 7:
                            avai7 = avai;
                            break;
                        case 8:
                            avai8 = avai;
                            break;
                        case 9:
                            avai9 = avai;
                            break;
                    }
                }
                var sersGraph = grapdata.GroupBy(x => x.Ser).Select(grp => grp.ToList()).ToList();
                return Json(new
                {
                    Success = true,
                    grapdata = JsonConvert.SerializeObject(sersGraph),
                    avai3tex = avai3,
                    avai4tex = avai4,
                    avai5tex = avai5,
                    avai6tex = avai6,
                    avai7tex = avai7,
                    avai8tex = avai8,
                    avai9tex = avai9,
                    sumAvai = sumAvai.ToString("####0.00%"),
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