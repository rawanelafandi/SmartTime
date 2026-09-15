using Microsoft.EntityFrameworkCore;
using SmartTime.Factory.Factories;
using SmartTime.Factory.Interfaces;
using SmartTime.Factory.Strategies;
using SmartTime.Repository.Data;
using SmartTime.Repository.Interfaces;
using SmartTime.Repository.Repositories;
using SmartTime.Services.Configuration;
using SmartTime.Services.Interfaces;
using SmartTime.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Database (code-first: DbContext + migrations create the schema) ---
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<SmartTimeDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// --- Repository / Unit of Work ---
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// --- Clockify integration ---
builder.Services.Configure<ClockifySettings>(builder.Configuration.GetSection(ClockifySettings.SectionName));
builder.Services.AddHttpClient<IClockifyService, ClockifyService>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.Brotli
    });

// --- Application services ---
builder.Services.AddScoped<ITimeTrackingService, TimeTrackingService>();
builder.Services.AddScoped<ICsvExportService, CsvExportService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITimeEntryService, TimeEntryService>();
// --- Export strategy + factory (bonus: strategy & factory patterns) ---
builder.Services.AddScoped<IExportStrategy, CsvExportStrategy>();
builder.Services.AddScoped<IExportStrategyFactory, ExportStrategyFactory>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();