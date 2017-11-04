using KCSG.Core.Models;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.MaterialManagement
{
    public interface IAcceptanceOfMaterialDomain
    {
        /// <summary>
        /// Find raw material by using P.O and partial delivery.
        /// 	If [P.O. No.] length is less than 7 characters, the system will search for Material Reception records then display list of Material Reception records whose [P.O. No.] start with [P.O. No.]. 
        /// If not, the system will display list of Material Reception records whose [P.O. No.] are equal to [P.O. No.].
        /// 	If [Partial Delivery] length is less than 2 characters, the system will search for Material Reception records then display list of Material Reception records whose [Partial Delivery] start with [Partial Delivery]. 
        /// If not, the system will display list of Material Reception records whose [Partial Delivery] are equal to [Partial Delivery]
        /// </summary>
        /// <param name="gridSettings"></param>
        /// <param name="pNo"></param>
        /// <param name="partialDelivery"></param>
        ResponseResult<GridResponse<object>> SearchRawMaterial(GridSettings gridSettings, string pNo, string partialDelivery);

        /// <summary>
        /// This function is for accepting a raw material by searching its pNo and partialDelivery information.
        /// </summary>
        /// <param name="pNo"></param>
        /// <param name="partialDelivery"></param>
        void AcceptRawMaterial(string pNo, string partialDelivery);
        
        /// <summary>
        /// This function is for rejecting a raw material by searching its pNo and partialDelivery information.
        /// </summary>
        /// <param name="pNo"></param>
        /// <param name="partialDelivery"></param>
        void RejectRawMaterial(string pNo, string partialDelivery);

    }
}