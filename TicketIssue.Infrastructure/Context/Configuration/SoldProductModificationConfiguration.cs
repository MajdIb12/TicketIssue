using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketIssue.Domain.Entities;

namespace TicketIssue.Infrastructure.Context.Configuration;

public class SoldProductModificationConfiguration : IEntityTypeConfiguration<SoldProductModification>
{
    public void Configure(EntityTypeBuilder<SoldProductModification> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AppliedAmount).IsRequired().HasColumnType("decimal(18,2)");
        builder.HasOne(x => x.SoldProduct)
               .WithMany(p => p.AppliedModifications)
               .HasForeignKey(x => x.SoldProductId)
               .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.FareModification)
               .WithMany()
               .HasForeignKey(x => x.FareModificationId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
