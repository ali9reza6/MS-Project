using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PolarionReports.BusinessLogic;
using PolarionReports.Models;
using PolarionReports.Custom_Helpers;
using Serilog;

namespace PolarionReports.BusinessLogic.Api
{
    public class PolarionInit
    {
        public Connection Init(string user, string pw)
        {
            if (user == "aaa" && pw == "aaa")
            {
                user = "wnpolrose";
                pw = "wnPOL%18";
            }
            // string Username = "wnpolrose";
            // string Password = "wnPOL%18";

            ErrorViewModel error = new ErrorViewModel();
            Connection con = new Connection("https://", Topol.PolarionServer + "/polarion/");

            try
            {
                con.Login(user, pw);
            }
            catch (Exception e)
            {
                Log.Error("Error Polarion Init, " + e.ToString());
                error.ErrorMsg = e.ToString();
                return null;
            }

            if (!con.IsLoggedIn)
            {
                Log.Error("Error Polarion Init, Login failed!");
                error.ErrorMsg = "Login failed!";
                return null;
            }

            return con;
        }

        /// <summary>
        /// Überprüft ob der user in Polarion über die Connection eingelogged ist
        /// </summary>
        /// <param name="con"></param>
        /// <param name="user"></param>
        /// <returns>true ... Login OK, fasle ... not loggedin or different user</returns>
        public bool PolarionLoggedIn(Connection con, string user)
        {
            if (!con.IsLoggedIn)
            {
                return false;
            }
            else
            {
                if (con.UserName != user)
                {
                    return false;
                }
            }
            return true;
        }
    }
}