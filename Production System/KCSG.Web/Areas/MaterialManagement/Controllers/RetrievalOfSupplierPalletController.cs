using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Web.Areas.Inquiry.ViewModels.InquiryByMaterialName;
using KCSG.Web.Areas.MaterialManagement.ViewModels.RetrievalOfSupplierPallet;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.Controllers
{
    [MvcAuthorize("TCRM131F")]
    public class RetrievalOfSupplierPalletController : KCSG.Web.Controllers.BaseController
    {
        #region Constructors

        /// <summary>
        ///     Initialize controller with dependency injections.
        /// </summary>
        /// <param name="retrieveSupplierPalletDomain"></param>
        /// <param name="identityService"></param>
        public RetrievalOfSupplierPalletController(IRetrieveSupplierPalletDomain retrieveSupplierPalletDomain,
            IIdentityService identityService, IStorageOfWarehousePalletDomain storageOfWarehousePalletDomain)
        {
            _retrieveSupplierPalletDomain = retrieveSupplierPalletDomain;
            _storageOfWarehousePalletDomain = storageOfWarehousePalletDomain;
            _identityDomain = identityService;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Domain which is used for handling material rejection/acceptance tasks.
        /// </summary>
        private readonly IRetrieveSupplierPalletDomain _retrieveSupplierPalletDomain;

        private readonly IStorageOfWarehousePalletDomain _storageOfWarehousePalletDomain;

        private readonly IIdentityService _identityDomain;

        #endregion

        #region Public Methods

        /// <summary>
        ///     This function is for rendering index view.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.ScreenId = Constants.PictureNo.TCRM131F;
            return View();
            //ViewBag.ScreenId = "TCRM131F";
            //var model = new RetrievalOfSupplierPalletViewModel()
            //{
            //    //PossibleRetrievalQuantity = 0,
            //    //PossibleRetrievalQuantityOfPallet = 0

            //};
            //return View();
        }

        /// <summary>
        ///     Retrieval of Supplier Pallet function
        ///     This function is fired when Retrieve button is clicked on client-side.
        /// </summary>
        /// <param name="retrievalOfSupplierPalletViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Retrieve(RetrievalOfSupplierPalletViewModel retrievalOfSupplierPalletViewModel)
        {
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var checkConveyorStatus = _storageOfWarehousePalletDomain.CheckedRecordInTM05(terminalNo);
            if (!checkConveyorStatus)
                return Json(new { Success = false, Message = MaterialResource.MSG15 });

            // Check whether device is valid or not.
            var isValidDevice = _retrieveSupplierPalletDomain.IsValidDevice();
            if (!isValidDevice)
            {
                // The message can be confused with the validation error messages because of status 400.
                // Use this custom header to clarify it.
                return Json(new { Success = false, Message = MaterialResource.MSG16 });
            }

            // As the parameters are valid. Analyze data and do Retrieval of Supplier Pallet function.
            _retrieveSupplierPalletDomain.ProcessSupplierPallet(retrievalOfSupplierPalletViewModel.SupplierCode,
                retrievalOfSupplierPalletViewModel.RequestedRetrievalQuantity, terminalNo);

            // Nothing is wrong, tell the client the result is successful.
             return Json(new { Success = true, Message = MaterialResource.MSG16 });
        }

        /// <summary>
        ///     Find validation errors and construct 'em as json object.
        /// </summary>
        /// <param name="modelStateDictionary"></param>
        /// <returns></returns>
        private IDictionary<string, string[]> FindValidationErrors(ModelStateDictionary modelStateDictionary)
        {
            return ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        /// <summary>
        /// Find Possible Retrieval Quantity
        /// </summary>
        /// <param name="supplierCode"></param>
        /// <returns></returns>
        public ActionResult FindPossibleRetrievalQuantity(string supplierCode)
        {
            var possibleRetrievalQuantity = _retrieveSupplierPalletDomain.FindPossibleRetrievalQuantity(supplierCode);
            var numberOfRecord = _retrieveSupplierPalletDomain.FindNumberOfMaterialShelfStatusRecord(supplierCode);
            return Json(new
            {
                possibleRetrievalQuantity,
                numberOfRecord
            });
        }

        /// <summary>
        /// Update product command.
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateProductCommand()
        {
            // Find terminal no of current user.
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);

            // Change statuses in database and receive items which have been changed.
            var items = _retrieveSupplierPalletDomain.UpdateProductCommand(terminalNo);

            return Json(items);
        }
        #endregion
    }
}