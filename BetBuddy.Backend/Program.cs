using BetBuddy.Backend.Api.Ai;
using BetBuddy.Backend.Api.Data;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<Kernel>(new KernelBuilder().BuildOllamaKernel());
builder.Services.AddSingleton<IBetBuddyAgent, BetBuddyAgent>();
builder.Services.AddScoped<FixtureRepository>();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();