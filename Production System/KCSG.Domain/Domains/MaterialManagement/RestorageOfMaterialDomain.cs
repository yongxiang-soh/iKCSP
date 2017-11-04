using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Office.Word;
using KCSG.Core.Constants;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.MaterialManagement;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.MaterialManagement;

namespace KCSG.Domain.Domains.MaterialManagement
{
    public class RestorageOfMaterialDomain : BaseDomain, IRestorageOfMaterialDomain
    {
        #region Properties

        /// <summary>
        /// Service which handles notifications between modules.
        /// </summary>
        private readonly INotificationService _notificationService;

        /// <summary>
        /// Service which handles identity operations.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        /// Domain which handles storage of material operation.
        /// </summary>
        private IStorageOfMaterialDomain _storageOfMaterialDomain;

        #endregion

        #region Constructors


        /// <summary>
        ///     Initialize domain with dependency injection.
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="notificationService"></param>
        /// <param name="storageOfMaterialDomain"></param>
        /// <param name="identityService"></param>
        public RestorageOfMaterialDomain(IUnitOfWork unitOfWork, INotificationService notificationService,
            IStorageOfMaterialDomain storageOfMaterialDomain, IIdentityService identityService,
            IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _notificationService = notificationService;
            _storageOfMaterialDomain = storageOfMaterialDomain;
            _identityService = identityService;
        }

        #endregion

