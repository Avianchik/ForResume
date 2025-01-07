using ToDoList.Domain.Entity;
using ToDoList.Domain.Filters;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;

namespace ToDoList.Service.Interfaces;

public interface ITaskService
{
    Task<IBaseResponse<TaskEntity>> CreateTask(CreateTaskViewModel model);

    Task<DataTableResult> GetTasks(TaskFilter filter);

    Task<IBaseResponse<bool>> EndTask(TaskChangeModel model);

    Task<IBaseResponse<bool>> GetTasks4PDF(TaskFilter filter);
    // Task<DataTableResult> GetCompletedTasks(TaskFilter filter);
}