namespace TicketIssue.Api.DTOs;

public record CreateDailyPassTicketDto(
    string PassengerName,
    int DurationInDays,
    List<int>? ModificationIds
);
