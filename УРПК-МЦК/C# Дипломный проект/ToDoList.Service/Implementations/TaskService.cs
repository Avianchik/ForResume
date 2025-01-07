using System.Data;
using System.Globalization;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoList.DAL.Interfaces;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Enum;
using ToDoList.Domain.Extenstions;
using ToDoList.Domain.Filters;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Service.Interfaces;

namespace ToDoList.Service.Implementations;

public class TaskService : ITaskService
{
    private readonly IBaseRepository<TaskEntity> _baseRepository;
    private ILogger<TaskService> _logger;
    private readonly IBaseRepository<UserEntity> _userRepository;
    private readonly IUserService _userService;

    public TaskService(IBaseRepository<TaskEntity> baseRepository,IBaseRepository<UserEntity> userRepository,
        ILogger<TaskService> logger, IUserService userService)
    {
        _userRepository = userRepository;
        _baseRepository = baseRepository;
        _logger = logger;
        _userService = userService;
    }

    public async Task<IBaseResponse<TaskEntity>> CreateTask(CreateTaskViewModel model)
    {
        try
        {
            var task = new TaskEntity();
            if (model.UserId.Equals(null))
            {
                var us2 = await _userService.GetById(model.NachId);
                model.UserId = long.Parse(us2.NachalnikId);
            }
            //Шифрование
            byte[] key = new byte[16];
            byte[] iv = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
                rng.GetBytes(iv);
            }
            
            task = new TaskEntity()
            {
                NachId = model.NachId,
                Name = Encrypt(model.Name,key,iv),
                Status = StatTask.notTaked,
                Created = DateTime.Now,
                UserId = model.UserId?? 0,
                EncryptionKey = key,
                InitializationVector = iv
            };
            
            await _baseRepository.Create(task);
            return new BaseResponse<TaskEntity>()
            {
                Description = "Задача создалась",
                StatusCode = StatusCode.OK
            };

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[TaskService.Create]: {ex.Message}");
            return new BaseResponse<TaskEntity>()
            {
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<DataTableResult> GetTasks(TaskFilter filter)
{
    try
    {
        DateTime startDate = DateTime.Now;
        DateTime endDate = DateTime.Now;
        DateTime startDate2 = DateTime.Now;
        DateTime endDate2 = DateTime.Now;
        string format = "dd-MM-yyyy";
        
        

        if (!string.IsNullOrWhiteSpace(filter.StartDate))
        {
            startDate = DateTime.ParseExact(filter.StartDate, format, CultureInfo.InvariantCulture);
        }
        if (!string.IsNullOrWhiteSpace(filter.EndDate))
        {
            endDate = DateTime.ParseExact(filter.EndDate, format, CultureInfo.InvariantCulture);
        }
        if (!string.IsNullOrWhiteSpace(filter.StartDate2))
        {
            startDate2 = DateTime.ParseExact(filter.StartDate2, format, CultureInfo.InvariantCulture);
        }
        if (!string.IsNullOrWhiteSpace(filter.EndDate2))
        {
            endDate2 = DateTime.ParseExact(filter.EndDate2, format, CultureInfo.InvariantCulture);
        }

        
        
        var query = _baseRepository.GetAll();
        var decryptedTasks = new List<TaskViewModel>();

        foreach (var currentTask in query)
        {
            var decryptedTask = new TaskViewModel()
            {
                Id = currentTask.Id,
                Name = Decrypt(currentTask.Name,currentTask.EncryptionKey,currentTask.InitializationVector) ,
                IsDone = currentTask.Status.GetDisplayName(),
                Created = currentTask.Created.ToLongDateString(),
                Ended = currentTask.Ended.HasValue? currentTask.Ended.Value.ToLongDateString():null,
                /*User = Decrypt(_userRepository.GetAll().FirstOrDefaultAsync(u => u.Id.ToString() == currentUser.UserId).EnFIO,currentTask.User.EncryptionKey,currentTask.User.InitializationVector),
                */
                UserId = currentTask.UserId.ToString(),
                NachId = currentTask.NachId
            };

            decryptedTasks.Add(decryptedTask);
        }
        
        
        if (!(filter.UserId.Equals(0)))
        {
            decryptedTasks = decryptedTasks.Where(x => x.UserId == (filter.UserId).ToString()).ToList();
        }
        
        
        
        foreach (var currentUser in decryptedTasks)
        {
            string FIO=null, UsFio=null;
            UserEntity Us,Nach;
            Us = _userRepository.GetAll().FirstOrDefault(u => u.Id.ToString() == currentUser.UserId);
            UsFio = Decrypt(Us.EnFIO, Us.EncryptionKey, Us.InitializationVector);
            currentUser.User = UsFio;
            Nach = _userRepository.GetAll().FirstOrDefault(u => u.Id == currentUser.NachId);
            UsFio= Decrypt(Nach.EnFIO, Nach.EncryptionKey, Nach.InitializationVector);
            currentUser.Nachalnik = UsFio;
        }

        if (!(filter.NachId.Equals(0)))
        {
            decryptedTasks = decryptedTasks.Where(x => x.NachId == filter.NachId).ToList();
        }

       
        
        if (filter.IsDone.HasValue)
        {
            decryptedTasks = decryptedTasks.Where(x => x.IsDone == filter.IsDone.Value.GetDisplayName()).ToList();
        }

        if (!string.IsNullOrWhiteSpace(filter.UserName))
        {
            decryptedTasks = decryptedTasks.Where(x => x.User.ToLower().StartsWith(filter.UserName)).ToList();
        }
        if (!string.IsNullOrWhiteSpace(filter.NachName))
        {
            decryptedTasks = decryptedTasks.Where(x => x.Nachalnik.ToLower().StartsWith(filter.NachName)).ToList();
        }
        
        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            decryptedTasks = decryptedTasks.Where(x => x.Name.StartsWith(filter.Name)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(filter.StartDate))
        {
            decryptedTasks = decryptedTasks.Where(x => DateTime.Parse(x.Created) >= startDate).ToList();
        }

        if (!string.IsNullOrWhiteSpace(filter.EndDate))
        {
            decryptedTasks = decryptedTasks.Where(x => DateTime.Parse(x.Created) <= endDate).ToList();
        }

        if (!string.IsNullOrWhiteSpace(filter.StartDate2))
        {
            _logger.LogError(startDate2.ToString());
            decryptedTasks = decryptedTasks.Where(x => x.Ended != null?DateTime.Parse(x.Ended) >= startDate2:false).ToList();
        }

        if (!string.IsNullOrWhiteSpace(filter.EndDate2))
        {
            decryptedTasks = decryptedTasks.Where(x => x.Ended != null?DateTime.Parse(x.Ended) <= endDate2:false).ToList();
        }

        // Dynamic Sorting
        if (!string.IsNullOrWhiteSpace(filter.SortColumn) && !string.IsNullOrWhiteSpace(filter.SortDirection))
        {
            decryptedTasks = ApplySorting(decryptedTasks, filter.SortColumn, filter.SortDirection);
        }
        else
        {
            decryptedTasks = decryptedTasks.OrderBy(x => x.Id).ToList(); // Default sorting by Id
        }

        var pagedTasks = decryptedTasks
            .Skip(filter.Skip)
            .Take(filter.PageSize)
            .ToList();

        var count = pagedTasks.Count;

        return new DataTableResult()
        {
            Data = pagedTasks,
            Total = count
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"[TaskService.GetTasks]: {ex.Message}");
        return new DataTableResult()
        {
            Data = null,
            Total = 0
        };
    }
}

    private List<TaskViewModel> ApplySorting(List<TaskViewModel> query, string sortColumn, string sortDirection)
    {
        var queryable = query.AsQueryable();

        var parameter = Expression.Parameter(typeof(TaskViewModel), "x");
        var property = Expression.Property(parameter, sortColumn);
        var lambda = Expression.Lambda(property, parameter);

        string methodName = sortDirection == "desc" ? "OrderByDescending" : "OrderBy";
        var methodCall = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(TaskViewModel), property.Type }, queryable.Expression, Expression.Quote(lambda));

        var sortedQuery = queryable.Provider.CreateQuery<TaskViewModel>(methodCall);

        return sortedQuery.ToList();
    }
    
