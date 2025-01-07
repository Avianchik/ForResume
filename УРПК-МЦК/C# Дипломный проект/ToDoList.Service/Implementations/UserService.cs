using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using ToDoList.Domain.ViewModels.User;
using ToDoList.Service.Interfaces;

namespace ToDoList.Service.Implementations;

public class UserService : IUserService
{
    private readonly IBaseRepository<UserEntity> _userRepository;
    private ILogger<UserService> _logger;
    private JWTService _jwtService;
    private readonly IBaseRepository<DepartamentEntity> _depRepository;
    private readonly IWebHostEnvironment _environment;

    public UserService(IBaseRepository<UserEntity> userRepository,
        ILogger<UserService> logger, JWTService jwtService, IBaseRepository<DepartamentEntity> depRepository)
    {
        _userRepository = userRepository;
        _logger = logger;
        _jwtService = jwtService;
        _depRepository = depRepository;
    }
    
    public async Task<UserEntity> GetByLogin(string login)
    {
        try
        {
            
            var users =  _userRepository.GetAll()
                .AsNoTracking()
                .ToList();
            var user = users.FirstOrDefault(u => Decrypt(u.EnLogin, u.EncryptionKey, u.InitializationVector) == login);
            return user;
            if (user == null)
            {
                return null;
            }else 
                return user;
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"[UserService.GetByLogin]: {ex.Message}");
            return null;
        }
    }
    
    public async Task<UserViewModel> GetById(long id)
    {
        
        try
        {
            var model = await _userRepository.GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
            if (model == null)
            {
                return null;
            }
            else
            {
                return new UserViewModel()
                {
                    Id = model.Id,
                    FIO = Decrypt(model.EnFIO,model.EncryptionKey,model.InitializationVector) ,
                    login = Decrypt(model.EnLogin,model.EncryptionKey,model.InitializationVector) ,
                    password = Decrypt(model.EnPass,model.EncryptionKey,model.InitializationVector) ,
                    role = Convert.ToString(model.role),
                    DepartamentId = Convert.ToString(model.DepartamentId),
                    Departament = Convert.ToString(model.Departament),
                    NachalnikId =Convert.ToString(model.NachalnikId) 
                };
            }
                
            
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, $"[UserService.GetByLogin]: {ex.Message}");
            return null;
        }
    }

    
    

    public async Task<IBaseResponse<bool>> Login(LoginUserViewModel model, HttpContext context)
    {
        try
        {
            
            var user = await GetByLogin(model.login);
            
            
            if (Decrypt( user.EnPass,user.EncryptionKey,user.InitializationVector) == model.password)
            {
                var result = user;
            }
            else
            {
                return new BaseResponse<bool>()
                {
                    Description = "Пароли не совпадают",
                    StatusCode = StatusCode.TaskNotFound
                };
            }
            var token = _jwtService.GenerateToken(user);
            context.Response.Cookies.Append("tasty-cookies", token);
            return new BaseResponse<bool>()
            {
                Description = "Вход успешен",
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[UserService.Login]: {ex.Message}");
            return new BaseResponse<bool>()
            {
                Description = $"{ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<UserEntity>> Create(CreateUserViewModel model)
    {
        try
        {
            var user = new UserEntity();
            _logger.LogInformation($"Запрос на создание пользователя - {model.NachalnikId}");
            
            var users =  _userRepository.GetAll()
                .AsNoTracking()
                .ToList();
            
            //ПРОВЕРИТЬ НА СОЗДАНИЕ АНАЛОГИЧНЫХ ЛОГИНОВ
            var existingUser = users.FirstOrDefault(u => Decrypt(u.EnLogin, u.EncryptionKey, u.InitializationVector) == model.login);

            if (existingUser != null)
            {
                return new BaseResponse<UserEntity>()
                {
                    Description = "Пользователь с таким логином уже существует",
                    StatusCode = StatusCode.TaskIsHasAlready
                };
            }
            
            
            
            //Шифрование
            byte[] key = new byte[16];
            byte[] iv = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
                rng.GetBytes(iv);
            }

            if (model.NachalnikId.Equals(null))
            {
                user = new UserEntity()
                {
                    EnFIO = Encrypt(model.FIO,key,iv),
                    EnLogin = Encrypt(model.login,key,iv),
                    EnPass = Encrypt(model.password,key,iv),
                    role = model.role,
                    DepartamentId = model.DepartamentId,
                    EncryptionKey = key,
                    InitializationVector = iv
                };
            }
            else
            {
                user = new UserEntity()
                {
                    EnFIO = Encrypt(model.FIO,key,iv),
                    EnLogin = Encrypt(model.login,key,iv),
                    EnPass = Encrypt(model.password,key,iv),
                    role = model.role,
                    DepartamentId = model.DepartamentId,
                    NachalnikId = model.NachalnikId,
                    EncryptionKey = key,
                    InitializationVector = iv
                };
            }
            
            

            await _userRepository.Create(user);
            // _logger.LogInformation(model.logID.ToString());
            // var logFIO = GetById(System.Convert.ToUInt32(model.logID));
            using (StreamWriter writer = new StreamWriter("logs\\app.log", true))
            {
                writer.WriteLine($"{DateTime.Now} | Создание пользователя {model.FIO}");
            }
            //_logger.LogInformation($"Пользователь создан: {user.FIO} ");
            return new BaseResponse<UserEntity>()
            {
                Description = "Пользователь создан",
                StatusCode = StatusCode.OK
            };

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[UserService.Create]: {ex.Message}");
            return new BaseResponse<UserEntity>()
            {
                Description = "Пользователь не создан",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<DataTableResult> GetAll(UserFilter filter)
{
    try
    {
        var query = await _userRepository.GetAll().ToListAsync();

        var decryptedUsers = new List<UserViewModel>();

        foreach (var currentUser in query)
        {
            string NachFIO = string.Empty;
            string Depart = string.Empty;
            string DepNum = string.Empty;
            string dd = string.Empty;

            if (currentUser.NachalnikId != null)
            {
                var Nach = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == currentUser.NachalnikId);
                if (Nach != null)
                {
                    NachFIO = Decrypt(Nach.EnFIO, Nach.EncryptionKey, Nach.InitializationVector);
                }
            }
            
            if (currentUser.DepartamentId != null)
            {
                DepFilter fil = new DepFilter()
                {
                    idDep = currentUser.DepartamentId.ToString()
                };
                var departament = await _depRepository.GetAll().FirstOrDefaultAsync(x=>x.DepartamentId==currentUser.DepartamentId);
                if (departament != null)
                {
                    Depart = Decrypt(departament.DepartamentName, departament.EncryptionKey, departament.InitializationVector);
                    DepNum =departament.DepartamentNumber.ToString();
                }
            }

            var decryptedUser = new UserViewModel()
            {
                Id = currentUser.Id,
                FIO = Decrypt(currentUser.EnFIO, currentUser.EncryptionKey, currentUser.InitializationVector),
                login = Decrypt(currentUser.EnLogin, currentUser.EncryptionKey, currentUser.InitializationVector),
                password = Decrypt(currentUser.EnPass, currentUser.EncryptionKey, currentUser.InitializationVector),
                role = currentUser.role.GetDisplayName(),
                depidd = currentUser.DepartamentId.ToString(),
                DepartamentId = DepNum,
                NachalnikId = currentUser.NachalnikId != null ? currentUser.NachalnikId.ToString() : string.Empty,
                Nachalnik = NachFIO,
                Departament = Depart // Устанавливаем DepartamentName
            };

            decryptedUsers.Add(decryptedUser);
        }

        var usersWithRole1= decryptedUsers.Where(x => x.Id == 0).ToList();;
        if (!string.IsNullOrWhiteSpace(filter.forn)&& filter.forn == "GlavDeportament")
        {
            usersWithRole1 = decryptedUsers.Where(x => x.role == "Начальник отдела").ToList();
            usersWithRole1 = usersWithRole1.Where(x => x.Id != filter.NachalnikId).ToList();
        }
        if (!string.IsNullOrWhiteSpace(filter.FIO))
        {
            string filterFIO = filter.FIO.ToLower(); // Приводим filter.FIO к нижнему регистру

            decryptedUsers = decryptedUsers.Where(x => x.FIO.ToLower().StartsWith(filterFIO)).ToList();
        }
        
        
        if (filter.role.HasValue)
        {
            decryptedUsers = decryptedUsers.Where(x => x.role == filter.role.Value.GetDisplayName()).ToList();
        }
        if (filter.DepartamentId != 0)
        {
            decryptedUsers = decryptedUsers.Where(x => x.depidd == filter.DepartamentId.ToString()).ToList();
        }

        if (filter.NachalnikId != 0)
        {
            decryptedUsers = decryptedUsers.Where(x => x.NachalnikId == filter.NachalnikId.ToString()).ToList();
        }
        if (!string.IsNullOrWhiteSpace(filter.departamentName))
        {
            string filterDepartamentName = filter.departamentName.ToLower(); // Приводим filter.departamentName к нижнему регистру

            decryptedUsers = decryptedUsers.Where(x => x.Departament.ToLower().StartsWith(filter.departamentName)).ToList();
        }
        decryptedUsers.AddRange(usersWithRole1);
        
        // Остальной код остается без изменений
        // Сортировка
        if (!string.IsNullOrWhiteSpace(filter.SortColumn) && !string.IsNullOrWhiteSpace(filter.SortDirection))
        {
            decryptedUsers = ApplySorting(decryptedUsers, filter.SortColumn, filter.SortDirection);
        }
        else
        {
            decryptedUsers = decryptedUsers.OrderBy(x => x.Id).ToList(); // Сортировка по умолчанию по Id
        }
        var pagedUsers = decryptedUsers
            .Skip(filter.Skip)
            .Take(filter.PageSize)
            .ToList();

        var count = pagedUsers.Count;

        return new DataTableResult()
        {
            Data = pagedUsers,
            Total = count
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"[UserService.GetAll]: {ex.Message}");
        return new DataTableResult()
        {
            Data = null,
            Total = 0
        };
    }
}




    private List<UserViewModel> ApplySorting(List<UserViewModel> query, string sortColumn, string sortDirection)
    {
        var queryable = query.AsQueryable();

        var parameter = Expression.Parameter(typeof(UserViewModel), "x");
        var property = Expression.Property(parameter, sortColumn);
        var lambda = Expression.Lambda(property, parameter);

        string methodName = sortDirection == "desc" ? "OrderByDescending" : "OrderBy";
        var methodCall = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(UserViewModel), property.Type }, queryable.Expression, Expression.Quote(lambda));

        var sortedQuery = queryable.Provider.CreateQuery<UserViewModel>(methodCall);

        return sortedQuery.ToList();
    }
    public async Task<IBaseResponse<bool>> DeleteUser(long id)
    {
        try
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                return new BaseResponse<bool>()
                {
                    Description = "Пользователь не найден",
                    StatusCode = StatusCode.TaskNotFound
                };
            }
            using (StreamWriter writer = new StreamWriter("logs\\app.log", true))
            {
                string s = Decrypt(user.EnFIO, user.EncryptionKey, user.InitializationVector);
                writer.WriteLine($"{DateTime.Now} | Удаление пользователя {s}");
            }

            await _userRepository.Delete(user);
            
            return new BaseResponse<bool>()
            {
                Description = "Пользователь удалён",
                StatusCode = StatusCode.OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"[UserService.Delete]: {ex.Message}");
            return new BaseResponse<bool>()
            {
                Description = $"{ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<bool>> UpdateUser(UpdateUserViewModel model)
    {
        try
        {
            var user = await _userRepository.GetAll()
                .FirstOrDefaultAsync(x => x.Id == model.Id);
        
            if (user == null)
            {
                return new BaseResponse<bool>()
                {
                    Description = "Пользователь не найден",
                    StatusCode = StatusCode.TaskNotFound
                };
            }

            switch (model.OperationUpdate) {
                case 1:
                {
                    string s = Decrypt(user.EnFIO, user.EncryptionKey, user.InitializationVector);
                    user.EnFIO = Encrypt(model.FIO,user.EncryptionKey,user.InitializationVector);
                    using (StreamWriter writer = new StreamWriter("logs\\app.log", true))
                    {
                        writer.WriteLine($"{DateTime.Now} | Смена ФИО пользователя с {s} на {model.FIO}");
                    }
                    break;
                }
                case 2:
                {
                    var usLog = Encrypt(model.login,user.EncryptionKey,user.InitializationVector);
                    using (StreamWriter writer = new StreamWriter("logs\\app.log", true))
                    {
                        string s = Decrypt(user.EnFIO, user.EncryptionKey, user.InitializationVector);
                        writer.WriteLine($"{DateTime.Now} | Попытка смены логина пользователя {s}");
                    }
                    var users =  _userRepository.GetAll()
                        .AsNoTracking()
                        .ToList();
                    var existingUser = users.FirstOrDefault(u => Decrypt(u.EnLogin, u.EncryptionKey, u.InitializationVector) == model.login);


                    if (existingUser != null)
                    {
                        return new BaseResponse<bool>()
                        {
                            Description = "Пользователь с таким логином уже существует",
                            StatusCode = StatusCode.TaskIsHasAlready
                        };
                    }

                    user.EnLogin = usLog;
                    break;
                }
                case 3:
                {
                    user.EnPass = Encrypt(model.password,user.EncryptionKey,user.InitializationVector);
                    using (StreamWriter writer = new StreamWriter("logs\\app.log", true))
                    {
                        string s = Decrypt(user.EnFIO, user.EncryptionKey, user.InitializationVector);
                        writer.WriteLine($"{DateTime.Now} | Cмена пароля пользователя {s}");
                    }
                    break;
                }
                case 4:
                {
                    user.NachalnikId = model.NachalnikId;
                    using (StreamWriter writer = new StreamWriter("logs\\app.log", true))
                    {
                        string s = Decrypt(user.EnFIO, user.EncryptionKey, user.InitializationVector);
                        writer.WriteLine($"{DateTime.Now} | Cмена начальника пользователя {s}");
                    }
                    break;
                }
                    
            }

            await _userRepository.Update(user);
            
            return new BaseResponse<bool>()
            {
                Description = "Пользователь изменен",
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