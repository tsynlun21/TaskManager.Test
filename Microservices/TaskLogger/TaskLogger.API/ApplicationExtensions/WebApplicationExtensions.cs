using System.Reflection;
using DotNetEnv;
using Infrustructure.Masstransit;
using MassTransit;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using TaskLogger.API.Consumers;
using TaskLogger.BLL.Services;
using TaskLogger.DAL.MongoDB;
using TaskLogger.DAL.MongoDB.Repository;
using TaskLogger.Domain.Services;

namespace TaskLogger.API.ApplicationExtensions;

public static class WebApplicationExtensions
{
    public static void AddDotNetEnv()
    {
        Env.Load();
    }

    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .WriteTo.File("logs/tasklogs.txt", rollingInterval: RollingInterval.Month,
                         restrictedToMinimumLevel: LogEventLevel.Error).CreateLogger();

        builder.Host.UseSerilog();

        return builder;
    }

    public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(opt =>
        {
            //opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            
            opt.SwaggerDoc(
                name: "v1",
                info: new OpenApiInfo()
                {
                    Title   = "TaskLogger.API",
                    Version = "v1",
                });
        });

        return builder;
    }

    public static WebApplicationBuilder ConfigureMongoDbSettings(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<MongoDbSettings>(opt =>
        {
            opt.ConnectionString = Env.GetString("MONGO_CONNECTION_STRING") ??
                                   throw new Exception("MongoDb connection string is missing");
            opt.DatabaseName = Env.GetString("MONGO_DB_NAME") ??
                               throw new Exception("MongoDb database name is missing");
            opt.CollectionName = Env.GetString("MONGO_DB_COLLECTION") ??
                                 throw new Exception("MongoDb collection name is missing");
        });

        return builder;
    }

    public static WebApplicationBuilder AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<TaskLogRepository>();
        return builder;
    }

    public static WebApplicationBuilder AddDomainServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ITaskLoggerService, TaskLoggerService>();
        return builder;
    }

    public static WebApplicationBuilder AddMassTransit(this WebApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddConsumer<LogActionConsumer>();
            
            x.UsingRabbitMq((ctx, conf) =>
            {
                var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? throw new Exception("RABBITMQ_HOST environment variable is not set");
                
                conf.Host(rabbitMqHost, hostConf =>
                {
                    hostConf.Username(Env.GetString("RABBITMQ_USERNAME") ?? throw new Exception("RABBITMQ_USER environment variable is not set"));
                    hostConf.Password(Env.GetString("RABBITMQ_PASSWORD") ?? throw new Exception("RABBITMQ_PASSWORD environment variable is not set"));
                });
                
                conf.ReceiveEndpoint(
                    queueName: RabbitQueueNames.TASK_LOGGER_QUEUE,
                    configureEndpoint: endpoint =>
                    {
                        endpoint.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(3)));
                        endpoint.Consumer<LogActionConsumer>(ctx);
                    });

                conf.ConfigureEndpoints(ctx);
            });
        });
        
        return builder;
    }
    
}