using System;
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
    public class TripsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TripsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Trips
        public async Task<IActionResult> Index(string sortOrder, int? pageNumber)
        {
            sortOrder ??= "created_date_desc"; // Default sorting

            ViewBag.CurrentSortOrder = sortOrder;
            ViewBag.SortOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "created_date_desc", Text = "Newest First" },
                new SelectListItem { Value = "created_date_asc", Text = "Oldest First" },
                new SelectListItem { Value = "departure_time_asc", Text = "Departure Time (Ascending)" },
                new SelectListItem { Value = "departure_time_desc", Text = "Departure Time (Descending)" },
                new SelectListItem { Value = "return_time_asc", Text = "Return Time (Ascending)" },
                new SelectListItem { Value = "return_time_desc", Text = "Return Time (Descending)" },
                new SelectListItem { Value = "price_asc", Text = "Price (Low to High)" },
                new SelectListItem { Value = "price_desc", Text = "Price (High to Low)" },
                new SelectListItem { Value = "free_seats_asc", Text = "Free Seats (Ascending)" },
                new SelectListItem { Value = "free_seats_desc", Text = "Free Seats (Descending)" },
                new SelectListItem { Value = "total_seats_asc", Text = "Total Seats (Ascending)" },
                new SelectListItem { Value = "total_seats_desc", Text = "Total Seats (Descending)" },
                new SelectListItem { Value = "status_asc", Text = "Status (Ascending)" },
                new SelectListItem { Value = "status_desc", Text = "Status (Descending)" }
            };

            // Fetch trips from the database
            IQueryable<Trip> trips = _context.Trips.Include(t => t.Driver);

            // Ensure all trips are fetched
            int totalTrips = await trips.CountAsync();
            Console.WriteLine($"Total trips in DB: {totalTrips}");

            // Start with OrderBy()
            IOrderedQueryable<Trip> orderedTrips = sortOrder switch
            {
                "created_date_asc" => trips.OrderBy(t => t.CreatedDate),
                "created_date_desc" => trips.OrderByDescending(t => t.CreatedDate),
                "departure_time_asc" => trips.OrderBy(t => t.DepartureTime),
                "departure_time_desc" => trips.OrderByDescending(t => t.DepartureTime),
                "return_time_asc" => trips.OrderBy(t => t.ReturnTime),
                "return_time_desc" => trips.OrderByDescending(t => t.ReturnTime),
                "price_asc" => trips.OrderBy(t => t.Price),
                "price_desc" => trips.OrderByDescending(t => t.Price),
                "free_seats_asc" => trips.OrderBy(t => t.FreeSeats),
                "free_seats_desc" => trips.OrderByDescending(t => t.FreeSeats),
                "total_seats_asc" => trips.OrderBy(t => t.TotalSeats),
                "total_seats_desc" => trips.OrderByDescending(t => t.TotalSeats),
                "status_asc" => trips.OrderBy(t => t.StatusTrip),
                "status_desc" => trips.OrderByDescending(t => t.StatusTrip),
                _ => trips.OrderByDescending(t => t.CreatedDate) // Default sorting
            };

            // Ensure "Upcoming" trips appear first
            orderedTrips = orderedTrips.ThenBy(t => t.StatusTrip == TripStatus.Upcoming ? 0 : 1);

            // Pagination
            int pageSize = 4;
            var paginatedTrips = await PaginatedList<Trip>.CreateAsync(orderedTrips, pageNumber ?? 1, pageSize);

            Console.WriteLine($"Showing Page {pageNumber ?? 1}, Trips Displayed: {paginatedTrips.Count}");

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
                CreatedDate = trip.CreatedDate
            }).ToList();

            // Return view with paginated list
            return View(new PaginatedList<TripViewModel>(tripViewModels, totalTrips, paginatedTrips.PageIndex, pageSize));
        }


        // GET: Trips/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trip == null)
            {
                return NotFound();
            }

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

            return View(tripViewModel);
        }

        // GET: Trips/Create
        [Authorize(Roles = "Admin,Driver")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Trips/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Driver")]
        public async Task<IActionResult> Create(TripViewModel tripViewModel)
        {

            tripViewModel.DriversId = User.Id();
            tripViewModel.StatusTrip = TripStatus.Upcoming;

            if (ModelState.IsValid)
            {

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
                    StatusTrip = tripViewModel.StatusTrip
                };

                _context.Add(trip);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {

                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                    }
                }
            }

            ViewData["DriversId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", tripViewModel.DriversId);
            return View(tripViewModel);
        }

        // GET: Trips/Edit/5
        [Authorize(Roles = "Admin,Driver")]
        public async Task<IActionResult> Edit(int? id)
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
                StatusTrip = trip.StatusTrip
            };


            ViewData["DriversId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", trip.DriversId);
            return View(tripViewModel);
        }

        // POST: Trips/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Driver")]
        public async Task<IActionResult> Edit(int id, TripViewModel tripViewModel)
        {
            if (id != tripViewModel.Id)
            {
                return NotFound();
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
                        trip.FreeSeats += change;

                        //here we only add the change, make a method to delete the requests if the free seats are less than the total seats
                    }

                    trip.CarModel = tripViewModel.CarModel;
                    trip.PlateNumber = tripViewModel.PlateNumber;

                    if (trip.FreeSeats <= 0)
                    {
                        trip.StatusTrip = TripStatus.Booked;
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
                return RedirectToAction(nameof(Index));
            }
            else
            {

                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                    }
                }
            }
            ViewData["DriversId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", tripViewModel.DriversId);
            return View(tripViewModel);
        }

        // GET: Trips/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.Driver)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // POST: Trips/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
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
            return RedirectToAction(nameof(Index));
        }

        private bool TripExists(int id)
        {
            return _context.Trips.Any(e => e.Id == id);
        }
    }
}
