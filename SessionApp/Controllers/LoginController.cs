using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JwtApp.Models;
using MySql.Data.MySqlClient;
using SessionIdApp.Controllers;
using System.Data;
using System.Security.Claims;

namespace SessionApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            UserModel user = Authenticate(userLogin);
            if (user != null)
            {
                user.SessionId = HttpContext.Session.Id;
                SaveSessionIdToDb(user);
                HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 }); //so session id is consistent
                Response.Cookies.Append("SessionId", HttpContext.Session.Id, new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(5)
                });
                Response.Cookies.Append("UserName", user.UserName, new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(5)
                });
                //saving some information about the user in claims
                var claims = new List<Claim>
                {
                    new Claim("SessionId", user.SessionId),
                    new Claim("UserName", user.UserName)
                };

                var identity = new ClaimsIdentity(claims, "Authenticated");
                var principal = new ClaimsPrincipal(identity);
                HttpContext.User = principal;
                return Ok(user.SessionId + "\n" + HttpContext.Session.Id);
            }
            return NotFound("User not found");
        }

        private void SaveSessionIdToDb(UserModel user)
        {
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
                pms[1].Value = user.SessionId;
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

        private UserModel Authenticate(UserLogin userLogin)
        {
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

                user.SessionId = Convert.ToString(cmd.Parameters["@p_SessionId"].Value);
                if (user.SessionId == null)//it means it did not find the user
                    return null;
                //else wecan extract all the data
                user.EmailAddress = Convert.ToString(cmd.Parameters["@p_EmailAddress"].Value);
                user.Role = Convert.ToString(cmd.Parameters["@p_Role"].Value);
                user.SurName = Convert.ToString(cmd.Parameters["@p_SurName"].Value);
                user.GivenName = Convert.ToString(cmd.Parameters["@p_GivenName"].Value);

            }
            catch (MySqlException e)
            {
                transaction.Rollback();
                Console.WriteLine("User not found!\n", e);
            }
            finally
            {
                con.Close();
            }
            return user;
        }
    }
}
