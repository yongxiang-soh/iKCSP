using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Core.Enumerations;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Interfaces.TabletisingCommandSubSystem;
using KCSG.Web.Areas.TabletisingCommandSubSystem.Models.ManagementOfProductLabel;
using KCSG.Web.Attributes;
using KCSG.Web.Models;
using KCSG.Web.Controllers;

namespace KCSG.Web.Areas.TabletisingCommandSubSystem.Controllers
{
    [MvcAuthorize("TCMD031F")]
    public class ManagementOfProductLabelController : BaseController
    {
        /// <summary>
        /// Domain which is for handling product label management.
        /// </summary>
        private readonly IManagementOfProductLabelDomain _managementOfProductLabelDomain;

        /// <summary>
        /// Domain which is for handling report export.
        /// </summary>
        private readonly IExportReportDomain _exportReportDomain;

        /// <summary>
        /// Service which handles system configuration.
        /// </summary>
        private readonly IConfigurationService _configurationService;

        /// <summary>
        /// Label print service.
        /// </summary>
        private readonly ILabelPrintService _labelPrintService;

        /// <summary>
        /// Service which handles identity operation.
        /// </summary>
        private readonly IIdentityService _identityService;

        /// <summary>
        /// Initiate controller with IoC.
        /// </summary>
        /// <param name="managementOfProductLabelDomain"></param>
        /// <param name="exportReportDomain"></param>
        /// <param name="configurationService"></param>
        public ManagementOfProductLabelController(
            IManagementOfProductLabelDomain managementOfProductLabelDomain,
            IExportReportDomain exportReportDomain,
            IConfigurationService configurationService,
            ILabelPrintService labelPrintService,
            IIdentityService identityService
            )
        {
            _managementOfProductLabelDomain = managementOfProductLabelDomain;
            _exportReportDomain = exportReportDomain;
            _configurationService = configurationService;
            _labelPrintService = labelPrintService;
            _identityService = identityService;
        }

        //
        // GET: /TabletisingCommandSubSystem/ManagementOfProductLabel/
        public ActionResult Index(string commandNo, string productCode, string lotNo, string tableLine)
        {
            var model = new ManagementOfProductLabelViewModel();
            
            if (!string.IsNullOrEmpty(commandNo) && !string.IsNullOrEmpty(productCode))
            {
                var product = _managementOfProductLabelDomain.GetProductItem(productCode.Trim());
                var kneading = _managementOfProductLabelDomain.GetKneadingCommand(commandNo.Trim(), lotNo.Trim());
                model.CsNo1 = _managementOfProductLabelDomain.GetCSNo1(commandNo.Trim(), lotNo.Trim()) ?? 0;
                model.MainFlow = false;
                model.Mode = "Online";
                model.CmdNo = commandNo;
                model.ProductionCode = productCode;
                model.ProductionName = product.F09_ProductDesp;
                model.ShelfLife = Int32.Parse(product.F09_ValidPeriod);
                model.Pieces = 1;
                model.ExternalModelName = product.F09_ProductDesp;
                model.InternalLabel = product.F09_ProductDesp;
                model.InternalLabelType = product.F09_TabletType;
                model.InternalLotNo = lotNo;
                model.ExternalLotNo = lotNo;
                model.CsNo1 = 1;
                model.CsNo2 = 0;
                model.Size1 = Int32.Parse(product.F09_TabletSize);
                model.InternalSize1 = Int32.Parse(product.F09_TabletSize);
                var lsSize2 = double.Parse(product.F09_TabletSize2);
                if (Math.Floor(lsSize2) == 0)
                {
                    model.Size2 = lsSize2 - 1;
                    model.InternalSize2 = lsSize2 - 1;
                }
                else
                {
                    model.Size2 = lsSize2;
                    model.InternalSize2 = lsSize2;
                }
                
                model.Quantity = product.F09_PackingUnit;
                model.ExternalLabel = Constants.ExternalLabel.Piece.ToString("D");
                model.InternalLabel = Constants.InternalLabel.Piece.ToString("D");
                if (kneading != null)
                {
                    if (kneading.F42_KndEptBgnDate != null)
                    {
                        model.MfgDate = kneading.F42_KndEptBgnDate.ToString("dd/MM/yyyy");
                        model.Expired = kneading.F42_KndEptBgnDate.AddDays(double.Parse(product.F09_ValidPeriod)).ToString();
                    }
                }
                else
                {
                    model.MfgDate = DateTime.Now.ToString("dd/MM/yyyy");
                    model.Expired = DateTime.Now.AddDays(double.Parse(product.F09_ValidPeriod)).ToString();
                }
            }
            else
            {
                model.MainFlow = true;
                model.ShelfLife = 9;
                model.CsNo1 = 1;
                model.CsNo2 = 0;
                model.Mode = "Offline";
                model.ExternalLabel = Constants.ExternalLabel.No.ToString("D");
                model.InternalLabel = Constants.InternalLabel.No.ToString("D");
                model.Pieces = 1;
                model.MfgDate = DateTime.Now.ToString("dd/MM/yyyy");
            }

            return View(model);
        }


