using Daira.Application.Response.FriendshipModule;
using Daira.Application.Shared;

namespace Daira.Application.Interfaces.FriendshipModule
{
    public interface IFriendShipService
    {
        // Send FriendShip Request
        Task<ResultResponse<FriendshipResponse>> SendFriendRequestAsync(string requestId, string addressId);
        // Accept FriendShip Request
        Task<ResultResponse<FriendshipResponse>> AcceptFriendRequest(Guid id);
        // Decline FriendShip Request
        Task<ResultResponse<FriendshipResponse>> DeclineFriendRequest(Guid id);
        // Get All Friends Requests for a User
        Task<ResultResponse<FriendshipResponse>> GetAllFriendRequests(string userId);
        // Get All Friends for a User
        Task<ResultResponse<FriendshipResponse>> GetAllFriends(string userId);
        // Delete All Friendship
        Task<ResultResponse<FriendshipResponse>> UnFriend(Guid Id);
    }
}
