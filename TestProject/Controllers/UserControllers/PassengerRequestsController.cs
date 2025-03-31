using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Extentions;
using TestProject.Models;

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

        // View all pending requests made by the logged-in user
        public IActionResult MyRequests()
        {
            var userId = User.Id(); // Get logged-in user ID

            //var requests = _context.Requests
            //                .Where(r => r.UserId == userId && r.StatusRequest == RequestStatus.Pending)
            //                .Include(r => r.Trip)
            //                .ToList();

            var trips = _context.Trips
                .Where(t => t.Requests!.Any(r => r.StatusRequest == RequestStatus.Pending && r.UserId == userId)) // Filter trips where THIS user has a pending request
                .Include(t => t.Requests)
                    .ThenInclude(r => r.User)
                .ToList();

            // Now filter requests in memory (keep only those from the current user)
            foreach (var trip in trips)
            {
                trip.Requests = trip.Requests
                    .Where(r => r.StatusRequest == RequestStatus.Pending && r.UserId == userId)
                    .ToList();
            }


            var driverRequest = _context.RequestDrivers
                                .Where(rd => rd.UserId == userId && rd.StatusRequest == RequestStatus.Pending)
                                .Include(rd => rd.User)
                                .ToList();

            ViewBag.IsDriver = driverRequest;
            return View(trips);
        }

        // Delete a request
        public IActionResult DeleteRequest(int id, string? returnUrl)
        {
            var request = _context.Requests.Find(id);

            if (request == null || request.UserId != User.Id())
            {
                return NotFound();
            }

            _context.Requests.Remove(request);
            _context.SaveChanges();

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl); // Redirects back to the previous page
            }

            return RedirectToAction("MyRequests");
        }
    }
}
