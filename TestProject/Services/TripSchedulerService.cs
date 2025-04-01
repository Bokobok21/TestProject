using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Models;

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
                         && t.CreatedDate.Add(interval) <= Now)
                        .ToList();

                    foreach (var trip in recurringTrips)
                    {
                        _logger.LogInformation("Processing recurring trip {TripId}", trip.Id);

                        var newDepartureTime = trip.NextRunDate.Value;

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

                        TimeSpan reccurenceInterval = TimeSpan.Parse(trip.RecurrenceInterval);

                        // Update next run date
                        trip.NextRunDate = newDepartureTime.Add(reccurenceInterval);

                        // Save to database
                        context.Trips.Add(newTrip);
                        await context.SaveChangesAsync();
                    }
                }

                // Run every minute (adjust as needed)
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
        }
    }
}