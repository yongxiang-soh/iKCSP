using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCSG.Core.Constants;
using KCSG.Data.DataModel;
using KCSG.Domain.Models.Inquiry.ByWareHouseLocation;

namespace KCSG.Domain.Interfaces.Inquiry
{
    public interface IInquiryCommonDomain
    {
        string GetShelfStatus(Constants.InquirySearchConditionWarehouseLocation type,string row, string bay, string level);
        string GetShelftypeTx51(string row, string bay, string level);
        string GetLabelByStatus(Constants.InquirySearchConditionWarehouseLocation type, string status);
        void Clearshelf(Constants.InquirySearchConditionWarehouseLocation type, string row, string bay, string level, bool commit);
        void Setshelf(Constants.InquirySearchConditionWarehouseLocation type, string row, string bay, string level,string status, bool commit);
        List<TX51_PdtShfSts> GetAllPdtShfStsByShelfType(string shelftype);

        #region f032f
        /// <summary>
        /// get Data for Sub screen TCFC032F
        /// </summary>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        RawMaterialShelftStatusItem GetRawMaterialShelftStatusItem(string row, string bay, string level);

        /// <summary>
        /// get AcceptanceClassification for Sub screen TCFC032F
        /// </summary>
        /// <param name="prcordNo"></param>
        /// <param name="prtdvrNo"></param>
        /// <returns></returns>

        string GetAcceptanceClassification(string prcordNo, string prtdvrNo);
        void UpdateSubscreentcf032f(RawMaterialShelftStatusItem model, string termino);
        Tuple<bool, string> PalletNo032fExit(string palletno);
        Tuple<bool, string> MaterialCode032fExit(string prcordNo, string prtdvrNo, string material);
        Tuple<bool, string> CanUpdate(string palletNoNew, string palletNoCurrent, string materialCode, string prcordNo, string prtdvrNo);
        #endregion

        #region f034f

        string Getkndcmd(string precode, string prelot);
        /// <summary>
        /// get Data for Sub screen TCFC034F
        /// </summary>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        PreProductShelfStatusItem GetPreProductShelftStatusItem(string row, string bay, string level);
        Tuple<bool, string> CanUpdate034f(int seqNo);
        void UpdateSubscreentcf034f(PreProductShelfStatusItem model);
        #endregion

        #region f038f
        /// <summary>
        /// get Data for Sub screen TCFC038F
        /// </summary>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        InquiryBySupplierPalletItem GetInquiryBySupplierPalletItem(string row, string bay, string level);
        void UpdateSubscreentcf038f(InquiryBySupplierPalletItem model);
        #endregion

        #region f03Af
        /// <summary>
        /// get Data for Sub screen TCFC03AF
        /// </summary>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        InquiryByPreProductShelfStatusEmptyItem GetInquiryByPreProductShelfStatusEmptyItem(string row, string bay, string level);
        void UpdateSubscreentcf03Af(InquiryByPreProductShelfStatusEmptyItem model);
        #endregion

        #region f036f
        /// <summary>
        /// get Data for Sub screen TCFC036F
        /// </summary>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        InquiryByProductShelfStatusItem GetInquiryByProductShelfStatusItem(string row, string bay, string level);
        string GetProductLotNo036f(string productcode, string preProductLotNo, int productClassification);
        double GetPackingUnit036f(string productcode);
        void UpdateSubscreentcf036f(InquiryByProductShelfStatusItem model);
        bool ProductLotNoNotbeentabletised(string productcode, string preProductLotNo, int productClassification);
        bool PalletNoExit(string palletNo);

        #endregion

        #region f037f
        /// <summary>
        /// get Data for Sub screen TCFC037F
        /// </summary>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        InquiryByProductShelfStatusExternalPreProductItem GetProductShelftStatusExternalPreProductItem(string row, string bay, string level);
        void UpdateSubscreentcf037f(InquiryByProductShelfStatusExternalPreProductItem model);
        bool PreProductIsexternal(string precode, string lotno);
        bool PalletSeqNoExit(int sepno);

        #endregion

        #region f03Bf
        /// <summary>
        /// get Data for Sub screen TCFC03BF
        /// </summary>
        /// <param name="row"></param>
        /// <param name="bay"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        InquiryByProductShelfStatusEmptyItem GetInquiryByProductShelfStatusEmptyItem(string row, string bay, string level);
        void UpdateSubscreentcf03Bf(InquiryByProductShelfStatusEmptyItem model);
        #endregion
    }
}
