using AutoMapper;
using Daira.Application.DTOs.CommentModule;
using Daira.Application.Interfaces;
using Daira.Application.Interfaces.CommentModule;
using Daira.Application.Shared;
using Daira.Infrastructure.Specefication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Daira.Infrastructure.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CommentService> _logger;
        private readonly UserManager<AppUser> _userManager;
        public CommentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CommentService> logger, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
        }
        public async Task<ResultResponse<CommentResponse>> AddComment(string userId, Guid postId, AddCommentDto addCommentDto)
        {
            var checkPost = await _unitOfWork.Repository<Post>().GetByIdAsync(postId);
            if (checkPost == null)
            {
                _logger.LogWarning("Post with ID {PostId} not found when adding comment by user {UserId}.", postId, userId);
                return ResultResponse<CommentResponse>.Failure("Post not found.");
            }
            var checkUser = await _userManager.FindByIdAsync(userId);
            if (checkUser == null)
            {
                _logger.LogWarning("Post with ID {PostId} not found when adding comment by user {UserId}.", postId, userId);
                return ResultResponse<CommentResponse>.Failure("Post not found.");
            }
            var newComment = new Comment
            {
                Content = addCommentDto.Content,
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };
            checkPost.CommentsCount += 1;
            _unitOfWork.Repository<Post>().Update(checkPost);
            await _unitOfWork.Repository<Comment>().AddAsync(newComment);
            await _unitOfWork.CommitAsync();
            var mapComment = _mapper.Map<CommentResponse>(newComment);
            return ResultResponse<CommentResponse>.Success(mapComment, "Comment added successfully.");

        }

        //DeleteComment
        public async Task<ResultResponse<CommentResponse>> DeleteComment(string userId, Guid commentId)
        {
            var checkCommnet = await _unitOfWork.Repository<Comment>().GetByIdTrackedAsync(commentId);
            if (checkCommnet is null)
            {
                _logger.LogWarning("Comment with ID {CommentId} not found when deleting by user {UserId}.", commentId, userId);
                return ResultResponse<CommentResponse>.Failure("Comment not found.");
            }
            if (checkCommnet.UserId != userId)
            {
                _logger.LogWarning("User with ID {UserId} attempted to delete comment {CommentId} without permission.", userId, commentId);
                return ResultResponse<CommentResponse>.Failure("You do not have permission to delete this comment.");
            }
            var getpost = await _unitOfWork.Repository<Post>().GetByIdAsync(checkCommnet.PostId);
            if (getpost != null)
            {
                getpost.CommentsCount -= 1;
                _unitOfWork.Repository<Post>().Update(getpost);
            }
            var mapDeletedComment = _mapper.Map<CommentResponse>(checkCommnet);
            _unitOfWork.Repository<Comment>().Delete(checkCommnet);
            await _unitOfWork.CommitAsync();

            return ResultResponse<CommentResponse>.Success(mapDeletedComment, "Comment deleted successfully.");
        }

        // GetCommentsByPostId
        public async Task<ResultResponse<CommentResponse>> GetCommentsByPostId(Guid postId)
        {
            var comments = new CommentSpecification(c => c.PostId == postId);
            var commentList = await _unitOfWork.Repository<Comment>().GetAllWithSpec(comments);
            if (!commentList.Any())
            {
                _logger.LogInformation("No comments found for post with ID {PostId}.", postId);
                return ResultResponse<CommentResponse>.Failure("No comments found for this post.");
            }
            var mapList = _mapper.Map<List<CommentResponse>>(commentList);
            return ResultResponse<CommentResponse>.Success(mapList, "Comments retrieved successfully.");
        }

        public async Task<ResultResponse<CommentResponse>> UpdateComment(string userId, Guid commentId, AddCommentDto updateCommentDto)
        {
            var checkComment = await _unitOfWork.Repository<Comment>().GetByIdAsync(commentId);
            if (checkComment is null)
            {
                _logger.LogWarning("Comment with ID {CommentId} not found when updating by user {UserId}.", commentId, userId);
                return ResultResponse<CommentResponse>.Failure("Comment not found.");
            }
            if (checkComment.UserId != userId)
            {
                _logger.LogWarning("User with ID {UserId} attempted to update comment {CommentId} without permission.", userId, commentId);
                return ResultResponse<CommentResponse>.Failure("You do not have permission to update this comment.");
            }
            checkComment.Content = updateCommentDto.Content;
            checkComment.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Comment>().Update(checkComment);
            await _unitOfWork.CommitAsync();
            var mapComment = _mapper.Map<CommentResponse>(checkComment);
            return ResultResponse<CommentResponse>.Success(mapComment, "Comment updated successfully.");
        }
    }
}
