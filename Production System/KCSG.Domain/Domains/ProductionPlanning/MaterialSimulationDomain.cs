using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.ProductionPlanning;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;
using log4net.Repository;

namespace KCSG.Domain.Domains.ProductionPlanning
{
    public class MaterialSimulationDomain:BaseDomain,IMaterialSimulationDomain
    {
          

        #region Constructor
        public MaterialSimulationDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
           
        }
        #endregion
      
        public Dictionary<string, double> GenerateMaterial(DateTime @from, DateTime to, bool inventoryUnderRetrieval,
            bool acceptedMaterialOnly,
            bool materialUsedInOtherCommands, string selectMaterial)
        {
           
          
            //lr_amount = w_tcpp051f.wf_getamount(ls_code)
            var lstAmount = GetAmount(selectMaterial, inventoryUnderRetrieval, acceptedMaterialOnly);
            //// get everyday cost and display it
            //Date ld_everyday
            var dcResult = new Dictionary<string, double>();
            //ld_everyday = w_tcpp051f.id_start
            //ii_array = 1
            //DO WHILE ld_everyday <= w_tcpp051f.id_end
            while (from <= to)
            {
            //    lr_amount = wf_everydaycost(ld_everyday, lr_amount)
                  lstAmount = EveryDayCost(from, materialUsedInOtherCommands, selectMaterial, lstAmount);
            ////	IF ld_everyday >= w_tcpp051f.id_start THEN
            //        istr_display[ii_array].day = ld_everyday
            //        istr_display[ii_array].value = lr_amount
                  dcResult.Add(from.ToString("dd/MM/yyyy"),ConvertHelper.ToDouble(lstAmount.ToString("F")));
            //        ii_array = ii_array + 1
                from = from.AddDays(1);
            ////	END IF
            //    ld_everyday = RelativeDate(ld_everyday, 1)
            //LOOP
            }
            return dcResult;
        }

        public IEnumerable<PreProductPlanSimuItem> GenerateProductPlan(DateTime from, DateTime to, bool inventoryUnderRetrieval, bool acceptedMaterialOnly, bool materialUsedInOtherCommands)
        {
            //var i = 0;
            //var dcMaterialAndAmount =  SaveAmount(ref i, inventoryUnderRetrieval, acceptedMaterialOnly);
            var lstAmount = SaveAmount(inventoryUnderRetrieval, acceptedMaterialOnly);
            var istr_pd = new List<PreProductPlanSimuItem>();
            while (from <= to)
            {
                var pd = new PreProductPlanSimuItem() { Date = from.ToString("dd/MM/yyyy"), Count = 0,PreproductISimulerItems = new List<PreproductISimulerItem>()};
                var pdtpln =
                    _unitOfWork.PdtPlnRepository.GetAll().ToList()
                        .Where(i => i.F39_KndEptBgnDate.Date == from.Date)
                        .OrderBy(i => i.F39_UpdateDate);
                foreach (var tx39PdtPln in pdtpln)
                {
                    if (!IsStatus(from, tx39PdtPln, materialUsedInOtherCommands))
                    {
                        Preproduct(tx39PdtPln.F39_PreProductCode, tx39PdtPln.F39_KneadingLine,
                            tx39PdtPln.F39_PrePdtLotAmt, tx39PdtPln.F39_KndCmdNo, ref pd, inventoryUnderRetrieval, acceptedMaterialOnly, ref  lstAmount);
                    }
                }
                istr_pd.Add(pd);
                from = from.AddDays(1);
           
        }
            return istr_pd;
        }

        public IEnumerable<TM03_PreProduct> GetPreProductName(DateTime from, DateTime to)
        {
            var pdtpln =
                _unitOfWork.PdtPlnRepository.GetAll()
                    .Where(i => i.F39_KndEptBgnDate >= from && i.F39_KndEptBgnDate <= to).OrderBy(i=>i.F39_UpdateDate);
                     
            var lstPreProduct =
                _unitOfWork.PreProductRepository.GetAll();
            var lstJoidTable =
                lstPreProduct.Join(pdtpln, tm03 => tm03.F03_PreProductCode, tx39PdtPln => tx39PdtPln.F39_PreProductCode,
                    (tm03, tx39PdtPln) => new {tm03, tx39PdtPln}).OrderBy(@t => @t.tx39PdtPln.F39_UpdateDate)
                    .Select(@t => new PreProductItem()
                    {
                        F03_PreProductCode = @t.tm03.F03_PreProductCode,
                        F03_PreProductName = @t.tm03.F03_PreProductName,
                    });
            var lstResult = new List<PreProductItem>();
            foreach (var preProductItem in lstJoidTable)
            {
                if (lstResult.All(i => i.F03_PreProductCode != preProductItem.F03_PreProductCode))
                {
                    lstResult.Add(preProductItem);
                }
            }
                

            return lstResult;
        }


