using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketIssue.Api.DTOs;
using TicketIssue.Domain.Entities;
using TicketIssue.Domain.Enums;
using TicketIssue.Domain.Services;
using TicketIssue.Infrastructure.Context;

namespace TicketIssue.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController(
    ApplicationDbContext context,
    FareCalculationEngine calculationEngine) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<TicketResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 10;

        var totalTickets = await context.SoldProducts.CountAsync();

        var pagedTicketsRaw = await context.SoldProducts
            .OrderByDescending(t => t.IssuedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new
            {
                TicketId = t.Id,
                PassengerName = t.PassengerName,
                BaseFare = t.BaseFare,
                FinalFare = t.FinalFare,
                ProductTypeEnum = t.ProductType,
                IssuedAt = t.IssuedAt,
                Breakdown = t.AppliedModifications.Select(am => new AppliedModificationDto
                (
                     am.FareModification != null ? am.FareModification.Name : "N/A",
                     am.AppliedAmount
                )).ToList()
            })
            .AsNoTracking()
            .ToListAsync();

        var tickets = pagedTicketsRaw.Select(t => new TicketResponseDto(
            t.TicketId,
            t.PassengerName,
            t.ProductTypeEnum.ToString(), 
            t.BaseFare,
            t.FinalFare,
            t.IssuedAt,
            t.Breakdown
        )).ToList();

        return Ok(new PaginatedResponse<TicketResponseDto>(tickets, totalTickets, pageNumber, pageSize));
    }



    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SoldProduct), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var ticket = await context.SoldProducts
            .Include(t => t.AppliedModifications)
                .ThenInclude(am => am.FareModification)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null)
        {
            return NotFound(new { Message = $"Ticket with ID {id} was not found." });
        }

        return Ok(ticket);
    }




    [HttpPost("point-to-point")]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePointToPointTicket([FromBody] CreatePointToPointTicketDto dto)
    {
        var ticket = new SoldProduct
        {
            PassengerName = dto.PassengerName,
            ProductType = ProductType.PointToPoint,
            DistanceInKm = dto.DistanceInKm
        };

        return await ProcessAndSaveTicketAsync(ticket, dto.ModificationIds);
    }

    [HttpPost("daily-pass")]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDailyPassTicket([FromBody] CreateDailyPassTicketDto dto)
    {
        var ticket = new SoldProduct
        {
            PassengerName = dto.PassengerName,
            ProductType = ProductType.DailyPass,
            DurationInDays = dto.DurationInDays
        };

        return await ProcessAndSaveTicketAsync(ticket, dto.ModificationIds);
    }



    [HttpGet("transactions-report")]
    [ProducesResponseType(typeof(PaginatedResponse<TransactionReportDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTransactionReport([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        var totalTransactions = await context.SoldProductModifications.CountAsync();

        var transactions = await context.SoldProductModifications
            .Include(spm => spm.FareModification)
            .Include(spm => spm.SoldProduct)
            .OrderByDescending(spm => spm.SoldProduct.IssuedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(spm => new TransactionReportDto
            (
                spm.Id,
                spm.SoldProductId,
                spm.SoldProduct.PassengerName,
                spm.FareModification.Name,
                spm.FareModification.ValueType.ToString(),
                spm.AppliedAmount, 
                spm.SoldProduct.IssuedAt
            ))        .AsNoTracking()
            .ToListAsync();

        return Ok(new PaginatedResponse<TransactionReportDto>
        (
            transactions,
            totalTransactions,
            pageNumber,
            pageSize
        ));
    }

    private async Task<IActionResult> ProcessAndSaveTicketAsync(SoldProduct ticket, List<int>? modificationIds)
    {
        var modifications = new List<FareModification>();
        if (modificationIds != null && modificationIds.Count != 0)
        {
            modifications = await context.FareModifications
                .Where(m => modificationIds.Contains(m.Id) && m.IsActive)
                .ToListAsync();
            if (!modifications.Any())
                return NotFound("No active fare modifications found for the provided IDs.");
        }

        try
        {
            calculationEngine.CalculateProductFare(ticket, modifications);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { ex.Message });
        }

        context.SoldProducts.Add(ticket);
        await context.SaveChangesAsync();

        var response = new TicketResponseDto(
            ticket.Id,
            ticket.PassengerName,
            ticket.ProductType.ToString(),
            ticket.BaseFare,
            ticket.FinalFare,
            ticket.IssuedAt,
            ticket.AppliedModifications.Select(m => new AppliedModificationDto(
                m.FareModification?.Name ?? "Unknown",
                m.AppliedAmount
            )).ToList()
        );

        return Ok(response);
    }
}
