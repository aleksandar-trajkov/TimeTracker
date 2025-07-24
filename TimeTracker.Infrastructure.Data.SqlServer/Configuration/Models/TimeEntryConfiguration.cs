using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTracker.Domain;

namespace TimeTracker.Infrastructure.Data.SqlServer.Configuration.Models;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("TimeEntries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.From)
            .IsRequired();

        builder.Property(x => x.To)
            .IsRequired();

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.CategoryId)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        // Relationships
        builder.HasOne(x => x.Category)
            .WithMany(x => x.TimeEntries)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(x => x.CategoryId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.From);
        builder.HasIndex(x => x.To);
    }
}