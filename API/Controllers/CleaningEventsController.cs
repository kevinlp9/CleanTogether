using CleanTogether.API.Data;
using CleanTogether.API.DTOs;
using CleanTogether.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CleanTogether.API.Controllers;

[Authorize]
[ApiController]
[Route("api/events")]
public class CleaningEventsController(AppDbContext context) : ControllerBase
{
    // 🧹 Crear evento
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateCleaningEventDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        using var ms = new MemoryStream();
        await dto.Image.CopyToAsync(ms);

        var ev = new CleaningEvent
        {
            Title = dto.Title,
            Description = dto.Description,
            EventDate = dto.EventDate,
            EventTime = dto.EventTime,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Image = ms.ToArray(),
            CreatedByUserId = userId
        };

        context.CleaningEvents.Add(ev);
        await context.SaveChangesAsync();

        return Ok(ev.Id);
    }

    // 📋 Listar eventos
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var events = await context.CleaningEvents
            .Include(e => e.EventAttendees)
            .Select(e => new CleaningEventResponseDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                EventDate = e.EventDate,
                EventTime = e.EventTime,
                Latitude = e.Latitude,
                Longitude = e.Longitude,
                AttendeesCount = e.EventAttendees.Count,
                ImageBase64 = Convert.ToBase64String(e.Image)
            })
            .ToListAsync();

        return Ok(events);
    }

    // 🙋 Asistir
    [HttpPost("{id}/attend")]
    public async Task<IActionResult> Attend(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var exists = await context.EventAttendees
            .AnyAsync(a => a.EventId == id && a.UserId == userId);

        if (exists)
            return BadRequest("Already attending");

        context.EventAttendees.Add(new EventAttendee
        {
            EventId = id,
            UserId = userId
        });

        await context.SaveChangesAsync();

        return Ok();
    }
}
