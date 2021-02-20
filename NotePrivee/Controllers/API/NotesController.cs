using Ganss.XSS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotePrivee.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NotePrivee.Controllers.API
{

    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly notepriveeContext _context;

        public NotesController(notepriveeContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retourne une note spécifique à partir de son couple ID/KEY
        /// </summary>
        /// <param name="id">id de la note</param>
        /// <param name="key">clé de déchiffrement</param>
        [HttpGet]
        public ActionResult<Note> Get(int id, string key)
        {
            Note note = _context.Notes.FirstOrDefault(m => m.Id == id);
            HtmlSanitizer sanitizer = new HtmlSanitizer();
            if (note == null) return NotFound();

            if (note.DateExpiration <= DateTime.Now)
            {
                _context.Notes.Remove(note); // suppression de la note
                return NotFound();
            }

            if (note.NombreVue == 1)
            {
                _context.Notes.Remove(note); // suppression de la note
            }

            Note decipheredNote = new Note();

            try
            {
                decipheredNote.Contenu = SimpleAES.AES256.Decrypt(note.Contenu, key); // déchiffrement
                decipheredNote.Contenu = sanitizer.Sanitize(decipheredNote.Contenu);
                decipheredNote.DateCreation = note.DateCreation;
                decipheredNote.DateExpiration = note.DateExpiration;
                decipheredNote.NombreVue = note.NombreVue;
            }
            catch (CryptographicException ex)
            {
                Console.WriteLine(ex);
                return NotFound();
            }

            note.NombreVue--;
            _context.SaveChanges();
            return Ok(decipheredNote);
        }

        /// <summary>
        /// Publie une note chiffrée, retourne l'URL de celle-ci
        /// </summary>
        /// <param name="content">contenu de la note</param>   
        [HttpPost]
        public ActionResult Post([FromBody] Note data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (data.Contenu == null) return BadRequest(data);
            string key = GenerateRandomKey();
            Dictionary<String, String> result = new Dictionary<String, String>();
            Note note = new Note
            {
                Contenu = SimpleAES.AES256.Encrypt(data.Contenu, key),
                DateCreation = DateTime.Now,
                DateExpiration = data.DateExpiration,
                NombreVue = data.NombreVue
            };
            
            _context.Add(note);
            _context.SaveChanges();
            result.Add("id", note.Id.ToString());
            result.Add("key", key);
            return Ok(result);
        }

        private static string GenerateRandomKey()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 30);
        }
    }
}
