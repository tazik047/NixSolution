using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace ASPTask2
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            StatsCounter.Load();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            StatsCounter.OpenNewSession();
            Session["LastAccess"] = DateTime.Now.ToShortDateString();
        }

        void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            /*Response.Write(Request.UserAgent + "<br />");
            Response.Write(Request.UserHostAddress + "<br />");
            Response.Write(Request.UserHostName + "<br />");
            Response.Write(Request.Path + "<br />");*/
            StatsCounter.IncRequest();
            if (HttpContext.Current.Session == null) return;
            string s = Session["LastAccess"].ToString();
            if (s.Equals(DateTime.Now.ToShortDateString())) return;
            Session_Start(sender, e);
        } 

        protected void Application_End(object sender, EventArgs e)
        {
            StatsCounter.Save();
        }
    }
}