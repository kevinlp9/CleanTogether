using CleanTogether.API.Data;
using CleanTogether.API.DTOs;
using CleanTogether.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CleanTogether.Helpers;

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

    // 📄 Obtener detalles de un evento
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ev = await context.CleaningEvents
            .Include(e => e.EventAttendees)
            .Include(e => e.CreatedByUser)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (ev == null)
            return NotFound("Evento no encontrado");

        var eventDto = new CleaningEventResponseDto
        {
            Id = ev.Id,
            Title = ev.Title,
            Description = ev.Description,
            EventDate = ev.EventDate,
            EventTime = ev.EventTime,
            Latitude = ev.Latitude,
            Longitude = ev.Longitude,
            AttendeesCount = ev.EventAttendees.Count,
            ImageBase64 = Convert.ToBase64String(ev.Image)
        };

        return Ok(eventDto);
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
    
    [HttpGet("nearby")]
    public async Task<IActionResult> GetNearby(
        [FromQuery] double lat,
        [FromQuery] double lng,
        [FromQuery] double radiusKm = 5)
    {
        var events = await context.CleaningEvents
            .Include(e => e.EventAttendees)
            .ToListAsync();

        var nearby = events
            .Select(e => new
            {
                Event = e,
                Distance = GeoHelper.DistanceKm(
                    lat, lng,
                    e.Latitude, e.Longitude)
            })
            .Where(x => x.Distance <= radiusKm)
            .OrderBy(x => x.Distance)
            .Select(x => new CleaningEventResponseDto
            {
                Id = x.Event.Id,
                Title = x.Event.Title,
                Description = x.Event.Description,
                EventDate = x.Event.EventDate,
                EventTime = x.Event.EventTime,
                Latitude = x.Event.Latitude,
                Longitude = x.Event.Longitude,
                AttendeesCount = x.Event.EventAttendees.Count,
                ImageBase64 = Convert.ToBase64String(x.Event.Image)
            })
            .ToList();

        return Ok(nearby);
    }

}
