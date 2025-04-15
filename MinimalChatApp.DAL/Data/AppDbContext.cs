using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MinimalChatApp.Entity;

namespace MinimalChatApp.DAL.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Message> Messages { get; set; }
        public DbSet<RequestLog> RequestLogs { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // 👈 Always first for Identity
            // 💡 Explicitly map entity to table name
            modelBuilder.Entity<Group>().ToTable("Groups");

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
            modelBuilder.Entity<ApplicationUser>().HasIndex(u => u.Email).IsUnique();
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
                .OnDelete(DeleteBehavior.Restrict)
                .HasPrincipalKey(u => u.Id);

            // GroupMember (User ↔ Group)
            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.GroupMemberships)
                .HasForeignKey(gm => gm.UserId);

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(gm => gm.GroupId);

        }
    }
}
