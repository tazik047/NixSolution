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
            Application["count"] = 0;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            int c = (int)Application["count"];
            Debug.WriteLine(c);
            Application["count"] = c + 1;
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}