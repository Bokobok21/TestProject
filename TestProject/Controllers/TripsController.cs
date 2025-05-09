﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Extentions;
using TestProject.Models;
using TestProject.Models.ViewModels;

namespace TestProject.Controllers
{
    [Authorize]
    public class TripsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TripsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Trips
        [AllowAnonymous]
        public async Task<IActionResult> Index(string sortOrder, string startPosition, string destination, int? pageNumber, string? returnUrl, string driverId)
        {

            sortOrder ??= "departure_time_asc"; // Default sorting

            ViewBag.CurrentSortOrder = sortOrder;
            ViewBag.CurrentStartPosition = startPosition;
            ViewBag.CurrentDestination = destination;
            ViewBag.CurrentDriverId = driverId;
            ViewBag.ReturnUrl = returnUrl;

            ViewBag.SortOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "departure_time_asc", Text = "Час на тръгване (най-ранен)" },
                new SelectListItem { Value = "departure_time_desc", Text = "Час на тръгване (най-късен)" },
                new SelectListItem { Value = "return_time_asc", Text = "Час на връщане (най-ранен)" },
                new SelectListItem { Value = "return_time_desc", Text = "Час на връщане (най-късен)" },
                new SelectListItem { Value = "price_asc", Text = "Цена (от ниска към висока)" },
                new SelectListItem { Value = "price_desc", Text = "Цена (от висока към ниска)" },
                new SelectListItem { Value = "free_seats_asc", Text = "Свободни места (най-малко първо)" },
                new SelectListItem { Value = "free_seats_desc", Text = "Свободни места (най-много първо)" },
                new SelectListItem { Value = "total_seats_asc", Text = "Общо места (най-малко първо)" },
                new SelectListItem { Value = "total_seats_desc", Text = "Общо места (най-много първо)" },
                new SelectListItem { Value = "created_date_desc", Text = "Най-нови" },
                new SelectListItem { Value = "created_date_asc", Text = "Най-стари" },
            };


            var drivers = await _context.ApplicationUsers
                .Where(u => u.Position == "Driver")
                .Select(d => new SelectListItem
                {
                    Value = d.Id,
                    Text = $"{d.FirstName} {d.LastName} ({d.UserName})"
                })
                .ToListAsync();

            ViewBag.Drivers = new SelectList(drivers, "Value", "Text", driverId);

            IQueryable<Trip> trips = _context.Trips.Include(t => t.Driver);

            if (!string.IsNullOrEmpty(driverId))
            {
                trips = trips.Where(t => t.DriversId == driverId);

                var driver = await _context.ApplicationUsers.FindAsync(driverId);

                if (driver != null)
                {
                    ViewBag.DriverName = $"{driver.FirstName} {driver.LastName}";
                }
            }

            var tripsList = await trips.ToListAsync();

            if (!string.IsNullOrEmpty(startPosition))
            {
                tripsList = tripsList.Where(t => t.StartPosition.Contains(startPosition, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(destination))
            {
                tripsList = tripsList.Where(t => t.Destination.Contains(destination, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            IOrderedEnumerable<Trip> orderedTrips = tripsList.OrderBy(t => t.StatusTrip != TripStatus.Upcoming);
            orderedTrips = orderedTrips.ThenBy(t => t.StatusTrip);

            switch (sortOrder)
            {
                case "created_date_asc":
                    orderedTrips = orderedTrips.ThenBy(t => t.CreatedDate);
                    break;
                case "created_date_desc":
                    orderedTrips = orderedTrips.ThenByDescending(t => t.CreatedDate);
                    break;
                case "departure_time_asc":
                    orderedTrips = orderedTrips.ThenBy(t => t.DepartureTime);
                    break;
                case "departure_time_desc":
                    orderedTrips = orderedTrips.ThenByDescending(t => t.DepartureTime);
                    break;
                case "return_time_asc":
                    orderedTrips = orderedTrips.ThenBy(t => t.ReturnTime);
                    break;
                case "return_time_desc":
                    orderedTrips = orderedTrips.ThenByDescending(t => t.ReturnTime);
                    break;
                case "price_asc":
                    orderedTrips = orderedTrips.ThenBy(t => t.Price);
                    break;
                case "price_desc":
                    orderedTrips = orderedTrips.ThenByDescending(t => t.Price);
                    break;
                case "free_seats_asc":
                    orderedTrips = orderedTrips.ThenBy(t => t.FreeSeats);
                    break;
                case "free_seats_desc":
                    orderedTrips = orderedTrips.ThenByDescending(t => t.FreeSeats);
                    break;
                case "total_seats_asc":
                    orderedTrips = orderedTrips.ThenBy(t => t.TotalSeats);
                    break;
                case "total_seats_desc":
                    orderedTrips = orderedTrips.ThenByDescending(t => t.TotalSeats);
                    break;
                default:
                    // Default secondary sort 
                    orderedTrips = orderedTrips.ThenBy(t => t.DepartureTime);
                    break;
            }
          

            // Pagination
            int pageSize = 6;
            var paginatedTrips = PaginatedList<Trip>.CreateFromList(orderedTrips, pageNumber ?? 1, pageSize);

            // Convert to ViewModel
            var tripViewModels = paginatedTrips.Select(trip => new TripViewModel
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
                CreatedDate = trip.CreatedDate,
                tripSchedule = trip.tripSchedule,
            }).ToList();

            ViewBag.TripCount = tripsList.Count;
            // Return view with paginated list
            return View(new PaginatedList<Trip>(paginatedTrips, tripsList.Count, paginatedTrips.PageIndex, pageSize));
        }


        // GET: Trips/Details/5
        public async Task<IActionResult> Details(int? id, string? returnUrl)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.Ratings!)
                     .ThenInclude(r => r.User)
                .Include(t => t.Driver)
                .Include(t => t.TripParticipants)
                .Include(t => t.Requests!)
                     .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (trip == null)
            {
                return NotFound();
            }

            var tripViewModel = new TripViewModel
            {
                Id = trip.Id,
                Driver = trip.Driver,
                DriversId = trip.DriversId,
                DriverName = trip.Driver.UserName,
                DriverPhoneNumber = trip.Driver.PhoneNumber,
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
                Ratings = trip.Ratings,
                TripParticipants = trip.TripParticipants,
                IsRecurring = trip.IsRecurring,
                RecurrenceInterval = trip.RecurrenceInterval,
                NextRunDate = trip.NextRunDate,
                Requests = trip.Requests
            };

            ViewBag.ReturnUrl = returnUrl;
            return View(tripViewModel);
        }

        // GET: Trips/Create
        [Authorize(Roles = "Admin,Driver")]
        public async Task<IActionResult> CreateAsync(string? returnUrl)
        {
            // Get user's existing trips
            var userId = User.Id();
            var userTrips = await GetUserOverlappingTrips(userId);

            // Pass to view
            ViewBag.UserTrips = System.Text.Json.JsonSerializer.Serialize(userTrips);
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Trips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Driver")]
        public async Task<IActionResult> Create(TripViewModel tripViewModel, string? returnUrl)
        {
            tripViewModel.DriversId = User.Id();
            tripViewModel.StatusTrip = TripStatus.Upcoming;

            var recurrenceInterval = TimeSpan.FromDays(tripViewModel.RecurrenceIntervalDays ?? 0)
              .Add(TimeSpan.FromHours(tripViewModel.RecurrenceIntervalHours ?? 0))
              .Add(TimeSpan.FromMinutes(tripViewModel.RecurrenceIntervalMinutes ?? 0));

            if (tripViewModel.IsRecurring && recurrenceInterval == TimeSpan.Zero)
            {
                ModelState.AddModelError("", "Интервалът трябва да е по-голям от нула.");
            }

            if (ModelState.IsValid)
            {
                if (tripViewModel.ImageFile != null && tripViewModel.ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine("wwwroot", "images", "trips");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(tripViewModel.ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await tripViewModel.ImageFile.CopyToAsync(stream);
                    }

                    tripViewModel.ImagePath = $"/images/trips/{uniqueFileName}";
                }
                else
                {
                    tripViewModel.ImagePath = "/images/trips/default-image.jpg"; // Default image path if no file is uploaded
                }

                var trip = new Trip
                {
                    DriversId = tripViewModel.DriversId,
                    StartPosition = tripViewModel.StartPosition,
                    Destination = tripViewModel.Destination,
                    DepartureTime = tripViewModel.DepartureTime,
                    ReturnTime = tripViewModel.ReturnTime,
                    Price = tripViewModel.Price,
                    TotalSeats = tripViewModel.TotalSeats,
                    FreeSeats = tripViewModel.TotalSeats,
                    CarModel = tripViewModel.CarModel,
                    PlateNumber = tripViewModel.PlateNumber,
                    ImagePath = tripViewModel.ImagePath,
                    StatusTrip = tripViewModel.StatusTrip,
                    IsRecurring = tripViewModel.IsRecurring,
                    RecurrenceInterval = recurrenceInterval.ToString(),
                    NextRunDate = tripViewModel.DepartureTime.Add(recurrenceInterval),
                    NextStart = tripViewModel.DepartureTime.Add(recurrenceInterval)
                };

                _context.Add(trip);
                await _context.SaveChangesAsync();

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    ViewBag.ReturnUrl = returnUrl;
                    return Redirect(returnUrl); // Redirects back to the previous page
                }

                return RedirectToAction(nameof(Index));

            }

            ViewData["DriversId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", tripViewModel.DriversId);
            return View(tripViewModel);
        }

        // GET: Trips/Edit/5
        [Authorize(Roles = "Admin,Driver")]
        public async Task<IActionResult> Edit(int? id, string? returnUrl, string? returnUrlOriginal)
        {
            if (id == null || _context.Trips == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }

            // Get user's existing trips (excluding current trip)
            var userId = User.Id();
            var userTrips = await GetUserOverlappingTrips(userId, id);

            // Pass to view
            ViewBag.UserTrips = System.Text.Json.JsonSerializer.Serialize(userTrips);


            TimeSpan reccuranceInterval = TimeSpan.Parse(trip.RecurrenceInterval);

            var tripViewModel = new TripViewModel
            {
                Id = trip.Id,
                DriversId = trip.DriversId,
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
                IsRecurring = trip.IsRecurring,
                RecurrenceInterval = trip.RecurrenceInterval,
                RecurrenceIntervalDays = reccuranceInterval.Days,
                RecurrenceIntervalHours = reccuranceInterval.Hours,
                RecurrenceIntervalMinutes = reccuranceInterval.Minutes
            };

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ReturnUrlOriginal = returnUrlOriginal;
            ViewData["DriversId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", trip.DriversId);
            return View(tripViewModel);
        }

        // POST: Trips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Driver")]
        public async Task<IActionResult> Edit(int id, TripViewModel tripViewModel, string? returnUrl, string? returnUrlOriginal)
        {
            if (id != tripViewModel.Id)
            {
                return NotFound();
            }

            var recurrenceInterval = TimeSpan.FromDays(tripViewModel.RecurrenceIntervalDays ?? 0)
            .Add(TimeSpan.FromHours(tripViewModel.RecurrenceIntervalHours ?? 0))
            .Add(TimeSpan.FromMinutes(tripViewModel.RecurrenceIntervalMinutes ?? 0));

            if (tripViewModel.IsRecurring && recurrenceInterval == TimeSpan.Zero)
            {
                ModelState.AddModelError("", "Интервалът трябва да е по-голям от нула.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var trip = await _context.Trips.FindAsync(id);
                    if (trip == null)
                    {
                        return NotFound();
                    }

                    // Handle image upload
                    if (tripViewModel.ImageFile != null && tripViewModel.ImageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine("wwwroot", "images", "trips");

                        // Ensure the directory exists
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(tripViewModel.ImageFile.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await tripViewModel.ImageFile.CopyToAsync(stream);
                        }

                        // Delete old image if it exists
                        if (!string.IsNullOrEmpty(trip.ImagePath))
                        {
                            string oldFilePath = Path.Combine("wwwroot", trip.ImagePath);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        trip.ImagePath = $"/images/trips/{uniqueFileName}";
                    }

                    // Update other properties
                    trip.StartPosition = tripViewModel.StartPosition;
                    trip.Destination = tripViewModel.Destination;
                    trip.DepartureTime = tripViewModel.DepartureTime;
                    trip.ReturnTime = tripViewModel.ReturnTime;
                    trip.Price = tripViewModel.Price;

                    if (trip.TotalSeats != tripViewModel.TotalSeats)
                    {
                        var change = tripViewModel.TotalSeats - trip.TotalSeats;

                        trip.TotalSeats = tripViewModel.TotalSeats;

                        if (trip.FreeSeats + change < 0)
                        {
                            var requests = await _context.Requests.Where(r => r.Trip.DriversId == trip.DriversId && r.StatusRequest != RequestStatus.Pending).ToListAsync();
                            _context.Requests.RemoveRange(requests);

                            var tripParticipants = await _context.TripParticipants.Where(tp => tp.Trip.DriversId == trip.DriversId && tp.Trip.StatusTrip != TripStatus.Finished).ToListAsync();
                            _context.TripParticipants.RemoveRange(tripParticipants);

                            trip.FreeSeats = trip.TotalSeats;
                        }
                        else
                        {
                            trip.FreeSeats += change;
                        }
                    }

                    trip.CarModel = tripViewModel.CarModel;
                    trip.PlateNumber = tripViewModel.PlateNumber;
                    trip.IsRecurring = tripViewModel.IsRecurring;
                    trip.RecurrenceInterval = recurrenceInterval.ToString();
                    trip.NextRunDate = tripViewModel.DepartureTime.Add(recurrenceInterval);
                    trip.NextStart = tripViewModel.DepartureTime.Add(recurrenceInterval);


                    if (trip.FreeSeats == 0 && trip.StatusTrip != tripViewModel.StatusTrip && tripViewModel.StatusTrip != TripStatus.Upcoming)
                    {
                        trip.StatusTrip = tripViewModel.StatusTrip;
                    }
                    else if (trip.FreeSeats == 0 && trip.StatusTrip == tripViewModel.StatusTrip && trip.StatusTrip != TripStatus.Upcoming)
                    {

                    }
                    else if (trip.FreeSeats == 0)
                    {
                        trip.StatusTrip = TripStatus.Booked;
                    }
                    else if (trip.FreeSeats > 0 && trip.StatusTrip == tripViewModel.StatusTrip && trip.StatusTrip == TripStatus.Booked)
                    {
                        trip.StatusTrip = TripStatus.Upcoming;
                    }
                    else
                    {
                        trip.StatusTrip = tripViewModel.StatusTrip;
                    }

                        _context.Update(trip);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TripExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrlOriginal) && Url.IsLocalUrl(returnUrlOriginal))
                {
                    return Redirect($"{returnUrl}?returnUrl={Uri.EscapeDataString(returnUrlOriginal)}"); // Redirects back to the previous page with returnUrl as a query parameter
                }
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl); 
                }

                //https://localhost:7052/Trips/Details/21?returnUrl=%2FDriverTrips%2FMyDriverTrips
                //https://localhost:7052/Trips/Details/21?returnUrl=%2FTrips%2FDetails%2F21
                return RedirectToAction("MyDriverTrips", "DriverTrips");

            }

            ViewData["DriversId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", tripViewModel.DriversId);
            return View(tripViewModel);
        }


        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string? returnUrl)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip != null)
            {
                if (!string.IsNullOrEmpty(trip.ImagePath))
                {
                    string filePath = Path.Combine("wwwroot", trip.ImagePath);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                _context.Trips.Remove(trip);
            }
            await _context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl); 
            }

            return RedirectToAction("MyDriverTrips", "DriverTrips");

        }

        private bool TripExists(int id)
        {
            return _context.Trips.Any(e => e.Id == id);
        }

        private async Task<List<object>> GetUserOverlappingTrips(string userId, int? excludeTripId = null)
        {
            var driverTripsQuery = _context.Trips
                .Where(t => t.DriversId == userId &&
                       (t.StatusTrip == TripStatus.Upcoming || t.StatusTrip == TripStatus.Ongoing));

            if (excludeTripId.HasValue)
            {
                driverTripsQuery = driverTripsQuery.Where(t => t.Id != excludeTripId.Value);
            }

            var driverTrips = await driverTripsQuery
                .Select(t => new {
                    id = t.Id,
                    start = t.DepartureTime,
                    end = t.ReturnTime,
                    startPosition = t.StartPosition,
                    destination = t.Destination
                })
                .ToListAsync();

            var passengerTripsQuery = _context.TripParticipants
                .Where(tp => tp.UserId == userId)
                .Include(tp => tp.Trip)
                .Where(tp => tp.Trip.StatusTrip == TripStatus.Upcoming || tp.Trip.StatusTrip == TripStatus.Ongoing);

            if (excludeTripId.HasValue)
            {
                passengerTripsQuery = passengerTripsQuery.Where(tp => tp.Trip.Id != excludeTripId.Value);
            }

            var passengerTrips = await passengerTripsQuery
                .Select(tp => new {
                    id = tp.Trip.Id,
                    start = tp.Trip.DepartureTime,
                    end = tp.Trip.ReturnTime,
                    startPosition = tp.Trip.StartPosition,
                    destination = tp.Trip.Destination
                })
                .ToListAsync();

            var userTrips = driverTrips.Concat(passengerTrips).Cast<object>().ToList();
            return userTrips;
        }


    }
}
