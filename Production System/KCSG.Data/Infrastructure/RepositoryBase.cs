using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using KCSG.Data.DatabaseContext;

namespace KCSG.Data.Infrastructure
{
    public abstract class
        RepositoryBase<T> where T : class
    {
        #region Constructor

        protected RepositoryBase(IKCSGDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        #endregion

        #region Protected Fields

        public IKCSGDbContext Context { get; private set; }
        protected readonly IDbSet<T> DbSet;

        #endregion

        #region Implementation Methods

        public void Add(T entity)
        {
            try
            {
                DbSet.Add(entity);
                Context.Context.Entry(entity).State = EntityState.Added;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public T Update(T entity)
        {
            //DbSet.Attach(entity);
            Context.Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }

        public void Delete(Expression<Func<T, bool>> where)
        {
            var lstItem = DbSet.Where(where);
            foreach (var item in lstItem)
                DbSet.Remove(item);
        }

        public async Task Deletes(Expression<Func<T, bool>> where)
        {
            await DbSet.Where(where).ForEachAsync(cs => DbSet.Remove(cs));
        }

        public T GetById(string id)
        {
            try
            {
                return DbSet.Find(id);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return DbSet.Where(where).FirstOrDefault();
        }

        public IQueryable<T> GetAll()
        {
            return DbSet.AsQueryable();
        }

        public IQueryable<T> GetMany(Expression<Func<T, bool>> where)
        {
            return DbSet.Where(where).AsQueryable();
        }

        public T Get(Guid key)
        {
            return Context.Set<T>().Find(key);
        }

        public TEntity AddOrUpdate<TEntity>(TEntity entity)
            where TEntity : class
        {
            var tracked = Context.Set<TEntity>().Find(KeyValuesFor(Context.Context, entity));
            if (tracked != null)
            {
                Context.Context.Entry(tracked).CurrentValues.SetValues(entity);
                return tracked;
            }

            Context.Context.Set<TEntity>().Add(entity);
            return entity;
        }

        private IEnumerable<string> KeysFor(DbContext context, Type entityType)
        {
            Contract.Requires(context != null);
            Contract.Requires(entityType != null);

            entityType = ObjectContext.GetObjectType(entityType);

            var metadataWorkspace =
                ((IObjectContextAdapter) context).ObjectContext.MetadataWorkspace;
            var objectItemCollection =
                (ObjectItemCollection) metadataWorkspace.GetItemCollection(DataSpace.OSpace);

            var ospaceType = metadataWorkspace
                .GetItems<EntityType>(DataSpace.OSpace)
                .SingleOrDefault(t => objectItemCollection.GetClrType(t) == entityType);

            if (ospaceType == null)
                throw new ArgumentException(
                    string.Format(
                        "The type '{0}' is not mapped as an entity type.",
                        entityType.Name),
                    "entityType");

            return ospaceType.KeyMembers.Select(k => k.Name);
        }

        private object[] KeyValuesFor(DbContext context, object entity)
        {
            Contract.Requires(context != null);
            Contract.Requires(entity != null);

            var entry = context.Entry(entity);
            return KeysFor(Context.Context, entity.GetType())
                .Select(k => entry.Property(k).CurrentValue)
                .ToArray();
        }

        #endregion
    }
}