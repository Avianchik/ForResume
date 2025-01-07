using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Filters;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Domain.ViewModels.Departament;
using ToDoList.Domain.ViewModels.User;
using ToDoList.Service.Interfaces;

namespace ToDoList.Controllers;

public class UserController : Controller
{
    
    public IActionResult LoginPage()
    {
        return View("~/Views/Pages/LoginPage.cshtml");
    }
    
    [Authorize]
    public IActionResult Users()
    {
        return View("~/Views/Pages/Users.cshtml");
    }
    [Authorize]
    public IActionResult RegUsers()
    {
        return View("~/Views/Pages/RegUsers.cshtml");
    }
    
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserController(IUserService userService,IHttpContextAccessor httpContextAccessor)
    {
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
    }
    
    public async Task<IActionResult> UserHandler(UserFilter filter)
    {
        var start = Request.Form["start"].FirstOrDefault();
        var length = Request.Form["length"].FirstOrDefault();

        var pageSize = length != null ? Convert.ToInt32(length) : 0;
        var skip = start != null ? Convert.ToInt32(start) : 0;

        filter.Skip = skip;
        filter.PageSize = pageSize;

        var response = await _userService.GetAll(filter);
        return Json(new { recordsFiltered = response.Total,recordsTotal = response.Total,data = response.Data });
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser( CreateUserViewModel model)
    {
        var response = await _userService.Create(model);
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return Ok(new { description = response.Description });
        }
        return BadRequest(new { description = response.Description });
    }
    
    [HttpPost]
    public async Task<IActionResult> UpdateUser(UpdateUserViewModel model)
    {
        var response = await _userService.UpdateUser(model);
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return Ok(new { description = response.Description });
        }
        return BadRequest(new { description = response.Description });
    }
    
    [HttpPost]
    public async Task<IActionResult> DeleteUser( long id)
    {
        var response = await _userService.DeleteUser(id);
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return Ok(new { description = response.Description });
        }
        return BadRequest(new { description = response.Description });
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginUserViewModel model)
    {
        var response = await _userService.Login(model,_httpContextAccessor.HttpContext);
        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return Ok(new { description = response.Description });
        }
        return BadRequest(new { description = response.Description });
    }
    
    [HttpGet]
    public async Task<IActionResult> FindUserInfo(long id)
    {
        try
        {
            var model = await _userService.GetById(id);
            return Json(model); // Возвращаем модель UserEntity в формате JSON
        }
        catch (Exception e)
        {
            return BadRequest("Пользователь не найден");
        }
    }

    
    [HttpPost]
    public IActionResult Logout()
    {
        // Удаление JWT-токена из куки
        Response.Cookies.Append("tasty-cookies", "", new CookieOptions
        {
            Expires = DateTime.UtcNow.AddDays(-1),
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        // Дополнительные действия, например, очистка сессии пользователя
        // ...
        return View("~/Views/Pages/LoginPage.cshtml");
    }
}

    