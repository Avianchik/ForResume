using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.Filters;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Domain.ViewModels.Departament;
using ToDoList.Service.Interfaces;

namespace ToDoList.Controllers;

public class DepController : Controller
{
    public IActionResult Dep()
    {
        return View("~/Views/Pages/Dep.cshtml");
    }
    
    private readonly IDepService _depService;

    public DepController(IDepService depService)
    {
        _depService = depService;
    }

    public async Task<IActionResult> DepHandler(DepFilter filter)
    {
        var start = Request.Form["start"].FirstOrDefault();
        var length = Request.Form["length"].FirstOrDefault();
        
        var pageSize = length != null ? Convert.ToInt32(length):0;
        var skip = start != null ? Convert.ToInt32(start) : 0;

        filter.Skip = skip;
        filter.PageSize = pageSize;
        
        var response = await _depService.GetAll(filter);
        return Json(new {recordsFiltered = response.Total,recordsTotal = response.Total,data = response.Data });
    }

    [HttpPost]
    public async Task<IActionResult> CreateDep([FromBody] CreateDepViewModel model)
    {
        var response = await _depService.Create(model);
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return Ok(new { description = response.Description });
        }
        return BadRequest(new { description = response.Description });
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateNameDep(UpdateDepViewModel model)
    {
        var response = await _depService.UpdateNameDep(model);
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return Ok(new { description = response.Description });
        }
        return BadRequest(new { description = response.Description });
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateIdDep(UpdateDepViewModel model)
    {
        var response = await _depService.UpdateIdDep(model);
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return Ok(new { description = response.Description });
        }
        return BadRequest(new { description = response.Description });
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteDep( long id)
    {
        var response = await _depService.DeleteDep(id);
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return Ok(new { description = response.Description });
        }
        return BadRequest(new { description = response.Description });
    }
}

    