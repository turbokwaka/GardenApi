using Microsoft.EntityFrameworkCore;

namespace GardenApi
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<PlantEntity> Plants { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>()
                .HasMany(e => e.Plants)
                .WithOne()
                .HasForeignKey(e => e.UserId);
        }
    }
}
