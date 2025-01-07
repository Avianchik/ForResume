using System.ComponentModel.DataAnnotations;

namespace ToDoList.Domain.ViewModels.Task;

public class TaskCompleatedViewModel
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public string UserId { get; set; }
}