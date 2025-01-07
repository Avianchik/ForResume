using System.ComponentModel.DataAnnotations;

namespace ToDoList.Domain.Enum;

public enum Role
{
    [Display(Name = "Начальник отдела")]
    GlavDeportament = 1,
    [Display(Name = "Заместитель начальника отдела")]
    ZamGlavDeportament = 2,
    [Display(Name = "Начальник бюро")]
    GlavBuro = 3,
    [Display(Name = "Сотрудник")]
    Worker = 4,
    [Display(Name = "Администратор")]
    Administrator = 5
}