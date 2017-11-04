using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Web.Areas.MaterialManagement.ViewModels.ForcedRetrievalOfRejectedMaterial;
using KCSG.Web.Attributes;
using Resources;

namespace KCSG.Web.Areas.MaterialManagement.Controllers
{
    [MvcAuthorize("TCRM151F")]
    public class ForcedRetrievalOfRejectedMaterialController : KCSG.Web.Controllers.BaseController
    {
        #region Constructors

        /// <summary>
        ///     Initialize controller with dependency injections.
        /// </summary>
        public ForcedRetrievalOfRejectedMaterialController(
            IForcedRetrievalOfRejectedMaterialDomain forcedRetrievalOfRejectedMaterialDomain,
            IIdentityService identityDomain)
        {
            _forcedRetrievalOfRejectedMaterialDomain = forcedRetrievalOfRejectedMaterialDomain;
            _identityDomain = identityDomain;
        }

        #endregion

        #region Private Methods

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

        #endregion

        #region Properties

        /// <summary>
        ///     Domain which is used for handling material rejection/acceptance tasks.
        /// </summary>
        private readonly IForcedRetrievalOfRejectedMaterialDomain _forcedRetrievalOfRejectedMaterialDomain;

        private readonly IIdentityService _identityDomain;

        #endregion

        #region Public Methods

        /// <summary>
        ///     This function is for rendering Forced Retrieval Of Rejected Material page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            var model = new AssignRejectedMaterialViewModel();
            return View(model);
        }

        /// <summary>
        ///     This function is for assigning rejected material.
        /// </summary>
        /// <returns></returns>
        /// <param name="assignRejectedMaterialViewModel">
        ///     Material information which is used for filtering materials to be
        ///     accepted.
        /// </param>
        [HttpPost]
        public ActionResult Assign(AssignRejectedMaterialViewModel assignRejectedMaterialViewModel)
        {
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Json(FindValidationErrors(ModelState));
            }

            // Do material assignment task by calling function from domain.
            var materialAssignmentTaskResult = _forcedRetrievalOfRejectedMaterialDomain.AsignRejectedMaterial(
                assignRejectedMaterialViewModel.ProductOrderNumber, assignRejectedMaterialViewModel.PartialDelivery,
                assignRejectedMaterialViewModel.MaterialCode,terminalNo);

            return Json(materialAssignmentTaskResult);
        }

        /// <summary>
        ///     This function is for rejecting material.
        /// </summary>
        /// <returns></returns>
        /// <param name="assignRejectedMaterialViewModel">
        ///     Material information which is used for filtering materials to be
        ///     rejected.
        /// </param>
        [HttpPost]
        public ActionResult Reject(AssignRejectedMaterialViewModel assignRejectedMaterialViewModel)
        {
            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Json(FindValidationErrors(ModelState));
            }

            // Do material assignment task by calling function from domain.
            _forcedRetrievalOfRejectedMaterialDomain.UnassignRejectedMaterials(
                assignRejectedMaterialViewModel.ProductOrderNumber, assignRejectedMaterialViewModel.PartialDelivery,
                assignRejectedMaterialViewModel.MaterialCode);

            return Json(true,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///     Do material retrieval task.
        /// </summary>
        /// <param name="assignRejectedMaterialViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Retrieve(AssignRejectedMaterialViewModel assignRejectedMaterialViewModel)
        {
            //Get current terminalNo
            var terminalNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                return Json(FindValidationErrors(ModelState));
            }

            // Find terminal device validation message.
            var terminalDeviceValidationMessage =
                _forcedRetrievalOfRejectedMaterialDomain.FindRetrievalMaterialValidationMessage(terminalNo);

            // Error message is detected.
            if ("MSG15".Equals(terminalDeviceValidationMessage))
            {
                // Because it can be confused with normal validation errors (Status: 400). Therefore, custom header is recommended.
                return Json(new { Success = false, Message = Resources.MaterialResource.MSG15 });

                
            }

            if ("MSG16".Equals(terminalDeviceValidationMessage))
            {
                // Because it can be confused with normal validation errors (Status: 400). Therefore, custom header is recommended.
                return Json(new { Success = false, Message = Resources.MaterialResource.MSG16 });
            }

            var result = _forcedRetrievalOfRejectedMaterialDomain.RetrieveRejectedMaterials(
                assignRejectedMaterialViewModel.ProductOrderNumber, assignRejectedMaterialViewModel.PartialDelivery,
                assignRejectedMaterialViewModel.MaterialCode, terminalNo);

            if (result)
            {
                return Json(new { Success = true });
            }
            else {
                return Json(new { Success = false, Message = Resources.MessageResource.Msg1 });           
            }
            
        }

        /// <summary>
        /// Reply message from C1.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public ActionResult PostProcessRejectedMaterial(ReplyThirdControllerViewModel parameter)
        {
            if (parameter == null)
            {
                parameter = new ReplyThirdControllerViewModel();
                TryValidateModel(parameter);
            }

            if (!ModelState.IsValid)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            var termialNo = _identityDomain.FindTerminalNo(HttpContext.User.Identity);
            var items = _forcedRetrievalOfRejectedMaterialDomain.PostProcessRejectedMaterial(termialNo, parameter.MaterialCode);
            return Json(items);
        }

        #endregion
    }
}