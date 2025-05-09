﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Extentions;
using TestProject.Models;

namespace TestProject.Controllers.UserControllers
{
    [Authorize]
    public class DriverTripsController : Controller
    {
        private readonly ApplicationDbContext _context; // Add this line to define the _context field

        public DriverTripsController(ApplicationDbContext context) // Add this constructor to initialize the _context field
        {
            _context = context;
        }

        public IActionResult MyDriverTrips()
        {
            var userId = User.Id();
            var trips = _context.Trips
                         .Where(t => t.DriversId == userId)
                            .Include(t => t.Driver)
                         .ToList();
           // IOrderedEnumerable<Trip> orderedTrips = trips.OrderBy(t => t.StatusTrip != TripStatus.Upcoming);
            IOrderedEnumerable<Trip> orderedTrips = trips.OrderBy(t => t.StatusTrip != TripStatus.Upcoming);
            orderedTrips = orderedTrips.ThenBy(t => t.StatusTrip);
            orderedTrips = orderedTrips.ThenBy(t => t.DepartureTime);

            ViewData["ShowActions"] = true;
            return View(orderedTrips);
        }

        public IActionResult DeleteTrip(int id)
        {
            var trip = _context.Trips.Find(id);

            if (trip == null || trip.DriversId != User.Id())
            {
                return NotFound();
            }

            _context.Trips.Remove(trip);
            _context.SaveChanges();

            return RedirectToAction("MyDriverTrips");
        }

    }
}
