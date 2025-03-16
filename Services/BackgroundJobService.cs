using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services;

public class BackgroundJobService
{
    private readonly ITaskService _taskService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<BackgroundJobService> _logger;

    public BackgroundJobService(
        ITaskService taskService,
        IHttpClientFactory httpClientFactory,
        ILogger<BackgroundJobService> logger)
    {
        _taskService = taskService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async System.Threading.Tasks.Task ProcessTask(int taskId)
    {
        Models.Task? task = null;

        try
        {
            task = await _taskService.GetTask(taskId);
            if (task == null)
            {
                _logger.LogError("Task {TaskId} not found", taskId);
                return;
            }

            _logger.LogInformation("Starting processing for Task {TaskId}", taskId);

            task.Status = Status.InProgress;
            await _taskService.UpdateTask(taskId, task);
            _logger.LogInformation("Task {TaskId} status changed to {Status}", taskId, task.Status);

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://jsonplaceholder.typicode.com/todos/{taskId}");

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Raw API response for Task {TaskId}: {Response}", taskId, responseContent);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("API request failed for Task {TaskId} with status {StatusCode}", taskId, response.StatusCode);
                task.Status = Status.Failed;
            }
            else
            {
                JsonPlaceholderTodo? todo = null;
                try
                {
                    todo = await response.Content.ReadFromJsonAsync<JsonPlaceholderTodo>();
                }
                catch (Exception jsonEx)
                {
                    _logger.LogError(jsonEx, "Failed to deserialize API response for Task {TaskId}", taskId);
                    task.Status = Status.Failed;
                }

                if (todo != null)
                {
                    var newStatus = todo.Completed ? Status.Completed : Status.InProgress;
                    _logger.LogInformation("Determined status for Task {TaskId}: {Status} (Completed flag: {Completed})",
                        taskId, newStatus, todo.Completed);
                    task.Status = newStatus;
                }
            }

            await _taskService.UpdateTask(taskId, task);
            _logger.LogInformation("Task {TaskId} final status updated to {Status}", taskId, task.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Task {TaskId}", taskId);
            if (task != null)
            {
                task.Status = Status.Failed;
                await _taskService.UpdateTask(taskId, task);
                _logger.LogError("Task {TaskId} marked as Failed due to processing error", taskId);
            }
        }
        finally
        {
            if (task != null)
            {
                try
                {
                    task.UpdatedAt = DateTime.UtcNow;
                    await _taskService.UpdateTask(taskId, task);
                    _logger.LogDebug("Task {TaskId} timestamp updated to {Timestamp}", taskId, task.UpdatedAt.ToString("o"));
                }
                catch (Exception finalEx)
                {
                    _logger.LogError(finalEx, "Failed to update Task {TaskId} timestamp", taskId);
                }
            }
        }
    }

}
public record JsonPlaceholderTodo(int UserId, int Id, string Title, bool Completed);