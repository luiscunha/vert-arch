using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Sacurt.VertArch.Api.Database;
using Sacurt.VertArch.Api.Extensions; 

var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

var assembly = typeof(Program).Assembly;

builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
});

builder.Services.AddCarter();
builder.Services.AddValidatorsFromAssembly(assembly);
 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.ApplyMigrations();
}

app.MapCarter();

app.UseHttpsRedirection();

app.Run();
