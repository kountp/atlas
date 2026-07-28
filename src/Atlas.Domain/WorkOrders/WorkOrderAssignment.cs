using Atlas.Domain.Common;

namespace Atlas.Domain.WorkOrders;

public sealed class WorkOrderAssignment : BaseEntity
{
    public Guid WorkOrderId { get; private set; }
    public Guid TechnicianUserId { get; private set; }
    public Guid AssignedByUserId { get; private set; }
    public DateTimeOffset AssignedAtUtc { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UnassignedAtUtc { get; private set; }
    public string? UnassignmentReason { get; private set; }

    private WorkOrderAssignment() { }

    internal WorkOrderAssignment(Guid workOrderId, Guid technicianUserId, Guid assignedByUserId)
    {
        WorkOrderId = workOrderId;
        TechnicianUserId = technicianUserId;
        AssignedByUserId = assignedByUserId;
    }

    public void Unassign(string reason)
    {
        if (UnassignedAtUtc is not null) return;
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Reason is required.", nameof(reason));
        UnassignedAtUtc = DateTimeOffset.UtcNow;
        UnassignmentReason = reason.Trim();
        Touch();
    }
}
