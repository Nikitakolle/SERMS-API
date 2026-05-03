using Microsoft.EntityFrameworkCore;
using SERMS.API.DTOs;
using SERMS.API.Interfaces;
using SERMS.Domain.Entities;
using SERMS.Domain.Interfaces.Repositories;

public class EventParticipantService : IEventParticipantService
{
    private readonly ILogger<EventParticipantService> _logger;
    private readonly IEventParticipantRepository _repo;
    private readonly IEventRepository _eventRepository;

    public EventParticipantService(
        IEventParticipantRepository repo,
        IEventRepository eventRepository,
        ILogger<EventParticipantService> logger)
    {
        _repo = repo;
        _eventRepository = eventRepository;
        _logger = logger;
    }

    public async Task<ApiResponseDto<string>> JoinEventAsync(int eventId, int userId)
    {
        _logger.LogInformation("JoinEvent called. EventId: {EventId}, UserId: {UserId}", eventId, userId);

        var eventData = await _eventRepository.GetByIdAsync(eventId);
        if (eventData == null)
        {
            _logger.LogWarning("Event not found with Id {EventId}", eventId);

            return ApiResponseDto<string>.FailResponse("Event not found");
        }

        var exists = await _repo.ExistsAsync(eventId, userId);
        if (exists)
        {
            _logger.LogWarning("User {UserId} already joined event {EventId}", userId, eventId);

            return ApiResponseDto<string>.FailResponse("User already joined this event");
        }

        var participants = await _repo.GetByEventIdAsync(eventId);

        if (participants.Count() >= eventData.Room.Capacity)
        {
            _logger.LogWarning("Event {EventId} is full", eventId);

            return ApiResponseDto<string>.FailResponse("Event is full");
        }

        var entity = new EventParticipant
        {
            EventId = eventId,
            UserId = userId
        };

        await _repo.AddAsync(entity);

        _logger.LogInformation("User {UserId} joined event {EventId} successfully", userId, eventId);

        return ApiResponseDto<string>.SuccessResponse("Joined successfully", null);
    }

    public async Task<ApiResponseDto<PagedResponseDto<UserResponseDto>>> GetParticipantByEventAsync(int eventId, PaginationParam param)
    {
        _logger.LogInformation("Fetching participants for Event {EventId}", eventId);

        var query = _repo.GetQueryable()
            .Where(x => x.EventId == eventId)
            .Select(x => x.User);

        var totalCount = await query.CountAsync();

        var users = await query
            .Skip((param.PageNumber - 1) * param.PageSize)
            .Take(param.PageSize)
            .ToListAsync();

        var data = users.Select(u => new UserResponseDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email
        });

        return ApiResponseDto<PagedResponseDto<UserResponseDto>>.SuccessResponse(
            "Participants fetched successfully",
            new PagedResponseDto<UserResponseDto>
            {
                Data = data,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize,
                TotalCount = totalCount
            }
        );
    }

    public async Task<ApiResponseDto<PagedResponseDto<EventResponseDto>>> GetEventsByUserAsync(int userId, PaginationParam param)
    {
        _logger.LogInformation("Fetching events for User {UserId}", userId);

        var query = _repo.GetQueryable()
            .Where(x => x.UserId == userId)
            .Select(x => x.Event);

        var totalCount = await query.CountAsync();

        var events = await query
            .Skip((param.PageNumber - 1) * param.PageSize)
            .Take(param.PageSize)
            .ToListAsync();

        var data = events.Select(e => new EventResponseDto
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            StartTime = e.StartTime,
            EndTime = e.EndTime
        });

        return ApiResponseDto<PagedResponseDto<EventResponseDto>>.SuccessResponse(
            "User events fetched successfully",
            new PagedResponseDto<EventResponseDto>
            {
                Data = data,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize,
                TotalCount = totalCount
            }
        );
    }
}