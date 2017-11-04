using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KCSG.Web.Areas.MaterialManagement.ViewModels.InterFloorMovementOfMaterialController
{
    public class InterFloorMovementOfMaterialViewModel
    {
        public string TerminalNo { get; set; }
        public int From { get; set; }
        public int To { get; set; }
    }
}