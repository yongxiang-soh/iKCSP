using System;
using System.Linq;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.MaterialManagement
{
    public class AcceptanceOfMaterialDomain : BaseDomain, IAcceptanceOfMaterialDomain
    {
        #region Constructor

        /// <summary>
        ///     Initialize instance with dependency injections.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="configurationService"></param>
        public AcceptanceOfMaterialDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find a list of Reception by using pNo and partial delivery information with pagination.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <param name="pNo"></param>
        /// <param name="partialDelivery"></param>
        /// <returns></returns>
        public ResponseResult<GridResponse<object>> SearchRawMaterial(GridSettings gridSettings, string pNo,
            string partialDelivery)
        {
            // Find the list of receptions.
            var receptions = _unitOfWork.ReceptionRepository.GetAll();

            // Filter reception by using specific conditions.
            receptions = FindRawMaterials(receptions, pNo, partialDelivery);

            // Find all materials in database.
            var materials = _unitOfWork.MaterialRepository.GetAll();

            // Join material and select reception to get the list of materials which need delivering.
            var deliveredMaterials = from reception in receptions
                from material in materials
                where reception.F30_MaterialCode.Equals(material.F01_MaterialCode)
                select new
                {
                    reception.F30_ExpectDate,
                    reception.F30_PrcOrdNo,
                    reception.F30_PrtDvrNo,
                    reception.F30_MaterialCode,
                    material.F01_MaterialDsp,
                    reception.F30_ExpectAmount,
                    reception.F30_StoragedAmount
                };

            // Do ordering and pagination.
            OrderByAndPaging(ref deliveredMaterials, gridSettings);

            // Build a grid and respond to client.
            var resultModel = new GridResponse<object>(deliveredMaterials, receptions.Count());
            return new ResponseResult<GridResponse<object>>(resultModel, true);
        }

        /// <summary>
        ///     Accept a raw material by using pNo and partial delivery.
        /// </summary>
        /// <param name="pNo"></param>
        /// <param name="partialDelivery"></param>
        public void AcceptRawMaterial(string pNo, string partialDelivery)
        {
            // Find all receptions in database.
            var receptions = _unitOfWork.ReceptionRepository.GetAll();

            // Do filtering.
            receptions = FindRawMaterials(receptions, pNo, partialDelivery);

            // Result is not unique.
            if (receptions.Count() != 1)
                throw new Exception();

            // Find the first matched reception in the searched list.
            var reception = receptions.First();

            // 	Set [Accepted Class] as “Accepted”.
            reception.F30_AcceptClass = Constants.TX30_Reception.Accepted.ToString("D");
            reception.F30_UpdateDate = DateTime.Now;
            reception.F30_UpdateCount++;

            // Update the record to database.
            _unitOfWork.ReceptionRepository.Update(reception);

            // Commit the changes.
            _unitOfWork.Commit();
        }

        /// <summary>
        ///     Reject a raw material by using pNo and partial delivery.
        /// </summary>
        /// <param name="pNo"></param>
        /// <param name="partialDelivery"></param>
        public void RejectRawMaterial(string pNo, string partialDelivery)
        {
            // Firstly, take all the receptions.
            var receptions = _unitOfWork.ReceptionRepository.GetAll();

            // Filter by using specific conditions.
            receptions = FindRawMaterials(receptions, pNo, partialDelivery);

            // Result is not unique.
            if (receptions.Count() != 1)
                throw new Exception();

            // Find the first matched reception in the searched list.
            var reception = receptions.First();

            // 	Set [Accepted Class] as “Accepted”.
            reception.F30_AcceptClass = Constants.TX30_Reception.Rejected.ToString("D");
            reception.F30_UpdateDate = DateTime.Now;
            reception.F30_UpdateCount++;

            // Update the record to database.
            _unitOfWork.ReceptionRepository.Update(reception);

            // Commit the changes.
            _unitOfWork.Commit();
        }

        /// <summary>
        ///     Find the first reception by using pNo and partial delivery.
        /// </summary>
        /// <param name="receptions">List of receptions which is needed filtering</param>
        /// <param name="pNo"></param>
        /// <param name="partialDelivery"></param>
        /// <returns></returns>
        private IQueryable<TX30_Reception> FindRawMaterials(IQueryable<TX30_Reception> receptions, string pNo,
            string partialDelivery)
        {
            // pNo is defined
            if (!string.IsNullOrWhiteSpace(pNo))
                receptions = receptions.Where(x => x.F30_PrcOrdNo.StartsWith(pNo.Trim()));

            // Partial delivery is defined.
            if (!string.IsNullOrWhiteSpace(partialDelivery))
                receptions = receptions.Where(x => x.F30_PrtDvrNo.StartsWith(partialDelivery.Trim()));

            // Convert enumeration to string of int.
            var nonAcceptedStatus = Constants.TX30_Reception.NonAccept.ToString("D");

            // Only return item in pending status.
            receptions = receptions.Where(x => x.F30_AcceptClass.Equals(nonAcceptedStatus));

            return receptions;
        }

        #endregion
    }
}