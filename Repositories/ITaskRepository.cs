using System.Threading.Tasks;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories;

public interface ITaskRepository
{
    Task<Models.Task> Add(Models.Task task);
    Task<Models.Task?> GetById(int id);
    Task<List<Models.Task>> GetAll();
    Task<Models.Task> Update(Models.Task task);
    Task<bool> Delete(Models.Task task);
}