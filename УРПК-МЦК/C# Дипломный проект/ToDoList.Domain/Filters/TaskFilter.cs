using ToDoList.Domain.Enum;

namespace ToDoList.Domain.Filters;

public class TaskFilter:PagingFilter
{
    public string Name { get; set; }
    public long UserId { get; set; }
    public string UserName { get; set; }
    
    public string NachName { get; set; }
    public long NachId { get; set; }

    public StatTask? IsDone { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string StartDate2 { get; set; }
    public string EndDate2 { get; set; }
}