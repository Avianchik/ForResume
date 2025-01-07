using System.ComponentModel.DataAnnotations;

namespace ToDoList.Domain.ViewModels.User;

public class UserCompleatedViewModel
{
    public long Id { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
}