using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Data.DataModel;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ManagementReport;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Areas.ManagementReport.ViewModels.MaterialStock;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.ManagementReport.Controllers
{
    [MvcAuthorize("TCSS017F")]
    public class PreProductStockController : KCSG.Web.Controllers.BaseController
    {

        #region Constructors
        /// <summary>
        ///     Initiate controller with dependency injections.
        /// </summary>
        /// <param name="inputOfKneadingCommandDomain"></param>
        /// <param name="exportReportDomain"></param>
        public PreProductStockController(IMaterialStockDomain MaterialStockDomain,
            IExportReportDomain exportReportDomain)
        {
            _MaterialStockDomain = MaterialStockDomain;
            _exportReportDomain = exportReportDomain;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Domain which handles input of kneading command function businesses
        /// </summary>
        private readonly IMaterialStockDomain _MaterialStockDomain;

        /// <summary>
        ///     Domain which provides functions to export reports.
        /// </summary>
        private readonly IExportReportDomain _exportReportDomain;

        #endregion

        // GET: Inquiry/MaterialStock
        public ActionResult Index()
        {
            ViewBag.ScreenId = "TCFC084P";
            var model = new MaterialStockSearchViewModel()
            {
                DateTime = DateTime.Now.ToString("G")
            };
            return View(model);
        }



    }
}