using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SERMS.API.DTOs;
using SERMS.API.Interfaces;
using SERMS.Domain.Entities;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class EventParticipantController : ControllerBase
{
    private readonly IEventParticipantService _service;
    private readonly ILogger<EventParticipantController> _logger;

    public EventParticipantController(IEventParticipantService service, ILogger<EventParticipantController> logger)
    {
        _service = service;
        _logger = logger;
    }

   
    [Authorize]
    [HttpPost("join")]
    public async Task<IActionResult> JoinEvent(int eventId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        _logger.LogInformation("User {UserId} attempting to join event {EventId}", userId, eventId);

        var result = await _service.JoinEventAsync(eventId, userId);

        if (!result.Success)
        {
            _logger.LogWarning("Join failed for User {UserId} and Event {EventId}", userId, eventId);
            return BadRequest(result);
        }

        return Ok(result);
    }

  
    [HttpGet("event/{eventId}")]
    public async Task<IActionResult> GetParticipants(int eventId, [FromQuery] PaginationParam param)
    {
        _logger.LogInformation("Fetching participants for Event {EventId}", eventId);

        var result = await _service.GetParticipantByEventAsync(eventId, param);

        return Ok(result);
    }

    
    [Authorize]
    [HttpGet("my-events")]
    public async Task<IActionResult> GetMyEvents([FromQuery] PaginationParam param)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        _logger.LogInformation("Fetching events for User {UserId}", userId);

        var result = await _service.GetEventsByUserAsync(userId, param);

        return Ok(result);
    }
}