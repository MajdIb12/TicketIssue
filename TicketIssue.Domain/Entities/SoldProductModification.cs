namespace TicketIssue.Domain.Entities;

public class SoldProductModification
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SoldProductId { get; set; }
    public SoldProduct? SoldProduct { get; set; }

    public int FareModificationId { get; set; }
    public FareModification? FareModification { get; set; }

  
    public decimal AppliedAmount { get; set; }
}