        public ResponseResult<GridResponse<SimulationPopUpItem>> GetDataPopUp(DateTime startDate,DateTime endDate,string preProductCode, string date, bool inventoryUnderRetrieval, bool acceptedMaterialOnly,bool materialUsedInOtherCommands, GridSettings settings)
        {
            var lstPreProductPlanSimuItem = GenerateProductPlan(startDate, endDate, inventoryUnderRetrieval, acceptedMaterialOnly,
                materialUsedInOtherCommands);

            var lstResult =
                lstPreProductPlanSimuItem.FirstOrDefault(i => i.Date == date)
                    .PreproductISimulerItems.FirstOrDefault(i => i.PreProductCode == preProductCode).SimulationPopUpItems;

            var resultModel = new GridResponse<SimulationPopUpItem>(lstResult,0);
            return new ResponseResult<GridResponse<SimulationPopUpItem>>(resultModel, true);
        }

        #region Private
        #region w_tcpp053
        private double EveryDayCost(DateTime date,bool materialUsedInOtherCommands,string materialCode,double amount)
        {
            //            DECLARE C_preproduct CURSOR FOR  
            //  SELECT tx39_pdtpln.f39_preproductcode,   
            //         tx39_pdtpln.f39_prepdtlotamt,   
            //         tm03_preproduct.f03_batchlot  
            //    FROM tx39_pdtpln,   
            //         tm03_preproduct  
            //   WHERE ( tm03_preproduct.f03_preproductcode = tx39_pdtpln.f39_preproductcode ) and  
            //         ( tx39_pdtpln.f39_kndeptbgndate = :ad_current )
            //ORDER BY tx39_pdtpln.f39_updatedate ASC
            //   USING SQLCA1   ;
           var pdtpln =
                _unitOfWork.PdtPlnRepository.GetAll()
                    .Where(i => i.F39_KndEptBgnDate == date)
                    .OrderBy(i => i.F39_UpdateDate);
            var lstBaclot =
                _unitOfWork.PreProductRepository.GetAll()
                    .Where(i => pdtpln.Select(o => o.F39_PreProductCode.Trim()).Contains(i.F03_PreProductCode.Trim()));
            //String ls_preproduct
            //Integer li_lotnumber, li_batchlot
            //Real lr_amount = 0, lr_tmp

            //// open the cursor
            //OPEN C_preproduct;

            //// Fetch the first row from the result set.
            //FETCH C_preproduct INTO :ls_preproduct, :li_lotnumber, :li_batchlot;

            //// Loop through result set until exhausted
            //DO WHILE SQLCA1.sqlcode = 0
            var lr_amount = 0.0;
            foreach (var tx39PdtPln in pdtpln)
            {
            //    IF w_tcpp051f.wf_if_status(ls_preproduct, ad_current) = FALSE THEN
                if (!IsStatus(date, tx39PdtPln, materialUsedInOtherCommands))
                {
                    //        FETCH C_preproduct INTO :ls_preproduct, :li_lotnumber, :li_batchlot;
                    //        continue
                    var backlot = lstBaclot.FirstOrDefault(i => i.F03_PreProductCode.Trim().Equals(tx39PdtPln.F39_PreProductCode.Trim()));
                    //    END IF
                    //    lr_tmp = wf_materialcost(ls_preproduct, em_mtcode.text, li_lotnumber, li_batchlot)
                    var trmAmount = MaterialCost(tx39PdtPln.F39_PreProductCode, materialCode, backlot.F03_BatchLot,
                        tx39PdtPln.F39_PrePdtLotAmt);
                    //    lr_amount = lr_amount + lr_tmp
                    lr_amount += trmAmount;
                    //    FETCH C_preproduct INTO :ls_preproduct, :li_lotnumber, :li_batchlot;
                }
                //LOOP
            }
            //// All done, so close the cursor
            //CLOSE C_preproduct;

            //// return the result
            //lr_tmp = ar_lastday - lr_amount
            //return lr_tmp
            var flResult = amount - lr_amount;
            if (flResult < 0)
                flResult = 0;

            return flResult;
        }
        private double MaterialCost(string preProductCode, string materialCode, int backLot, int lotnumber)
        {
            //Real lr_3f, lr_4f
            //Real lr_result

            //SELECT sum(f02_3flayinamount),   
            //       sum(f02_4flayinamount)  
            //  INTO :lr_3f,   
            //       :lr_4f  
            //  FROM tm02_prepdtmkp  
            // WHERE ( f02_preproductcode = :as_preproduct ) AND  
            //       ( f02_materialcode = :as_material )   ;
            var lstPreDtmkp = _unitOfWork.PrePdtMkpRepository.GetAll()
                  .Where(i => i.F02_PreProductCode == preProductCode && i.F02_MaterialCode == materialCode);
            var sum3FlayInAmount = lstPreDtmkp.Any()?lstPreDtmkp.Sum(i => i.F02_3FLayinAmount):0;
            var sum4FlayInAmount = lstPreDtmkp.Any()? lstPreDtmkp.Sum(i => i.F02_4FLayinAmount):0;
            //IF SQLCA.SQLCode = -1 THEN
            //    f_tcdberror(SQLCA.SQLCode, SQLCA.SQLErrText)
            //    return 0
            //ELSEIF SQLCA.SQLCode = 100 THEN
            //    return 0
            if (!lstPreDtmkp.Any()) return 0;
            //ELSE
            //    IF IsNull(lr_3f) THEN
            //        lr_3f = 0
            //    END IF
            //    IF IsNull(lr_4f) THEN
            //        lr_4f = 0
            //    END IF
            //    lr_result = (lr_3f + lr_4f) * ai_batch * ai_lot
            var reuslt = (sum3FlayInAmount + sum4FlayInAmount) * backLot * lotnumber;
            //    return lr_result
            //END IF
            return reuslt;
            }
        #endregion
        #region w_tcpp051 method
        private bool IsStock(string palletNo, string materialCode, bool inventoryUnderRetrieval)
            {

            //// select stock from tx33_mtrshfstk
            //String ls_stock_flag
            //// datetime ldt_date

            //SELECT DISTINCT f33_stockflag   
            //  INTO :ls_stock_flag   
            //  FROM tx33_mtrshfstk  
            // WHERE ( f33_palletno = :as_pallet ) AND  
            //       ( f33_materialcode = :as_material )   ;
            var mtrshfstk =
                _unitOfWork.MaterialShelfStockRepository.GetMany(
                    i => i.F33_PalletNo == palletNo && i.F33_MaterialCode == materialCode).FirstOrDefault();
            //// if database error of not found
            //IF SQLCA.SQLCode = -1 THEN
            //    f_tcdberror(SQLCA.SQLCode, SQLCA.SQLErrText)
            //    return FALSE
            //ELSEIF SQLCA.SQLCode = 100 THEN
            //    return FALSE
            if (mtrshfstk == null)
            {
                return false;
            }
            //END IF

            //// if found
            //IF ls_stock_flag = tx33_stkflg_stk THEN	// the material's stock flag is 3
            //    return TRUE
            if (mtrshfstk.F33_StockFlag == Constants.TX33_MtrShfStk.Stocked.ToString("D"))
            {
                return true;
            }
            //ELSEIF ib_stock = TRUE THEN
            //    IF ls_stock_flag = tx33_stkflg_notstk THEN	// the material's stock flag is 0
            //        return TRUE
            if (inventoryUnderRetrieval)
            {
                return mtrshfstk.F33_StockFlag == Constants.TX33_MtrShfStk.NotInStock.ToString("D");
            }
            //    ELSE
            //        return FALSE
            return false;
            //    END IF
            //ELSE 
            //    return FALSE
            //END IF
        }
        private double GetAmount(string materialCode, bool inventoryUnderRetrieval, bool acceptedMaterialOnly)
        {
            //Real lr_amount = 0, lr_tmp
            var amount = 0.0;
            //// declare a cursor for select palletno in tx33_mtrshfstk
            //DECLARE C_palletno CURSOR FOR  
            // SELECT DISTINCT f33_palletno  
            //   FROM tx33_mtrshfstk  
            //  WHERE f33_materialcode = :as_material
            //  USING SQLCA1   ;
            var lstmtrshfstk = _unitOfWork.MaterialShelfStockRepository.GetMany(i => i.F33_MaterialCode == materialCode);
            //String ls_palletno
            //// open the cursor
            //OPEN C_palletno;
            //// Fetch the first row from the result set.
            //FETCH C_palletno INTO :ls_palletno;
            //// Loop through result set until exhausted
            //DO WHILE SQLCA1.sqlcode = 0
            foreach (var tx33MtrShfStk in lstmtrshfstk)
            {
                //    // add to the amount
                //    IF wf_if_stock(ls_palletno, as_material) AND wf_if_accept(ls_palletno) THEN
                if (IsStock(tx33MtrShfStk.F33_PalletNo, materialCode, inventoryUnderRetrieval) &&
                    IsAccept(tx33MtrShfStk.F33_PalletNo, acceptedMaterialOnly))
                {
                    //        // sum the amount
                    //       SELECT sum(f33_amount)  
                    //        INTO :lr_tmp  
                    //        FROM tx33_mtrshfstk  
                    //       WHERE ( f33_palletno = :ls_palletno ) AND  
                    //             ( f33_materialcode = :as_material )   ;

                    //        lr_amount = lr_amount + lr_tmp
                    amount += tx33MtrShfStk.F33_Amount;
                    //    END IF
                }

                //    // Fetch the next row from the result set
                //    FETCH C_palletno INTO :ls_palletno;

                //LOOP
            }
            //// All done, so close the cursor
            //CLOSE C_palletno;
            //// return the material amount
            //return lr_amount
            return amount;
        }
        private bool IsStatus(DateTime date, TX39_PdtPln pdtPln, bool materialUsedInOtherCommands)
        {

            //String ls_status_flag
            //SELECT f39_status  
            //  INTO :ls_status_flag  
            //  FROM tx39_pdtpln  
            // WHERE ( f39_preproductcode = :as_code ) AND  
            //       ( f39_kndeptbgndate = :as_date )   ;

            //// if database error of not found
            //IF SQLCA.SQLCode = -1 THEN
            //    f_tcdberror(SQLCA.SQLCode, SQLCA.SQLErrText)
            //    return FALSE
            //ELSEIF SQLCA.SQLCode = 100 THEN
            //    return FALSE
            if (pdtPln == null)
            {
                return false;
            }
            //END IF

            //// if found
            //IF ls_status_flag = tx39_sts_notcmd THEN	// include material used in other commands
            //    return TRUE
            if (pdtPln.F39_Status == Constants.F39_Status.NotCommanded.ToString("D"))
            {
                return true;
            }
            //ELSEIF ib_status = FALSE THEN
            //    IF ls_status_flag = tx39_sts_cmdover THEN	// only unused material
            //        return TRUE
            if (!materialUsedInOtherCommands)
            {
                return pdtPln.F39_Status == Constants.F39_Status.Commanded.ToString("D");
            }
            //    ELSE
            //        return FALSE
            return false;
            //    END IF
            //ELSE
            //    return FALSE
            //END IF
        }
        private bool IsAccept(string paletNo, bool acceptedMaterialOnly)
        {
            //f_tcdebuglog( "Enter: function w_tcpp051::wf_if_accept()" )
            //// include all inventory
            //IF ib_accept = FALSE THEN
            //    return TRUE
            //END IF
            if (!acceptedMaterialOnly)
            {
                return true;
            }
         
            //// only accepted material
            //String ls_accept_flag, ls_prcordno, ls_prtdvrno

            //SELECT f32_prcordno, f32_prtdvrno
            //  INTO :ls_prcordno, :ls_prtdvrno
            //  FROM tx32_mtrshf
            // WHERE f32_palletno = :as_palletno;
            var mtrshf = _unitOfWork.MaterialShelfRepository.GetAll().FirstOrDefault(i => i.F32_PalletNo == paletNo);
            //IF SQLCA.SQLCode = -1 THEN	// database error
            //    f_tcdberror(SQLCA.SQLCode ,SQLCA.SQLErrText)
            //    return FALSE
            //ELSEIF SQLCA.SQLCode = 100 THEN	// not found
            //    return FALSE
            if (mtrshf == null)
            {
                return false;
            }
            //ELSEIF IsNull(ls_prcordno) OR Trim(ls_prcordno) = "" OR IsNull(ls_prtdvrno) OR Trim(ls_prtdvrno) = "" THEN	// if found
            //    return TRUE
            //END IF
            if (string.IsNullOrEmpty(mtrshf.F32_PrcOrdNo) || string.IsNullOrEmpty(mtrshf.F32_PrtDvrNo))
            {
                return true;
            }
            //SELECT f30_acceptclass  
            //  INTO :ls_accept_flag  
            //  FROM tx30_reception
            // WHERE ( f30_prcordno = :ls_prcordno ) and
            //         ( f30_prtdvrno = :ls_prtdvrno );
            var acceptClass = _unitOfWork.ReceptionRepository.GetMany(
                i => i.F30_PrcOrdNo == mtrshf.F32_PrcOrdNo && i.F30_PrtDvrNo == mtrshf.F32_PrtDvrNo).FirstOrDefault();
            //IF SQLCA.SQLCode = -1 THEN	// database error
            //    f_tcdberror(SQLCA.SQLCode ,SQLCA.SQLErrText)
            //    return FALSE
            //ELSEIF SQLCA.SQLCode = 100 THEN	// not found
            //    return FALSE
            if (acceptClass == null)
            {
                return false;
            }
            //ELSEIF ls_accept_flag = tx30_acpcls_acp THEN	// if found
            //    return TRUE
            return acceptClass.F30_AcceptClass == Constants.TX30_Reception.Accepted.ToString("D");
            //ELSE
            //    return FALSE
            //END IF
        }

