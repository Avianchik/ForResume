using ToDoList.Domain.Enum;
using ToDoList.Domain.Entity;

namespace ToDoList.Domain.ViewModels.User;

public class LoginUserViewModel
{
    public string login { get; set; }
    
    public string password { get; set; }
}