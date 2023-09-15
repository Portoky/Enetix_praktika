using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SessionApp.Authorization;
using SessionIdApp.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SessionApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetIdController : ControllerBase
    {
        [HttpGet("hi")]
        public IActionResult Index()
        {
            HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });
            if (isAuthorized())//gets the current session id and checks if its in the db
            {
                return Ok(HttpContext.Session.Id);
            }
            else
            {
                return RedirectToAction("GetId");
            }
        }

        [AllowAnonymous]
        [HttpGet("Public")]
        public async Task<IActionResult> GetId()
        {
            HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });
            string url = "https://localhost:44379/api/cookie";
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string sessionId = await response.Content.ReadAsStringAsync();
                    return Ok(sessionId);
                }
                else
                {
                    return NotFound("NOOOO");
                }
            }
        }

        [AllowAnonymous]
        [HttpGet("Public2")]
        public IActionResult GetAnotherId()
        {
            return Ok(HttpContext.Session.Id);
        }

        public bool isAuthorized()
        {
            string sessionId = HttpContext.Session.Id;
            return IsSessionIdInDb(sessionId);
        }
        protected bool IsSessionIdInDb(string sessionId)
        {
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = Constants.mConnStr;
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            MySqlTransaction transaction = con.BeginTransaction();
            cmd.CommandText = "GetUserBySessionId";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            try
            {
                MySqlParameter[] pms = new MySqlParameter[2];

                pms[0] = new MySqlParameter("@p_SessionId", MySqlDbType.VarChar, 50);
                pms[0].Value = sessionId;
                pms[0].Direction = ParameterDirection.Input;

                pms[1] = new MySqlParameter("@p_UserName", MySqlDbType.VarChar, 50);
                pms[1].Direction = ParameterDirection.Output;

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
            string userNameFromDb = Convert.ToString(cmd.Parameters["@p_UserName"].Value);
            if (userNameFromDb != "")
            {
                return true;
            }
            return false;
        }
    }
}
