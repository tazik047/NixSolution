using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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
                Pages = File.ReadAllLines(HttpContext.Current.Server.MapPath(section
                    .SelectSingleNode("PathPage").InnerText)).Select(s => s.ToLower()).ToArray(),
            };
            /*var pages = File.ReadAllLines(HttpContext.Current.Server.MapPath(section
                .SelectSingleNode("PathPage").InnerText));
            result.Pages = new string[pages.Length];
            for (int i = 0; i < pages.Length; i++)
                result.Pages[i] = pages[i].ToLower();*/
            return result;
        }
    }
}