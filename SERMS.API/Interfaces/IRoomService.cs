using SERMS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using SERMS.API.DTOs;
using SERMS.API.Services;

namespace SERMS.API.Interfaces
{
    public interface IRoomService
    {
        Task<PagedResponseDto<RoomResponseDto>> GetAllRoomsAsync(PaginationParam param);
        Task<RoomResponseDto> GetRoomByIdAsync(int id);
        Task CreateRoomAsync(CreateRoomDto dto);
        Task UpdateRoomAsync(UpdateRoomDto dto);
        Task DeleteRoomAsync(int id);

    }
}
