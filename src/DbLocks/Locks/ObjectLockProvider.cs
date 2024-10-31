using DbLocks.EntityFramework.Store;

namespace DbLocks
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