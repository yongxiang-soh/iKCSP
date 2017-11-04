using System;
using System.Linq;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.TabletisingCommandSubSystem;

namespace KCSG.Domain.Domains.TabletisingCommandSubSystem
{
    public class PreProductChargingDomain : IPreProductCharging
    {
        private readonly IUnitOfWork _unitOfWork;

        public PreProductChargingDomain(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //public PreProductChargingItem GetPreProductChargingItem(string kndcmdno,string prepdtlotno,string tabletline)
        //{
        //    var data = new PreProductChargingItem
        //    {
        //       KneadingCmdNo = kndcmdno,
        //       PreProductLotNo = prepdtlotno,
        //       IsError = false
        //    };
        //    var tx41 =
        //        _unitOfWork.TabletCommandRepository.Get(
        //            x => x.F41_Status.Trim().Equals("2") && x.F41_TabletLine.Trim() == tabletline);

        //    if (tx41 == null)
        //    {
        //        // Check Table tx56_TbtPdt
        //        if (!string.IsNullOrWhiteSpace(kndcmdno) && !string.IsNullOrWhiteSpace(prepdtlotno))
        //        {
        //            var tx56 =
        //                _unitOfWork.TabletProductRepository.Get(
        //                    x =>
        //                        x.F56_KndCmdNo.Trim().Equals(kndcmdno.Trim()) &&
        //                        x.F56_PrePdtLotNo.Trim().Equals(prepdtlotno.Trim()) &&
        //                        x.F56_Status.Trim().Equals("1"));
        //            if (tx56 != null)
        //            {
        //                var tx09 =
        //                    _unitOfWork.ProductRepository.Get(
        //                        x => x.F09_ProductCode.Trim().Equals(tx56.F56_ProductCode.Trim()));
        //                data.ProductCode = tx56.F56_ProductCode.Trim();
        //                data.ProductLotNo = tx56.F56_ProductLotNo.Trim();
        //                data.ProductName = tx09.F09_ProductDesp.Trim();
        //            }
        //        }
        //        var tx41_1 = _unitOfWork.TabletCommandRepository.Get(
        //            x => x.F41_KndCmdNo.Trim().Equals(kndcmdno) && x.F41_PrePdtLotNo.Trim() == prepdtlotno);
        //        if (tx41_1 != null)
        //        {
        //            if (tx41_1.F41_Status.Trim().Equals("2") || tx41_1.F41_Status.Trim().Equals("3"))
        //            {
        //                data.ContainerCode1 = "";
        //                data.PreProductCode = "";
        //                data.PreProductLotNo = "";
        //                data.PreProductName = "";
        //                data.KneadingCmdNo = "";
        //                data.ProductCode = "";
        //                data.ProductName = "";
        //            }
        //        }

        //    }
        //    else
        //    {
        //        {
        //            if (tx41.F41_RtrEndCntAmt == tx41.F41_ChgCntAmt)
        //            {
        //                // UPDATE Tx41_TbtCmd SET Tx41_TbtCmd.F41_Status = :TX41_START[1], tx41_tbtcmd.f41_updatedate = getdate() &
        //                var tx41_2 = _unitOfWork.TabletCommandRepository.Get(
        //                    x => x.F41_KndCmdNo.Trim().Equals(kndcmdno) && x.F41_PrePdtLotNo.Trim() == prepdtlotno);
        //                if (tx41_2 != null)
        //                {
        //                    tx41_2.F41_Status = "1";
        //                    tx41_2.F41_UpdateDate = DateTime.Now;
        //                    _unitOfWork.TabletCommandRepository.Update(tx41_2);
        //                }
        //                data.IsError = true;
        //                data.ErrorCode = "MSG38";
        //            }
        //            else
        //            {
        //                tx41.F41_ChgCntAmt += 1;
        //                var tx49 =
        //                    _unitOfWork.PreProductShelfStockRepository.Get(
        //                        x =>
        //                            x.F49_KndCmdNo.Trim().Equals(kndcmdno) &&
        //                            x.F49_PreProductLotNo.Trim().Equals(prepdtlotno) &&
        //                            x.F49_ContainerSeqNo.Equals(tx41.F41_ChgCntAmt));
        //                if (tx49 == null)
        //                {
        //                    data.IsError = true;
        //                    data.ErrorCode = "MSG39";
        //                }
        //                else
        //                {
        //                    data.LockStatus = Constants.LockStatus.Lock;
        //                    var tm03 = _unitOfWork.PreProductRepository.Get(
        //                            x => x.F03_PreProductCode.Trim().Equals(tx41.F41_PreproductCode.Trim()));
        //                    var tx56 =
        //                   _unitOfWork.TabletProductRepository.Get(
        //                       x =>
        //                           x.F56_KndCmdNo.Trim().Equals(kndcmdno.Trim()) &&
        //                           x.F56_PrePdtLotNo.Trim().Equals(prepdtlotno.Trim()) &&
        //                           x.F56_Status.Trim().Equals("1"));

        //                    data.PreProductCode = tx41.F41_PreproductCode.Trim();
        //                    data.PreProductLotNo = tx41.F41_PrePdtLotNo.Trim();
        //                    data.PreProductName = tm03.F03_PreProductName.Trim();

        //                    data.ProductCode = tx56.F56_ProductCode.Trim();
        //                    data.ProductLotNo = tx56.F56_ProductLotNo.Trim();
        //                    data.ProductName = tm03.F03_PreProductName.Trim();
        //                    data.ContainerCode1 = tx49.F49_ContainerCode.Trim();
        //                }
        //            }

        //        }
        //    }
        //    _unitOfWork.Commit();
        //    return data;
        //}

        public PreProductChargingItem GetPreProductChargingItemByContainerCode(string containerCode, string termonalno)
        {
            var tabletline = string.Empty;
            var terminal = _unitOfWork.TerminalRepository.Get(c => c.F06_TerminalNo.Trim() == termonalno.Trim());
            if (terminal != null)
                tabletline = terminal.F06_TabletLine?.Trim();
            var data = new PreProductChargingItem
            {
                IsError = false
            };
            var kndcmdno = "";
            var prepdtlotno = "";
            if (!string.IsNullOrEmpty(containerCode))
            {
                var tx49 =
                    _unitOfWork.PreProductShelfStockRepository.Get(
                        x =>
                            x.F49_ContainerCode.Trim().Equals(containerCode));
                if (tx49 == null)
                {
                    data.IsError = true;
                    data.ErrorCode = "Cannot find corresponding records from DB !";
                    return data;
                }
                kndcmdno = tx49.F49_KndCmdNo.Trim();
                prepdtlotno = tx49.F49_PreProductLotNo.Trim();
            }



            var tx41 =
                _unitOfWork.TabletCommandRepository.Get(
                    x => x.F41_Status.Trim().Equals("2") && (x.F41_TabletLine.Trim() == tabletline));
            if (tx41 == null)
            {
                data.IsError = true;
                data.ErrorCode = "Cannot find corresponding records from DB !";
                // Check Table tx56_TbtPdt
                if (!string.IsNullOrWhiteSpace(kndcmdno) && !string.IsNullOrWhiteSpace(prepdtlotno))
                {
                    var tx56 =
                        _unitOfWork.TabletProductRepository.Get(
                            x =>
                                x.F56_KndCmdNo.Trim().Equals(kndcmdno.Trim()) &&
                                x.F56_PrePdtLotNo.Trim().Equals(prepdtlotno.Trim()) &&
                                x.F56_Status.Trim().Equals("1"));
                    if (tx56 != null)
                    {
                        var tx09 =
                            _unitOfWork.ProductRepository.Get(
                                x => x.F09_ProductCode.Trim().Equals(tx56.F56_ProductCode.Trim()));

                        data.ProductCode = tx56.F56_ProductCode.Trim();
                        data.ProductLotNo = tx56.F56_ProductLotNo.Trim();
                        data.ProductName = tx09 != null ? tx09.F09_ProductDesp.Trim() : "";

                        //data.ContainerCode1 = containerCode;
                        data.KneadingCmdNo = kndcmdno;
                        data.PreProductLotNo = prepdtlotno;
                    }
                }
                var tx41_1 = _unitOfWork.TabletCommandRepository.Get(
                    x => x.F41_KndCmdNo.Trim().Equals(kndcmdno) && (x.F41_PrePdtLotNo.Trim() == prepdtlotno));
                if (tx41_1 != null)
                {
                    if (tx41_1.F41_Status.Trim().Equals("7") || tx41_1.F41_Status.Trim().Equals("8") ||
                        tx41_1.F41_Status.Trim().Equals("9"))
                    {
                        //data.ContainerCode1 = "";
                        data.PreProductCode = "";
                        data.PreProductLotNo = "";
                        data.PreProductName = "";
                        data.KneadingCmdNo = "";
                        data.ProductCode = "";
                        data.ProductName = "";
                    }
                }
                else
                {
                    //data.ContainerCode1 = "";
                    data.PreProductCode = "";
                    data.PreProductLotNo = "";
                    data.PreProductName = "";
                    data.KneadingCmdNo = "";
                    data.ProductCode = "";
                    data.ProductName = "";
                }
            }
            else
            {
                if (tx41.F41_RtrEndCntAmt == tx41.F41_ChgCntAmt)
                {
                    // UPDATE Tx41_TbtCmd SET Tx41_TbtCmd.F41_Status = :TX41_START[1], tx41_tbtcmd.f41_updatedate = getdate() &
                    var tx41_2 = _unitOfWork.TabletCommandRepository.Get(
                        x =>
                            x.F41_KndCmdNo.Trim().Equals(kndcmdno.Trim()) &&
                            (x.F41_PrePdtLotNo.Trim() == prepdtlotno.Trim()));
                    if (tx41_2 != null)
                    {
                        tx41_2.F41_Status = "1";
                        tx41_2.F41_UpdateDate = DateTime.Now;
                        _unitOfWork.TabletCommandRepository.Update(tx41_2);
                    }
                    data.IsError = true;
                    data.ErrorCode = "The charging container number is more than retrieved container amount !";
                }
                else
                {
                    var tm03 = _unitOfWork.PreProductRepository.Get(
                        x => x.F03_PreProductCode.Trim().Equals(tx41.F41_PreproductCode.Trim()));
                    var tx56 =
                        _unitOfWork.TabletProductRepository.Get(
                            x =>
                                x.F56_KndCmdNo.Trim().Equals(tx41.F41_KndCmdNo.Trim()) &&
                                x.F56_PrePdtLotNo.Trim().Equals(tx41.F41_PrePdtLotNo.Trim()) &&
                                x.F56_Status.Trim().Equals("1"));
                    if (tx56 != null)
                    {
                        var tx09 =
                            _unitOfWork.ProductRepository.Get(
                                x => x.F09_ProductCode.Trim().Equals(tx56.F56_ProductCode.Trim()));

                        data.KneadingCmdNo = tx41.F41_KndCmdNo.Trim();
                        data.PreProductCode = tx41.F41_PreproductCode.Trim();
                        data.PreProductLotNo = tx41.F41_PrePdtLotNo.Trim();
                        data.PreProductName = tm03 != null ? tm03.F03_PreProductName.Trim() : "";

                        data.ProductCode = tx56.F56_ProductCode.Trim();
                        data.ProductLotNo = tx56.F56_ProductLotNo.Trim();
                        data.ProductName = tx09 != null ? tx09.F09_ProductDesp.Trim() : "";
                        var tx49 = 
                            _unitOfWork.PreProductShelfStockRepository.Get(
                                x =>
                                    x.F49_PreProductLotNo.Trim().Equals(tx41.F41_PrePdtLotNo)&&x.F49_KndCmdNo==tx41.F41_KndCmdNo&&x.F49_PreProductCode == tx41.F41_PreproductCode);
                        if (tx49!=null)
                        {
                            data.ContainerCode1 = tx49.F49_ContainerCode.Trim();
                        }
                       
                    }
                }
            }

            _unitOfWork.Commit();
            return data;
        }

        public void UpdatePreProductCharging(PreProductChargingItem model)
        {
            var tx41 = _unitOfWork.TabletCommandRepository.Get(
                x =>
                    x.F41_KndCmdNo.Trim().Equals(model.KneadingCmdNo) &&
                    (x.F41_PrePdtLotNo.Trim() == model.PreProductLotNo));

            if (tx41 != null)
            {
                var tx49 =
                    _unitOfWork.PreProductShelfStockRepository.Get(
                        x =>
                            x.F49_ContainerCode.Trim().Equals(model.ContainerCode1));
                if (tx49 != null)
                {
                    if (tx41.F41_ChgCntAmt < tx49.F49_ContainerSeqNo)
                        tx41.F41_ChgCntAmt += 1;
                    tx41.F41_UpdateCount += 1;
                    tx41.F41_Status = "1";
                    tx41.F41_UpdateDate = DateTime.Now;
                    _unitOfWork.TabletCommandRepository.Update(tx41);

                    tx49.F49_ShelfStatus = "4";
                    tx49.F49_UpdateDate = DateTime.Now;
                    _unitOfWork.PreProductShelfStockRepository.Update(tx49);
                    _unitOfWork.Commit();
                }
            }
        }

        public IQueryable<TX49_PrePdtShfStk> GetTX49_PrePdtShfStkByCode(string code)
        {
            var tx49 = _unitOfWork.PreProductShelfStockRepository.GetAll().Where(
                c => c.F49_ContainerCode.Trim() == code.Trim());
            return tx49;
        }


        public TX41_TbtCmd GetTX41_TbtCmdByCmd(string cmdNo)
        {
            var tx49 = _unitOfWork.TabletCommandRepository.GetAll().FirstOrDefault(
                c => c.F41_KndCmdNo.Trim() == cmdNo.Trim());
            return tx49;
        }
    }
}