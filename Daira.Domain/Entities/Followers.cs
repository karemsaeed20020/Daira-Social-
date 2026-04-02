

namespace Daira.Domain.Entities
{
    public class Follower : BaseEntity
    {
        public string FollowerId { get; set; }
        public string FollowingId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public AppUser FollowerUser { get; set; }
        public AppUser FollowingUser { get; set; }
    }
}