        /// <summary>
        ///     This function is for handling restoraging material.
        /// </summary>
        /// <param name="palletNo"></param>
        /// <param name="materialCode"></param>
        /// <param name="restorageMaterialItems"></param>
        public void RestoreMaterial(string palletNo, string materialCode, string terminalNo,
            IList<RestorageMaterialItem> restorageMaterialItems)
        {
            // Material shelf statuses find.
            var materialShelfStatuses = _unitOfWork.MaterialShelfStatusRepository.GetAll();

            // Suitable location into which material should be stored.
            TX31_MtrShfSts emptyLocation = null;
            emptyLocation = materialShelfStatuses.FirstOrDefault(x => x.F31_PalletNo.Trim().Equals(palletNo) && x.F31_TerminalNo.Trim().Equals(terminalNo));

            if (emptyLocation == null)
                emptyLocation = materialShelfStatuses.FirstOrDefault(x => x.F31_ShelfStatus.Equals("0"));

            if (emptyLocation == null)
                throw new Exception("No empty location has been found");

            // Find a list of materials.
            var materials = _unitOfWork.MaterialRepository.GetAll();

            // Find the material by code.
            var material = materials.FirstOrDefault(x => x.F01_MaterialCode.Equals(materialCode));
            if (material == null)
                throw new Exception();

            //var shelfStatuses = _unitOfWork.MaterialShelfStatusRepository.GetShelfStorageMaterial(material.F01_LiquidClass);
            //var shelfStatus = Constants.F31_ShelfStatus.EmptyShelf.ToString("D");
            //var lstShelfStatuses =
            //    _unitOfWork.MaterialShelfStatusRepository.GetMany(i => i.F31_ShelfStatus.Trim().Equals(shelfStatus));

            //	[Is_LiquidClass] is pupulated from field [F01_LiquidClass] of Material Master DB record (based on current [Material Code]).

            var tm01 =
                _unitOfWork.MaterialRepository.Get(x => x.F01_MaterialCode.Trim().Equals(materialCode));

            //if (tm01.F01_LiquidClass.Equals("0"))
            //{
            //    lstShelfStatuses = lstShelfStatuses.OrderBy(x => x.F31_CmnShfAgnOrd);
            //}
            //else
            //{
            //    lstShelfStatuses = lstShelfStatuses.OrderBy(x => x.F31_LqdShfAgnOrd);
            //}

            // update material shelf status and get F31_ShelfRow,F31_ShelfBay,F31_ShelfLevel
            var row = string.Empty;
            var bay = string.Empty;
            var level = string.Empty;
            //var tx31s = lstShelfStatuses.ToList();
            //var lasttx31 = tx31s.LastOrDefault();
            //foreach (var shelfStatuses in tx31s)
            //{
            emptyLocation.F31_ShelfStatus = "4";
            emptyLocation.F31_TerminalNo = terminalNo;
            emptyLocation.F31_UpdateDate = DateTime.Now;
            emptyLocation.F31_StockTakingFlag = Constants.TX31_StkTkgFlg.InvNotChk.ToString("D");
            emptyLocation.F31_PalletNo = palletNo;
            emptyLocation.F31_SupplierCode = string.Empty;
            emptyLocation.F31_LoadAmount = 0;
            emptyLocation.F31_Amount = restorageMaterialItems.Sum(x => x.PackUnit * x.PackQuantity + x.Fraction);
            emptyLocation.F31_UpdateDate = DateTime.Now;

            //shelfStatuses.F31_UpdateCount += 1;
            row = emptyLocation.F31_ShelfRow;
            bay = emptyLocation.F31_ShelfBay;
            level = emptyLocation.F31_ShelfLevel;

            //if (emptyLocation.Equals(lasttx31))
            //{
            //    shelfStatuses.F31_StockTakingFlag = Constants.TX31_StkTkgFlg.InvNotChk.ToString("D");
            //    shelfStatuses.F31_PalletNo = palletNo;
            //    shelfStatuses.F31_SupplierCode = string.Empty;
            //    shelfStatuses.F31_LoadAmount = 0;
            //    shelfStatuses.F31_Amount = restorageMaterialItems.Sum(x => x.PackUnit*x.PackQuantity + x.Fraction);
            //    shelfStatuses.F31_UpdateDate = DateTime.Now;
            //    //shelfStatuses.F31_UpdateCount += 1;
            //    row = shelfStatuses.F31_ShelfRow;
            //    bay = shelfStatuses.F31_ShelfBay;
            //    level = shelfStatuses.F31_ShelfLevel;
            //}
            _unitOfWork.MaterialShelfStatusRepository.Update(emptyLocation);
            //}

            //	Delete all [TX33_MtrShfStk] records whose [Pallet No.] is current [Pallet No.] and [Material Code] is current [Material Code].
            _unitOfWork.MaterialShelfStockRepository.Delete(x => x.F33_MaterialCode.Equals(materialCode) && x.F33_PalletNo.Trim().Equals(palletNo));

            //var lastShelfStatus = materialShelfStatuses.LastOrDefault();
            var lsShelfNo = string.Format("{0}{1}{2}", row,
                   bay, level);

            // Find no manage record with system id "0000"
            var isNoManage = false;
            var f48MtrWhsCmdNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                Constants.GetColumnInNoManager.MtrWhsCmdNo);
            var convCode = GetConveyorCode(terminalNo);

            InsertTX34(Constants.F34_CommandNo.Storage, f48MtrWhsCmdNo,
                Constants.CmdType.cmdType,
                Constants.TX34_StrRtrType.MaterialReStorage.ToString("D"), Constants.TC_CMDSTS.TC_CMDSTS_0, palletNo,
                convCode, lsShelfNo, terminalNo,
                Constants.PictureNo.TCRM051F);


            //	Create corresponding material shelf stock records (table [TX33_MtrShfStk]) for each “Valid item” using the following pseudo code (under SQL format - Refer document)
            foreach (var restorageMaterialItem in restorageMaterialItems)
            {
                var tx33MtrShfStk = new TX33_MtrShfStk();
                tx33MtrShfStk.F33_PalletNo = palletNo;
                tx33MtrShfStk.F33_MaterialCode = materialCode;
                tx33MtrShfStk.F33_MaterialLotNo = restorageMaterialItem.MaterialLotNo;
                tx33MtrShfStk.F33_Amount = restorageMaterialItem.PackUnit * restorageMaterialItem.PackQuantity +
                                           restorageMaterialItem.Fraction;
                tx33MtrShfStk.F33_StockFlag = Constants.F33_StockFlag.TX33_StkFlg_Str;
                tx33MtrShfStk.F33_AddDate = DateTime.Now;
                tx33MtrShfStk.F33_UpdateDate = DateTime.Now;

                _unitOfWork.MaterialShelfStockRepository.Add(tx33MtrShfStk);
            }


