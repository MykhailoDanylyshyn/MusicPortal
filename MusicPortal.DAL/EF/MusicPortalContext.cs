using Microsoft.EntityFrameworkCore;
using MusicPortal.DAL.Entities;

namespace MusicPortal.DAL.EF;

public class MusicPortalContext(
    DbContextOptions<MusicPortalContext> options)
    : DbContext(options)
{
    public DbSet<Record> Records { get; set; }

    public DbSet<Artist> Artists { get; set; }

    public DbSet<Genre> Genres { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Playlist> Playlists { get; set; }

    public DbSet<PlaylistRecord> PlaylistRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Record>()
            .HasOne(r => r.Artist)
            .WithMany(a => a.MusicalRecords)
            .HasForeignKey(r => r.ArtistId)
            .IsRequired(false);

        modelBuilder.Entity<Record>()
            .HasOne(r => r.Genre)
            .WithMany(g => g.Records)
            .HasForeignKey(r => r.GenreId)
            .IsRequired();

        modelBuilder.Entity<PlaylistRecord>()
            .HasKey(pr => new { pr.PlaylistId, pr.RecordId });

        modelBuilder.Entity<PlaylistRecord>()
            .HasOne(pr => pr.Playlist)
            .WithMany(p => p.PlaylistRecords)
            .HasForeignKey(pr => pr.PlaylistId);

        modelBuilder.Entity<PlaylistRecord>()
            .HasOne(pr => pr.Record)
            .WithMany(r => r.PlaylistRecords)
            .HasForeignKey(pr => pr.RecordId);

        modelBuilder.Entity<Playlist>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId);
        
    }
}