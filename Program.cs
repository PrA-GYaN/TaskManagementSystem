using Serilog;
using Hangfire;
using Hangfire.PostgreSql;
using TaskManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Repositories;
using TaskManagementSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Hangfire with PostgreSQL
builder.Services.AddHangfire(config =>
{
    var options = new PostgreSqlStorageOptions
    {
        SchemaName = "hangfire",
        PrepareSchemaIfNecessary = true
    };
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"), options);
});
builder.Services.AddHangfireServer();

// Configure Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext());

// Register services
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<BackgroundJobService>();

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();
app.UseAuthorization();
app.MapControllers();

app.Run();