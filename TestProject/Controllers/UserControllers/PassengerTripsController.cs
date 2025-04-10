using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Extentions;
using TestProject.Models;

namespace TestProject.Controllers.UserControllers
{
    [Authorize]
    public class PassengerTripsController : Controller
    {
        private readonly ApplicationDbContext _context; // Add this line to define the _context field

        public PassengerTripsController(ApplicationDbContext context) // Add this constructor to initialize the _context field
        {
            _context = context;
        }

        public IActionResult MyTrips()
        {
            var userId = User.Id();
            var trips = _context.TripParticipants
                         .Where(tp => tp.UserId == userId)
                         .Include(tp => tp.Trip)
                            .ThenInclude(tp => tp.Driver)
                         .Select(tp => tp.Trip)
                         .ToList();

            // IOrderedEnumerable<Trip> orderedTrips = trips.OrderBy(t => t.StatusTrip != TripStatus.Upcoming);
            IOrderedEnumerable<Trip> orderedTrips = trips.OrderBy(t => t.StatusTrip != TripStatus.Upcoming);
            orderedTrips = orderedTrips.ThenBy(t => t.StatusTrip);
            orderedTrips = orderedTrips.ThenBy(t => t.DepartureTime);


            return View(orderedTrips);
        }
    }
}
