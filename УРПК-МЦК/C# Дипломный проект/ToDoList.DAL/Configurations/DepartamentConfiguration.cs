using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoList.Domain.Entity;

namespace ToDoList.DAL.Configurations;

public class DepartamentConfiguration : IEntityTypeConfiguration<DepartamentEntity>
{
        
    public void Configure(EntityTypeBuilder<DepartamentEntity> builder)//отделы
    {
        builder.HasKey(u => u.DepartamentId);
                
        builder 
            .HasMany(d=>d.Users)
            .WithOne(u=>u.Departament)
            .HasForeignKey(d=>d.DepartamentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}