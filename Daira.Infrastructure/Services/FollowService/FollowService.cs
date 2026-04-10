using AutoMapper;
using Daira.Application.Interfaces;
using Daira.Application.Interfaces.FollowerModule;
using Daira.Application.Response.FollowerModule;
using Daira.Application.Shared;
using Daira.Infrastructure.Specefication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Daira.Infrastructure.Services.FollowService
{
    public class FollowService : IFolloweService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<FollowService> _logger;
        private readonly UserManager<AppUser> _userManager;
        public FollowService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<FollowService> logger, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
        }
        // Follow User
        public async Task<ResultResponse<FollowerResponse>> FollowUserAsync(string followerId, string followeeId)
        {
            if (followeeId == followerId)
            {
                _logger.LogWarning("FollowUserAsync: User {FollowerId} attempted to follow themselves.", followerId);
                return ResultResponse<FollowerResponse>.Failure("You cannot follow yourself.");
            }
            var followee = await _userManager.FindByIdAsync(followeeId);
            if (followee is null)
            {
                _logger.LogWarning("FollowUserAsync: Followee with ID {FolloweeId} not found.", followeeId);
                return ResultResponse<FollowerResponse>.Failure("Followee not found.");
            }
            var spec = new FollowerSpecification(f => f.FollowerId == followerId && f.FollowingId == followeeId);
            var followExist = await _unitOfWork.Repository<Follower>().GetByIdSpecTracked(spec);
            if (followExist is not null)
            {
                _logger.LogInformation("FollowUserAsync: User {FollowerId} already follows {FolloweeId}.", followerId, followeeId);
                return ResultResponse<FollowerResponse>.Failure("You are already following this user.");
            }
            var follow = new Follower
            {
                FollowerId = followerId,
                FollowingId = followeeId,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Follower>().AddAsync(follow);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("FollowUserAsync: User {FollowerId} successfully followed {FolloweeId}.", followerId, followeeId);
            var followDto = _mapper.Map<FollowerResponse>(follow);
            return ResultResponse<FollowerResponse>.Success(followDto, "Successfully followed the user.");
        }

        public async Task<ResultResponse<FollowerResponse>> GetFollowersAsync(string userId)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);
            if (existingUser is null)
            {
                _logger.LogWarning("GetFollowersAsync: User with ID {UserId} not found.", userId);
                return ResultResponse<FollowerResponse>.Failure("User not found.");
            }
            var spec = new FollowerSpecification(f => f.FollowingId == userId);
            var followers = await _unitOfWork.Repository<Follower>().GetAllWithSpec(spec);
            if (!followers.Any())
            {
                _logger.LogInformation("GetFollowersAsync: User {UserId} has no followers.", userId);
                return ResultResponse<FollowerResponse>.Success(new List<FollowerResponse>(), "User has no followers.");
            }
            var followerDtos = _mapper.Map<List<FollowerResponse>>(followers);
            _logger.LogInformation("GetFollowersAsync: Retrieved {Count} followers for user {UserId}.", followerDtos.Count, userId);
            return ResultResponse<FollowerResponse>.Success(followerDtos, "Followers retrieved successfully.");
        }

        public async Task<ResultResponse<FollowerResponse>> GetFollowingAsync(string userId)
        {
            var existingUser = await _userManager.FindByIdAsync(userId);
            if (existingUser is null)
            {
                _logger.LogWarning("GetFollowingAsync: User with ID {UserId} not found.", userId);
                return ResultResponse<FollowerResponse>.Failure("User not found.");
            }
            var spec = new FollowerSpecification(f => f.FollowerId == userId);
            var followers = await _unitOfWork.Repository<Follower>().GetAllWithSpec(spec);
            if (!followers.Any())
            {
                _logger.LogInformation("GetFollowingAsync: User {UserId} has no followers.", userId);
                return ResultResponse<FollowerResponse>.Success(new List<FollowerResponse>(), "User has no Following.");
            }
            var followerDtos = _mapper.Map<List<FollowerResponse>>(followers);
            _logger.LogInformation("GetFollowingAsync: Retrieved {Count} followers for user {UserId}.", followerDtos.Count, userId);
            return ResultResponse<FollowerResponse>.Success(followerDtos, "Following retrieved successfully.");
        }
        // UnFollow User
        public async Task<ResultResponse<FollowerResponse>> UnFollowUserAsync(string followerId, string followeeId)
        {
            if (followeeId == followerId)
            {
                _logger.LogWarning("UnFollowUserAsync: User {FollowerId} attempted to unfollow themselves.", followerId);
                return ResultResponse<FollowerResponse>.Failure("You cannot unfollow yourself.");
            }
            var follower = await _userManager.FindByIdAsync(followerId);
            if (follower is null)
            {
                _logger.LogWarning(" Follower with ID {FollowerId} not found.", followerId);
                return ResultResponse<FollowerResponse>.Failure("Follower not found.");
            }
            var followee = await _userManager.FindByIdAsync(followeeId);
            if (followee is null)
            {
                _logger.LogWarning(" Followee with ID {FolloweeId} not found.", followeeId);
                return ResultResponse<FollowerResponse>.Failure("Followee not found.");
            }
            var spec = new FollowerSpecification(f => f.FollowerId == followerId && f.FollowingId == followeeId);
            var followExist = await _unitOfWork.Repository<Follower>().GetByIdSpecTracked(spec);
            if (followExist is null)
            {
                _logger.LogInformation(" User {FollowerId} does not follow {FolloweeId}.", followerId, followeeId);
                return ResultResponse<FollowerResponse>.Failure("You are not following this user.");
            }
            _unitOfWork.Repository<Follower>().Delete(followExist);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation(" User {FollowerId} successfully unfollowed {FolloweeId}.", followerId, followeeId);
            return ResultResponse<FollowerResponse>.Success("Successfully unfollowed the user.");
        }
    }
}
