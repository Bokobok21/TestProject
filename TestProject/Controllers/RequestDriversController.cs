using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TestProject.Data;
using TestProject.Models;

namespace TestProject.Controllers
{
    public class RequestDriversController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestDriversController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RequestDrivers
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.RequestDrivers.Include(r => r.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: RequestDrivers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestDriver = await _context.RequestDrivers
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requestDriver == null)
            {
                return NotFound();
            }

            return View(requestDriver);
        }

        // GET: RequestDrivers/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: RequestDrivers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ImagePath,StatusRequest,Date")] RequestDriver requestDriver)
        {
            if (ModelState.IsValid)
            {
                _context.Add(requestDriver);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", requestDriver.UserId);
            return View(requestDriver);
        }

        // GET: RequestDrivers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestDriver = await _context.RequestDrivers.FindAsync(id);
            if (requestDriver == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", requestDriver.UserId);
            return View(requestDriver);
        }

        // POST: RequestDrivers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,ImagePath,StatusRequest,Date")] RequestDriver requestDriver)
        {
            if (id != requestDriver.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(requestDriver);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestDriverExists(requestDriver.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", requestDriver.UserId);
            return View(requestDriver);
        }

        // GET: RequestDrivers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestDriver = await _context.RequestDrivers
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requestDriver == null)
            {
                return NotFound();
            }

            return View(requestDriver);
        }

        // POST: RequestDrivers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var requestDriver = await _context.RequestDrivers.FindAsync(id);
            if (requestDriver != null)
            {
                _context.RequestDrivers.Remove(requestDriver);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestDriverExists(int id)
        {
            return _context.RequestDrivers.Any(e => e.Id == id);
        }
    }
}
