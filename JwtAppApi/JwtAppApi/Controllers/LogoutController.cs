using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace JwtAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        [EnableCors("AllowSpecificOrigin")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });
            if (HttpContext.Request.Cookies[SessionVariables.SessionKeySessionId] != null)
            {
                HttpContext.Response.Cookies.Delete(SessionVariables.SessionKeySessionId, new CookieOptions()
                {
                    Secure = true,
                    IsEssential = true,
                    HttpOnly = true
                });
            }
            if (HttpContext.Request.Cookies[SessionVariables.SessionAccessToken] != null)
            {
                HttpContext.Response.Cookies.Delete(SessionVariables.SessionAccessToken, new CookieOptions()
                {
                    Secure = true,
                    IsEssential = true,
                    HttpOnly = true
                });
            }
            if (HttpContext.Request.Cookies[SessionVariables.SessionKeyUsername] != null)
            {
                HttpContext.Response.Cookies.Delete(SessionVariables.SessionKeyUsername, new CookieOptions()
                {
                    Secure = true,
                    IsEssential = true,
                    HttpOnly = true
                });
            }
            if (HttpContext.Request.Cookies[SessionVariables.Role] != null)
            {
                HttpContext.Response.Cookies.Delete(SessionVariables.Role, new CookieOptions()
                {
                    Secure = true,
                    IsEssential = true,
                    HttpOnly = true
                });
            }
            return Ok("Logged out");
        }
    }
}
