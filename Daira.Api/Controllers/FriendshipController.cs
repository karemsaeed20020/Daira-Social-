using Daira.Application.Interfaces.FriendshipModule;
using Daira.Application.Response.FriendshipModule;
using Daira.Application.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Daira.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FriendshipController : ControllerBase
    {
        private readonly IFriendShipService _friendShipService;
        public FriendshipController(IFriendShipService friendShipService)
        {
            _friendShipService = friendShipService;
        }
        //GetAll  Friend Request 
        [HttpGet("GetAll-friendRequest")]
        [ProducesResponseType(typeof(ResultResponse<FriendshipResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllFriendShip()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();
            var result = await _friendShipService.GetAllFriendRequests(userId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //GetAll  Friends 
        [HttpGet("GetAll-friends")]
        [ProducesResponseType(typeof(ResultResponse<FriendshipResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllFriends()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();
            var result = await _friendShipService.GetAllFriends(userId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //Send FriendRequest
        [HttpPost("request-friendship/{addresseeId}")]
        [ProducesResponseType(typeof(ResultResponse<FriendshipResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendFriendRequest(string addresseeId)
        {
            var requestId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (requestId is null) return Unauthorized();
            var result = await _friendShipService.SendFriendRequestAsync(requestId, addresseeId);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //Accept FriendRequest
        [HttpPut("accept-friendship/{id}")]
        [ProducesResponseType(typeof(ResultResponse<FriendshipResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AcceptFriendShip(Guid id)
        {
            var result = await _friendShipService.AcceptFriendRequest(id);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //Decline Friend Request 
        [HttpPut("decline-friendship/{id}")]
        [ProducesResponseType(typeof(ResultResponse<FriendshipResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeclineFriendRequest(Guid id)
        {
            var result = await _friendShipService.DeclineFriendRequest(id);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //UnFriend Friend Request 
        [HttpPost("unFriend/{id}")]
        [ProducesResponseType(typeof(ResultResponse<FriendshipResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UnFriend(Guid id)
        {
            var reult = await _friendShipService.UnFriend(id);
            if (!reult.Succeeded) return BadRequest(reult);
            return Ok(reult);
        }

    }
}
