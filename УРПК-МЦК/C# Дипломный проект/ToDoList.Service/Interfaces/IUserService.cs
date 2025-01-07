using Microsoft.AspNetCore.Http;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Filters;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.User;
using ToDoList.Domain.ViewModels.Departament;

namespace ToDoList.Service.Interfaces;

public interface IUserService
{
    Task<IBaseResponse<UserEntity>> Create(CreateUserViewModel model);

    Task<DataTableResult> GetAll(UserFilter filter);

    Task<IBaseResponse<bool>> DeleteUser(long id);
    
    Task<IBaseResponse<bool>> UpdateUser(UpdateUserViewModel model);
    
    Task<UserEntity> GetByLogin(string login);
    
    Task<UserViewModel> GetById(long id);

    Task<IBaseResponse<bool>> Login(LoginUserViewModel model, HttpContext context);

    string Decrypt(byte[] cipheredtext, byte[] key, byte[] iv);

}