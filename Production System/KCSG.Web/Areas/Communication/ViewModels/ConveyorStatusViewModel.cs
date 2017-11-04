using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using KCSG.Core.Constants;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.Communication.ViewModels
{
    public class ConveyorStatusViewModel
    {
        public Grid ConveyorGrid { get; set; }
        [DisplayName("Conveyor Code")]
        public string ConveyorCode { get; set; }
        [DisplayName("Using Buffer")]
        public int? UsingBuffer { get; set; }
         [DisplayName("Conveyor Status")]
        public Constants.F05_StrRtrSts? ConveyorStatus { get; set; }
    }
}