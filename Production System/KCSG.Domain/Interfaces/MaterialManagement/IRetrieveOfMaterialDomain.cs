using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KCSG.Core.Models;
using KCSG.Data.DataModel;
using KCSG.Domain.Models;
using KCSG.Domain.Models.MaterialManagement;
using KCSG.Domain.Models.ProductionPlanning;
using KCSG.jsGrid.MVC;

namespace KCSG.Domain.Interfaces.MaterialManagement
{
    public interface IRetrieveOfMaterialDomain
    {
        /// <summary>
        /// Find Material Shelf Status
        /// </summary>
        /// <param name="materialCode"></param>
        /// <returns></returns>
        Task<IQueryable<TX31_MtrShfSts>> FindMaterialShelfStatusesAsync(string materialCode);

        /// <summary>
        /// Find Material Shelf Status = 5
        /// </summary>
        /// <param name="materialCode"></param>
        /// <returns></returns>
        Task<IQueryable<TX31_MtrShfSts>> FindMaterialShelfStatusesForDetailAsync(string materialCode);

        /// <summary>
        /// Retrieve Or Assign Pallet
        /// </summary>
        /// <param name="materialCode"></param>
        /// <param name="quantity"></param>
        /// <param name="terminalNo"></param>

        /// <returns></returns>
        Task<ResponseResult<AssignPalletItem>> RetrieveOrAssignPallet(string materialCode, double quantity, string terminalNo);

        /// <summary>
        /// Unassign a list of pallets by using specific conditions.
        /// </summary>
        /// <param name="materialCode"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        Task UnassignPalletsList(string materialCode, string terminalNo);

        /// <summary>
        /// Find a list of pallets by using specific conditions.
        /// </summary>
        /// <param name="materialCode"></param>
        /// <param name="quantity"></param>
        /// <param name="terminalNo"></param>
        /// <param name="gridSettings"></param>
        /// <returns></returns>
        Task<ResponseResult<GridResponse<PalletGridDetail>>> FindPalletsList(string materialCode, double quantity, string terminalNo, GridSettings gridSettings);

        ///// <summary>
        ///// Find a list of details of a pallet by using specific conditions.
        ///// </summary>
        ///// <param name="palletNo"></param>
        ///// <returns></returns>
        //Task<IList<TX33_MtrShfStk>> FindPalletDetails(string palletNo);

        /// <summary>
        /// Unassign a specific pallet by using specific conditions for search.
        /// </summary>
        /// <param name="shelfRow"></param>
        /// <param name="shelfBay"></param>
        /// <param name="shelfLevel"></param>
        /// <returns></returns>
        Task UnassignSpecificPallet(string shelfRow, string shelfBay, string shelfLevel);

        /// <summary>
        /// Retrieve material by using specific conditions.
        /// </summary>
        /// <param name="materialCode"></param>
        /// <param name="quantity"></param>
        /// <param name="terminalNo"></param>
        /// <returns></returns>
        Task RetrieveMaterial(string materialCode, double quantity, string terminalNo);

        /// <summary>
        /// Caculate tally
        /// </summary>
        /// <param name="materialCode"></param>
        /// <returns></returns>
        Task<double> ReCalculateTally(string materialCode);

        /// <summary>
        /// Post-Retrieve Material
        /// </summary>
        /// <param name="terminalNo"></param>
        /// <param name="materialCode"></param>
        /// <returns></returns>
        Task<IList<FirstCommunicationResponse>> PostRetrieveMaterial(string terminalNo, string materialCode);


        Task<bool> CheckAssignPallet(string materialCode);
    }
}
