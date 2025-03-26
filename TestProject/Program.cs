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

// make the trip clicking for details with the a tag to the title and the button and picture

// user panel - my request - driver and passenger perspective and my trips - driver and passenger perspectives


// in the list all users admind panel when i tourist just got changed to driver via a request in the panel it isn't reflected and it still shows tourist even though he is a driver now 


// he shoudn't be able to join a trip at the same time of another one 
// a/an driver/user can't be on two trips at the same time

// fix editing of status in trips because it resets or doesn't change the status 

// keep order of request when free seats are changed to delete the users that have entered last and only keep the first ones

// password iziskvaniq are still in english

//if statements for the joining in trip back to list meaingng(also if it joins resend him to the same page so there is no issue with the edit after?)
// when joining a tirp you then you can't edit it 


// admin should accept trips???



// fix foulders and files
// the role should be passenger not tourist


// AIs and other useful info
// v0 ; codepen ; cursor ai ; saveWeb2Zip + b12 website builder ai ; 


// prompts

//trips edit page 
// free seats aren't updated 
// reserved status isn't blue but it is green rn
// add the validations for create/edit trip