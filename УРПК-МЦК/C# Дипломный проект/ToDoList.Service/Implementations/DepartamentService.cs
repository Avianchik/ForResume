using System.Linq.Expressions;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoList.DAL.Interfaces;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Enum;
using ToDoList.Domain.Extenstions;
using ToDoList.Domain.Filters;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Departament;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Service.Interfaces;

namespace ToDoList.Service.Implementations;

public class DepartamentService : IDepService
{
    private readonly IBaseRepository<DepartamentEntity> _depRepository;
    private ILogger<DepartamentService> _logger;
    private readonly IBaseRepository<UserEntity> _userRepository;

    public DepartamentService(IBaseRepository<DepartamentEntity> depRepository,
        ILogger<DepartamentService> logger,IBaseRepository<UserEntity> userRepository)
    {
        _depRepository = depRepository;
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<IBaseResponse<DepartamentEntity>> Create(CreateDepViewModel model)
    {
        try
        {
            _logger.LogInformation($"Запрос на создание отдела - {model.DepartamentName}");
            var deps =  _depRepository.GetAll().AsNoTracking().ToList();
            var existingUser = deps.FirstOrDefault(u => Decrypt(u.DepartamentName, u.EncryptionKey, u.InitializationVector) == model.DepartamentName);
            if (existingUser != null)
            {
                return new BaseResponse<DepartamentEntity>()
                {
                    Description = "Отдел с таким названием уже есть",
                    StatusCode = StatusCode.TaskIsHasAlready
                };
            }
            // Получаем последнее значение DepartamentNumber
            long lastDepartamentNumber = deps.Any() ? deps.Max(x => x.DepartamentNumber) + 1 : 1;
            //Шифрование
            byte[] key = new byte[16];
            byte[] iv = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
                rng.GetBytes(iv);
            }
            
            var dep = new DepartamentEntity()
            {
                DepartamentName = Encrypt(model.DepartamentName,key,iv),
                EncryptionKey = key,
                InitializationVector = iv,
                DepartamentNumber = lastDepartamentNumber
                
            };
            await _depRepository.Create(dep);
            
            

            _logger.LogInformation($"Отдел создан ");
            return new BaseResponse<DepartamentEntity>()
            {
                Description = "Отдел создан",
                StatusCode = StatusCode.OK
            };

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[DepService.Create]: {ex.Message}");
            return new BaseResponse<DepartamentEntity>()
            {
                Description = "Отдел не создан",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<DataTableResult> GetAll(DepFilter filter)
    {
        try
        {
            var query = _depRepository.GetAll();
            var decryptedDeps = new List<DepViewModel>();

            foreach (var currentDep in query)
            {
                var decryptedDep = new DepViewModel()
                {
                    DepartamentNumber = currentDep.DepartamentNumber,
                    DepartamentId = currentDep.DepartamentId,
                    DepartamentName = Decrypt(currentDep.DepartamentName, currentDep.EncryptionKey, currentDep.InitializationVector)
                };

                decryptedDeps.Add(decryptedDep);
            }
            if (!string.IsNullOrWhiteSpace(filter.DepName))
            {
                string filterDepName = filter.DepName.ToLower(); // Приводим filter.FIO к нижнему регистру
                
                decryptedDeps = decryptedDeps.Where(x => x.DepartamentName.ToLower().StartsWith(filterDepName)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter.idDep))
            {
                decryptedDeps = decryptedDeps.Where(x => x.DepartamentNumber.ToString().StartsWith(filter.idDep)).ToList();
            }
   
            
            if (!string.IsNullOrWhiteSpace(filter.SortColumn) && !string.IsNullOrWhiteSpace(filter.SortDirection))
            {
                decryptedDeps = ApplySorting(decryptedDeps, filter.SortColumn, filter.SortDirection);
            }
            else
            {
                decryptedDeps = decryptedDeps.OrderBy(x => x.DepartamentNumber).ToList(); // Default sorting by Id
            }
            
            var dep = decryptedDeps
                .Select(x => new DepViewModel()
                {
                    DepartamentNumber = x.DepartamentNumber,
                    DepartamentId = x.DepartamentId,
                    DepartamentName = x.DepartamentName
                })
                .Skip(filter.Skip)
                .Take(filter.PageSize)
                .ToList();
            
            var count = query.Count();
            
            return new DataTableResult()
            {
                Data = dep,
                Total = count
            };
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"[DepartamentService.GetAll]: {ex.Message}");
            return new DataTableResult()
            {
                Data = null,
                Total = 0
            };
        }
    }
    
    private List<DepViewModel> ApplySorting(List<DepViewModel> query, string sortColumn, string sortDirection)
    {
        var queryable = query.AsQueryable();

        var parameter = Expression.Parameter(typeof(DepViewModel), "x");
        var property = Expression.Property(parameter, sortColumn);
        var lambda = Expression.Lambda(property, parameter);

        string methodName = sortDirection == "desc" ? "OrderByDescending" : "OrderBy";
        var methodCall = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(DepViewModel), property.Type }, queryable.Expression, Expression.Quote(lambda));

        var sortedQuery = queryable.Provider.CreateQuery<DepViewModel>(methodCall);

        return sortedQuery.ToList();
    }
    
