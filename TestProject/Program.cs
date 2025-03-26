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
builder.Services.AddHostedService<TripStatusUpdater>();

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

// admin panel - listallusers and driverrequests (should i have otherr admin things for things that normally no but for the presentation)
// user panel - my request - driver and passenger perspective and my trips - driver and passenger perspectives

// he shoudn't be able to join a trip at the same time of another one 
// a/an driver/user can't be on two trips at the same time

//if statements for the joining in trip back to list meaingng(also if it joins resend him to the same page so there is no issue with the edit after?)
// meaning of the top line is to fix the back to list fucntion

// in the list all users admind panel when i tourist just got changed to driver via a request in the panel it isn't reflected and it still shows tourist even though he is a driver now???

// password iziskvaniq are still in english?
// admin should accept trips???

// fix foulders and files
// trips pictures aren't deleted when it is changed in edit form
// the role should be passenger not tourist


// AIs and other useful info
// v0 ; codepen ; cursor ai ; saveWeb2Zip + b12 website builder ai ; 


// prompts

