using System;
using System.Linq;
using KCSG.Core.Constants;
using KCSG.Domain.Interfaces.SystemManagement;
using KCSG.Data.Infrastructure;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.Services;
using Microsoft.AspNet.SignalR;

namespace KCSG.Domain.Domains.SystemManagement
{
    public class StartEndSystemDomain : IStartEndSystemDomain
    {
        #region Properties

        /// <summary>
        /// Service which is used for broadcasting message from client.
        /// </summary>
        private readonly INotificationService _notificationDomain;

        /// <summary>
        /// Initiate instance of unit of work.
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;

        #endregion

        #region Constructor

        /// <summary>
        /// Initiate domain with dependency injection
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="notificationDomain"></param>
        public StartEndSystemDomain(IUnitOfWork unitOfWork, INotificationService notificationDomain)
        {
            _unitOfWork = unitOfWork;
            _notificationDomain = notificationDomain;
        }

        #endregion

        #region Methods

        public ResponseResult EndSystem(Enum startOrEnd, string terminalNo)
        {
            //deletes unused information of material warehouse 
            _unitOfWork.MaterialShelfStockRepository.Delete(
                    i => i.F33_Amount <= 0 && i.F33_StockFlag.Trim().Equals(Constants.F33_StockFlag.TX33_StkFlg_NotStk));
           

            //Get all [f32_palletno]’s value from “tx32_mtrshf” table
            var lstMtrShf = _unitOfWork.MaterialShelfRepository.GetAll();
            foreach (var tx32MtrShf in lstMtrShf)
            {
                var lstMtrShfStkForPalletNo = _unitOfWork.MaterialShelfStockRepository.GetAll().Where(i => i.F33_PalletNo.Trim().Equals(tx32MtrShf.F32_PalletNo.Trim()));
                if (lstMtrShfStkForPalletNo.Any()) continue;
                var lstmtrshfsts =
                    _unitOfWork.MaterialShelfStatusRepository.GetAll().Where(i => i.F31_PalletNo.Trim().Equals(tx32MtrShf.F32_PalletNo.Trim()));
                foreach (var tx31MtrShfStse in lstmtrshfsts)
                {
                    tx31MtrShfStse.F31_PalletNo = null;
                    tx31MtrShfStse.F31_UpdateDate = DateTime.Now;
                    _unitOfWork.MaterialShelfStatusRepository.Update(tx31MtrShfStse);
                }
                _unitOfWork.MaterialShelfRepository.Delete(tx32MtrShf);

                // Show notification MSG5"Unused information of material warehouse deleted!"
            }
            _notificationDomain.BroadcastNotificationMessage(null, null, "Unused information of material warehouse deleted!");
            //if (lstMtrShf.Any())
            //{

            //    var palletNo = lstMtrShf.Select(i => i.F32_PalletNo.Trim());
            //    var lstMtrShfStkForPalletNo = _unitOfWork.MaterialShelfStockRepository.GetAll().Where(i => palletNo.Contains(i.F33_PalletNo.Trim()));
            //    if (!lstMtrShfStkForPalletNo.Any())
            //    {
            //        var lstmtrshfsts =
            //            _unitOfWork.MaterialShelfStatusRepository.GetAll().Where(i => palletNo.Contains(i.F31_PalletNo.Trim()));
            //        foreach (var tx31MtrShfStse in lstmtrshfsts)
            //        {
            //            tx31MtrShfStse.F31_PalletNo = null;
            //            tx31MtrShfStse.F31_UpdateDate = DateTime.Now;
            //            _unitOfWork.MaterialShelfStatusRepository.Update(tx31MtrShfStse);
            //        }
            //        _unitOfWork.MaterialShelfRepository.Delete(i => palletNo.Contains(i.F32_PalletNo.Trim()));

            //        // Show notification MSG5"Unused information of material warehouse deleted!"

            //        _notificationDomain.BroadcastNotificationMessage(null, null, "Unused information of material warehouse deleted!");
            //    }

            //}

            //deletes unused information of pre-product
            _unitOfWork.PreProductShelfStockRepository.Delete(
                    i => i.F49_Amount < 0 && i.F49_ShelfStatus.Trim().Equals(Constants.F49_ShelfStatus.TX49_StkFlg_NotStk));
            // Show notification MSG5"Unused information of material warehouse deleted!"
            _notificationDomain.BroadcastNotificationMessage(null, null, "Unused information of pre-product warehouse deleted!");



            //deletes unused information of product warehouse 

            _unitOfWork.ProductShelfStockRepository.Delete(
                 i =>
                     i.F40_StockFlag.Trim().Equals(Constants.F40_StockFlag.TX40_StkFlg_NotStk) &&
                     (i.F40_Amount < 0 || Math.Abs(i.F40_Amount - i.F40_ShippedAmount) <= 0));
            //if (lstpdtShfStk.Any())
            //{
            //    foreach (var pdtShfStkItem in lstpdtShfStk)
            //    {
            //        _unitOfWork.ProductShelfStockRepository.Delete(pdtShfStkItem);
            //    }
            //}

            //Delete outside pre-product stock from “tx53_outsideprepdtstk” table 

            _unitOfWork.OutSidePrePdtStkRepository.Delete((
                i => i.F53_Amount < 0 && i.F53_StockFlag.Trim().Equals(Constants.F53_StokcFlag.TX53_StkFlg_NotStk)));


            //Get all [f57_palletno]’s value from “tx57_pdtshf” table

            var lstPdtShf = _unitOfWork.ProductShelfRepository.GetAll();
            foreach (var tx57PdtShf in lstPdtShf)
            {
                 var lstPdtShfStkForPalletNo =
                    _unitOfWork.ProductShelfStockRepository.GetAll().Where(i => i.F40_PalletNo.Trim().Equals(tx57PdtShf.F57_PalletNo.Trim()));
                var lstOutsidePrePdtStkForPalletNo =
                    _unitOfWork.OutSidePrePdtStkRepository.GetAll().Where(i => i.F53_PalletNo.Trim().Equals(tx57PdtShf.F57_PalletNo.Trim()));
                if (lstPdtShfStkForPalletNo.Any() || lstOutsidePrePdtStkForPalletNo.Any()) continue;
                var lstpdtshfsts =
                    _unitOfWork.ProductShelfStatusRepository.GetMany(
                        i => i.F51_PalletNo.Trim().Equals(tx57PdtShf.F57_PalletNo.Trim()));
                foreach (var tx51PdtShfStse in lstpdtshfsts)
                {
                    tx51PdtShfStse.F51_PalletNo = null;
                    tx51PdtShfStse.F51_UpdateDate = DateTime.Now;
                    _unitOfWork.ProductShelfStatusRepository.Update(tx51PdtShfStse);
                }
                _unitOfWork.ProductShelfRepository.Delete(tx57PdtShf);
                // show message"Unused information of product warehouse deleted!"
            }
            _notificationDomain.BroadcastNotificationMessage(null, null,
                      "Unused information of product warehouse deleted!");
            //if (lstMtrShf.Any())
            //{
            //    var lstF57PalletNo = lstPdtShf.Select(i => i.F57_PalletNo.Trim());


            //    var lstPdtShfStkForPalletNo =
            //        _unitOfWork.ProductShelfStockRepository.GetAll().Where(i => lstF57PalletNo.Contains(i.F40_PalletNo.Trim()));
            //    var lstOutsidePrePdtStkForPalletNo =
            //        _unitOfWork.OutSidePrePdtStkRepository.GetAll().Where(i => lstF57PalletNo.Contains(i.F53_PalletNo.Trim()));
            //    if (!lstPdtShfStkForPalletNo.Any() && !lstOutsidePrePdtStkForPalletNo.Any())
            //    {
            //        var lstpdtshfsts =
            //            _unitOfWork.ProductShelfStatusRepository.GetMany(i => lstF57PalletNo.Contains(i.F51_PalletNo.Trim()));
            //        foreach (var tx51PdtShfStse in lstpdtshfsts)
            //        {
            //            tx51PdtShfStse.F51_PalletNo = null;
            //            tx51PdtShfStse.F51_UpdateDate = DateTime.Now;
            //            _unitOfWork.ProductShelfStatusRepository.Update(tx51PdtShfStse);
            //        }
            //        _unitOfWork.ProductShelfRepository.Delete(i => lstF57PalletNo.Contains(i.F57_PalletNo.Trim()));
            //        // show message"Unused information of product warehouse deleted!"
            //        _notificationDomain.BroadcastNotificationMessage(null, null, "Unused information of product warehouse deleted!");
            //    }

            //}
            
            var termStatus = _unitOfWork.TermStatusRepository.GetMany(i => i.F17_TermNo.Trim().Equals(terminalNo.Trim())).FirstOrDefault();
            if (termStatus != null)
            {
                termStatus.F17_InUsePictureNo = null;
                _unitOfWork.TermStatusRepository.Update(termStatus);
            }

            //show message "Status of the terminals reset!"
            _notificationDomain.BroadcastNotificationMessage(null, null, "Status of the terminals reset!");

            //stop the conveyor 
            if (startOrEnd.ToString() == Constants.StatusStart.ReStart.ToString() ||
                startOrEnd.ToString() == Constants.StatusEnd.NormalEnd.ToString() ||
                startOrEnd.ToString() == Constants.StatusEnd.ForcedEnd.ToString())
            {
                var lstconveyor = _unitOfWork.ConveyorRepository.GetAll();
                foreach (var conveyor in lstconveyor)
                {
                    conveyor.F05_StrRtrSts = Constants.F05_StrRtrSts.NotUse.ToString("D");
                    conveyor.F05_BufferUsing = 0;
                    conveyor.F05_UsingTerm = null;
                    _unitOfWork.ConveyorRepository.Update(conveyor);
                }
                var lstDevice = _unitOfWork.DeviceRepository.GetMany(i => i.F14_DeviceCode.Trim() != "SYSTEM");
                foreach (var tm14Device in lstDevice)
                {
                    tm14Device.F14_DeviceStatus = Constants.F14_DeviceStatus.Online.ToString("D");
                    tm14Device.F14_UpdateDate = DateTime.Now;
                    _unitOfWork.DeviceRepository.Update(tm14Device);
                }
            }
            var device = _unitOfWork.DeviceRepository.GetMany(i => i.F14_DeviceCode.Trim() == "SYSTEM").FirstOrDefault();
            if (device != null)
            {
                if (Constants.StatusStart.NormalStart.Equals(startOrEnd) ||
                    Constants.StatusStart.ReStart.Equals(startOrEnd))
                {
                    device.F14_DeviceStatus = Constants.StatusStart.NormalStart.ToString("D");
                }
                else
                {
                   device.F14_DeviceStatus = Constants.StatusEnd.NormalEnd.Equals(startOrEnd)
                            ? Constants.StatusEnd.NormalEnd.ToString("D")
                            : Constants.StatusEnd.ForcedEnd.ToString("D");
                }
                device.F14_UpdateDate = DateTime.Now;
                _unitOfWork.DeviceRepository.Update(device);
            }
            
            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        public TM14_Device GetStatus()
        {
            var device = _unitOfWork.DeviceRepository.GetAll().FirstOrDefault(i => i.F14_DeviceCode.Equals("SYSTEM"));
            return device;
        }

        #endregion
    }
}
