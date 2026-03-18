using E_commerce.Api.Abstraction;
using E_commerce.Core;
using E_commerce.Core.Interfaces;
using E_commerce.Core.Mapping;
using E_commerce.Core.Services;
using E_commerce.Infrastructure;
using Microsoft.Extensions.FileProviders;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();



builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IFileProvider>(
               new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
               );
builder.Services.AddSingleton<IImageMangementService, ImageMangementService>();

builder.Services.AddInfrastructureDependencies(builder.Configuration);
builder.Services.AddCoreDependencies();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

//builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(typeof(MappingConfiguration).Assembly);

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

app.UseExceptionHandler();

app.Run();
