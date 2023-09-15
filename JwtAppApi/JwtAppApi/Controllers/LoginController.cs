using JwtAppApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Web.Helpers;
using System.Text.Json;
using Microsoft.AspNetCore.Cors;
using System.Net;

namespace JwtAppApi.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private IConfiguration _config;
        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });

            var user = Authenticate(userLogin);
            if (user != null)
            {
                SaveSessionIdToDb(user);
                var token = GenerateToken(user);
                List<string> sessionInfo = GenerateSessionInfo(user, token);
                SaveCookies();
                string referringUrl = HttpContext.Session.GetString(SessionVariables.ReferringUrl);
                return Ok(referringUrl);
            }

            return NotFound("User Not Found");
        }

        [HttpGet]
        public IActionResult InputCredentialsToken()
        {
            HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });

            HttpContext.Session.SetString(SessionVariables.ReferringUrl, HttpContext.Request.Query["referringUrl"].ToString());
            return View("Pages/Login/Login.cshtml");
        }
        private string GenerateToken(UserModel user)
        {
            HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.GivenName, user.GivenName),
                new Claim(ClaimTypes.Surname, user.SurName),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: credentials);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }

        private List<string> GenerateSessionInfo(UserModel user, string token)
        {
            HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });
            List<string> sessionInfo = new List<string>();
            //we access our session through httpcontext
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SessionVariables.SessionKeyUsername)))
            {
                HttpContext.Session.SetString(SessionVariables.SessionKeyUsername, user.UserName);
                HttpContext.Session.SetString(SessionVariables.SessionKeySessionId, HttpContext.Session.Id);
            }
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SessionVariables.SessionAccessToken)))
            {
                HttpContext.Session.SetString(SessionVariables.SessionAccessToken, token.ToString());
            }
            if(string.IsNullOrWhiteSpace(HttpContext.Session.GetString(SessionVariables.Role)))
            {
                HttpContext.Session.SetString(SessionVariables.Role, user.Role);
            }
            var username = HttpContext.Session.GetString(SessionVariables.SessionKeyUsername);
            var sessionId = HttpContext.Session.GetString(SessionVariables.SessionKeySessionId);

            sessionInfo.Add(username);
            sessionInfo.Add(sessionId);

            return sessionInfo;
        }

        private UserModel Authenticate(UserLogin userLogin)
        {
            HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = Constants.mConnStr;
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            MySqlTransaction transaction = con.BeginTransaction();
            cmd.CommandText = "UserLogin";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            UserModel user = new UserModel();
            user.UserName = userLogin.UserName;
            user.Password = userLogin.Password;
            try
            {
                MySqlParameter[] pms = new MySqlParameter[7];
                pms[0] = new MySqlParameter("@p_UserName", MySqlDbType.VarChar, 50);
                pms[0].Value = user.UserName;
                pms[0].Direction = ParameterDirection.InputOutput;

                pms[1] = new MySqlParameter("@p_Password", MySqlDbType.VarChar, 50);
                pms[1].Value = user.Password;
                pms[1].Direction = ParameterDirection.InputOutput;

                pms[2] = new MySqlParameter("@p_SessionId", MySqlDbType.VarChar, 50);
                pms[2].Direction = ParameterDirection.Output;

                pms[3] = new MySqlParameter("@p_EmailAddress", MySqlDbType.VarChar, 50);
                pms[3].Direction = ParameterDirection.Output;

                pms[4] = new MySqlParameter("@p_Role", MySqlDbType.VarChar, 50);
                pms[4].Direction = ParameterDirection.Output;

                pms[5] = new MySqlParameter("@p_SurName", MySqlDbType.VarChar, 50);
                pms[5].Direction = ParameterDirection.Output;

                pms[6] = new MySqlParameter("@p_GivenName", MySqlDbType.VarChar, 50);
                pms[6].Direction = ParameterDirection.Output;

                cmd.Parameters.AddRange(pms);
                cmd.ExecuteNonQuery();
                transaction.Commit();
                //else wecan extract all the data
                user.EmailAddress = Convert.ToString(cmd.Parameters["@p_EmailAddress"].Value);
                if (user.EmailAddress == "")
                    return null;
                user.Role = Convert.ToString(cmd.Parameters["@p_Role"].Value);
                user.SurName = Convert.ToString(cmd.Parameters["@p_SurName"].Value);
                user.GivenName = Convert.ToString(cmd.Parameters["@p_GivenName"].Value);
                
            }
            catch (MySqlException e)
            {
                transaction.Rollback();
                Console.WriteLine("User not found!\n", e);
                user = null;
            }
            finally

            {
                con.Close();
                
            }
            return user;
        }
        private void SaveCookies()
        {
            HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });

            HttpContext.Response.Cookies.Append(SessionVariables.SessionKeySessionId, HttpContext.Session.GetString(SessionVariables.SessionKeySessionId), new CookieOptions
            {
                IsEssential = true,
                Secure = true,
                HttpOnly = true
            });
            HttpContext.Response.Cookies.Append(SessionVariables.SessionAccessToken, HttpContext.Session.GetString(SessionVariables.SessionAccessToken), new CookieOptions
            {
                IsEssential = true,
                Secure = true,
                HttpOnly = true
            });
            HttpContext.Response.Cookies.Append(SessionVariables.SessionKeyUsername, HttpContext.Session.GetString(SessionVariables.SessionKeyUsername), new CookieOptions
            {
                IsEssential = true,
                Secure = true,
                HttpOnly = true
            });
            HttpContext.Response.Cookies.Append(SessionVariables.Role, HttpContext.Session.GetString(SessionVariables.Role), new CookieOptions
            {
                IsEssential = true,
                Secure = true,
                HttpOnly = true
            });
        }
        private void SaveSessionIdToDb(UserModel user)
        {
            HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = Constants.mConnStr;
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            MySqlTransaction transaction = con.BeginTransaction();
            cmd.CommandText = "SaveSessionId";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                MySqlParameter[] pms = new MySqlParameter[2];

                pms[0] = new MySqlParameter("@p_UserName", MySqlDbType.VarChar, 50);
                pms[0].Value = user.UserName;
                pms[0].Direction = ParameterDirection.Input;

                pms[1] = new MySqlParameter("@p_sessionId", MySqlDbType.VarChar, 50);
                pms[1].Value = HttpContext.Session.Id;
                pms[1].Direction = ParameterDirection.Input;

                cmd.Parameters.AddRange(pms);
                cmd.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (MySqlException e)
            {
                transaction.Rollback();
                Console.WriteLine("Session Id could not be saved", e);
            }
            finally
            {
                con.Close();
            }
        }
    }
}
