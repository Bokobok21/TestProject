using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Data.SeedDb;
using TestProject.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting.Internal;
using TestProject.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddHostedService<TripSchedulerService>();

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


// in the partial view ( possibly make it only for one trip and not the whole list)

// trip about background service is reccuring should stay vissible or not 
// style display none should be fixed


// when changing the user's role, check if the user is a driver and if he is delete all his trips/put them in hold
// tripParticipant should be able to be modified by the admin 

// admin should accept trips?

//when tripparticipant is denied delete the request so he can request again..

// fix editing of status in trips because it resets or doesn't change the status

// he shoudn't be able to join a trip at the same time of another one 
// a/an driver/user can't be on two trips at the same time

// keep order of request when free seats are changed to delete the users that have entered last and only keep the first ones

// fix registering with the same email and changing password


// fix foulders and files
// the role should be passenger not tourist


// AIs and other useful info
// v0 ; codepen ; cursor ai ; saveWeb2Zip + b12 website builder ai ; 


// prompts

