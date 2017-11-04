using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using KCSG.Core.Constants;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.EnvironmentManagement;
using KCSG.Domain.Interfaces.ProductCertificationManagement;
using KCSG.Domain.Models.EnvironmentManagement;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Domains.EnvironmentManagement
{
    public class ProductMasterManagementDomain : BaseDomain, IProductMasterManagementDomain
    {
        public ProductMasterManagementDomain(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        public IList<TM09_Product> GetProducts()
        {
            var lstF85Code = _unitOfWork.EnvProdRepository.GetAll().Select(i => i.F85_Code);
            var products = _unitOfWork.ProductRepository.GetMany(i => !lstF85Code.Contains(i.F09_ProductCode));
            return products.ToList();
        }


        public ResponseResult<GridResponse<ProductMasterManagementItem>> SearchCriteria(string location,
            jsGrid.MVC.GridSettings gridSettings)
        {
            var f80Type = Constants.TypeOfTable.CALC_TE81_TEMP.ToString("D");
            var teEnvMesp =
                _unitOfWork.EnvMespRepository.GetMany(
                    i => i.F80_Name.Trim().Equals(location.Trim()) && i.F80_Type.Equals(f80Type));

            if (!teEnvMesp.Any())
                return null;

            var te85EnvProds = _unitOfWork.EnvProdRepository.GetAll();
            var products = _unitOfWork.ProductRepository.GetAll();
            var te80EnvMesps = _unitOfWork.EnvMespRepository.GetAll();

            var lstResult = from te85EnvProd in te85EnvProds
                from product in products
                from te80EnvMesp in te80EnvMesps
                where (
                    te85EnvProd.F85_Code == product.F09_ProductCode &&
                    te85EnvProd.F85_Type == te80EnvMesp.F80_Type &&
                    te85EnvProd.F85_Id == te80EnvMesp.F80_Id &&
                    te80EnvMesp.F80_Name == location
                    )
                select new ProductMasterManagementItem()
                {
                    F85_Code = te85EnvProd.F85_Code,
                    F85_M_Usl = te85EnvProd.F85_M_Usl,
                    F85_M_Lsl = te85EnvProd.F85_M_Lsl,
                    F85_M_Ucl = te85EnvProd.F85_M_Ucl,
                    F85_M_Lcl = te85EnvProd.F85_M_Lcl,
                    F85_R_Usl = te85EnvProd.F85_R_Usl,
                    F85_R_Lsl = te85EnvProd.F85_R_Lsl,
                    F85_No_Lot = te85EnvProd.F85_No_Lot,
                    F85_Type = te85EnvProd.F85_Type,
                    F85_Id = te85EnvProd.F85_Id,
                    F85_From = te85EnvProd.F85_From,
                    F85_To = te85EnvProd.F85_To,
                    ProductName = product.F09_ProductDesp
                };

            var itemCount = lstResult.Count();
            OrderByAndPaging(ref lstResult, gridSettings);

            var resultModel = new GridResponse<ProductMasterManagementItem>(lstResult, itemCount);
            return new ResponseResult<GridResponse<ProductMasterManagementItem>>(resultModel, true);
        }


       

        public void Add(ProductMasterManagementItem item)
        {
            var te85EnvProd = new Te85_Env_Prod();
            te85EnvProd.F85_Code = item.NewProductName;

            te85EnvProd.F85_M_Usl = item.USLMean;
            te85EnvProd.F85_M_Ucl = item.UCLMean;
            te85EnvProd.F85_M_Lsl = item.LSLMean;
            te85EnvProd.F85_M_Lcl = item.LCLMean;

            te85EnvProd.F85_R_Usl = item.USLRange;
            te85EnvProd.F85_R_Lsl = item.LSLRange;

            te85EnvProd.F85_No_Lot = item.NoOFLot;

            te85EnvProd.F85_From = DateTime.Now;
            te85EnvProd.F85_To = DateTime.Now;

            te85EnvProd.F85_Type = Constants.EnvType.TYPE_RM.ToString("D");

            te85EnvProd.F85_Id = Int32.Parse(item.Location);

            _unitOfWork.EnvProdRepository.Add(te85EnvProd);

            _unitOfWork.Commit();
        }

        public bool CheckLocation(int id)
        {
            var result = _unitOfWork.EnvMespRepository.GetMany(i => i.F80_Id.Equals(id) && i.F80_Type.Equals("1"));
            return result.Any();
        }

        public bool CheckValueEntered(int locationId)
        {
            var te85EnvProds = _unitOfWork.EnvProdRepository.GetAll();
            var products = _unitOfWork.ProductRepository.GetAll();
            var te80EnvMesps = _unitOfWork.EnvMespRepository.GetAll();
            var results = from te85EnvProd in te85EnvProds
                from product in products
                from te80EnvMesp in te80EnvMesps
                where te85EnvProd.F85_Code.Equals(product.F09_ProductCode) &&
                      te85EnvProd.F85_Type.Equals(te80EnvMesp.F80_Type) &&
                      te85EnvProd.F85_Id==te80EnvMesp.F80_Id &&
                      te80EnvMesp.F80_Id==locationId && te80EnvMesp.F80_Type.Equals("1")
                select new
                {
                   
                    te85EnvProd.F85_Code,
                    product.F09_ProductDesp,
                    te85EnvProd.F85_M_Usl,
                    te85EnvProd.F85_M_Lsl,
                    te85EnvProd.F85_M_Ucl,
                    te85EnvProd.F85_M_Lcl,
                    te85EnvProd.F85_R_Usl,
                    te85EnvProd.F85_R_Lsl,
                    te85EnvProd.F85_No_Lot,
                    te85EnvProd.F85_Type,
                    te85EnvProd.F85_Id,
                    te85EnvProd.F85_From,
                    te85EnvProd.F85_To
                };

            if(!results.Any())
                return false;
            var item = results.FirstOrDefault();
            if (item.F09_ProductDesp == null || item.F85_Code == null || item.F85_M_Usl == null ||
                item.F85_M_Lsl == null || item.F85_M_Ucl == null || item.F85_M_Lcl == null || item.F85_R_Usl == null ||
                item.F85_R_Lsl == null || item.F85_No_Lot == null || item.F85_Type == null || item.F85_Id == null ||
                item.F85_From == null || item.F85_To == null)
            {
                return false;
            }
            return true;
        }

        public void Edit(ProductMasterManagementItem item)
        {
            var te85EnvProd = _unitOfWork.EnvProdRepository.Get(i => i.F85_Code.Equals(item.F85_Code));

            te85EnvProd.F85_M_Usl = item.USLMean;
            te85EnvProd.F85_M_Ucl = item.UCLMean;
            te85EnvProd.F85_M_Lsl = item.LSLMean;
            te85EnvProd.F85_M_Lcl = item.LCLMean;

            te85EnvProd.F85_R_Usl = item.USLRange;
            te85EnvProd.F85_R_Lsl = item.LSLRange;

            te85EnvProd.F85_No_Lot = item.NoOFLot;

            _unitOfWork.EnvProdRepository.Update(te85EnvProd);

            _unitOfWork.Commit();
        }

        public void Delete(string code)
        {
            _unitOfWork.EnvProdRepository.Delete(i => i.F85_Code.Equals(code));
            _unitOfWork.Commit();
        }
    }
}