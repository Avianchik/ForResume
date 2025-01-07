using Microsoft.EntityFrameworkCore;
using ToDoList.Domain.Entity;
using ToDoList.DAL.Configurations;
using ToDoList.Domain.Enum;

namespace ToDoList.DAL;

public class AppDbContext : DbContext
{
    public DbSet<TaskEntity> Tasks { get; set; }

    
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<DepartamentEntity> Departaments { get; set; }
    // public DbSet<HierarchyEntity> Hierarchys { get; set; }
    
     protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DepartamentConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new TaskConfiguration());
        // modelBuilder.ApplyConfiguration(new HierarchyConfiguration());
        
    }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        //Database.EnsureDeleted();//Удаление базы при запуске
        Database.EnsureCreated();
    }

    
    
}