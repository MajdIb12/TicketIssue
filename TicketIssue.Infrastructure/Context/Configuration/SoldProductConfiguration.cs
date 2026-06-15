using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketIssue.Domain.Entities;

namespace TicketIssue.Infrastructure.Context.Configuration;

public class SoldProductConfiguration : IEntityTypeConfiguration<SoldProduct>
{
    public void Configure(EntityTypeBuilder<SoldProduct> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PassengerName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ProductType).HasConversion<string>().IsRequired();
        builder.Property(x => x.BaseFare).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(x => x.FinalFare).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(x => x.IssuedAt).IsRequired();
        builder.Property(x => x.DistanceInKm).HasColumnType("decimal(18,2)");
    }
}
