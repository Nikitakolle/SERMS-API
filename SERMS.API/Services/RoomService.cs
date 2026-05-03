using Microsoft.EntityFrameworkCore;
using SERMS.API.DTOs;
using SERMS.API.Interfaces;
using SERMS.Domain.Entities;
using SERMS.Domain.Interfaces.Repositories;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly ILogger<RoomService> _logger;

    public RoomService(IRoomRepository roomRepository, ILogger<RoomService> logger)
    {
        _roomRepository = roomRepository;
        _logger = logger;
    }

    public async Task<PagedResponseDto<RoomResponseDto>> GetAllRoomsAsync(PaginationParam param)
    {
        _logger.LogInformation("Fetching all rooms with pagination Page {Page}, Size {Size}",
            param.PageNumber, param.PageSize);

        var query = _roomRepository.GetQueryable();

        var totalCount = await query.CountAsync();

        var rooms = await query
            .Skip((param.PageNumber - 1) * param.PageSize)
            .Take(param.PageSize)
            .ToListAsync();

        var data = rooms.Select(r => new RoomResponseDto
        {
            Id = r.Id,
            Name = r.Name,
            Capacity = r.Capacity,
            Equipment = r.Equipment
        });

        return new PagedResponseDto<RoomResponseDto>
        {
            Data = data,
            PageNumber = param.PageNumber,
            PageSize = param.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<RoomResponseDto> GetRoomByIdAsync(int id)
    {
        var rm = await _roomRepository.GetByIdAsync(id);

        if (rm == null)
        {
            _logger.LogWarning("Room not found with Id {RoomId}", id);
            return null;
        }

        return new RoomResponseDto
        {
            Id = rm.Id,
            Name = rm.Name,
            Capacity = rm.Capacity,
            Equipment = rm.Equipment
        };
    }

    public async Task CreateRoomAsync(CreateRoomDto dto)
    {
        var room = new Room
        {
            Name = dto.Name,
            Capacity = dto.Capacity,
            Equipment = dto.Equipment
        };

        await _roomRepository.AddAsync(room);
        await _roomRepository.SaveChangesAsync();

        _logger.LogInformation("Room created successfully with Name {RoomName}", dto.Name);
    }

    public async Task UpdateRoomAsync(UpdateRoomDto dto)
    {
        var rms = await _roomRepository.GetByIdAsync(dto.Id);

        if (rms == null)
        {
            _logger.LogWarning("Attempt to update non-existing room Id {RoomId}", dto.Id);
            return;
        }

        rms.Name = dto.Name;
        rms.Capacity = dto.Capacity;
        rms.Equipment = dto.Equipment;

        _roomRepository.Update(rms);
        await _roomRepository.SaveChangesAsync();

        _logger.LogInformation("Room updated successfully with Id {RoomId}", dto.Id);
    }

    public async Task DeleteRoomAsync(int id)
    {
        var rm = await _roomRepository.GetByIdAsync(id);

        if (rm != null)
        {
            _roomRepository.Delete(rm);
            await _roomRepository.SaveChangesAsync();

            _logger.LogInformation("Room deleted successfully with Id {RoomId}", id);
        }
        else
        {
            _logger.LogWarning("Attempt to delete non-existing room Id {RoomId}", id);
        }
    }
}