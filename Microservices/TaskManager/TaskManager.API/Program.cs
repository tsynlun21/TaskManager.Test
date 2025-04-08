using Serilog;
using TaskManager.API.ApplicationExtensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

WebApplicationExtensions.AddDotNetEnv();

builder.AddSwagger()
       .AddDataContext()
       .AddDomainServices()
       .AddMassTransit()
       .AddSerilog()
       .AddQuartzWithJobs();


var app = builder.Build();

app.UseSerilogRequestLogging(o =>
{
       o.MessageTemplate =
              "{RequestMethod} {RequestPath} with Query {RequestQuery} responded {StatusCode} in {Elapsed:0.0000} ms";
       o.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
       {
              diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
              diagnosticContext.Set("RequestPath", httpContext.Request.Path);
              diagnosticContext.Set("RequestQuery", httpContext.Request.QueryString.Value ?? "empty");
              diagnosticContext.Set("StatusCode", httpContext.Response.StatusCode);
       };
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Urls.Add("http://*:5001");
app.Run();