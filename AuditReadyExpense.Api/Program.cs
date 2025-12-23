using AuditReadyExpense.Application.Contracts;
using AuditReadyExpense.Application.UseCases;
using AuditReadyExpense.Infrastructure.Persistence;
using AuditReadyExpense.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext (Postgres)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// DI: Repositories + Use Cases
builder.Services.AddScoped<IExpenseRepository, EfExpenseRepository>();
builder.Services.AddScoped<ExpenseWorkflowService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Für den Anfang kannst du HTTPS-Redirect drin lassen oder auskommentieren.
// In Docker/bei lokalen HTTP-Tests nervt es manchmal.
// app.UseHttpsRedirection();

app.MapControllers();

app.Run();