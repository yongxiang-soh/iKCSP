using System;
using System.Collections.Generic;
using System.Linq;
using KCSG.Core.Constants;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models;
using KCSG.Domain.Models.MaterialManagement;

namespace KCSG.Domain.Domains.MaterialManagement
{
    public class ForcedRetrievalOfRejectedMaterialDomain : BaseDomain, IForcedRetrievalOfRejectedMaterialDomain
    {

        #region Properties

        /// <summary>
        /// Service which handles notification between modules.
        /// </summary>
        private readonly INotificationService _notificationService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize domain with dependency injections.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="notificationService"></param>
        public ForcedRetrievalOfRejectedMaterialDomain(IUnitOfWork unitOfWork, INotificationService notificationService, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Assign a list of materials which match with spefic conditions defined by client request.
        /// </summary>
        /// <param name="pNo">Product order number</param>
        /// <param name="partialDelivery">Partial delivery code</param>
        /// <param name="materialCode">Material code</param>
        /// <returns></returns>
        public AssignRejectedMaterialResult AsignRejectedMaterial(string pNo, string partialDelivery,
            string materialCode, string terminalNo)
        {
            var shelfStatusMaterial = Constants.F31_ShelfStatus.Material.ToString("D");
            // Take all material shelves from database.
            var materialShelves = _unitOfWork.MaterialShelfRepository.GetAll();

            // Take all material shelf statuses from database.
            var materialShelfStatuses = _unitOfWork.MaterialShelfStatusRepository.GetAll();

            // Take all receptions from repository.
            var receptions = _unitOfWork.ReceptionRepository.GetAll();

            // Base on the condition defined in document, retrieve the condition statisfied Material status records.
            materialShelfStatuses = FindMaterials(materialShelfStatuses, receptions, materialShelves, pNo,
                partialDelivery, materialCode);

            // Find all actionable items from material shelf statuses filtered list.
            materialShelfStatuses = from materialShelfStatus in materialShelfStatuses
                                    where materialShelfStatus.F31_ShelfStatus.Equals(shelfStatusMaterial)
                                    select materialShelfStatus;

            // Result initialization.
            var assignRejectedMaterialResult = new AssignRejectedMaterialResult();

            //	For each retrieved record above, the system will:
            foreach (var materialShelfStatus in materialShelfStatuses)
            {
                //	Update corresponding [tx31_mtrshfsts] record as below:
                materialShelfStatus.F31_ShelfStatus = Constants.F31_ShelfStatus.ReservedForRetrieval.ToString("d");
                materialShelfStatus.F31_UpdateDate = DateTime.Now;
                //Set [f31_terminalno] as current application terminal.
                materialShelfStatus.F31_TerminalNo = terminalNo;

                // Increase record counter by one.
                assignRejectedMaterialResult.Assigned++;

                // Increase the total assigned item amount.
                if (materialShelfStatus.F31_Amount != null)
                    assignRejectedMaterialResult.AssignedQuantity += materialShelfStatus.F31_Amount.Value;

                // Update this current processed item.
                _unitOfWork.MaterialShelfStatusRepository.Update(materialShelfStatus);

                break;
            }

            // Save changes to database.
            _unitOfWork.Commit();

            return assignRejectedMaterialResult;
        }

        /// <summary>
        /// This function is for unassign rejected materials by using specific conditions.
        /// </summary>
        /// <param name="pNo">Product order number</param>
        /// <param name="partialDelivery">Partial delivery code</param>
        /// <param name="materialCode">Material code</param>
        public void UnassignRejectedMaterials(string pNo, string partialDelivery, string materialCode)
        {
            var reservedForRetrieval = Constants.F31_ShelfStatus.ReservedForRetrieval.ToString("D");

            // Take all material shelves from database.
            var materialShelves = _unitOfWork.MaterialShelfRepository.GetAll();

            // Take all material shelf statuses from database.
            var materialShelfStatuses = _unitOfWork.MaterialShelfStatusRepository.GetAll();

            // Take all receptions from repository.
            var receptions = _unitOfWork.ReceptionRepository.GetAll();

            // Base on the condition defined in document, retrieve the condition statisfied Material status records.
            materialShelfStatuses = FindMaterials(materialShelfStatuses, receptions, materialShelves, pNo,
                partialDelivery, materialCode);

            // Find all actionable items from material shelf statuses filtered list.
            materialShelfStatuses = from materialShelfStatus in materialShelfStatuses
                                    where materialShelfStatus.F31_ShelfStatus.Equals(reservedForRetrieval)
                                    select materialShelfStatus;

            //	For each “Actionable Item” (which is mentioned above), the system will update as below
            foreach (var materialShelfStatus in materialShelfStatuses)
            {
                //	Set [f31_shelfstatus] as 3
                materialShelfStatus.F31_ShelfStatus = Constants.F31_ShelfStatus.Material.ToString("d");
                //	Set [f31_terminalno] as blank
                materialShelfStatus.F31_TerminalNo = "";
                //	Set [f31_updatedate] as current date and time
                materialShelfStatus.F31_UpdateDate = DateTime.Now;

                // Update this current being processed item into database.
                _unitOfWork.MaterialShelfStatusRepository.Update(materialShelfStatus);
            }

            // Save changes.
            _unitOfWork.Commit();
        }

        /// <summary>
        /// Validate information of terminal by using specific conditions defined in [3.15.3	UC 33: Retrieve Rejected Material]
        /// </summary>
        /// <returns></returns>
        public string FindRetrievalMaterialValidationMessage(string terminalNo)
        {
            //or [f14_devicestatus] is “1” (Offline), [f14_devicestatus] is “2” (Error) or [f14_usepermitclass] is “1” (Prohibited), 
            //the system will show error message using template MSG 16.
            var terminalDevices = _unitOfWork.DeviceRepository.GetAll();
            var error = Constants.F05_StrRtrSts.Error.ToString("D");
            var offline = Constants.F14_DeviceStatus.Offline.ToString();
            var deviceStatusError = Constants.F14_DeviceStatus.Error.ToString();
            var usePermitClass = Constants.F14_UsePermitClass.Prohibited.ToString("D");
            var conveyor = _unitOfWork.ConveyorRepository.GetMany(
                i => i.F05_TerminalNo.Trim().Equals(terminalNo));
            if (!conveyor.Any())
            {
                return "MSG15";
            }
            if (conveyor.Any(i=>i.F05_StrRtrSts.Equals(error)))
            {
                return "MSG15";
            }
            //	If there is no [tm14_device] record whose [f14_devicecode] is “ATW001” 
            if (!terminalDevices.Any(x => x.F14_DeviceCode.Equals(Constants.DeviceCode.ATW001)))
                return "MSG16";

            if (terminalDevices.Any(x => x.F14_DeviceStatus.Equals(offline) ||
                                         x.F14_DeviceStatus.Equals(deviceStatusError) ||
                                         x.F14_UsePermitClass.Equals(usePermitClass)))
                return "MSG16";

            return null;
        }

        /// <summary>
        /// Finding rejected materials by using specific conditions.
        /// </summary>
        public bool RetrieveRejectedMaterials(string pNo, string partialDelivery, string materialCode, string terminalNo)
        {
            var shelfStatusMaterial = Constants.F31_ShelfStatus.ReservedForRetrieval.ToString("D");
            // Take all material shelves from database.
            var materialShelves = _unitOfWork.MaterialShelfRepository.GetAll();

            // Take all material shelf statuses from database.
            var materialShelfStatuses = _unitOfWork.MaterialShelfStatusRepository.GetAll();

            // Take all receptions from repository.
            var receptions = _unitOfWork.ReceptionRepository.GetAll();
            
            // Find previous actionable items.
            var items = FindMaterialsLite(materialShelfStatuses, receptions, materialShelves, pNo, partialDelivery,
                materialCode);

            // Find all actionable items from material shelf statuses filtered list.
            items = items.Where(x => x.MaterialStatus.Equals(shelfStatusMaterial));

            //	If there is a[tx48_nomanage] record whose[f48_systemid] = “0000”, the system will:
            var isNoManage = false;

            var itemAmount = items.Count();
            if (itemAmount < 1)
                return false;
            
            foreach (var item in items)
            {
                // Find all material stocks.
                var materialStocks = _unitOfWork.MaterialShelfStockRepository.GetAll();
                materialStocks = materialStocks.Where(x => x.F33_PalletNo.Equals(item.PalletNo));
                materialStocks = materialStocks.Where(x => x.F33_MaterialCode.Equals(item.MaterialCode));

                foreach (var materialStock in materialStocks)
                    materialStock.F33_StockFlag = Constants.TX33_MtrShfStk.Retrieving.ToString("d");

                //	[Is_ConveyorCode] is [f05_conveyorcode] of [tm05_conveyor] whose [f05_terminalno] is current application terminal.
                var lsConveyorCode = GetConveyorCode(terminalNo);
                var f48MtrWhsCmdNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage, Constants.GetColumnInNoManager.MtrWhsCmdNo, 1);
                InsertTX34(Constants.F34_CommandNo.Retrieval, f48MtrWhsCmdNo,
                    Constants.CmdType.cmdType,
                    Constants.TX34_StrRtrType.Material.ToString("D"), Constants.TC_CMDSTS.TC_CMDSTS_0, item.PalletNo,
                    string.Format("{0}", item.ShelfRow + item.ShelfBay + item.ShelfLevel), lsConveyorCode,
                    //Constants.TerminalNo.A001,
                    terminalNo,
                    Constants.PictureNo.TCRM151F);
            }

            //Send message to C1
            var msgId = "0011";
            var pictureNo = Constants.PictureNo.TCRM151F;

            _notificationService.SendMessageToC1(new
            {
                msgId,
                terminalNo,
                pictureNo
            });

            // Save changes into database.
            _unitOfWork.Commit();

            return true;
        }

