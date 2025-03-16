using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Models.Task> Add(Models.Task task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<Models.Task?> GetById(int id) =>
        await _context.Tasks.FindAsync(id);

    public async Task<List<Models.Task>> GetAll() =>
        await _context.Tasks.ToListAsync();

    public async Task<Models.Task> Update(Models.Task task)
    {
        _context.Entry(task).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> Delete(Models.Task task)
    {
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }
}