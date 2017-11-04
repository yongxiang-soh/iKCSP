using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;

namespace KCSG.Data.Repositories
{
    public class OutOfPlanProductRepository : RepositoryBase<TX58_OutPlanPdt>
    {
        public OutOfPlanProductRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public void InsertOutPlanProduct(string preProductLotNo, string productCode, string productSeqNo, string productLotNo, string status, double tbtCmdEndPackAtm, double tbtCmdEndFrtAtm, double storageAmt, DateTime endDate, string certificationFlag)
        {
            var outPlanProduct = new TX58_OutPlanPdt();
            outPlanProduct.F58_PrePdtLotNo = preProductLotNo;
            outPlanProduct.F58_ProductCode=productCode;
            outPlanProduct.F58_PdtSeqNo = productSeqNo;
            outPlanProduct.F58_ProductLotNo = productLotNo;
            outPlanProduct.F58_Status = status;
            outPlanProduct.F58_TbtCmdEndPackAmt = tbtCmdEndPackAtm;
            outPlanProduct.F58_TbtCmdEndFrtAmt = tbtCmdEndFrtAtm;
            outPlanProduct.F58_TbtCmdEndAmt = tbtCmdEndFrtAtm;
            outPlanProduct.F58_StorageAmt = storageAmt;
            outPlanProduct.F58_TbtEndDate = endDate;
            outPlanProduct.F58_CertificationFlag = certificationFlag;
            outPlanProduct.F58_AddDate = DateTime.Now;
            outPlanProduct.F58_UpdateDate = DateTime.Now;
            outPlanProduct.F58_UpdateCount = 0;
            Add(outPlanProduct);

        }

        public void UpdateOutPlanProduct(TX58_OutPlanPdt outPlanProduct, string productLotNo, double tbtCmdEndPackAmt, double tbtCmdEndFrmAmt, DateTime endDate, string status, double storageAmt, string certificationFlag, DateTime certificationDate, DateTime shipDate)
        {
            outPlanProduct.F58_ProductLotNo = productLotNo;
            outPlanProduct.F58_TbtCmdEndPackAmt = tbtCmdEndPackAmt;
            outPlanProduct.F58_TbtCmdEndAmt = tbtCmdEndPackAmt;
            outPlanProduct.F58_TbtCmdEndFrtAmt = tbtCmdEndFrmAmt;
            outPlanProduct.F58_TbtEndDate = endDate;
            outPlanProduct.F58_UpdateDate = DateTime.Now;

            outPlanProduct.F58_Status = status;

            outPlanProduct.F58_StorageAmt = storageAmt;

            outPlanProduct.F58_CertificationFlag = certificationFlag;
            outPlanProduct.F58_CertificationDate = certificationDate;
            outPlanProduct.F58_ShipDate = shipDate;

            Update(outPlanProduct);
        }

        public void UpdateStatusInOutPlanProduct( TX58_OutPlanPdt outPlanProduct,string status)
        {
            outPlanProduct.F58_Status = status;
            outPlanProduct.F58_UpdateDate = DateTime.Now;
            Update(outPlanProduct);
        }

        public void UpdateStorageAmtInOutPlanProduct(TX58_OutPlanPdt outPlanProduct, double storageAmt)
        {
           
            outPlanProduct.F58_StorageAmt = storageAmt;
            outPlanProduct.F58_UpdateDate = DateTime.Now;
            Update(outPlanProduct);
        }

        public void UpdateCertificationFlagAndCertificationDate(TX58_OutPlanPdt outPlanProduct, string certificationFlag, DateTime certificationDate)
        {
           
            outPlanProduct.F58_CertificationFlag = certificationFlag;
            outPlanProduct.F58_CertificationDate = certificationDate;
            outPlanProduct.F58_UpdateDate = DateTime.Now;
            Update(outPlanProduct);
        }
    }
}
