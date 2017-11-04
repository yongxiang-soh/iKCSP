using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels.AvailabilityDataEdit;
using KCSG.Web.Areas.EnvironmentManagement.ViewModels.AvailabilityMixingRollMachine;
using KCSG.Web.Attributes;
using Newtonsoft.Json;
using Resources;

namespace KCSG.Web.Areas.EnvironmentManagement.Controllers
{
    [MvcAuthorize("TCEN073F")]
    public class AvailabilityDataEditController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        private readonly IEnvironmentBaseDomain _environmentBaseDomain;
        private readonly IAvailabityDataEditDomain _availabityDataEditDomain;

        #endregion

        #region Constructor

        public AvailabilityDataEditController(IEnvironmentBaseDomain environmentBaseDomain,
            IAvailabityDataEditDomain availabityDataEditDomain)
        {
            _environmentBaseDomain = environmentBaseDomain;
            _availabityDataEditDomain = availabityDataEditDomain;
        }

        #endregion

        //
        // GET: /EnvironmentManagement/AvailabilityDataEdit/
        public ActionResult Index(string environmentDate = "", Constants.EnvMode envMode = Constants.EnvMode.ControlLine,
            string machine = "")
        {
            ViewBag.ListLocation = _environmentBaseDomain.GetLocationItemByType("2").Select(x => new SelectListItem
            {
                Text = x.F80_Name,
                Value = string.Format("{0}~{1}",x.F80_Id.ToString(),x.F80_Name.Trim()),
                Selected = string.Format("{0}~{1}", x.F80_Id.ToString(), x.F80_Name.Trim()).Equals(machine.Trim())
            });

            DateTime dt1;
            if (DateTime.TryParseExact(environmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out dt1))
            {
                var model = new AvailabilityDataEditViewModel()
                {
                    EnvironmentDate = dt1.ToString("dd/MM/yyyy"),
                    EnvMode = envMode,
                    //Machine = machine

                };
                return View(model);
            }
            else
            {
                var model = new AvailabilityDataEditViewModel()
                {
                    EnvironmentDate = DateTime.Now.ToString("dd/MM/yyyy"),
                    EnvMode = envMode
                };
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Search(AvailabilityDataEditViewModel model)
        {
            var locationValue = model.Machine.Split('~')[0];
            var id = Convert.ToInt32(locationValue);

            var dt1 = DateTime.ParseExact(model.EnvironmentDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var dt2 = dt1.AddDays(1);
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

            //if (ModelState.IsValid)
            //{
            //string avai3 = String.Empty;

            double sumAvai = 0;
            var grapdata = new List<Graphtbl>();

            var dtm1 = dt1.AddHours(8);
            var dtm2 = dt1.AddDays(1).AddHours(8);
            _environmentBaseDomain.CalcAval(id + 10, dtm1, dtm2, grapdata);

            var count = _environmentBaseDomain.Countt82Status(id, "O", dtm1, dtm2);
            var total = _environmentBaseDomain.Countt82Status(id, string.Empty, dtm1, dtm2);

            if (total != 0)
            {
                sumAvai = (double)count/total;
            }
            var sersGraph = grapdata.GroupBy(x => x.Ser).Select(grp => grp.ToList()).ToList();
            return Json(new
            {
                Success = true,
                sumAvai = sumAvai.ToString("####0.00%"),
                grapdata = JsonConvert.SerializeObject(sersGraph),
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(string status, string environmentTime, string time, string machineValue)
        {
            try
            {
                var id = Convert.ToInt32(machineValue.Split('~')[0]);

                var result = _availabityDataEditDomain.GetStatusInTe82_Env_Aval(id, environmentTime, time);
                if(result==null)
                    return Json(new{Success=false});

                _availabityDataEditDomain.Edit(status, environmentTime, time, id);
                return Json(new {Success = true, Message = EnvironmentResource.MSG9});
            }
            catch (Exception e)
            {
                return Json(new {Success = false, Message = e.Message});
            }
        }

        [HttpPost]
        public ActionResult GetStatus(string machineValue, string environmentDate, string time)
        {
            var id = Convert.ToInt32(machineValue.Split('~')[0]);

            var result = _availabityDataEditDomain.GetStatusInTe82_Env_Aval(id, environmentDate, time);
            return Json(new
            {
                result
            });
        }
    }
}