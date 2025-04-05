using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Models;

namespace TestProject.Controllers
{
    [Authorize]
    public class TripParticipantsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TripParticipantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TripParticipants
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TripParticipants.Include(t => t.Trip).Include(t => t.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TripParticipants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tripParticipant = await _context.TripParticipants
                .Include(t => t.Trip)
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tripParticipant == null)
            {
                return NotFound();
            }

            return View(tripParticipant);
        }

        // GET: TripParticipants/Create
        public IActionResult Create()
        {
            ViewData["TripId"] = new SelectList(_context.Trips, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: TripParticipants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,TripId")] TripParticipant tripParticipant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tripParticipant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TripId"] = new SelectList(_context.Trips, "Id", "Id", tripParticipant.TripId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", tripParticipant.UserId);
            return View(tripParticipant);
        }

        // GET: TripParticipants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tripParticipant = await _context.TripParticipants.FindAsync(id);
            if (tripParticipant == null)
            {
                return NotFound();
            }
            ViewData["TripId"] = new SelectList(_context.Trips, "Id", "Id", tripParticipant.TripId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", tripParticipant.UserId);
            return View(tripParticipant);
        }

        // POST: TripParticipants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,TripId")] TripParticipant tripParticipant)
        {
            if (id != tripParticipant.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tripParticipant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TripParticipantExists(tripParticipant.Id))
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
            ViewData["TripId"] = new SelectList(_context.Trips, "Id", "Id", tripParticipant.TripId);
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", tripParticipant.UserId);
            return View(tripParticipant);
        }


        // POST: TripParticipants/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, string returnUrl, string returnUrlOriginal, int tripId, string userId)
        {
            var tripParticipant = await _context.TripParticipants.FindAsync(id);
            if (tripParticipant != null)
            {
                // Store the number of seats before removing the participant
                int seatsToReturn = tripParticipant.NumberOfSeats;

                _context.TripParticipants.Remove(tripParticipant);
                await _context.SaveChangesAsync();

                // Return the seats to the trip
                var trip = await _context.Trips.FindAsync(tripId);
                if (trip != null)
                {
                    trip.FreeSeats += seatsToReturn;

                    // If the trip was fully booked before, update its status
                    if (trip.StatusTrip == TripStatus.Booked && trip.FreeSeats > 0)
                    {
                        trip.StatusTrip = TripStatus.Upcoming;
                    }

                    await _context.SaveChangesAsync();
                }
            }

            var request = await _context.Requests.Include(r => r.User).Where(r => r.User.Id == userId && r.TripId == tripId).FirstOrDefaultAsync();
            if (request != null)
            {
                _context.Requests.Remove(request);
                await _context.SaveChangesAsync();
            }

            //var trip = await _context.Trips.FindAsync(tripId);
            //if (trip != null)
            //{
            //    trip.FreeSeats++;
            //    await _context.SaveChangesAsync();
            //}


            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrlOriginal) && Url.IsLocalUrl(returnUrlOriginal))
            {
                return Redirect($"{returnUrl}?returnUrl={Uri.EscapeDataString(returnUrlOriginal)}"); // Redirects back to the previous page with returnUrl as a query parameter
            }
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("MyRequests", "PassengerRequests");
        }

        private bool TripParticipantExists(int id)
        {
            return _context.TripParticipants.Any(e => e.Id == id);
        }
    }
}
