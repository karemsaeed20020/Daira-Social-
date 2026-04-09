using AutoMapper;
using Daira.Application.Interfaces;
using Daira.Application.Interfaces.FriendshipModule;
using Daira.Application.Response.FriendshipModule;
using Daira.Application.Shared;
using Daira.Infrastructure.Specefication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Daira.Infrastructure.Services.FriendShipService
{
    public class FriendShipService : IFriendShipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly UserManager<AppUser> _userManager;
        public FriendShipService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<FriendShipService> logger, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
        }
        public async Task<ResultResponse<FriendshipResponse>> AcceptFriendRequest(Guid id)
        {
            var existFriendShip = await _unitOfWork.Repository<Friendship>().GetByIdAsync(id);
            if (existFriendShip is null)
            {
                _logger.LogWarning("AcceptFriendAsync:FriendShip is nopt Found");
                return ResultResponse<FriendshipResponse>.Failure("FriendShip is not found");
            }
            if (existFriendShip.Status == RequestStatus.Declined || existFriendShip.Status == RequestStatus.Accepted || existFriendShip.Status == RequestStatus.Blocked)
            {
                _logger.LogWarning("AcceptFriendShipAsync: cant Accept This friendShip due To Request Status");
                return ResultResponse<FriendshipResponse>.Failure("Cannot Accept This FriendShip due To Request Status ");
            }
            existFriendShip.Status = RequestStatus.Accepted;
            existFriendShip.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Friendship>().Update(existFriendShip);
            await _unitOfWork.CommitAsync();
            return ResultResponse<FriendshipResponse>.Success("Successfully Accept Request the user");
        }

        public async Task<ResultResponse<FriendshipResponse>> DeclineFriendRequest(Guid id)
        {

            var existFriendShip = await _unitOfWork.Repository<Friendship>().GetByIdAsync(id);
            if (existFriendShip is null)
            {
                _logger.LogWarning("Decline FrinedShip Async :FriendShip is not Found");
                return ResultResponse<FriendshipResponse>.Failure("FriendShip is not found");
            }
            if (existFriendShip.Status == RequestStatus.Accepted || existFriendShip.Status == RequestStatus.Blocked)
            {
                _logger.LogWarning("Decline FrinedShip Async: cant Decline This friendShip due To Request Status");
                return ResultResponse<FriendshipResponse>.Failure("Cannot Decline This FriendShip due To Request Status ");
            }
            existFriendShip.Status = RequestStatus.Declined;
            existFriendShip.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Friendship>().Update(existFriendShip);

            await _unitOfWork.CommitAsync();
            return ResultResponse<FriendshipResponse>.Success("Successfully Decline Request the user");
        }

        public async Task<ResultResponse<FriendshipResponse>> GetAllFriendRequests(string userId)
        {
            var existUser = await _userManager.FindByIdAsync(userId);
            if (existUser is null)
            {
                _logger.LogWarning("GetAllFriendRequest: {userId} Not Found ", userId);
                return ResultResponse<FriendshipResponse>.Failure("User Not Found");
            }
            var spec = new FriendshipSpecification(f => f.AddresseeId == userId && f.Status == RequestStatus.Pending);
            var listFriendships = await _unitOfWork.Repository<Friendship>().GetAllWithSpec(spec);
            if (!listFriendships.Any())
            {
                _logger.LogWarning("GetAllFriendShip: user has no FriendShip");
                return ResultResponse<FriendshipResponse>.Success("NoFriendRequest Founded");
            }
            var mapListFriendShip = _mapper.Map<List<FriendshipResponse>>(listFriendships);
            return ResultResponse<FriendshipResponse>.Success(mapListFriendShip);
        }

        public async Task<ResultResponse<FriendshipResponse>> GetAllFriends(string userId)
        {
            var existUser = await _userManager.FindByIdAsync(userId);
            if (existUser is null)
            {
                _logger.LogWarning("GetAllFriends: {userId} Not Found ", userId);
                return ResultResponse<FriendshipResponse>.Failure("User Not Found");
            }
            var spec = new FriendshipSpecification(f =>
 (f.RequesterId == userId || f.AddresseeId == userId)
 && f.Status == RequestStatus.Accepted);
            var listFriendships = await _unitOfWork.Repository<Friendship>().GetAllWithSpec(spec);
            if (!listFriendships.Any())
            {
                _logger.LogWarning("GetAllFriends: user has no Friends");
                return ResultResponse<FriendshipResponse>.Success("NoFriend Founded");
            }
            var mapListFriendShip = _mapper.Map<List<FriendshipResponse>>(listFriendships);
            return ResultResponse<FriendshipResponse>.Success(mapListFriendShip);
        }

        public async Task<ResultResponse<FriendshipResponse>> SendFriendRequestAsync(string requestId, string addressId)
        {
            if (requestId == addressId)
            {
                _logger.LogWarning("FriendShipUserAsync: User {userId} attempted to sendRequest themselves.", requestId);
                return ResultResponse<FriendshipResponse>.Failure("You cannot Send Request to yourself.");
            }
            var existAddressId = await _userManager.FindByIdAsync(addressId);
            if (existAddressId == null)
            {
                _logger.LogWarning("FriendShipUserAsync: User {userId} attempted to sendRequest to non-existent user {addressId}.", requestId, addressId);
                return ResultResponse<FriendshipResponse>.Failure("The user you are trying to Send Request to does not exist.");
            }
            var spec = new FriendshipSpecification(f => f.RequesterId == requestId && f.AddresseeId == addressId);
            var existFriendShip = await _unitOfWork.Repository<Friendship>().GetByIdSpecTracked(spec);
            if (existFriendShip != null)
            {
                _logger.LogWarning("FriendShipUserAsync: User {userId} attempted to sendRequest to user {addressId} but a friendship already exists.", requestId, addressId);
                return ResultResponse<FriendshipResponse>.Failure("A friendship already exists between you and this user.");
            }
            var friendShip = new Friendship
            {
                RequesterId = requestId,
                AddresseeId = addressId,
                CreatedAt = DateTime.Now,
                Status = RequestStatus.Pending
            };
            await _unitOfWork.Repository<Friendship>().AddAsync(friendShip);
            await _unitOfWork.CommitAsync();
            var mapfriendShip = _mapper.Map<FriendshipResponse>(friendShip);
            return ResultResponse<FriendshipResponse>.Success(mapfriendShip, "Successfully Send Request the user.");
        }

        public async Task<ResultResponse<FriendshipResponse>> UnFriend(Guid Id)
        {
            var existingFriendShip = await _unitOfWork.Repository<Friendship>().GetByIdAsync(Id);
            if (existingFriendShip is null)
            {
                _logger.LogWarning("UnFriend: {FriendShipId} Not Found ", Id);
                return ResultResponse<FriendshipResponse>.Failure("FriendShipId Not Found");
            }
            if (existingFriendShip.Status == RequestStatus.Pending || existingFriendShip.Status == RequestStatus.Declined || existingFriendShip.Status == RequestStatus.Blocked)
            {
                _logger.LogWarning("UnFriend:cant delete FriendShip With {Status} Not Found ", existingFriendShip.Status);
                return ResultResponse<FriendshipResponse>.Failure("cannot delete FriendShip");
            }
            _unitOfWork.Repository<Friendship>().Delete(existingFriendShip);
            await _unitOfWork.CommitAsync();
            return ResultResponse<FriendshipResponse>.Success("Successfully UnFriend");
        }
    }
}
