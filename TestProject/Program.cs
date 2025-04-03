using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Data.SeedDb;
using TestProject.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using TestProject.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddHostedService<TripSchedulerService>();
builder.Services.AddHostedService<TripStatusUpdaterService>();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await SeedRoles.Initialize(serviceProvider);
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
//app.MapControllers();

app.Run();

//back to list fucntion - javascript:history.back()

// not return time but departing time 


// view all drivers
// reserve multiple seats


// trips pictures aren't deleted when it is changed in edit form
// add icons from mom


// fix foulders and files
// fix the project name
// fix the role to be passenger not tourist

// AIs and other useful info
// v0 ; napkin ai ; wireframe ; sonnet-claude ; heruUi ai ; NotebookLM ; ai agents (gemini) ; figma ; eraser ; paper ; codepen ; cursor ai ; bolt(.new) ai ; saveWeb2Zip ; b12 website builder ai ; 
// Localend ; ShareX ; TranslucentTB ; Revo Uninstaller ; 

// documentation
// 2.5 2.5 2.5 3 for the left one 30 rows 60 characters per row min 1800 max 2000 characters
// format painter (double cliker for not exiting until clicking escape)

