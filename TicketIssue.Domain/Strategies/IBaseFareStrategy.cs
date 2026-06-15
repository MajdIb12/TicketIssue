using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketIssue.Domain.Entities;
using TicketIssue.Domain.Enums;

namespace TicketIssue.Domain.Strategies;

public interface IBaseFareStrategy
{
    bool IsMatch(ProductType productType);

    decimal CalculateBaseFare(SoldProduct product);
}
