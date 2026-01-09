using Microsoft.EntityFrameworkCore;

namespace TaskTracking
{
    public class AppDbContext : DbContext
    {
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Coworker> Coworkers { get; set; }
        public DbSet<Project> Projects { get; set; }

        //private readonly string _connectionString;

        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        //public AppDbContext (string connectionString)
        //{
        //    _connectionString = connectionString;
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    //optionsBuilder.UseNpgsql("Host=localhost;Database=InhabitedMindTest_1;Username=postgres;Password=admin");
        //    optionsBuilder.UseNpgsql(Initializer.GetEarlyConnectionString());
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>()
                .HasOne<Project>()
                .WithMany()
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Task>()
                .HasOne<Coworker>()
                .WithMany()
                .HasForeignKey(t => t.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Task>()
                .HasOne<Coworker>()
                .WithMany()
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne<Coworker>()
                .WithMany()
                .HasForeignKey(t => t.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(modelBuilder);
        }
    }
}