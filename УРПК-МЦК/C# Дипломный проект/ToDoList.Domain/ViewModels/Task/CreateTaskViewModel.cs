using ToDoList.Domain.Enum;

namespace ToDoList.Domain.ViewModels.Task;

public class CreateTaskViewModel
{
    public string Name { get; set; }
    public DateTime Created { get; set; }
    public long? UserId { get; set; } 
    public long NachId { get; set; } 
}