        /// <summary>
        /// Print label from configuration.
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Print(ManagementOfProductLabelViewModel parameters)
        {
            #region Parameters validation

            // Parameter hasn't been 
            if (parameters == null)
            {
                parameters = new ManagementOfProductLabelViewModel();
                TryValidateModel(parameters);
            }

            if (!ModelState.IsValid)
            {
                // Tell the client parameters submitted to server is invalid.
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            #endregion

            #region Label print

            // Find terminal from idenitty.
            var terminalName = _identityService.FindTerminalNo(HttpContext.User.Identity);

            // Print internal label.
            await PrintInternalLabelAsync(parameters, terminalName);
            await PrintExternalLabelAsync(parameters, terminalName);

            #endregion

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        /// <summary>
        /// Print internal label using specific conditions.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        private async Task PrintInternalLabelAsync(ManagementOfProductLabelViewModel parameters, string terminal)
        {
            // No internal label is specified for printing.
            if (string.IsNullOrEmpty(parameters.InternalLabel) || "1".Equals(parameters.InternalLabel) && "0".Equals(parameters.CodeLabel))
                return;

            var originalLabelContent = "";
            var copies = 1;
            var vendorCode = "";
            var specificCode = "";
            var codeHeading = "";
            bool smallFont = false;

            //! Check for Small Font
            if (parameters.SmallFont)
                smallFont = true;
            else
                smallFont = false;

            // Find number of copies from selected radio button.
            if (Constants.InternalLabel.Pieces.ToString("D").Equals(parameters.InternalLabel) ||
                Constants.InternalLabel.Atm2P.ToString("D").Equals(parameters.InternalLabel) ||
                Constants.InternalLabel.Buzen2P.ToString("D").Equals(parameters.InternalLabel) ||
                Constants.InternalLabel.Sts2P.ToString("D").Equals(parameters.InternalLabel) ||
                Constants.InternalLabel.Stlgg2P.ToString("D").Equals(parameters.InternalLabel) ||
                Constants.InternalLabel.Renesas2P.ToString("D").Equals(parameters.InternalLabel))
                copies = 2;

            // Find printer content.
            if (Constants.CodeLabel.Atm.ToString("D").Equals(parameters.CodeLabel))
            {
                //! Specific Code Label
                if (!string.IsNullOrEmpty(parameters.SpecificCodeLabel))
                    specificCode = parameters.SpecificCodeLabel + " ";
                else
                    specificCode = "ATM ";

                //! KAP Yes/No
                if (parameters.KAP)
                    vendorCode = "KAP";
                else
                    vendorCode = "KCS";

                if (!string.IsNullOrEmpty(parameters.CodeHeading))
                    codeHeading = parameters.CodeHeading + " ";
                else
                    codeHeading = ""; 

                originalLabelContent = _labelPrintService.FindInternalPrintContent(InternalLabelCollection.AtmKap);
            }
            else
            {
                if (Constants.InternalLabel.Piece.ToString("D").Equals(parameters.InternalLabel) ||
                    Constants.InternalLabel.Pieces.ToString("D").Equals(parameters.InternalLabel))
                {
                    if (smallFont)
                        originalLabelContent = _labelPrintService.FindInternalPrintContent(InternalLabelCollection.PieceSmallFont);
                    else
                        originalLabelContent = _labelPrintService.FindInternalPrintContent(InternalLabelCollection.Piece);
                }
                    

                else if (Constants.InternalLabel.Atm1P.ToString("D").Equals(parameters.InternalLabel) ||
                    Constants.InternalLabel.Atm2P.ToString("D").Equals(parameters.InternalLabel))
                    originalLabelContent = _labelPrintService.FindInternalPrintContent(InternalLabelCollection.Atm);

                else if (Constants.InternalLabel.Buzen1P.ToString("D").Equals(parameters.InternalLabel) ||
                    Constants.InternalLabel.Buzen2P.ToString("D").Equals(parameters.InternalLabel))
                    originalLabelContent = _labelPrintService.FindInternalPrintContent(InternalLabelCollection.Buzen);

                else if (Constants.InternalLabel.Sts1P.ToString("D").Equals(parameters.InternalLabel) ||
                    Constants.InternalLabel.Sts2P.ToString("D").Equals(parameters.InternalLabel))
                    originalLabelContent = _labelPrintService.FindInternalPrintContent(InternalLabelCollection.Sts);

                else if (Constants.InternalLabel.Stlgg1P.ToString("D").Equals(parameters.InternalLabel) ||
                    Constants.InternalLabel.Stlgg2P.ToString("D").Equals(parameters.InternalLabel))
                    originalLabelContent = _labelPrintService.FindInternalPrintContent(InternalLabelCollection.Stlgg);

                else if (Constants.InternalLabel.Renesas.ToString("D").Equals(parameters.InternalLabel) ||
                    Constants.InternalLabel.Renesas2P.ToString("D").Equals(parameters.InternalLabel))
                    originalLabelContent = _labelPrintService.FindInternalPrintContent(InternalLabelCollection.Renesas);
            }

            if (parameters.CsNo2 < parameters.CsNo1)
                parameters.CsNo2 = parameters.CsNo1;

            var szSupplierName = "";

            if (parameters.SupplierName == "1")
                szSupplierName = "KAP";
            else
                szSupplierName = "KCSP";

            // Print piece by piece.
            for (var csNo = parameters.CsNo1; csNo <= parameters.CsNo2; csNo++)
            {
                var printParameters = new
                {
                    InternalModelName = parameters.ExternalModelName == null ? parameters.ExternalModelName : parameters.ExternalModelName.Trim(),
                    ProductionCode = parameters.ProductionCode == null ? parameters.ProductionCode : parameters.ProductionCode.Trim(),
                    Productionname = parameters.ProductionName == null ? parameters.ProductionName : parameters.ProductionName.Trim(),
                    InternalLotNo = parameters.InternalLotNo == null ? parameters.InternalLotNo : parameters.InternalLotNo.Trim(),
                    parameters.ExternalLotNo,
                    InternalLabelType = parameters.InternalLabelType == null ? parameters.InternalLabelType : parameters.InternalLabelType.Trim(),
                    parameters.InternalSize1,
                    parameters.InternalSize2,
                    parameters.MfgDate,
                    CsNo = csNo.ToString("D3"),
                    pieceCsNo = csNo,
                    atmQuantity = parameters.Quantity.ToString().PadLeft(3, '0').PadRight(5, '0'),
                    VendorCode = vendorCode,
                    SupplierName = szSupplierName,
                    PartNo = parameters.ScsPartNo,
                    parameters.Quantity,
                    CodeHeading = codeHeading,
                    SpecificCodeLabel = specificCode
                };

                for (var piece = 0; piece < parameters.Pieces; piece++)
                {
                    // Find external label form.
                    var labelContent = await _exportReportDomain.ExportToFlatFileAsync(printParameters, originalLabelContent);

                    // Find all active printers in the list.
                    var internalLabelPrinters = _labelPrintService.InternalPrinters.Where(x => x.IsEnabled && x.Terminal.Equals(terminal));
                    foreach (var internalLabelPrinter in internalLabelPrinters)
                    {
                        for (var copy = 0; copy < copies; copy++)
                        {
                            if (internalLabelPrinter.IsUsbPrinter)
                            {
                                _exportReportDomain.ExportLabelPrint(internalLabelPrinter,
                                    _configurationService.ApplicationDataPath, labelContent);
                            }
                            else
                            {
                                _labelPrintService.Print(
                                    new IPEndPoint(IPAddress.Parse(internalLabelPrinter.Ip), internalLabelPrinter.Port),
                                    labelContent);
                            }
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Print internal label using specific conditions.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="terminal"></param>
        /// <returns></returns>
        private async Task PrintExternalLabelAsync(ManagementOfProductLabelViewModel parameters, string terminal)
        {
            // No internal label is specified for printing.
            if (string.IsNullOrEmpty(parameters.ExternalLabel) || "1".Equals(parameters.ExternalLabel))
                return;

            var originalLabelContent = "";
            bool smallFont = false;

            //! Check for Small Font
            if (parameters.SmallFont)
                smallFont = true;
            else
                smallFont = false;

            if (Constants.ExternalLabel.Piece.ToString("D").Equals(parameters.ExternalLabel))
            {
                if(smallFont)
                    originalLabelContent = _labelPrintService.FindExternalPrintContent(ExternalLabelCollection.PieceSmall);
                else
                    originalLabelContent = _labelPrintService.FindExternalPrintContent(ExternalLabelCollection.Piece);
            }
            else if (Constants.ExternalLabel.Atm.ToString("D").Equals(parameters.ExternalLabel))
                originalLabelContent = _labelPrintService.FindExternalPrintContent(ExternalLabelCollection.Atm);

            else if (Constants.ExternalLabel.Buzen.ToString("D").Equals(parameters.ExternalLabel))
                originalLabelContent = _labelPrintService.FindExternalPrintContent(ExternalLabelCollection.Buzen);

            else if (Constants.ExternalLabel.Stlgg.ToString("D").Equals(parameters.ExternalLabel))
                originalLabelContent = _labelPrintService.FindExternalPrintContent(ExternalLabelCollection.Stlgg);

            else if (Constants.ExternalLabel.Chippac.ToString("D").Equals(parameters.ExternalLabel))
                originalLabelContent = _labelPrintService.FindExternalPrintContent(ExternalLabelCollection.Chippac);

            else if (Constants.ExternalLabel.Sts.ToString("D").Equals(parameters.ExternalLabel))
                originalLabelContent = _labelPrintService.FindExternalPrintContent(ExternalLabelCollection.Sts);

            else if (Constants.ExternalLabel.Renesas.ToString("D").Equals(parameters.ExternalLabel))
                originalLabelContent = _labelPrintService.FindExternalPrintContent(ExternalLabelCollection.Renesas);

            else if (Constants.ExternalLabel.GTBF.ToString("D").Equals(parameters.ExternalLabel))
                originalLabelContent = _labelPrintService.FindExternalPrintContent(ExternalLabelCollection.GTBF);

            // Print piece by piece.
            if (parameters.CsNo2 < parameters.CsNo1)
                parameters.CsNo2 = parameters.CsNo1;

            var product = _managementOfProductLabelDomain.GetProductItem(parameters.ProductionCode.Trim());

            for (var csNo = parameters.CsNo1; csNo <= parameters.CsNo2; csNo++)
            {
                for (var piece = 0; piece < parameters.Pieces; piece++)
                {
                    var szQuantity = "";
                    if (parameters.Quantity != null)
                        szQuantity = parameters.Quantity.Value.ToString().PadLeft(3, '0');
                    else
                        szQuantity = "000";

                    var szSupplierName = "";
                    if (parameters.SupplierName == "1")
                    {
                        szSupplierName = "KAP";
                    }
                    else
                    {
                        szSupplierName = "KCSP";
                    }

                    string[] dateSplitMfgDate = parameters.MfgDate.Split('/');
                    var szMfgDateGTBF = dateSplitMfgDate[2] + "/" + dateSplitMfgDate[1] + "/" + dateSplitMfgDate[0];

                    string[] dateSplit = parameters.Expired.Split('/');

                    var szExpiredGTBF = dateSplit[2] + "/" + dateSplit[1] + "/" + dateSplit[0];

                    if (dateSplit[1] == "01")
                        dateSplit[1] = "JAN";
                    else if(dateSplit[1] == "02")
                        dateSplit[1] = "FEB";
                    else if (dateSplit[1] == "03")
                        dateSplit[1] = "MAR";
                    else if (dateSplit[1] == "04")
                        dateSplit[1] = "APR";
                    else if (dateSplit[1] == "05")
                        dateSplit[1] = "MAY";
                    else if (dateSplit[1] == "06")
                        dateSplit[1] = "JUN";
                    else if (dateSplit[1] == "07")
                        dateSplit[1] = "JUL";
                    else if (dateSplit[1] == "08")
                        dateSplit[1] = "AUG";
                    else if (dateSplit[1] == "09")
                        dateSplit[1] = "SEP";
                    else if (dateSplit[1] == "10")
                        dateSplit[1] = "OCT";
                    else if (dateSplit[1] == "11")
                        dateSplit[1] = "NOV";
                    else if (dateSplit[1] == "12")
                        dateSplit[1] = "DEC";

                    var szExpiredSTLGG = dateSplit[0] + "-" + dateSplit[1] + "-" + dateSplit[2];
                    
                    var printParameters = new
                    {
                        ExternalModelName = parameters.ProductionName.Trim(),
                        ProductCode = parameters.ProductionCode != null ? parameters.ProductionCode.Trim() : null,
                        ProductName = parameters.ProductionName != null ? parameters.ProductionName.Trim() : null,
                        InternalLotNo = parameters.InternalLotNo != null ? parameters.InternalLotNo.Trim() : null,
                        ExternalLotNo = parameters.ExternalLotNo != null ? parameters.ExternalLotNo.Trim() : null,
                        ExternalLabelType = parameters.ExternalLabelType != null ? parameters.ExternalLabelType.Trim() : null,
                        parameters.Size1,
                        parameters.Size2,
                        CsNo = csNo,
                        parameters.MfgDate,  
                        parameters.Expired,
                        StandaloneCsNo = csNo.ToString("D3"),
                        StandaloneQuantity = szQuantity,
                        Quantity = product.F09_PackingUnit,
                        //StandaloneQuantity =
                        //parameters.Quantity.HasValue ? parameters.Quantity.Value.ToString("D3") : "000",
                        PartNo = parameters.ScsPartNo,
                        SupplierName = szSupplierName,
                        ExpiredSTLGG = szExpiredSTLGG,
                        ExpiredGTBF = szExpiredGTBF, 
                        MfgDateGTBF = szMfgDateGTBF
                    };

                    // Find label content.
                    var labelContent =
                        await _exportReportDomain.ExportToFlatFileAsync(printParameters, originalLabelContent);

                    // Find all active printers in the list.
                    var externalLabelPrinters = _labelPrintService.ExternalPrinters.Where(x => x.IsEnabled && x.Terminal.Equals(terminal));
                    foreach (var externalLabelPrinter in externalLabelPrinters)
                    {
                        if (externalLabelPrinter.IsUsbPrinter)
                        {
                            _exportReportDomain.ExportLabelPrint(externalLabelPrinter,
                                _configurationService.ApplicationDataPath, labelContent);
                        }
                        else
                        {
                            _labelPrintService.Print(
                                new IPEndPoint(IPAddress.Parse(externalLabelPrinter.Ip), externalLabelPrinter.Port),
                                labelContent);
                        }
                    }
                }
            }
        }
    }
}