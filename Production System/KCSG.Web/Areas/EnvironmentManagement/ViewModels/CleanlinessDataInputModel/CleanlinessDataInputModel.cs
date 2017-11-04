using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KCSG.jsGrid.MVC;

namespace KCSG.Web.Areas.EnvironmentManagement.ViewModels
{
    public class CleanlinessDataInputModel
    {
        public SearchCriteriaModel SearchCriteriaModel { get; set; }
        public Conv1 Conv1 { get; set; }
        public Conv2 Conv2 { get; set; }
        public Mega1 Mega1 { get; set; }
        public Mega2 Mega2 { get; set; }

        public string DateDiameter1 { get; set; }

        public string DateDiameter2 { get; set; }
        public string DateReport { get; set; }
        public string Company { get; set; }
    }

    public class Conv1
    {
        public double?[,] Datas { get; set; }
        public double[] Cp { get; set; }
        public double[] Sigma { get; set; }
        public double[] Mean { get; set; }
    } 
    public class Conv2
    {
        public double?[,] Datas { get; set; }
        public double[] Cp { get; set; }
        public double[] Sigma { get; set; }
        public double[] Mean { get; set; }
    }
    public class Mega1
    {
        public double?[,] Datas { get; set; }
        public double[] Cp { get; set; }
        public double[] Sigma { get; set; }
        public double[] Mean { get; set; }
    } 
    public class Mega2
    {
        public double?[,] Datas { get; set; }
        public double[] Cp { get; set; }
        public double[] Sigma { get; set; }
        public double[] Mean { get; set; }
    }
}