        #endregion
        //count begin 0
        private Dictionary<string, double> SaveAmount(bool inventoryUnderRetrieval,
            bool acceptedMaterialOnly)
        {
            var istr_amount = new Dictionary<string, double>();
            //f_tcdebuglog( "Enter: function w_tcpp051::wf_saveamount()" )
            //// save all material amount
            // DECLARE C_code CURSOR FOR  
            //  SELECT f01_materialcode	
            //    FROM tm01_material
            //    USING SQLCA2  ;
            var lstMaterial = _unitOfWork.MaterialRepository.GetAll();
            //OPEN C_code;
            //String ls_code
            //Real lr_amount
            //ii_count = 1
            //FETCH C_code INTO :ls_code;
            //DO WHILE SQLCA2.SQLCode = 0
            foreach (var tm01Material in lstMaterial)
            {
                //    lr_amount = wf_getamount(ls_code)
                //    istr_amount[ii_count].code = ls_code
                //    istr_amount[ii_count].amount = lr_amount
                //    ii_count = ii_count + 1
                var amount = GetAmount(tm01Material.F01_MaterialCode, inventoryUnderRetrieval, acceptedMaterialOnly);
                istr_amount.Add(tm01Material.F01_MaterialCode, amount);
                
                //    FETCH C_code INTO :ls_code;
                //LOOP

                //CLOSE C_code;

                //ii_count = ii_count - 1
            }
            return istr_amount;

                }


