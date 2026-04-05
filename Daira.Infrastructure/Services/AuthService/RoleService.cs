using Daira.Application.DTOs.RolesDto;
using Daira.Application.Interfaces;
using Daira.Application.Interfaces.Auth;
using Daira.Application.Response.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Daira.Infrastructure.Services.AuthService
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RoleService> _logger;
        public RoleService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IUnitOfWork unitOfWork, ILogger<RoleService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<RoleResponse> CreateRoleAsync(CreateRoleDto dto)
        {
            if (await _roleManager.RoleExistsAsync(dto.Name))
            {
                return RoleResponse.Failure($"Role '{dto.Name}' already exists.");
            }
            var role = new AppRole(dto.Name, dto.Description ?? string.Empty);
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                return RoleResponse.Failure(result.Errors.Select(e => e.Description));
            }
            _logger.LogInformation("Created role: {RoleName}", dto.Name);
            return RoleResponse.Success(MapToDto(role), $"Role '{dto.Name}' created successfully.");
        }

        public Task<RoleResponse> DeleteRoleAsync(string roleId)
        {
            throw new NotImplementedException();
        }

        public async Task<RoleListResponse> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.OrderBy(r => r.Name).Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name!,
                NormalizedName = r.NormalizedName,
                Description = r.Description,
            }).ToListAsync();

            return RoleListResponse.Success(roles);
        }

        public async Task<RoleResponse> GetRoleByIdAsync(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return RoleResponse.Failure("Role not found.");
            }
            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name!,
                NormalizedName = role.NormalizedName,
                Description = role.Description,
            };
            return RoleResponse.Success(roleDto);
        }

        public async Task<RoleResponse> GetRoleByNameAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is null) return RoleResponse.Failure("Role not found.");
            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name!,
                NormalizedName = role.NormalizedName,
                Description = role.Description,
            };
            return RoleResponse.Success(roleDto);
        }

        //Private
        private static RoleDto MapToDto(AppRole role)
        {
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name!,
                NormalizedName = role.NormalizedName,
                Description = role.Description,

            };
        }

        public async Task<RoleResponse> UpdateRoleAsync(string roleId, UpdateRoleDto dto)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return RoleResponse.Failure("Role not found.");
            }

            // Check if new name conflicts with existing role
            if (!string.IsNullOrEmpty(dto.Name) && dto.Name != role.Name)
            {
                if (await _roleManager.RoleExistsAsync(dto.Name))
                {
                    return RoleResponse.Failure($"Role '{dto.Name}' already exists.");
                }
                role.Name = dto.Name;
            }

            if (dto.Description != null)
            {
                role.Description = dto.Description;
            }


            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                return RoleResponse.Failure(result.Errors.Select(e => e.Description));
            }

            _logger.LogInformation("Updated role: {RoleId}", roleId);
            return RoleResponse.Success(MapToDto(role), "Role updated successfully.");
        }
    }
}
