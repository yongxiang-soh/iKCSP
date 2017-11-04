using System.Collections.Generic;
using KCSG.Domain.Models;
using KCSG.Domain.Models.MaterialManagement;

namespace KCSG.Domain.Interfaces.MaterialManagement
{
    public interface IForcedRetrievalOfRejectedMaterialDomain
    {
        /// <summary>
        /// Find material and accept reject material.
        /// </summary>
        /// <param name="pNo"></param>
        /// <param name="partialDelivery"></param>
        /// <param name="materialCode"></param>
        /// <returns></returns>
        AssignRejectedMaterialResult AsignRejectedMaterial(string pNo, string partialDelivery, string materialCode,string terminalNo);

        /// <summary>
        /// This function is for de-assign rejected materials by using specific conditions.
        /// </summary>
        /// <param name="pNo">Product order number</param>
        /// <param name="partialDelivery">Partial delivery code</param>
        /// <param name="materialCode">Material code</param>
        void UnassignRejectedMaterials(string pNo, string partialDelivery, string materialCode);

        /// <summary>
        /// Validate information of terminal by using specific conditions defined in [3.15.3	UC 33: Retrieve Rejected Material]
        /// </summary>
        /// <returns></returns>
        string FindRetrievalMaterialValidationMessage(string terminalNo);

        /// <summary>
        /// Do rejected material retrieval.
        /// </summary>
        /// <param name="pNo"></param>
        /// <param name="partialDelivery"></param>
        /// <param name="materialCode"></param>
        bool RetrieveRejectedMaterials(string pNo, string partialDelivery, string materialCode,string terminalNo);

        /// <summary>
        /// Post Process Rejected material
        /// Refer UC38 - srs material management v1.0.1
        /// </summary>
        IList<FirstCommunicationResponse> PostProcessRejectedMaterial(string terminalNo, string materialCode);
    }
}