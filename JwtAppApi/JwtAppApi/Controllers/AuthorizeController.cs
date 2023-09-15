using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Text;
using System.Runtime.CompilerServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using static System.Net.WebRequestMethods;
using MySql.Data.MySqlClient;
using System.Data;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Cors;

namespace JwtAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private IConfiguration _config;
        public AuthorizeController(IConfiguration config)
        {
            _config = config;
        }

        [EnableCors("AllowSpecificOrigin")]
        [HttpPost("token")]
        public IActionResult AuthorizeToken([FromBody] string token)
        {
            HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });
            if (string.IsNullOrEmpty(token))     
            {
                if (HttpContext.Request.Cookies[SessionVariables.SessionAccessToken] != null)
                {
                    token = HttpContext.Request.Cookies[SessionVariables.SessionAccessToken];
                }
            }
            if(string.IsNullOrEmpty(token) || !ValidateToken(token))
            {
                return NotFound();
            }
            return Ok("kul");
        }
        [EnableCors("AllowSpecificOrigin")]
        [HttpPost("sessionId")]
        public IActionResult AuthorizeSessionId([FromBody] string sessionId)
        {
            HttpContext.Session.Set("What", new byte[] { 1, 2, 3, 4, 5 });
            if(string.IsNullOrEmpty(sessionId))
            {
                if (HttpContext.Request.Cookies[SessionVariables.SessionKeySessionId] != null)
                {
                    sessionId = HttpContext.Request.Cookies[SessionVariables.SessionKeySessionId];
                }
            }
            if (string.IsNullOrEmpty(sessionId) || !IsSessionIdInDb(sessionId))
            {
                return NotFound();
            }
            return Ok("kul");
        }
        private bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();
            try
            {
                SecurityToken validatedToken;
                IPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                return true;
            }
            catch (SecurityTokenException ex)
            {
                return false;
            }
        }
        private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])) // The same key as the one that generate the token
            };
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
