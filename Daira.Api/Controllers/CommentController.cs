using Daira.Application.DTOs.CommentModule;
using Daira.Application.Interfaces.CommentModule;
using Daira.Application.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Daira.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }
        //Get Comments by Post Id
        [HttpGet("get-comments/{postId}")]
        [ProducesResponseType(typeof(IEnumerable<ResultResponse<CommentResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCommentsByPostId(Guid postId)
        {
            var result = await _commentService.GetCommentsByPostId(postId);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        //Add Comment
        [HttpPost("add-comment/{id}")]
        [ProducesResponseType(typeof(ResultResponse<CommentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddComment(Guid id, [FromBody] AddCommentDto addCommentDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _commentService.AddComment(userId!, id, addCommentDto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //Delete Comment
        [HttpDelete("delete-comment/{id}")]
        [ProducesResponseType(typeof(ResultResponse<CommentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();
            var result = await _commentService.DeleteComment(userId, id);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //Update Comment 
        [HttpPut("update-commnet/{id}")]
        [ProducesResponseType(typeof(ResultResponse<CommentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] AddCommentDto updateCommentDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();
            var result = await _commentService.UpdateComment(userId, id, updateCommentDto);
            if (!result.Succeeded)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

