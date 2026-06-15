using TicketIssue.Domain.Entities;
using TicketIssue.Domain.Enums;

namespace TicketIssue.Domain.Strategies;

public class PointToPointFareStrategy : IBaseFareStrategy
{
    private const decimal BasePrice = 10.00m;
    private const decimal RatePerKm = 2.50m;

    public bool IsMatch(ProductType productType) => productType == ProductType.PointToPoint;

    public decimal CalculateBaseFare(SoldProduct product)
    {
        if (product.DistanceInKm == null || product.DistanceInKm <= 0)
            throw new ArgumentException("Distance in KM must be provided and greater than zero for Point-to-Point trips");

        return BasePrice + ((decimal)product.DistanceInKm * RatePerKm);
    }
}