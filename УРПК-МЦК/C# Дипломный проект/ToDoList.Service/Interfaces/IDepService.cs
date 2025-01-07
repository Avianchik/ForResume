using ToDoList.Domain.Entity;
using ToDoList.Domain.Filters;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Departament;
using ToDoList.Domain.ViewModels.Task;

namespace ToDoList.Service.Interfaces;

public interface IDepService
{
    Task<IBaseResponse<DepartamentEntity>> Create(CreateDepViewModel model);

    Task<DataTableResult> GetAll(DepFilter filter);

    Task<IBaseResponse<bool>> DeleteDep(long id);
    
    Task<IBaseResponse<bool>> UpdateNameDep(UpdateDepViewModel model);
    
    Task<IBaseResponse<bool>> UpdateIdDep(UpdateDepViewModel model);
}