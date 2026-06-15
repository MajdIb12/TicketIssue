namespace TicketIssue.Api.DTOs;

public record PaginatedResponse<T>(
    List<T> Data,
    int TotalCount,
    int PageNumber,
    int PageSize
);
