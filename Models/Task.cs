namespace TaskManagementSystem.Models;
using System.Text.Json.Serialization;

[JsonConverter(typeof(JsonStringEnumConverter))]
//THis is used to return String Status Instead of Indexed Integer Array
public enum Status
{
    Pending,
    InProgress,
    Completed,
    Failed
}

public class Task
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ExecutionDateTime { get; set; }
    public Status Status { get; set; } = Status.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}