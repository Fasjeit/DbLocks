namespace DbLocks.EntityFramework.Entities
{
    /// <summary>
    /// Base entity class.
    /// </summary>
    public interface IBaseEntity
    {
        public string? Id { get; set; }
    }
}