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
builder.Services.AddHostedService<TripStatusUpdater>();


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

// add a button to demote yourself from driver to tourist 

// admin panel - listallusers and driverrequests (should i have otherr admin things for things that normally no but for the presentation)


//fix the back to list fucntion - javascript:history.back()





// trips pictures aren't deleted when it is changed in edit form
// fix foulders and files
// the role should be passenger not tourist

// AIs and other useful info
// v0 ; codepen ; cursor ai ; saveWeb2Zip + b12 website builder ai ; 


// prompts
// v0 - about the time - make it - **Visual Calendar**: Create a calendar view that shows all of a user's trips, making it easier to visualize their schedule.
// disable the times that are already taken by the user (look for example of the return time can't be before the departure)
// do not sumbit the form if there are errors - before i couldn't? now i can (if the time is before the current then i can't sumbit it so look how its done there ) 
// give the option to change times on booked as well not only upcoming (ez)
// i can't join any trips for that matter (should be cap)
// maybe if i add a calendar it would be better (some chance easier)


//fix the calendar
//and do the same design to the driver requests
// leave an overall review and the review page needs fix 

// if possible fix the calendar to be red when unpiccable times also to not sumbit the form if there is error
// and the times to be not to the top of the screen and reserved to be able to edit as well

// promt - for the travel dashboard.make it in bulgarian language.remove the create trip and browse all cards.make only two cards per row because there are now only 4 in total and if the user is not a driver / admin there would only be two so make that be displayed better when there are only two.


// documentation
// 2.5 2.5 2.5 3 for the left one 30 rows 60 characters per row max 2000 characters
// format painter (double cliker for not exiting until clicking escape)


// - code for the overlapping ->


//1st method
//// Get user's existing trips
//var userTrips = await _context.Trips
//    .Where(t => t.DriversId == userId &&
//           (t.StatusTrip == TripStatus.Upcoming || t.StatusTrip == TripStatus.Ongoing))
//    .Select(t => new {
//        Start = t.DepartureTime,
//        End = t.ReturnTime,
//        Id = t.Id // Include ID to exclude current trip in edit mode
//    })
//    .ToListAsync();

//// Pass to view
//ViewBag.UserTrips = JsonSerializer.Serialize(userTrips);




////2nd methodpublic class TripViewModel
//{
//    // Existing properties...

//    public List<OverlappingTrip> UserTrips { get; set; }
//}

//public class OverlappingTrip
//{
//    public DateTime Start { get; set; }
//    public DateTime End { get; set; }
//    public int Id { get; set; }
//}



// js for create/edit ---------------------------------------

//// Add this to both Create.cshtml and Edit.cshtml scripts

//// Parse user trips from ViewBag
//const userTrips = @Html.Raw(ViewBag.UserTrips ?? "[]");
//const currentTripId = @(Model.Id != 0 ? Model.Id : 0); // For edit page

//// Function to check if a datetime is within any existing trip
//function isOverlappingWithExistingTrip(newStart, newEnd)
//{
//    for (const trip of userTrips) {
//        // Skip current trip in edit mode
//        if (trip.id === currentTripId) continue;

//        const tripStart = new Date(trip.start);
//        const tripEnd = new Date(trip.end);

//        // Check if there's an overlap
//        if ((newStart >= tripStart && newStart <= tripEnd) ||
//            (newEnd >= tripStart && newEnd <= tripEnd) ||
//            (newStart <= tripStart && newEnd >= tripEnd))
//        {
//            return {
//            overlaps: true,
//                tripStart,
//                tripEnd
//            }
//            ;
//        }
//    }

//    return { overlaps: false }
//    ;
//}

//// Modify the validateDepartureTime function
//function validateDepartureTime()
//{
//    if (!isUpcoming) return true; // Skip validation for non-upcoming trips

//    const departureTime = new Date(departureTimeInput.value);
//    const now = new Date();

//    // Check if in the past
//    if (departureTime < now)
//    {
//        departureTimeInput.classList.add('invalid');
//        departureTimeError.textContent = '����� �� ���������� �� ���� �� ���� � ��������';
//        departureTimeError.classList.add('visible');
//        return false;
//    }

//    // Check for return time if it exists
//    if (returnTimeInput.value)
//    {
//        const returnTime = new Date(returnTimeInput.value);
//        const overlap = isOverlappingWithExistingTrip(departureTime, returnTime);

//        if (overlap.overlaps)
//        {
//            departureTimeInput.classList.add('invalid');
//            departureTimeError.textContent = `����� ����� �������� ����� ${ overlap.tripStart.toLocaleString('bg-BG')}
//            � ${ overlap.tripEnd.toLocaleString('bg-BG')}`;
//            departureTimeError.classList.add('visible');
//            return false;
//        }
//    }

//    departureTimeInput.classList.remove('invalid');
//    departureTimeError.classList.remove('visible');
//    return true;
//}

//// Modify the validateReturnTime function
//function validateReturnTime()
//{
//    if (!departureTimeInput.value)
//    {
//        returnTimeInput.classList.add('invalid');
//        returnTimeError.textContent = '����� �������� ��� �� ����������';
//        returnTimeError.classList.add('visible');
//        return false;
//    }

//    const departureTime = new Date(departureTimeInput.value);
//    const returnTime = new Date(returnTimeInput.value);

//    // Check if return is before departure
//    if (returnTime <= departureTime)
//    {
//        returnTimeInput.classList.add('invalid');
//        returnTimeError.textContent = '����� �� ������� ������ �� ���� ���� ���� �� ����������';
//        returnTimeError.classList.add('visible');
//        return false;
//    }

//    // Check for overlapping trips
//    const overlap = isOverlappingWithExistingTrip(departureTime, returnTime);
//    if (overlap.overlaps)
//    {
//        returnTimeInput.classList.add('invalid');
//        returnTimeError.textContent = `����� ����� �������� ����� ${ overlap.tripStart.toLocaleString('bg-BG')}
//        � ${ overlap.tripEnd.toLocaleString('bg-BG')}`;
//        returnTimeError.classList.add('visible');
//        return false;
//    }

//    returnTimeInput.classList.remove('invalid');
//    returnTimeError.classList.remove('visible');
//    return true;
//}












