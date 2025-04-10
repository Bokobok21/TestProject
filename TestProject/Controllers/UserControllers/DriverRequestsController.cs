using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Extentions;
using TestProject.Models;

namespace TestProject.Controllers.UserControllers
{
    [Authorize]
    public class DriverRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DriverRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> PendingRequests()
        {
            // Fetch all pending requests along with related trip and user details
            var pendingRequests = await _context.Requests
                .Where(r => r.StatusRequest == RequestStatus.Pending)
                .Include(r => r.Trip)
                .Include(r => r.User)
                .Where(r => r.Trip.DriversId == User.Id())
                .ToListAsync();

            return View(pendingRequests);
        }
    }
}
