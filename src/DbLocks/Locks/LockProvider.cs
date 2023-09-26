namespace CryptoPro.Hub.Service.Common.Locks
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using DbLocks.EntityFramework.Store;

    /// <summary>
    /// Поставщик объектов синхронизации
    /// </summary>
    public class LockProvider : IDisposable
    {
        /// <summary>
        /// Тип объекта блокировки
        /// </summary>
        private readonly string lockType;

        /// <summary>
        /// Хранилище сущностей блокировок
        /// </summary>
        private readonly IObjectLockStore lockStore;

        /// <summary>
        /// Ресурсы очещены
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// Инициализирует объект поставщика объектов синхронизации
        /// </summary>
        /// <param name="lockStore"></param>
        /// <param name="lockType"></param>
        public LockProvider(IObjectLockStore lockStore, string lockType)
        {
            this.lockStore = lockStore
                ?? throw new ArgumentNullException(nameof(lockStore));
            this.lockType = lockType
                ?? throw new ArgumentNullException(nameof(lockType));
        }

        /// <summary>
        /// Получить объект синхронизации
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual ObjectLock Get(string id)
        {
            return new ObjectLock(id, this.lockType, this.lockStore);
        }

        /// <summary>
        /// Получить объект синхронизации с контролем над ним
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual ObjectLock GetAndAcquire(string id, CancellationToken cancellationToken)
        {
            var objectLock = new ObjectLock(id, this.lockType, this.lockStore);
            objectLock.Acquire(cancellationToken);
            return objectLock;
        }

        /// <summary>
        /// Получить объект синхронизации с контролем над ним
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async ValueTask<ObjectLock> GetAndAcquireAsync(string id, CancellationToken cancellationToken)
        {
            var objectLock = new ObjectLock(id, this.lockType, this.lockStore);
            await objectLock.AcquireAsync(cancellationToken).ConfigureAwait(false);
            return objectLock;
        }

        /// <summary>
        /// Осводить ресурсы
        /// </summary>
        /// <param name="disposing"></param>
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