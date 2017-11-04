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
using KCSG.Domain.Models.Inquiry.ByExternalPre_ProductName;
using KCSG.Domain.Models.Inquiry.ByPreProductName;
using KCSG.Domain.Models.PreProductManagement;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.Inquiry
{
    public class InquiryByPreProductNameDomain : BaseDomain, IInquiryByPreProductNameDomain
    {
        #region Constructor

        public InquiryByPreProductNameDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        public ResponseResult<GridResponse<PreProductShelfStatusItem>> SearchCriteria(string preProductCode,
            GridSettings gridSettings,out double total)
        {           
            //Get all record tx37
            var preProductShelfStatuss = _unitOfWork.PreProductShelfStatusRepository.GetAll();
            //Get all record tx49
            var preProductShelfStocks = _unitOfWork.PreProductShelfStockRepository.GetAll();            

            var shelfStatus = Constants.F49_ShelfStatus.TX49_StkFlg_Stk;
            if (!string.IsNullOrEmpty(preProductCode))
            {
                var result = from preProductShelfStatus in preProductShelfStatuss
                             join preProductShelfStock in preProductShelfStocks on preProductShelfStatus.F37_ContainerCode equals preProductShelfStock.F49_ContainerCode

                             where preProductShelfStock.F49_PreProductCode.Trim().Equals(preProductCode.Trim()) &&
                                   preProductShelfStock.F49_ShelfStatus.Trim().Equals(shelfStatus)                             
                             orderby new
                             {
                                 preProductShelfStock.F49_PreProductLotNo,
                                 preProductShelfStatus.F37_ShelfRow,
                                 preProductShelfStatus.F37_ShelfBay,
                                 preProductShelfStatus.F37_ShelfLevel
                             }
                             select
                             new PreProductShelfStatusItem
                             {
                                 F49_PreProductLotNo = preProductShelfStock.F49_PreProductLotNo,
                                 F49_Amount = preProductShelfStock.F49_Amount,
                                 F49_ContainerCode = preProductShelfStock.F49_ContainerCode,
                                 ShelfNo = preProductShelfStatus.F37_ShelfRow + "-" + preProductShelfStatus.F37_ShelfBay + "-" + preProductShelfStatus.F37_ShelfLevel,
                             };


                var itemCount = result.Count();
                double tempTotal = 0;
                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);
                if (result.Any())
                {
                    tempTotal = result.Sum(i => i.F49_Amount);
                }
                total = tempTotal;
                var resultModel = new GridResponse<PreProductShelfStatusItem>(result, itemCount);
                return new ResponseResult<GridResponse<PreProductShelfStatusItem>>(resultModel, true);
            }
            else
            {
                var result = from preProductShelfStatus in preProductShelfStatuss
                             join preProductShelfStock in preProductShelfStocks on preProductShelfStatus.F37_ContainerCode equals preProductShelfStock.F49_ContainerCode

                             where preProductShelfStock.F49_ShelfStatus.Trim().Equals(shelfStatus)
                             orderby new
                             {
                                 preProductShelfStock.F49_PreProductLotNo,
                                 preProductShelfStatus.F37_ShelfRow,
                                 preProductShelfStatus.F37_ShelfBay,
                                 preProductShelfStatus.F37_ShelfLevel
                             }
                             select
                             new PreProductShelfStatusItem
                             {
                                 F49_PreProductLotNo = preProductShelfStock.F49_PreProductLotNo,
                                 F49_Amount = preProductShelfStock.F49_Amount,
                                 F49_ContainerCode = preProductShelfStock.F49_ContainerCode,
                                 ShelfNo = preProductShelfStatus.F37_ShelfRow + "-" + preProductShelfStatus.F37_ShelfBay + "-" + preProductShelfStatus.F37_ShelfLevel,
                             };


                var itemCount = result.Count();
                double tempTotal = 0;
                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);
                if (result.Any())
                {
                    tempTotal = result.Sum(i => i.F49_Amount);
                }
                total = tempTotal;
                var resultModel = new GridResponse<PreProductShelfStatusItem>(result, itemCount);
                return new ResponseResult<GridResponse<PreProductShelfStatusItem>>(resultModel, true);
            }                        
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
        public async Task<PrintPreProductNameItem> SearchRecordsForPrintingNormal()
        {
            //Get all record tx37
            var preProductShelfStatuss = _unitOfWork.PreProductShelfStatusRepository.GetAll();
            //Get all record tx49
            var preProductShelfStocks = _unitOfWork.PreProductShelfStockRepository.GetAll();

            // Get all tm03.
            var preProducts = _unitOfWork.PreProductRepository.GetAll();

            var condition1 = Constants.F49_ShelfStatus.TX49_StkFlg_Stk;

            // Record which should be printed.
            var printPreProductNameItem = new PrintPreProductNameItem();
            
            
            // Find Pre Product Name item.
            var preProductNameItems = from preProduct in preProducts
                                      join preProductShelfStock in preProductShelfStocks on preProduct.F03_PreProductCode equals preProductShelfStock.F49_PreProductCode
                                      join preProductShelfStatus in preProductShelfStatuss on preProductShelfStock.F49_ContainerCode equals preProductShelfStatus.F37_ContainerCode
                                      where preProductShelfStock.F49_ShelfStatus.Trim().Equals(condition1)                                       
                                    //group materialShelfStock by materialShelfStock.F33_MaterialCode into grp
                                     orderby new
                                     {
                                         preProductShelfStock.F49_PreProductCode,
                                         preProductShelfStock.F49_PreProductLotNo,
                                         preProductShelfStock.F49_ContainerCode
                                     }
                                        
                                       select new FindPrintPreProductNameItem()
                                       {
                                           PreProductCode = preProductShelfStock.F49_PreProductCode,
                                           PreProductName = preProduct.F03_PreProductName,
                                           ContainerCode = preProductShelfStock.F49_ContainerCode,
                                           PreProductLotNo = preProductShelfStock.F49_PreProductLotNo,
                                           ShelfNo = preProductShelfStatus.F37_ShelfRow + "-" + preProductShelfStatus.F37_ShelfBay + "-" + preProductShelfStatus.F37_ShelfLevel,
                                           Amount = preProductShelfStock.F49_Amount,
                                       };

            if (!preProductNameItems.Any())
            {
                return null;
            }
            

            var groups = preProductNameItems.GroupBy(a => new {a.PreProductCode})
                .Select(g1 => new 
                    {
                        PreProductCode = g1.Key.PreProductCode,
                        PreProductTotal = g1.Sum(c => c.Amount),                        
                        Groups = g1
                        .GroupBy(b => new{b.PreProductCode,b.PreProductLotNo})
                        .Select(g2 => new
                            {
                               PreProductCode = g2.Key.PreProductCode,
                               PreProductLotNo = g2.Key.PreProductLotNo,
                               LotTotal = g2.Sum(d =>d.Amount),
                               Group2 = g2.Select(g3 => new FindPrintPreProductNameItem()
                               {
                                   PreProductCode = g3.PreProductCode,
                                   PreProductName = g3.PreProductName,
                                   ContainerCode = g3.ContainerCode,
                                   PreProductLotNo = g3.PreProductLotNo,
                                   ShelfNo = g3.ShelfNo,
                                   Amount = g3.Amount,
                               })
                            })
                    });

            


            foreach (var groupA in groups.ToList())
            {
                var printPreProductNameGroup = new PrintPreProductNameGroup();
                foreach (var groupB in groupA.Groups.ToList())
                {
                    PrintPreProductNameGroupItem printPreProductNameGroupItem = new PrintPreProductNameGroupItem();
                    foreach (var groupC in groupB.Group2.ToList())
                    {                        
                        printPreProductNameGroupItem.FindPrintPreProductNameItem.Add(groupC);
                    }                           
                    printPreProductNameGroupItem.LotTotal = groupB.LotTotal;
                    printPreProductNameGroupItem.LotTotalString = String.Format("{0:#,##0.00}", groupB.LotTotal);
                    printPreProductNameGroup.PrintPreProductNameGroupItem.Add(printPreProductNameGroupItem);                                      
                }
                printPreProductNameGroup.PreproductTotal = groupA.PreProductTotal;
                printPreProductNameGroup.PreproductTotalString = String.Format("{0:#,##0.00}", groupA.PreProductTotal);
                printPreProductNameItem.PrintPreProductNameGroup.Add(printPreProductNameGroup);
            }         
                                                                 
            
            await _unitOfWork.CommitAsync();

            //printPreProductNameItem.FindPrintPreProductNameItem = await preProductNameItems.ToListAsync();

            return printPreProductNameItem;
        }

        public async Task<PrintPreProductNameItem> SearchRecordsForPrintingExternal()
        {
            // Find all tx51
            var productShelfStatuss = _unitOfWork.ProductShelfStatusRepository.GetAll();            

            // Find all tx53
            var outSidePreShelfStocks = _unitOfWork.OutSidePrePdtStkRepository.GetAll();

            // Get all tm03.
            var preProducts = _unitOfWork.PreProductRepository.GetAll();

            // Record which should be printed.
            var printPreProductNameItem = new PrintPreProductNameItem();

            var condition1 = Constants.F51_ShelfStatus.TX51_ShfSts_ExtPrePdt;
            var condition2 = Constants.F51_ShelfType.Normal.ToString("D");
            

            // Find Pre Product Name item.
            var preProductNameItems = from preProduct in preProducts
                                      join outSidePreShelfStock in outSidePreShelfStocks on preProduct.F03_PreProductCode equals outSidePreShelfStock.F53_OutSidePrePdtCode
                                      join productShelfStatus in productShelfStatuss on outSidePreShelfStock.F53_PalletNo equals productShelfStatus.F51_PalletNo
                                      where productShelfStatus.F51_ShelfStatus.Trim().Equals(condition1)
                                     && productShelfStatus.F51_ShelfType.Trim().Equals(condition2)
                                    orderby new
                                    {
                                        outSidePreShelfStock.F53_OutSidePrePdtCode,
                                        outSidePreShelfStock.F53_OutSidePrePdtLotNo
                                    }
                                    select new FindPrintPreProductNameItem
                                    {
                                        OutsidePreProductCode = outSidePreShelfStock.F53_OutSidePrePdtCode,
                                        PreProductName = preProduct.F03_PreProductName,
                                        PalletNo = outSidePreShelfStock.F53_PalletNo,
                                        OutsidePreProductLotNo = outSidePreShelfStock.F53_OutSidePrePdtLotNo,
                                        ShelfNo = productShelfStatus.F51_ShelfRow + "-" + productShelfStatus.F51_ShelfBay + "-" + productShelfStatus.F51_ShelfLevel,
                                        Amount = outSidePreShelfStock.F53_Amount,                                        
                                    };

            

            if (!preProductNameItems.Any())
            {
                return null;
            }

            var groups = preProductNameItems.GroupBy(a => new { a.OutsidePreProductCode })
                .Select(g1 => new
                {
                    OutsidePreProductCode = g1.Key.OutsidePreProductCode,
                    PreProductTotal = g1.Sum(c => c.Amount),
                    Groups = g1
                    .GroupBy(b => new { b.OutsidePreProductCode, b.OutsidePreProductLotNo })
                    .Select(g2 => new
                    {
                        OutsidePreProductCode = g2.Key.OutsidePreProductCode,
                        OutsidePreProductLotNo = g2.Key.OutsidePreProductLotNo,
                        LotTotal = g2.Sum(d => d.Amount),
                        Group2 = g2.Select(g3 => new FindPrintPreProductNameItem()
                        {
                            OutsidePreProductCode = g3.OutsidePreProductCode,
                            PreProductName = g3.PreProductName,                            
                            PalletNo = g3.PalletNo,
                            OutsidePreProductLotNo = g3.OutsidePreProductLotNo,
                            ShelfNo = g3.ShelfNo,
                            Amount = g3.Amount
                        })
                    })
                });
            



            foreach (var groupA in groups.ToList())
            {
                var printPreProductNameGroup = new PrintPreProductNameGroup();
                foreach (var groupB in groupA.Groups.ToList())
                {
                    PrintPreProductNameGroupItem printPreProductNameGroupItem = new PrintPreProductNameGroupItem();
                    foreach (var groupC in groupB.Group2.ToList())
                    {
                        printPreProductNameGroupItem.FindPrintPreProductNameItem.Add(groupC);
                    }
                    printPreProductNameGroupItem.LotTotal = groupB.LotTotal;
                    printPreProductNameGroupItem.LotTotalString = String.Format("{0:#,##0.00}", groupB.LotTotal);
                    printPreProductNameGroup.PrintPreProductNameGroupItem.Add(printPreProductNameGroupItem);
                }
                printPreProductNameGroup.PreproductTotal = groupA.PreProductTotal;
                printPreProductNameGroup.PreproductTotalString = String.Format("{0:#,##0.00}", groupA.PreProductTotal);
                printPreProductNameItem.PrintPreProductNameGroup.Add(printPreProductNameGroup);
            }   
            


            await _unitOfWork.CommitAsync();

            //printPreProductNameItem.FindPrintPreProductNameItem = await preProductNameItems.ToListAsync();

            return printPreProductNameItem;
        }        

        public string GetById(string preProductCode)
        {
            var entity = _unitOfWork.PreProductRepository.GetAll().FirstOrDefault(i => i.F03_PreProductCode.Trim().Equals(preProductCode.Trim()));           
            var result = Mapper.Map<PreProductItem>(entity);            
            if (result == null)
            {
                return "";
            }
            return result.F03_PreProductName;
        }

        public class GroupValueItem
        {
            public string OutsidePreProductCode { get; set; }

            public string OutsidePreProductLotNo { get; set; }

            public string PreProductCode { get; set; }
            public string PreProductLotNo { get; set; }

            public double Total { get; set; }

        }
    }
}
