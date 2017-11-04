using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using KCSG.Core.Constants;
using KCSG.Data.DatabaseContext;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;

namespace KCSG.Data.Repositories
{
    public class ProductShelfStockRepository : RepositoryBase<TX40_PdtShfStk>
    {
        public ProductShelfStockRepository(IKCSGDbContext context)
            : base(context)
        {
        }

        public void InsertProductShelfStock(string palletNo, string preProductLotNo, string productCode,
            string productLotNo, string stockFlag, int packageAmount, double fraction, double amount,
            DateTime tabletingEndDate, string shipCommandNo, double assignAmount, string certificationFlg,
            DateTime? certificationDate, DateTime addDate)
        {
            if (!string.IsNullOrEmpty(palletNo) && !string.IsNullOrEmpty(preProductLotNo) &&
                !string.IsNullOrEmpty(preProductLotNo))
            {
                if (certificationDate == DateTime.MinValue)
                {
                    certificationDate = null;
                }
                var productShelfStock = new TX40_PdtShfStk();
                productShelfStock.F40_PalletNo = palletNo;
                productShelfStock.F40_PrePdtLotNo = preProductLotNo;
                productShelfStock.F40_ProductCode = productCode;
                productShelfStock.F40_ProductLotNo = productLotNo;
                productShelfStock.F40_StockFlag = stockFlag;
                productShelfStock.F40_PackageAmount = packageAmount;
                productShelfStock.F40_Fraction = fraction;
                productShelfStock.F40_Amount = amount;
                productShelfStock.F40_TabletingEndDate = tabletingEndDate;
                productShelfStock.F40_ShipCommandNo = shipCommandNo;
                productShelfStock.F40_ShippedAmount = 0;
                productShelfStock.F40_AssignAmount = assignAmount;
                productShelfStock.F40_CertificationFlg = certificationFlg;
                productShelfStock.F40_CertificationDate = certificationDate;
                productShelfStock.F40_AddDate = addDate;
                productShelfStock.F40_UpdateDate = DateTime.Now;
                productShelfStock.F40_UpdateCount = 0;

                Add(productShelfStock);
            }
        }

        public void InsertProductShelfStock(string palletNo, string preProductLotNo, string productCode,
            string productLotNo, string stockFlag, int packageAmount, double fraction, double amount,
            DateTime tabletingEndDate, string certificationFlg)
        {
            if (!string.IsNullOrEmpty(palletNo) && !string.IsNullOrEmpty(preProductLotNo) &&
                !string.IsNullOrEmpty(preProductLotNo))
            {
                var productShelfStock = new TX40_PdtShfStk();
                productShelfStock.F40_PalletNo = palletNo;
                productShelfStock.F40_PrePdtLotNo = preProductLotNo;
                productShelfStock.F40_ProductCode = productCode;
                productShelfStock.F40_ProductLotNo = productLotNo;
                productShelfStock.F40_StockFlag = stockFlag;
                productShelfStock.F40_PackageAmount = packageAmount;
                productShelfStock.F40_Fraction = fraction;
                productShelfStock.F40_Amount = amount;

                productShelfStock.F40_TabletingEndDate = tabletingEndDate != DateTime.MinValue ? tabletingEndDate : DateTime.Now;

                productShelfStock.F40_CertificationFlg = certificationFlg;
                productShelfStock.F40_AddDate = DateTime.Now;
                productShelfStock.F40_UpdateDate = DateTime.Now;
                productShelfStock.F40_UpdateCount = 0;

                Add(productShelfStock);
            }
        }


        public void UpdateProductShelfStock(TX40_PdtShfStk productShelfStock, string certificationFlg,
            DateTime certificationDate)
        {
            productShelfStock.F40_CertificationFlg = certificationFlg;
            productShelfStock.F40_CertificationDate = certificationDate;

            Update(productShelfStock);
        }

        public void UpdateProductShelfStock(TX40_PdtShfStk productShelfStock, string stockFlag, string shipCommandNo,
            double assignAmount)
        {
            productShelfStock.F40_StockFlag = stockFlag;
            productShelfStock.F40_ShipCommandNo = shipCommandNo;
            productShelfStock.F40_AssignAmount = assignAmount;
            productShelfStock.F40_UpdateDate = DateTime.Now;

            Update(productShelfStock);
        }

        public void UpdateProductShelfStock(TX40_PdtShfStk productShelfStock, string stockFlag)
        {
            productShelfStock.F40_StockFlag = stockFlag;
            productShelfStock.F40_UpdateDate = DateTime.Now;
        }

        public void UpdateProductShelfStock(TX40_PdtShfStk productShelfStock,bool addShipping)
        {
            if (addShipping )
            {
                productShelfStock.F40_ShippedAmount += productShelfStock.F40_AssignAmount.Value;
            }
            productShelfStock.F40_ShipCommandNo = string.Empty;
            productShelfStock.F40_AssignAmount = 0;
            productShelfStock.F40_UpdateDate = DateTime.Now;

            Update(productShelfStock);
        }

        public IQueryable<TX40_PdtShfStk> GetByPalletNo(string palletNo)
        {
            return GetAll().Where(i => i.F40_PalletNo.Trim().Equals(palletNo.Trim()));
        }
    }
}