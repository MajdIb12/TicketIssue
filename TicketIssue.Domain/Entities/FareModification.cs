using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketIssue.Domain.Enums;

namespace TicketIssue.Domain.Entities;

public class FareModification
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ModificationValueType ValueType { get; set; }
    public decimal Value { get; set; }
    public bool IsActive { get; set; } = true;
}
