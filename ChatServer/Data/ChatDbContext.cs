using Microsoft.EntityFrameworkCore;
using ChatServer.Models;

namespace ChatServer.Data
{
    public class ChatDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<FileUpload> FileUploads { get; set; }

        public ChatDbContext(DbContextOptions<ChatDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique constraint trên Username
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Foreign keys
            modelBuilder.Entity<ChatMessage>()
                .Property(m => m.Timestamp)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<FileUpload>()
                .Property(f => f.UploadedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
