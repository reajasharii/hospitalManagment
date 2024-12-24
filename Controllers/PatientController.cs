using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Patient")]
public class PatientController : Controller
{
    public IActionResult Dashboard()
    {
        return View(); // Return the Dashboard view for Patient
    }
}
