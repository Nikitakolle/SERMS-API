using SERMS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using SERMS.API.DTOs;

namespace SERMS.API.Interfaces
{
    public interface IEventService
    {
        
        Task<PagedResponseDto<EventResponseDto>> GetAllEventAsync(PaginationParam param );
        Task<EventResponseDto> GetEventByIdAsync(int id);
        Task<ApiResponseDto<EventResponseDto>> CreateEventAsync(CreateEventDto dto, int id);
        Task UpdateEventAsync(UpdateEventDto dto);
        Task DeleteEventAsync(int id);

    }
}


