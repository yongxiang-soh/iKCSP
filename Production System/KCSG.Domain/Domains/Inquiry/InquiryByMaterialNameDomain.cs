using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.Inquiry;
using KCSG.Domain.Models.Inquiry;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Data.Repositories;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.Inquiry
{
    public class InquiryByMaterialNameDomain : BaseDomain, IInquiryByMaterialNameDomain
    {
        #region Constructor

        public InquiryByMaterialNameDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion
        public ResponseResult<GridResponse<MaterialShelfStatusItem>> SearchCriteria(string materialCode,
            GridSettings gridSettings,out double total)
        {            
            var materialShelfStatuss = _unitOfWork.MaterialShelfStatusRepository.GetAll();
            var materialShelfStocks = _unitOfWork.MaterialShelfStockRepository.GetAll();

            var material = Constants.F31_ShelfStatus.Material.ToString("D");
            if (!string.IsNullOrEmpty(materialCode))
            {
                var result = from materialShelfStatus in materialShelfStatuss
                             join materialShelfStock in materialShelfStocks on materialShelfStatus.F31_PalletNo equals materialShelfStock.F33_PalletNo

                             where materialShelfStock.F33_MaterialCode.Trim().Equals(materialCode.Trim()) &&
                                   materialShelfStatus.F31_ShelfStatus.Trim().Equals(material)                             
                             orderby new
                             {
                                 materialShelfStock.F33_MaterialLotNo,
                                 materialShelfStatus.F31_ShelfBay,
                                 materialShelfStatus.F31_ShelfRow
                             }
                             select
                             new MaterialShelfStatusItem
                             {
                                 F33_MaterialLotNo = materialShelfStock.F33_MaterialLotNo,
                                 F33_Amount = materialShelfStock.F33_Amount,
                                 F31_ShelfRow = materialShelfStatus.F31_ShelfRow,
                                 F31_ShelfBay = materialShelfStatus.F31_ShelfBay,
                                 F31_ShelfLevel = materialShelfStatus.F31_ShelfLevel,
                                 ShelfNo1 = materialShelfStatus.F31_ShelfRow + "-" + materialShelfStatus.F31_ShelfBay + "-" + materialShelfStatus.F31_ShelfLevel,
                             };


                var itemCount = result.Count();
                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);
                if (result.Any())
                {
                    total = result.Sum(i => i.F33_Amount);
                }
                else
                {
                    total = 0;
                }
                
                var resultModel = new GridResponse<MaterialShelfStatusItem>(result, itemCount);
                return new ResponseResult<GridResponse<MaterialShelfStatusItem>>(resultModel, true);
            }
            else
            {
                var result = from materialShelfStatus in materialShelfStatuss
                             join materialShelfStock in materialShelfStocks on materialShelfStatus.F31_PalletNo equals materialShelfStock.F33_PalletNo

                             where materialShelfStatus.F31_ShelfStatus.Trim().Equals(material)                                                                
                             orderby new
                             {
                                 materialShelfStock.F33_MaterialLotNo,
                                 materialShelfStatus.F31_ShelfBay,
                                 materialShelfStatus.F31_ShelfRow
                             }
                             select
                             new MaterialShelfStatusItem
                             {
                                 F33_MaterialLotNo = materialShelfStock.F33_MaterialLotNo,
                                 F33_Amount = materialShelfStock.F33_Amount,
                                 F31_ShelfRow = materialShelfStatus.F31_ShelfRow,
                                 F31_ShelfBay = materialShelfStatus.F31_ShelfBay,
                                 F31_ShelfLevel = materialShelfStatus.F31_ShelfLevel,
                                 ShelfNo1 = materialShelfStatus.F31_ShelfRow + "-" + materialShelfStatus.F31_ShelfBay + "-" + materialShelfStatus.F31_ShelfLevel,
                             };

                var itemCount = result.Count();
                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);
                total = result.Sum(i => i.F33_Amount);
                var resultModel = new GridResponse<MaterialShelfStatusItem>(result, itemCount);
                return new ResponseResult<GridResponse<MaterialShelfStatusItem>>(resultModel, true);
            }            
            //var itemCount = lstResult.Count();
            //var pdtPlnItems = lstResult.AsQueryable();
            //OrderByAndPaging(ref pdtPlnItems, gridSettings);
            //var resultModel = new GridResponse<PdtPlnItem>(pdtPlnItems, itemCount);
            //return new ResponseResult<GridResponse<PdtPlnItem>>(resultModel, true);
        }


        public bool DeleteMaterialName(string deleteDate, string deleteCode)
        {
            var mydate = ConvertHelper.ConvertToDateTimeFull(deleteDate);
            var status = Constants.F39_Status.Commanded.ToString();
            var date = ConvertHelper.ConvertToDateTimeFull(deleteDate);
            var pdtPlnItem =
                _unitOfWork.PdtPlnRepository.GetAll().FirstOrDefault(i => i.F39_KndEptBgnDate == date && i.F39_PreProductCode.Trim().Equals(deleteCode.Trim()));
            if (pdtPlnItem != null)
            {
                try
                {
                    var lstPreProduct =
                        _unitOfWork.KneadingCommandRepository.GetMany(
                            i =>
                                (i.F42_PreProductCode.Trim() == deleteCode.Trim()) &&
                                (i.F42_KndEptBgnDate.Year == mydate.Year) &&
                                (i.F42_KndEptBgnDate.Month == mydate.Month) && (i.F42_KndEptBgnDate.Day == mydate.Day));
                    foreach (var preProductItem in lstPreProduct)
                        _unitOfWork.KneadingCommandRepository.Delete(preProductItem);
                }
                catch
                {
                    return false;
                }


                pdtPlnItem.F39_KndCmdNo = new string(' ', 6);

                pdtPlnItem.F39_Status = Constants.F39_Status.NotCommanded.ToString("D");

                pdtPlnItem.F39_StartLotNo = new string(' ', 10);
                _unitOfWork.PdtPlnRepository.Update(pdtPlnItem);
            }
            _unitOfWork.Commit();
            return true;
        }


        public ResponseResult CreateOrUpdate(string selectedValue, int within)
        {
            //char[] charSeparators = new char[] { '#' };
            string[] s = selectedValue.TrimEnd('#').Trim().Split('#');
            var result = new List<TX39_PdtPln>();
            List<DateTime> lstSelectedDate = new List<DateTime>();
            List<string> lstSelectedCode = new List<string>();
            for (int i = 0; i < s.Length; i++)
            {
                //DateTime selectedDate = ConvertHelper.ConvertToDateTimeFull(s[i].Split(',')[0]);
                //var selectedCode = s[i].Split(',')[1];
                //lstSelectedDate.Add(selectedDate);
                //lstSelectedCode.Add(selectedCode);
                var date = ConvertHelper.ConvertToDateTimeFull(s[i].Split(',')[0]);
                var preProductCode = s[i].Split(',')[1];
                result.Add(_unitOfWork.PdtPlnRepository.GetAll().FirstOrDefault(c => c.F39_KndEptBgnDate == date && c.F39_PreProductCode.Trim() == preProductCode.Trim()));
            }
            var lstResult = result.OrderBy(i => i.F39_PrePdtLotAmt);

            //var result = _unitOfWork.PdtPlnRepository.GetMany(i => lstSelectedCode.Contains(i.F39_PreProductCode) && lstSelectedDate.Contains(i.F39_KndEptBgnDate)).OrderBy(i => i.F39_PrePdtLotAmt);

            var nomanageItem = _unitOfWork.NoManageRepository.GetMany(i => i.F48_SystemId == "00000").FirstOrDefault();

            var isCount = 0;
            if (result.Any())
            {
                foreach (var pdtPlnItem in lstResult)
                {
                    if (pdtPlnItem.F39_Status.Trim() == Constants.F39_Status.NotCommanded.ToString("D"))
                    {

                        DateTime productDate = pdtPlnItem.F39_KndEptBgnDate;
                        string day;
                        string month;
                        string year;
                        if (productDate == DateTime.MinValue)
                        {
                            var today = DateTime.Now.ToString("dd/MM/yy");

                            day = today.Split('/')[0];
                            month = today.Split('/')[1];
                            year = today.Split('/')[2];
                        }
                        else
                        {
                            var date = productDate.ToString("dd/MM/yy");
                            day = date.Split('/')[0];
                            month = date.Split('/')[1];
                            if (productDate.Month == 10)
                                month = "X";
                            if (productDate.Month == 11)
                                month = "Y";
                            if (productDate.Month == 12)
                                month = "Z";

                            year = date.Split('/')[2];
                        }

                        var bookNo = 0;
                        var konrenSequence = "";
                        var sequence1 = "";
                        var sequence2 = "000";
                        var preProductSequence = "";
                        int iI_kndcmdno;
                        var kneadingLine = pdtPlnItem.F39_KneadingLine;

                        #region Insert Or Update tx48
                        if (nomanageItem != null)
                        {
                            bookNo = nomanageItem.F48_KndCmdBookNo;
                            if (kneadingLine == Constants.KndLine.Megabit.ToString("D"))
                            {
                                iI_kndcmdno = nomanageItem.F48_MegaKndCmdNo;
                            }
                            else if (kneadingLine == Constants.KndLine.Conventional.ToString("D"))
                            {
                                iI_kndcmdno = nomanageItem.F48_GnrKndCmdNo;
                            }
                            else
                            {
                                iI_kndcmdno = nomanageItem.F48_OutKndCmdNo;

                            }

                            iI_kndcmdno = iI_kndcmdno > 999 ? 1 : iI_kndcmdno + 1;
                            if (kneadingLine == Constants.KndLine.Megabit.ToString("D"))
                            {
                                nomanageItem.F48_MegaKndCmdNo = iI_kndcmdno;

                            }
                            else if (kneadingLine == Constants.KndLine.Conventional.ToString("D"))
                            {
                                nomanageItem.F48_GnrKndCmdNo = iI_kndcmdno;
                            }
                            else
                            {
                                nomanageItem.F48_OutKndCmdNo = iI_kndcmdno;
                            }
                            nomanageItem.F48_KndCmdBookNo = bookNo < 0 ? bookNo + 1 : bookNo;
                            _unitOfWork.NoManageRepository.Update(nomanageItem);

                            sequence1 = string.Format("000{0}", iI_kndcmdno);
                            sequence1 = sequence1.Substring(sequence1.Length - 3);
                        }
                        else
                        {
                            var nomanage = new TX48_NoManage();
                            nomanage.F48_SystemId = "00000";
                            nomanage.F48_MegaKndCmdNo = kneadingLine == Constants.KndLine.Megabit.ToString("D") ? Convert.ToInt32(Constants.KndLine.Conventional.ToString("D")) : Convert.ToInt32(Constants.KndLine.Megabit.ToString("D"));
                            nomanage.F48_GnrKndCmdNo = kneadingLine == Constants.KndLine.Megabit.ToString("D") ? Convert.ToInt32(Constants.KndLine.Megabit.ToString("D")) : Convert.ToInt32(Constants.KndLine.Conventional.ToString("D"));
                            nomanage.F48_MtrWhsCmdNo = 0;
                            nomanage.F48_PrePdtWhsCmdNo = 0;
                            nomanage.F48_PrePdtWhsCmdNo = 0;
                            nomanage.F48_KndCmdBookNo = 0;
                            nomanage.F48_AddDate = DateTime.Now;
                            nomanage.F48_UpdateDate = DateTime.Now;
                            nomanage.F48_KneadSheefNo = 0;
                            if (kneadingLine != Constants.KndLine.Megabit.ToString("D") &&
                                kneadingLine != Constants.KndLine.Conventional.ToString("D"))
                            {
                                nomanage.F48_OutKndCmdNo = 1;
                            }
                            else
                            {
                                nomanage.F48_OutKndCmdNo = 0;
                            }

                            nomanage.F48_CnrKndCmdNo = 0;
                            nomanage.F48_AddDate = DateTime.Now;
                            nomanage.F48_UpdateDate = DateTime.Now;
                            _unitOfWork.NoManageRepository.Add(nomanage);
                            _unitOfWork.Commit();

                            sequence1 = "001";
                            bookNo = 1;
                        }
                        #endregion
                        #region Update TM03_PreProduct

                        //Get PreProduct Record where [f03_preproductcode] is equal to Pre-product Code
                        var preProduct = _unitOfWork.PreProductRepository.GetById(pdtPlnItem.F39_PreProductCode);

                        //Update [f03_lotnoend] by plus itself with [f39_prepdtlotamt] of “tx39_pdtpln” table
                        if (preProduct != null)
                        {
                            //Set [Sequence 2] = 2 last characters of {"000"+ ([f03_lotnoend]+1)}
                            sequence2 = sequence2 + (preProduct.F03_LotNoEnd + 1).ToString();
                            sequence2 = sequence2.Substring(sequence2.Length - 2);
                            //Update
                            preProduct.F03_LotNoEnd += pdtPlnItem.F39_PrePdtLotAmt;

                            _unitOfWork.PreProductRepository.Update(preProduct);


                        }

                        #endregion
                        //var preProductCode = pdtPlnItem.F39_PreProductCode;
                        //var preProductItem = _unitOfWork.PreProductRepository.GetById(preProductCode);
                        //if (preProductItem != null)
                        //{
                        //    sequence2 = string.Format("{0}{1}", sequence2, (preProductItem.F03_LotNoEnd + 1));
                        //    preProductItem.F03_LotNoEnd = pdtPlnItem.F39_PrePdtLotAmt;
                        //    _unitOfWork.PreProductRepository.Update(preProductItem);
                        //}
                        //sequence2 = sequence2.Substring(sequence1.Length - 2);

                        //Set [Konren sequence] in form of: “A”/or “B” + [Month] + [Sequence 1], where:“A”/or “B”: If Kneading line is “Conventional” set to “A”; or if Kneading line is “Megabit” set to “B”.

                        konrenSequence = kneadingLine == Constants.KndLine.Conventional.ToString("D") ? "A" : "B";
                        konrenSequence = string.Format("{0}{1}{2}", konrenSequence, month, sequence1);

                        //Set [Pre-product Sequence] in form of: “A”/or “B” + [Year] + [Month] + [Day] + [Sequence 2], where:“A”/or “B”: If Kneading line is “Conventional” set to “A”; or if Kneading line is “Megabit” set to “B”.

                        preProductSequence = kneadingLine == Constants.KndLine.Conventional.ToString("D") ? "A" : "B";
                        preProductSequence = string.Format("{0}{1}{2}{3}{4}", preProductSequence, year, month, day, sequence2);

                        #region Update “tx39_pdtpln”

                        pdtPlnItem.F39_KndCmdNo = konrenSequence;
                        pdtPlnItem.F39_Status = Constants.F39_Status.Commanded.ToString("D");
                        pdtPlnItem.F39_StartLotNo = preProductSequence;

                        _unitOfWork.PdtPlnRepository.Update(pdtPlnItem);

                        #endregion

                        #region Insert TX42_KndCmd

                        isCount += 1;
                        if (isCount == lstResult.Count())
                        {
                            //Set [f42_lotseqno] to the order number within the loop
                            var orderNumber = 0;
                            var lotSeqNo = 0;

                            for (int i = 0; i < s.Length; i++)
                            {
                                orderNumber++;
                                var date = ConvertHelper.ConvertToDateTimeFull(s[i].Split(',')[0]);
                                var preProductCode = s[i].Split(',')[1];
                                if (date == pdtPlnItem.F39_KndEptBgnDate && preProductCode.Trim() == pdtPlnItem.F39_PreProductCode.Trim())
                                {
                                    lotSeqNo = orderNumber;
                                    break;
                                }
                            }

                            //•	Suppose [li_maxcmdseq] is the maximum command sequence. Set [li_maxcmdseq] to:Set to { mMaximum of [f42_commandseqno]} + 1, where [f42_kndcmdbookno] = [Book No] above,
                            //Set to  or set to 1 for other cases (error or null case).
                            var kndcmd = _unitOfWork.KneadingCommandRepository.GetMany(i => i.F42_KndCmdBookNo.Equals(bookNo)).OrderByDescending(i => i.F42_CommandSeqNo);
                            var liMaxcmdseq = 1;
                            if (kndcmd.Any())
                            {
                                liMaxcmdseq = kndcmd.FirstOrDefault().F42_CommandSeqNo + 1;
                            }

                            var kndCmdItem = new TX42_KndCmd();
                            kndCmdItem.F42_KndCmdNo = konrenSequence;
                            kndCmdItem.F42_PrePdtLotNo = preProductSequence;
                            kndCmdItem.F42_PreProductCode = pdtPlnItem.F39_PreProductCode;
                            kndCmdItem.F42_KndEptBgnDate = pdtPlnItem.F39_KndEptBgnDate;
                            kndCmdItem.F42_OutSideClass = Constants.F42_OutSideClass.PreProduct.ToString("D");
                            kndCmdItem.F42_Status = Constants.F42_Status.TX42_Sts_NotKnd;
                            kndCmdItem.F42_ThrowAmount = 0;
                            if (preProduct != null)
                            {
                                kndCmdItem.F42_NeedAmount = preProduct.F03_AllMtrAmtPerBth;
                            }
                            else
                                kndCmdItem.F42_NeedAmount = 0;
                            kndCmdItem.F42_StgCtnAmt = 0;
                            kndCmdItem.F42_BatchEndAmount = 0;
                            kndCmdItem.F42_KndCmdBookNo = bookNo;
                            kndCmdItem.F42_LotSeqNo = lotSeqNo;
                            kndCmdItem.F42_CommandSeqNo = liMaxcmdseq;
                            kndCmdItem.F42_MtrRtrFlg = Constants.F42_MtrRtrFlg.NoTretrieval.ToString("D");
                            kndCmdItem.F42_AddDate = DateTime.Now;
                            kndCmdItem.F42_UpdateDate = DateTime.Now;
                            kndCmdItem.F42_UpdateCount = 0;
                            _unitOfWork.KneadingCommandRepository.Add(kndCmdItem);
                        }
                        #endregion

                        //update  preProductSequence
                        //preProductSequence = Int32.Parse(preProductSequence) > 100
                        //    ? "01"
                        //    : (Int32.Parse(preProductSequence) + 1).ToString();
                    }
                }
            }

            _unitOfWork.Commit();
            return new ResponseResult(true);
        }

        /// <summary>
        ///     This function is for exporting data from kneading command database to list.
        /// </summary>
        /// <returns></returns>
        public async Task<PrintMaterialNameItem> SearchRecordsForPrinting()
        {                        
            // Find all tx31
            var materialShelfStatuss = _unitOfWork.MaterialShelfStatusRepository.GetAll();

            // Find all tx33
            var materialShelfStocks = _unitOfWork.MaterialShelfStockRepository.GetAll();

            // Find all materials.
            var materials = _unitOfWork.MaterialRepository.GetAll();

            // Record which should be printed.
            var printMaterialNameItem = new PrintMaterialNameItem();

            var condition1 = Constants.F31_ShelfStatus.Material.ToString("D");
            var condition2 = Constants.TM01_Material_EntrustedClass.Normal.ToString("D");
            
            // Find Material Name item.
            var materialNameItems = from material in materials
                                       join materialShelfStock in materialShelfStocks on material.F01_MaterialCode equals materialShelfStock.F33_MaterialCode
                                       join materialShelfStatus in materialShelfStatuss on materialShelfStock.F33_PalletNo equals materialShelfStatus.F31_PalletNo                                       
                                       where materialShelfStatus.F31_ShelfStatus.Trim().Equals(condition1)
                                        && material.F01_EntrustedClass.Equals(condition2)                                                                                                                            
                                       orderby new
                                       {
                                           materialShelfStock.F33_MaterialCode ,
                                           materialShelfStock.F33_MaterialLotNo                                       
                                       }
                                       select new FindPrintMaterialNameItem
                                       {
                                           MaterialCode = materialShelfStock.F33_MaterialCode,
                                           MaterialName = material.F01_MaterialDsp,
                                           LotNo = materialShelfStock.F33_MaterialLotNo,
                                           PalletNo = materialShelfStock.F33_PalletNo,
                                           RBL = materialShelfStatus.F31_ShelfRow + "-" + materialShelfStatus.F31_ShelfBay + "-" + materialShelfStatus.F31_ShelfLevel,
                                           Quantity = materialShelfStock.F33_Amount,
                                           //Total = materialShelfStocks.Sum(i => i.F33_Amount)
                                       };

            var a = materialNameItems.GroupBy(item => item.MaterialCode)
                .Select(group => group.Sum(item => item.Quantity));

            var groupItems =
                materialNameItems.GroupBy(x => new { x.MaterialCode})
                    .Select(y => new GroupValueItem()
                    {
                        MaterialCode = y.Key.MaterialCode,
                        Total = y.Sum(c => c.Quantity)
                    });

            foreach (var groupitem in groupItems.ToList())
            {
                var printMaterialNameGroupItem = new PrintMaterialNameGroupItem()
                {
                    MaterialCode = groupitem.MaterialCode,                    
                    LotTotal = groupitem.Total,
                    LotTotalString = String.Format("{0:#,##0.00}", groupitem.Total)
                };
                foreach (var item in materialNameItems.ToList())
                {
                    if (item.MaterialCode == groupitem.MaterialCode)
                    {
                        printMaterialNameGroupItem.FindPrintMaterialNameItem.Add(item);
                    }
                }
                printMaterialNameItem.PrintMaterialNameGroupItem.Add(printMaterialNameGroupItem);
            }

            //var totalSecondItems = (await secondResult.SumAsync(x => (double?)x.Key.Amount)) ?? 0;

            //•	o	Show “PAGE: “ + current page with format as ###            
            //printExternalPreProductNameItem.Page = "1";

            //// Get Total quantity
            //printPreProductNameItem.PreproductTotal = printPreProductNameItem.PrintPreProductNameGroupItem.Sum(c => c.LotTotal);
            //printPreProductNameItem.PreproductTotalString = String.Format("{0:#,##0.00}", printPreProductNameItem.PreproductTotal);
            

            await _unitOfWork.CommitAsync();

            //printMaterialNameItem.FindPrintMaterialNameItem = await materialNameItems.ToListAsync();

            return printMaterialNameItem;
        }

        public async Task<PrintMaterialNameItem> SearchRecordsForPrintingBailment()
        {
            // Find all tx31
            var materialShelfStatuss = _unitOfWork.MaterialShelfStatusRepository.GetAll();

            // Find all tx33
            var materialShelfStocks = _unitOfWork.MaterialShelfStockRepository.GetAll();

            // Find all materials.
            var materials = _unitOfWork.MaterialRepository.GetAll();

            // Record which should be printed.
            var printMaterialNameItem = new PrintMaterialNameItem();

            var condition1 = Constants.F31_ShelfStatus.Material.ToString("D");
            var condition2 = Constants.TM01_Material_EntrustedClass.Bailment.ToString("D");
            
            // Find Material Name item.
            var materialNameItems = from material in materials
                                    join materialShelfStock in materialShelfStocks on material.F01_MaterialCode equals materialShelfStock.F33_MaterialCode
                                    join materialShelfStatus in materialShelfStatuss on materialShelfStock.F33_PalletNo equals materialShelfStatus.F31_PalletNo
                                    where materialShelfStatus.F31_ShelfStatus.Trim().Equals(condition1)
                                     && material.F01_EntrustedClass.Equals(condition2)
                                    orderby new
                                    {
                                        materialShelfStock.F33_MaterialCode,
                                        materialShelfStock.F33_MaterialLotNo
                                    }
                                    select new FindPrintMaterialNameItem
                                    {
                                        MaterialCode = materialShelfStock.F33_MaterialCode,
                                        MaterialName = material.F01_MaterialDsp,
                                        PalletNo = materialShelfStock.F33_PalletNo,
                                        LotNo = materialShelfStock.F33_MaterialLotNo,
                                        RBL = materialShelfStatus.F31_ShelfRow + "-" + materialShelfStatus.F31_ShelfBay + "-" + materialShelfStatus.F31_ShelfLevel,
                                        Quantity = materialShelfStock.F33_Amount,
                                        //Total = materialShelfStocks.Sum(i => i.F33_Amount)
                                    };            

            var groupItems =
                materialNameItems.GroupBy(x => new { x.MaterialCode })
                    .Select(y => new GroupValueItem()
                    {
                        MaterialCode = y.Key.MaterialCode,
                        Total = y.Sum(c => c.Quantity)
                    });

            foreach (var groupitem in groupItems.ToList())
            {
                var printMaterialNameGroupItem = new PrintMaterialNameGroupItem()
                {
                    MaterialCode = groupitem.MaterialCode,
                    LotTotal = groupitem.Total,
                    LotTotalString = String.Format("{0:#,##0.00}", groupitem.Total)
                };
                foreach (var item in materialNameItems.ToList())
                {
                    if (item.MaterialCode == groupitem.MaterialCode)
                    {
                        printMaterialNameGroupItem.FindPrintMaterialNameItem.Add(item);
                    }
                }
                printMaterialNameItem.PrintMaterialNameGroupItem.Add(printMaterialNameGroupItem);
            }


            await _unitOfWork.CommitAsync();

            //printMaterialNameItem.FindPrintMaterialNameItem = await materialNameItems.ToListAsync();

            return printMaterialNameItem;
        }        

        //public async Task<PrintMaterialNameItem> SearchRecordsForPrintingAll()
        //{
        //    // Find all tx31
        //    var materialShelfStatuss = _unitOfWork.MaterialShelfStatusRepository.GetAll();

        //    // Find all tx33
        //    var materialShelfStocks = _unitOfWork.MaterialShelfStockRepository.GetAll();

        //    // Find all materials.
        //    var materials = _unitOfWork.MaterialRepository.GetAll();

        //    // Record which should be printed.
        //    var printMaterialNameItem = new PrintMaterialNameItem();

        //    var condition1 = Constants.F31_ShelfStatus.Material.ToString("D");
            

        //    // Find Material Name item.
        //    var materialNameItems = from material in materials
        //                            join materialShelfStock in materialShelfStocks on material.F01_MaterialCode equals materialShelfStock.F33_MaterialCode
        //                            join materialShelfStatus in materialShelfStatuss on materialShelfStock.F33_PalletNo equals materialShelfStatus.F31_PalletNo
        //                            where materialShelfStatus.F31_ShelfStatus.Trim().Equals(condition1)
        //                             && (material.F01_EntrustedClass.Equals("0") || material.F01_EntrustedClass.Equals("1"))
        //                            orderby new
        //                            {
        //                                materialShelfStock.F33_MaterialCode,
        //                                materialShelfStock.F33_MaterialLotNo
        //                            }
        //                            select new FindPrintMaterialNameItem
        //                            {
        //                                MaterialCode = materialShelfStock.F33_MaterialCode,
        //                                MaterialName = material.F01_MaterialDsp,
        //                                LotNo = materialShelfStock.F33_PalletNo,
        //                                PalletNo = materialShelfStock.F33_MaterialLotNo,
        //                                RBL = materialShelfStatus.F31_ShelfRow + "-" + materialShelfStatus.F31_ShelfBay + "-" + materialShelfStatus.F31_ShelfLevel,
        //                                Quantity = materialShelfStock.F33_Amount,
        //                                //Total = materialShelfStocks.Sum(i => i.F33_Amount)
        //                            };

        //    //•	o	Show “PAGE: “ + current page with format as ###            
        //    //printMaterialNameItem.Page = "1";

        //    // Get Total quantity
        //    if (materialNameItems.Any())
        //    {
        //        printMaterialNameItem.Total = materialNameItems.Sum(i => i.Quantity);
        //    }    
        //    // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
        //    printMaterialNameItem.Datetime = DateTime.Today.ToString("g");

        //    /*
        //     * o	Show {Company Name} = value from profile:
        //     * ProfileString("toshiba.ini","server","companyname","")
        //    */
        //    //printMaterialNameItem.CompanyName = "Toshiba";


        //    await _unitOfWork.CommitAsync();

        //    printMaterialNameItem.FindPrintMaterialNameItem = await materialNameItems.ToListAsync();

        //    return printMaterialNameItem;
        //}        

        public MaterialItem GetById(string materialCode)
        {
            var entity = _unitOfWork.MaterialRepository.GetAll().FirstOrDefault(i => i.F01_MaterialCode.Trim().Equals(materialCode.Trim()));
            var result = Mapper.Map<MaterialItem>(entity);                        
            return result;
        }

        public class GroupValueItem
        {            
            public string MaterialCode { get; set; }            
            public double Total { get; set; }

        }
    }
}
