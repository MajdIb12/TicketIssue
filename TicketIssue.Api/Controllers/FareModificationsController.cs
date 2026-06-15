using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketIssue.Api.DTOs;
using TicketIssue.Domain.Entities;
using TicketIssue.Infrastructure.Context;

namespace TicketIssue.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FareModificationsController(ApplicationDbContext context) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var modifications = await context.FareModifications
            .AsNoTracking() 
            .ToListAsync();

        return Ok(modifications);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FareModification), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var modification = await context.FareModifications.FindAsync(id);

        if (modification == null)
            return NotFound(new { Message = $"Modification with ID {id} not found." });

        return Ok(modification);
    }

    [HttpPost]
    [ProducesResponseType(typeof(FareModification), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateModificationDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { Message = "Name is required." });

        var modification = new FareModification
        {
            Name = dto.Name,
            ValueType = dto.ValueType,
            Value = dto.Value,
            IsActive = dto.IsActive
        };

        context.FareModifications.Add(modification);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = modification.Id }, modification);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(FareModification), StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateModificationDto dto)
    {
        var modification = await context.FareModifications.FindAsync(id);

        if (modification == null)
            return NotFound(new { Message = $"Modification with ID {id} not found." });

        // تحديث الحقول
        modification.Name = dto.Name;
        modification.ValueType = dto.ValueType;
        modification.Value = dto.Value;
        modification.IsActive = dto.IsActive;

        await context.SaveChangesAsync();

        return Ok(new { Message = "Modification updated successfully.", Data = modification });
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var modification = await context.FareModifications.FindAsync(id);

        if (modification == null)
            return NotFound(new { Message = $"Modification with ID {id} not found." });

        try
        {
            context.FareModifications.Remove(modification);
            await   context.SaveChangesAsync();

            return Ok(new { Message = $"Modification with ID {id} was deleted successfully." });
        }
        catch (DbUpdateException)
        {
            return BadRequest(new
            {
                Message = "Cannot delete this modification because it is already linked to issued tickets. Try setting IsActive to false instead."
            });
        }
    }
}