            //Send message to server C1. The content of the message contains [MsgId], [TermNo] and [PicNo], in which:
            //[MsgId] is “0001”.
            var msgId = "0001";

            var pictureNo = Constants.PictureNo.TCRM051F;
            _notificationService.SendMessageToC1(new
            {
                msgId,
                terminalNo,
                pictureNo
            });

            // Save changes into database.
            _unitOfWork.Commit();
        }

        /// <summary>
        ///     This function is for unassigning material by using pallet number and material code.
        /// </summary>
        /// <param name="palletNo"></param>
        /// <param name="materialCode"></param>
        public void UnassginMaterial(string palletNo, string materialCode)
        {
            // The system will delete corresponding [tx33_mtrshfstk] record whose [Pallet No.] is current [Pallet No.] and [Material Code] is current [Material Code]
            if (!string.IsNullOrEmpty(palletNo) && !string.IsNullOrEmpty(materialCode))
            {
                _unitOfWork.MaterialShelfStockRepository.Delete(
                    x => x.F33_PalletNo.Equals(palletNo.Trim()) && x.F33_MaterialCode.Equals(materialCode.Trim()));
                _unitOfWork.Commit();
            }
        }

        /// <summary>
        ///     Empty material storage by using material code.
        /// </summary>
        /// <param name="materialCode"></param>
        /// <param name="terminalNo"></param>
        public void EmptyMaterialStorage(string materialCode, string terminalNo)
        {
            // Find all material shelf statuses in database.
            var materialShelfStatuses = _unitOfWork.MaterialShelfStatusRepository.GetAll();
            /*
             * 	Update each [TX31_MtrShfSts] record whose [F31_ShelfStatus] is “0” (empty shelf) and [F31_CmnShfAgnOrd] is not blank using the following pseudo code (under SQL format):
             */
            var materialShelfStatus = materialShelfStatuses.Where(x => x.F31_ShelfStatus.Equals("0") && x.F31_CmnShfAgnOrd != 1).FirstOrDefault();

            // Find the first materials using material code.
            var materials = _unitOfWork.MaterialRepository.GetAll();
            var material = materials.FirstOrDefault(x => x.F01_MaterialCode.Equals(materialCode));

            // As document writen: 	If there is no [TX31_MtrShfSts] record whose [F31_ShelfStatus] is “0” (empty shelf) and [F31_CmnShfAgnOrd
            if (material == null || materialShelfStatus == null)
                throw new Exception("MSG18");

            // Find current time of system.
            var systemTime = DateTime.Now;
            //get conveyor code
            var conveyorCode = GetConveyorCode(terminalNo);
            var lsShelfNo = "";

            //foreach (var materialShelfStatus in scaffoldedMaterialShelfStatuses)
            //{

            lsShelfNo = materialShelfStatus.F31_ShelfRow + materialShelfStatus.F31_ShelfBay +
                        materialShelfStatus.F31_ShelfLevel;

            materialShelfStatus.F31_ShelfStatus =
                Constants.F31_ShelfStatus.ReservedForStorage.ToString("D");
            materialShelfStatus.F31_StockTakingFlag = Constants.TX31_StkTkgFlg.InvNotChk.ToString("D");
            materialShelfStatus.F31_SupplierCode = null;
            materialShelfStatus.F31_TerminalNo = terminalNo;
            materialShelfStatus.F31_LoadAmount = 0;
            materialShelfStatus.F31_PalletNo = null;
            materialShelfStatus.F31_Amount = 0;
            materialShelfStatus.F31_StorageDate = systemTime;
            materialShelfStatus.F31_UpdateDate = systemTime;

            _unitOfWork.MaterialShelfStatusRepository.Update(materialShelfStatus);

            //Insert Or Update tx48_nomanage
            var isNoManage = true;
            var f48MtrWhsCmdNo = _unitOfWork.NoManageRepository.CreateOrUpdateTX48(ref isNoManage,
                Constants.GetColumnInNoManager.MtrWhsCmdNo, 1, 0, 0, 0, 0);

            //Insert TX34
            InsertTX34(Constants.F34_CommandNo.Storage, f48MtrWhsCmdNo,
                    Constants.CmdType.cmdType,
                    Constants.TX34_StrRtrType.WarehousePallet.ToString("D"), Constants.TC_CMDSTS.TC_CMDSTS_0, materialShelfStatus.F31_PalletNo,
                    conveyorCode, lsShelfNo, terminalNo,
                    Constants.PictureNo.TCRM051F);


            var msgId = "0001";
            var pictureNo = Constants.PictureNo.TCRM091F;
            //send message to C1
            _notificationService.SendMessageToC1(new
            {
                msgId,
                terminalNo,
                pictureNo
            });

            //}


            _unitOfWork.Commit();
        }

