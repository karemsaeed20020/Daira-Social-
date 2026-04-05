using Daira.Application.DTOs.RolesDto;
using Daira.Application.Interfaces.Auth;
using Daira.Application.Response.Roles;
using Microsoft.AspNetCore.Mvc;

namespace Daira.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
