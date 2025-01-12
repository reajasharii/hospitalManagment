using System.Linq;
using HospitalManagement.Data;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    public class HospitalStaffsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HospitalStaffsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HospitalStaffs
        public IActionResult Index()
        {
            var staff = _context.HospitalStaffs.ToList();
            return View(staff);
        }

        // GET: HospitalStaffs/Details/5
        public IActionResult Details(int id)
        {
            var staffMember = _context.HospitalStaffs.Find(id);
            if (staffMember == null)
                return NotFound();

            return View(staffMember);
        }

        // GET: HospitalStaffs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HospitalStaffs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(HospitalStaff staff)
        {
            if (ModelState.IsValid)
            {
                _context.Add(staff);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }

        // GET: HospitalStaffs/Edit/5
        public IActionResult Edit(int id)
        {
            var staffMember = _context.HospitalStaffs.Find(id);
            if (staffMember == null)
                return NotFound();

            return View(staffMember);
        }

        // POST: HospitalStaffs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, HospitalStaff staff)
        {
            if (id != staff.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(staff);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }

        // GET: HospitalStaffs/Delete/5
        public IActionResult Delete(int id)
        {
            var staffMember = _context.HospitalStaffs.Find(id);
            if (staffMember == null)
                return NotFound();

            return View(staffMember);
        }

        // POST: HospitalStaffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var staffMember = _context.HospitalStaffs.Find(id);
            _context.HospitalStaffs.Remove(staffMember);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
