using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Data.SeedDb;
using TestProject.Models;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();



// not sure of this is needed for adminpolicy auhorization
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//});



var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await SeedRoles.Initialize(serviceProvider);
}

//// @if(User.IsInRole("Admin")) {...} for view pages



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


// comments for what i have to work on

// make an registration for the application user
// make a method for deleting user and when a user is deleted delete all his requests and trip participants
// make a method for the admin to see all users with their roles and to be able to change them
// keep order of request when free seats are changed to delete the users that have entered last and only keep the first ones
// a/an driver/user can't be on two trips at the same time
// ask about the tripPariticipant table why they are null properties



//---------------- this is super messy but this is my old comments----------------
//i have four tabbles/entitie/models iin asp net core i have applicationuser and trip and review and request and i have admin/moderator/tourist roles. only drivers can make a trip and when you first register you become tourist but you can apply if you want to be a driver and there youe enter the info for the cars you have and their plate numbers. so this probably mmeans i need to make another table cars. the drivers has the option to add more cars later if he wants. 
// kak shte se integrira service/Iservice, partiaView, annotations in asp net core
// izpolzvai DisplayFor and EditFor
// v0 i codepen
// cursor ai,
// fix model connection - viewmodels - views(forumpage) - controllers - services - repository 