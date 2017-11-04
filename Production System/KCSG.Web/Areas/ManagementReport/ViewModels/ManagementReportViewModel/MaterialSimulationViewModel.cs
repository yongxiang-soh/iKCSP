using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;
using Resources;

namespace KCSG.Web.Areas.ManagementReport.ViewModels.ManagementReportViewModel
{
    public class ManagementReportViewModel
    {
        
        [DisplayName(@"From")]
        [Remote("CheckDate", "MaterialMovementHistory", HttpMethod = "OPTIONS",ErrorMessageResourceType = typeof(MessageResource), ErrorMessageResourceName ="MSG57" ,AdditionalFields = "To")]
        public string From { get; set; }
        
        [DisplayName(@"To")]
        [Remote("CheckDate", "MaterialMovementHistory", HttpMethod = "OPTIONS", AdditionalFields = "From")]
        public string To { get; set; }

    }
}