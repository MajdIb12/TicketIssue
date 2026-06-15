namespace TicketIssue.Api.DTOs;

public record AppliedModificationDto(
    string ModificationName,
    decimal AppliedAmount
);