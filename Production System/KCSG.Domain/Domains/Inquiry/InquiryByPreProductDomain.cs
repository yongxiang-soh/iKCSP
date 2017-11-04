using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Core.Helper;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Data.Infrastructure;
using KCSG.Domain.Interfaces.Inquiry;
using KCSG.Domain.Models.Inquiry;
using KCSG.Data.Repositories;
using KCSG.Domain.Interfaces.Services;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Domain.Models.PreProductManagement;
using KCSG.jsGrid.MVC;
using log4net;

namespace KCSG.Domain.Domains.Inquiry
{
    public class InquiryByPreProductDomain : BaseDomain, IInquiryByPreProductDomain
    {
        #region Constructor

        public InquiryByPreProductDomain(IUnitOfWork unitOfWork, IConfigurationService configurationService)
            : base(unitOfWork, configurationService)
        {
            _unitOfWork = unitOfWork;
        }

        #endregion
        public ResponseResult<GridResponse<SearchInquiryByPreProductCode>> SearchCriteria(string preProductCode, string lotNo,
    GridSettings gridSettings, out double total)
        {
            var materialLotNoR = _unitOfWork.KneadingRecordRepository.GetAll();
            var materialDetailss = _unitOfWork.MaterialRepository.GetAll();
            var materialLotNoC = _unitOfWork.KneadingCommandRepository.GetAll();

            var result1 = from rs1 in materialLotNoR
                         join cs1 in materialLotNoC on new { test1 = rs1.F43_KndCmdNo, test2 = rs1.F43_PrePdtLotNo } equals
                         new { test1 = cs1.F42_KndCmdNo, test2 = cs1.F42_PrePdtLotNo }
                         select new
                         {
                             rs1.F43_MaterialLotNo,
                             rs1.F43_KndCmdNo,
                             rs1.F43_LayinginAmount,
                             rs1.F43_MaterialCode,
                             rs1.F43_PrePdtLotNo,
                             cs1.F42_PreProductCode
                         }
                ;
            var result = from rs2 in result1
                join rs3 in materialDetailss on rs2.F43_MaterialCode equals rs3.F01_MaterialCode
                select new SearchInquiryByPreProductCode
                {
                    F01_Materialdsp = rs3.F01_MaterialDsp,
                    F43_KndCmdNo = rs2.F43_KndCmdNo,
                    F43_LayinginAmount = rs2.F43_LayinginAmount,
                    F43_MaterialCode = rs2.F43_MaterialCode,
                    F42_PreProductCode = rs2.F42_PreProductCode,
                    F43_MaterialLotNo = rs2.F43_MaterialLotNo,
                    F43_PrePdtLotNo = rs2.F43_PrePdtLotNo,
                };


            if (!string.IsNullOrEmpty(preProductCode) || !string.IsNullOrEmpty(lotNo))
            {
                result = result.Where(x => x.F42_PreProductCode == preProductCode && x.F43_PrePdtLotNo == lotNo);
                var itemCount = result.Count();
                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);
                try
                {

                    total = result.Sum(i => i.F43_LayinginAmount);
                }
                catch (Exception ex)
                {

                    total = 0;
                }
                var resultModel = new GridResponse<SearchInquiryByPreProductCode>(result, itemCount);
                return new ResponseResult<GridResponse<SearchInquiryByPreProductCode>>(resultModel, true);
            }
            else
            {
                //var result = from materialShelfStatus in materialLotNoss
                //             join materialShelfStock in materialDetailss on materialShelfStatus.F31_PalletNo equals materialShelfStock.F33_PalletNo

                //             where materialShelfStatus.F31_ShelfStatus.Trim().Equals(material)
                //             orderby new
                //             {
                //                 materialShelfStock.F33_MaterialLotNo,
                //                 materialShelfStatus.F31_ShelfBay,
                //                 materialShelfStatus.F31_ShelfRow
                //             }
                //             select
                //             new MaterialShelfStatusItem
                //             {
                //                 F33_MaterialLotNo = materialShelfStock.F33_MaterialLotNo,
                //                 F33_Amount = materialShelfStock.F33_Amount,
                //                 F31_ShelfRow = materialShelfStatus.F31_ShelfRow,
                //                 F31_ShelfBay = materialShelfStatus.F31_ShelfBay,
                //                 F31_ShelfLevel = materialShelfStatus.F31_ShelfLevel,
                //                 ShelfNo1 = materialShelfStatus.F31_ShelfRow + "-" + materialShelfStatus.F31_ShelfBay + "-" + materialShelfStatus.F31_ShelfLevel,
                //             };

                var itemCount = result.Count();
                if (gridSettings != null)
                    OrderByAndPaging(ref result, gridSettings);
                 try
                {

                    total = result.Sum(i => i.F43_LayinginAmount);
                }
                catch (Exception ex)
                {

                    total = 0;
                }
                var resultModel = new GridResponse<SearchInquiryByPreProductCode>(result, itemCount);
                return new ResponseResult<GridResponse<SearchInquiryByPreProductCode>>(resultModel, true);
            }
            //var itemCount = lstResult.Count();
            //var pdtPlnItems = lstResult.AsQueryable();
            //OrderByAndPaging(ref pdtPlnItems, gridSettings);
            //var resultModel = new GridResponse<PdtPlnItem>(pdtPlnItems, itemCount);
            //return new ResponseResult<GridResponse<PdtPlnItem>>(resultModel, true);
        }

    }
}
