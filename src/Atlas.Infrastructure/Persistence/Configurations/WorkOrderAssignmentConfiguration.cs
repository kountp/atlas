using Atlas.Domain.WorkOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Infrastructure.Persistence.Configurations;

public sealed class WorkOrderAssignmentConfiguration : IEntityTypeConfiguration<WorkOrderAssignment>
{
    public void Configure(EntityTypeBuilder<WorkOrderAssignment> builder)
    {
        builder.ToTable("WorkOrderAssignments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.UnassignmentReason).HasMaxLength(1000);
        builder.HasIndex(x => new { x.WorkOrderId, x.TechnicianUserId, x.UnassignedAtUtc });
    }
}
