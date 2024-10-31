namespace DbLocks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using DbLocks.EntityFramework.Store;

    /// <summary>
    /// DD locks provider.
    /// </summary>
    public class LockProvider : IDisposable
    {
        /// <summary>
        /// Locked object type.
        /// </summary>
        private readonly string lockType;

        /// <summary>
        /// Lock db store.
        /// </summary>
        private readonly IObjectLockStore lockStore;

        /// <summary>
        /// Is object disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Creates lock provider.
        /// </summary>
        /// <param name="lockStore">Lock db store.</param>
        /// <param name="lockType">Locked object type.</param>
        public LockProvider(IObjectLockStore lockStore, string lockType)
        {
            this.lockStore = lockStore
                ?? throw new ArgumentNullException(nameof(lockStore));
            this.lockType = lockType
                ?? throw new ArgumentNullException(nameof(lockType));
        }

        /// <summary>
        /// Get lock for an object.
        /// </summary>
        /// <param name="id">Object id.</param>
        /// <returns>Object lock.</returns>
        public virtual ObjectLock Get(string id)
        {
            return new ObjectLock(id, this.lockType, this.lockStore);
        }

        /// <summary>
        /// Get and acquire lock for an object.
        /// </summary>
        /// <param name="id">Object id.</param>
        /// <returns>Acquired object lock.</returns>
        public virtual ObjectLock GetAndAcquire(string id, CancellationToken cancellationToken)
        {
            var objectLock = new ObjectLock(id, this.lockType, this.lockStore);
            objectLock.Acquire(cancellationToken);
            return objectLock;
        }

        /// <summary>
        /// Get and acquire lock for an object.
        /// </summary>
        /// <param name="id">Object id.</param>
        /// <returns>Acquired object lock.</returns>
        public virtual async ValueTask<ObjectLock> GetAndAcquireAsync(string id, CancellationToken cancellationToken)
        {
            var objectLock = new ObjectLock(id, this.lockType, this.lockStore);
            await objectLock.AcquireAsync(cancellationToken).ConfigureAwait(false);
            return objectLock;
        }

        /// <summary>
        /// Dispose and release lock.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                this.disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined ValueTasks associated with
        /// freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}