    public async Task<IBaseResponse<bool>> GetTasks4PDF(TaskFilter filter)
{
    try
    {
        DateTime startDate = DateTime.Now;
        DateTime endDate = DateTime.Now;
        DateTime startDate2 = DateTime.Now;
        DateTime endDate2 = DateTime.Now;
        string format = "dd-MM-yyyy";
        
        

        if (!string.IsNullOrWhiteSpace(filter.StartDate))
        {
            startDate = DateTime.ParseExact(filter.StartDate, format, CultureInfo.InvariantCulture);
        }
        if (!string.IsNullOrWhiteSpace(filter.EndDate))
        {
            endDate = DateTime.ParseExact(filter.EndDate, format, CultureInfo.InvariantCulture);
        }
        if (!string.IsNullOrWhiteSpace(filter.StartDate2))
        {
            startDate2 = DateTime.ParseExact(filter.StartDate2, format, CultureInfo.InvariantCulture);
        }
        if (!string.IsNullOrWhiteSpace(filter.EndDate2))
        {
            endDate2 = DateTime.ParseExact(filter.EndDate2, format, CultureInfo.InvariantCulture);
        }

        
        
        var query = _baseRepository.GetAll();
        var decryptedTasks = new List<TaskViewModel>();

        foreach (var currentTask in query)
        {
            var decryptedTask = new TaskViewModel()
            {
                Id = currentTask.Id,
                Name = Decrypt(currentTask.Name,currentTask.EncryptionKey,currentTask.InitializationVector) ,
                IsDone = currentTask.Status.GetDisplayName(),
                Created = currentTask.Created.ToLongDateString(),
                Ended = currentTask.Ended.HasValue? currentTask.Ended.Value.ToLongDateString():null,
                /*User = Decrypt(_userRepository.GetAll().FirstOrDefaultAsync(u => u.Id.ToString() == currentUser.UserId).EnFIO,currentTask.User.EncryptionKey,currentTask.User.InitializationVector),
                */
                UserId = currentTask.UserId.ToString(),
                NachId = currentTask.NachId
            };

            decryptedTasks.Add(decryptedTask);
        }
        
        
        if (!(filter.UserId.Equals(0)))
        {
            decryptedTasks = decryptedTasks.Where(x => x.UserId == (filter.UserId).ToString()).ToList();
        }
        
        string FIO=null, UsFio=null;
        UserEntity Us;
        
        foreach (var currentUser in decryptedTasks)
        {
            Us = _userRepository.GetAll().FirstOrDefault(u => u.Id.ToString() == currentUser.UserId);
            UsFio = Decrypt(Us.EnFIO, Us.EncryptionKey, Us.InitializationVector);
            currentUser.User = UsFio;
        }

        if (!(filter.NachId.Equals(0)))
        {
            decryptedTasks = decryptedTasks.Where(x => x.NachId == filter.NachId).ToList();
        }

       
        
        if (filter.IsDone.HasValue)
        {
            decryptedTasks = decryptedTasks.Where(x => x.IsDone == filter.IsDone.Value.GetDisplayName()).ToList();
        }

        if (!string.IsNullOrWhiteSpace(filter.UserName))
        {
            decryptedTasks = decryptedTasks.Where(x => x.User.ToLower().StartsWith(filter.UserName)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            decryptedTasks = decryptedTasks.Where(x => x.Name.StartsWith(filter.Name)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(filter.StartDate))
        {
            decryptedTasks = decryptedTasks.Where(x => DateTime.Parse(x.Created) >= startDate).ToList();
        }

        if (!string.IsNullOrWhiteSpace(filter.EndDate))
        {
            decryptedTasks = decryptedTasks.Where(x => DateTime.Parse(x.Created) <= endDate).ToList();
        }

        if (!string.IsNullOrWhiteSpace(filter.StartDate2))
        {
            _logger.LogError(startDate2.ToString());
            decryptedTasks = decryptedTasks.Where(x => x.Ended != null?DateTime.Parse(x.Ended) >= startDate2:false).ToList();
        }

        if (!string.IsNullOrWhiteSpace(filter.EndDate2))
        {
            decryptedTasks = decryptedTasks.Where(x => x.Ended != null?DateTime.Parse(x.Ended) <= endDate2:false).ToList();
        }

       

        var pagedTasks = decryptedTasks
            .Skip(filter.Skip)
            .Take(filter.PageSize)
            .ToList();

        var count = pagedTasks.Count;
        if (PDFGeneration(decryptedTasks, "Отчёт.pdf"))
        {
            return new BaseResponse<bool>()
            {
                Description = "Файл создан",
                StatusCode = StatusCode.OK
            };
        }
        else
        {
            return new BaseResponse<bool>()
            {
                Description = "Файл не создан",
                StatusCode = StatusCode.TaskNotFound
            };
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"[TaskService.GetTasks]: {ex.Message}");
        
        return new BaseResponse<bool>()
        {
            Description = "Файл не создан",
            StatusCode = StatusCode.TaskNotFound
        };
    }
}
    // Метод для генерации PDF
    public bool PDFGeneration(List<TaskViewModel> tasks, string filename)
    {
        try
        {
            // Проверяем, существует ли файл
            if (File.Exists(filename))
            {
                // Если файл существует, то удаляем его
                File.Delete(filename);
            }
        // Создаем документ PDF
        using (var pdfDoc = new iTextSharp.text.Document(PageSize.A4, 25, 25, 30, 30))
        {
            // Создаем поток для записи PDF
            using (var pdfWriter = PdfWriter.GetInstance(pdfDoc, new FileStream("wwwroot\\Отчёт.pdf", FileMode.Create)))
            {
                pdfDoc.Open();
                
                // Создаем таблицу
                var table = new PdfPTable(5);
                table.TotalWidth = 500f;
                table.LockedWidth = true;
                //table.SetWidths(new float[] { 20, 100, 20, 50, 20, 50 });
                string ttf = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIAL.TTF");
                var baseFont = BaseFont.CreateFont(ttf, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                var font = new iTextSharp.text.Font(baseFont, iTextSharp.text.Font.DEFAULTSIZE, iTextSharp.text.Font.NORMAL);
                // Заголовки столбцов
                var header = new PdfPCell(new Phrase("Название",font));
                header.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(header);

                header = new PdfPCell(new Phrase("Статус",font));
                header.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(header);

                header = new PdfPCell(new Phrase("Создано",font));
                header.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(header);

                header = new PdfPCell(new Phrase("Завершено",font));
                header.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(header);

                header = new PdfPCell(new Phrase("Работник",font));
                header.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(header);

                // Добавляем данные в таблицу
                foreach (var task in tasks)
                {
                    
                    table.AddCell(new Phrase(task.Name,font));
                    table.AddCell(new Phrase(task.IsDone,font));
                    table.AddCell(new Phrase(task.Created,font));
                    table.AddCell(new Phrase(task.Ended,font));
                    table.AddCell(new Phrase(task.User,font));
                    
                }

                // Добавляем таблицу в документ
                
                pdfDoc.Add(table);
                 // Добавляем место для подписи
                 pdfDoc.Add(new Paragraph(" ", font));
                 pdfDoc.Add(new Paragraph("                                         Подпись: ________________________        Дата: ____________________", font));
                pdfDoc.Close();
            }
            
            return true;
        }
        }
        catch (Exception e)
        {
            
            _logger.LogError(e.ToString());
            return false;
        }
        
    }


    public async Task<IBaseResponse<bool>> EndTask(TaskChangeModel model)
    {
        try
        {
            var task = await _baseRepository.GetAll().FirstOrDefaultAsync(x => x.Id == model.Id);
            if (task == null)
            {
                return new BaseResponse<bool>()
                {
                    Description = "Задача не найдена",
                    StatusCode = StatusCode.TaskNotFound
                };
            }

            StatTask st=StatTask.notTaked;

            switch (model.Stat)
            {
                case 1:
                    st = StatTask.notTaked;
                    break;
                case 2:
                    st = StatTask.compleated;
                    break;
                case 3:
                    st = StatTask.inWork;
                    break;
                case 4:
                    st = StatTask.canseled;
                    break;
                case 5:
                    st = StatTask.isStoped;
                    break;
                
            }

            if(st==StatTask.compleated)task.Ended = DateTime.Now;
            if (model.Stat != null && model.Stat != 0)
            {
                task.Status = st;
                await _baseRepository.Update(task);
                return new BaseResponse<bool>()
                {
                    Description = "Статус обновлён",
                    StatusCode = StatusCode.OK
                };
            }
            else
            {
                return new BaseResponse<bool>()
                {
                    Description = "Статус не обновлён",
                    StatusCode = StatusCode.OK
                };
            }
            
            
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[TaskService.EndTask]: {ex.Message}");
            return new BaseResponse<bool>()
            {
                Description = $"{ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
    /*public async Task<DataTableResult> GetCompletedTasks(TaskFilter filter)
    {
        try
        {
            var tasks = await _baseRepository.GetAll()
                .Where(x => (x.Status==StatTask.compleated))
                .Select(x => new TaskCompleatedViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        UserId = x.UserId.ToString(),
                        
                    }
                ) .ToListAsync();
            var count = _baseRepository.GetAll().Count(x => !(x.Status==StatTask.compleated));
            return new DataTableResult()
            {
                Data = tasks,
                Total = count
            };

        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"[TaskService.GetCompleatedTask]: {ex.Message}");
            return new DataTableResult()
            {
                Data = null,
                Total = 0
            };
        }
    }*/
    public static byte[] Encrypt(string tex, byte[] key, byte[] iv)
    {
        byte[] cipheredText;
        using ( Aes aes=Aes.Create())
        {
            ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream,encryptor,CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(tex);
                    }

                    cipheredText = memoryStream.ToArray();
                }
            }
            
        }
        return cipheredText;
    }
    public string Decrypt(byte[] cipheredtext, byte[] key, byte[] iv)
    {
        string tex = String.Empty;
        using (Aes aes = Aes.Create())
        {
            ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
            using (MemoryStream memoryStream = new MemoryStream(cipheredtext))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        tex = streamReader.ReadToEnd();
                    }
                }
            }
        }
        return tex;
    }
}