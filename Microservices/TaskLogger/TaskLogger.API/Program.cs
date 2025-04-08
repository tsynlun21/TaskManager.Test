using DotNetEnv;
using TaskLogger.API.ApplicationExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

WebApplicationExtensions.AddDotNetEnv();

builder.AddSwagger()
       .ConfigureMongoDbSettings()
       .AddRepositories()
       .AddDomainServices()
       .AddMassTransit()
       .AddSerilog();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseAuthorization();

app.MapControllers();

app.Urls.Add("http://*:5002");
app.Run();