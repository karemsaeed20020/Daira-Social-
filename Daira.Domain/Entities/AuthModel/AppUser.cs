
namespace Daira.Domain.Entities.AuthModel
{
    public class AppUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public string? PictureUrl { get; set; }
        public bool IsVerified { get; set; } = false;
        public bool IsPrivate { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string? FullName => $"{FirstName} {LastName}".Trim();
        // Navigation Properties
        public List<Post> Posts { get; set; } = new();
        public List<RefreshToken> RefreshTokens { get; set; } = new();
        public ICollection<Notification> ReceivedNotifications { get; set; } = new List<Notification>();
        public ICollection<Notification> TriggeredNotifications { get; set; } = new List<Notification>();
        public List<Comment> Comments { get; set; } = new();
        public List<Like> Likes { get; set; } = new();
        public List<Message> Messages { get; set; } = new();
        public List<Follower> Following { get; set; } = new();
        public List<Follower> Follower { get; set; } = new();
        public List<Friendship> SentFriendRequests { get; set; } = new();
        public List<Friendship> ReceivedFriendRequests { get; set; } = new();
        public List<ConversationParticipant> ConversationParticipants { get; set; } = new();
    }
}