        /// <summary>
        /// This function is for searching items which matche with conditions defined in [KCSG.TC_SRS_Material Management Sub System_v0.7 - 3.15.1	UC 31: Assign Rejected Material]
        /// This will join 3 tables : MaterialShelfStatus, Receptions and MaterialShelves for result filtering.
        /// </summary>
        /// <param name="materialShelfStatuses">List of material shelf statuses which are needed filtering</param>
        /// <param name="receptions">List of receptions which are needed filtering</param>
        /// <param name="materialShelves">List of material shelves which are needed filtering</param>
        /// <param name="pNo"></param>
        /// <param name="partialDelivery"></param>
        /// <param name="materialCode"></param>
        /// <returns></returns>
        private IQueryable<TX31_MtrShfSts> FindMaterials(IQueryable<TX31_MtrShfSts> materialShelfStatuses,
            IQueryable<TX30_Reception> receptions, IQueryable<TX32_MtrShf> materialShelves,
            string pNo, string partialDelivery, string materialCode)
        {
            var accept = Constants.TX30_Reception.Rejected.ToString("D");
            var shelfStatusMaterial = Constants.F31_ShelfStatus.Material.ToString("D");

            // Material shelf statuses filter.
            //materialShelfStatuses = materialShelfStatuses.Where(x => x.F31_ShelfStatus.Equals(shelfStatusMaterial));
            

            // Receptions filter.
            receptions = receptions.Where(x => x.F30_AcceptClass.Equals(accept));
            receptions = receptions.Where(x => x.F30_MaterialCode.Equals(materialCode));

            //	If [P. O. No.] is not blank, f30_prcordno = [P. O. No.].
            //	If [P. O. No.] is blank, f30_prcordno like ‘%’
            if (!string.IsNullOrEmpty(pNo))
                receptions = receptions.Where(x => x.F30_PrcOrdNo.Equals(pNo));

            //	If [Partial Delivery] is not blank, f30_prtdvrno = [Partial Delivery]
            //	If [Partial Delivery] is blank, f30_prtdvrno like ‘%’
            if (!string.IsNullOrEmpty(partialDelivery))
                receptions = receptions.Where(x => x.F30_PrtDvrNo.Equals(partialDelivery));

            // Query should be refered to [KCSG.TC_SRS_Material Management Sub System_v0.7.docx - 3.15.1.2]            

            return from materialShelfStatus in materialShelfStatuses
                   from reception in receptions
                   from materialShelf in materialShelves
                   where
                       (
                           materialShelfStatus.F31_PalletNo.Equals(materialShelf.F32_PalletNo)
                           && reception.F30_PrcOrdNo.Equals(materialShelf.F32_PrcOrdNo)
                           && reception.F30_PrtDvrNo.Equals(materialShelf.F32_PrtDvrNo)
                           )
                   orderby new
                   {
                       materialShelfStatus.F31_ShelfRow,
                       materialShelfStatus.F31_ShelfBay,
                       materialShelfStatus.F31_ShelfLevel
                   }
                   select materialShelfStatus;
        }

