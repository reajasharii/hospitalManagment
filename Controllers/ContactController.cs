// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using HospitalManagement.Models;
// using System.Threading.Tasks;
// using HospitalManagement.Data;
// using System.Linq;

// namespace HospitalManagement.Controllers
// {
//     public class ContactController : Controller
//     {
//         private readonly ApplicationDbContext _context;

//         public ContactController(ApplicationDbContext context)
//         {
//             _context = context;
//         }

//         // CREATE - GET
//         public IActionResult Create()
//         {
//             return View();
//         }

//         // CREATE - POST
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,PhoneNumber,ReasonForContact,Description")] Contact contact)
//         {
//             if (ModelState.IsValid)
//             {
//                 _context.Add(contact);
//                 await _context.SaveChangesAsync();
//                 return RedirectToAction(nameof(Index));
//             }
//             return View(contact);
//         }

//         // READ - Index
//         public async Task<IActionResult> Index()
//         {
//             return View(await _context.Contact.ToListAsync());
//         }

//         // READ - Details
//         public async Task<IActionResult> Details(int? id)
//         {
//             if (id == null)
//             {
//                 return NotFound();
//             }

//             var contact = await _context.Contact
//                 .FirstOrDefaultAsync(m => m.Id == id);
//             if (contact == null)
//             {
//                 return NotFound();
//             }

//             return View(contact);
//         }

//         // GET: Contact/Delete/5
// public IActionResult Delete(int id)
// {
//     var contact = _context.Contact.FirstOrDefault(c => c.Id == id);
//     if (contact == null)
//         return NotFound();

//     return View(contact);
// }

// // POST: Contact/Delete/5
// [HttpPost, ActionName("Delete")]
// [ValidateAntiForgeryToken]
// public IActionResult DeleteConfirmed(int id)
// {
//     var contact = _context.Contact.FirstOrDefault(c => c.Id == id);
//     if (contact == null)
//         return NotFound();

//     try
//     {
//         _context.Contact.Remove(contact);
//         _context.SaveChanges();
//     }
//     catch (DbUpdateException)
//     {
//         ModelState.AddModelError(string.Empty, "Unable to delete. The contact might be referenced by other entities.");
//         return View(contact);
//     }

//     return RedirectToAction(nameof(Index));
// }

    
//     }
    
// }
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
                // Kërkoni të dhënat e përdoruesit të kyçur për të plotësuar formularin
                var userName = User.Identity.Name;
                // Mund të merrni dhe informacione të tjera nëse janë të disponueshme
            }

            return View();
        }

        // CREATE - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,PhoneNumber,ReasonForContact,Description")] Contact contact)
        {
            if (ModelState.IsValid)
            {
                // Kontrolloni nëse përdoruesi është i kyçur dhe plotësoni të dhënat
                if (User.Identity.IsAuthenticated)
                {
                    contact.FirstName = User.Identity.Name; // Përdorimi i emrit të përdoruesit të kyçur për fushat e formularit
                    // Mund të bëni gjithashtu të ngjashme për email dhe fusha të tjera
                }

                _context.Add(contact);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Your message has been sent successfully!";
                return RedirectToAction(nameof(Create));  // Redirect back to the Create form after successful submission
            }
            return View(contact);
        }

        // READ - Index
        public async Task<IActionResult> Index()
        {
            return View(await _context.Contact.ToListAsync());
        }

        // READ - Details
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

        // GET: Contact/Delete/5
        public IActionResult Delete(int id)
        {
            var contact = _context.Contact.FirstOrDefault(c => c.Id == id);
            if (contact == null)
                return NotFound();

            return View(contact);
        }

        // POST: Contact/Delete/5
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
