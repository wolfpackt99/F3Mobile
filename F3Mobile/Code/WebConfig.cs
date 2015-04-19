using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace F3Mobile.Code
{
    public static class WebConfig
    {
        public static string MailChimpUrl
        {
            get { return ConfigurationManager.AppSettings.Get("MailChimpUrl") ?? string.Empty; }
        }

    }
}