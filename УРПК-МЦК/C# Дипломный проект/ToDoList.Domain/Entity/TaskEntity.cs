using ToDoList.Domain.Enum;

namespace ToDoList.Domain.Entity;

public class TaskEntity
{
    public long Id { get; set; }
    public byte[] Name { get; set; }
    public StatTask Status { get; set; }
    public DateTime Created { get; set; }
    
    public DateTime? Ended { get; set; }
    
    public UserEntity? User { get; set; }
    
    public UserEntity? Nach { get; set; }
    
    public long UserId { get; set; } 
    
    public long NachId { get; set; } 
    
    public byte[] EncryptionKey { get; set; }
    public byte[] InitializationVector { get; set; }
}