using System.ComponentModel.DataAnnotations;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Enum;

namespace ToDoList.Domain.ViewModels.User;

public class DecryptedUsers
{
    [Display(Name = "Номер")]
    public long Id { get; set; }
    
    [Display(Name = "ФИО")]
    public string FIO { get; set; }
    [Display(Name = "Логин")]
    public string login { get; set; }
    [Display(Name = "Пароль")]
    public string password { get; set; }
    [Display(Name = "Роль")]
    public string role { get; set; }
    [Display(Name = "Номер отдела")]
    public long DepartamentId { get; set; } 
    [Display(Name = "Название отдела")]
    public string DepartamentName { get; set; } 
    [Display(Name = "Начальник")]
    public long NachalnikId { get; set; }
    
    [Display(Name = "ФИО начальника")]
    public string NachalnikFIO { get; set; }
    
    
}