        private IQueryable<FindRetrievalMaterialResult> FindMaterialsLite(
            IQueryable<TX31_MtrShfSts> materialShelfStatuses,
            IQueryable<TX30_Reception> receptions, IQueryable<TX32_MtrShf> materialShelves,
            string pNo, string partialDelivery, string materialCode)
        {
            // Convert enumerable to string.
            //var accept = Constants.TX30_Reception.Accepted.ToString("D");
            var accept = Constants.TX30_Reception.Rejected.ToString("D");
            var shelfStatusMaterial = Constants.F31_ShelfStatus.ReservedForRetrieval.ToString("D");
            //var shelfStatusMaterial = Constants.F31_ShelfStatus.Material.ToString("D");

            // Material shelf statuses filter.
            //materialShelfStatuses = materialShelfStatuses.Where(x => x.F31_ShelfStatus.Equals(shelfStatusMaterial));

            // Receptions filter.
            receptions = receptions.Where(x => x.F30_AcceptClass.Trim().Equals(accept));
            receptions = receptions.Where(x => x.F30_MaterialCode.Trim().Equals(materialCode));

            // tx31 filter.
            materialShelfStatuses = materialShelfStatuses.Where(x => x.F31_ShelfStatus.Trim().Equals(shelfStatusMaterial));

            //	If [P. O. No.] is not blank, f30_prcordno = [P. O. No.].
            //	If [P. O. No.] is blank, f30_prcordno like ‘%’
            if (!string.IsNullOrEmpty(pNo))
                receptions = receptions.Where(x => x.F30_PrcOrdNo.Trim().Equals(pNo));

            //	If [Partial Delivery] is not blank, f30_prtdvrno = [Partial Delivery]
            //	If [Partial Delivery] is blank, f30_prtdvrno like ‘%’
            if (!string.IsNullOrEmpty(partialDelivery))
                receptions = receptions.Where(x => x.F30_PrtDvrNo.Trim().Equals(partialDelivery));

            // Query should be refered to [KCSG.TC_SRS_Material Management Sub System_v0.7.docx - 3.15.1.2]
            var result = from materialShelfStatus in materialShelfStatuses
                from reception in receptions
                from materialShelf in materialShelves
                where
                    (
                        materialShelfStatus.F31_PalletNo.Equals(materialShelf.F32_PalletNo)
                        && reception.F30_PrcOrdNo.Equals(materialShelf.F32_PrcOrdNo)
                        && reception.F30_PrtDvrNo.Equals(materialShelf.F32_PrtDvrNo)
                        )
                orderby new
                {
                    materialShelfStatus.F31_ShelfRow,
                    materialShelfStatus.F31_ShelfBay,
                    materialShelfStatus.F31_ShelfLevel
                }
                select new FindRetrievalMaterialResult
                {
                    PalletNo = materialShelf.F32_PalletNo,
                    MaterialCode = reception.F30_MaterialCode,
                    ShelfRow = materialShelfStatus.F31_ShelfRow,
                    ShelfBay = materialShelfStatus.F31_ShelfBay,
                    ShelfLevel = materialShelfStatus.F31_ShelfLevel,
                    MaterialStatus = materialShelfStatus.F31_ShelfStatus
                };
            return result;
        }

