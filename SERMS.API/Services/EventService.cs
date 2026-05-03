using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SERMS.API.DTOs;
using SERMS.API.Interfaces;
using SERMS.API.Services;
using SERMS.Domain.Entities;
using SERMS.Domain.Interfaces.Repositories;
using SERMS.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace SERMS.API.Services
{
    public class EventService: IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<EventService> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly INotificationService _notificationService;
        public EventService (IEventRepository eventRepository, ILogger<EventService> logger, IBackgroundJobClient backgroundJobClient,
    INotificationService notificationService)
        {
            _eventRepository = eventRepository;
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
            _notificationService = notificationService;
        }
        public async Task<PagedResponseDto<EventResponseDto>> GetAllEventAsync(PaginationParam param)
        {
            var query = _eventRepository.GetQueryable();
            var totalCount =await query.CountAsync();
            var events = await query.Skip((param.PageNumber - 1) *
                param.PageSize).Take(param.PageSize).ToListAsync();
            var data = events.Select(e => new EventResponseDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                StartTime = e.StartTime,
                EndTime = e.EndTime
            });
            _logger.LogInformation("Fetching events with pagination Page {PageNumber}, Size {PageSize}",
    param.PageNumber, param.PageSize);
            return new PagedResponseDto<EventResponseDto>
            {
                Data= data,
                PageNumber= param.PageNumber,
                PageSize = param.PageSize,
                TotalCount = totalCount
            };
        }
        public async Task<EventResponseDto> GetEventByIdAsync(int id)
        {
            var events = await _eventRepository.GetByIdAsync(id);
            if (events == null)
            {
                _logger.LogWarning("Event not found with Id {EventId}", id);
                return null;
            }
            return new EventResponseDto
            {
                Id = events.Id,
                Title = events.Title,
                Description = events.Description,
                StartTime = events.StartTime,
                EndTime = events.EndTime
            };

        }


        public async Task<ApiResponseDto<EventResponseDto>> CreateEventAsync(CreateEventDto dto, int OrganizerId)
        {
           
            if (dto.StartTime >= dto.EndTime)
            {
                _logger.LogWarning("Invalid event time: Start {Start} End {End}", dto.StartTime, dto.EndTime);

                return ApiResponseDto<EventResponseDto>.FailResponse(
                    "Start time must be before end time"
                );
            }

            
            var conflicts = await _eventRepository.GetConflictingEventsAsync(
                dto.RoomId,
                dto.StartTime,
                dto.EndTime
            );

            if (conflicts.Any())
            {
                _logger.LogWarning("Conflict detected for Room {RoomId} between {Start} and {End}",
                    dto.RoomId, dto.StartTime, dto.EndTime);

               
                var suggestion = await GetNextAvailableSlot(
                    dto.RoomId,
                    dto.StartTime,
                    dto.EndTime
                );

                var responseData = new
                {
                    conflicts = conflicts.Select(c => new
                    {
                        c.Id,
                        c.StartTime,
                        c.EndTime
                    }),
                    suggestedSlot = new
                    {
                        Start = suggestion.start,
                        End = suggestion.end
                    }
                };

                return ApiResponseDto<EventResponseDto>.FailResponse(
                    "Time slot conflict. Suggested next available slot provided.",
                    responseData
                );
            }

            
            var events = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                RoomId = dto.RoomId,
                OrganizerId = OrganizerId
            };

            await _eventRepository.AddAsync(events);
            await _eventRepository.SaveChangesAsync();

           
            var reminderTime = events.StartTime.AddMinutes(-30);


            if (reminderTime <= DateTime.UtcNow)
            {
                _logger.LogWarning("Reminder skipped for Event {EventId} (too late)", events.Id);
            }
            else
            {
                _backgroundJobClient.Schedule<INotificationService>(
                    x => x.SendEventReminder(events.Id),
                    reminderTime
                );

                _logger.LogInformation("Reminder scheduled for Event {EventId} at {ReminderTime}",
                    events.Id, reminderTime);
            }
         
            var response = new EventResponseDto
            {
                Id = events.Id,
                Title = events.Title,
                Description = events.Description,
                StartTime = events.StartTime,
                EndTime = events.EndTime
            };

            _logger.LogInformation("Event created successfully with Title {Title}", dto.Title);

            return ApiResponseDto<EventResponseDto>.SuccessResponse(
                "Event created successfully",
                response
            );
        }
        
        public async Task UpdateEventAsync(UpdateEventDto dto)
        {
            var events = await _eventRepository.GetByIdAsync(dto.Id);
            if (events == null)
            {
                _logger.LogWarning("Attempt to update non-existing event Id {EventId}", dto.Id);
                return;
            }
            _logger.LogInformation("Updating event Id {EventId}", dto.Id);

            if (dto.StartTime >= dto.EndTime)
            {
                _logger.LogWarning("Invalid update time for Event {EventId}", dto.Id);
                return;
            }
            events.Title = dto.Title;
            events.Description = dto.Description;
            events.StartTime = dto.StartTime;
            events.EndTime = dto.EndTime;

            _eventRepository.Update(events);
            await _eventRepository.SaveChangesAsync();

        }
        public async Task DeleteEventAsync(int id)
        {
            var events = await _eventRepository.GetByIdAsync(id);
            if (events != null)
            {
                _eventRepository.Delete(events);
                await _eventRepository.SaveChangesAsync();
                _logger.LogInformation("Deleting event Id {EventId}", id);
            }
            else
            {
                _logger.LogWarning("Attempt to delete non-existing event Id {EventId}", id);
            }
        }

        private async Task<(DateTime start, DateTime end)> GetNextAvailableSlot(
    int roomId,
    DateTime requestedStart,
    DateTime requestedEnd)
        {
            var duration = requestedEnd - requestedStart;

            var events = await _eventRepository.GetQueryable()
                .Where(e => e.RoomId == roomId && e.EndTime >= requestedStart)
                .OrderBy(e => e.StartTime)
                .ToListAsync();

           
            if (!events.Any())
                return (requestedStart, requestedStart.Add(duration));

          
            for (int i = 0; i < events.Count - 1; i++)
            {
                var currentEnd = events[i].EndTime;
                var nextStart = events[i + 1].StartTime;

                if (nextStart - currentEnd >= duration)
                {
                    return (currentEnd, currentEnd.Add(duration));
                }
            }
           
            var lastEventEnd = events.Last().EndTime;
            return (lastEventEnd, lastEventEnd.Add(duration));
        }

    }
}



