using HospitalManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace HospitalManagement.Controllers
{
    public class ContactController : Controller
    {
        // Veprimi GET për shfaqjen e formularit
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Veprimi POST për dërgimin e formularit
        [HttpPost]
        public IActionResult Index(Contact contact)
        {
            if (ModelState.IsValid)
            {
                // Printimi për debug ose ruajtja në bazën e të dhënave
                Console.WriteLine($"First Name: {contact.FirstName}");
                Console.WriteLine($"Last Name: {contact.LastName}");
                Console.WriteLine($"Email: {contact.Email}");
                Console.WriteLine($"Phone Number: {contact.PhoneNumber}");
                Console.WriteLine($"Reason: {contact.ReasonForContact}");
                Console.WriteLine($"Message: {contact.Description}");

                // Mesazh i suksesshëm
                TempData["SuccessMessage"] = "Thank you for contacting us. We will get back to you soon.";

                return RedirectToAction("Index");
            }

            return View(contact);
        }
    }
}
