using System.ComponentModel.DataAnnotations.Schema;
using ToDoList.Domain.Enum;
using ToDoList.Domain.Entity;

namespace ToDoList.Domain.Entity;

public class UserEntity
{
    public long Id { get; set; }
    
    public byte[] EnNickname { get; set; }
    
    public byte[] EnLogin { get; set; }
    
    public byte[] EnPass { get; set; }
    
    public byte[] EncryptionKey { get; set; }
    public byte[] InitializationVector { get; set; }
    
    public Role role { get; set; }
    public List<TaskEntity>? Task { get; set; }
}