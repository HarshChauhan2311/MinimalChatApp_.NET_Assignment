using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Models;
using MinimalChatApp.Models;

namespace MinimalChatApp.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<RequestLog> RequestLogs { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            // Configure identity columns to start at 1
            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .UseIdentityColumn(seed: 1, increment: 1);

            modelBuilder.Entity<Message>()
                .Property(m => m.Id)
                .UseIdentityColumn(seed: 1, increment: 1);

            modelBuilder.Entity<RequestLog>()
                .Property(r => r.Id)
                .UseIdentityColumn(seed: 1, increment: 1);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.Id)
                .UseIdentityColumn(seed: 1, increment: 1);

            // Message → Sender (User)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Message → Receiver (User)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique Email constraint
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}
