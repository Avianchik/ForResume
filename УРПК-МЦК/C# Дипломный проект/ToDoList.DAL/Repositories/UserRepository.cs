using Microsoft.EntityFrameworkCore;
using ToDoList.DAL.Interfaces;
using ToDoList.Domain.Entity;

namespace ToDoList.DAL.Repositories;


public class UserRepository : IBaseRepository<UserEntity>
{
    private readonly AppDbContext _appDbContext;

    public UserRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task Create(UserEntity entity)
    {
        await _appDbContext.Users.AddAsync(entity);
        await _appDbContext.SaveChangesAsync();
    }

    public IQueryable<UserEntity> GetAll()
    {
        return _appDbContext.Users
            .AsNoTracking();
    }

    public async Task Delete(UserEntity entity)
    {
        _appDbContext.Users.Remove(entity);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<UserEntity> Update(UserEntity entity)
    {
        _appDbContext.Users.Update(entity);
        await _appDbContext.SaveChangesAsync();

        return entity;
    }
}