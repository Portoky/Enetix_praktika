using JwtApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace JwtApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        [HttpGet("Protected")]

        public IActionResult Protected()
        {

            /*HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });
            string token = HttpContext.Request.Query["token"].ToString();
            if(string.IsNullOrEmpty(token) && HttpContext.Request.Cookies["token"] == null) //unauthorized
            {
                //return RedirectToPage("/401.cshtml");
                return View("Pages/errors/401.cshtml");
            }
            if(!string.IsNullOrEmpty(token) ) 
            {
                HttpContext.Response.Cookies.Append(SessionVariables.SessionAccessToken, token, new CookieOptions 
                {
                    Expires = DateTime.Now.AddMinutes(15),
                    IsEssential = true,
                    Secure = true
                });
            }*/
            HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });
            return View("Pages/User/Protected.cshtml");
        }

        [HttpGet("Public")]
        public IActionResult Public()
        {
            HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });
            return View("Pages/User/Public.cshtml");
        }

        [HttpGet("Admins")]
        public IActionResult AdminsEndpoint()
        {
            
            var currentUser = GetCurrentUser();
            if (currentUser.Role != "Admin")
                return Unauthorized();
            return Ok($" you are an {currentUser.Role}");
        }
        private UserModel GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;
                return new UserModel
                {
                    UserName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                    EmailAddress = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                    GivenName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.GivenName)?.Value,
                    SurName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Surname)?.Value,
                    Role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value
                };
            }
            return null;
        }
    }
}
