using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Data.DatabaseContext
{
    public interface IKCSGDbContext
    {
        DbSet<T> Set<T>() where T : class;

        /// <summary>
        /// Save changes into database synchronously.
        /// </summary>
        int SaveChanges();

        /// <summary>
        /// Accessor of database context.
        /// </summary>
        DbContext Context { get; }

        /// <summary>
        /// Save changes into database asynchronously.
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Find stock taking pre-products by using specific conditions.
        /// </summary>
        /// <param name="fromShelfNo"></param>
        /// <param name="toShelfNo"></param>
        /// <param name="f37_stocktakingflag"></param>
        /// <param name="f37_shelfStatus"></param>
        /// <returns></returns>
        ObjectResult<FindStockTakingPreProduct_Result> FindStockTakingPreProduct(Nullable<int> fromShelfNo,
            Nullable<int> toShelfNo, string f37_stocktakingflag, string f37_shelfStatus);

        ObjectResult<GetStockTakingPreProductByShelf_Result1> GetStockTakingPreProductByShelf(Nullable<int> shelfNo);
        ObjectResult<TX37_PrePdtShfSts> FindPrePdtShfSts(string location);
        ObjectResult<TX51_PdtShfSts> GetPdtShfSts(string location);
        int DeleteMonthlyProcess(Nullable<System.DateTime> startDate, string notSotoked);
    }
}
