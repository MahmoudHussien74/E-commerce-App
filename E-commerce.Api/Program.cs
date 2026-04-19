using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiDependencies(builder.Configuration);


builder.Host.UseSerilog((context, configuration) =>
         configuration.ReadFrom.Configuration(context.Configuration));


var app = builder.Build();

app.UseSerilogRequestLogging();

await E_commerce.Infrastructure.Data.IdentityDataSeeder.SeedAsync(app.Services);

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce API v1");
    options.RoutePrefix = "swagger";         
    options.DocumentTitle = "E-Commerce API";
});

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()); // Adjust for production!
app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();


app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
