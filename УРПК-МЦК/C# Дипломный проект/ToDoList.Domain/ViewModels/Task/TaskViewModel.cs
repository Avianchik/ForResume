using System.ComponentModel.DataAnnotations;
using ToDoList.Domain.Entity;

namespace ToDoList.Domain.ViewModels.Task;

public class TaskViewModel
{
    public long Id { get; set; }
    
    [Display(Name = "Название")]
    public string Name { get; set; }
    
    [Display(Name = "Готовность")]
    public string IsDone { get; set; }
    
    [Display(Name = "Дата создания")]
    public string Created { get; set; }
    
    [Display(Name = "Закреплённый работник")]
    public string User { get; set; }
    [Display(Name = "Закреплённый выдавший")]
    public string Nachalnik { get; set; }
    [Display(Name = "Дата завершения")]
    public string Ended { get; set; }
    public string UserId { get; set; }
    
    public long NachId { get; set; }
    
    public UserEntity Usera { get; set; }
    
}