using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Models;
using HospitalManagement.Data;
using System.Threading.Tasks;
using System.Linq;

namespace HospitalManagement.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE - GET
        public IActionResult Create()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userName = User.Identity.Name;

            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,PhoneNumber,ReasonForContact,Description")] Contact contact)
        {
            if (ModelState.IsValid)
            {

                if (User.Identity.IsAuthenticated)
                {
                    contact.FirstName = User.Identity.Name;

                }

                _context.Add(contact);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your message has been sent successfully!";
                return RedirectToAction(nameof(Create));
            }
            return View(contact);
        }


        public async Task<IActionResult> Index()
        {
            return View(await _context.Contact.ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contact
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }


        public IActionResult Delete(int id)
        {
            var contact = _context.Contact.FirstOrDefault(c => c.Id == id);
            if (contact == null)
                return NotFound();

            return View(contact);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var contact = _context.Contact.FirstOrDefault(c => c.Id == id);
            if (contact == null)
                return NotFound();

            try
            {
                _context.Contact.Remove(contact);
                _context.SaveChanges();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Unable to delete. The contact might be referenced by other entities.");
                return View(contact);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
