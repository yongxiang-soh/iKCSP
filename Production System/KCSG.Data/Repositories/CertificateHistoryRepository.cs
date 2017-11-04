using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;

namespace KCSG.Data.Repositories
{
    public class CertificateHistoryRepository : RepositoryBase<TH67_CrfHst>
    {
        public CertificateHistoryRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public void InsertCertificateHistory(string productCode, string preProductLotNo, string productFlg, double amount, string productNotLo, string certificationFlag, DateTime certificationDate)
        {
            var certificateHistory = new TH67_CrfHst();
            certificateHistory.F67_ProductCode = productCode;
            certificateHistory.F67_PrePdtLotNo = preProductLotNo;
            certificateHistory.F67_ProductFlg = productFlg;
            certificateHistory.F67_Amount = amount;
            certificateHistory.F67_ProductLotNo = productNotLo;
            certificateHistory.F67_CertificationFlag = certificationFlag;
            certificateHistory.F67_CertificationDate = certificationDate;
            certificateHistory.F67_AddDate = DateTime.Now;
            certificateHistory.F67_UpdateDate = DateTime.Now;
            certificateHistory.F67_UpdateCount = 0;

            Add(certificateHistory);

        }
    }
}
