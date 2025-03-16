using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;

namespace TaskManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<Models.Task>>> GetTasks() =>
        Ok(await _service.GetAllTasks());

    [HttpGet("{id}")]
    public async Task<ActionResult<Models.Task>> GetTask(int id)
    {
        var task = await _service.GetTask(id);
        return task != null ? Ok(task) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Models.Task>> CreateTask(Models.Task task)
    {
        var createdTask = await _service.CreateTask(task);
        return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, Models.Task task)
    {
        var updatedTask = await _service.UpdateTask(id, task);
        return updatedTask != null ? Ok(updatedTask) : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        await _service.DeleteTask(id);
        return NoContent();
    }
}