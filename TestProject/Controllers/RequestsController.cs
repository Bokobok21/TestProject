using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Extentions;
using TestProject.Models;
using TestProject.Models.ViewModels;

namespace TestProject.Controllers
{
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendJoinRequest(int tripId, string? returnUrl)
        {
            //var userId = _userManager.GetUserId(User); // Get the current user's ID
            var userId = User.Id();
            var trip = await _context.Trips.FindAsync(tripId);

            if (trip == null || trip.FreeSeats <= 0)
            {
                return NotFound(); // No free seats or trip doesn't exist
            }

            // Check if the user has already sent a request for this trip
            var existingRequest = await _context.Requests
                .FirstOrDefaultAsync(r => r.TripId == tripId && r.UserId == userId);

            if (existingRequest != null)
            {
                ModelState.AddModelError(string.Empty, "You have already sent a request for this trip.");

                var tripViewModel = new TripViewModel
                {
                    Id = trip.Id,
                    DriversId = trip.DriversId,
                    DriverName = trip.Driver?.UserName,
                    StartPosition = trip.StartPosition,
                    Destination = trip.Destination,
                    DepartureTime = trip.DepartureTime,
                    ReturnTime = trip.ReturnTime,
                    Price = trip.Price,
                    TotalSeats = trip.TotalSeats,
                    FreeSeats = trip.FreeSeats,
                    CarModel = trip.CarModel,
                    PlateNumber = trip.PlateNumber,
                    ImagePath = trip.ImagePath,
                    StatusTrip = trip.StatusTrip,
                };

                ViewBag.ReturnUrl = returnUrl;
                return View("~/Views/Trips/Details.cshtml", tripViewModel);
            }

            // Create a new request
            var request = new Request
            {
                TripId = tripId,
                UserId = userId,
                StatusRequest = RequestStatus.Pending,
                Date = DateTime.Now
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            ViewBag.ReturnUrl = returnUrl;
            return RedirectToAction("Details", "Trips", new { id = tripId, returnUrl = returnUrl });
        }

        public async Task<IActionResult> ApproveRequest(int requestId)
        {
            var request = await _context.Requests.FindAsync(requestId);
            if (request == null || request.StatusRequest != RequestStatus.Pending)
            {
                return NotFound();
            }

            var trip = await _context.Trips.FindAsync(request.TripId);
            if (trip == null || trip.FreeSeats <= 0)
            {
                return NotFound(); // No free seats or trip doesn't exist
            }

            // Add user to TripParticipants
            var tripParticipant = new TripParticipant
            {
                TripId = request.TripId,
                UserId = request.UserId
            };
            _context.TripParticipants.Add(tripParticipant);

            // Decrement FreeSeats
            trip.FreeSeats -= 1;

            // Update trip status if FreeSeats becomes 0
            if (trip.FreeSeats == 0)
            {
                trip.StatusTrip = TripStatus.Booked;
            }

            // Update request status
            request.StatusRequest = RequestStatus.Accepted;

            await _context.SaveChangesAsync();
            return RedirectToAction("PendingRequests", "DriverRequests");
        }

        public async Task<IActionResult> DenyRequest(int requestId)
        {
            var request = await _context.Requests.Include(r => r.User).FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null || request.StatusRequest != RequestStatus.Pending)
            {
                return NotFound();
            }

            var requests = await _context.Requests.Where(r => r.UserId == request.User.Id).ToListAsync();
            _context.Requests.RemoveRange(requests);

            await _context.SaveChangesAsync();

            return RedirectToAction("PendingRequests", "DriverRequests");
        }


        // GET: Requests
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Requests.Include(r => r.Trip).Include(r => r.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Requests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests
                .Include(r => r.Trip)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // GET: Requests/Create
        public IActionResult Create()
        {
            ViewData["TripId"] = new SelectList(_context.Trips, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Requests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TripId,UserId,StatusRequest,Date")] Request request)
        {
            if (ModelState.IsValid)
            {
                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TripId"] = new SelectList(_context.Trips, "Id", "Id", request.TripId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", request.UserId);
            return View(request);
        }

        // GET: Requests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.Requests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            ViewData["TripId"] = new SelectList(_context.Trips, "Id", "Id", request.TripId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", request.UserId);
            return View(request);
        }

        // POST: Requests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TripId,UserId,StatusRequest,Date")] Request request)
        {
            if (id != request.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(request);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestExists(request.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TripId"] = new SelectList(_context.Trips, "Id", "Id", request.TripId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", request.UserId);
            return View(request);
        }


        // POST: Requests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl)
        {
            var request = await _context.Requests.FindAsync(id);
            if (request != null)
            {
                _context.Requests.Remove(request);
            }

            await _context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl); // Redirects back to the previous page
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RequestExists(int id)
        {
            return _context.Requests.Any(e => e.Id == id);
        }
    }
}
