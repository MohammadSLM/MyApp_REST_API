using DataAccess;
using DataAccess.Repositories;
using DataAccess.Repositories.UserRepositories;
using ElmahCore.Mvc;
using ElmahCore.Sql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Web;
using Services.JwtServices;
using WebFramework.Middlewares;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
//var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args); ;

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    });
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddElmah<SqlErrorLog>(options =>
    {
    //must be seperated from main projects db
    options.ConnectionString = builder.Configuration.GetConnectionString("Elmah");
    });

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCustomExceptionHandler();

    app.UseElmah();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}
