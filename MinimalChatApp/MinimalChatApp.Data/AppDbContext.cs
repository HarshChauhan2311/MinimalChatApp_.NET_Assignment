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
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 💡 Explicitly map entity to table name
            modelBuilder.Entity<Group>().ToTable("Groups");

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

            modelBuilder.Entity<Group>()
                .Property(g => g.GroupId)
                .UseIdentityColumn(seed: 1, increment: 1);

            modelBuilder.Entity<GroupMember>()
                .Property(gm => gm.Id)
                .UseIdentityColumn(seed: 1, increment: 1);

            // Unique constraint
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Group>().HasIndex(g => g.GroupName).IsUnique();

            // User → Message (Sender)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → Message (Receiver)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);


            // Group → Message
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Group)
                .WithMany(g => g.Messages)
                .HasForeignKey(m => m.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Group → Creator (User)
            modelBuilder.Entity<Group>()
                .HasOne(g => g.Creator)
                .WithMany(u => u.CreatedGroups)
                .HasForeignKey(g => g.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // GroupMember (User ↔ Group)
            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.GroupMemberships)
                .HasForeignKey(gm => gm.UserId);

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(gm => gm.GroupId);

          
            base.OnModelCreating(modelBuilder);
        }
    }
}
