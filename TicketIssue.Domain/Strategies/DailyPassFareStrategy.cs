using TicketIssue.Domain.Entities;
using TicketIssue.Domain.Enums;

namespace TicketIssue.Domain.Strategies;

public class DailyPassFareStrategy : IBaseFareStrategy
{
    private const decimal FlatDailyRate = 50.00m;

    public bool IsMatch(ProductType productType) => productType == ProductType.DailyPass;

    public decimal CalculateBaseFare(SoldProduct product)
    {
        if (product.DurationInDays == null || product.DurationInDays <= 0)
            throw new ArgumentException("Duration in days must be provided and greater than zero for Daily Pass");

        return (decimal)product.DurationInDays * FlatDailyRate;
    }
}