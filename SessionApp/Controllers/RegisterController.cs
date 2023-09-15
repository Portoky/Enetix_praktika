using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SessionIdApp.Controllers;
using SessionIdApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SessionApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {

        
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register([FromBody] RegisterModel registerModel)
        {
            HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });//?
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = Constants.mConnStr;
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            MySqlTransaction transaction = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT INTO registered_users (SessionID, Username, Password, EmailAddress, Role, SurName, GivenName, IsActive ) VALUES('" + HttpContext.Session.Id + "', '" + registerModel.UserName  + "', '" +
                    registerModel.Password + "', '" + registerModel.EmailAddress + "', '" + registerModel.Role + "', '" + registerModel.SurName + "', '" + registerModel.GivenName + "', " + 1 + ");";
                cmd.ExecuteNonQuery();
                transaction.Commit();
                Response.Cookies.Append("SessionId", HttpContext.Session.Id);
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
                transaction.Rollback();
                return Ok(e.Message + "\n" + cmd.CommandText);
            }
            finally
            {
                con.Close();
            }
            return Ok(HttpContext.Session.Id + "\n" + cmd.CommandText);

        }

    }
}

