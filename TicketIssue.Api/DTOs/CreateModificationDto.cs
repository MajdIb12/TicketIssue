using TicketIssue.Domain.Enums;

namespace TicketIssue.Api.DTOs;

public record CreateModificationDto(string Name, ModificationValueType ValueType, decimal Value, bool IsActive);
