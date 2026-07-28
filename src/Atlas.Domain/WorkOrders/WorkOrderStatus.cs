namespace Atlas.Domain.WorkOrders;

public enum WorkOrderStatus
{
    Draft = 0,
    Scheduled = 1,
    Assigned = 2,
    EnRoute = 3,
    InProgress = 4,
    Paused = 5,
    AwaitingParts = 6,
    AwaitingCustomer = 7,
    Completed = 8,
    Cancelled = 9
}
