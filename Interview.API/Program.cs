using Infrastructure;
using Interview.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Personal organisational preference / habit to keep each layer responsible for its own DI registration -
// Helps keep the program file clean and readable.
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (builder.Configuration.GetValue("HttpsRedirection:Enabled", true))
{
    app.UseHttpsRedirection();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

// Made this async because if I replaced the in-memory database with a real database - this would be an async operation.
// Doesn't make much difference in its current state, but just wanted to demonstrate the understanding.
await app.Services.InitialiseInfrastructureAsync();

app.Run();
