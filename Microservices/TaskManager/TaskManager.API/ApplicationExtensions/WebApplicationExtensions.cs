using System.Reflection;
using DotNetEnv;
using Infrustructure.Masstransit;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Persistance.Masstransit;
using Quartz;
using Serilog;
using TaskManager.API.Jobs;
using TaskManager.API.Masstransit.Consumers;
using TaskManager.BLL.Services;
using TaskManager.DAL.DataContext;
using TaskManager.Domain.Interfaces.Services;

namespace TaskManager.API.ApplicationExtensions;

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
                    .Enrich.FromLogContext()
                    .WriteTo.Console(
                         outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();
        

        builder.Host.UseSerilog();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(opt =>
        {
            opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
            
            opt.SwaggerDoc(
                name: "v1",
                info: new OpenApiInfo()
                {
                    Title   = "TaskManager.API",
                    Version = "v1",
                });
        });

        return builder;
    }
    
    public static WebApplicationBuilder AddDataContext(this WebApplicationBuilder builder)
    {
        var connectionString = Env.GetString("CONNECTION_STRING");
        
        builder.Services.AddDbContext<TaskManagerContext>(opt => opt.UseNpgsql(connectionString));
        
        return builder;
    }

    public static WebApplicationBuilder AddDomainServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITaskManagerService, TaskManagerService>();
        builder.Services.AddScoped<ICleanupService, CleanupService>();
        return builder;
    }

    public static WebApplicationBuilder AddMassTransit(this WebApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddConsumer<AddTaskConsumer>();
            x.AddConsumer<ChangeStatusConsumer>();
            x.AddConsumer<GetTasksConsumer>();
            x.AddConsumer<DeleteTaskConsumer>();
            x.AddConsumer<UpdateTaskInfoConsumer>();
            
            x.UsingRabbitMq((ctx, conf) =>
            {
                var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? throw new Exception("RABBITMQ_HOST environment variable is not set");
                
                conf.Host(rabbitMqHost, hostConf =>
                {
                    hostConf.Username(Env.GetString("RABBITMQ_USERNAME") ?? throw new Exception("RABBITMQ_USER environment variable is not set"));
                    hostConf.Password(Env.GetString("RABBITMQ_PASSWORD") ?? throw new Exception("RABBITMQ_PASSWORD environment variable is not set"));
                });
                
                conf.ReceiveEndpoint(
                    queueName: RabbitQueueNames.TASK_MANAGER_QUEUE,
                    configureEndpoint: endpoint =>
                    {
                        endpoint.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(3)));
                        endpoint.Consumer<AddTaskConsumer>(ctx);
                        endpoint.Consumer<ChangeStatusConsumer>(ctx);
                        endpoint.Consumer<GetTasksConsumer>(ctx);
                        endpoint.Consumer<DeleteTaskConsumer>(ctx);
                        endpoint.Consumer<UpdateTaskInfoConsumer>(ctx);
                    });

                conf.ConfigureEndpoints(ctx);
            });
        });
        
        return builder;
    }

    public static WebApplicationBuilder AddQuartzWithJobs(this WebApplicationBuilder builder)
    {
        builder.Services.AddQuartz(q =>
        {
            var jobkey = new JobKey("CleanupDeletedTasksJob");

            q.AddJob<CleanupDeletedTasksJob>(opts => opts.WithIdentity(jobkey));

            q.AddTrigger(opts => opts.ForJob(jobkey)
                                     .WithIdentity("CleanupDeletedTasksTrigger")
                                     .WithSimpleSchedule(s => s.WithInterval(TimeSpan.FromSeconds(10))
                                                               .RepeatForever()));
        });
        
        builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        return builder;
    }
}