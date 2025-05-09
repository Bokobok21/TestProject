﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Models;
using TestProject.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace TestProject.Controllers
{
    [Authorize]
    public class DriversController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DriversController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Drivers
        public async Task<IActionResult> Index()
        {
            // Get all users with Driver role
            var drivers = await _context.ApplicationUsers
                .Where(u => u.Position == "Driver")
                .ToListAsync();

            // Get trip counts for each driver
            var driverViewModels = new List<DriverViewModel>();
            foreach (var driver in drivers)
            {
                var tripCount = await _context.Trips
                    .Where(t => t.DriversId == driver.Id)
                    .CountAsync();

                var ratingAvg = await _context.Ratings
                    .Where(r => r.Trip.DriversId == driver.Id)
                    .Select(r => r.Score)
                    .DefaultIfEmpty()
                    .AverageAsync();

                driverViewModels.Add(new DriverViewModel
                {
                    Driver = driver,
                    TripCount = tripCount,
                    AverageRating = ratingAvg
                });
            }

            double ratingWeight = 0.7;
            double tripCountWeight = 0.3;

            driverViewModels = driverViewModels
            .OrderByDescending(d => (ratingWeight * d.AverageRating) + (tripCountWeight * d.TripCount)) // Sort by weighted score
            .ToList();

            return View(driverViewModels);
        }
    }
}

