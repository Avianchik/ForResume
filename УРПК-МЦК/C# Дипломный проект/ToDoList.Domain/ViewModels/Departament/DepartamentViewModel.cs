using System.ComponentModel.DataAnnotations;

namespace ToDoList.Domain.ViewModels.Departament;

public class DepViewModel
{
    [Display(Name = "Key")]
    public long DepartamentId { get; set; }
    
    [Display(Name = "Название")]
    public string DepartamentName { get; set; }
    
    [Display(Name = "Номер")]
    public long DepartamentNumber{ get; set; }
}