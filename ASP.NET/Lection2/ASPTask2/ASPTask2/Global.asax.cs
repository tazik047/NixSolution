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

        void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (Session["LastAccess"].Equals(DateTime.Now.ToShortDateString())) return;
            Session_Start(sender, e);
        }

        void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            //SiteStatsBLL.Request(HttpContext.Current);
            Response.Write(Request.UserAgent + "<br />");
            Response.Write(Request.UserHostAddress + "<br />");
            Response.Write(Request.UserHostName + "<br />");
            Response.Write(Request.Path + "<br />");
            if(!Session["LastAccess"].Equals(DateTime.Now.ToShortDateString()))
                StatsCounter.OpenNewSession();
            StatsCounter.IncRequest();
        } 

        protected void Application_End(object sender, EventArgs e)
        {
            StatsCounter.Save();
        }
    }
}