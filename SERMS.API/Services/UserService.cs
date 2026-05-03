using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SERMS.API.DTOs;
using SERMS.API.Interfaces;
using SERMS.Domain.Entities;
using SERMS.Domain.Interfaces.Repositories;
using SERMS.Infrastructure.Repositories;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace SERMS.API.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IConfiguration configuration, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
            _logger = logger;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null)
            {
                _logger.LogWarning("Login failed - user not found for email {Email}", dto.Email);
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning("Login failed - invalid password for email {Email}", dto.Email);
                return null;
            }

            _logger.LogInformation("Generating token for user {UserId}", user.Id);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<PagedResponseDto<UserResponseDto>> GetAllUsersAsync(PaginationParam param)
        {
            _logger.LogInformation("Fetching users with pagination Page {Page}, Size {Size}",
                param.PageNumber, param.PageSize);

            var query = _userRepository.GetQueryable();
            var totalCount = await query.CountAsync();

            var users = await query
                .Skip((param.PageNumber - 1) * param.PageSize)
                .Take(param.PageSize)
                .ToListAsync();

            var data = users.Select(u => new UserResponseDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role.ToString()
            });

            return new PagedResponseDto<UserResponseDto>
            {
                Data = data,
                PageNumber = param.PageNumber,
                PageSize = param.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User not found with Id {UserId}", id);
                return null;
            }

            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task CreateUserAsync(CreateUserDto dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Role = Enum.Parse<Role>(dto.Role)
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("User created successfully with Email {Email}", dto.Email);
        }

        public async Task UpdateUserAsync(UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(dto.Id);

            if (user == null)
            {
                _logger.LogWarning("Attempt to update non-existing user Id {UserId}", dto.Id);
                return;
            }

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.Role = Enum.Parse<Role>(dto.Role);
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("User updated successfully with Id {UserId}", dto.Id);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user != null)
            {
                _userRepository.Delete(user);
                await _userRepository.SaveChangesAsync();

                _logger.LogInformation("User deleted successfully with Id {UserId}", id);
            }
            else
            {
                _logger.LogWarning("Attempt to delete non-existing user Id {UserId}", id);
            }
        }
    }
}
