using AutoMapper;
using Daira.Application.DTOs.PostModule;
using Daira.Application.Interfaces;
using Daira.Application.Interfaces.PostModule;
using Daira.Application.Response.LikeModule;
using Daira.Application.Response.PostModule;
using Daira.Application.Shared;
using Daira.Infrastructure.Specefication;
using Microsoft.Extensions.Logging;

namespace Daira.Infrastructure.Services.PostService
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public PostService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PostService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        // Create Post
        public async Task<ResultResponse<PostResponse>> CreatePostAsync(string userId, CreatePostDto createPostDto)
        {
            try
            {
                _logger.LogInformation("Creating post for user {UserId}", userId);
                var post = new Post
                {
                    Content = createPostDto.Content,
                    ImageUrl = createPostDto.ImageUrl,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<Post>().AddAsync(post);
                await _unitOfWork.CommitAsync();
                var spec = new PostSpecification(post.Id);
                var createdPost = await _unitOfWork.Repository<Post>().GetByIdSpecTracked(spec);
                if (createdPost is null)
                {
                    _logger.LogError("Failed to retrieve created post {PostId}", post.Id);
                    return ResultResponse<PostResponse>.Failure("Post could not be retrieved after creation.");
                }
                var postDto = _mapper.Map<PostResponse>(createdPost);
                _logger.LogInformation("Post {PostId} created successfully", post.Id);

                return ResultResponse<PostResponse>.Success(postDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post for user {UserId}", userId);
                return ResultResponse<PostResponse>.Failure("An unexpected error occurred while creating the post.");

            }
        }
        // Delete Post
        public async Task<ResultResponse<PostResponse>> DeletePostAsync(string userId, Guid postId)
        {
            var spec = new PostSpecification(postId);
            var getPost = await _unitOfWork.Repository<Post>().GetByIdSpecTracked(spec);
            if (getPost is null)
            {
                _logger.LogWarning("Post {PostId} not found for deletion", postId);
                return ResultResponse<PostResponse>.Failure("Post not found.");
            }
            if (getPost.User.Id != userId)
            {
                return ResultResponse<PostResponse>.Failure("You are not authorized to delete this post.");
            }
            _unitOfWork.Repository<Post>().Delete(getPost);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Post {PostId} deleted successfully", postId);
            return ResultResponse<PostResponse>.Success("Post deleted successfully.");
        }
        // Get All Posts for Specific User
        public async Task<ResultResponse<PostResponse>> GetAllPostForSpecificUser(string userId)
        {
            var spec = new PostSpecification(r => r.User.Id == userId);
            var getPosts = await _unitOfWork.Repository<Post>().GetAllWithSpec(spec);
            if (!getPosts.Any()) return ResultResponse<PostResponse>.Failure("No posts found for this user ");
            var postDtos = _mapper.Map<List<PostResponse>>(getPosts);
            return ResultResponse<PostResponse>.Success(postDtos);
        }


        // Get Post by Id
        public async Task<ResultResponse<PostResponse>> GetPostByIdAsync(Guid postId)
        {
            var spec = new PostSpecification(postId);
            var getPost = await _unitOfWork.Repository<Post>().GetByIdSpecTracked(spec);
            if (getPost is null)
            {
                _logger.LogWarning("Post {PostId} not found", postId);
                return ResultResponse<PostResponse>.Failure("Post not found ");
            }
            var postDto = _mapper.Map<PostResponse>(getPost);
            if (postDto is null)
            {
                _logger.LogError("Mapping failed for post {PostId}", postId);
                return ResultResponse<PostResponse>.Failure("Failed to map post data ");
            }
            return ResultResponse<PostResponse>.Success(postDto);
        }
        // Get Post Likes
        public async Task<ResultResponse<LikeResponse>> GetPostLikesAsync(Guid postId)
        {
            var existPost = await _unitOfWork.Repository<Post>().GetByIdAsync(postId);
            if (existPost is null)
            {
                _logger.LogWarning("Post {PostId} not found for retrieving likes", postId);
                return ResultResponse<LikeResponse>.Failure("Post not found.");
            }
            var spec = new LikeSpecification(l => l.PostId == postId);
            var likes = await _unitOfWork.Repository<Like>().GetAllWithSpec(spec);
            if (!likes.Any())
            {
                _logger.LogInformation("No likes found for post {PostId}", postId);
                return ResultResponse<LikeResponse>.Failure("No likes found for this post.");
            }
            var likeDtos = _mapper.Map<List<LikeResponse>>(likes);
            _logger.LogInformation("Retrieved {Count} likes for post {PostId}", likeDtos.Count, postId);
            return ResultResponse<LikeResponse>.Success(likeDtos, "Likes retrieved successfully ");

        }

        public async Task<ResultResponse<LikeResponse>> LikePostAsync(string userId, Guid postId)
        {
            var existPost = await _unitOfWork.Repository<Post>().GetByIdAsync(postId);
            if (existPost is null)
            {
                _logger.LogWarning("Post {PostId} not found for liking", postId);
                return ResultResponse<LikeResponse>.Failure("Post not found.");
            }
            var spec = new LikeSpecification(l => l.UserId == userId && l.PostId == postId);
            var existLike = await _unitOfWork.Repository<Like>().GetByIdSpec(spec);
            if (existLike is not null)
            {
                _logger.LogInformation("User {UserId} already liked post {PostId}", userId, postId);
                return ResultResponse<LikeResponse>.Failure("You have already liked this post ");
            }
            existPost.LikesCount += 1;
            var like = new Like
            {
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _unitOfWork.Repository<Post>().Update(existPost);
            await _unitOfWork.Repository<Like>().AddAsync(like);
            await _unitOfWork.CommitAsync();
            var likeDto = _mapper.Map<LikeResponse>(like);
            _logger.LogInformation("User {UserId} liked post {PostId} successfully", userId, postId);
            return ResultResponse<LikeResponse>.Success(likeDto, "Post liked successfully ");

        }

        public async Task<ResultResponse<LikeResponse>> UnLikePostAsync(string userId, Guid postId)
        {
            var existPost = await _unitOfWork.Repository<Post>().GetByIdAsync(postId);
            if (existPost is null)
            {
                _logger.LogWarning("Post {PostId} not found for unliking", postId);
                return ResultResponse<LikeResponse>.Failure("Post not found.");
            }
            var spec = new LikeSpecification(l => l.UserId == userId && l.PostId == postId);
            var existLike = await _unitOfWork.Repository<Like>().GetByIdSpec(spec);
            if (existLike is null)
            {
                _logger.LogInformation("User {UserId} has not liked post {PostId} to unlike", userId, postId);
                return ResultResponse<LikeResponse>.Failure("You have not liked this post yet.");
            }
            existPost.LikesCount -= 1;
            _unitOfWork.Repository<Post>().Update(existPost);
            _unitOfWork.Repository<Like>().Delete(existLike);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("User {UserId} unliked post {PostId} successfully", userId, postId);
            return ResultResponse<LikeResponse>.Success("Post unliked successfully ");
        }



        // Update Post
        public async Task<ResultResponse<PostResponse>> UpdatePostAsync(string userId, Guid postId, UpdatePostDto updatePostDto)
        {
            var spec = new PostSpecification(postId);
            var getPost = await _unitOfWork.Repository<Post>().GetByIdSpecTracked(spec);
            if (getPost is null)
            {
                _logger.LogWarning("Post {PostId} not found for update", postId);
                return ResultResponse<PostResponse>.Failure("Post not found ");
            }
            if (getPost.User.Id != userId)
            {
                _logger.LogWarning("User {UserId} unauthorized to update post {PostId}", userId, postId);
                return ResultResponse<PostResponse>.Failure("You are not authorized to update this post.");
            }
            getPost.Content = updatePostDto.Content ?? getPost.Content;
            getPost.ImageUrl = updatePostDto.ImageUrl ?? getPost.ImageUrl;
            getPost.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();
            var mapPost = _mapper.Map<PostResponse>(getPost);
            return ResultResponse<PostResponse>.Success(mapPost);
        }
    }
}
