using Microsoft.EntityFrameworkCore;
using TaskTracking.Entities.Coworker;
using TaskTracking.Entities.Project;
using TaskTracking.Entities.Task;

namespace TaskTracking
{
    public class AppDbContext : DbContext
    {
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<CoworkerModel> Coworkers { get; set; }
        public DbSet<ProjectModel> Projects { get; set; }

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
            modelBuilder.Entity<TaskModel>()
                .HasOne(p => p.Project)
                .WithMany()
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskModel>()
                .HasOne(p => p.Manager)
                .WithMany()
                .HasForeignKey(t => t.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskModel>()
                .HasOne(p => p.Employee)
                .WithMany()
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProjectModel>()
                .HasOne(p => p.Manager)
                .WithMany()
                .HasForeignKey(t => t.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}