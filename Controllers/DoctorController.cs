using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize(Roles = "Doctor")]
public class DoctorController : Controller
{
    public IActionResult Dashboard()
    {
        return View(); // Return the Dashboard view for Doctor
    }
}
