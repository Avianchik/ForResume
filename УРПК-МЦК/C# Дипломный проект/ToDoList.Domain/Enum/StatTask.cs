using System.ComponentModel.DataAnnotations;

namespace ToDoList.Domain.Enum;

public enum StatTask
{
    [Display(Name = "Не взято")]
    notTaked = 1,
    [Display(Name = "Выполнено")]
    compleated = 2,
    [Display(Name = "В работе")]
    inWork = 3,
    [Display(Name = "Отменено")]
    canseled = 4,
    [Display(Name = "Работа приостановлена")]
    isStoped = 5
}