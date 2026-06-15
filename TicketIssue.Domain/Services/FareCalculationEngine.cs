using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketIssue.Domain.Entities;
using TicketIssue.Domain.Enums;
using TicketIssue.Domain.Strategies;

namespace TicketIssue.Domain.Services;


public class FareCalculationEngine
{
    private readonly IEnumerable<IBaseFareStrategy> _strategies;

    public FareCalculationEngine(IEnumerable<IBaseFareStrategy> strategies)
    {
        _strategies = strategies;
    }

    public void CalculateProductFare(SoldProduct product, List<FareModification> requestedModifications)
    {
        var strategy = _strategies.FirstOrDefault(s => s.IsMatch(product.ProductType))
            ?? throw new NotSupportedException($"No fare policy found for product type: {product.ProductType}");

        product.BaseFare = strategy.CalculateBaseFare(product);

        decimal runningFare = product.BaseFare;

        foreach (var mod in requestedModifications)
        {
            decimal appliedAmount = 0;

            if (mod.ValueType == ModificationValueType.FixedAmount)
            {
                appliedAmount = mod.Value;
            }
            else if (mod.ValueType == ModificationValueType.Percentage)
            {
                appliedAmount = product.BaseFare * (mod.Value / 100);
            }

            runningFare += appliedAmount;

            product.AppliedModifications.Add(new SoldProductModification
            {
                SoldProductId = product.Id,
                FareModificationId = mod.Id,
                FareModification = mod,
                AppliedAmount = appliedAmount
            });
        }

        // 3. تخزين السعر النهائي
        product.FinalFare = runningFare;
    }
}