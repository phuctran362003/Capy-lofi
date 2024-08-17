using API.Dependencies;
using Microsoft.OpenApi.Models;
using Service.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddProjectServices(builder.Configuration); // Using the DI class for dependency injection

// Add services for SignalR
builder.Services.AddSignalR();

// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CapyLofi API", Version = "v1" });
});

//Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CapyLofi API v1");
        c.InjectJavascript("/custom-swagger.js"); // Ensure this path is correct
        c.InjectStylesheet("/custom-swagger.css"); // Ensure this path is correct
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Ensure this is placed correctly
app.UseRouting();
app.MapHub<ChatHub>("/chat-hub");

// Use CORS
app.UseCors("AllowAll");

app.UseAuthentication(); // Ensure this comes before UseAuthorization
app.UseAuthorization();
app.MapControllers();

app.Run();