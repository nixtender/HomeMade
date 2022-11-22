using DAL.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<User>()
                .HasIndex(f => f.Email)
                .IsUnique();
            modelBuilder
                .Entity<User>()
                .HasIndex(f => f.Name)
                .IsUnique();
            
            modelBuilder.Entity<Avatar>().ToTable(nameof(Avatars));
            modelBuilder.Entity<Avatar>().HasOne(b => b.User).WithOne(b => b.Avatar).HasForeignKey<Avatar>(b => b.UserId);
            /*modelBuilder.Entity<LikePost>().ToTable(nameof(LikePosts));
            modelBuilder.Entity<LikePost>().HasOne(b => b.Post).WithOne(b => b.LikePost).HasForeignKey<LikePost>(b => b.PostId);
            modelBuilder.Entity<LikeComment>().ToTable(nameof(LikeComments));
            modelBuilder.Entity<LikeComment>().HasOne(b => b.Comment).WithOne(b => b.LikeComment).HasForeignKey<LikeComment>(b => b.CommentId);*/
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
            => optionsBuilder.UseNpgsql(b => b.MigrationsAssembly("Api"));

        public DbSet<User> Users => Set<User>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<Attach> Attaches => Set<Attach>();
        public DbSet<Avatar> Avatars => Set<Avatar>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<PostPicture> PostPictures => Set<PostPicture>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<LikePost> LikePosts => Set<LikePost>();
        public DbSet<LikeComment> LikeComments => Set<LikeComment>();
    }
}
