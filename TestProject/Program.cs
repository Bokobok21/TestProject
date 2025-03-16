using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Data.SeedDb;
using TestProject.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;


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


// when changing the user's role, check if the user is a driver and if he is delete all his trips
// tripParticipant should be able to be modified by the admin 
// not sure but delete the user's request when he becomes a driver 
// not sure but the drivers images aren't being deleted when edited

// admin
// ???? // redovni marshruti i ednokratni marshruti

// driver
// the driver should accept you for the trip not the admin

// passenger
// make a method for the passenger to see all his requests and to be able to delete them ??????
// should be able to search for trips 
// he shoudn't be able to join a trip at the same time of another one 

// make a method for the driver to see all his trips and to be able to delete them
// make a method for the driver to see all the requests for his trips and to be able to accept them

// keep order of request when free seats are changed to delete the users that have entered last and only keep the first ones
// a/an driver/user can't be on two trips at the same time
// fix registering with the same email and changing password
// fix sorting of the trips
// for each user display which trips he has entered


// fix foulders and files
// the role should be passenger not tourist


// AIs and other useful info
// v0 ; codepen ; cursor ai ; saveWeb2Zip + b12 website builder ai ; 


// prompts

