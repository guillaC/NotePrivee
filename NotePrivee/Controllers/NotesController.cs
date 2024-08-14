using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Ganss.Xss;
using Ganss.XSS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotePrivee.Models;
using Westwind.AspNetCore.Markdown;

namespace NotePrivee.Controllers
{
    public class NotesController : Controller
    {
        private readonly notepriveeContext _context;

        public NotesController(notepriveeContext context)
        {
            _context = context;
        }

        // GET: Notes/Read/5
        public async Task<IActionResult> Read(int? id, string key)
        {
            if (id == null || key == null)
            {
                return View(null);
            }

            var note = await _context.Notes.FirstOrDefaultAsync(m => m.Id == id);

            if (note == null)
            {
                return View(null);
            }

            Note decipheredNote = new Note
            (
                Contenu: note.Contenu,
                DateCreation: note.DateCreation,
                DateExpiration: note.DateExpiration,
                NombreVue: note.NombreVue - 1
            );

            if (note.DateExpiration <= DateTime.Now)
            {
                _context.Notes.Remove(note); // suppression de la note
                return View(null);
            }

            HtmlSanitizer sanitizer = new HtmlSanitizer();
            try
            {
                decipheredNote.Contenu = SimpleAES.AES256.Decrypt(decipheredNote.Contenu, key); // déchiffrement
                decipheredNote.Contenu = sanitizer.Sanitize(decipheredNote.Contenu);
                decipheredNote.Contenu = Markdown.Parse(decipheredNote.Contenu); // conversion markdown -> html
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine(ex);
                return View(null);
            }
            if (note.NombreVue == 1)
            {
                _context.Notes.Remove(note); // suppression de la note
            }
            else
            {
                note.NombreVue -= 1;
            }
            await _context.SaveChangesAsync();
            return View(decipheredNote);
        }

        // GET: Notes/Index
        public IActionResult Index()
        {
            return View();
        }

        // POST: Notes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Contenu,DateExpiration,NombreVue")] Note note)
        {
            if (ModelState.IsValid)
            {
                if (note.Contenu is not null)
                {
                    var baseUrl = $"{this.Request.Scheme}://{this.Request.Host.Value}{this.Request.PathBase.Value}";
                    string key = GenerateRandomKey();
                    note.Contenu = SimpleAES.AES256.Encrypt(note.Contenu, key);
                    _context.Add(note);
                    await _context.SaveChangesAsync();
                    ViewData["Url"] = baseUrl + "/Notes/Read/" + note.Id.ToString() + "?key=" + key;
                } else
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View("~/Views/Notes/Index.cshtml", note);
        }

        private bool NoteExists(int id)
        {
            return _context.Notes.Any(e => e.Id == id);
        }

        private static string GenerateRandomKey()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 30);
        }
    }
}
