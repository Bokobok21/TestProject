using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestProject.Data;
using TestProject.Models;

public class TripStatusUpdater : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TripStatusUpdater> _logger;

    public TripStatusUpdater(IServiceScopeFactory scopeFactory, ILogger<TripStatusUpdater> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var now = DateTime.Now;

                    // Update trips that should now be "Ongoing"
                    var ongoingTrips = context.Trips
                        .Where(t => t.DepartureTime <= now && t.StatusTrip == TripStatus.Upcoming)
                        .ToList();

                    foreach (var trip in ongoingTrips)
                    {
                        trip.StatusTrip = TripStatus.Ongoing;
                    }

                    // Update trips that should now be "Finished"
                    var finishedTrips = context.Trips
                        .Where(t => t.ReturnTime <= now && t.StatusTrip == TripStatus.Ongoing)
                        .ToList();

                    foreach (var trip in finishedTrips)
                    {
                        trip.StatusTrip = TripStatus.Finished;
                    }

                    if (ongoingTrips.Any() || finishedTrips.Any())
                    {
                        await context.SaveChangesAsync();
                        _logger.LogInformation("Статусът на пътешествието е сменен успешно.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Промяната на статуса на пътешествието не беше успешна.");
            }

            // Wait for 5 minutes before checking again
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
