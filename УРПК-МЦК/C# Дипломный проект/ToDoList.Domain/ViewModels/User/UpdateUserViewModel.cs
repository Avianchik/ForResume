using ToDoList.Domain.Entity;
using ToDoList.Domain.Enum;

namespace ToDoList.Domain.ViewModels.User;

public class UpdateUserViewModel
{
    public long OperationUpdate { get; set; }
    public long Id { get; set; }
    
    public string FIO { get; set; }
    
    public string login { get; set; }
    
    public string password { get; set; }
    public byte[] EncryptionKey { get; set; }
    public byte[] InitializationVector { get; set; }
    public Role role { get; set; }
    
    public DepartamentEntity? Departament { get; set; }
    
    public long? DepartamentId { get; set; } 
    
    public long NachalnikId { get; set; }
    
}