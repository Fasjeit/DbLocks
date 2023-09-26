namespace DbLocks.EntityFramework.Store
{
    using System.Threading.Tasks;

    /// <summary>
    /// The store for a Lock.
    /// </summary>
    public interface IObjectLockStore
    {
        /// <summary>
        /// Creates a lock object in store.
        /// </summary>
        /// <param name="objectId">
        /// Object id.
        /// </param>
        /// <param name="objectType">
        /// The object type.
        /// </param>
        bool Acquire(string objectId, string objectType);

        /// <summary>
        /// Creates a lock object in store.
        /// </summary>
        /// <param name="objectId">
        /// Object id.
        /// </param>
        /// <param name="objectType">
        /// The object type.
        /// </param>
        ValueTask<bool> AcquireAsync(string objectId, string objectType);

        /// <summary>
        /// Creates a lock object in store.
        /// </summary>
        /// <param name="objectId">
        /// Object id.
        /// </param>
        /// <param name="objectType">
        /// The object type.
        /// </param>
        void Delete(string objectId, string objectType);

        /// <summary>
        /// Creates a lock object in store.
        /// </summary>
        /// <param name="objectId">
        /// Object id.
        /// </param>
        /// <param name="objectType">
        /// The object type.
        /// </param>
        ValueTask DeleteAsync(string objectId, string objectType);
    }
}