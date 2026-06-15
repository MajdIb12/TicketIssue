namespace TicketIssue.Api.DTOs;

public record TicketResponseDto(
    Guid TicketId,
    string PassengerName,
    string ProductType,
    decimal BaseFare,
    decimal FinalFare,
    DateTime IssuedAt,
    List<AppliedModificationDto> Breakdown
);
