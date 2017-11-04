using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlTypes;
using System.Threading.Tasks;
using KCSG.Data.DataModel;

namespace KCSG.Data.DatabaseContext
{
    public class KCSGDbContext : DbContext, IKCSGDbContext
    {
        public KCSGDbContext() : base("Name=KCSGConnectionString") { }

        public DbSet<TM01_Material> Materials { get; set; }
        public DbSet<TM09_Product> Products { get; set; }
        public DbSet<TM02_PrePdtMkp> PrePdtMkps { get; set; }
        public DbSet<TM03_PreProduct> PreProducts { get; set; }
        public DbSet<TX39_PdtPln> PdtPlns { get; set; }
        public DbSet<TM11_PckMtr> PckMtrs { get; set; }
        public DbSet<TM15_SubMaterial> SubMaterials { get; set; }

        public DbSet<TX30_Reception> Receptions { get; set; }

        public DbContext Context
        {
            get { return this; }
        }

        /// <summary>
        /// Find stock-taking pre-product by using specific conditions.
        /// </summary>
        /// <param name="fromShelfNo"></param>
        /// <param name="toShelfNo"></param>
        /// <param name="page"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        public virtual ObjectResult<FindStockTakingPreProduct_Result> FindStockTakingPreProduct(Nullable<int> fromShelfNo, Nullable<int> toShelfNo, string f37_stocktakingflag, string f37_shelfStatus)
        {
            var fromShelfNoParameter = fromShelfNo.HasValue ?
                new ObjectParameter("fromShelfNo", fromShelfNo) :
                new ObjectParameter("fromShelfNo", typeof(int));

            var toShelfNoParameter = toShelfNo.HasValue ?
                new ObjectParameter("toShelfNo", toShelfNo) :
                new ObjectParameter("toShelfNo", typeof(int));

            var f37_stocktakingflagParameter = f37_stocktakingflag != null ?
                new ObjectParameter("f37_stocktakingflag", f37_stocktakingflag) :
                new ObjectParameter("f37_stocktakingflag", typeof(string));

            var f37_shelfStatusParameter = f37_shelfStatus != null ?
                new ObjectParameter("f37_shelfStatus", f37_shelfStatus) :
                new ObjectParameter("f37_shelfStatus", typeof(string));

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<FindStockTakingPreProduct_Result>("FindStockTakingPreProduct", fromShelfNoParameter, toShelfNoParameter, f37_stocktakingflagParameter, f37_shelfStatusParameter);
        }
        public virtual ObjectResult<GetStockTakingPreProductByShelf_Result1> GetStockTakingPreProductByShelf(Nullable<int> shelfNo)
        {
            var shelfNoParameter = shelfNo.HasValue ?
                new ObjectParameter("ShelfNo", shelfNo) :
                new ObjectParameter("ShelfNo", typeof(int));

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetStockTakingPreProductByShelf_Result1>("GetStockTakingPreProductByShelf", shelfNoParameter);
        }
        public virtual ObjectResult<TX37_PrePdtShfSts> FindPrePdtShfSts(string location)
        {
            var locationParameter = location != null ?
                new ObjectParameter("location", location) :
                new ObjectParameter("location", typeof(string));

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<TX37_PrePdtShfSts>("FindPrePdtShfSts", locationParameter);
        }
        public virtual ObjectResult<TX51_PdtShfSts> GetPdtShfSts(string location)
        {
            var locationParameter = location != null ?
                new ObjectParameter("location", location) :
                new ObjectParameter("location", typeof(string));

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<TX51_PdtShfSts>("GetPdtShfSts", locationParameter);
        }
        public virtual int DeleteMonthlyProcess(Nullable<System.DateTime> startDate, string notSotoked)
        {
            var startDateParameter = startDate.HasValue ?
                new ObjectParameter("startDate", startDate) :
                new ObjectParameter("startDate", typeof(System.DateTime));

            var notSotokedParameter = notSotoked != null ?
                new ObjectParameter("notSotoked", notSotoked) :
                new ObjectParameter("notSotoked", typeof(string));

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DeleteMonthlyProcess", startDateParameter, notSotokedParameter);
        }
    }
}
