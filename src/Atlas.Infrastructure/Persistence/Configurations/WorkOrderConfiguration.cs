using Atlas.Domain.WorkOrders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Atlas.Infrastructure.Persistence.Configurations;

public sealed class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        builder.ToTable("WorkOrders");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Number).HasMaxLength(40).IsRequired();
        builder.Property(x => x.Title).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(8000);
        builder.Property(x => x.CompletionNotes).HasMaxLength(8000);
        builder.Property(x => x.CustomerSignatureName).HasMaxLength(200);
        builder.Property(x => x.CustomerSignatureData).HasColumnType("text");
        builder.Property(x => x.CheckInLatitude).HasPrecision(9, 6);
        builder.Property(x => x.CheckInLongitude).HasPrecision(9, 6);
        builder.Property(x => x.CheckOutLatitude).HasPrecision(9, 6);
        builder.Property(x => x.CheckOutLongitude).HasPrecision(9, 6);
        builder.HasIndex(x => new { x.CompanyId, x.Number }).IsUnique();
        builder.HasIndex(x => new { x.CompanyId, x.Status });
        builder.HasMany(x => x.Assignments).WithOne().HasForeignKey(x => x.WorkOrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Tasks).WithOne().HasForeignKey(x => x.WorkOrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Parts).WithOne().HasForeignKey(x => x.WorkOrderId).OnDelete(DeleteBehavior.Cascade);
    }
}
