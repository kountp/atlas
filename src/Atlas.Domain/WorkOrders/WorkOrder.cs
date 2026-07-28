using Atlas.Domain.Common;

namespace Atlas.Domain.WorkOrders;

public sealed class WorkOrder : BaseEntity
{
    private readonly List<WorkOrderAssignment> _assignments = [];
    private readonly List<WorkOrderTask> _tasks = [];
    private readonly List<WorkOrderPart> _parts = [];

    public Guid CompanyId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid SiteId { get; private set; }
    public Guid? TicketId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public WorkOrderStatus Status { get; private set; } = WorkOrderStatus.Draft;
    public DateTimeOffset? ScheduledStartUtc { get; private set; }
    public DateTimeOffset? ScheduledEndUtc { get; private set; }
    public DateTimeOffset? CheckInUtc { get; private set; }
    public DateTimeOffset? CheckOutUtc { get; private set; }
    public decimal? CheckInLatitude { get; private set; }
    public decimal? CheckInLongitude { get; private set; }
    public decimal? CheckOutLatitude { get; private set; }
    public decimal? CheckOutLongitude { get; private set; }
    public string? CustomerSignatureName { get; private set; }
    public string? CustomerSignatureData { get; private set; }
    public DateTimeOffset? CustomerSignedAtUtc { get; private set; }
    public string? CompletionNotes { get; private set; }

    public IReadOnlyCollection<WorkOrderAssignment> Assignments => _assignments.AsReadOnly();
    public IReadOnlyCollection<WorkOrderTask> Tasks => _tasks.AsReadOnly();
    public IReadOnlyCollection<WorkOrderPart> Parts => _parts.AsReadOnly();

    private WorkOrder() { }

    public WorkOrder(Guid companyId, Guid customerId, Guid siteId, string number, string title, string? description = null, Guid? ticketId = null)
    {
        if (companyId == Guid.Empty) throw new ArgumentException("Company is required.", nameof(companyId));
        if (customerId == Guid.Empty) throw new ArgumentException("Customer is required.", nameof(customerId));
        if (siteId == Guid.Empty) throw new ArgumentException("Site is required.", nameof(siteId));
        if (string.IsNullOrWhiteSpace(number)) throw new ArgumentException("Number is required.", nameof(number));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));

        CompanyId = companyId;
        CustomerId = customerId;
        SiteId = siteId;
        TicketId = ticketId;
        Number = number.Trim();
        Title = title.Trim();
        Description = description?.Trim() ?? string.Empty;
    }

    public void Schedule(DateTimeOffset startUtc, DateTimeOffset endUtc)
    {
        if (endUtc <= startUtc) throw new ArgumentException("End must be later than start.", nameof(endUtc));
        EnsureNotClosed();
        ScheduledStartUtc = startUtc;
        ScheduledEndUtc = endUtc;
        Status = WorkOrderStatus.Scheduled;
        Touch();
    }

    public void Assign(Guid technicianUserId, Guid assignedByUserId)
    {
        if (technicianUserId == Guid.Empty) throw new ArgumentException("Technician is required.", nameof(technicianUserId));
        EnsureNotClosed();
        if (_assignments.Any(x => x.TechnicianUserId == technicianUserId && x.UnassignedAtUtc is null)) return;

        _assignments.Add(new WorkOrderAssignment(Id, technicianUserId, assignedByUserId));
        if (Status is WorkOrderStatus.Draft or WorkOrderStatus.Scheduled)
            Status = WorkOrderStatus.Assigned;
        Touch();
    }

    public void CheckIn(decimal latitude, decimal longitude)
    {
        EnsureNotClosed();
        ValidateCoordinates(latitude, longitude);
        if (CheckInUtc is not null) throw new InvalidOperationException("Work order is already checked in.");

        CheckInUtc = DateTimeOffset.UtcNow;
        CheckInLatitude = latitude;
        CheckInLongitude = longitude;
        Status = WorkOrderStatus.InProgress;
        Touch();
    }

    public void CheckOut(decimal latitude, decimal longitude, string? completionNotes)
    {
        EnsureNotClosed();
        ValidateCoordinates(latitude, longitude);
        if (CheckInUtc is null) throw new InvalidOperationException("Check-in is required before check-out.");
        if (CheckOutUtc is not null) throw new InvalidOperationException("Work order is already checked out.");

        CheckOutUtc = DateTimeOffset.UtcNow;
        CheckOutLatitude = latitude;
        CheckOutLongitude = longitude;
        CompletionNotes = completionNotes?.Trim();
        Touch();
    }

    public void CaptureCustomerSignature(string signerName, string signatureData)
    {
        if (string.IsNullOrWhiteSpace(signerName)) throw new ArgumentException("Signer name is required.", nameof(signerName));
        if (string.IsNullOrWhiteSpace(signatureData)) throw new ArgumentException("Signature data is required.", nameof(signatureData));

        CustomerSignatureName = signerName.Trim();
        CustomerSignatureData = signatureData;
        CustomerSignedAtUtc = DateTimeOffset.UtcNow;
        Touch();
    }

    public void Complete(bool signatureRequired = true)
    {
        EnsureNotClosed();
        if (CheckInUtc is null || CheckOutUtc is null)
            throw new InvalidOperationException("Check-in and check-out are required before completion.");
        if (_tasks.Any(x => !x.IsCompleted))
            throw new InvalidOperationException("All work-order tasks must be completed.");
        if (signatureRequired && CustomerSignedAtUtc is null)
            throw new InvalidOperationException("Customer signature is required.");

        Status = WorkOrderStatus.Completed;
        Touch();
    }

    public void Cancel(string reason)
    {
        EnsureNotClosed();
        if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Cancellation reason is required.", nameof(reason));
        CompletionNotes = reason.Trim();
        Status = WorkOrderStatus.Cancelled;
        Touch();
    }

    public void AddTask(string title, bool required = true)
    {
        EnsureNotClosed();
        _tasks.Add(new WorkOrderTask(Id, title, required));
        Touch();
    }

    public void AddPart(Guid stockItemId, decimal quantity, string? serialNumber = null)
    {
        EnsureNotClosed();
        _parts.Add(new WorkOrderPart(Id, stockItemId, quantity, serialNumber));
        Touch();
    }

    private void EnsureNotClosed()
    {
        if (Status is WorkOrderStatus.Completed or WorkOrderStatus.Cancelled)
            throw new InvalidOperationException("Closed work orders cannot be modified.");
    }

    private static void ValidateCoordinates(decimal latitude, decimal longitude)
    {
        if (latitude is < -90 or > 90) throw new ArgumentOutOfRangeException(nameof(latitude));
        if (longitude is < -180 or > 180) throw new ArgumentOutOfRangeException(nameof(longitude));
    }
}
