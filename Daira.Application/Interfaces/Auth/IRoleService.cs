using Daira.Application.DTOs.RolesDto;
using Daira.Application.Response.Roles;

namespace Daira.Application.Interfaces.Auth
{
    public interface IRoleService
    {
        Task<RoleListResponse> GetAllRolesAsync();
        Task<RoleResponse> GetRoleByIdAsync(string roleId);
        Task<RoleResponse> GetRoleByNameAsync(string roleName);
        Task<RoleResponse> CreateRoleAsync(CreateRoleDto dto);
        Task<RoleResponse> UpdateRoleAsync(string roleId, UpdateRoleDto dto);
        Task<RoleResponse> DeleteRoleAsync(string roleId);
    }
}
