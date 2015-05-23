using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;

namespace ASPTask2
{
    public class SiteStatsConfigHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {

            var result = new SiteStatsSettings
            {
                ConnectionString = section.SelectSingleNode("ConnectionString").InnerText,
            };
            var pages = File.ReadAllLines(HttpContext.Current.Server.MapPath(section
                .SelectSingleNode("PathPage").InnerText));
            result.Pages = new string[pages.Length];
            for (int i = 0; i < pages.Length; i++)
                result.Pages[i] = pages[i];
            return result;
        }
    }
}