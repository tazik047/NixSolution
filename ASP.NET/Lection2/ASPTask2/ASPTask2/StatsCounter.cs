﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Xml.Linq;

namespace ASPTask2
{
    public static class StatsCounter
    {
        private static string _path
        {
            //get { return System.IO.Path.Combine(Environment.CurrentDirectory, "Stats.xml"); }
            get { return HttpContext.Current.Server.MapPath("Stats.xml"); }
        }

        private static XDocument xml;

        private const string countRequest = "TotalRequest";

        private const string countRequestToday = "RequestToday";

        private const string countUniqueUsers = "TotalUsers";

        private const string uniqueUsersData = "TotalUsersInfo";

        private const string countUniqueUsersToday = "UsersToday";

        private const string uniqueUsersTodayData = "UsersTodayInfo";

        private const string currentDay = "CurrentDay";

        public static void Save()
        {
            var application = HttpContext.Current.Application;
            updateElement(countRequest,"count",application[countRequest]);
            updateElement(countRequestToday, "count", application[countRequestToday]);
            updateElement(countUniqueUsers, "count", application[countUniqueUsers]);
            updateElement(countUniqueUsersToday, "count", application[countUniqueUsersToday]);
            updateElement(currentDay, "date", application[currentDay]);

            var settings = WebConfigurationManager.GetSection("siteStats") as SiteStatsSettings;
            if(xml.Root.Element("Pages")!=null)
                xml.Root.Element("Pages").Remove();
            foreach (var page in settings.Pages)
                xml.Add(new XElement("Pages"),
                    new XAttribute("name", page),
                    new XAttribute("count", application[page]));
            xml.Save(_path);
        }

        private static void updateElement(string key, string nameAttribute, object value)
        {
            var element = xml.Root.Element(key);
            if (element == null)
                xml.Root.Add(new XElement(key,
                    new XAttribute(nameAttribute, value)));
            else
                element.Attribute(nameAttribute).Value = value.ToString();
        }

        public static void Load()
        {
            xml = XDocument.Load(_path);
            var application = HttpContext.Current.Application;

            XElement element = xml.Root.Element(countRequest);
            loadElement(application, element, countRequest);

            element = xml.Root.Element(countRequestToday);
            loadElement(application, element, countRequestToday);

            element = xml.Root.Element(countUniqueUsers);
            loadElement(application, element, countUniqueUsers);

            element = xml.Root.Element(countUniqueUsersToday);
            loadElement(application, element, countUniqueUsersToday);

            element = xml.Root.Element(currentDay);
            application[currentDay] = element == null ? DateTime.Now : DateTime.Parse(element.Attribute("date").Value);

            var pagesXml = xml.Elements("Pages");
            var settings = WebConfigurationManager.GetSection("siteStats") as SiteStatsSettings;
            foreach (var page in settings.Pages)
            {
                element = pagesXml.FirstOrDefault(x => page == x.Attribute("name").Value);
                loadElement(application, element, page);
            }
            checkNextDay(application);
        }

        private static void loadElement(HttpApplicationState application, XElement element, string key)
        {
            application[key] = element == null ? 0 : Convert.ToInt32(element.Attribute("count").Value);
        }

        public static void OpenNewSession()
        {
            var request = HttpContext.Current.Request;
            var application = HttpContext.Current.Application;
            checkNextDay(application);
            if (isUnique(uniqueUsersData, request))
            {
                incValue(application, countUniqueUsers);
                addUserElement(uniqueUsersData, request);
            }
            if (isUnique(uniqueUsersTodayData, request))
            {
                incValue(application, countUniqueUsersToday);
                addUserElement(uniqueUsersTodayData, request);
            }
        }

        private static bool isUnique(string key, HttpRequest request)
        {
            var element = xml.Root.Element(key);
            if (element == null)
            {
                xml.Root.Add(new XElement(key));
                return true;
            }
            return element.Elements().FirstOrDefault(e =>
                e.Attribute("ip").Value.Equals(request.UserHostAddress) &&
                e.Attribute("dns").Value.Equals(request.UserHostName) &&
                e.Attribute("browser").Value.Equals(request.UserAgent)) == null;
        }

        private static void addUserElement(string key, HttpRequest request)
        {
            var t = xml.Root.Element(key);
            t.Add(new XElement("User",
                    new XAttribute("ip", request.UserHostAddress),
                    new XAttribute("dns", request.UserHostName),
                    new XAttribute("browser", request.UserAgent)));
        }

        public static void IncRequest()
        {
            var application = HttpContext.Current.Application;
            checkNextDay(application);
            incValue(application, countRequest);
            incValue(application, countRequestToday);

            var currentPage = HttpContext.Current.Request.Path;
            var settings = WebConfigurationManager.GetSection("siteStats") as SiteStatsSettings;
            if (settings.Pages.Contains(currentPage.ToLower()))
                incValue(application, currentPage);
        }

        private static void incValue(HttpApplicationState application, string key)
        {
            int c = (int)application[key];
            application[key] = c + 1;
        }

        private static void checkNextDay(HttpApplicationState application)
        {
            var appDate = (DateTime)application[currentDay];
            if (DateTime.Now.ToShortDateString().Equals(appDate.ToShortDateString())) return;
            application[countRequestToday] = 0;
            application[countUniqueUsersToday] = 0;
            xml.Root.Element(uniqueUsersTodayData).Remove();
            application[currentDay] = DateTime.Now;
        }

        public static string GetStats()
        {
            var application = HttpContext.Current.Application;
            StringBuilder sb = new StringBuilder("<div>");
            const string format = "<div class=\"item\">{0} - <i>{1}</i></div>";
            sb.AppendFormat(format, "Общее количество запросов", application[countRequest]);
            sb.AppendFormat(format, "Количество запросов за сегодня", application[countRequestToday]);
            sb.AppendFormat(format, "Общее количество уникальных пользователей", application[countUniqueUsers]);
            sb.AppendFormat(format, "Количество уникальных пользователей за сегодня", application[countUniqueUsersToday]);
            var settings = WebConfigurationManager.GetSection("siteStats") as SiteStatsSettings;
            sb.Append("<div class=\"item\">Статистика по страницам:");
            string appUrl = HttpContext.Current.Request.ApplicationPath.Substring(0,
                HttpContext.Current.Request.ApplicationPath.Length - 1);
            foreach (var page in settings.Pages)
            {
                sb.AppendFormat("<div class = \"sub-item\"><a href = \"{2}{0}\">{0}</a> - запросов <i>{1}</i></div>", 
                    page, application[page], appUrl);
            }
            sb.Append("</div></div>");
            return sb.ToString();
        }
    }
}