

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddApiDependencies(builder.Configuration);



var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();



app.Run();