using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoList.Domain.Entity;

namespace ToDoList.DAL.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
{
        
    public void Configure(EntityTypeBuilder<TaskEntity> builder)//задачи
    {
        builder.HasKey(u => u.Id);
                
        builder 
            .HasOne(t=>t.User)
            .WithMany(u=>u.Tasks)
            .HasForeignKey(t=>t.UserId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}