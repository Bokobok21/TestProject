using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Globalization;
using TestProject.Data;
using TestProject.Models;
using static System.Formats.Asn1.AsnWriter;

namespace TestProject.Services
{
    public class TripSchedulerService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<TripSchedulerService> _logger;

        public TripSchedulerService(IServiceProvider services, ILogger<TripSchedulerService> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checking for recurring trips at {Time}", DateTime.Now);

                using (var scope = _services.CreateScope())
                {

                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var Now = DateTime.Now;

                    //// Get due recurring trips
                    //var recurringTrips = await context.Trips
                    //    .Where(t => t.IsRecurring && TimeSpan.TryParse(t.RecurrenceInterval, out TimeSpan interval) && t.RecurrenceInterval != "00:00:00" && t.NextRunDate.HasValue && t.CreatedDate.Add(TimeSpan.Parse(t.RecurrenceInterval)) <= Now)
                    //    .ToListAsync();

                    var allRecurringTrips = await context.Trips
                     .Where(t => t.IsRecurring && t.RecurrenceInterval != "00:00:00" && t.NextRunDate.HasValue)
                     .ToListAsync(); // Fetch first

                    var recurringTrips = allRecurringTrips
                        .Where(t => TimeSpan.TryParse(t.RecurrenceInterval, out TimeSpan interval)
                         && t.tripSchedule.Add(interval) <= Now)
                        .ToList();

                    // createddate + interval so it doesn't keep reccuring; 

                    foreach (var trip in recurringTrips)
                    {
                        _logger.LogInformation("Processing recurring trip {TripId}", trip.Id);

                        var newDepartureTime = trip.NextStart;
                        TimeSpan reccurenceInterval = TimeSpan.Parse(trip.RecurrenceInterval);

                        if (await UserHasNoOverlappingTrips(trip))
                        {

                            // Create new trip instance
                            var newTrip = new Trip
                            {
                                Driver = trip.Driver,
                                DriversId = trip.DriversId,
                                StartPosition = trip.StartPosition,
                                Destination = trip.Destination,
                                DepartureTime = newDepartureTime,
                                ReturnTime = newDepartureTime.Add(trip.ReturnTime - trip.DepartureTime),
                                Price = trip.Price,
                                TotalSeats = trip.TotalSeats,
                                FreeSeats = trip.TotalSeats, // Reset seats
                                CarModel = trip.CarModel,
                                PlateNumber = trip.PlateNumber,
                                ImagePath = trip.ImagePath,
                                StatusTrip = TripStatus.Upcoming,
                                CreatedDate = Now,
                                IsRecurring = false // New trips are not templates
                            };
                            //trip.NextRunDate = newDepartureTime.Add(reccurenceInterval);
                            context.Trips.Add(newTrip);
                        }

                        trip.NextStart = trip.NextStart.Add(reccurenceInterval);
                        // Update next run date
                        trip.tripSchedule = trip.tripSchedule.Add(reccurenceInterval);

                        // Save to database
                        await context.SaveChangesAsync();
                    }
                }

                // Run every minute (adjust as needed)
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
        private async Task<bool> UserHasNoOverlappingTrips(Trip trip)
        {
            using (var scope = _services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var overlappingTrips = await context.Trips
                    .Where(t => t.DriversId == trip.DriversId &&
                                (t.StatusTrip == TripStatus.Upcoming || t.StatusTrip == TripStatus.Ongoing))
                    .ToListAsync(); // Fetch all relevant trips first

                var NewDepartureTime = trip.NextStart;
                var NewReturnTime = NewDepartureTime.Add(trip.ReturnTime - trip.DepartureTime);

                // Apply the overlap logic
                bool hasOverlap = overlappingTrips.Any(t =>
                    (NewDepartureTime >= t.DepartureTime && NewDepartureTime <= t.ReturnTime) || // New trip starts inside existing trip
                    (NewReturnTime >= t.DepartureTime && NewReturnTime <= t.ReturnTime) || // New trip ends inside existing trip
                    (NewDepartureTime <= t.DepartureTime && NewReturnTime >= t.ReturnTime) // New trip fully contains existing trip
                );

                return !hasOverlap; // Return true if there is NO overlap
            }
        }

    }
}

