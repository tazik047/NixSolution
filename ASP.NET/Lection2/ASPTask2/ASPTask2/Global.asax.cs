﻿using System;
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
        /// <summary>
        /// Событие начала работы приложения
        /// </summary>
        protected void Application_Start(object sender, EventArgs e)
        {
            StatsCounter.Load();
        }

        /// <summary>
        /// Событие, возникающее когда пользователь первый раз посетил сайт.
        /// </summary>
        protected void Session_Start(object sender, EventArgs e)
        {
            StatsCounter.OpenNewSession();
            Session["LastAccess"] = DateTime.Now.ToShortDateString(); // сохраняем дату создания сессии
        }

        /// <summary>
        /// Событие, возникающее после завершения выполнения обработчика событий приложения ASP.NET
        /// </summary>
        void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            StatsCounter.IncRequest();
            if (HttpContext.Current.Session == null) return; //проверяем существование сессии
            string s = Session["LastAccess"].ToString();
            if (!s.Equals(DateTime.Now.ToShortDateString())) // если дата создания сессии не равна текущей дате
                Session_Start(sender, e);
        } 
        
        /// <summary>
        /// Событие возникающее после обработки запроса.
        /// </summary>
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            StatsCounter.Save();
        }
    }
}