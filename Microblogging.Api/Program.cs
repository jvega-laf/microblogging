using Microblogging.Api.Endpoints;
using Microblogging.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Agregar capas de infraestructura y aplicación
builder.Services.AddApplication();     // → lo definiremos en Application
builder.Services.AddInfrastructure(builder.Configuration); // → lo definiremos en Infrastructure

var app = builder.Build();

// Middleware
// if (app.Environment.IsDevelopment())
// {
app.UseSwagger();
app.UseSwaggerUI();
// }

app.MapTweetEndpoints();

app.MapFollowEndpoints();

app.UseHttpsRedirection();

// Aquí se registrarán los endpoints de la API
app.MapGet("/", () => "Microblogging API 🚀");

app.Run();

public partial class Program { } // ← ¡Necesario para tests!