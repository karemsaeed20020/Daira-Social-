using Daira.Application.DTOs.PostModule;
using Daira.Application.Response.PostModule;
using Daira.Application.Shared;

namespace Daira.Application.Interfaces.PostModule
{
    public interface IPostService
    {
        // Create Post
        Task<ResultResponse<PostResponse>> CreatePostAsync(string userId, CreatePostDto createPostDto);
        // Get Post by Id
        Task<ResultResponse<PostResponse>> GetPostByIdAsync(Guid postId);
        // Update Post
        Task<ResultResponse<PostResponse>> UpdatePostAsync(string userId, Guid postId, UpdatePostDto updatePostDto);
        // Delete Post
        Task<ResultResponse<PostResponse>> DeletePostAsync(string userId, Guid postId);
        // Get All Posts For Specific User
        Task<ResultResponse<PostResponse>> GetAllPostForSpecificUser(string userId);

    }
}
