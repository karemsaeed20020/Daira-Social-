using Daira.Application.DTOs.PostModule;
using Daira.Application.Interfaces.PostModule;
using Daira.Application.Response.PostModule;
using Daira.Application.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Daira.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }
        // Create Post
        [HttpPost("create-post")]
        [ProducesResponseType(typeof(ResultResponse<PostResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePost(CreatePostDto createPostDto)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user is null)
            {
                return Unauthorized();
            }
            var result = await _postService.CreatePostAsync(user, createPostDto);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);


        }

        //GetById
        [HttpGet("get-post/{id}")]
        [ProducesResponseType(typeof(ResultResponse<PostResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _postService.GetPostByIdAsync(id);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //update Post
        [HttpPut("update-post/{id}")]
        [ProducesResponseType(typeof(ResultResponse<PostResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePost(Guid id, UpdatePostDto updatePostDto)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user is null)
            {
                return Unauthorized();
            }
            var result = await _postService.UpdatePostAsync(user, id, updatePostDto);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }

        //Delete Post
        [HttpDelete("delete-post/{id}")]
        [ProducesResponseType(typeof(ResultResponse<PostResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user is null)
            {
                return Unauthorized();
            }
            var result = await _postService.DeletePostAsync(user, id);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);

        }

        //GetPosts for specific user
        [HttpGet("get-posts")]
        [ProducesResponseType(typeof(ResultResponse<PostResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPostsForSpecificUser()
        {
            var user = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (user is null)
            {
                return Unauthorized();
            }
            var result = await _postService.GetAllPostForSpecificUser(user);
            if (!result.Succeeded) return BadRequest(result);
            return Ok(result);
        }
    }
}
