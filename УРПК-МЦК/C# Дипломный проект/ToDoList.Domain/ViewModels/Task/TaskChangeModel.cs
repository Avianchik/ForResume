using System.ComponentModel.DataAnnotations;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Enum;

namespace ToDoList.Domain.ViewModels.Task;

public class TaskChangeModel
{
    public long Id { get; set; }
    public int Stat { get; set; }

    
}