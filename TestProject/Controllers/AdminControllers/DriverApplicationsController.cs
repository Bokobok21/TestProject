using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Threading.Tasks;
using TestProject.Data;
using TestProject.Models;

//[Authorize(Roles = "Admin")]
public class DriverApplicationsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly SignInManager<ApplicationUser> _signinManager;

    public DriverApplicationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, SignInManager<ApplicationUser> signinManager)
    {
        _context = context;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
        _signinManager = signinManager;
    }

    // GET: DriverApplications
    public async Task<IActionResult> Index(string? returnUrl)
    {
        var requests = await _context.RequestDrivers
            .Include(a => a.User)
            .Where(a => a.StatusRequest == RequestStatus.Pending)
            .ToListAsync();

        ViewBag.ReturnUrl = returnUrl;
        return View(requests);
    }

    // POST: DriverApplications/Approve/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id, string? returnUrl)
    {
        var request = await _context.RequestDrivers.FindAsync(id);
        if (request == null || request.StatusRequest != RequestStatus.Pending)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return NotFound();
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        _context.RequestDrivers.RemoveRange(request);

        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // Set user role to "Driver"
        await _userManager.AddToRoleAsync(user, "Driver");

        // Update RequestDriver status
        request.StatusRequest = RequestStatus.Accepted;
        user.DateOfDriverAcceptance = DateTime.Now;
        user.Position = "Driver";

        if (string.IsNullOrEmpty(user.ImagePath))
        {
            user.ImagePath = "/images/drivers/default-image-Driver.jpg"; // Default image path
        }

        //await _signinManager.RefreshSignInAsync(user);

        await _userManager.UpdateAsync(user);

        await _context.SaveChangesAsync();


        //if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    return Redirect(returnUrl); // Redirects back to the previous page
        //}

       /* return RedirectToAction("MyRequests", "PassengerRequests")*/;
        return RedirectToAction("Index");
    }

    // POST: DriverApplications/Deny/5
    [HttpPost]
    public async Task<IActionResult> Deny(int id, string? returnUrl)
    {
        var request = await _context.RequestDrivers.FindAsync(id);
        if (request == null || request.StatusRequest != RequestStatus.Pending)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user != null && !string.IsNullOrEmpty(user.ImagePath))
        {
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, user.ImagePath);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

            user.ImagePath = null;
            await _userManager.UpdateAsync(user);
        }

        // Update RequestDriver status
        request.StatusRequest = RequestStatus.Rejected;

        await _context.SaveChangesAsync();

        //if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    return Redirect(returnUrl); // Redirects back to the previous page
        //}

        //return RedirectToAction("MyRequests", "PassengerRequests");
        return RedirectToAction("Index");

    }
}