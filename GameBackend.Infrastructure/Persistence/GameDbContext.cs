using GameBackend.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameBackend.Infrastructure.Persistence;

public class GameDbContext : DbContext
{
    public DbSet<GameSession> GameSessions => Set<GameSession>();

    public GameDbContext(DbContextOptions<GameDbContext> options)
        : base(options)
    {
    }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<Leaderboard> Leaderboards => Set<Leaderboard>();
    public DbSet<LeaderboardEntry> LeaderboardEntries => Set<LeaderboardEntry>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Player>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasIndex(x => x.Username).IsUnique();
            entity.Property(x => x.Metadata).HasColumnType("jsonb");
        });

        builder.Entity<Leaderboard>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.GameId, x.Name }).IsUnique();
        });

        builder.Entity<LeaderboardEntry>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.LeaderboardId, x.PlayerId }).IsUnique();
            entity.Property(x => x.Metadata).HasColumnType("jsonb");
            entity.HasOne(x => x.Player)
                .WithMany()
                .HasForeignKey(x => x.PlayerId);
            entity.HasOne(x => x.Leaderboard)
                .WithMany(x => x.Entries)
                .HasForeignKey(x => x.LeaderboardId);
        });

        builder.Entity<GameSession>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.PlayerId, x.GameId, x.Status });
            entity.Property(x => x.Metadata).HasColumnType("jsonb");
            entity.HasOne(x => x.Player)
                .WithMany()
                .HasForeignKey(x => x.PlayerId);
        });
    }
}