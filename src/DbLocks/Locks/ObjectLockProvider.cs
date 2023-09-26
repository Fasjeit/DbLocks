using DbLocks.EntityFramework.Store;

namespace CryptoPro.Hub.Service.Common.Locks
{
    /// <summary>
    /// Object LockProvider sample.
    /// </summary>
    public class ObjectLockProvider : LockProvider
    {
        public ObjectLockProvider(IObjectLockStore lockStore)
            : base(lockStore, "object")
        {
        }
    }
}