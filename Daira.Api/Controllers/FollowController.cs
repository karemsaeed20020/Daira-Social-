using Daira.Application.Interfaces.FollowerModule;
using Daira.Application.Response.FollowerModule;
using Daira.Application.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Daira.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly IFolloweService _followeService;
        public FollowController(IFolloweService followeService)
        {
            _followeService = followeService;
        }
        //Follow User
        [HttpPost("follow-user/{id}")]
        [ProducesResponseType(typeof(ResultResponse<FollowerResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FollowUser(string id)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user is null)
            {
                return Unauthorized();
            }
            var result = await _followeService.FollowUserAsync(user, id);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //UnFollow User
        [HttpPost("unfollow-user/{id}")]
        [ProducesResponseType(typeof(ResultResponse<FollowerResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UnFollowUser(string id)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user is null)
            {
                return Unauthorized();
            }
            var result = await _followeService.UnFollowUserAsync(user, id);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //Get All Followers of a User
        [HttpGet("get-followers")]
        [ProducesResponseType(typeof(ResultResponse<FollowerResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFollowers()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
            {
                return Unauthorized();
            }
            var result = await _followeService.GetFollowersAsync(UserId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //Get All Following of a User
        [HttpGet("get-following")]
        [ProducesResponseType(typeof(ResultResponse<FollowerResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFollowing()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId is null)
            {
                return Unauthorized();
            }
            var result = await _followeService.GetFollowingAsync(UserId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
    }
}
