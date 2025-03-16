using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Extentions;

namespace TestProject.Controllers.UserControllers
{
    [Authorize]
    public class PassengerRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PassengerRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // View all requests made by the logged-in user
        public IActionResult MyRequests()
        {
            var userId = User.Id(); // Get logged-in user ID
            var requests = _context.Requests
                            .Where(r => r.UserId == userId)
                            .Include(r => r.Trip)
                            .ToList();

            return View(requests);
        }

        // Delete a request
        public IActionResult DeleteRequest(int id)
        {
            var request = _context.Requests.Find(id);

            if (request == null || request.UserId != User.Id())
            {
                return NotFound();
            }

            _context.Requests.Remove(request);
            _context.SaveChanges();

            return RedirectToAction("MyRequests");
        }
    }
}
