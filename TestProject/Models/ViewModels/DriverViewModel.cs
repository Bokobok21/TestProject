using System;
using System.Collections.Generic;

namespace TestProject.Models.ViewModels
{
    public class DriverViewModel
    {
        public ApplicationUser Driver { get; set; }
        public int TripCount { get; set; }
        public double AverageRating { get; set; }
    }
}

