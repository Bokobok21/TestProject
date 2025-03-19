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

                    // Get due recurring trips
                    var recurringTrips = await context.Trips
                        .Where(t => t.IsRecurring && t.RecurrenceInterval.HasValue && t.NextRunDate.HasValue && t.NextRunDate <= Now)
                        .ToListAsync();

                    foreach (var trip in recurringTrips)
                    {
                        _logger.LogInformation("Processing recurring trip {TripId}", trip.Id);

                        var newDepartureTime = trip.NextRunDate.Value;

                        // Create new trip instance
                        var newTrip = new Trip
                        {
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

                        // Update next run date
                        trip.NextRunDate = newDepartureTime.Add(trip.RecurrenceInterval.Value);

                        // Save to database
                        context.Trips.Add(newTrip);
                        await context.SaveChangesAsync();
                    }
                }

                // Run every minute (adjust as needed)
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}