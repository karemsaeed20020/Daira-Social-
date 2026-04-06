using Daira.Application.DTOs.RolesDto;
using Daira.Application.Interfaces.Auth;
using Daira.Application.Response.Roles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Daira.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }
        //GetAllRoles
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoleResponse>> GetRoles()
        {
            var result = await _roleService.GetAllRolesAsync();
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //GetRoleById
        [HttpGet("getById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoleResponse>> GetById(string id)
        {
            var result = await _roleService.GetRoleByIdAsync(id);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }


        //GetRoleByName
        [HttpGet("getByName/{Name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoleResponse>> GetByName(string Name)
        {
            var result = await _roleService.GetRoleByNameAsync(Name);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //CreateRole
        [HttpPost("CreateRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoleResponse>> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            var result = await _roleService.CreateRoleAsync(createRoleDto);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //GetRoleFor Specific User
        [HttpGet("GetUserRole/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserRolesResponse>> GetUserRole(string userId)
        {
            var result = await _roleService.GetUserRolesAsync(userId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }


        //GetUsersInRole
        [HttpGet("UsersInRole/{Name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UsersInRoleResponse>> GetUsersInRole(string Name)
        {
            var result = await _roleService.GetUsersInRoleAsync(Name);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //RemoveRoleFromUser
        [HttpDelete("user/{userId}/remove/{Name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AssignRoleResponse>> RemoveRoleFromUser(string userId, string Name)
        {
            var result = await _roleService.RemoveRoleFromUserAsync(userId, Name);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //AssignRoleToUser
        [HttpPost("AssignRole/{Name}/User/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AssignRoleResponse>> AssignRoleToUser(string userId, string Name)
        {
            var result = await _roleService.AssignRoleToUserAsync(userId, Name);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //DeleteRole
        [HttpDelete("deleteRole/{Name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoleResponse>> DeleteRole(string Name)
        {
            var result = await _roleService.DeleteRoleAsync(Name);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //UpdateRole
        [HttpPut("updateRole")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateRole(string roleId, [FromBody] UpdateRoleDto updateRoleDto)
        {
            var result = await _roleService.UpdateRoleAsync(roleId, updateRoleDto);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }


    }
}
