using ToDoList.Domain.Enum;

namespace ToDoList.Domain.Filters;

public class DepFilter:PagingFilter
{
    public string DepName { get; set; }
    
    public string idDep { get; set; }
}