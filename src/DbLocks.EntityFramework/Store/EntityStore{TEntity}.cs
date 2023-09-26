namespace DbLocks.EntityFramework.Store
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DbLocks.EntityFramework.Entities;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Ef entity store.
    /// </summary>
    /// <typeparam name="TEntity">Entity type..</typeparam>
    internal class EntityStore<TEntity>
        where TEntity : class, IBaseEntity
    {
        /// <summary>
        /// Creates new <see cref="EntityStore{TEntity}"/> using  <see cref="DbContext"/>.
        /// </summary>
        /// <param name="context">Контекст базы данных.</param>
        public EntityStore(DbContext context)
        {
            this.Context = context;
            this.DbEntitySet = context.Set<TEntity>();
        }

        /// <summary>
        /// Gets DbContext.
        /// </summary>
        public DbContext Context { get; private set; }

        /// <summary>
        /// Gets Entity collection.
        /// </summary>
        public IQueryable<TEntity> EntitySet => this.DbEntitySet;

        /// <summary>
        /// Gets Entity collection as No Tracking.
        /// </summary>
        public IQueryable<TEntity> EntitySetNoTracking => this.DbEntitySet.AsNoTracking();

        /// <summary>
        /// Gets EntitySet for Store.
        /// </summary>
        public DbSet<TEntity> DbEntitySet { get; private set; }

        /// <summary>
        /// Finds entity by id.
        /// </summary>
        /// <param name="id">
        /// Entity Id.
        /// </param>
        /// <param name="noTracking">
        /// Do not truck changes.
        /// </param>
        /// <returns>
        /// Entity with specified id; null, if not found.
        /// </returns>
        /// <remarks>
        /// noTracking should be set to true for all entities with explicit update.
        ///
        /// Updating noTracking entities:
        /// var blog = await store.GetByIdAsync(id, noTracking : true);
        /// blog.Rating = 5;
        /// store.Update(blog);
        /// context.SaveChanges();
        ///
        /// Updating traking entities:
        /// var blog = await store.GetByIdAsync(id, noTracking : false);
        /// blog.Rating = 5;
        /// context.SaveChanges();
        /// </remarks>
        public virtual ValueTask<TEntity?> GetByIdAsync(object id, bool noTracking = true)
        {
            if (noTracking)
            {
                return new ValueTask<TEntity?>(this.DbEntitySet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id as string));
            }
            return this.DbEntitySet.FindAsync(id);
        }

        /// <summary>
        /// Finds entity by id.
        /// </summary>
        /// <param name="id">
        /// Entity Id.
        /// </param>
        /// <param name="noTracking">
        /// Do not truck changes.
        /// </param>
        /// <returns>
        /// Entity with specified id; null, if not found.
        /// </returns>
        /// <remarks>
        /// noTracking should be set to true for all entities with explicit update.
        ///
        /// Updating noTracking entities:
        /// var blog = store.GetById(id, noTracking : true);
        /// blog.Rating = 5;
        /// store.Update(blog);
        /// context.SaveChanges();
        ///
        /// Updating traking entities:
        /// var blog = store.GetById(id, noTracking : false);
        /// blog.Rating = 5;
        /// context.SaveChanges();
        /// </remarks>
        public virtual TEntity? GetById(object id, bool noTracking = true)
        {
            if (noTracking)
            {
                return this.DbEntitySet.AsNoTracking().FirstOrDefault(e => e.Id == id as string);
            }
            return this.DbEntitySet.Find(id);
        }

        /// <summary>
        /// Creates new entity in store.
        /// </summary>
        /// <param name="entity">New entity.</param>
        public void Create(TEntity entity)
        {
            this.DbEntitySet.Add(entity);
        }

        /// <summary>
        /// Deletes entity from store.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        public void Delete(TEntity entity)
        {
            this.DbEntitySet.Remove(entity);
        }

        /// <summary>
        /// Deletes entity from store.
        /// </summary>
        /// <param name="id">Entity id to delete.</param>
        public void Delete(string id)
        {
            var existedEntity = this.DbEntitySet.Find(id);
            if (existedEntity != null)
            {
                this.DbEntitySet.Remove(existedEntity);
            }
        }

        /// <summary>
        /// Deletes entity from store.
        /// </summary>
        public void Delete(params string[] keys)
        {
            var existedEntity = this.DbEntitySet.Find(keys);
            if (existedEntity != null)
            {
                this.DbEntitySet.Remove(existedEntity);
            }
        }

        /// <summary>
        /// Deletes entity from store.
        /// </summary>
        public void Delete(IEnumerable<TEntity> entities)
        {
            this.DbEntitySet.RemoveRange(entities);
        }

        /// <summary>
        /// Update no-traking entity.
        /// </summary>
        /// <param name="entity">
        /// Entity to update.
        /// </param>
        /// <remarks>
        /// Cretes tracking entity, copy its values from noTracking entity.
        /// </remarks>
        public virtual void Update(TEntity entity)
        {
            var existedEntity = this.DbEntitySet.Find(entity.Id);
            if (existedEntity != null)
            {
                this.Context.Entry(existedEntity).CurrentValues.SetValues(entity);
            }
        }
    }
}