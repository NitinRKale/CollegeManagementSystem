using CollegeWebApplication.Data;
using CollegeWebApplication.IRepository;
using CollegeWebApplication.Models;
using CollegeWebApplication.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Ensure log folder exists (must match path in appsettings.json)
Directory.CreateDirectory(@"E:\ApplicationLogs");

// Configure Serilog from configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews()
   .AddJsonOptions(options =>
   {
       // A property naming policy, or null to leave property names unchanged.
       options.JsonSerializerOptions.PropertyNamingPolicy = null;
   });


// Register the DbContext with SQL Server using the connection string from appsettings.json
builder.Services.AddDbContext<CollegeWebDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CollegeConnection")));

// Register Identity with ApplicationUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configure identity options here if needed
    options.Password.RequiredLength = 5; // Example: Set minimum password length to 5 characters
    options.Password.RequireNonAlphanumeric = false; // Example: Disable requirement for non-alphanumeric characters
})
  .AddEntityFrameworkStores<CollegeWebDbContext>()
  .AddDefaultTokenProviders();


// Configure authentication cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            // Handle redirect to login page when user is not authenticated
            context.Response.Redirect("/Account/Login");
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = context =>
        {
            // Handle redirect to access denied page when user does not have permission
            context.Response.Redirect("/Home/UnAuthorized");
            return Task.CompletedTask;
        }
    };
});

// Register your repository for DI

////builder.Services.AddScoped<DepartmentRepository>();
builder.Services.AddScoped<IStudentMasterEFRepository, StudentMasterEFRepository>();
builder.Services.AddScoped<ICityMasterADORepository, CityMasterADORepository>();
builder.Services.AddScoped<ICityMasterADOSPRepository, CityMasterADOSPRepository>();

var app = builder.Build();

//seed Roles - Insert Roles into Role table
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roleNames = { "SuperAdmin", "Admin", "SuperUser", "User" }; // Define your roles here
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
