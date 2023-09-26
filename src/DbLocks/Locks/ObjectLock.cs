namespace CryptoPro.Hub.Service.Common.Locks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using DbLocks.EntityFramework.Store;

    /// <summary>
    /// Syncronization lock object.
    /// </summary>
    public class ObjectLock : IDisposable, IAsyncDisposable
    {
        /// <summary>
        ///  Lock store.
        /// </summary>
        private readonly IObjectLockStore lockStore;

        /// <summary>
        /// Locked object id.
        /// </summary>
        private readonly string objectId;

        /// <summary>
        /// Object type.
        /// </summary>
        private readonly string objectType;

        /// <summary>
        /// Lock disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Lock acquired.
        /// </summary>
        private bool acquired = false;

        /// <summary>
        /// Creates new object lock.
        /// </summary>
        /// <param name="objectId">
        /// Locked object id.
        /// </param>
        /// <param name="objectType">
        /// Locked object type.
        /// </param>
        /// <param name="lockStore">
        /// Lock store.
        /// </param>
        public ObjectLock(
            string objectId,
            string objectType,
            IObjectLockStore lockStore)
        {
            this.objectId = objectId
                ?? throw new ArgumentNullException(nameof(objectId));
            this.objectType = objectType
                ?? throw new ArgumentNullException(nameof(objectType));
            this.lockStore = lockStore
                 ?? throw new ArgumentNullException(nameof(lockStore));
        }

        /// <summary>
        /// Acquire object lock.
        /// </summary>
        public virtual void Acquire(CancellationToken cancellationToken)
        {
            if (this.acquired)
            {
                throw new InvalidOperationException(
                    $"Lock for object [{this.objectType}] with id [{this.objectId}] already acquired");
            }
            while (!cancellationToken.IsCancellationRequested)
            {
                this.acquired = this.lockStore.Acquire(this.objectId, this.objectType);
                if (this.acquired)
                {
                    break;
                }
                Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken).GetAwaiter().GetResult();
            }
            if (!this.acquired)
            {
                throw new Exception($"Cannot acquire lock dor object [{this.objectType}] with id [{this.objectId}].");
            }
        }

        /// <summary>
        /// Acquire object lock.
        /// </summary>
        public virtual async ValueTask AcquireAsync(CancellationToken cancellationToken)
        {
            if (this.acquired)
            {
                throw new InvalidOperationException(
                    $"Lock for object [{this.objectType}] with id [{this.objectId}] already acquired");
            }
            while (!cancellationToken.IsCancellationRequested)
            {
                this.acquired = await this.lockStore.AcquireAsync(this.objectId, this.objectType).ConfigureAwait(false);
                if (this.acquired)
                {
                    break;
                }
                await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken).ConfigureAwait(false);
            }
            if (!this.acquired)
            {
                throw new Exception($"Cannot acquire lock dor object [{this.objectType}] with id [{this.objectId}].");
            }
        }

        /// <summary>
        /// Release object lock.
        /// </summary>
        /// <returns></returns>
        public void Release()
        {
            if (this.acquired)
            {
                this.lockStore.Delete(
                    this.objectId,
                    this.objectType);
                this.acquired = false;
            }
        }

        /// <summary>
        /// Release object lock.
        /// </summary>
        /// <returns></returns>
        public async ValueTask ReleaseAsync()
        {
            if (this.acquired)
            {
                await this.lockStore.DeleteAsync(
                        this.objectId,
                        this.objectType)
                    .ConfigureAwait(false);
                this.acquired = false;
            }
        }

        /// <summary>
        /// Dispose lock.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this.disposed)
                {
                    this.Release();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await this.DisposeAsyncCore().ConfigureAwait(false);

            // Dispose of managed resources.
            this.Dispose(false);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (!this.disposed)
            {
                await this.ReleaseAsync().ConfigureAwait(false);
            }
            this.disposed = true;
        }
    }
}