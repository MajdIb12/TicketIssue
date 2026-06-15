using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketIssue.Domain.Entities;
using TicketIssue.Domain.Enums;
using TicketIssue.Infrastructure.Context;

namespace TicketIssue.Infrastructure;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();

        if (context.FareModifications.Any()) return;

        var modifications = new List<FareModification>
        {
            new() {
                Name = "First Class Tier",
                ValueType = ModificationValueType.Percentage,
                Value = 20.00m 
            },
            new() {
                Name = "Business Class Tier",
                ValueType = ModificationValueType.Percentage,
                Value = 10.00m 
            },
            new() {
                Name = "Extra Luggage Plan (Heavy)",
                ValueType = ModificationValueType.FixedAmount,
                Value = 50.00m 
            },
            new() {
                Name = "Economy Promo Discount",
                ValueType = ModificationValueType.FixedAmount,
                Value = -15.00m
            }
        };

        context.FareModifications.AddRange(modifications);
        await context.SaveChangesAsync();
    }
}
