using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoList.Domain.Entity;

namespace ToDoList.DAL.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    
        public void Configure(EntityTypeBuilder<UserEntity> builder)//пользователь
        {
                builder.HasKey(u => u.Id);

                builder//Отдел
                        .HasOne(u => u.Departament)
                        .WithMany(d => d.Users)
                        .HasForeignKey(u => u.DepartamentId)
                        .OnDelete(DeleteBehavior.Cascade);
                builder//Задачи
                        .HasMany(c=>c.Tasks)
                        .WithOne(u=>u.User)
                        .HasForeignKey(u => u.UserId)
                        .OnDelete(DeleteBehavior.NoAction);
        }
}