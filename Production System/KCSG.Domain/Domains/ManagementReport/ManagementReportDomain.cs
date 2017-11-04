using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using System.Web.Configuration;
using AutoMapper;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.ManagementReport;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.ManagementReport;
using KCSG.Domain.Models.Inquiry.ByPreProductName;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.Inquiry
{
    public class ManagementReportDomain : BaseDomain, IManagementReportDomain
    {
        #region Constructor

        public ManagementReportDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion


        
        /// <summary>
        ///     This function is for exporting data for management report module.
        /// </summary>
        /// <returns></returns>
        public async Task<PrintManagementReportItem> SearchMaterialMovementHistory(string from, string to)
        {
            if (string.IsNullOrEmpty(from))
            {
                from = "01/01/0001";
            }
            if (string.IsNullOrEmpty(to))
            {
                to = "30/12/9999";
            }

            DateTime fromdate = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todate = DateTime.ParseExact(to, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            // Find all tx01
            var materials = _unitOfWork.MaterialRepository.GetAll();

            // Find all tx61
            var materialStorageRetrieveHistorys = _unitOfWork.MaterialStorageRetrieveHistoryRepository.GetAll().Where(i => i.F61_StgRtrDate >= fromdate && i.F61_StgRtrDate <= todate); ;

            
            var printManagementReportItem = new PrintManagementReportItem();


            // Find Management Report item.
            var managementReportItems = from materialStorageRetrieveHistory in materialStorageRetrieveHistorys
                                        join material in materials on materialStorageRetrieveHistory.F61_MaterialCode equals material.F01_MaterialCode

                                        orderby new
                                        {
                                            materialStorageRetrieveHistory.F61_StgRtrDate,
                                            materialStorageRetrieveHistory.F61_PalletNo,
                                            materialStorageRetrieveHistory.F61_MaterialCode,
                                            materialStorageRetrieveHistory.F61_MaterialLotNo

                                        }
                                        select new FindPrintManagementReportItem()
                                        {
                                            SDate1 = materialStorageRetrieveHistory.F61_StgRtrDate,
                                            SRF = materialStorageRetrieveHistory.F61_StgRtrCls,
                                            From = materialStorageRetrieveHistory.F61_From,
                                            To = materialStorageRetrieveHistory.F61_To,
                                            PalletNo = materialStorageRetrieveHistory.F61_PalletNo,
                                            PONo = materialStorageRetrieveHistory.F61_PrcPdtNo,
                                            PD = materialStorageRetrieveHistory.F61_PrtDvrNo,
                                            MaterialCode = materialStorageRetrieveHistory.F61_MaterialCode,
                                            MaterialName = material.F01_MaterialDsp,
                                            LotNo = materialStorageRetrieveHistory.F61_MaterialLotNo,
                                            PU = material.F01_PackingUnit,
                                            Unit = material.F01_Unit,
                                            Quantity = materialStorageRetrieveHistory.F61_Amount
                                        };

            var firstResult = from managementReportItem in managementReportItems
                              group managementReportItems by new
                              {
                                  managementReportItem.MaterialCode,
                                  managementReportItem.Quantity,
                              }
                                  into firstGroup
                              select firstGroup;

            var totalFirstItems = (await firstResult.SumAsync(x => (double?)x.Key.Quantity)) ?? 0;


            //•	o	Show “PAGE: “ + current page with format as ###            
            printManagementReportItem.Page = "1";

            // Get Total quantity
            printManagementReportItem.Total = totalFirstItems.ToString("###,###,###,##0.00");

            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printManagementReportItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printManagementReportItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printManagementReportItem.FindPrintManagementReportItem = await managementReportItems.ToListAsync();

            var groupby = printManagementReportItem.FindPrintManagementReportItem.Select(m => new FindPrintManagementReportItem()
            {
                SDate = m.SDate1.HasValue ? m.SDate1.Value.ToString("dd/MM/yyyy") : "",
                Stime = m.SDate1.HasValue ? m.SDate1.Value.ToString("HH:mm") : "",
                SRF = m.SRF,
                Conveyor = m.SRF.Trim().Equals("1")? m.To : m.From,
                RBL = m.SRF.Trim().Equals("1") ? m.From.Substring(0, 2) +"-"+ m.From.Substring(2, 2) + "-" + m.From.Substring(4, 2) : (m.To.Substring(0, 2) + "-" + m.To.Substring(2, 2) + "-" + m.To.Substring(4, 2)),
                PalletNo = m.PalletNo,
                PONo = m.PONo,
                PD = m.PD,
                MaterialCode = m.MaterialCode,
                MaterialName = m.MaterialName,
                LotNo = m.LotNo,
                PUstring = m.PU.ToString("###,###,###,##0.00"),
                Unit = m.Unit,
                Quantity = m.Quantity,
                Quantityst = m.Quantity.ToString("###,###,###,##0.00")
            }).ToList().GroupBy(m => new{ m.SDate, m.PalletNo} );
            var groupitems = new List<GroupItems>();
            foreach (var items in groupby)
            {
                var groupitem = new GroupItems();
                groupitem.FindPrintManagementReportItem = items.ToList();
                groupitem.Totalgroup = groupitem.FindPrintManagementReportItem.Sum(i => i.Quantity).ToString("###,###,###,##0.00");
                groupitems.Add(groupitem);
            }
            printManagementReportItem.GroupItemses = groupitems;
            return printManagementReportItem;
        }
        public async Task<PrintManagementReportItem> SearchPreProductMovementHistory(string from, string to)
        {
            if (string.IsNullOrEmpty(from))
            {
                from = "01/01/0001";
            }
            if (string.IsNullOrEmpty(to))
            {
                to = "30/12/9999";
            }

            DateTime fromdate = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todate = DateTime.ParseExact(to, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            // Find all tx03
            var materials = _unitOfWork.PreProductRepository.GetAll();

            // Find all tx64
            var materialStorageRetrieveHistorys = _unitOfWork.PreProductStorageRetrieveHistoryRepository.GetAll().Where(i => i.F64_StgRtrDate >= fromdate && i.F64_StgRtrDate <= todate); ;

            
            var printManagementReportItem = new PrintManagementReportItem();


            // Find Management Report item.
            var managementReportItems = from materialStorageRetrieveHistory in materialStorageRetrieveHistorys
                                        join material in materials on materialStorageRetrieveHistory.F64_PreProductCode equals material.F03_PreProductCode

                                        orderby new
                                        {
                                            materialStorageRetrieveHistory.F64_StgRtrDate,
                                            materialStorageRetrieveHistory.F64_ContainerCode

                                        }
                                        select new FindPrintManagementReportItem()
                                        {
                                            SDate1 = materialStorageRetrieveHistory.F64_StgRtrDate,
                                            SRF = materialStorageRetrieveHistory.F64_StgRtrCls,
                                            From = materialStorageRetrieveHistory.F64_From,
                                            To = materialStorageRetrieveHistory.F64_To,
                                            ContainerCode = materialStorageRetrieveHistory.F64_ContainerCode,
                                            PreProductCode = materialStorageRetrieveHistory.F64_PreProductCode,
                                            PreProductName = material.F03_PreProductName,
                                            LotNo = materialStorageRetrieveHistory.F64_PreProductLotNo,
                                            Quantity = materialStorageRetrieveHistory.F64_Amount
                                        };




            //•	o	Show “PAGE: “ + current page with format as ###            
            printManagementReportItem.Page = "1";

            // Get Total quantity
            printManagementReportItem.Total = 0.ToString("###,###,###,##0.00");

            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printManagementReportItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printManagementReportItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printManagementReportItem.FindPrintManagementReportItem = await managementReportItems.ToListAsync();

            printManagementReportItem.FindPrintManagementReportItem = printManagementReportItem.FindPrintManagementReportItem.Select(m => new FindPrintManagementReportItem()
            {
                SDate = m.SDate1.HasValue ? m.SDate1.Value.ToString("dd/MM/yyyy") : "",
                Stime = m.SDate1.HasValue ? m.SDate1.Value.ToString("HH:mm") : "",
                SRF = m.SRF,
                Conveyor = m.SRF.Trim().Equals("2")? m.From : m.To,
                RBL = m.SRF.Trim().Equals("2") ? m.To.Substring(0, 2) +"-"+ m.To.Substring(2, 2) + "-" + m.To.Substring(4, 2) : (m.From.Substring(0, 2) + "-" + m.From.Substring(2, 2) + "-" + m.From.Substring(4, 2)),
                ContainerCode = m.ContainerCode,
                PreProductName = m.PreProductName,
                PreProductCode = m.PreProductCode,
                LotNo = m.LotNo,
                Quantityst = m.Quantity.ToString("###,###,###,##0.00")
            }).ToList();
            return printManagementReportItem;
        }
        public async Task<PrintManagementReportItem> SearchProductMovementHistory(string from, string to)
        {
            if (string.IsNullOrEmpty(from))
            {
                from = "01/01/0001";
            }
            if (string.IsNullOrEmpty(to))
            {
                to = "30/12/9999";
            }

            DateTime fromdate = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todate = DateTime.ParseExact(to, "dd/MM/yyyy", CultureInfo.InvariantCulture);


            // Find all tx66
            var materialStorageRetrieveHistorys = _unitOfWork.ProductWarehouseCommandHistoryRepository.GetAll().Where(i => i.F66_AddDate >= fromdate && i.F66_AddDate <= todate); ;


            var printManagementReportItem = new PrintManagementReportItem();


            // Find Management Report item.
            var managementReportItems = from materialStorageRetrieveHistory in materialStorageRetrieveHistorys
                                        orderby new
                                        {
                                            materialStorageRetrieveHistory.F66_AddDate,
                                            materialStorageRetrieveHistory.F66_CmdSeqNo

                                        }
                                        select new FindPrintManagementReportItem()
                                        {
                                            SDate1 = materialStorageRetrieveHistory.F66_CommandEndDate,
                                            SRT = materialStorageRetrieveHistory.F66_StrRtrType,
                                            From = materialStorageRetrieveHistory.F66_From,
                                            To = materialStorageRetrieveHistory.F66_To,
                                            SID = materialStorageRetrieveHistory.F66_CmdSeqNo,
                                            CmdID = materialStorageRetrieveHistory.F66_CommandNo,
                                            CmdType = materialStorageRetrieveHistory.F66_CommandType,
                                            PalletNo = materialStorageRetrieveHistory.F66_PalletNo,
                                            TerminalNo = materialStorageRetrieveHistory.F66_TerminalNo,
                                            PictureNo = materialStorageRetrieveHistory.F66_PictureNo,
                                            AbnormalCode = materialStorageRetrieveHistory.F66_AbnormalCode,
                                        };




            //•	o	Show “PAGE: “ + current page with format as ###            
            printManagementReportItem.Page = "1";

            // Get Total quantity
            printManagementReportItem.Total = 0.ToString("###,###,###,##0.00");

            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printManagementReportItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printManagementReportItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printManagementReportItem.FindPrintManagementReportItem = await managementReportItems.ToListAsync();

            printManagementReportItem.FindPrintManagementReportItem = printManagementReportItem.FindPrintManagementReportItem.Select(m => new FindPrintManagementReportItem()
            {
                SDate = m.SDate1.HasValue? m.SDate1.Value.ToString("dd/MM/yyyy HH:mm") : "",
                SRT = m.SRT,
                From = m.From,
                To = m.To,
                SID = m.SID,
                CmdID = m.CmdID,
                CmdType = m.CmdType,
                PalletNo = m.PalletNo,
                TerminalNo = m.TerminalNo,
                PictureNo = m.PictureNo,
                AbnormalCode = m.AbnormalCode,
            }).ToList();
            return printManagementReportItem;
        }
        public async Task<PrintManagementReportItem> SearchMaterialMovementRecord(string from, string to)
        {
            if (string.IsNullOrEmpty(from))
            {
                from = "01/01/0001";
            }
            if (string.IsNullOrEmpty(to))
            {
                to = "30/12/9999";
            }

            DateTime fromdate = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todate = DateTime.ParseExact(to, "dd/MM/yyyy", CultureInfo.InvariantCulture);


            // Find all tx60
            var materialStorageRetrieveHistorys = _unitOfWork.MaterialWarehouseCommandHistoryRepository.GetAll().Where(i => i.F60_AddDate >= fromdate && i.F60_AddDate <= todate); ;


            var printManagementReportItem = new PrintManagementReportItem();


            // Find Management Report item.
            var managementReportItems = from materialStorageRetrieveHistory in materialStorageRetrieveHistorys
                                        orderby new
                                        {
                                            materialStorageRetrieveHistory.F60_AddDate,
                                            materialStorageRetrieveHistory.F60_CmdSeqNo

                                        }
                                        select new FindPrintManagementReportItem()
                                        {
                                            SDate1 = materialStorageRetrieveHistory.F60_CommandEndDate,
                                            SRT = materialStorageRetrieveHistory.F60_StrRtrType,
                                            From = materialStorageRetrieveHistory.F60_From,
                                            To = materialStorageRetrieveHistory.F60_To,
                                            SID = materialStorageRetrieveHistory.F60_CmdSeqNo,
                                            CmdID = materialStorageRetrieveHistory.F60_CommandNo,
                                            CmdType = materialStorageRetrieveHistory.F60_CommandType,
                                            PalletNo = materialStorageRetrieveHistory.F60_PalletNo,
                                            TerminalNo = materialStorageRetrieveHistory.F60_TerminalNo,
                                            PictureNo = materialStorageRetrieveHistory.F60_PictureNo,
                                            AbnormalCode = materialStorageRetrieveHistory.F60_AbnormalCode,
                                        };




            //•	o	Show “PAGE: “ + current page with format as ###            
            printManagementReportItem.Page = "1";

            // Get Total quantity
            printManagementReportItem.Total = 0.ToString("###,###,###,##0.00");

            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printManagementReportItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printManagementReportItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printManagementReportItem.FindPrintManagementReportItem = await managementReportItems.ToListAsync();

            printManagementReportItem.FindPrintManagementReportItem = printManagementReportItem.FindPrintManagementReportItem.Select(m => new FindPrintManagementReportItem()
            {
                SDate = m.SDate1.HasValue ? m.SDate1.Value.ToString("dd/MM/yyyy HH:mm") : "",
                SRT = m.SRT,
                From = m.From,
                To = m.To,
                SID = m.SID,
                CmdID = m.CmdID,
                CmdType = m.CmdType,
                PalletNo = m.PalletNo,
                TerminalNo = m.TerminalNo,
                PictureNo = m.PictureNo,
                AbnormalCode = m.AbnormalCode,
            }).ToList();
            return printManagementReportItem;
        }

        public async Task<PrintManagementReportItem> SearchPreProductMovementRecord(string from, string to)
        {
            if (string.IsNullOrEmpty(from))
            {
                from = "01/01/0001";
            }
            if (string.IsNullOrEmpty(to))
            {
                to = "30/12/9999";
            }

            DateTime fromdate = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todate = DateTime.ParseExact(to, "dd/MM/yyyy", CultureInfo.InvariantCulture);


            // Find all tx63
            var materialStorageRetrieveHistorys = _unitOfWork.PreProductWarehouseCommandHistoryRepository.GetAll().Where(i => i.F63_AddDate >= fromdate && i.F63_AddDate <= todate); ;


            var printManagementReportItem = new PrintManagementReportItem();


            // Find Management Report item.
            var managementReportItems = from materialStorageRetrieveHistory in materialStorageRetrieveHistorys
                                        orderby new
                                        {
                                            materialStorageRetrieveHistory.F63_AddDate,
                                            materialStorageRetrieveHistory.F63_CmdSeqNo

                                        }
                                        select new FindPrintManagementReportItem()
                                        {
                                            SDate1 = materialStorageRetrieveHistory.F63_CommandEndDate,
                                            SRT = materialStorageRetrieveHistory.F63_StrRtrType,
                                            From = materialStorageRetrieveHistory.F63_From,
                                            To = materialStorageRetrieveHistory.F63_To,
                                            SID = materialStorageRetrieveHistory.F63_CmdSeqNo,
                                            CmdID = materialStorageRetrieveHistory.F63_CommandNo,
                                            CmdType = materialStorageRetrieveHistory.F63_CommandType,
                                            ContainerNo = materialStorageRetrieveHistory.F63_ContainerNo,
                                            TerminalNo = materialStorageRetrieveHistory.F63_TerminalNo,
                                            PictureNo = materialStorageRetrieveHistory.F63_PictureNo,
                                            AbnormalCode = materialStorageRetrieveHistory.F63_AbnormalCode,
                                        };




            //•	o	Show “PAGE: “ + current page with format as ###            
            printManagementReportItem.Page = "1";

            // Get Total quantity
            printManagementReportItem.Total = "";

            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printManagementReportItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printManagementReportItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printManagementReportItem.FindPrintManagementReportItem = await managementReportItems.ToListAsync();

            printManagementReportItem.FindPrintManagementReportItem = printManagementReportItem.FindPrintManagementReportItem.Select(m => new FindPrintManagementReportItem()
            {
                SDate = m.SDate1.HasValue ? m.SDate1.Value.ToString("dd/MM/yyyy HH:mm") : "",
                SRT = m.SRT,
                From = m.From,
                To = m.To,
                ContainerNo = m.ContainerNo,
                SID = m.SID,
                CmdID = m.CmdID,
                CmdType = m.CmdType,
                PalletNo = m.PalletNo,
                TerminalNo = m.TerminalNo,
                PictureNo = m.PictureNo,
                AbnormalCode = m.AbnormalCode,
            }).ToList();
            return printManagementReportItem;
        }

        public async Task<PrintManagementReportItem> SearchProductMovementRecord(string from, string to)
        {
            if (string.IsNullOrEmpty(from))
            {
                from = "01/01/0001";
            }
            if (string.IsNullOrEmpty(to))
            {
                to = "30/12/9999";
            }

            DateTime fromdate = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todate = DateTime.ParseExact(to, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            // Find all tx09
            var materials = _unitOfWork.ProductRepository.GetAll();

            // Find all tx65
            var materialStorageRetrieveHistorys =
                _unitOfWork.ProductStorageRetrieveHistoryRepository.GetAll()
                    .Where(i => i.F65_StgRtrDate >= fromdate && i.F65_StgRtrDate <= todate);


            var printManagementReportItem = new PrintManagementReportItem();


            // Find Management Report item.
            var managementReportItems = from materialStorageRetrieveHistory in materialStorageRetrieveHistorys
                                        join material in materials on materialStorageRetrieveHistory.F65_ProductCode equals material.F09_ProductCode

                                        orderby new
                                        {
                                            materialStorageRetrieveHistory.F65_StgRtrDate,
                                            materialStorageRetrieveHistory.F65_PalletNo,
                                            materialStorageRetrieveHistory.F65_ProductCode,
                                            materialStorageRetrieveHistory.F65_ProductLotNo

                                        }
                                        select new FindPrintManagementReportItem()
                                        {
                                            SDate1 = materialStorageRetrieveHistory.F65_StgRtrDate,
                                            SRF = materialStorageRetrieveHistory.F65_StgRtrCls,
                                            From = materialStorageRetrieveHistory.F65_From,
                                            To = materialStorageRetrieveHistory.F65_To,
                                            PalletNo = materialStorageRetrieveHistory.F65_PalletNo,
                                            ProductCode = materialStorageRetrieveHistory.F65_ProductCode,
                                            ProductName = material.F09_ProductDesp,
                                            LotNo = materialStorageRetrieveHistory.F65_ProductLotNo,
                                            PU = material.F09_PackingUnit,
                                            Quantity = materialStorageRetrieveHistory.F65_Amount
                                        };

            var firstResult = from managementReportItem in managementReportItems
                              group managementReportItems by new
                              {
                                  managementReportItem.ProductCode,
                                  managementReportItem.Quantity,
                              }
                                  into firstGroup
                              select firstGroup;

            var totalFirstItems = (await firstResult.SumAsync(x => (double?)x.Key.Quantity)) ?? 0;


            //•	o	Show “PAGE: “ + current page with format as ###            
            printManagementReportItem.Page = "1";

            // Get Total quantity
            printManagementReportItem.Total = totalFirstItems.ToString("###,###,###,##0.00");

            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printManagementReportItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printManagementReportItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printManagementReportItem.FindPrintManagementReportItem = await managementReportItems.ToListAsync();

            var groupby = printManagementReportItem.FindPrintManagementReportItem.Select(m => new FindPrintManagementReportItem()
            {
                SDate = m.SDate1.HasValue ? m.SDate1.Value.ToString("dd/MM/yyyy") : "",
                Stime = m.SDate1.HasValue ? m.SDate1.Value.ToString("HH:mm") : "",
                SRF = m.SRF,
                Conveyor = m.SRF.Trim().Equals("1") ? m.To : m.From,
                RBL = m.SRF.Trim().Equals("1") ? m.From.Substring(0, 2) + "-" + m.From.Substring(2, 2) + "-" + m.From.Substring(4, 2) : (m.To.Substring(0, 2) + "-" + m.To.Substring(2, 2) + "-" + m.To.Substring(4, 2)),
                PalletNo = m.PalletNo,
                ProductCode = m.ProductCode,
                ProductName = m.ProductName,
                LotNo = m.LotNo,
                PUstring = m.PU.ToString("###,###,###,##0.00"),
                Quantity = m.Quantity,
                Quantityst = m.Quantity.ToString("###,###,###,##0.00")
            }).ToList().GroupBy(m => new { m.SDate,m.PalletNo});

            var groupitems = new List<GroupItems>();
            foreach (var items in groupby)
            {
                var groupitem = new GroupItems();
                groupitem.FindPrintManagementReportItem = items.ToList();
                groupitem.Totalgroup = groupitem.FindPrintManagementReportItem.Sum(i => i.Quantity).ToString("###,###,###,##0.00");
                groupitems.Add(groupitem);
            }
            printManagementReportItem.GroupItemses = groupitems;
            return printManagementReportItem;
        }
        public async Task<PrintManagementReportItem> SearchProductShippingRecord(string from, string to)
        {
            if (string.IsNullOrEmpty(from))
            {
                from = "01/01/0001";
            }
            if (string.IsNullOrEmpty(to))
            {
                to = "30/12/9999";
            }

            DateTime fromdate = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todate = DateTime.ParseExact(to, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            // Find all tx09
            var products = _unitOfWork.ProductRepository.GetAll();
            // Find all tx10
            var endUsers = _unitOfWork.EndUserRepository.GetAll();
            // Find all tx44
            var shippingPlans = _unitOfWork.ShippingPlanRepository.GetAll().Where(i => i.F44_DeliveryDate >= fromdate && i.F44_DeliveryDate <= todate);
            // Find all tx45
            var shipCommands = _unitOfWork.ShipCommandRepository.GetAll();

           
            var printManagementReportItem = new PrintManagementReportItem();


            // Find Management Report item.
            var managementReportItems = from shippingPlan in shippingPlans
                                        join endUser in endUsers on shippingPlan.F44_EndUserCode equals endUser.F10_EndUserCode
                                        join shipCommand in shipCommands on shippingPlan.F44_ShipCommandNo equals shipCommand.F45_ShipCommandNo
                                        join product in products on shippingPlan.F44_ProductCode equals product.F09_ProductCode
                                        orderby new
                                        {
                                            shipCommand.F45_ShipCommandNo,
                                            shippingPlan.F44_ProductLotNo

                                        }
                                        select new FindPrintManagementReportItem()
                                        {
                                            SDate1 = shippingPlan.F44_DeliveryDate,
                                            SDate2 = shipCommand.F45_ShipDate,
                                            ShippingNo = shipCommand.F45_ShipCommandNo,
                                            ProductCode = shippingPlan.F44_ProductCode,
                                            ProductName = product.F09_ProductDesp,
                                            LotNo = shippingPlan.F44_ProductLotNo,
                                            EndUserCode = endUser.F10_EndUserCode,
                                            EndUserName = endUser.F10_EndUserName,
                                            Quantity = shipCommand.F45_ShippedAmount
                                        };

            var firstResult = from managementReportItem in managementReportItems
                              group managementReportItems by new
                              {
                                  managementReportItem.ProductCode,
                                  managementReportItem.Quantity,
                              }
                                  into firstGroup
                              select firstGroup;

            var totalFirstItems = (await firstResult.SumAsync(x => (double?)x.Key.Quantity)) ?? 0;


            //•	o	Show “PAGE: “ + current page with format as ###            
            printManagementReportItem.Page = "1";

            // Get Total quantity
            printManagementReportItem.Total = totalFirstItems.ToString("###,###,###,##0.00");

            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printManagementReportItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printManagementReportItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printManagementReportItem.FindPrintManagementReportItem = await managementReportItems.ToListAsync();

            var groupby = printManagementReportItem.FindPrintManagementReportItem.Select(m => new FindPrintManagementReportItem()
            {
                Delivery = m.SDate1.HasValue ? m.SDate1.Value.ToString("dd/MM/yyyy") : "",
                RetrievalDate = m.SDate2.HasValue ? m.SDate2.Value.ToString("dd/MM/yyyy") : "",
                EndUserName = m.EndUserName,
                EndUserCode = m.EndUserCode,
                ShippingNo = m.ShippingNo,
                ProductCode = m.ProductCode,
                ProductName = m.ProductName,
                LotNo = m.LotNo,
                Quantity = m.Quantity,
                Quantityst = m.Quantity.ToString("###,###,###,##0.00"),
            }).ToList().GroupBy(m => new { m.ShippingNo, m.ProductCode,m.ProductName,m.Delivery,m.EndUserCode,m.EndUserName,m.RetrievalDate});
            var groupitems = new List<GroupItems>();
            foreach (var items in groupby)
            {
                var groupitem = new GroupItems();
                groupitem.FindPrintManagementReportItem = items.ToList();
                groupitem.Totalgroup = groupitem.FindPrintManagementReportItem.Sum(i => i.Quantity).ToString("###,###,###,##0.00");
                groupitems.Add(groupitem);
            }
            printManagementReportItem.GroupItemses = groupitems;
            return printManagementReportItem;
        }

        public async Task<PrintManagementReportItem> SearchProductCertificationRecord(string from, string to)
        {
            if (string.IsNullOrEmpty(from))
            {
                from = "01/01/0001";
            }
            if (string.IsNullOrEmpty(to))
            {
                to = "30/12/9999";
            }

            DateTime fromdate = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todate = DateTime.ParseExact(to, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            // Find all tx09
            var products = _unitOfWork.ProductRepository.GetAll();
            // Find all tx67
            var CertificateHistorys = _unitOfWork.CertificateHistoryRepository.GetAll().Where(i => i.F67_CertificationDate >= fromdate && i.F67_CertificationDate <= todate && !(i.F67_ProductFlg.Equals("0")) && i.F67_CertificationFlag.Equals("1"));


            var printManagementReportItem = new PrintManagementReportItem();


            // Find Management Report item.
            var managementReportItems = from certificateHistory in CertificateHistorys
                join product in products on certificateHistory.F67_ProductCode.Trim() equals product.F09_ProductCode.Trim()
                orderby new
                    {
                        certificateHistory.F67_CertificationDate,
                        certificateHistory.F67_ProductCode,
                        certificateHistory.F67_ProductLotNo

                    }
                    select new FindPrintManagementReportItem()
                    {
                        SDate1 = certificateHistory.F67_CertificationDate,
                        ProductCode = certificateHistory.F67_ProductCode,
                        PreProductCode = product.F09_PreProductCode,
                        ProductName = product.F09_ProductDesp,
                        LotNo = certificateHistory.F67_ProductLotNo,
                        Quantity = certificateHistory.F67_Amount
                    };




            //•	o	Show “PAGE: “ + current page with format as ###            
            printManagementReportItem.Page = "1";

            // Get Total quantity
            printManagementReportItem.Total = "";

            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printManagementReportItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printManagementReportItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printManagementReportItem.FindPrintManagementReportItem = await managementReportItems.ToListAsync();

            printManagementReportItem.FindPrintManagementReportItem = printManagementReportItem.FindPrintManagementReportItem.Select(m => new FindPrintManagementReportItem()
            {
                SDate = m.SDate1.HasValue ? m.SDate1.Value.ToString("dd/MM/yyyy") : "",
                PreProductCode = m.PreProductCode,
                ProductCode = m.ProductCode,
                ProductName = m.ProductName,
                LotNo = m.LotNo,
                Quantityst = m.Quantity.ToString("###,###,###,##0.00")
            }).ToList();

            return printManagementReportItem;
        }
        public async Task<PrintManagementReportItem> SearchMaterialRetrievalRecord(string from, string to)
        {
            if (string.IsNullOrEmpty(from))
            {
                from = "01/01/0001";
            }
            if (string.IsNullOrEmpty(to))
            {
                to = "30/12/9999";
            }

            DateTime fromdate = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todate = DateTime.ParseExact(to, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            // Find all tx01
            var materials = _unitOfWork.MaterialRepository.GetAll();
            // Find all tx02
            var prePdtMkps = _unitOfWork.PrePdtMkpRepository.GetAll();
            // Find all tx03
            var preProducts = _unitOfWork.PreProductRepository.GetAll();
            // Find all tx39
            var pdtPlns = _unitOfWork.PdtPlnRepository.GetAll();
            // Find all tx42
            var kneadingCommands = _unitOfWork.KneadingCommandRepository.GetAll();
            // Find all tx43
            var kneadingRecords = _unitOfWork.KneadingRecordRepository.GetAll().Where(i => i.F43_AddDate >= fromdate && i.F43_AddDate <= todate);
            


            var printManagementReportItem = new PrintManagementReportItem();


            // Find Management Report item.
            var managementReportItems = from kneadingCommand in kneadingCommands
                join kneadingRecord in kneadingRecords on
                new {prelot = kneadingCommand.F42_PrePdtLotNo, kcmdno = kneadingCommand.F42_KndCmdNo} equals
                new {prelot = kneadingRecord.F43_PrePdtLotNo, kcmdno = kneadingRecord.F43_KndCmdNo}
                join pdtPln in pdtPlns on
                new {precode = kneadingCommand.F42_PreProductCode, kcmdno = kneadingCommand.F42_KndCmdNo} equals
                new {precode = pdtPln.F39_PreProductCode, kcmdno = pdtPln.F39_KndCmdNo}
                join preProduct in preProducts on kneadingCommand.F42_PreProductCode equals
                preProduct.F03_PreProductCode
                join prePdtMkp in prePdtMkps on
                new {precode = kneadingCommand.F42_PreProductCode, matcode = kneadingRecord.F43_MaterialCode} equals
                new {precode = prePdtMkp.F02_PreProductCode, matcode = prePdtMkp.F02_MaterialCode}
                join material in materials on prePdtMkp.F02_MaterialCode equals material.F01_MaterialCode
                orderby new
                {
                    kneadingRecord.F43_KndCmdNo,
                    kneadingRecord.F43_PrePdtLotNo,
                    kneadingRecord.F43_MaterialCode,
                    kneadingRecord.F43_MaterialLotNo,
                    kneadingRecord.F43_BatchSeqNo

                }
                select new FindPrintManagementReportItem
                {
                    CmdNo = kneadingCommand.F42_KndCmdNo,
                    ProductionLine = pdtPln.F39_KneadingLine,
                    PreProductCode = kneadingCommand.F42_PreProductCode,
                    PreProductName = preProduct.F03_PreProductName,
                    LotNo = kneadingCommand.F42_PrePdtLotNo,
                    SDate1 = kneadingCommand.F42_KndBgnDate,
                    SDate2 = kneadingCommand.F42_KndEndDate,
                    MaterialCode = kneadingRecord.F43_MaterialCode,
                    MaterialName = material.F01_MaterialDsp,
                    MaterialLotNo = kneadingRecord.F43_MaterialLotNo,
                    Schagre = prePdtMkp.F02_3FLayinAmount + prePdtMkp.F02_4FLayinAmount,
                    BatchSeqNo = kneadingRecord.F43_BatchSeqNo,
                    LayingAmuont = kneadingRecord.F43_LayinginAmount,
                    BatchLot = preProduct.F03_BatchLot,
                };
            //var grouping = from managementReportItem in managementReportItems
            //    group managementReportItems by new
            //    {
            //        managementReportItem.CmdNo,
            //        managementReportItem.LotNo,
            //    };
            // new list printitem
            var groupitem = new List<FindPrintManagementReportItem>();
            var resultlist = managementReportItems.ToList().GroupBy(m => new { m.CmdNo , m.LotNo , m.ProductionLine } );
            foreach (var kcmd in resultlist)
            {
                var printitem = new FindPrintManagementReportItem();
                printitem.CmdNo = kcmd.First().CmdNo;
                printitem.ProductionLine = kcmd.First().ProductionLine;
                printitem.PreProductCode = kcmd.First().PreProductCode;
                printitem.PreProductName = kcmd.First().PreProductName;
                printitem.LotNo = kcmd.First().LotNo;
                printitem.SDate1st = kcmd.First().SDate1.HasValue ? kcmd.First().SDate1.Value.ToString("dd/MM/yyyy HH:mm") : "";
                printitem.SDate2st = kcmd.First().SDate2.HasValue ? kcmd.First().SDate2.Value.ToString("dd/MM/yyyy HH:mm") : "";
                printitem.BatchLot = kcmd.First().BatchLot;
                var groupitematt = new List<FindPrintRetrievalItem>();
                int no = 0;
                double totalgroup = 0;
                double totalschagre = 0;
                foreach (var item in kcmd)
                {
                    no++;
                    var printitematt = new FindPrintRetrievalItem();
                    printitematt.No = no;
                    printitematt.MaterialCode = item.MaterialCode;
                    printitematt.MaterialName = item.MaterialName;
                    printitematt.MaterialLotNo = item.MaterialLotNo;
                    printitematt.Scharged = item.Schagre.ToString("###,###,###,##0.00");
                    printitematt.Charge1 = item.BatchSeqNo == 1 ? item.LayingAmuont.Value.ToString("###,###,###,##0.00") : "";
                    printitematt.Chargev1 = item.BatchSeqNo == 1 ? "v" : "";
                    printitematt.Charge2 = item.BatchSeqNo == 2 ? item.LayingAmuont.Value.ToString("###,###,###,##0.00") : "";
                    printitematt.Chargev2 = item.BatchSeqNo == 2 ? "v" : "";
                    printitematt.Charge3 = item.BatchSeqNo == 3 ? item.LayingAmuont.Value.ToString("###,###,###,##0.00") : "";
                    printitematt.Chargev3 = item.BatchSeqNo == 3 ? "v" : "";
                    printitematt.Charge4 = item.BatchSeqNo == 4 ? item.LayingAmuont.Value.ToString("###,###,###,##0.00") : "";
                    printitematt.Chargev4 = item.BatchSeqNo == 4 ? "v" : "";
                    printitematt.Charge5 = item.BatchSeqNo == 5 ? item.LayingAmuont.Value.ToString("###,###,###,##0.00") : "";
                    printitematt.Chargev5 = item.BatchSeqNo == 5 ? "v" : "";
                    printitematt.Total = item.LayingAmuont.Value.ToString("###,###,###,##0.00");
                    totalgroup = totalgroup + (item.LayingAmuont.HasValue ? item.LayingAmuont.Value : 0);
                    totalschagre = totalschagre + item.Schagre;
                    groupitematt.Add(printitematt);
                }
                printitem.TotalGroupst = totalgroup.ToString("###,###,###,##0.00");
                printitem.TotalSchagre = totalschagre.ToString("###,###,###,##0.00");
                printitem.Totalst = (totalschagre*kcmd.First().BatchLot).ToString("###,###,###,##0.00");
                if (string.IsNullOrEmpty(printitem.Totalst))
                {
                    printitem.Totalst = "0";
                }
                if (string.IsNullOrEmpty(printitem.TotalSchagre))
                {
                    printitem.TotalSchagre = "0";
                }
                printitem.FindPrintRetrievalItem = groupitematt;

                groupitem.Add(printitem);
            }

            //var firstResult = from managementReportItem in managementReportItems
            //                  group managementReportItems by new
            //                  {
            //                      managementReportItem.ProductCode,
            //                      managementReportItem.Quantity,
            //                  }
            //                      into firstGroup
            //                  select firstGroup;

            //var totalFirstItems = (await firstResult.SumAsync(x => (double?)x.Key.Quantity)) ?? 0;


            //•	o	Show “PAGE: “ + current page with format as ###            
            printManagementReportItem.Page = "1";

            // Get Total quantity
            //printManagementReportItem.Total = totalFirstItems;

            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printManagementReportItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printManagementReportItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            //printManagementReportItem.FindPrintManagementReportItem = await managementReportItems.ToListAsync();

            //printManagementReportItem.FindPrintManagementReportItem = printManagementReportItem.FindPrintManagementReportItem.Select(m => new FindPrintManagementReportItem()
            //{
            //    Delivery = m.SDate1.HasValue ? m.SDate1.Value.ToString("d") : "",
            //    RetrievalDate = m.SDate1.HasValue ? m.SDate1.Value.ToString("d") : "",
            //    EndUserName = m.EndUserName,
            //    EndUserCode = m.EndUserCode,
            //    ShippingNo = m.ShippingNo,
            //    ProductCode = m.ProductCode,
            //    ProductName = m.ProductName,
            //    LotNo = m.LotNo,
            //    Quantity = m.Quantity
            //}).ToList();
            printManagementReportItem.FindPrintManagementReportItem = groupitem;
            return printManagementReportItem;
        }

        public async Task<PrintManagementReportItem> SearchPreProductRetrievalRecord(string from, string to)
        {
            if (string.IsNullOrEmpty(from))
            {
                from = "01/01/0001";
            }
            if (string.IsNullOrEmpty(to))
            {
                to = "30/12/9999";
            }

            DateTime fromdate = DateTime.ParseExact(from, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime todate = DateTime.ParseExact(to, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            // Find all tx09
            var products = _unitOfWork.ProductRepository.GetAll();
            // Find all tx03
            var preProducts = _unitOfWork.PreProductRepository.GetAll();
            // Find all tx41
            var tabletCommands = _unitOfWork.TabletCommandRepository.GetAll().Where(p => p.F41_Status.Equals("7"));
            // Find all tx42
            var kneadingCommands = _unitOfWork.KneadingCommandRepository.GetAll();
            // Find all tx56
            var tabletProducts =
                _unitOfWork.TabletProductRepository.GetAll()
                    .Where(i => i.F56_AddDate >= fromdate && i.F56_AddDate <= todate);



            var printManagementReportItem = new PrintManagementReportItem();


            // Find Management Report item.
            var managementReportItemsst = from tabletCommand in tabletCommands
                                        join kneadingCommand in kneadingCommands on new { preprolot = tabletCommand.F41_PrePdtLotNo, kcmdno = tabletCommand.F41_KndCmdNo } equals new { preprolot = kneadingCommand.F42_PrePdtLotNo, kcmdno = kneadingCommand.F42_KndCmdNo }
                                        join tabletProduct in tabletProducts on new { preprolot = tabletCommand.F41_PrePdtLotNo, kcmdno = tabletCommand.F41_KndCmdNo } equals new { preprolot = tabletProduct.F56_PrePdtLotNo, kcmdno = tabletProduct.F56_KndCmdNo }
                                        join preProduct in preProducts on tabletCommand.F41_PreproductCode equals preProduct.F03_PreProductCode
                                        join product in products on tabletProduct.F56_ProductCode equals product.F09_ProductCode
                                        orderby new
                                        {
                                            tabletProduct.F56_ProductCode

                                        }
                                        select new FindPrintManagementReportItem()
                                        {
                                            PreProductCode = tabletCommand.F41_PreproductCode,
                                            PreProductName = preProduct.F03_PreProductName,
                                            LotNo = tabletCommand.F41_PrePdtLotNo,
                                            ReceiveQuantity = kneadingCommand.F42_ThrowAmount,
                                            ProductName = product.F09_ProductDesp,
                                            ProductCode = tabletProduct.F56_ProductCode,
                                            LotNochild = tabletProduct.F56_ProductLotNo,
                                            ValidQuantity = tabletProduct.F56_TbtCmdEndAmt,
                                            Lose = tabletProduct.F56_TbtCmdEndAmt - tabletProduct.F56_TbtCmdAmt,
                                            Fraction = tabletProduct.F56_TbtCmdEndFrtAmt
                                        };

            var groupitem = new List<FindPrintManagementReportItem>();
            var managementReportItems = managementReportItemsst.ToList().Select(m => new FindPrintManagementReportItem()
            {
                PreProductCode = m.PreProductCode,
                PreProductName = m.PreProductName,
                LotNo = m.LotNo,
                ReceiveQuantityst = m.ReceiveQuantity.ToString("###,###,###,##0.00"),
                ProductName = m.ProductName,
                ProductCode = m.ProductCode,
                LotNochild = m.LotNochild,
                ValidQuantity = m.ValidQuantity,
                Lose = m.Lose,
                Fraction = m.Fraction
            });
            var resultlist = managementReportItems.ToList().GroupBy(m => new { m.CmdNo, m.LotNo });
            foreach (var kcmd in resultlist)
            {
                var printitem = new FindPrintManagementReportItem();
                printitem.PreProductCode = kcmd.First().PreProductCode;
                printitem.PreProductName = kcmd.First().PreProductName;
                printitem.LotNo = kcmd.First().LotNo;
                printitem.ReceiveQuantityst = kcmd.First().ReceiveQuantityst;
                printitem.ValidQuantity1 = kcmd.First().ValidQuantity < 0.005 ? "" : kcmd.First().ValidQuantity.ToString("###,###,###,##0.00");
                int i = 0;
                foreach (var itemprint in kcmd)
                {
                    i++;
                    if (i <= 5)
                    {
                        if (i == 1)
                        {
                            printitem.r11 = itemprint.ProductCode;
                            printitem.r12 = itemprint.ProductName;
                            printitem.r13 = itemprint.LotNochild;
                            printitem.r14 = itemprint.ValidQuantity < 0.005 ? "" : itemprint.ValidQuantity.ToString("###,###,###,##0.00");
                            printitem.r15 = itemprint.Lose < 0.005 ? "" : itemprint.Lose.ToString("###,###,###,##0.00");
                            printitem.r16 = itemprint.Fraction < 0.005 ? "" : itemprint.Fraction.ToString("###,###,###,##0.00");
                        }
                        if (i == 2)
                        {
                            printitem.r21 = itemprint.ProductCode;
                            printitem.r22 = itemprint.ProductName;
                            printitem.r23 = itemprint.LotNochild;
                            printitem.r24 = itemprint.ValidQuantity < 0.005 ? "" : itemprint.ValidQuantity.ToString("###,###,###,##0.00");
                            printitem.r25 = itemprint.Lose < 0.005 ? "" : itemprint.Lose.ToString("###,###,###,##0.00");
                            printitem.r26 = itemprint.Fraction < 0.005 ? "" : itemprint.Fraction.ToString("###,###,###,##0.00");
                        }
                        if (i == 3)
                        {
                            printitem.r31 = itemprint.ProductCode;
                            printitem.r32 = itemprint.ProductName;
                            printitem.r33 = itemprint.LotNochild;
                            printitem.r34 = itemprint.ValidQuantity < 0.005 ? "" : itemprint.ValidQuantity.ToString("###,###,###,##0.00");
                            printitem.r35 = itemprint.Lose < 0.005 ? "" : itemprint.Lose.ToString("###,###,###,##0.00");
                            printitem.r36 = itemprint.Fraction < 0.005 ? "" : itemprint.Fraction.ToString("###,###,###,##0.00");
                        }
                        if (i == 4)
                        {
                            printitem.r41 = itemprint.ProductCode;
                            printitem.r42 = itemprint.ProductName;
                            printitem.r43 = itemprint.LotNochild;
                            printitem.r44 = itemprint.ValidQuantity < 0.005 ? "" : itemprint.ValidQuantity.ToString("###,###,###,##0.00");
                            printitem.r45 = itemprint.Lose < 0.005 ? "" : itemprint.Lose.ToString("###,###,###,##0.00");
                            printitem.r46 = itemprint.Fraction < 0.005 ? "" : itemprint.Fraction.ToString("###,###,###,##0.00");
                        }
                        if (i == 5)
                        {
                            printitem.r51 = itemprint.ProductCode;
                            printitem.r52 = itemprint.ProductName;
                            printitem.r53 = itemprint.LotNochild;
                            printitem.r54 = itemprint.ValidQuantity < 0.005 ? "" : itemprint.ValidQuantity.ToString("###,###,###,##0.00");
                            printitem.r55 = itemprint.Lose < 0.005 ? "" : itemprint.Lose.ToString("###,###,###,##0.00");
                            printitem.r56 = itemprint.Fraction < 0.005 ? "" : itemprint.Fraction.ToString("###,###,###,##0.00");
                        }
                    }
                    

                }
                

                groupitem.Add(printitem);
            }


            var totalFirstItems =  0;


            //•	o	Show “PAGE: “ + current page with format as ###            
            printManagementReportItem.Page = "1";

            // Get Total quantity
            printManagementReportItem.Total = totalFirstItems.ToString("###,###,###,##0.00");

            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printManagementReportItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printManagementReportItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printManagementReportItem.FindPrintManagementReportItem = groupitem;
            return printManagementReportItem;
        }

        //section for update inline case

        #region Material Consumer Received


        public bool CheckConsumerMaterials(string yearmonth)
        {
            DateTime fromdate = DateTime.ParseExact("01/" + yearmonth, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime nextmonth = fromdate.AddMonths(1);
            // Find item 91
            var materialTotals = _unitOfWork.MaterialTotalRepository.GetAll();

            // Find item 01
            var materials = _unitOfWork.MaterialRepository.GetAll();
            // Find item 30
            var receptions = _unitOfWork.ReceptionRepository.GetAll().Where(m => m.F30_AcceptClass.Equals("1") && m.F30_UpdateDate >= fromdate && m.F30_UpdateDate < nextmonth);
            // Find item 43
            var kneadingRecords = _unitOfWork.KneadingRecordRepository.GetAll().Where(m => m.F43_UpdateDate >= fromdate && m.F43_UpdateDate < nextmonth);


            var consumerMaterialsResult = from material in materials
                                          join materialTotal in materialTotals on material.F01_MaterialCode equals materialTotal.F91_MaterialCode
                                          where materialTotal.F91_YearMonth >= fromdate && materialTotal.F91_YearMonth < nextmonth

                                          select new SubReceivedConsumedMaterialItem
                                          {
                                              MaterialCode = materialTotal.F91_MaterialCode,
                                              MaterialName = material.F01_MaterialDsp,
                                              Recieved = materialTotal.F91_Received,
                                              Remain = materialTotal.F91_PrevRemainder,
                                              Remaincurr = (materialTotal.F91_Received + materialTotal.F91_PrevRemainder - materialTotal.F91_Used),
                                              Used = materialTotal.F91_Used,
                                              Updatedate = materialTotal.F91_UpdateDate,
                                          };

            if (consumerMaterialsResult.Count() == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<ResponseResult<GridResponse<object>>> LoadConsumerMaterials(string yearmonth, int page)
        {
            var pageIndex = 0;
            if (page > 0)
            {
                pageIndex = page;
            }
            DateTime fromdate = DateTime.ParseExact("01/" + yearmonth, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime nextmonth = fromdate.AddMonths(1);
            // Find item 91
            var materialTotals = _unitOfWork.MaterialTotalRepository.GetAll();

            // Find item 01
            var materials = _unitOfWork.MaterialRepository.GetAll();


            var consumerMaterialsResult = from material in materials
                                          join materialTotal in materialTotals on material.F01_MaterialCode equals materialTotal.F91_MaterialCode
                                          where materialTotal.F91_YearMonth >= fromdate && materialTotal.F91_YearMonth < nextmonth

                                          select new SubReceivedConsumedMaterialItem
                                          {
                                              MaterialCode = materialTotal.F91_MaterialCode,
                                              MaterialName = material.F01_MaterialDsp,
                                              Recieved = materialTotal.F91_Received,
                                              Remain = materialTotal.F91_PrevRemainder,
                                              Remaincurr =(materialTotal.F91_Received + materialTotal.F91_PrevRemainder - materialTotal.F91_Used),
                                              Used = materialTotal.F91_Used,
                                              //Recievedst = materialTotal.F91_Received.ToString("###,###,###,##0.00"),
                                              //Remainst = materialTotal.F91_PrevRemainder.ToString("###,###,###,##0.00"),
                                              //Usedst = materialTotal.F91_Used.ToString("###,###,###,##0.00"),
                                              //Remaincurrst = (materialTotal.F91_Received + materialTotal.F91_PrevRemainder - materialTotal.F91_Used).ToString("###,###,###,##0.00"),

                                              Updatedate = materialTotal.F91_UpdateDate,
                                              YearMonth = materialTotal.F91_YearMonth
                                          };
            var consumerMaterialsResultst = consumerMaterialsResult.ToList().Skip((pageIndex - 1) * 30).Take(30).Select(m => new SubReceivedConsumedMaterialItem
            {
                MaterialCode = m.MaterialCode,
                MaterialName = m.MaterialName,
                Recieved = Math.Round(m.Recieved, 2),
                Remain = Math.Round(m.Remain, 2),
                Remaincurr = m.Remaincurr,
                Used = Math.Round(m.Used, 2),
                Receivedst = m.Recieved.ToString("###,###,###,##0.00"),
                Remainst = m.Remain.ToString("###,###,###,##0.00"),
                Usedst = m.Used.ToString("###,###,###,##0.00"),
                Remaincurrst = m.Remaincurr.ToString("###,###,###,##0.00"),
                Updatedatest = m.Updatedate.HasValue ? m.Updatedate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                Updatedate = m.Updatedate,
                YearMonth = m.YearMonth,
                YearMonthst = m.YearMonth.HasValue ? m.YearMonth.Value.ToString("MM/yyyy") : ""
            });
            // Count the total record which matches with the conditions.
            var totalRecords = await consumerMaterialsResult.CountAsync();


            // Build a grid and respond to client.
            var resultModel = new GridResponse<object>(consumerMaterialsResultst, totalRecords);
            return new ResponseResult<GridResponse<object>>(resultModel, true);
        }

        public async Task<PrintManagementReportItem> PrintConsumerMaterials(string yearmonth)
        {
            DateTime fromdate = DateTime.ParseExact("01/" + yearmonth, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime nextmonth = fromdate.AddMonths(1);


            // Find item 91
            var materialTotals = _unitOfWork.MaterialTotalRepository.GetAll();

            // Find item 01
            var materials = _unitOfWork.MaterialRepository.GetAll();



            var printManagementReportItem = new PrintManagementReportItem();


            // Find Management Report item.
            var managementReportItems = from material in materials
                                        join materialTotal in materialTotals on material.F01_MaterialCode equals materialTotal.F91_MaterialCode
                                        where materialTotal.F91_YearMonth >= fromdate && materialTotal.F91_YearMonth < nextmonth

                                        select new FindPrintManagementReportItem()
                                        {
                                            MaterialCode = materialTotal.F91_MaterialCode,
                                            MaterialName = material.F01_MaterialDsp,
                                            Unit = "Kg",
                                            Received = materialTotal.F91_Received,
                                            Used = materialTotal.F91_Used,
                                            Remain = materialTotal.F91_PrevRemainder
                                        };




            //•	o	Show “PAGE: “ + current page with format as ###            
            printManagementReportItem.Page = "1";
            printManagementReportItem.Month = yearmonth;

            // Get Total quantity
            printManagementReportItem.Total = "";

            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printManagementReportItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printManagementReportItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printManagementReportItem.FindPrintManagementReportItem = await managementReportItems.ToListAsync();
            printManagementReportItem.FindPrintManagementReportItem = printManagementReportItem.FindPrintManagementReportItem.Select(m => new FindPrintManagementReportItem()
            {
                MaterialCode = m.MaterialCode,
                MaterialName = m.MaterialName,
                Unit = "Kg",
                Receivedst = m.Received.ToString("###,###,###,##0.00"),
                Usedst = m.Used.ToString("###,###,###,##0.00"),
                Remainst = m.Remain.ToString("###,###,###,##0.00"),
                Remaincurrst = (m.Received + m.Remain - m.Used).ToString("###,###,###,##0.00")
            }).ToList();
            return printManagementReportItem;
        }

        public async Task<ResponseResult<GridResponse<object>>> Recalculate(string yearmonth,string matcode)
        {
            DateTime fromdate = DateTime.ParseExact("01/" + yearmonth, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime nextmonth = fromdate.AddMonths(1);
            // Find item 91
            var materialTotals = _unitOfWork.MaterialTotalRepository.GetAll();

            // Find item 01
            var materials = _unitOfWork.MaterialRepository.GetAll();
            // Find item 30
            var receptions = _unitOfWork.ReceptionRepository.GetAll().Where(m => m.F30_AcceptClass.Equals("1") && m.F30_UpdateDate >= fromdate && m.F30_UpdateDate < nextmonth);
            // Find item 43
            var kneadingRecords = _unitOfWork.KneadingRecordRepository.GetAll().Where(m => m.F43_UpdateDate >= fromdate && m.F43_UpdateDate < nextmonth);

            var listolditem = materialTotals.Where(m => m.F91_YearMonth == fromdate).Where(m => m.F91_MaterialCode.Trim().Equals(matcode.Trim())).FirstOrDefault();


            //// check item to add or update to tx91
            //var listaddingitem = from material in materials
            //                     join reception in receptions on material.F01_MaterialCode equals reception.F30_MaterialCode
            //                     join kneadingRecord in kneadingRecords on material.F01_MaterialCode equals kneadingRecord.F43_MaterialCode
            //                     where true
            //                     select new TX91MaterialTotal
            //                     {
            //                         F91_MaterialCode = material.F01_MaterialCode,
            //                         F91_YearMonth = fromdate,
            //                         F91_AddDate = DateTime.Now,
            //                         F91_UpdateDate = DateTime.Now
            //                     };
            //// add item for tx 91
            //var addingitems = listaddingitem.ToList();
            if (matcode == null)
            {
                foreach (var material in materials)
                {
                    double pre = 0;
                    double used = 0;
                    double rec = 0;
                    var oldpre = materialTotals.Where(m => m.F91_MaterialCode.Trim().Equals(material.F01_MaterialCode.Trim())).ToList().Where(m => m.F91_YearMonth == fromdate.AddMonths(-1)).FirstOrDefault();
                    if (oldpre != null)
                    {
                        pre = oldpre.F91_PrevRemainder;
                        used = oldpre.F91_Used;
                        rec = oldpre.F91_Received;
                    }
                    TX91_MaterialTotal updatetx91 = new TX91_MaterialTotal();
                    updatetx91.F91_MaterialCode = material.F01_MaterialCode;
                    updatetx91.F91_Received = TotalReceived(fromdate, material.F01_MaterialCode);
                    updatetx91.F91_Used = TotalUsed(fromdate, material.F01_MaterialCode);
                    updatetx91.F91_PrevRemainder =rec + pre -
                          used;
                    updatetx91.F91_YearMonth = fromdate;
                    updatetx91.F91_UpdateDate = DateTime.Now;
                    updatetx91.F91_AddDate = DateTime.Now;
                    //if (listolditem.Any(m => m.F91_MaterialCode.Equals(tx91MaterialTotal.F91_MaterialCode)))
                    //{
                    //    var updateitem =
                    //        listolditem.Where(m => m.F91_MaterialCode.Equals(tx91MaterialTotal.F91_MaterialCode)).First();
                    //    updateitem.F91_Received = TotalReceived(fromdate, tx91MaterialTotal.F91_MaterialCode);
                    //    updateitem.F91_Used = TotalUsed(fromdate, tx91MaterialTotal.F91_MaterialCode);
                    //    updateitem.F91_PrevRemainder = TotalReceived(fromdate, tx91MaterialTotal.F91_MaterialCode) > 0 && TotalUsed(fromdate, tx91MaterialTotal.F91_MaterialCode) > TotalUsed(fromdate, tx91MaterialTotal.F91_MaterialCode) ? TotalUsed(fromdate, tx91MaterialTotal.F91_MaterialCode) - TotalUsed(fromdate, tx91MaterialTotal.F91_MaterialCode) : 0;
                    //    updateitem.F91_UpdateDate = DateTime.Now;;
                    //    _unitOfWork.MaterialTotalRepository.Update(updateitem);
                    //}
                    //else
                    //{
                    _unitOfWork.MaterialTotalRepository.Add(updatetx91);
                    //}
                }
            }
            else
            {
                if (listolditem != null)
                {
                    listolditem.F91_Received = TotalReceived(fromdate, matcode);
                    listolditem.F91_Used = TotalUsed(fromdate, matcode);
                    //listolditem.F91_PrevRemainder = TotalReceived(fromdate, matcode) > 0 && TotalReceived(fromdate, matcode) > TotalUsed(fromdate, matcode) ? TotalReceived(fromdate, matcode) - TotalUsed(fromdate, matcode) : 0;
                    listolditem.F91_UpdateDate = DateTime.Now;
                }

            }

            _unitOfWork.Commit();

            var consumerMaterialsResult = from materialTotal in materialTotals
                                          join material in materials on materialTotal.F91_MaterialCode equals material.F01_MaterialCode
                                          where materialTotal.F91_YearMonth == fromdate

                                          select new SubReceivedConsumedMaterialItem
                                          {
                                              MaterialCode = materialTotal.F91_MaterialCode,
                                              MaterialName = material.F01_MaterialDsp,
                                              Recieved = materialTotal.F91_Received,
                                              Remain = materialTotal.F91_PrevRemainder,
                                              Remaincurr = (materialTotal.F91_Received + materialTotal.F91_PrevRemainder - materialTotal.F91_Used),
                                              Used = materialTotal.F91_Used,
                                              Updatedate = materialTotal.F91_UpdateDate,
                                              YearMonth = materialTotal.F91_YearMonth
                                          };
            // Count the total record which matches with the conditions.
            var totalRecords = await consumerMaterialsResult.CountAsync();
            var consumerMaterialsResultst = consumerMaterialsResult.ToList().Take(30).Select(m => new SubReceivedConsumedMaterialItem
            {
                MaterialCode = m.MaterialCode,
                MaterialName = m.MaterialName,
                Recieved = m.Recieved,
                Remain = m.Remain,
                Remaincurr = m.Remaincurr,
                Used = m.Used,
                Receivedst = m.Recieved.ToString("###,###,###,##0.00"),
                Remainst = m.Remain.ToString("###,###,###,##0.00"),
                Usedst = m.Used.ToString("###,###,###,##0.00"),
                Remaincurrst = m.Remaincurr.ToString("###,###,###,##0.00"),
                Updatedatest = m.Updatedate.HasValue ? m.Updatedate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                Updatedate = m.Updatedate,
                YearMonth = m.YearMonth,
                YearMonthst = m.YearMonth.HasValue ? m.YearMonth.Value.ToString("MM/yyyy") : ""
            });
            //var realPageIndex = gridSettings.PageIndex - 1;

            // Do pagination.
            //consumerMaterialsResult = consumerMaterialsResult;
            //.Skip(realPageIndex * gridSettings.PageSize)
            //.Take(gridSettings.PageSize);

            // Build a grid and respond to client.
            var resultModel = new GridResponse<object>(consumerMaterialsResultst, totalRecords);
            return new ResponseResult<GridResponse<object>>(resultModel, true);
        }


        public bool UpdateConsumer(DateTime yearMonth, double received, double remain, double used, string materialCode)
        {

            var itemold =
                _unitOfWork.MaterialTotalRepository
                    .GetAll()
                    .FirstOrDefault(m => m.F91_MaterialCode == materialCode && m.F91_YearMonth == yearMonth);
            try
            {
                itemold.F91_Received = received;
                itemold.F91_Used = used;
                itemold.F91_PrevRemainder = remain;
                itemold.F91_UpdateDate = DateTime.Now;
                _unitOfWork.MaterialTotalRepository.Update(itemold);
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private double TotalReceived(DateTime fromdate, string materialcode)
        {
            double totalReceived = 0;
            var endmonth = fromdate.AddMonths(1);
            var receptions = _unitOfWork.ReceptionRepository.GetAll().Where(m => m.F30_AcceptClass.Equals("1") && m.F30_MaterialCode.Equals(materialcode) && m.F30_UpdateDate >= fromdate && m.F30_UpdateDate < endmonth);
            try
            {

                totalReceived = receptions.Sum(m => m.F30_StoragedAmount);
            }
            catch (Exception ex)
            {
                //show detail
                var error = ex.Message;
            }
            return totalReceived;
        }

        private double TotalUsed(DateTime fromdate, string materialcode)
        {
            double totalUsed = 0;
            var endmonth = fromdate.AddMonths(1);
            var kneadingRecords = _unitOfWork.KneadingRecordRepository.GetAll().Where(m => m.F43_MaterialCode.Equals(materialcode) && m.F43_UpdateDate >= fromdate && m.F43_UpdateDate < endmonth);
            try
            {

                totalUsed = kneadingRecords.Sum(m => m.F43_LayinginAmount);
            }
            catch (Exception ex)
            {
                //show detail
                var error = ex.Message;
            }
            return totalUsed;
        }


        #endregion

        #region Preproduct consumer received

        public bool CheckConsumerPreProducts(string yearmonth)
        {
            DateTime fromdate = DateTime.ParseExact("01/" + yearmonth, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime nextmonth = fromdate.AddMonths(1);
            // Find item 92
            var preProductTotals = _unitOfWork.PreProductTotalRepository.GetAll();

            // Find item 03
            var preproducts = _unitOfWork.PreProductRepository.GetAll();
            

            var consumerPreProductsResult = from preproduct in preproducts
                                          join preProductTotal in preProductTotals on preproduct.F03_PreProductCode equals preProductTotal.F92_PrepdtCode
                                          where preProductTotal.F92_YearMonth >= fromdate && preProductTotal.F92_YearMonth < nextmonth

                                          select new SubReceivedConsumedPreproductItem()
                                          {
                                              PreProductCode = preProductTotal.F92_PrepdtCode,
                                              PreProductName = preproduct.F03_PreProductName,
                                              Recieved = preProductTotal.F92_PrevRemainder,
                                              Remain = preProductTotal.F92_PrevRemainder,
                                              Remaincurr = (preProductTotal.F92_Received + preProductTotal.F92_PrevRemainder - preProductTotal.F92_Used),
                                              Used = preProductTotal.F92_Used,
                                              Updatedate = preProductTotal.F92_UpdateDate,
                                          };

            if (consumerPreProductsResult.Count() == 0)
            {
                return false;
            }

            return true;
        }
        public async Task<ResponseResult<GridResponse<object>>> LoadConsumerPreProducts(string yearmonth,int page)
        {
            var pageIndex = 0;
            if (page > 0)
            {
                pageIndex = page;
            }
            DateTime fromdate = DateTime.ParseExact("01/" + yearmonth, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime nextmonth = fromdate.AddMonths(1);
            // Find item 92
            var preproductTotals = _unitOfWork.PreProductTotalRepository.GetAll();

            // Find item 03
            var preproducts = _unitOfWork.PreProductRepository.GetAll();


            var consumerPreProductsResult = from preproduct in preproducts
                                          join preproductTotal in preproductTotals on preproduct.F03_PreProductCode equals preproductTotal.F92_PrepdtCode
                                          where preproductTotal.F92_YearMonth >= fromdate && preproductTotal.F92_YearMonth < nextmonth

                                          select new SubReceivedConsumedPreproductItem()
                                          {
                                              PreProductCode = preproductTotal.F92_PrepdtCode,
                                              PreProductName = preproduct.F03_PreProductName,
                                              Recieved = preproductTotal.F92_Received,
                                              Remain = preproductTotal.F92_PrevRemainder,
                                              Remaincurr = (preproductTotal.F92_Received + preproductTotal.F92_PrevRemainder - preproductTotal.F92_Used),
                                              Used = preproductTotal.F92_Used,
                                              //Recievedst = preproductTotal.F92_Received.ToString("###,###,###,##0.00"),
                                              //Remainst = preproductTotal.F92_PrevRemainder.ToString("###,###,###,##0.00"),
                                              //Usedst = preproductTotal.F92_Used.ToString("###,###,###,##0.00"),
                                              //Remaincurrst = (preproductTotal.F92_Received + preproductTotal.F92_PrevRemainder - preproductTotal.F92_Used).ToString("###,###,###,##0.00"),

                                              Updatedate = preproductTotal.F92_UpdateDate,
                                              YearMonth = preproductTotal.F92_YearMonth
                                          };

            // Count the total record which matches with the conditions.
            var totalRecords = await consumerPreProductsResult.CountAsync();
            var consumerPreProductsResultst = consumerPreProductsResult.ToList().Skip((pageIndex - 1) * 30).Take(30).Select(m => new SubReceivedConsumedPreproductItem
            {
                PreProductCode = m.PreProductCode,
                PreProductName = m.PreProductName,
                Recieved = Math.Round(m.Recieved, 2),
                Remain = Math.Round(m.Remain, 2),
                Remaincurr = m.Remaincurr,
                Used = Math.Round(m.Used, 2),
                Recievedst = m.Recieved.ToString("###,###,###,##0.00"),
                Remainst = m.Remain.ToString("###,###,###,##0.00"),
                Usedst = m.Used.ToString("###,###,###,##0.00"),
                Remaincurrst = m.Remaincurr.ToString("###,###,###,##0.00"),
                Updatedatest = m.Updatedate.HasValue ? m.Updatedate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                Updatedate = m.Updatedate,
                YearMonth = m.YearMonth,
                YearMonthst = m.YearMonth.HasValue ? m.YearMonth.Value.ToString("MM/yyyy") : ""
            });

            // Build a grid and respond to client.
            var resultModel = new GridResponse<object>(consumerPreProductsResultst, totalRecords);
            return new ResponseResult<GridResponse<object>>(resultModel, true);
        }

        public async Task<ResponseResult<GridResponse<object>>> RecalculatePreProduct(string yearmonth,string precode)
        {
            DateTime fromdate = DateTime.ParseExact("01/" + yearmonth, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime nextmonth = fromdate.AddMonths(1);
            // Find item 92
            var preProductTotals = _unitOfWork.PreProductTotalRepository.GetAll();

            // Find item 01
            var preproducts = _unitOfWork.PreProductRepository.GetAll();
            // Find item 64
            var preProductStorageRetrieveHistorys = _unitOfWork.PreProductStorageRetrieveHistoryRepository.GetAll().Where(m => m.F64_UpdateDate >= fromdate && m.F64_UpdateDate < nextmonth);
            //// Find item 43
            //var kneadingRecords = _unitOfWork.KneadingRecordRepository.GetAll().Where(m => m.F43_UpdateDate >= fromdate && m.F43_UpdateDate < nextmonth);

            var listolditem = preProductTotals.Where(m => m.F92_YearMonth == fromdate).Where(m => m.F92_PrepdtCode.Trim().Equals(precode)).FirstOrDefault();


            // check item to add or update to tx91
            var listaddingitem = from preproduct in preproducts
                                 join preProductStorageRetrieveHistory in preProductStorageRetrieveHistorys on preproduct.F03_PreProductCode equals preProductStorageRetrieveHistory.F64_PreProductCode
                                 where true
                                 select new TX92PreProductTotal()
                                 {
                                     F92_PreProductCode = preproduct.F03_PreProductCode,
                                     F92_YearMonth = fromdate,
                                     F92_AddDate = DateTime.Now,
                                     F92_UpdateDate = DateTime.Now
                                 };
            // add item for tx 92
            var addingitems = listaddingitem.ToList();
            if (precode == null)
            {
                foreach (var preproduct in preproducts)
                {
                    double pre = 0;
                    double used = 0;
                    double rec = 0;
                    var oldpre = preProductTotals.Where(m => m.F92_PrepdtCode.Trim().Equals(preproduct.F03_PreProductCode.Trim())).ToList().Where(m => m.F92_YearMonth == fromdate.AddMonths(-1)).FirstOrDefault();
                    if (oldpre != null)
                    {
                        pre = oldpre.F92_PrevRemainder;
                        used = oldpre.F92_Used;
                        rec = oldpre.F92_Received;
                    }
                    TX92_PrepdtTotal updatetx92 = new TX92_PrepdtTotal();
                    updatetx92.F92_PrepdtCode = preproduct.F03_PreProductCode;
                    updatetx92.F92_Received = TotalPreProductReceived(fromdate, preproduct.F03_PreProductCode);
                    updatetx92.F92_Used = TotalPreProductUsed(fromdate, preproduct.F03_PreProductCode);
                    updatetx92.F92_PrevRemainder =rec + pre -
                          used;
                    updatetx92.F92_YearMonth = fromdate;
                    updatetx92.F92_UpdateDate = DateTime.Now;
                    updatetx92.F92_AddDate = DateTime.Now;
                    //if (listolditem.Any(m => m.F92_PrepdtCode.Equals(tx92MaterialTotal.F92_PreProductCode)))
                    //{
                    //    var itemupdate =
                    //        listolditem.Where(m => m.F92_PrepdtCode.Equals(tx92MaterialTotal.F92_PreProductCode)).First();
                    //    itemupdate.F92_Received = TotalPreProductReceived(fromdate, tx92MaterialTotal.F92_PreProductCode);
                    //    itemupdate.F92_Used = TotalPreProductReceived(fromdate, tx92MaterialTotal.F92_PreProductCode);
                    //    itemupdate.F92_PrevRemainder = TotalPreProductReceived(fromdate, tx92MaterialTotal.F92_PreProductCode) > 0 && TotalPreProductReceived(fromdate, tx92MaterialTotal.F92_PreProductCode) > TotalPreProductUsed(fromdate, tx92MaterialTotal.F92_PreProductCode) ? TotalPreProductReceived(fromdate, tx92MaterialTotal.F92_PreProductCode) - TotalPreProductUsed(fromdate, tx92MaterialTotal.F92_PreProductCode) : 0;
                    //    itemupdate.F92_UpdateDate = DateTime.Now;
                    //    _unitOfWork.PreProductTotalRepository.Update(itemupdate);
                    //}
                    //else
                    //{
                    _unitOfWork.PreProductTotalRepository.Add(updatetx92);
                    //}
                }
            }
            else
            {
                if (listolditem != null)
                {
                    listolditem.F92_Received = TotalPreProductReceived(fromdate, precode);
                    listolditem.F92_Used = TotalPreProductUsed(fromdate, precode);
                    //listolditem.F92_PrevRemainder = TotalPreProductReceived(fromdate, precode) > 0 && TotalPreProductReceived(fromdate, precode) > TotalPreProductUsed(fromdate, precode) ? TotalPreProductReceived(fromdate, precode) - TotalPreProductUsed(fromdate, precode) : 0;
                    listolditem.F92_UpdateDate = DateTime.Now;
                    _unitOfWork.PreProductTotalRepository.Update(listolditem);
                }

            }


            _unitOfWork.Commit();

            var consumerMaterialsResult = from preProductTotal in preProductTotals
                                          join preproduct in preproducts on preProductTotal.F92_PrepdtCode equals preproduct.F03_PreProductCode
                                          where preProductTotal.F92_YearMonth == fromdate

                                          select new SubReceivedConsumedPreproductItem()
                                          {
                                              PreProductCode = preProductTotal.F92_PrepdtCode,
                                              PreProductName = preproduct.F03_PreProductName,
                                              Recieved = preProductTotal.F92_Received,
                                              Remain = preProductTotal.F92_PrevRemainder,
                                              Remaincurr = (preProductTotal.F92_Received + preProductTotal.F92_PrevRemainder - preProductTotal.F92_Used),
                                              Used = preProductTotal.F92_Used,
                                              Updatedate = preProductTotal.F92_UpdateDate,
                                              YearMonth = preProductTotal.F92_YearMonth
                                          };
            // Count the total record which matches with the conditions.
            var totalRecords = await consumerMaterialsResult.CountAsync();
            var consumerPreProductsResultst = consumerMaterialsResult.ToList().Take(30).Select(m => new SubReceivedConsumedPreproductItem
            {
                PreProductCode = m.PreProductCode,
                PreProductName = m.PreProductName,
                Recieved = m.Recieved,
                Remain = m.Remain,
                Remaincurr = m.Remaincurr,
                Used = m.Used,
                Recievedst = m.Recieved.ToString("###,###,###,##0.00"),
                Remainst = m.Remain.ToString("###,###,###,##0.00"),
                Usedst = m.Used.ToString("###,###,###,##0.00"),
                Remaincurrst = m.Remaincurr.ToString("###,###,###,##0.00"),
                Updatedatest = m.Updatedate.HasValue ? m.Updatedate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                Updatedate = m.Updatedate,
                YearMonth = m.YearMonth,
                YearMonthst = m.YearMonth.HasValue ? m.YearMonth.Value.ToString("MM/yyyy") : ""
            });
            //var realPageIndex = gridSettings.PageIndex - 1;

            // Do pagination.
            //consumerMaterialsResult = consumerMaterialsResult;
            //.Skip(realPageIndex * gridSettings.PageSize)
            //.Take(gridSettings.PageSize);

            // Build a grid and respond to client.
            var resultModel = new GridResponse<object>(consumerPreProductsResultst, totalRecords);
            return new ResponseResult<GridResponse<object>>(resultModel, true);
        }

        public async Task<PrintManagementReportItem> PrintConsumerPreProducts(string yearmonth)
        {
            DateTime fromdate = DateTime.ParseExact("01/" + yearmonth, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime nextmonth = fromdate.AddMonths(1);


            // Find item 92
            var preproductTotals = _unitOfWork.PreProductTotalRepository.GetAll();

            // Find item 03
            var preproducts = _unitOfWork.PreProductRepository.GetAll();



            var printManagementReportItem = new PrintManagementReportItem();


            // Find Management Report item.
            var managementReportItems = from preproduct in preproducts
                                        join preproductTotal in preproductTotals on preproduct.F03_PreProductCode equals preproductTotal.F92_PrepdtCode
                                        where preproductTotal.F92_YearMonth >= fromdate && preproductTotal.F92_YearMonth < nextmonth

                                        select new FindPrintManagementReportItem()
                                        {
                                            PreProductCode = preproductTotal.F92_PrepdtCode,
                                            PreProductName = preproduct.F03_PreProductName,
                                            Unit = "Kg",
                                            Received = preproductTotal.F92_Received,
                                            Used = preproductTotal.F92_Used,
                                            Remain = preproductTotal.F92_PrevRemainder
                                        };




            //•	o	Show “PAGE: “ + current page with format as ###            
            printManagementReportItem.Page = "1";
            printManagementReportItem.Month = yearmonth;

            // Get Total quantity
            printManagementReportItem.Total = "";

            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printManagementReportItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printManagementReportItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printManagementReportItem.FindPrintManagementReportItem = await managementReportItems.ToListAsync();
            printManagementReportItem.FindPrintManagementReportItem = printManagementReportItem.FindPrintManagementReportItem.Select(m => new FindPrintManagementReportItem()
            {
                PreProductCode = m.PreProductCode,
                PreProductName = m.PreProductName,
                Unit = "Kg",
                Receivedst = m.Received.ToString("###,###,###,##0.00"),
                Usedst = m.Used.ToString("###,###,###,##0.00"),
                Remainst = m.Remain.ToString("###,###,###,##0.00"),
                Remaincurrst = (m.Received + m.Remain - m.Used).ToString("###,###,###,##0.00")
            }).ToList();
            return printManagementReportItem;
        }

        public bool UpdatePreProductConsumer(DateTime yearMonth, double received, double remain, double used, string preproductCode)
        {

            var itemold =
                _unitOfWork.PreProductTotalRepository
                    .GetAll()
                    .FirstOrDefault(m => m.F92_PrepdtCode == preproductCode && m.F92_YearMonth == yearMonth);
            try
            {
                itemold.F92_Received = received;
                itemold.F92_Used = used;
                itemold.F92_PrevRemainder = remain;
                itemold.F92_UpdateDate = DateTime.Now;
                _unitOfWork.PreProductTotalRepository.Update(itemold);
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private double TotalPreProductReceived(DateTime fromdate, string preproductcode)
        {
            double totalReceived = 0;
            var endmonth = fromdate.AddMonths(1);
            var preProductStorageRetrieveHistorys = _unitOfWork.PreProductStorageRetrieveHistoryRepository.GetAll().Where(m => m.F64_StgRtrCls.Equals("0") && m.F64_PreProductCode.Equals(preproductcode) && m.F64_UpdateDate >= fromdate && m.F64_UpdateDate < endmonth);
            try
            {

                totalReceived = preProductStorageRetrieveHistorys.Sum(m => m.F64_Amount);
            }
            catch (Exception ex)
            {
                //show detail
                var error = ex.Message;
            }
            return totalReceived;
        }

        private double TotalPreProductUsed(DateTime fromdate, string preproductcode)
        {
            double totalUsed = 0;
            var endmonth = fromdate.AddMonths(1);
            var preProductStorageRetrieveHistorys = _unitOfWork.PreProductStorageRetrieveHistoryRepository.GetAll().Where(m => m.F64_StgRtrCls.Equals("1") && m.F64_PreProductCode.Equals(preproductcode) && m.F64_UpdateDate >= fromdate && m.F64_UpdateDate < endmonth);
            try
            {

                totalUsed = preProductStorageRetrieveHistorys.Sum(m => m.F64_Amount);
            }
            catch (Exception ex)
            {
                //show detail
                var error = ex.Message;
            }
            return totalUsed;
        }
        #endregion

        #region Product Certificate Shipped List


        public bool CheckConsumerCerfiticates(string yearmonth)
        {
            DateTime fromdate = DateTime.ParseExact("01/" + yearmonth, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime nextmonth = fromdate.AddMonths(1);
            // Find item 93
            var productTotals = _unitOfWork.ProductTotalRepository.GetAll();

            // Find item 09
            var products = _unitOfWork.ProductRepository.GetAll();
          

            var consumerProductsResult = from product in products
                                          join productTotal in productTotals on product.F09_ProductCode equals productTotal.F93_ProductCode
                                          where productTotal.F93_YearMonth >= fromdate && productTotal.F93_YearMonth < nextmonth

                                          select new SubReceivedConsumedProductItem()
                                          {
                                              ProductCode = productTotal.F93_ProductCode,
                                              ProductName = product.F09_ProductDesp,
                                              Recieved = productTotal.F93_Received,
                                              Remain = productTotal.F93_PrevRemainder,
                                              Remaincurr = (productTotal.F93_Received + productTotal.F93_PrevRemainder - productTotal.F93_Used),
                                              Used = productTotal.F93_Used,
                                              Updatedate = productTotal.F93_UpdateDate,
                                          };

            if (consumerProductsResult.Count() == 0)
            {
                return false;
            }

            return true;
        }

        public async Task<ResponseResult<GridResponse<object>>> LoadConsumerCerfiticates(string yearmonth,int page)
        {
            var pageIndex = 0;
            if (page > 0)
            {
                pageIndex = page;
            }
            DateTime fromdate = DateTime.ParseExact("01/" + yearmonth, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime nextmonth = fromdate.AddMonths(1);
            // Find item 93
            var productTotals = _unitOfWork.ProductTotalRepository.GetAll();

            // Find item 09
            var products = _unitOfWork.ProductRepository.GetAll();


            var consumerProductsResult = from product in products
                                         join productTotal in productTotals on product.F09_ProductCode equals productTotal.F93_ProductCode
                                         where productTotal.F93_YearMonth >= fromdate && productTotal.F93_YearMonth < nextmonth

                                         select new SubReceivedConsumedProductItem()
                                         {
                                             ProductCode = productTotal.F93_ProductCode,
                                             ProductName = product.F09_ProductDesp,
                                             Recieved = productTotal.F93_Received,
                                             Remain = productTotal.F93_PrevRemainder,
                                             Remaincurr = (productTotal.F93_Received + productTotal.F93_PrevRemainder - productTotal.F93_Used),
                                             Used = productTotal.F93_Used,
                                             //Recievedst = productTotal.F93_Received.ToString("###,###,###,##0.00"),
                                             //Remainst = productTotal.F93_PrevRemainder.ToString("###,###,###,##0.00"),
                                             //Usedst = productTotal.F93_Used.ToString("###,###,###,##0.00"),
                                             //Remaincurrst = (productTotal.F93_Received + productTotal.F93_PrevRemainder - productTotal.F93_Used).ToString("###,###,###,##0.00"),
                                             Updatedate = productTotal.F93_UpdateDate,
                                             YearMonth = productTotal.F93_YearMonth
                                         };

            // Count the total record which matches with the conditions.
            var totalRecords = await consumerProductsResult.CountAsync();

            var consumerProductsResultst = consumerProductsResult.ToList().Skip((pageIndex - 1) * 30).Take(30).Select(m => new SubReceivedConsumedProductItem
            {
                ProductCode = m.ProductCode,
                ProductName = m.ProductName,
                Recieved = Math.Round(m.Recieved, 2),
                Remain = Math.Round(m.Remain, 2),
                Remaincurr = m.Remaincurr,
                Used = Math.Round(m.Used, 2),
                Recievedst = m.Recieved.ToString("###,###,###,##0.00"),
                Remainst = m.Remain.ToString("###,###,###,##0.00"),
                Usedst = m.Used.ToString("###,###,###,##0.00"),
                Remaincurrst = m.Remaincurr.ToString("###,###,###,##0.00"),

                Updatedatest = m.Updatedate.HasValue ? m.Updatedate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                YearMonth = m.YearMonth,
                YearMonthst = m.YearMonth.HasValue ? m.YearMonth.Value.ToString("MM/yyyy") : ""
            });
            // Build a grid and respond to client.
            var resultModel = new GridResponse<object>(consumerProductsResultst, totalRecords);
            return new ResponseResult<GridResponse<object>>(resultModel, true);
        }

        public async Task<PrintManagementReportItem> PrintConsumerCerfiticates(string yearmonth)
        {
            DateTime fromdate = DateTime.ParseExact("01/" + yearmonth, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime nextmonth = fromdate.AddMonths(1);


            // Find item 93
            var productTotals = _unitOfWork.ProductTotalRepository.GetAll();

            // Find item 09
            var products = _unitOfWork.ProductRepository.GetAll();



            var printManagementReportItem = new PrintManagementReportItem();


            // Find Management Report item.
            var managementReportItems = from product in products
                                        join productTotal in productTotals on product.F09_ProductCode equals productTotal.F93_ProductCode
                                        where productTotal.F93_YearMonth >= fromdate && productTotal.F93_YearMonth < nextmonth

                                        select new FindPrintManagementReportItem()
                                        {
                                            ProductCode = productTotal.F93_ProductCode,
                                            ProductName = product.F09_ProductDesp,
                                            Unit = "Kg",
                                            Received = productTotal.F93_Received,
                                            Used = productTotal.F93_Used,
                                            Remain = productTotal.F93_PrevRemainder
                                        };




            //•	o	Show “PAGE: “ + current page with format as ###            
            printManagementReportItem.Page = "1";
            printManagementReportItem.Month = yearmonth;

            // Get Total quantity
            printManagementReportItem.Total = "";

            // o	Show {current date/time} with format: dd-mm-yyyy hh:ss                                           
            printManagementReportItem.Datetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            /*
             * o	Show {Company Name} = value from profile:
             * ProfileString("toshiba.ini","server","companyname","")
            */
            printManagementReportItem.CompanyName = WebConfigurationManager.AppSettings["CompanyName"];


            await _unitOfWork.CommitAsync();

            printManagementReportItem.FindPrintManagementReportItem = await managementReportItems.ToListAsync();
            printManagementReportItem.FindPrintManagementReportItem = printManagementReportItem.FindPrintManagementReportItem.Select(m => new FindPrintManagementReportItem()
            {
                ProductCode = m.ProductCode,
                ProductName = m.ProductName,
                Unit = "Kg",
                Receivedst = m.Received.ToString("###,###,###,##0.00"),
                Usedst = m.Used.ToString("###,###,###,##0.00"),
                Remainst = m.Remain.ToString("###,###,###,##0.00"),
                Remaincurrst = (m.Received + m.Remain - m.Used).ToString("###,###,###,##0.00")
            }).ToList();
            return printManagementReportItem;
        }

        public async Task<ResponseResult<GridResponse<object>>> RecalculateCerfiticate(string yearmonth,string procode)
        {
            DateTime fromdate = DateTime.ParseExact("01/" + yearmonth, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            DateTime nextmonth = fromdate.AddMonths(1);
            // Find item 93
            var productTotals = _unitOfWork.ProductTotalRepository.GetAll();

            // Find item 09
            var products = _unitOfWork.ProductRepository.GetAll();
            // Find item 67
            var certificateHistorys = _unitOfWork.CertificateHistoryRepository.GetAll().Where(m => m.F67_CertificationFlag.Equals("1") && m.F67_UpdateDate >= fromdate && m.F67_UpdateDate < nextmonth);
            // Find item 44
            var shippingPlans = _unitOfWork.ShippingPlanRepository.GetAll().Where(m => m.F44_UpdateDate >= fromdate && m.F44_UpdateDate < nextmonth);

            var listolditem = productTotals.Where(m => m.F93_YearMonth == fromdate).Where(m => m.F93_ProductCode.Trim().Contains(procode.Trim())).FirstOrDefault();


            // check item to add or update to tx93
            var listaddingitem = from product in products
                                 join productTotal in productTotals on product.F09_ProductCode equals productTotal.F93_ProductCode
                                 where true
                                 select new TX93ProductTotal
                                 {
                                     F93_ProductCode = product.F09_ProductCode,
                                     F93_YearMonth = fromdate,
                                     F93_AddDate = DateTime.Now,
                                     F93_UpdateDate = DateTime.Now
                                 };
            // add item for tx 93
            var addingitems = listaddingitem.ToList();
            if (procode == null)
            {
                foreach (var product in products)
                {
                    double pre = 0;
                    double used = 0;
                    double rec = 0;
                    var oldpre = productTotals.Where(m => m.F93_ProductCode.Trim().Equals(product.F09_ProductCode.Trim())).ToList().Where(m => m.F93_YearMonth == fromdate.AddMonths(-1)).FirstOrDefault();
                    if (oldpre != null)
                    {
                        pre = oldpre.F93_PrevRemainder;
                        used = oldpre.F93_Used;
                        rec = oldpre.F93_Received;
                    }
                    TX93_ProductTotal updatetx93 = new TX93_ProductTotal();
                    updatetx93.F93_ProductCode = product.F09_ProductCode;
                    updatetx93.F93_Received = TotalCerfiticateReceived(fromdate, product.F09_ProductCode);
                    updatetx93.F93_Used = TotalCerfiticateUsed(fromdate, product.F09_ProductCode);
                    updatetx93.F93_PrevRemainder =rec + pre -
                          used;
                    updatetx93.F93_YearMonth = fromdate;
                    updatetx93.F93_UpdateDate = DateTime.Now;
                    updatetx93.F93_AddDate = DateTime.Now;
                    //if (listolditem.Any(m => m.F93_ProductCode.Equals(tx93MaterialTotal.F93_ProductCode)))
                    //{
                    //    var updateitem =
                    //        listolditem.Where(m => m.F93_ProductCode.Equals(tx93MaterialTotal.F93_ProductCode)).First();
                    //    updatetx93.F93_Received = TotalCerfiticateReceived(fromdate, tx93MaterialTotal.F93_ProductCode);
                    //    updatetx93.F93_Used = TotalCerfiticateUsed(fromdate, tx93MaterialTotal.F93_ProductCode);
                    //    updatetx93.F93_PrevRemainder = TotalCerfiticateReceived(fromdate, tx93MaterialTotal.F93_ProductCode) > 0 && TotalCerfiticateReceived(fromdate, tx93MaterialTotal.F93_ProductCode) > TotalCerfiticateUsed(fromdate, tx93MaterialTotal.F93_ProductCode) ? TotalCerfiticateReceived(fromdate, tx93MaterialTotal.F93_ProductCode) - TotalCerfiticateUsed(fromdate, tx93MaterialTotal.F93_ProductCode) : 0;
                    //    updateitem.F93_UpdateDate = DateTime.Now; ;
                    //    _unitOfWork.ProductTotalRepository.Update(updateitem);
                    //}
                    //else
                    //{
                    _unitOfWork.ProductTotalRepository.Add(updatetx93);
                    //}
                }
            }
            else
            {
                if (listolditem != null)
                {
                    listolditem.F93_Received = TotalCerfiticateReceived(fromdate, procode);
                    listolditem.F93_Used = TotalCerfiticateUsed(fromdate, procode);
                    listolditem.F93_PrevRemainder = TotalCerfiticateReceived(fromdate, procode) > 0 && TotalCerfiticateReceived(fromdate, procode) > TotalCerfiticateUsed(fromdate, procode) ? TotalCerfiticateReceived(fromdate, procode) - TotalCerfiticateUsed(fromdate, procode) : 0;
                    listolditem.F93_UpdateDate = DateTime.Now;
                    _unitOfWork.ProductTotalRepository.Update(listolditem);
                }
            }

            _unitOfWork.Commit();

            var consumerProductsResult = from product in products
                                         join productTotal in productTotals on product.F09_ProductCode equals productTotal.F93_ProductCode
                                         where productTotal.F93_YearMonth >= fromdate && productTotal.F93_YearMonth < nextmonth

                                         select new SubReceivedConsumedProductItem()
                                         {
                                             ProductCode = productTotal.F93_ProductCode,
                                             ProductName = product.F09_ProductDesp,
                                             Recieved = productTotal.F93_Received,
                                             Remain = productTotal.F93_PrevRemainder,
                                             Remaincurr = (productTotal.F93_Received + productTotal.F93_PrevRemainder - productTotal.F93_Used),
                                             Used = productTotal.F93_Used,
                                             Updatedate = productTotal.F93_UpdateDate,
                                             YearMonth = productTotal.F93_YearMonth,
                                         };

            // Count the total record which matches with the conditions.
            var totalRecords = await consumerProductsResult.CountAsync();
            var consumerProductsResultst = consumerProductsResult.ToList().Take(30).Select(m => new SubReceivedConsumedProductItem
            {
                ProductCode = m.ProductCode,
                ProductName = m.ProductName,
                Recieved = m.Recieved,
                Remain = m.Remain,
                Remaincurr = m.Remaincurr,
                Used = m.Used,
                Recievedst = m.Recieved.ToString("###,###,###,##0.00"),
                Remainst = m.Remain.ToString("###,###,###,##0.00"),
                Usedst = m.Used.ToString("###,###,###,##0.00"),
                Remaincurrst = m.Remaincurr.ToString("###,###,###,##0.00"),
                Updatedatest = m.Updatedate.HasValue ? m.Updatedate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                Updatedate = m.Updatedate,
                YearMonth = m.YearMonth,
                YearMonthst = m.YearMonth.HasValue ? m.YearMonth.Value.ToString("MM/yyyy") : ""
            });
            //var realPageIndex = gridSettings.PageIndex - 1;

            // Do pagination.
            //consumerMaterialsResult = consumerMaterialsResult;
            //.Skip(realPageIndex * gridSettings.PageSize)
            //.Take(gridSettings.PageSize);

            // Build a grid and respond to client.
            var resultModel = new GridResponse<object>(consumerProductsResultst, totalRecords);
            return new ResponseResult<GridResponse<object>>(resultModel, true);
        }


        public bool UpdateConsumerCerfiticate(DateTime yearMonth, double received, double remain, double used, string productcode)
        {

            var itemold =
                _unitOfWork.ProductTotalRepository
                    .GetAll()
                    .FirstOrDefault(m => m.F93_ProductCode == productcode && m.F93_YearMonth == yearMonth);
            try
            {
                itemold.F93_Received = received;
                itemold.F93_Used = used;
                itemold.F93_PrevRemainder = remain;
                itemold.F93_UpdateDate = DateTime.Now;
                _unitOfWork.ProductTotalRepository.Update(itemold);
                _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private double TotalCerfiticateReceived(DateTime fromdate, string productcode)
        {
            double totalReceived = 0;
            var endmonth = fromdate.AddMonths(1);
            var certificateHistorys = _unitOfWork.CertificateHistoryRepository.GetAll().Where(m => m.F67_CertificationFlag.Equals("1") && m.F67_ProductCode.Equals(productcode) && m.F67_UpdateDate >= fromdate && m.F67_UpdateDate < endmonth);
            try
            {

                totalReceived = certificateHistorys.Sum(m => m.F67_Amount);
            }
            catch (Exception ex)
            {
                //show detail
                var error = ex.Message;
            }
            return totalReceived;
        }

        private double TotalCerfiticateUsed(DateTime fromdate, string productcode)
        {
            double totalUsed = 0;
            var endmonth = fromdate.AddMonths(1);
            var shippingPlans = _unitOfWork.ShippingPlanRepository.GetAll().Where(m => m.F44_ProductCode.Equals(productcode) && m.F44_UpdateDate >= fromdate && m.F44_UpdateDate < endmonth);
            try
            {

                totalUsed = shippingPlans.Sum(m => m.F44_ShippedAmount);
            }
            catch (Exception ex)
            {
                //show detail
                var error = ex.Message;
            }
            return totalUsed;
        }


        #endregion
    }
}
