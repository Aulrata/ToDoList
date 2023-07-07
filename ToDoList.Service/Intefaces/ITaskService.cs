using ToDoList.Domain.Entity;
using ToDoList.Domain.Filters.Task;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;

namespace ToDoList.Service.Intefaces
{
    public interface ITaskService
    {
        Task<IBaseResponse<TaskEntity>> Create(CreateTaskViewModel model);

        Task<IBaseResponse<IEnumerable<TaskViewModel>>> GetTasks(TaskFilter filter);

        Task<IBaseResponse<bool>> EndTask(int id);

    }
}
