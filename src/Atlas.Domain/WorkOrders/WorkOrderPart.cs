using Atlas.Domain.Common;

namespace Atlas.Domain.WorkOrders;

public sealed class WorkOrderPart : BaseEntity
{
    public Guid WorkOrderId { get; private set; }
    public Guid StockItemId { get; private set; }
    public decimal Quantity { get; private set; }
    public string? SerialNumber { get; private set; }

    private WorkOrderPart() { }

    internal WorkOrderPart(Guid workOrderId, Guid stockItemId, decimal quantity, string? serialNumber)
    {
        if (stockItemId == Guid.Empty) throw new ArgumentException("Stock item is required.", nameof(stockItemId));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        WorkOrderId = workOrderId;
        StockItemId = stockItemId;
        Quantity = quantity;
        SerialNumber = string.IsNullOrWhiteSpace(serialNumber) ? null : serialNumber.Trim();
    }
}
