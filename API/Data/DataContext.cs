using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace API.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions options):base(options)
        {
            
        }
        public DbSet<AppUser> Users { get; set; }
    
        public DbSet<UserLike> Likes { get; set; }  
        public DbSet<Messages> Messages { get; set; }  

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserLike>().HasKey(k=>new {k.SourceUserId, k.LikedUserId});

            builder.Entity<UserLike>()
                   .HasOne(s=>s.SourceUser)
                   .WithMany(l=>l.LikedUsers)
                   .HasForeignKey(s=>s.SourceUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserLike>()
                   .HasOne(s=>s.LikedUser)
                   .WithMany(l=>l.LikedByUsers)
                   .HasForeignKey(s=>s.LikedUserId)
                   .OnDelete(DeleteBehavior.Cascade);
           
            builder.Entity<Messages>()
                   .HasOne(u => u.Sender)
                   .WithMany(m => m.MessagesSended)
                   /* .HasForeignKey(m=>m.SenderId) */
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Messages>()
                   .HasOne(u => u.Recipient)
                   .WithMany(m => m.MessagesRecieved)
                   /* .HasForeignKey(m=>m.RecipientId) */
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}