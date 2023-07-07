using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using ToDoList.DAL.Intefaces;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Enum;
using ToDoList.Domain.Extenstions;
using ToDoList.Domain.Filters.Task;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Service.Intefaces;

namespace ToDoList.Service.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly IBaseRepository<TaskEntity> _taskRepository;
        private ILogger<TaskService> _logger;

        public TaskService(IBaseRepository<TaskEntity> taskRepository, ILogger<TaskService> logger)
        {
            _taskRepository = taskRepository;
            _logger = logger;
        }

        public async Task<IBaseResponse<IEnumerable<TaskViewModel>>> CalculateCompletedTasks()
        {
            try
            {
                var tasks = await _taskRepository.GetAll()
                    .Where(x => x.Created.Date == DateTime.Today)
                    .Select(x => new TaskViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        IsDone = x.IsDone ==true ? "Готово": "Не готова",
                        Description = x.Description,
                        Priority = x.Priority.ToString(),
                        Created = x.Created.ToString(CultureInfo.InvariantCulture),
                    }).ToListAsync();

                return new BaseResponse<IEnumerable<TaskViewModel>>()
                {
                    Data = tasks,
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"[TaskService.CalculateCompletedTasks]: {ex.Message}");
                return new BaseResponse<IEnumerable<TaskViewModel>>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<TaskEntity>> Create(CreateTaskViewModel model)
        {
            try
            {
                model.Validate();
                _logger.LogInformation($"Запрос на создание задачи - {model.Name}");

                var task = await _taskRepository.GetAll()
                    .Where(x=> x.Created.Date == DateTime.Today)
                    .FirstOrDefaultAsync(x=> x.Name == model.Name);

                if (task != null)
                {
                    return new BaseResponse<TaskEntity>()
                    {
                        Description = "Задача с таким названием уже есть.",
                        StatusCode = StatusCode.TaskIsHasAlready,
                    };
                }

                task = new TaskEntity()
                {
                    Name = model.Name,
                    Description = model.Description,
                    IsDone = false,
                    Priority = model.Priority,
                    Created = DateTime.Now,
                };

                await _taskRepository.Create(task);
                return new BaseResponse<TaskEntity>()
                {
                    Description = "Задача создалась",
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[TaskService.Create]: {ex.Message}");
                return new BaseResponse<TaskEntity>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<bool>> EndTask(int id)
        {
            try
            {
                var task = await _taskRepository.GetAll().FirstOrDefaultAsync(x=> x.Id == id);
                if (task == null)
                {
                    return new BaseResponse<bool>()
                    {
                        Description = "Задача не найдена",
                        StatusCode = StatusCode.TaskNotFound,
                    };
                }
                task.IsDone = true;
                await _taskRepository.Update(task);
                return new BaseResponse<bool>()
                {
                    Description = "Задача завершена",
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[TaskService.EndTask]: {ex.Message}");
                return new BaseResponse<bool>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<IEnumerable<TaskCompletedViewModel>>> GetCompletedTasks()
        {
            try
            {
                var task = await _taskRepository.GetAll()
                    .Where(x => x.IsDone)
                    .Where(x => x.Created.Date == DateTime.Today)
                    .Select(x => new TaskCompletedViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                    }).ToListAsync();

                return new BaseResponse<IEnumerable<TaskCompletedViewModel>>()
                {
                    Data = task,
                    StatusCode = StatusCode.OK,
                };
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"[TaskService.GetCompletedTasks]: {ex.Message}");
                return new BaseResponse<IEnumerable<TaskCompletedViewModel>>()
                {
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }

        public async Task<IBaseResponse<IEnumerable<TaskViewModel>>> GetTasks(TaskFilter filter)
        {
            try
            {
                var tasks = await _taskRepository.GetAll()
                    .Where(x=>!x.IsDone)
                    .WhereIf(!string.IsNullOrWhiteSpace(filter.Name),x=>x.Name == filter.Name)
                    .WhereIf(filter.Priority.HasValue, x=>x.Priority == filter.Priority)
                    .Select(x => new TaskViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        IsDone = x.IsDone == true ? "Готова" : "Не готова",
                        Priority = x.Priority.GetDisplayName(),
                        Created = x.Created.ToLongDateString(),
                    })
                    .ToListAsync();

                return new BaseResponse<IEnumerable<TaskViewModel>>()
                {
                    StatusCode = StatusCode.OK,
                    Data = tasks,
                };
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"[TaskService.GetTasks]: {ex.Message}");
                return new BaseResponse<IEnumerable<TaskViewModel>> ()
                { 
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError,
                };
            }
        }
    }
}