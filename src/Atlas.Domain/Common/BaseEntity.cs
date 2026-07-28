namespace Atlas.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    // Audit fields are writable because the DbContext lives in a separate assembly
    // and is responsible for maintaining them during SaveChanges.
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string CreatedBy { get; set; } = "system";
    public DateTimeOffset? ModifiedAtUtc { get; set; }
    public string? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset? DeletedAtUtc { get; set; }
    public string? DeletedBy { get; set; }

    // Optimistic concurrency token used by Company and Customer mappings.
    public byte[] Version { get; set; } = Array.Empty<byte>();

    protected void Touch()
    {
        ModifiedAtUtc = DateTimeOffset.UtcNow;
    }

    public void SoftDelete(string deletedBy = "system")
    {
        if (IsDeleted)
            return;

        IsDeleted = true;
        DeletedAtUtc = DateTimeOffset.UtcNow;
        DeletedBy = deletedBy;
        Touch();
    }
}
