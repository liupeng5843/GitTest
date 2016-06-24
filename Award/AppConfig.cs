using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimpleComm.Helper;

namespace MHD.Original.Award
{
    public class AppConfig
    {
        public static string OriginalHost { get; set; }

        static AppConfig()
        {
            OriginalHost = ConfigHelper.GetAppSetting("orgianlHost");
        }
    }
}