namespace CryptoPro.Hub.EntityFramework.Store
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DbLocks.EntityFramework.Entities;
    using DbLocks.EntityFramework.Store;
    using Microsoft.EntityFrameworkCore;
    using Npgsql;

    /// <summary>
    /// Locks store.
    /// </summary>
    public class ObjectLockStore : IObjectLockStore
    {
        public DbContext Context { get; }

        private readonly EntityStore<ObjectLockEntity> objectLockStore;

        public ObjectLockStore(DbContext dbContext)
        {
            this.Context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.objectLockStore = new EntityStore<ObjectLockEntity>(dbContext);
        }

        /// <summary>
        /// Creates a lock object in store.
        /// </summary>
        /// <param name="objectId">
        /// The ValueTask id.
        /// </param>
        /// <param name="objectType">
        /// The object type.
        /// </param>
        public bool Acquire(string objectId, string objectType)
        {
            if (objectId == null)
            {
                throw new ArgumentNullException(nameof(objectId));
            }

            if (objectType == null)
            {
                throw new ArgumentNullException(nameof(objectType));
            }

            if (this.objectLockStore.EntitySet.Any(l => l.Id == objectId && l.Type == objectType))
            {
                // Already locked - return false
                return false;
            }

            try
            {
                this.objectLockStore.Create(
                    new ObjectLockEntity
                    {
                        Id = objectId,
                        Type = objectType
                    });

                this.Context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                var postgressExeption = ex.InnerException as PostgresException;
                if (postgressExeption != null)
                {
                    // Probably lock with Id already exists
                    // 23505 unique_violation
                    if (postgressExeption.SqlState == "23505")
                    {
                        // Someone locked before us
                        // just tell the lock is already aquired
                        return false;
                    }
                }
                throw ex;
            }
            return true;
        }

        /// <summary>
        /// Creates a lock object in store.
        /// </summary>
        /// <param name="objectId">
        /// The ValueTask id.
        /// </param>
        /// <param name="objectType">
        /// The object type.
        /// </param>
        public async ValueTask<bool> AcquireAsync(string objectId, string objectType)
        {
            if (objectId == null)
            {
                throw new ArgumentNullException(nameof(objectId));
            }

            if (objectType == null)
            {
                throw new ArgumentNullException(nameof(objectType));
            }

            if (this.objectLockStore.EntitySet.Any(l => l.Id == objectId && l.Type == objectType))
            {
                // Already locked - return false
                return false;
            }

            try
            {
                this.objectLockStore.Create(
                    new ObjectLockEntity
                    {
                        Id = objectId,
                        Type = objectType
                    });
                await this.Context.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (DbUpdateException ex)
            {
                var postgressExeption = ex.InnerException as PostgresException;
                if (postgressExeption != null)
                {
                    // Probably lock with Id already exists
                    // 23505 unique_violation
                    if (postgressExeption.SqlState == "23505")
                    {
                        // Someone locked before us
                        // just tell the lock is aquired
                        return false;
                    }
                }
                throw ex;
            }
            return true;
        }

        /// <summary>
        /// Deletes a lock object in store.
        /// </summary>
        /// <param name="objectId">
        /// The ValueTask id.
        /// </param>
        /// <param name="objectType">
        /// The object type.
        /// </param>
        public void Delete(string objectId, string objectType)
        {
            if (objectId == null)
            {
                throw new ArgumentNullException(nameof(objectId));
            }

            if (objectType == null)
            {
                throw new ArgumentNullException(nameof(objectType));
            }

            if (this.objectLockStore.DbEntitySet.Local.Any(
                l => l.Id == objectId && l.Type == objectType))
            {
                var entity =
                    this.objectLockStore.DbEntitySet.Find(
                        objectId,
                        objectType);

                if (entity == null)
                {
                    return;
                }

                this.objectLockStore.Delete(entity);
            }
            else
            {
                this.objectLockStore.Delete(objectId, objectType);
            }

            this.Context.SaveChanges();
        }

        /// <summary>
        /// Deletes a lock object in store.
        /// </summary>
        /// <param name="objectId">
        /// The ValueTask id.
        /// </param>
        /// <param name="objectType">
        /// The object type.
        /// </param>
        public async ValueTask DeleteAsync(string objectId, string objectType)
        {
            if (objectId == null)
            {
                throw new ArgumentNullException(nameof(objectId));
            }

            if (objectType == null)
            {
                throw new ArgumentNullException(nameof(objectType));
            }

            if (this.objectLockStore.DbEntitySet.Local.Any(
                l => l.Id == objectId && l.Type == objectType))
            {
                var entity =
                    await this.objectLockStore.DbEntitySet.FindAsync(
                        objectId,
                        objectType).ConfigureAwait(false);

                if (entity == null)
                {
                    return;
                }

                this.objectLockStore.Delete(entity);
            }
            else
            {
                this.objectLockStore.Delete(objectId);
            }

            await this.Context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}