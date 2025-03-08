using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using TestProject.Models;
using TestProject.Data;
using Microsoft.EntityFrameworkCore;

namespace TestProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/PendingRequests
        public async Task<IActionResult> PendingRequests()
        {   
            // Fetch all pending requests along with related trip and user details
            var pendingRequests = await _context.Requests
                .Where(r => r.StatusRequest == RequestStatus.Pending)
                .Include(r => r.Trip)
                .Include(r => r.User)
                .ToListAsync();

            return View(pendingRequests);
        }
    }
}
