using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SERMS.API.DTOs;
using SERMS.API.Interfaces;
using SERMS.API.Services;
using System.Threading.Tasks;

namespace SERMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            _logger.LogInformation("Login attempt for email {Email}", dto.Email);

            var token = await _userService.LoginAsync(dto);

            if (token == null)
            {
                _logger.LogWarning("Login failed for email {Email}", dto.Email);
                return Unauthorized("Invalid email or password");
            }

            _logger.LogInformation("Login successful for email {Email}", dto.Email);

            return Ok(new ApiResponseDto<string>
            {
                Success = true,
                Message = "Login successful",
                Data = token
            });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAll([FromQuery] PaginationParam param)
        {
            _logger.LogInformation("Fetching users - Page {Page}, Size {Size}",
                param.PageNumber, param.PageSize);

            var users = await _userService.GetAllUsersAsync(param);

            return Ok(new ApiResponseDto<PagedResponseDto<UserResponseDto>>
            {
                Success = true,
                Message = "Users fetched Successfully",
                Data = users
            });
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserResponseDto>> GetById(int id)
        {
            _logger.LogInformation("Fetching user with Id {UserId}", id);

            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User not found with Id {UserId}", id);
                return NotFound("User not found");
            }

            return Ok(new ApiResponseDto<UserResponseDto>
            {
                Success = true,
                Message = "User fetched Successfully",
                Data = user
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            _logger.LogInformation("Creating user with email {Email}", dto.Email);

            await _userService.CreateUserAsync(dto);

            return StatusCode(201, new ApiResponseDto<string>
            {
                Success = true,
                Message = "User created successfully",
                Data = null
            });
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, UpdateUserDto dto)
        {
            if (id != dto.Id)
            {
                _logger.LogWarning("User ID mismatch: URL {UrlId} vs Body {BodyId}", id, dto.Id);
                return BadRequest("Id mismatch");
            }

            _logger.LogInformation("Updating user Id {UserId}", id);

            await _userService.UpdateUserAsync(dto);

            return Ok(new ApiResponseDto<string>
            {
                Success = true,
                Message = "User updated successfully",
                Data = null
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deleting user Id {UserId}", id);

            await _userService.DeleteUserAsync(id);

            return Ok(new ApiResponseDto<string>
            {
                Success = true,
                Message = "User deleted successfully",
                Data = null
            });
        }
    }

}



