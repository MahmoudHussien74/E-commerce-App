var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiDependencies(builder.Configuration);

var app = builder.Build();

// Swagger is enabled in all environments to support deployment testing & API consumers.
// To restrict to Development only, wrap these two calls inside: if (app.Environment.IsDevelopment())
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce API v1");
    options.RoutePrefix = "swagger";          // accessible at /swagger
    options.DocumentTitle = "E-Commerce API";
});

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});



app.Run();
