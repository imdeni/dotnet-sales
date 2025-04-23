using Microsoft.EntityFrameworkCore;
using TechnicalTest.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin() // Allows all origins to make requests
               .AllowAnyMethod() // Allows all HTTP methods (GET, POST, PUT, DELETE, etc.)
               .AllowAnyHeader(); // Allows all headers in the request
    });
});

// Add DbContext with SQL Server connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();  // Enforces secure connections
}

app.UseHttpsRedirection();  // Redirect HTTP to HTTPS
app.UseRouting();  // Enable routing

// Use CORS policy
app.UseCors("AllowAllOrigins"); // This applies the CORS policy globally

app.UseAuthorization();  // Enable authorization middleware

// Default route configuration (use SoOrder controller as default)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=SoOrder}/{action=Index}/{id?}");

app.Run();
