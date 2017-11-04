using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Math;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.KneadingCommand;
using KCSG.Domain.Models.KneadingCommand;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.Data.Repositories;
using KCSG.Domain.Interfaces.Services;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.KneadingCommand
{
    public class InputOfKneadingCommandDomain : BaseDomain, IInputOfKneadingCommandDomain
    {
        #region Constructor

        public InputOfKneadingCommandDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion

        public ResponseResult<GridResponse<PdtPlnItem>> SearchCriteria(DateTime dateCurrent, DateTime date, Enum line,
            GridSettings gridSettings)
        {

            string kndLine;
            var result = _unitOfWork.PdtPlnRepository.GetAll();

            kndLine = Constants.KndLine.Megabit.Equals(line)
                ? Constants.KndLine.Megabit.ToString("D")
                : Constants.KndLine.Conventional.ToString("D");
            var notCommandedStatus = Constants.F39_Status.NotCommanded.ToString("D");
            result =
                result.Where(
                    i =>
                        i.F39_KndEptBgnDate <= date && i.F39_KndEptBgnDate >= dateCurrent && i.F39_KneadingLine.Trim().Equals(kndLine) &&
                        i.F39_Status.Trim().Equals(notCommandedStatus));
            
            var lstPreProductCode = result.Select(i => i.F39_PreProductCode);
            var lstPreProduct =
                _unitOfWork.PreProductRepository.GetAll().Where(i => lstPreProductCode.Contains(i.F03_PreProductCode));
            var lstResult = new List<PdtPlnItem>();

            foreach (var prtPln in result)
            {
                var prdPlnItem = new PdtPlnItem();
                var lstPreProductItem =
                    lstPreProduct.FirstOrDefault(i => i.F03_PreProductCode == prtPln.F39_PreProductCode);
                prdPlnItem.F39_KndEptBgnDate = prtPln.F39_KndEptBgnDate;
                prdPlnItem.F39_PreProductCode = prtPln.F39_PreProductCode;
                prdPlnItem.F39_PrePdtLotAmt = prtPln.F39_PrePdtLotAmt;
                prdPlnItem.F39_Status = prtPln.F39_Status;
                prdPlnItem.F39_AddDate = prtPln.F39_AddDate;

                if (lstPreProductItem != null)
                {
                    prdPlnItem.Quantity = lstPreProductItem.F03_BatchLot;
                    prdPlnItem.F39_PreProductName = lstPreProductItem.F03_PreProductName;
                    prdPlnItem.YieldDrate = lstPreProductItem.F03_YieldRate;
                    prdPlnItem.MaterialAmount = lstPreProductItem.F03_AllMtrAmtPerBth;
                }
                lstResult.Add(prdPlnItem);
            }

            var itemCount = lstResult.Count();
            var pdtPlnItems = lstResult.AsQueryable();
            OrderByAndPaging(ref pdtPlnItems, gridSettings);
            var resultModel = new GridResponse<PdtPlnItem>(pdtPlnItems, itemCount);
            return new ResponseResult<GridResponse<PdtPlnItem>>(resultModel, true);
        }


        public ResponseResult<GridResponse<PdtPlnItem>> SearchCriteriaSelected(string selectedValue,
            GridSettings gridSettings)
        {
            var result = new List<TX39_PdtPln>();
            var lstPdtPln = _unitOfWork.PdtPlnRepository.GetAll();
            string[] s = selectedValue.TrimEnd('#').Split('#');

            for (int i = 0; i < s.Length; i++)
            {
                var date = ConvertHelper.ConvertToDateTimeFull(s[i].Split(',')[0]);
                var preProductCode = s[i].Split(',')[1];
                result.Add(lstPdtPln.FirstOrDefault(c => c.F39_KndEptBgnDate == date && c.F39_PreProductCode.Trim() == preProductCode.Trim()));

            }


            var lstResult = new List<PdtPlnItem>();

            foreach (var prtPln in result.ToList())
            {
                var prdPlnItem = new PdtPlnItem();
                var preProductItem = _unitOfWork.PreProductRepository.GetMany(i => i.F03_PreProductCode == prtPln.F39_PreProductCode).FirstOrDefault();
                var kndCmdItem = _unitOfWork.KneadingCommandRepository.GetMany(i => i.F42_PreProductCode == prtPln.F39_PreProductCode && i.F42_KndEptBgnDate == prtPln.F39_KndEptBgnDate).FirstOrDefault();
                prdPlnItem.F39_KndEptBgnDate = prtPln.F39_KndEptBgnDate;
                prdPlnItem.F39_PreProductCode = prtPln.F39_PreProductCode;
                prdPlnItem.F39_PrePdtLotAmt = prtPln.F39_PrePdtLotAmt;
                prdPlnItem.F39_Status = prtPln.F39_Status;
                prdPlnItem.F39_AddDate = prtPln.F39_AddDate;

                if (preProductItem != null)
                {
                    prdPlnItem.Quantity = preProductItem.F03_BatchLot;
                    prdPlnItem.F39_PreProductName = preProductItem.F03_PreProductName;
                    prdPlnItem.YieldDrate = preProductItem.F03_YieldRate;
                    prdPlnItem.MaterialAmount = preProductItem.F03_AllMtrAmtPerBth;
                }
                if (kndCmdItem != null)
                {
                    prdPlnItem.CmdNo = kndCmdItem.F42_KndCmdNo;
                    prdPlnItem.LotNo = kndCmdItem.F42_PrePdtLotNo;
                    prdPlnItem.CommandSequenceNo = kndCmdItem.F42_CommandSeqNo;
                }

                lstResult.Add(prdPlnItem);
            }

            var itemCount = lstResult.Count();


            var queryable = lstResult.AsQueryable();
            //OrderByAndPaging(ref queryable, gridSettings);


            queryable = queryable.Skip((gridSettings.PageIndex - 1) * gridSettings.PageSize).Take(gridSettings.PageSize);
            var resultModel = new GridResponse<PdtPlnItem>(queryable, itemCount);
            return new ResponseResult<GridResponse<PdtPlnItem>>(resultModel, true);
        }

        public bool DeleteKneadingCommand(string deleteDate, string deleteCode)
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


        /// <summary>
        /// Initiate a list of kneading commands base on the specific conditions.
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <param name="within"></param>
        /// <param name="lotQuantity"></param>
        /// <returns></returns>
        public ResponseResult CreateOrUpdate(string selectedValue, int within, int lotQuantity)
        {
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
            //var lstResult = result.OrderBy(i => i.F39_PrePdtLotAmt);

            //var result = _unitOfWork.PdtPlnRepository.GetMany(i => lstSelectedCode.Contains(i.F39_PreProductCode) && lstSelectedDate.Contains(i.F39_KndEptBgnDate)).OrderBy(i => i.F39_PrePdtLotAmt);

            var nomanageItem = _unitOfWork.NoManageRepository.GetMany(i => i.F48_SystemId == "00000").FirstOrDefault();
            
            if (result.Any())
            {
                foreach (var pdtPlnItem in result)
                {
                    if (pdtPlnItem.F39_Status.Trim() == Constants.F39_Status.NotCommanded.ToString("D"))
                    {

                        DateTime productDate = pdtPlnItem.F39_KndEptBgnDate;
                        string day;
                        string month;
                        string year;
                        string szBaseLotNo = "";

                        if (productDate == DateTime.MinValue)
                        {
                            var today = DateTime.Now.ToString("dd/MM/yy");

                            day = today.Split('/')[0];
                            month = today.Split('/')[1];
                            year = today.Split('/')[2];
                            
                            var szYear = today.Split('/')[2];
                            szYear = szYear.Substring(szYear.Length - 1, 1);

                            var szMonth = month.PadLeft(2, '0').Substring(1, 1);
                            szBaseLotNo = $"{szYear}{szMonth}{day}";
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

                            var szYear = date.Split('/')[2];
                            szYear = szYear.Substring(szYear.Length - 1, 1);
                            
                            if (month != "X" && month != "Y" && month != "Z")
                                month = date.Split('/')[1].PadLeft(2, '0').Substring(1, 1);

                            szBaseLotNo = $"{szYear}{month}{day}";
                        }

                        var bookNo = 0;
                        var konrenSequence = "";
                        var sequence1 = "";
                        //var sequence2 = "000";
                        var iLotSequence = 0;
                        //var preProductSequence = "";
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
                            iLotSequence = preProduct.F03_LotNoEnd + 1;
                            //sequence2 = sequence2 + (preProduct.F03_LotNoEnd + 1).ToString();
                            //sequence2 = sequence2.Substring(sequence2.Length - 2);
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

                        // Calculate the base sequence which should be applied to kneading command.
                        var basePreProductSequence = kneadingLine == Constants.KndLine.Conventional.ToString("D") ? "A" : "B";
                        basePreProductSequence += szBaseLotNo;
                        
                        #region Insert TX42_KndCmd

                        ////Set [f42_lotseqno] to the order number within the loop
                        //count += 1;

                        //•	Suppose [li_maxcmdseq] is the maximum command sequence. Set [li_maxcmdseq] to:Set to { mMaximum of [f42_commandseqno]} + 1, where [f42_kndcmdbookno] = [Book No] above,
                        //Set to  or set to 1 for other cases (error or null case).
                        var kndcmd = _unitOfWork.KneadingCommandRepository.GetMany(i => i.F42_KndCmdBookNo.Equals(bookNo)).OrderByDescending(i => i.F42_CommandSeqNo).FirstOrDefault();
                        var liMaxcmdseq = 1;
                        if (kndcmd != null)
                            liMaxcmdseq = kndcmd.F42_CommandSeqNo + 1;

                        var szLotSequence = "";
                        for (var i = 0; i < pdtPlnItem.F39_PrePdtLotAmt; i++ , liMaxcmdseq++)
                        {
                            iLotSequence += 1;
                            if (iLotSequence > 99)
                                iLotSequence = 1;
                            szLotSequence = $"{basePreProductSequence}{iLotSequence.ToString().PadLeft(2, '0')}";

                            var kndCmdItem = new TX42_KndCmd();
                            kndCmdItem.F42_KndCmdNo = konrenSequence;
                            kndCmdItem.F42_PrePdtLotNo = szLotSequence;
                            kndCmdItem.F42_PreProductCode = pdtPlnItem.F39_PreProductCode;
                            kndCmdItem.F42_KndEptBgnDate = pdtPlnItem.F39_KndEptBgnDate;
                            kndCmdItem.F42_OutSideClass = Constants.F42_OutSideClass.PreProduct.ToString("D");
                            kndCmdItem.F42_Status = Constants.F42_Status.TX42_Sts_NotKnd;
                            kndCmdItem.F42_ThrowAmount = 0;
                            if (preProduct != null)
                                kndCmdItem.F42_NeedAmount = preProduct.F03_AllMtrAmtPerBth;
                            else
                                kndCmdItem.F42_NeedAmount = 0;
                            kndCmdItem.F42_StgCtnAmt = 1;
                            kndCmdItem.F42_BatchEndAmount = 0;
                            kndCmdItem.F42_KndCmdBookNo = bookNo;
                            kndCmdItem.F42_LotSeqNo = iLotSequence;
                            kndCmdItem.F42_CommandSeqNo = liMaxcmdseq;
                            kndCmdItem.F42_MtrRtrFlg = Constants.F42_MtrRtrFlg.NoTretrieval.ToString("D");
                            kndCmdItem.F42_AddDate = DateTime.Now;
                            kndCmdItem.F42_UpdateDate = DateTime.Now;
                            kndCmdItem.F42_UpdateCount = 0;
                            
                            _unitOfWork.KneadingCommandRepository.Add(kndCmdItem);

                        }

                        #endregion

                        #region Update “tx39_pdtpln”

                        pdtPlnItem.F39_KndCmdNo = konrenSequence;
                        pdtPlnItem.F39_Status = Constants.F39_Status.Commanded.ToString("D");
                        pdtPlnItem.F39_StartLotNo = szLotSequence;

                        _unitOfWork.PdtPlnRepository.Update(pdtPlnItem);

                        #endregion
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
        public async Task<PrintKneadingCommandItem> SearchRecordsForPrinting(string preProductCode, string commandNo)
        {
            // Query is so complicated, please refer BR 7 - SRS 1.1
            var prePdtMkps = _unitOfWork.PrePdtMkpRepository.GetAll();

            // Find all pre-products
            var preProducts = _unitOfWork.PreProductRepository.GetAll();

            // Find all materials.
            var materials = _unitOfWork.MaterialRepository.GetAll();

            /*
             * 	“Command Date:” (In format of DD/MM/YYYY): Retrieved from [f42_kndeptbgndate] in TX42_KNDCMD, where:
             * -	[f42_kndcmdno] = [CmdNo] of selected row.
             * -	[f42_preproductcode] = [Pre-product Code] of selected row.
             * -	Ascending order by [f42_lotseqno].
             */
            var kneadingCommands = _unitOfWork.KneadingCommandRepository.GetAll();
            if (!string.IsNullOrEmpty(commandNo) && !string.IsNullOrEmpty(preProductCode))
            {
                kneadingCommands = kneadingCommands.Where(x => x.F42_KndCmdNo.Trim().Equals(commandNo.Trim()) && x.F42_PreProductCode.Trim().Equals(preProductCode.Trim()));
            }
            kneadingCommands = kneadingCommands.OrderBy(x => x.F42_LotSeqNo);

            // Find the first result.
            var kneadingCommand = await kneadingCommands.FirstOrDefaultAsync();

            // Record which should be printed.
            var printKneadingCommandItem = new PrintKneadingCommandItem();


            // Find kneading command item.
            var kneadingCommandItems = from prePdtMkp in prePdtMkps
                                       from preProduct in preProducts
                                       from material in materials
                                       where preProduct.F03_PreProductCode.Equals(prePdtMkp.F02_PreProductCode)
                                             && material.F01_MaterialCode.Equals(prePdtMkp.F02_MaterialCode)
                                             && prePdtMkp.F02_PreProductCode.Equals(preProductCode)
                                       orderby new
                                       {
                                           prePdtMkp.F02_ThrawSeqNo,
                                           prePdtMkp.F02_PotSeqNo,
                                           prePdtMkp.F02_MsrSeqNo
                                       }
                                       select new FindPrintKneadingCommandItem
                                       {

                                           ChargedOrder = prePdtMkp.F02_ThrawSeqNo,
                                           PotSeq = prePdtMkp.F02_PotSeqNo,
                                           MaterialName = prePdtMkp.F02_MaterialCode,
                                           LoadPosition = prePdtMkp.F02_LoadPosition,
                                           ThirdFloorQuantity = prePdtMkp.F02_3FLayinAmount,
                                           FourthFloorQuantity = prePdtMkp.F02_4FLayinAmount,
                                           State = material.F01_State,
                                           Colour = material.F01_Color,
                                           CrushingOne = prePdtMkp.F02_MilingFlag1,
                                           CrushingTwo = prePdtMkp.F02_MilingFlag2,
                                           MixedMode = preProduct.F03_MixMode,
                                           BatchLot = preProduct.F03_BatchLot,
                                           LayinPriority = prePdtMkp.F02_LayinPriority,
                                           ThirdLayinAmount = prePdtMkp.F02_3FLayinAmount,
                                           FourthLayinAmount = prePdtMkp.F02_4FLayinAmount,
                                           PreProductName = preProduct.F03_PreProductName,
                                           LotNo = !string.IsNullOrEmpty(commandNo) ? kneadingCommand.F42_PrePdtLotNo : "",
                                           MixDate1 = preProduct.F03_MixDate1,
                                           MixDate2 = preProduct.F03_MixDate2,
                                           MixDate3 = preProduct.F03_MixDate3

                                       };

            //•	Show the following information on the left side:
            if (!string.IsNullOrEmpty(commandNo))
            {
                if (kneadingCommand != null)
                {
                    printKneadingCommandItem.CommandDate = kneadingCommand.F42_KndEptBgnDate.ToString("dd/MM/yyyy");
                    printKneadingCommandItem.LotNo = kneadingCommand.F42_PrePdtLotNo;
                }

            }

            //	“Command No:”: Retrieved from [CmdNo] of selected row.
            printKneadingCommandItem.CommandNo = commandNo;

            //if (kneadingCommand != null)
            //    printKneadingCommandItem.CommandDate = kneadingCommand.F42_KndEptBgnDate.ToString("dd/MM/yyyy");

            //	“PreProduct Code:”: Retrieved from [Pre-product Code] of selected row
            printKneadingCommandItem.PreProductCode = preProductCode;


            //	“PreProduct Name:”: Retrieved from [f03_preproductname] in TM03_PREPRODUCT with (**) condition,	
            var kneadingCommandItem = await kneadingCommandItems.FirstOrDefaultAsync();
            if (kneadingCommandItem != null)
                printKneadingCommandItem.PreProductName = kneadingCommandItem.PreProductName;

            /*
             * 	“Lot No:”: Retrieved from [f42_prepdtlotno] in TX42_KNDCMD, where:
             * -	[f42_kndcmdno] = [CmdNo] of selected row.
             * -	[f42_preproductcode] = [Pre-product Code] of selected row.
             * -	Ascending order by [f42_lotseqno].
             */
            //if (kneadingCommand != null)
            //    printKneadingCommandItem.LotNo = kneadingCommand.F42_PrePdtLotNo;

            //	“Issued: ”: Set as today (DD/MM/YYYY),
            printKneadingCommandItem.Issued = DateTime.Now.ToString("dd/MM/yyyy");

            /*
             * 	“Issued No: ”: System will get Issued No by doing as follow:
             * -	Suppose Sequence No is selected from [f48_pdtwhscmdno] in TX48_NOMANAGE, where [f48_systemid] = “00000”.
             */
            var noManages = _unitOfWork.NoManageRepository.GetAll();
            var noManage = await noManages.Where(x => x.F48_SystemId.Equals("00000")).FirstOrDefaultAsync();

            //-	If there is any record found:
            if (noManage != null)
            {
                // TODO: move 9999 to Constants
                //	Increment Sequence No by 1 (if it > 9999, then reset to 1),
                noManage.F48_PdtWhsCmdNo++;
                if (noManage.F48_PdtWhsCmdNo > 9999)
                    noManage.F48_PdtWhsCmdNo = 1;

                //	Suppose Issued No is the temporary variable which set equal to 4 last characters of {“0000” + Sequence No}.
                printKneadingCommandItem.IssuedNo = "0000" + noManage.F48_PdtWhsCmdNo;
            }
            else
            {
                /*
                 * Set [f48_systemid] =”00000”,
                 * Set [f48_megakndcmdno] = 0,
                 * Set [f48_gnrkndcmdno] = 0,
                 * Set [f48_mtrwhscmdno] = 0,
                 * Set [f48_prepdtwhscmdno] = 0,
                 * Set [f48_pdtwhscmdno] = 1,
                 * Set [f48_kndcmdbookno] = 0,
                 * Set [f48_adddate] and [f48_updatedate] = Current date time,
                 * Set [f48_kneadsheefno] = 0,
                 * Set [f48_outkndcmdno] = 0,
                 * Set [f48_cnrkndcmdno] = 0,
                 */
                noManage = new TX48_NoManage();
                noManage.F48_SystemId = "00000";
                noManage.F48_PdtWhsCmdNo = 1;
                noManage.F48_AddDate = noManage.F48_UpdateDate = DateTime.Now;
                _unitOfWork.NoManageRepository.Add(noManage);

                printKneadingCommandItem.IssuedNo = "0001";
            }

            /*
             * 	“Mixed Mode:”: Retrieved from [f03_mixmode] in TM03_PREPRODUCT with (**) condition.
             */
            if (kneadingCommandItem != null)
                printKneadingCommandItem.MixedMode = kneadingCommandItem.MixedMode;

            /*
             * 	“Batch:”: In format of: "1/" + {Batch Lot}, in which {Batch Lot} is retrieved from [f03_batchlot] in TM03_PREPRODUCT, where [f03_preproductcode] = [Pre-product Code] of selected row.
             */
            if (kneadingCommandItem != null)
                printKneadingCommandItem.Batch = "1/" + kneadingCommandItem.BatchLot;

            await _unitOfWork.CommitAsync();

            printKneadingCommandItem.FindPrintKneadingCommandItems = await kneadingCommandItems.ToListAsync();

            return printKneadingCommandItem;
        }
    }
}
