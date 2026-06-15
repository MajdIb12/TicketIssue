namespace TicketIssue.Api.DTOs;

public record CreatePointToPointTicketDto(
    string PassengerName,
    double DistanceInKm,
    List<int>? ModificationIds
);
