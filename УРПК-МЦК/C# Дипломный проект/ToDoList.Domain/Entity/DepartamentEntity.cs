using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ToDoList.Domain.Enum;

namespace ToDoList.Domain.Entity;

public class DepartamentEntity
{
    
    
    
    public long DepartamentId { get; set; }

    
    public long DepartamentNumber { get; set; }

    public byte[] DepartamentName { get; set; }
    
    public List<UserEntity>? Users { get; set; }
    
    public byte[] EncryptionKey { get; set; }
    public byte[] InitializationVector { get; set; }

}