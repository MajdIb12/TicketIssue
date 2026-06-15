using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketIssue.Domain.Entities;

namespace TicketIssue.Infrastructure.Context.Configuration;

public class FareModificationConfiguration : IEntityTypeConfiguration<FareModification>
{
    public void Configure(EntityTypeBuilder<FareModification> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ValueType).HasConversion<string>().IsRequired();
        builder.Property(x => x.Value).IsRequired().HasColumnType("decimal(18,2)");
    }
}