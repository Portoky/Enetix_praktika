using Microsoft.AspNetCore.Mvc;

namespace SessionApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        [HttpGet("Protected")]

        public IActionResult Protected()
        {
            HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });
            return View("Pages/User/Protected.cshtml");
        }

        [HttpGet("Public")]
        public IActionResult Public()
        {
            HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });
            return View("Pages/User/Public.cshtml");
        }
    }
}
