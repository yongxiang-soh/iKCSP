using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Data.DataModel;

namespace KCSG.Domain.Models.ProductionPlanning
{
    public class PrintPreProductItem:TM03_PreProduct
    {
        public string MasterialCode { get; set; }
        public string MasterialName { get; set; }
        public string WSeq { get; set; }
        public int C_Pri { get; set; }
        public double Sum3F4F { get; set; }
        public string PSeq { get; set; }
        public string Post { get; set; }
        public String Method { get; set; }
        public string Additive { get; set; }
        public string CSeq { get; set; }
      
        public string MaterialMakeUp  { get; set; }
        public string Crushing1 { get; set; }
        public string Crushing2 { get; set; }

        public String MixDate1 { get; set; }
        public String MixDate2 { get; set; }
        public String MixDate3 { get; set; }
        public string LowTmpClass
        {
            get
            {
                return Enum.GetName(typeof(Constants.Temperature), Convert.ToInt32(this.F03_LowTmpClass));
            }
        }

        public string KneadingLine
        {
            get
            {
                if (this.F03_KneadingLine.Equals(((int)Constants.KndLine.Megabit).ToString()))
                {
                    return "Mega";
                }
                else
                {
                    if (this.F03_ColorClass.Equals(((int)Constants.ColorClass.Black).ToString()))
                    {
                        return "Conv(B)";
                    }
                    else
                    {
                        return "Conv(C)";
                    }
                }
            }
        }
        public string EquilibriumTime
        {
            get
            {
                if (this.F03_TmpRetTime.Value.Date.Equals(Convert.ToDateTime(Constants.LastDummyDate).Date))
                {
                    return "31 days " + this.F03_TmpRetTime.Value.ToString("HH:mm");
                }
                else
                {
                    return this.F03_TmpRetTime.Value.Day + " days " + this.F03_TmpRetTime.Value.ToString("HH:mm");
                }
            }
        }
        public string Factor { get; set; }
        public string LoadPosition { get; set; }
        public string MilingFlag1 { get; set; }
        public string MilingFlag2 { get; set; }
        public List<MaterialPrintItem> MaterialPrintItems { get; set; }
        public string YieldRate { get; set; }
    }

    public class MaterialPrintItem
    {
        public string MasterialCode { get; set; }
        public string MasterialName { get; set; }
        public string WSeq { get; set; }
        public int C_Pri { get; set; }
        public string Sum3F4F { get; set; }
        public string PSeq { get; set; }
        public string Post { get; set; }
        public String Method { get; set; }
        public string Additive { get; set; }
        public string CSeq { get; set; }
        public string Crushing1 { get; set; }
        public string Crushing2 { get; set; }
        public string LoadPosition { get; set; }
        public string MilingFlag1 { get; set; }
        public string MilingFlag2 { get; set; }
    }
}
