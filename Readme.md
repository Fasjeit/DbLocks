# DbLocks

**DbLocks** is a C# library sample that provides an easy way to create postgress db based sync locks on objects.

## Usage

```csharp
// Create db table for the lock entity.
public class ObjectLockEntity : IBaseEntity
{
    public string? Id { get; set; }

    public string? Type { get; set; }
}

// Create typed lock provider.
public class ClientLockProvider : LockProvider
{
    public ClientLockProvider(IObjectLockStore lockStore)
        : base(lockStore, "Client")
    {
    }
}

var clientLockProvider = new ClientLockProvider(store);

// Acquire lock client object.
// If the object already locked by other lock - wait untill lock will be released.
await using (var clientLock =
    await clientLockProvider.GetAndAcquireAsync(client.Id, this.cnsSource.Token))
{
    // Use client exclusively.
    // All other GetAndAcquireAsync will have to wait untill release.
}// lock will be released during Dispose
```
