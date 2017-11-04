using System;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Domains.ProductionPlanning;
using KCSG.Domain.Interfaces.Services;

namespace KCSG.Domain.Models.ProductionPlanning
{
    public class PdtPlnItem : TX39_PdtPln
    {
        
        public bool IsCreate { get; set; }
        public string PreProductName
        {
            get
            {
                try
                {
                    var configurationService = DependencyResolver.Current.GetService<IConfigurationService>();
                    var preproduct =
                        new PreProductDomain(new UnitOfWork(new KCSGDbContext()), configurationService).GetById(
                            F39_PreProductCode);
                    return preproduct != null ? preproduct.F03_PreProductName : "";
                }
                catch
                {
                    return "";
                }
            }

        }

        public string Color
        {
            get
            {
                if (string.IsNullOrEmpty(this.F39_ColorClass))
                {
                    return "";
                }
                return this.F39_ColorClass == "0" ? "B" : "C";
            }
        }

        public string F39_PreProductName { get; set; }
        public string F03_PreProductName { get; set; }

        public TM03_PreProduct PreProduct { get; set; }

        //Add field for kneading command
        public int Quantity { get; set; }
        public double MaterialAmount { get; set; }
        public double YieldDrate { get; set; }

        //
        public string CmdNo { get; set; }
        public string LotNo { get; set; }
        public int CommandSequenceNo { get; set; }
        public int KndLine { get; set; }

        public string Status { get { return Enum.GetName(typeof(Constants.F39_Status), Convert.ToInt32(this.F39_Status)); } }
        public string StatusYet { get { return Enum.GetName(typeof(Constants.Status), Convert.ToInt32(this.F39_Status)); } }

    }

    public class PdtPlnItem1 : TX39_PdtPln
    {

        public bool IsCreate { get; set; }
        //public string PreProductName
        //{
        //    get
        //    {   
        //        var preproduct = new PreProductDomain(new UnitOfWork(new KCSGDbContext())).GetById(this.F39_PreProductCode);
        //        return preproduct != null ? preproduct.F03_PreProductName : "";
        //    }

        //}
        public string F39_PreProductName { get; set; }
        public string F03_PreProductName { get; set; }

        public TM03_PreProduct PreProduct { get; set; }

        //Add field for kneading command
        public int Quantity { get; set; }
        public double MaterialAmount { get; set; }
        public double YieldDrate { get; set; }

        //
        public string CmdNo { get; set; }
        public string LotNo { get; set; }
        public int CommandSequenceNo { get; set; }
        public int KndLine { get; set; }

       
    }
}
