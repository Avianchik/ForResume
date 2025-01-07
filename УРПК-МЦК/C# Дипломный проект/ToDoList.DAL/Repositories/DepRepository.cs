using Microsoft.EntityFrameworkCore;
using ToDoList.DAL.Interfaces;
using ToDoList.Domain.Entity;

namespace ToDoList.DAL.Repositories;


public class DepRepository : IBaseRepository<DepartamentEntity>
{
    private readonly AppDbContext _appDbContext;

    public DepRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task Create(DepartamentEntity entity)
    {
        await _appDbContext.Departaments.AddAsync(entity);
        await _appDbContext.SaveChangesAsync();
    }

    public IQueryable<DepartamentEntity> GetAll()
    {
        return _appDbContext.Departaments
            .AsNoTracking();
    }

    public async Task Delete(DepartamentEntity entity)
    {
        _appDbContext.Departaments.Remove(entity);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<DepartamentEntity> Update(DepartamentEntity entity)
    {
        _appDbContext.Departaments.Update(entity);
        await _appDbContext.SaveChangesAsync();

        return entity;
    }
}