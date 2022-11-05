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

            modelBuilder.Entity<UserAvatar>()
                .ToTable(nameof(Avatars));

            modelBuilder.Entity<User>()
                .HasOne(u => u.Avatar)
                .WithOne(a => a.User);
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<Attach> Attaches => Set<Attach>();
        public DbSet<UserAvatar> Avatars => Set<UserAvatar>();

    }
}
