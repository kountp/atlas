using Atlas.Domain.Common;

namespace Atlas.Domain.WorkOrders;

public sealed class WorkOrderTask : BaseEntity
{
    public Guid WorkOrderId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public bool IsRequired { get; private set; }
    public bool IsCompleted { get; private set; }
    public Guid? CompletedByUserId { get; private set; }
    public DateTimeOffset? CompletedAtUtc { get; private set; }
    public string? Notes { get; private set; }

    private WorkOrderTask() { }

    internal WorkOrderTask(Guid workOrderId, string title, bool isRequired)
    {
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Task title is required.", nameof(title));
        WorkOrderId = workOrderId;
        Title = title.Trim();
        IsRequired = isRequired;
    }

    public void Complete(Guid userId, string? notes = null)
    {
        if (userId == Guid.Empty) throw new ArgumentException("User is required.", nameof(userId));
        IsCompleted = true;
        CompletedByUserId = userId;
        CompletedAtUtc = DateTimeOffset.UtcNow;
        Notes = notes?.Trim();
        Touch();
    }
}
