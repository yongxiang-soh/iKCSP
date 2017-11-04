using System;
using System.Collections.Generic;
using KCSG.Core.Constants;
using KCSG.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using KCSG.Data.DatabaseContext;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Domains;
using KCSG.Domain.Domains.ProductionPlanning;
using KCSG.Domain.Interfaces;
using KCSG.Domain.Interfaces.ProductionPlanning;

namespace KCSG.Domain.Models.ProductionPlanning
{
    public class PreProductItem : TM03_PreProduct
    {
        private IPrePdtMkpDomain _prePdtMkpDomain;
        public PreProductItem()
        {
            _prePdtMkpDomain = new PrePdtMkpDomain(new UnitOfWork(new KCSGDbContext()));
        }
       
        public bool IsCreate { get; set; }

        public List<PrePdtMkpItem> ListPrePdtMkp { get; set; }

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
                    if (this.F03_KneadingLine.Equals(((int)Constants.F39_KneadingLine.ConventionalB).ToString()))
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

        public string Eq_T
        {
            get
            {
                if (this.F03_TmpRetTime.Value.Date.Equals(Convert.ToDateTime(Constants.LastDummyDate).Date))
                {
                    return "days " + this.F03_TmpRetTime.Value.ToString("HH:mm");
                }
                else
                {
                    return this.F03_TmpRetTime.Value.Day + " days " + this.F03_TmpRetTime.Value.ToString("HH:mm");
                }
            }
        }
        public string T_Qty
        {
            get
            {
                var lstPdtMkp = _prePdtMkpDomain.GetAllByPreProduct(this.F03_PreProductCode);
                return (lstPdtMkp.Sum(i => i.F02_3FLayinAmount) + lstPdtMkp.Sum(i => i.F02_4FLayinAmount)).ToString("F");
            }
        }
    }
}
