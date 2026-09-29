using FluentValidation;
using TestTask.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IValidator<RequestModel>, RequestValidator>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddScoped<TestService>(_ => new TestService(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "api/swagger";
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TestTask.API v1");
    });
}

app.MapControllers();
app.Run();