        /// <summary>
        ///     Check whether device is valid or not.
        /// </summary>
        /// <returns></returns>
        public bool IsValidDevice()
        {
            /*
             * 	If there is no [tm14_device] record whose [f14_devicecode] is “ATW001” 
             * or [f14_devicestatus] is “1” (Offline), 
             * [f14_devicestatus] is “2” (Error) 
             * or [f14_usepermitclass] is “1” (Prohibited), 
             * the system will show error message using template MSG 16
             */
            var invalidStatuses = new[] { "1", "2" };

            var devices = _unitOfWork.DeviceRepository.GetAll();
            devices = devices.Where(x => x.F14_DeviceCode.Equals("ATW001"));
            devices = devices.Where(x => !invalidStatuses.Contains(x.F14_DeviceStatus));
            devices =
                devices.Where(x => !x.F14_UsePermitClass.Equals(Constants.F14_UsePermitClass.Prohibited.ToString()));

            return devices.Any();
        }

        /// <summary>
        /// Proceed data after messages sent back from C3.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProceedData()
        {
            // Find current terminal which is sending request.
            var terminalNo = _identityService.FindTerminalNo(HttpContext.Current.User.Identity);
            _storageOfMaterialDomain.PostStoreMaterial(terminalNo, "TCRM051F");

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        /// <summary>
        /// Find material shelf stocks in database base on specific information.
        /// </summary>
        /// <param name="materialCode"></param>
        /// <param name="palletNo"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        public IList<MaterialDetailInformationViewModel> FindMaterialShelfStocks(string materialCode, string palletNo, string terminalNo)
        {
            var materialShelfStatuses = _unitOfWork.MaterialShelfStatusRepository.GetAll();
            var materialShelfStocks = _unitOfWork.MaterialShelfStockRepository.GetAll();
            var materials = _unitOfWork.MaterialRepository.GetAll();

            var stocks = from materialShelfStatus in materialShelfStatuses
                         from materialShelfStock in materialShelfStocks
                         from material in materials
                         where terminalNo.Equals(materialShelfStatus.F31_TerminalNo.Trim())
                         && materialShelfStatus.F31_PalletNo.Trim().Equals(palletNo)
                         && materialShelfStatus.F31_PalletNo.Equals(materialShelfStock.F33_PalletNo)
                         && material.F01_MaterialCode.Equals(materialShelfStock.F33_MaterialCode)
                         select new MaterialDetailInformationViewModel
                         {
                             F01_PackingUnit = material.F01_PackingUnit,
                             F33_Amount = materialShelfStock.F33_Amount,
                             F33_MaterialLotNo = materialShelfStock.F33_MaterialLotNo
                         };

            return stocks.ToList();
        }
    }
}