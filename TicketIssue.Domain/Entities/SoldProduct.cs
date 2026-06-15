using TicketIssue.Domain.Enums;

namespace TicketIssue.Domain.Entities;

public class SoldProduct
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string PassengerName { get; set; } = string.Empty;
    public ProductType ProductType { get; set; }

    public double? DistanceInKm { get; set; } 
    public int? DurationInDays { get; set; } 

    public decimal BaseFare { get; set; }       
    public decimal FinalFare { get; set; } 

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    public ICollection<SoldProductModification> AppliedModifications { get; set; } = [];
}
