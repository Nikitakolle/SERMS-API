using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SERMS.API.DTOs;
using SERMS.API.Interfaces;
using SERMS.API.Services;
using SERMS.Domain.Entities;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SERMS.API.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventController> _logger;

        public EventController(IEventService eventService, ILogger<EventController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAll([FromQuery] PaginationParam param)
        {
            _logger.LogInformation("Fetching all events - Page {Page}, Size {Size}",
    param.PageNumber, param.PageSize);

            var events = await _eventService.GetAllEventAsync(param);
           
            return Ok(new ApiResponseDto<PagedResponseDto<EventResponseDto>>
            {
                Success = true,
                Message = "Events fetched Successfully",
                Data = events
            });
        }

        
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<EventResponseDto>> GetById(int id)
        {
            _logger.LogInformation("Fetching event with Id {EventId}", id);

            var e = await _eventService.GetEventByIdAsync(id);

            if (e == null)
            {
                _logger.LogWarning("Event not found with Id {EventId}", id);
                return NotFound("Event not found");
            }

            return Ok(new ApiResponseDto<EventResponseDto>
            {
                Success = true,
                Message = "Event fetched Successfully",
                Data = e
            });
        }

      
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateEventDto dto)
        {
            
            var organizerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            _logger.LogInformation("User {UserId} is attempting to create an event", organizerId);

            var result = await _eventService.CreateEventAsync(dto, organizerId);

            if (!result.Success)

                return BadRequest(result);

            return Ok(result);
        }

       
            

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateEventDto dto)
        {
            _logger.LogInformation("Request to update event Id {EventId}", id);

            if (id != dto.Id)
            {
                _logger.LogWarning("Event ID mismatch: URL Id {UrlId} vs Body Id {BodyId}", id, dto.Id);
                return BadRequest("Id mismatch");
            }

            await _eventService.UpdateEventAsync(dto);
          
            return Ok(new ApiResponseDto<string>
            {
                Success = true,
                Message = "Event updated successfully",
                Data = null
            });
            
        }

       
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Request to delete event Id {EventId}", id);

            await _eventService.DeleteEventAsync(id);
            
            return Ok(new ApiResponseDto<string>
            {
                Success = true,
                Message = "Event deleted successfully",
                Data = null
            });
        }
    }
}