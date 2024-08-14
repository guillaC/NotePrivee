using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotePrivee.Models;
using NotePrivee.Services;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NotePrivee.Controllers
{
    public class AdminController : Controller
    {
        private readonly notepriveeContext _context;

        public AdminController(notepriveeContext context)
        {
            _context = context;
        }

        // GET: Admin/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Admin/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login([Bind("Username,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = AdminService.Hash(user.Password);
                if (_context.Users.Where(u => u.Username == user.Username && u.Password == user.Password).Count() == 1)
                {
                    var userClaims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                    };

                    var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");
                    var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });

                    HttpContext.SignInAsync(userPrincipal);
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Login));
        }

        // GET: Admin
        [Authorize]
        public async Task<IActionResult> Index()
        {
            List<Note> Notes = await _context.Notes.ToListAsync();
            List<User> Users = await _context.Users.ToListAsync();

            ViewData["Chart"] = AdminService.GenerateChartNotes(12, Notes);
            ViewData["Notes"] = Notes;
            ViewData["Users"] = Users;
            return View();
        }

        // POST: Admin/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Username,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = AdminService.Hash(user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Username,Password")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.Password = AdminService.Hash(user.Password);
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Clear
        [Authorize]
        [HttpPost, ActionName("Clear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear(int id)
        {
            List<Note> notes = await _context.Notes.ToListAsync();
            foreach (Note note in notes)
            {
                _context.Notes.Remove(note);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
