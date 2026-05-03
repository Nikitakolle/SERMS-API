using SERMS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using SERMS.API.DTOs;

namespace SERMS.API.Interfaces
{
    public interface IUserService
    {
        
        Task<string> LoginAsync(LoginDto dto);
        Task<PagedResponseDto<UserResponseDto>> GetAllUsersAsync(PaginationParam param);
        Task<UserResponseDto> GetUserByIdAsync(int id);
        Task CreateUserAsync(CreateUserDto dto);
        Task UpdateUserAsync(UpdateUserDto dto);
        Task DeleteUserAsync(int id);
       
         
    }
}
