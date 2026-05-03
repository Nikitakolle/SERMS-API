using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SERMS.API.DTOs;
using SERMS.API.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly ILogger<RoomController> _logger;

    public RoomController(IRoomService roomService, ILogger<RoomController> logger)
    {
        _roomService = roomService;
        _logger = logger;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> GetAll([FromQuery] PaginationParam param)
    {
        _logger.LogInformation("Fetching rooms - Page {Page}, Size {Size}",
            param.PageNumber, param.PageSize);

        var rooms = await _roomService.GetAllRoomsAsync(param);

        return Ok(new ApiResponseDto<PagedResponseDto<RoomResponseDto>>
        {
            Success = true,
            Message = "Rooms fetched Successfully",
            Data = rooms
        });
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<RoomResponseDto>> GetById(int id)
    {
        _logger.LogInformation("Fetching room with Id {RoomId}", id);

        var r = await _roomService.GetRoomByIdAsync(id);

        if (r == null)
        {
            _logger.LogWarning("Room not found with Id {RoomId}", id);
            return NotFound("Room not found");
        }

        return Ok(new ApiResponseDto<RoomResponseDto>
        {
            Success = true,
            Message = "Room fetched Successfully",
            Data = r
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateRoomDto dto)
    {
        _logger.LogInformation("Creating room with Name {RoomName}", dto.Name);

        await _roomService.CreateRoomAsync(dto);

        return StatusCode(201, new ApiResponseDto<string>
        {
            Success = true,
            Message = "Room created successfully",
            Data = null
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, UpdateRoomDto dto)
    {
        if (id != dto.Id)
        {
            _logger.LogWarning("Room ID mismatch: URL {UrlId} vs Body {BodyId}", id, dto.Id);
            return BadRequest("Id mismatch");
        }

        _logger.LogInformation("Updating room Id {RoomId}", id);

        await _roomService.UpdateRoomAsync(dto);

        return Ok(new ApiResponseDto<string>
        {
            Success = true,
            Message = "Room updated successfully",
            Data = null
        });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting room Id {RoomId}", id);

        await _roomService.DeleteRoomAsync(id);

        return Ok(new ApiResponseDto<string>
        {
            Success = true,
            Message = "Room deleted successfully",
            Data = null
        });
    }
}