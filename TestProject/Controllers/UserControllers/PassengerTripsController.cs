using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Extentions;

namespace TestProject.Controllers.UserControllers
{
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
                         .Select(tp => tp.Trip)
                         .ToList();

            return View(trips);
        }
    }
}
