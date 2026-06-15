using TicketIssue.Domain.Enums;

namespace TicketIssue.Api.DTOs;

public record UpdateModificationDto(string Name, ModificationValueType ValueType, decimal Value, bool IsActive);

public record TransactionReportDto(Guid TransactionId, Guid TicketId, string PassengerName, string ModificationName, string ModificationType, decimal AppliedAmount, DateTime TransactionDate);