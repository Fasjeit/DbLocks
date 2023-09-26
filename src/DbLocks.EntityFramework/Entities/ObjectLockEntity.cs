namespace DbLocks.EntityFramework.Entities
{
    /// <summary>
    /// The Object lock entity.
    /// </summary>
    public class ObjectLockEntity : IBaseEntity
    {
        public string? Id { get; set; }

        public string? Type { get; set; }
    }
}