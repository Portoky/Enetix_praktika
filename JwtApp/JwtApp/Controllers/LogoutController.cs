using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Data;

namespace JwtApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult Logout()
        {
            Response.Cookies.Delete(SessionVariables.SessionAccessToken);
            Response.Cookies.Delete(SessionVariables.SessionKeySessionId);
            Response.Cookies.Delete(SessionVariables.SessionKeyUsername);
            RemoveSessionIDFromDatabase();
            Constant.sessionId = "";
            HttpContext.Session.Clear();
            return Ok("Logged out");
        }

        private void RemoveSessionIDFromDatabase()
        {
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = Constant.mConnStr;
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            MySqlTransaction transaction = con.BeginTransaction();
            try
            {
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM sessions WHERE SessionID = '" + HttpContext.Session.GetString(SessionVariables.SessionKeySessionId) + "';";
                cmd.ExecuteNonQuery();
                transaction.Commit();
            }
            catch (MySqlException e)
            {
                Console.WriteLine(e.Message);
                transaction.Rollback();
            }
            finally
            {
                con.Close();
            }
        }
    }
}