        /// <summary>
        /// Post Process Rejected material
        /// Refer UC38 - srs material management v1.0.1
        /// </summary>
        public IList<FirstCommunicationResponse> PostProcessRejectedMaterial(string terminalNo ,string materialCode)
        {
            var pictureNo = Constants.PictureNo.TCRM151F;
            var status1 = Constants.F34_Status.status6;
            var status2 = Constants.F34_Status.status7;
            var status3 = Constants.F34_Status.status9;

            var items = new List<FirstCommunicationResponse>();

            //find material warehouse command records 
            var materialWarehouseCommands =
                _unitOfWork.MaterialWarehouseCommandRepository.GetMany(
                    i =>i.F34_TerminalNo.Trim().Equals(terminalNo.Trim())&&
                        i.F34_PictureNo.Trim().Equals(pictureNo) &&
                        (i.F34_Status.Equals(status1) || i.F34_Status.Equals(status2) || i.F34_Status.Equals(status3)))
                    .OrderBy(i => i.F34_AddDate);

            foreach (var materialWarehouseCommand in materialWarehouseCommands)
            {
                var oldStatus = materialWarehouseCommand.F34_Status;
                if (materialWarehouseCommand.F34_Status == status1)
                    materialWarehouseCommand.F34_Status = Constants.F34_Status.statusC;
                else if (materialWarehouseCommand.F34_Status == status2)
                    materialWarehouseCommand.F34_Status = Constants.F34_Status.statusD;
                else
                    materialWarehouseCommand.F34_Status = Constants.F34_Status.statusF;

                materialWarehouseCommand.F34_UpdateDate = DateTime.Now;
                _unitOfWork.MaterialWarehouseCommandRepository.Update(materialWarehouseCommand);

                var item = AutoMapper.Mapper.Map<FirstCommunicationResponse>(materialWarehouseCommand);
                item.OldStatus = oldStatus;
                items.Add(item);

            }
            _unitOfWork.Commit();
            return items;
        }

        #endregion
    }
}