using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Extentions;
using TestProject.Models.ViewModels;
using TestProject.Models;

//[Authorize(Roles = "Admin")]
public class ListAllUsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public ListAllUsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    // GET: Admin/Users
    public async Task<IActionResult> Users(string roleFilter, string searchTerm, int? pageNumber)
    {
        // Fetch all roles for the dropdown
        ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name", roleFilter);
        ViewBag.CurrentRoleFilter = roleFilter;

        // Query users with their roles
        var usersQuery = from u in _context.Users
                         join ur in _context.UserRoles on u.Id equals ur.UserId into userRoles
                         from ur in userRoles.DefaultIfEmpty()
                         join r in _context.Roles on ur.RoleId equals r.Id into userRolesRoles
                         from r in userRolesRoles.DefaultIfEmpty()
                         group r by u into g
                         select new UserViewModel
                         {
                             User = g.Key,
                             Role = g.Select(x => x.Name).FirstOrDefault() ?? "No Role"
                         };

        // Calculate total users before applying filters
        var totalUsers = await usersQuery.CountAsync();

        // Apply role filter (if not "All Roles")
        if (!string.IsNullOrEmpty(roleFilter) && roleFilter != "All Roles")
        {
            usersQuery = usersQuery.Where(u => u.Role == roleFilter);
        }

        // Materialize the query into memory
        var userList = await usersQuery.ToListAsync();

        // Set count message in ViewBag
        if (!string.IsNullOrEmpty(roleFilter) && roleFilter != "All Roles")
        {
            ViewBag.UserCountMessage = $"Number of users with role '{roleFilter}': {userList.Count}";
        }
        else
        {
            ViewBag.UserCountMessage = $"Total Users: {totalUsers}";
        }


        if (!string.IsNullOrEmpty(searchTerm))
        {
            userList = userList.Where(u =>
            u.User.UserName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || // Search UserName
            u.User.FirstName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || // Search FirstName
            u.User.LastName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList(); // Search LastName
        }

        ViewBag.FilteredUserCountMessage = $"Number of users with these filters: {userList.Count}";

        // Pagination
        int pageSize = 3;
        var paginatedUsers = PaginatedList<UserViewModel>.CreateFromList(userList, pageNumber ?? 1, pageSize);

        return View(paginatedUsers);
    }

    // POST: Admin/EditRole
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRole(string userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return NotFound();

        // Validate role exists
        var role = await _roleManager.FindByNameAsync(newRole);
        if (role == null)
        {
            ModelState.AddModelError("", "Invalid role selected.");
            return RedirectToAction(nameof(Users));
        }

        // Remove all existing roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // Assign new role
        await _userManager.AddToRoleAsync(user, newRole);

        // Update user position and other properties based on role
        user.Position = newRole;
        if (newRole == "Driver")
        {
            user.DateOfDriverAcceptance = DateTime.Now;
            user.ImagePath = "/images/drivers/default-image-Driver.jpg";

            var requestDrivers = await _context.RequestDrivers.Where(rd => rd.UserId == user.Id).ToListAsync();
            _context.RequestDrivers.RemoveRange(requestDrivers);
        }
        else if (newRole == "Tourist")
        {
            user.DateOfDriverAcceptance = null;
            user.ImagePath = null;

            var trips = await _context.Trips.Where(rd => rd.DriversId == user.Id && rd.StatusTrip != TripStatus.Finished).ToListAsync();
            _context.Trips.RemoveRange(trips);

            var requests = await _context.Requests.Where(r => r.Trip.DriversId == user.Id && r.Trip.StatusTrip != TripStatus.Finished).ToListAsync();
            _context.Requests.RemoveRange(requests);

            var tripParticipants = await _context.TripParticipants.Where(tp => tp.Trip.DriversId == user.Id && tp.Trip.StatusTrip != TripStatus.Finished).ToListAsync();
            _context.TripParticipants.RemoveRange(tripParticipants);
        }

        // Save changes to user
        await _userManager.UpdateAsync(user);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Users));
    }

    [HttpPost, ActionName("DeleteUser")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUserConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        // Remove related data
        var trips = await _context.Trips.Where(t => t.DriversId == user.Id).ToListAsync();
        _context.Trips.RemoveRange(trips);

        var requests = await _context.Requests.Where(r => r.UserId == user.Id).ToListAsync();
        _context.Requests.RemoveRange(requests);

        var requestDrivers = await _context.RequestDrivers.Where(rd => rd.UserId == user.Id).ToListAsync();
        _context.RequestDrivers.RemoveRange(requestDrivers);

        var tripParticipants = await _context.TripParticipants.Where(tp => tp.UserId == user.Id).ToListAsync();
        _context.TripParticipants.RemoveRange(tripParticipants);

        // Delete the user
        await _userManager.DeleteAsync(user);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Users));
    }
}