        private void Preproduct(string f39PreProductCode, string f39KneadingLine, int f39PrePdtLotAmt, string f39KndCmdNo , ref PreProductPlanSimuItem item,bool inventoryUnderRetrieval,bool acceptedMaterialOnly,ref Dictionary<string,double> lstAmount )
        {
            item.Count += 1;
            var count = item.Count;
            var preProduct = _unitOfWork.PreProductRepository.GetMany(i => i.F03_PreProductCode == f39PreProductCode).FirstOrDefault();
            var pItem = new PreproductISimulerItem();
           
            pItem.KndLine = f39KneadingLine==Constants.F39_KneadingLine.Megabit.ToString("D");
            pItem.Code = count;
            if (preProduct != null)
            {
            pItem.Name = preProduct.F03_PreProductName;
                pItem.PreProductCode = preProduct.F03_PreProductCode;
                pItem.batch = preProduct.F03_BatchLot;
            }
            pItem.Command = f39KndCmdNo;
            pItem.Lot = f39PrePdtLotAmt;
           
            pItem.Count = 0;
            var prepdtmkp =
                _unitOfWork.PrePdtMkpRepository.GetAll().Where(i => i.F02_PreProductCode == f39PreProductCode);
            var checkSimulation = true;
            pItem.SimulationPopUpItems  = new List<SimulationPopUpItem>();
            foreach (var tm02prepdtmkp in prepdtmkp)
            {
                var simulationPopUp = new SimulationPopUpItem();
                var flag = Material(tm02prepdtmkp.F02_MaterialCode, tm02prepdtmkp.F02_3FLayinAmount,
                    tm02prepdtmkp.F02_4FLayinAmount, preProduct.F03_BatchLot, f39PrePdtLotAmt, ref lstAmount, ref simulationPopUp);
                if (!flag)
                {
                    checkSimulation = false;
                }
                pItem.SimulationPopUpItems .Add(simulationPopUp);
            }
            if (prepdtmkp.Any())
            {
                pItem.Simulation = !checkSimulation ? "NG" : "OK";
            }
            item.PreproductISimulerItems.Add(pItem);
        }

      
        private bool Material( string materialCode, double f02_3FLayinAmount, double f02_4FLayinAmount,
            int f03BatchLot, int f39PrePdtLotAmt, ref Dictionary<string, double> totalAmount, ref SimulationPopUpItem popUpItem)
        {

//f_tcdebuglog( "Enter: function w_tcpp054::wf_material()" )
//Integer li_count
//istr_pd[ii_count].preproduct[ai_count].count += 1
//li_count = istr_pd[ii_count].preproduct[ai_count].count
           
//String ls_name
//SELECT f01_materialdsp  
//  INTO :ls_name	
//  FROM tm01_material  
// WHERE f01_materialcode = :as_code   ;
            var material = _unitOfWork.MaterialRepository.GetById(materialCode);
         var amount =    totalAmount.FirstOrDefault(i => i.Key.Trim() == materialCode.Trim());
//Real lr_qty, lr_stock, lr_remainder
//lr_qty = ( ar_3f + ar_4f ) * ai_batch * ai_lot

//Integer li_i
//Boolean lb_flag
//FOR li_i = 1 TO w_tcpp051f.ii_count
//    IF w_tcpp051f.istr_amount[li_i].code = as_code THEN
//        lr_stock = w_tcpp051f.istr_amount[li_i].amount
//        lr_remainder = lr_stock - lr_qty
//        w_tcpp051f.istr_amount[li_i].amount = lr_remainder
            var lr_Qty = (f02_3FLayinAmount + f02_4FLayinAmount) * f03BatchLot * f39PrePdtLotAmt;
            var lrReminder = amount.Value - lr_Qty;
            if (amount.Key!=null)
            {
                totalAmount.Remove(amount.Key);
                totalAmount.Add(amount.Key, lrReminder);
            }
            else
            {
                totalAmount.Add(materialCode, lrReminder);
            }
           
//        IF lr_remainder < 0 THEN
//            lb_flag = FALSE
//        ELSE
//            lb_flag = TRUE
//        END IF
//        exit
//    END IF
//NEXT
          
                popUpItem.MaterialCode = materialCode;
                if (material != null)
                {
                    popUpItem.MaterialName = material.F01_MaterialDsp;
                }
                popUpItem.RequiredQuantity = lr_Qty;
                popUpItem.Remainder = lrReminder;
                popUpItem.Stock = amount.Value;
           
          

//istr_pd[ii_count].preproduct[ai_count].material[li_count].code = as_code
//istr_pd[ii_count].preproduct[ai_count].material[li_count].name = ls_name
//istr_pd[ii_count].preproduct[ai_count].material[li_count].qty = lr_qty
//istr_pd[ii_count].preproduct[ai_count].material[li_count].stock = lr_stock
//istr_pd[ii_count].preproduct[ai_count].material[li_count].remainder = lr_remainder

//return lb_flag
            return !(lrReminder < 0);
        }


    
        #endregion
    }
}
