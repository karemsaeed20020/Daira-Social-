using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daira.Infrastructure.Persistence.Configuarations
{
    public class CommentConfiguaration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comments");
            builder.Property(c => c.Content).HasColumnType("nvarchar(max)");
            builder.Property(c => c.CreatedAt)
              .HasDefaultValueSql("GETUTCDATE()");

            //Relationships
            builder.HasOne(c => c.Post).WithMany(p => p.Comments).HasForeignKey(p => p.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(c => c.AppUser).WithMany(p => p.Comments).HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //Index
            builder.HasIndex(c => c.PostId).HasDatabaseName("idx_comments_post_id");
            builder.HasIndex(c => c.UserId).HasDatabaseName("idx_comments_user_id");
        }
    }
}
