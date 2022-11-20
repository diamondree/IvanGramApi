using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql(a => a.MigrationsAssembly("IvanGram"));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(f => f.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne(u => u.Avatar)
                .WithOne(a => a.User);

            modelBuilder.Entity<UserAvatar>()
                .ToTable(nameof(Avatars));

            modelBuilder.Entity<PostFile>()
                .ToTable(nameof(PostFiles));

            modelBuilder.Entity<PostLike>()
                .ToTable(nameof(PostLikes));

            modelBuilder.Entity<PostCommentLike>()
                .ToTable(nameof(CommentLikes));
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<Attach> Attaches => Set<Attach>();
        public DbSet<UserAvatar> Avatars => Set<UserAvatar>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<PostComment> PostComments => Set<PostComment>();
        public DbSet<PostFile> PostFiles => Set<PostFile>();
        public DbSet<Subscription> Subscriptions => Set<Subscription>();
        public DbSet<Like> Likes => Set<Like>();
        public DbSet<PostLike> PostLikes => Set<PostLike>();
        public DbSet<PostCommentLike> CommentLikes => Set<PostCommentLike>();
    }
}
