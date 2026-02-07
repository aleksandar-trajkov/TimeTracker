using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTracker.Domain.Auth;

namespace TimeTracker.Infrastructure.Data.SqlServer.Configuration.Models
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Key)
                .IsRequired();

            builder.Property(x => x.UserId)
                .IsRequired();

            // Relationships
            builder.HasOne(x => x.User)
                .WithMany(x => x.Permissions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Composite unique index to prevent duplicate permissions per user
            builder.HasIndex(x => new { x.UserId, x.Key })
                .IsUnique();

            // Index for performance
            builder.HasIndex(x => x.UserId);
        }
    }
}
