
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ToDoList.Domain.Filters;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Domain.ViewModels.User;
using ToDoList.Service.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace ToDoList.Controllers;

public class TaskController : Controller
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }
    
    [Authorize]
    public IActionResult Tasks()
    {
        return View("~/Views/Pages/Tasks.cshtml");
    }
    [HttpPost]
    public async Task<IActionResult> EndTask(TaskChangeModel model)
    {
        var response = await _taskService.EndTask(model);
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return Ok(new { description = response.Description });
        }
        return BadRequest(new { description = response.Description });
    }
    
    public async Task<IActionResult> TaskHandler(TaskFilter filter)
    {
        var start = Request.Form["start"].FirstOrDefault();
        var length = Request.Form["length"].FirstOrDefault();
        
        var pageSize = length != null ? Convert.ToInt32(length):0;
        var skip = start != null ? Convert.ToInt32(start) : 0;

        filter.Skip = skip;
        filter.PageSize = pageSize;
        
        var response = await _taskService.GetTasks(filter);
        return Json(new {recordsFiltered = response.Total,recordsTotal = response.Total,data = response.Data });
    }
    
    [HttpPost]
    public async Task<IActionResult> PDFCreate(TaskFilter filter)
    {
        
        
        var response = await  _taskService.GetTasks4PDF(filter);
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return Ok(new { description = response.Description });
        }
        return BadRequest(new { description = response.Description });
    }
    

    [HttpGet("download")]
    public IActionResult DownloadFile()
    {
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Отчёт.pdf");

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "application/pdf", "Отчёт.pdf");
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskViewModel model)
    {
        var response = await _taskService.CreateTask(model);
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return Ok(new { description = response.Description });
        }
        return BadRequest(new { description = response.Description });
    }

    /*public async Task<IActionResult> GetCompletedTasks(TaskFilter filter)
    {
        var start = Request.Form["start"].FirstOrDefault();
        var length = Request.Form["length"].FirstOrDefault();
        
        var pageSize = length != null ? Convert.ToInt32(length):0;
        var skip = start != null ? Convert.ToInt32(start) : 0;

        filter.Skip = skip;
        filter.PageSize = pageSize;
        
        var response = await _taskService.GetCompletedTasks(filter);
        return Json(new {recordsFiltered = response.Total,recordsTotal = response.Total,data = response.Data });
    }*/
}

    