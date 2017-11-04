using System;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Domains.ProductionPlanning;

namespace KCSG.Domain.Models.ProductManagement
{
    public class ProductCertificationItem :  TH67_CrfHst
    {
        public bool IsCreate { get; set; }
        public string F09_ProductDesp { get; set; }

        public string CertificationDate { get; set; }
    }
}
