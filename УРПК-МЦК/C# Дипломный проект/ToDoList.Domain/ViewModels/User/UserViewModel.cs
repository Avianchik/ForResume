using System.ComponentModel.DataAnnotations;

namespace ToDoList.Domain.ViewModels.User;

public class UserViewModel
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
    public string DepartamentId { get; set; } 
    [Display(Name = "Название отдела")]
    public string Departament { get; set; } 
    [Display(Name = "Начальник")]
    public string NachalnikId { get; set; }
    
    [Display(Name = "ФИО начальника")]
    public string Nachalnik { get; set; }
    [Display(Name = "Номер отдела")]
    public string depidd { get; set; } 
    
}