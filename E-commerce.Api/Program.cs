using E_commerce.Core;
using E_commerce.Core.Interfaces;
using E_commerce.Core.Mapping;
using E_commerce.Core.Services;
using E_commerce.Infrastructure;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();


builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructureDependencies(builder.Configuration);
builder.Services.AddCoreDependencies();

//builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(typeof(CategoryMapping).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
