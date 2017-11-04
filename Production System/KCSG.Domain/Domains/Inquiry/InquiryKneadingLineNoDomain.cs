using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.KneadingCommand;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.Inquiry
{
    public class InquiryKneadingLineNoDomain : BaseDomain, IInquiryKneadingLineNoDomain
    {
        private readonly INotificationService _notificationService;
        #region Constructors

        /// <summary>
        ///     Initialize domain with dependency injections.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public InquiryKneadingLineNoDomain(IUnitOfWork unitOfWork, INotificationService notificationService, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Load kneading commands from database.
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <param name="kneadingCommandline"></param>
        /// <returns></returns>
        public async Task<ResponseResult<GridResponse<object>>> LoadKneadingCommands(GridSettings gridSettings,
            Constants.KndLine kneadingCommandline)
        {
            var kneadingLineMegabit = Constants.KndLine.Megabit.ToString("D");

            // Find all kneading commands (TX42_KndCmd)
            var kneadingCommands = _unitOfWork.PdtPlnRepository.GetAll();

            // Find all Preproducts (TM03_PreProduct)
            var preProducts = _unitOfWork.PreProductRepository.GetAll();

            if (kneadingCommandline == Constants.KndLine.Conventional)
                kneadingCommands = kneadingCommands.Where(x => x.F39_KneadingLine.Trim().Equals("1"));
            else
                kneadingCommands = kneadingCommands.Where(x => x.F39_KneadingLine.Trim().Equals("0"));
            kneadingCommands = kneadingCommands.Where(x => x.F39_Status.Trim() != "0");


            var loadKneadingCommandsResult = from kneadingCommand in kneadingCommands
                                             join preproduct in preProducts on
                                             kneadingCommand.F39_PreProductCode.Trim() equals preproduct.F03_PreProductCode.Trim()
                                             select new PdtPlnItem1
                                             {
                                                 F39_KndCmdNo = kneadingCommand.F39_KndCmdNo,
                                                 F39_PreProductCode = kneadingCommand.F39_PreProductCode,
                                                 F03_PreProductName = preproduct.F03_PreProductName,
                                                 LotNo = kneadingCommand.F39_StartLotNo,
                                                 MaterialAmount = kneadingCommand.F39_PrePdtLotAmt,
                                                 F39_Status = kneadingCommand.F39_Status == "0" ? "Yet" : kneadingCommand.F39_Status== "1"? "Command": kneadingCommand.F39_Status == "2" ? "Produce" : kneadingCommand.F39_Status == "3" ? "End" : kneadingCommand.F39_Status == "4" ? "F End" : kneadingCommand.F39_Status == "5" ? "Stopped" : kneadingCommand.F39_Status == "6" ? "Stopping" : ""
                                             };


            //Ascending order by: [F39_commandseqno], then [F39_lotseqno]
            loadKneadingCommandsResult = loadKneadingCommandsResult.OrderBy(x => new
            {
                x.F39_KndCmdNo
            });

            // Count the total record which matches with the conditions.
            var totalRecords = await loadKneadingCommandsResult.CountAsync();

            var realPageIndex = gridSettings.PageIndex - 1;

            // Do pagination.
            loadKneadingCommandsResult = loadKneadingCommandsResult.Skip(realPageIndex * gridSettings.PageSize)
                .Take(gridSettings.PageSize);

            // Build a grid and respond to client.
            var resultModel = new GridResponse<object>(loadKneadingCommandsResult, totalRecords);
            return new ResponseResult<GridResponse<object>>(resultModel, true);
        }

        /// <summary>
        ///     Interrupt kneading command by using terminal number and list of kneading command results.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="kneadingCommandResults"></param>
        //public async Task InterruptKneadingCommand(string terminalNo,
        //    IEnumerable<FindKneadingCommandItem> kneadingCommandResults)
        //{
        //    if (kneadingCommandResults == null)
        //        return;

        //    // Retrieve all product plans.
        //    var productPlans = _unitOfWork.PdtPlnRepository.GetAll();

        //    var sentFromAtoC = Constants.F55_Status.SentFromAToC.ToString();
        //    var conventionalB = Constants.F39_KneadingLine.ConventionalB.ToString("D");

        //    /*
        //     * 	Suppose CmdNo 2, LotNo 2 and Updated Date 5 are retrieved from [f55_kndcmdno], [f55_prepdtlotno] and [f55_updatedate] in “tx55_kndcmdmsrsnd” table respectively, where:
        //     * 	[f55_terminalno] is Terminal No above.
        //     * 	[f55_pictureno] is equal to “TCPS041F”
        //     * 	[f55_status] is equal to “Sent from A to C” (or “0”)
        //     */
        //    terminalNo = RefineTerminalNo(terminalNo);
        //    var kneadingCommandMsrSnds = _unitOfWork.KndCmdMsrSndRepository.GetAll();
        //    kneadingCommandMsrSnds = kneadingCommandMsrSnds.Where(x => x.F55_KndCmdNo.Trim().Equals(terminalNo));
        //    kneadingCommandMsrSnds =
        //        kneadingCommandMsrSnds.Where(x => x.F55_PrePdtLotNo.Trim().Equals(Constants.PictureNo.TCPS041F));
        //    kneadingCommandMsrSnds = kneadingCommandMsrSnds.Where(x => x.F55_Status.Trim().Equals(sentFromAtoC));


        //    //If there is returned data,
        //    if (kneadingCommandMsrSnds.Any())
        //        foreach (var kneadingCommandMsrSnd in kneadingCommandMsrSnds)
        //            _unitOfWork.KndCmdMsrSndRepository.Delete(
        //                x =>
        //                    x.F55_KndCmdNo.Trim().Equals(kneadingCommandMsrSnd.F55_KndCmdNo) &&
        //                    (x.F55_UpdateDate == kneadingCommandMsrSnd.F55_UpdateDate) &&
        //                    x.F55_Status.Trim().Equals(sentFromAtoC));

        //    //	For each row, get Product Status, Pre-product Code, CmdNo, LotNo, Production Date and Updated Date 1
        //    foreach (var kneadingCommandResult in kneadingCommandResults)
        //        if (kneadingCommandResult.KneadingStatus.Equals(conventionalB))
        //        {
        //            //system re-checks whether the kneading command is paused or not by checking if there is any existing record from “tx55_kndcmdmsrsnd” table where [f55_kndcmdno] is equal to CmdNo above 
        //            //and [f55_status] is “Sent from A to C” (or “0”)
        //            var isKneadingCommandPaused =
        //                kneadingCommandMsrSnds.Any(
        //                    x =>
        //                        x.F55_KndCmdNo.Trim().Equals(kneadingCommandResult.F42_KndCmdNo) &&
        //                        x.F55_Status.Trim().Equals(sentFromAtoC));

        //            //o	If no record found, then set [f39_status] in “tx39_pdtpln” table to “Termination”, 
        //            //where [f39_preproductcode] is equal to Pre-product Code, 
        //            //[f39_kndeptbgndate] is equal to Production Date, and [f39_updatedate] is equal to Updated Date 1
        //            if (isKneadingCommandPaused)
        //            {
        //                var pdtPlns =
        //                    productPlans.Where(
        //                        x =>
        //                            x.F39_PreProductCode.Trim().Equals(kneadingCommandResult.F42_PreProductCode) &&
        //                            (x.F39_KndEptBgnDate == kneadingCommandResult.ProductionDate) &&
        //                            (x.F39_UpdateDate == kneadingCommandResult.UpdatedDate1));

        //                foreach (var pdtPln in pdtPlns)
        //                {
        //                    var termination = Constants.F39_Status.Termination.ToString("D");
        //                    pdtPln.F39_Status = termination;
        //                }
        //            }
        //            else
        //            {
        //                var pdtPlns =
        //                    productPlans.Where(
        //                        x =>
        //                            x.F39_PreProductCode.Trim().Equals(kneadingCommandResult.F42_PreProductCode) &&
        //                            (x.F39_KndEptBgnDate == kneadingCommandResult.ProductionDate) &&
        //                            (x.F39_UpdateDate == kneadingCommandResult.UpdatedDate1));

        //                foreach (var pdtPln in pdtPlns)
        //                {
        //                    var terminating = Constants.F39_Status.Terminating.ToString("D");
        //                    pdtPln.F39_Status = terminating;
        //                }
        //            }
        //        }

        //    // Save changes to database.
        //    await _unitOfWork.CommitAsync();
        //}

        /// <summary>
        ///     This function is for start kneading commands.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="kneadingCommandResults"></param>
        /// <param name="kneadingLine"></param>
        //public async Task StartKneadingCommand(string terminalNo,
        //    IList<FindKneadingCommandItem> kneadingCommandResults, Constants.KndLine kneadingLine)
        //{
        //    // Refine terminal number by using specific rule defined in document.
        //    terminalNo = RefineTerminalNo(terminalNo);

        //    // Check whether the first record matches with the condition found or not.
        //    var bConditionMet = false;

        //    foreach (var kneadingCommandResult in kneadingCommandResults)
        //    {
        //        // Whether condition statisfied or not.
        //        var isConditionSatisfied = false;

        //        //	If Kneading Status is “Kneading” AND Product Status is “Terminating”, check if the selected record is the first record in data window or not.
        //        if (kneadingCommandResult.KneadingStatus.Trim().Equals(Constants.F42_Status.TX42_Sts_Knd)
        //            && kneadingCommandResult.ProductStatus.Trim().Equals(Constants.F39_Status.Terminating.ToString("D")))

        //            if (!bConditionMet)
        //            {
        //                //If yes, update [f39_status] in “tx39_pdtpln” table to “Kneading”, where [f39_preproductcode] 
        //                //is equal to Pre-product Code,
        //                //[f39_kndeptbgndate] is equal to Production Date and [f39_updatedate] is equal to Updated Date 1
        //                var productPlans = _unitOfWork.PdtPlnRepository.GetAll();
        //                productPlans =
        //                    productPlans.Where(
        //                        x =>
        //                            x.F39_PreProductCode.Trim().Equals(kneadingCommandResult.F42_PreProductCode) &&
        //                            x.F39_KndEptBgnDate.Equals(kneadingCommandResult.ProductionDate)
        //                            && x.F39_UpdateDate.Equals(kneadingCommandResult.UpdatedDate1));
        //                foreach (var productPlan in productPlans)
        //                    productPlan.F39_Status = Constants.F39_Status.Kneading.ToString("D");

        //                isConditionSatisfied = true;
        //            }

        //        //	If Kneading Status is “Not Kneaded” AND (Product Status is “Commanded” OR “Terminating” OR “Termination”)
        //        //if (kneadingCommandResult.KneadingStatus.Equals(Constants..ToString("D"))
        //        //    && (kneadingCommandResult.ProductStatus.Equals(Constants.F39_Status.Terminating.ToString("D") || ))
        //        //  	If Kneading Status is “Not Kneaded” AND (Product Status is “Commanded” OR “Terminating” OR “Termination”), then:
        //        if (kneadingCommandResult.KneadingStatus.Trim().Equals(Constants.F42_Status.TX42_Sts_NotKnd) &&
        //            (kneadingCommandResult.ProductStatus.Trim().Equals(Constants.F39_Status.Commanded.ToString("D")) ||
        //             kneadingCommandResult.ProductStatus.Trim().Equals(Constants.F39_Status.Terminating.ToString("D")) ||
        //             kneadingCommandResult.ProductStatus.Trim().Equals(Constants.F39_Status.Termination.ToString("D"))))
        //        {
        //            // Find the current system time.
        //            var systemTime = DateTime.Now;

        //            var tx55Kndcmdmsrsnds = _unitOfWork.KndCmdMsrSndRepository.GetAll();
        //            tx55Kndcmdmsrsnds =
        //                tx55Kndcmdmsrsnds.Where(
        //                    x =>
        //                        x.F55_KndCmdNo.Trim().Equals(kneadingCommandResult.F42_KndCmdNo) &&
        //                        x.F55_PrePdtLotNo.Equals(kneadingCommandResult.F42_KndCmdNo));

        //            var tx55Kndcmdmsrsnd = await tx55Kndcmdmsrsnds.FirstOrDefaultAsync();

        //            var isInsert = false;
        //            if (tx55Kndcmdmsrsnd == null)
        //            {
        //                tx55Kndcmdmsrsnd = new TX55_KndCmdMsrSnd();
        //                isInsert = true;
        //            }

        //            tx55Kndcmdmsrsnd.F55_KndCmdNo = kneadingCommandResult.F42_KndCmdNo;
        //            tx55Kndcmdmsrsnd.F55_PrePdtLotNo = kneadingCommandResult.LotNo;
        //            tx55Kndcmdmsrsnd.F55_PrePdtCode = kneadingCommandResult.F42_PreProductCode;
        //            tx55Kndcmdmsrsnd.F55_Status = Constants.F55_Status.SentFromAToC.ToString("D");
        //            tx55Kndcmdmsrsnd.F55_Priority = "0";
        //            tx55Kndcmdmsrsnd.F55_MsrSndCls = kneadingLine.ToString("D");
        //            tx55Kndcmdmsrsnd.F55_TerminalNo = terminalNo;
        //            tx55Kndcmdmsrsnd.F55_PictureNo = Constants.PictureNo.TCPS041F;
        //            tx55Kndcmdmsrsnd.F55_AbnormalCode = "";
        //            tx55Kndcmdmsrsnd.F55_AddDate = systemTime;
        //            tx55Kndcmdmsrsnd.F55_UpdateDate = systemTime;

        //            if (isInsert)
        //                _unitOfWork.KndCmdMsrSndRepository.Add(tx55Kndcmdmsrsnd);


        //            /*
        //             * o	Check if the selected record is the first record in data window. 
        //             * If yes, update [f39_status] in “tx39_pdtpln” table to “Kneading”, 
        //             * where [f39_preproductcode] is equal to Pre-product Code, [f39_kndeptbgndate] is equal to Production Date and [f39_updatedate] is equal to Updated Date 1.
        //             */
        //            if (!bConditionMet)
        //            {
        //                var result = kneadingCommandResult;

        //                var productPlans = _unitOfWork.PdtPlnRepository.GetAll();
        //                productPlans = productPlans.Where(
        //                        x =>
        //                            x.F39_PreProductCode.Trim().Equals(result.F42_PreProductCode) &&
        //                            (x.F39_KndEptBgnDate == result.ProductionDate) &&
        //                            (x.F39_UpdateDate == result.UpdatedDate1));

        //                //var tx39Pdtpln =
        //                //    _unitOfWork.PdtPlnRepository.Get(
        //                //        x =>
        //                //            x.F39_PreProductCode.Trim().Equals(result.F42_PreProductCode) &&
        //                //            (x.F39_KndEptBgnDate == result.ProductionDate) &&
        //                //            (x.F39_UpdateDate == result.UpdatedDate1));
        //                //if (tx39Pdtpln != null)
        //                //    tx39Pdtpln.F39_Status = Constants.F39_Status.Kneading.ToString("D");

        //                foreach (var productPlan in productPlans)
        //                {
        //                    productPlan.F39_Status = Constants.F39_Status.Kneading.ToString("D");
        //                    _unitOfWork.PdtPlnRepository.Update(productPlan);
        //                }
                            


        //                isConditionSatisfied = true;
        //            }
        //        }

        //        if (isConditionSatisfied)
        //            bConditionMet = true;
        //    }
        //    //System sends message to C4/C5 ....
        //    var lsSndMsg = "1002" + terminalNo + "Tcps041f" + "0020";
        //    if (lsSndMsg.Length != 20)
        //        throw new Exception("MSG 9");
        //    _notificationService.SendMessageToC4(lsSndMsg);

        //    // Save changes to database.
        //    var records = await _unitOfWork.CommitAsync();
        //}

        /// <summary>
        ///     This function is for preventing kneading command from executing.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="kneadingCommandResult"></param>
        //public async Task StopKneadingCommand(string terminalNo, FindKneadingCommandItem kneadingCommandResult)
        //{
        //    var sentFromAtoC = Constants.F55_Status.SentFromAToC.ToString();

        //    //	Get Updated Date 3 from [f55_updatedate] in “tx55_kndcmdmsrsnd” table 
        //    //where [f55_kndcmdno] is equal to CmdNo, [f55_prepdtlotno] is equal to LotNo above AND [f55_status] is “Sent from A to C” (or “0”)
        //    var kndcmdmsrsnds = _unitOfWork.KndCmdMsrSndRepository.GetAll();
        //    kndcmdmsrsnds = kndcmdmsrsnds.Where(x => x.F55_KndCmdNo.Trim().Equals(kneadingCommandResult.F42_KndCmdNo));
        //    kndcmdmsrsnds = kndcmdmsrsnds.Where(x => x.F55_PrePdtLotNo.Trim().Equals(kneadingCommandResult.LotNo));
        //    kndcmdmsrsnds = kndcmdmsrsnds.Where(x => x.F55_Status.Trim().Equals(sentFromAtoC));
        //    var kndcmdmsrsnd = kndcmdmsrsnds.FirstOrDefault();

        //    if (kndcmdmsrsnd != null)
        //    {
        //        //•	Delete that record from “tx55_kndcmdmsrsnd” table where [f55_kndcmdno] is equal to CmdNo, 
        //        //[f55_prepdtlotno] is equal to LotNo, [f55_updatedate] is equal to Updated Date 3 above AND [f55_status] is “Sent from A to C” (or “0”).
        //        _unitOfWork.KndCmdMsrSndRepository.Delete(
        //            x => x.F55_KndCmdNo.Trim().Equals(kneadingCommandResult.F42_KndCmdNo)
        //                 && x.F55_PrePdtLotNo.Trim().Equals(kneadingCommandResult.LotNo)
        //                 && x.F55_Status.Trim().Equals(sentFromAtoC)
        //                 && (x.F55_UpdateDate == kndcmdmsrsnd.F55_UpdateDate));

        //        //If the query runs well but no record is affected from the above query, system shows message MSG 12
        //        var affectedItem =
        //            _unitOfWork.KndCmdMsrSndRepository.Get(
        //                x => x.F55_KndCmdNo.Trim().Equals(kneadingCommandResult.F42_KndCmdNo)
        //                     && x.F55_PrePdtLotNo.Trim().Equals(kneadingCommandResult.LotNo)
        //                     && x.F55_Status.Trim().Equals(sentFromAtoC)
        //                     && (x.F55_UpdateDate == kndcmdmsrsnd.F55_UpdateDate));

        //        //If the query runs well but no record is affected from the above query, system shows message MSG 12
        //        if (affectedItem == null)
        //            throw new Exception("MSG12");
        //    }

        //    //•	Update [f42_status] in “tx42_kndcmd” table of that record to “Forced Completed”, 
        //    //where [f42_kndcmdno] is equal to CmdNo, [f42_prepdtlotno] is equal to LotNo and [f42_updatedate] is equal to Updated Date 2 above
        //    var tx42Records = _unitOfWork.KneadingCommandRepository.GetAll();
        //    tx42Records = tx42Records.Where(x => x.F42_KndCmdNo.Trim().Equals(kneadingCommandResult.F42_KndCmdNo));
        //    tx42Records = tx42Records.Where(x => x.F42_PrePdtLotNo.Trim().Equals(kneadingCommandResult.LotNo));
        //    tx42Records = tx42Records.Where(x => x.F42_UpdateDate.Equals(kneadingCommandResult.UpdateDate2));

        //    foreach (var tx42Record in tx42Records)
        //        tx42Record.F42_Status = Constants.F42_Status.TX42_Sts_FrcCmp;

        //    //•	Suppose Lot Amount, Lot End Amount and Updated Date 4 are retrieved from [f39_prepdtlotamt], [f39_endlotamont] and [f39_updatedate] 
        //    //of “tx39_pdtpln” table where [f39_preproductcode] is equal to Pre-product Code and [f39_kndeptbgndate] is equal to Production Date above
        //    var f39Records = _unitOfWork.PdtPlnRepository.GetAll();
        //    f39Records = f39Records.Where(x => x.F39_PreProductCode.Trim().Equals(kneadingCommandResult.F42_PreProductCode));
        //    f39Records = f39Records.Where(x => x.F39_KndEptBgnDate.Equals(kneadingCommandResult.ProductionDate));
        //    var f39Record = f39Records.FirstOrDefault();
        //    if (f39Record == null)
        //        throw new Exception();

        //    //•	System re-checks whether the kneading command is stopped or not by checking if there is 
        //    //any existing record from “tx55_kndcmdmsrsnd” table where [f55_kndcmdno] is equal to CmdNo and [f55_status] is “Sent from A to C” (or “0”)

        //    //	If there is no record found, means the kneading command is over. 
        //    if (kndcmdmsrsnd == null)
        //    {
        //        //System will double check if Lot Amount is equal to Lot End Amount plus 1. 
        //        ////If it is correct, then Kneading is truly over
        //        if (f39Record.F39_PrePdtLotAmt == f39Record.F39_EndLotAmont + 1)
        //        {
        //            //Once the Kneading is over, system will update [f39_status] of that record in “tx39_pdtpln” 
        //            //table to “Completed” and increment [f39_endlotamont] by 1, where [f39_kndeptbgndate] is equal to Production Date and [f39_updatedate] is equal to Updated Date 4
        //            var tx39PdtplnRecords = _unitOfWork.PdtPlnRepository.GetAll();
        //            tx39PdtplnRecords =
        //                tx39PdtplnRecords.Where(x => x.F39_KndEptBgnDate == kneadingCommandResult.ProductionDate);
        //            tx39PdtplnRecords = tx39PdtplnRecords.Where(x => x.F39_UpdateDate == f39Record.F39_UpdateDate);

        //            foreach (var tx39PdtplnRecord in tx39PdtplnRecords)
        //            {
        //                tx39PdtplnRecord.F39_Status = Constants.F39_Status.Completed.ToString("D");
        //                tx39PdtplnRecord.F39_EndLotAmont++;
        //                _unitOfWork.PdtPlnRepository.Update(tx39PdtplnRecord);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        //	If there is existing record found, which means the kneading is not over yet. 
        //        //Then system updates [f39_endlotamont] of that record in “tx39_pdtpln” table by increment itself by 1, 
        //        //where [f39_kndeptbgndate] is equal to Production Date and [f39_updatedate] is equal to Updated Date 4.
        //        var tx39PdtplnRecords = _unitOfWork.PdtPlnRepository.GetAll();
        //        tx39PdtplnRecords =
        //            tx39PdtplnRecords.Where(x => x.F39_KndEptBgnDate == kneadingCommandResult.ProductionDate);
        //        tx39PdtplnRecords = tx39PdtplnRecords.Where(x => x.F39_UpdateDate == f39Record.F39_UpdateDate);

        //        foreach (var tx39PdtplnRecord in tx39PdtplnRecords)
        //        {
        //            tx39PdtplnRecord.F39_EndLotAmont++;
        //            tx39PdtplnRecord.F39_KndEptBgnDate = kneadingCommandResult.ProductionDate;
        //            tx39PdtplnRecord.F39_UpdateDate = f39Record.F39_UpdateDate;

        //            _unitOfWork.PdtPlnRepository.Update(tx39PdtplnRecord);
        //        }
        //    }

        //    // Save changes.
        //    _unitOfWork.Commit();
        //}

        /// <summary>
        ///     Refine terminal number by using rule defined in document.
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        private string RefineTerminalNo(string terminalNo)
        {
            if (!string.IsNullOrEmpty(terminalNo))
            {
                //	Remove leading and trailing spaces of [gs_TermNo]
                terminalNo = terminalNo.Trim();

                //If its length is less than 4, then fill those last missing spaces by ' ' before return [gs_TermNo].
                for (var i = terminalNo.Length; i < 4; i++)
                    terminalNo += " ";

                return terminalNo;
            }

            return "    ";
        }
        #endregion
    }
}