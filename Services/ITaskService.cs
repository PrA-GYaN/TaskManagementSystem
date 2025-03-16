using System.Threading.Tasks;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services;

public interface ITaskService
{
    Task<Models.Task> CreateTask(Models.Task task);
    Task<Models.Task?> GetTask(int id);
    Task<List<Models.Task>> GetAllTasks();
    Task<Models.Task?> UpdateTask(int id, Models.Task task);
    Task<bool> DeleteTask(int id);
}