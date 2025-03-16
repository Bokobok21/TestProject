using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using TestProject.Data;
using TestProject.Models;

namespace TestProject.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public ApplicationUser CurrentUser { get; set; }
        public bool HasDriverRequest { get; set; }

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment webHostEnvironment,
            ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
        }

        public class InputModel
        {
            public IFormFile? ImageFile { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string StatusMessage { get; set; }

        // Add this property to store the username
        public string Username { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            
            CurrentUser = await _userManager.GetUserAsync(User);
            if (CurrentUser == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            // Ensure ImagePath is set to default if null
            if (string.IsNullOrEmpty(CurrentUser.ImagePath))
            {
                CurrentUser.ImagePath = "/images/drivers/default-image-Driver.jpg";
            }

            // Check if the user has a pending driver request
            HasDriverRequest = await _context.RequestDrivers
                .AnyAsync(r => r.UserId == CurrentUser.Id && r.StatusRequest == RequestStatus.Pending);

            // Re-fetch the user's roles to reflect any changes made by the admin
            var roles = await _userManager.GetRolesAsync(CurrentUser);
            // Set the Username property
            Username = await _userManager.GetUserNameAsync(CurrentUser);

            await _signInManager.RefreshSignInAsync(CurrentUser);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                Username = await _userManager.GetUserNameAsync(user);
                return Page();
            }

            if (User.IsInRole("Driver") && Input.ImageFile != null && Input.ImageFile.Length > 0)
            {
                // Check if the current image is not the default image before deleting
                if (!string.IsNullOrEmpty(user.ImagePath) && !user.ImagePath.Equals("/images/drivers/default-image-Driver.jpg", StringComparison.OrdinalIgnoreCase))
                {
                    var oldImage = Path.Combine(_webHostEnvironment.WebRootPath, user.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImage))
                    {
                        System.IO.File.Delete(oldImage);
                    }
                }

                // Handle new image upload
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "drivers");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(Input.ImageFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Input.ImageFile.CopyToAsync(stream);
                }

                // Update ImagePath
                user.ImagePath = $"/images/drivers/{uniqueFileName}";
                StatusMessage = "Profile picture updated successfully!";
            }
            else if (string.IsNullOrEmpty(user.ImagePath) || user.ImagePath.Equals("/images/drivers/default-image-Driver.jpg", StringComparison.OrdinalIgnoreCase))
            {
                // Ensure the default image path is set if no file is uploaded
                user.ImagePath = "/images/drivers/default-image-Driver.jpg";
            }

            // Save changes to the database
            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);

            return RedirectToPage();
        }
    }
}