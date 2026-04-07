using Daira.Application.DTOs.PostModule;
using Daira.Application.Response.LikeModule;
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

        //LikePost
        Task<ResultResponse<LikeResponse>> LikePostAsync(string userId, Guid postId);

        //UnLikePost
        Task<ResultResponse<LikeResponse>> UnLikePostAsync(string userId, Guid postId);

        //Get Post Likes
        Task<ResultResponse<LikeResponse>> GetPostLikesAsync(Guid postId);

    }
}
