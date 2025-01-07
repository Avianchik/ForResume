using ToDoList.Domain.Enum;
using ToDoList.Domain.Entity;

namespace ToDoList.Domain.ViewModels.User;

public class CreateUserViewModel
{
    public string FIO { get; set; }
    
    public string login { get; set; }
    
    public string password { get; set; }
    public string logID { get; set; }
    public Role role { get; set; }
    
    public DepartamentEntity? Departament { get; set; }
    //
    public long DepartamentId { get; set; } 
    //
    public long? NachalnikId { get; set; }
    //
    // public HierarchyEntity? Nachalnik { get; set; }
    //
    // public HierarchyEntity? Podchineniy { get; set; }
}