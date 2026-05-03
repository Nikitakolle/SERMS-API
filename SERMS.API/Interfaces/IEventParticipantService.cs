using SERMS.Domain.Entities;
using SERMS.API.DTOs;

namespace SERMS.API.Interfaces
{
    public interface IEventParticipantService
    {
        Task <ApiResponseDto<string>> JoinEventAsync(int eventId, int userId);
        Task<ApiResponseDto<PagedResponseDto<UserResponseDto>>> GetParticipantByEventAsync(int eventId, PaginationParam param);
        Task<ApiResponseDto<PagedResponseDto<EventResponseDto>>> GetEventsByUserAsync(int userId, PaginationParam param);

    }
}
