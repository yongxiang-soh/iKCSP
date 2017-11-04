using System.Timers;
using System.Web.Mvc;
using KCSG.Domain.Interfaces.Services;
using KCSG.jsGrid.MVC;
using KCSG.jsGrid.MVC.Enums;
using KCSG.Web.Attributes;

namespace KCSG.Web.Areas.Communication.Controllers
{
    [MvcAuthorize]
    public class BaseCommucationController : KCSG.Web.Controllers.BaseController
    {
        #region Properties

        /// <summary>
        /// Service which handles configuration business.
        /// </summary>
        protected readonly IConfigurationService _configurationService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initiate controller with IoC.
        /// </summary>
        /// <param name="configurationService"></param>
        public BaseCommucationController(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initiate queue grid.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Grid GenerateGridQueue(string url)
        {
            return new Grid("QueueGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                //.SetAutoload(false)
                .SetSearchUrl(url)
                .SetDefaultSorting("AddDate", SortOrder.Asc)
                .SetFields(
                    new Field("CommandNo")
                        .SetWidth(30)
                        .SetTitle(" ")
                        .SetItemTemplate("gridHelper.generateRadiobox")
                        .SetSorting(false),
                    new Field("CommandNo")
                        .SetWidth(100)
                        .SetTitle("Command No")
                        .SetItemTemplate("gridHelper.displayNumberT")
                        .SetSorting(false),
                    new Field("CommandSeqNo")
                        .SetTitle("Command Seq No")
                        .SetItemTemplate("gridHelper.displayNumberT")
                        .SetWidth(150),
                    new Field("CommandType")
                        .SetTitle("Command Type")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(150),
                    new Field("StrRtrType")
                        .SetTitle("StrRtr Type")
                        .SetSorting(false)
                        .SetWidth(100),
                    new Field("Status")
                        .SetTitle("Status")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("Priority")
                        .SetTitle("Priority")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("PalletNo")
                        .SetTitle("Pallet No")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberT"),
                    new Field("From")
                        .SetTitle("From")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("To")
                        .SetTitle("To")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("CommandSendDate")
                        .SetTitle("Command Send Date")
                        .SetWidth(150)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayDateFormat"),
                    new Field("CommandEndDate")
                        .SetTitle("Command End Date")
                        .SetWidth(150)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayDateFormat"),
                    new Field("TerminalNo")
                        .SetTitle("Terminal No")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("PictureNo")
                        .SetTitle("Picture No")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("AbnormalCode")
                        .SetTitle("Abnormal Code")
                        .SetWidth(150)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("AddDate")
                        .SetTitle("Add Date")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayDateFormat"),
                    new Field("UpdateDate")
                        .SetTitle("Update Date")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayDateFormat"),
                    new Field("RetryCount")
                        .SetTitle("Retry Count")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumberT")
                );
        }

        /// <summary>
        /// Initiate history grid.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Grid GenerateGridHistory(string url)
        {
            return new Grid("HistoryGrid")
                .SetMode(GridMode.Listing)
                .SetWidth("100%")
                .SetSelected(true)
                .SetSorting(true)
                .SetPaging(true)
                .SetPageSize(30)
                .SetPageLoading(true)
                .SetAutoload(false)
                .SetSearchUrl(url)
                .SetDefaultSorting("AddDate", SortOrder.Asc)
                .SetFields(
                    //new Field("CommandNo")
                    //       .SetWidth(30)
                    //       .SetTitle(" ")
                    //       .SetItemTemplate("gridHelper.generateRadiobox")
                    //       .SetSorting(false),
                    new Field("CommandNo")
                        .SetWidth(100)
                        .SetTitle("Command No")
                        .SetItemTemplate("gridHelper.displayNumberT")
                        .SetSorting(false),
                    new Field("CommandSeqNo")
                        .SetTitle("Command Seq No ")
                        .SetItemTemplate("gridHelper.displayNumberT")
                        .SetWidth(150),
                    new Field("CommandType")
                        .SetTitle("Command Type")
                        .SetItemTemplate("gridHelper.generateNameColumn")
                        .SetSorting(false)
                        .SetWidth(150),
                    new Field("Priority")
                        .SetTitle("Priority")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("From")
                        .SetTitle("From")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("To")
                        .SetTitle("To")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("CommandDate")
                        .SetTitle("Command Date")
                        .SetWidth(150)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayDate"),
                    new Field("PalletNo")
                        .SetTitle("Pallet No")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayNumber"),
                    new Field("AbnormalCode")
                        .SetTitle("Abnormal Code")
                        .SetWidth(150)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.generateNameColumn"),
                    new Field("AddDate")
                        .SetTitle("Add Date")
                        .SetWidth(100)
                        .SetSorting(false)
                        .SetItemTemplate("gridHelper.displayDate")
                );
        }

        #endregion
    }
}