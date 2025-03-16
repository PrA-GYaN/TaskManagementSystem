using System.Threading.Tasks;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repositories;
using Hangfire;

namespace TaskManagementSystem.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly IBackgroundJobClient _backgroundJob;

    public TaskService(ITaskRepository repository, IBackgroundJobClient backgroundJob)
    {
        _repository = repository;
        _backgroundJob = backgroundJob;
    }

    public async Task<Models.Task> CreateTask(Models.Task task)
    {
        var createdTask = await _repository.Add(task);
        _backgroundJob.Schedule<BackgroundJobService>(
            x => x.ProcessTask(createdTask.Id),
            createdTask.ExecutionDateTime
        );
        return createdTask;
    }

    public async Task<Models.Task?> GetTask(int id) =>
        await _repository.GetById(id);

    public async Task<List<Models.Task>> GetAllTasks() =>
        await _repository.GetAll();

    public async Task<Models.Task?> UpdateTask(int id, Models.Task task)
    {
        var existingTask = await _repository.GetById(id);
        if (existingTask == null) return null;

        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.ExecutionDateTime = task.ExecutionDateTime;
        existingTask.Status = task.Status;
        existingTask.UpdatedAt = DateTime.UtcNow;

        return await _repository.Update(existingTask);
    }

    public async Task<bool> DeleteTask(int id)
    {
        var task = await _repository.GetById(id);
        if (task != null) await _repository.Delete(task);
        return true;
    }
}