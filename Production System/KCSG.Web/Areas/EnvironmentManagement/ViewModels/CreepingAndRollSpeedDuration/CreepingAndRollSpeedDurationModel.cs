using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCSG.Web.Areas.EnvironmentManagement.ViewModels
{
    public class CreepingAndRollSpeedDurationModel
    {
        public object SearchCriteriaModel { get; set; }
        public ChartModel LeftModel { get; set; }
        public ChartModel RightModel { get; set; }
        public ChartModel RollModel { get; set; }
    }
}