    /*public async Task<DataTableResult> GetAll4User()
    {
        try
        {
            var dep =await _depRepository.GetAll()
                .Select(x => new DepViewModel()
                {
                    DepartamentId = x.DepartamentId,
                    DepartamentName = x.DepartamentName
                })
                .ToListAsync();
            var count = _depRepository.GetAll().Count();
            
            return new DataTableResult()
            {
                Data = dep,
                Total = count
            };
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"[DepartamentService.GetAll4User]: {ex.Message}");
            return new DataTableResult()
            {
                Data = null,
                Total = 0
            };
        }
    }*/

    public async Task<IBaseResponse<bool>> DeleteDep(long id)
    {
        try
        {
            var dep = await _depRepository.GetAll().FirstOrDefaultAsync(x => x.DepartamentId == id);
            if (dep == null)
            {
                return new BaseResponse<bool>()
                {
                    Description = "Отдел не найден",
                    StatusCode = StatusCode.TaskNotFound
                };
            }
            

            await _depRepository.Delete(dep);
            
            return new BaseResponse<bool>()
            {
                Description = "Отдел удалён",
                StatusCode = StatusCode.OK
            };
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
    
    public async Task<IBaseResponse<bool>> UpdateNameDep(UpdateDepViewModel model)
    {
        try
        {
            var dep = await _depRepository.GetAll()
                .FirstOrDefaultAsync(x => x.DepartamentId == model.DepartamentId);
        
            if (dep == null)
            {
                return new BaseResponse<bool>()
                {
                    Description = "Отдел не найден",
                    StatusCode = StatusCode.TaskNotFound
                };
            }

            var allDepartaments = await _depRepository.GetAll().ToListAsync();

            var existingDep = allDepartaments.FirstOrDefault(x =>
                x.DepartamentName.SequenceEqual(
                    DepartamentService.Encrypt(model.DepartamentName, x.EncryptionKey, x.InitializationVector)));

            if (existingDep != null && existingDep.DepartamentId != model.DepartamentId)
            {
                return new BaseResponse<bool>()
                {
                    Description = "Отдел с таким названием уже существует",
                    StatusCode = StatusCode.TaskIsHasAlready
                };
            }

            dep.DepartamentName = Encrypt(model.DepartamentName,dep.EncryptionKey, dep.InitializationVector);

            await _depRepository.Update(dep);
            
            return new BaseResponse<bool>()
            {
                Description = "Название отдела изменено",
                StatusCode = StatusCode.OK
            };
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
    
    public async Task<IBaseResponse<bool>> UpdateIdDep(UpdateDepViewModel model)
    {
        try
        {
            
            var dep = await _depRepository.GetAll()
                .FirstOrDefaultAsync(x => x.DepartamentId == model.DepartamentId);
        
            if (dep == null)
            {
                return new BaseResponse<bool>()
                {
                    Description = "Отдел не найден",
                    StatusCode = StatusCode.TaskNotFound
                };
            }

            long id = long.Parse(model.DepartamentName);
            
            
            var allDepartaments =await  _depRepository.GetAll().FirstOrDefaultAsync(x=>x.DepartamentNumber==id);

            

            if (allDepartaments != null )
            {
                return new BaseResponse<bool>()
                {
                    Description = "Отдел с таким номером уже существует",
                    StatusCode = StatusCode.TaskIsHasAlready
                };
            }
            
            
            dep.DepartamentNumber = id;

            await _depRepository.Update(dep);
            
            return new BaseResponse<bool>()
            {
                Description = "Номер отдела изменен",
                StatusCode = StatusCode.OK
            };
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