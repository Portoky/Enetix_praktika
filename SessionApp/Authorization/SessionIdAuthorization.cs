using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MySql.Data.MySqlClient;
using SessionIdApp.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;



namespace SessionApp.Authorization
{
    public class SessionIdAuthorization : ControllerBase /*: AuthorizationHandler<SessionIdRequirement>*/
    {
        //protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SessionIdRequirement requirement)
        //{               
        //    var authFilterContext = context.Resource as AuthorizationFilterContext;
        //    var sessionId = authFilterContext.HttpContext.Request.Cookies["SessionId"];
        //    var userName = authFilterContext.HttpContext.Request.Cookies["UserName"];
        //    if(sessionId != null && userName != null && IsSessionIdInDb(sessionId, userName))
        //    {
        //        context.Succeed(requirement);
        //    }
        //    return Task.FromResult(0);
        //}
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
            if (userNameFromDb != null)
            {
                return true;
            }
            return false;
        }
    }
}
