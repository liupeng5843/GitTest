using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WindowsServer.Log;


namespace ShiJu.Service.Controllers
{
    public class SettingsController : ApiController
    {
        // GET: Setting
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Route("Settings/Versions")]
        public HttpResponseMessage GetVersions()
        {
            var versionsJsonString = File.ReadAllText(HttpContext.Current.Server.MapPath("~/Versions.json"));
            return this.JsonResponseJson(0, "Succeeded", "Versions", versionsJsonString);
        }

        [Route("Settings/PublishAnnouncement")]
        public HttpResponseMessage GetPublishAnnouncement()
        {
            var json = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/PublishAnnouncement.json"));
            //var resultString = HttpUtility.UrlDecode(json);
            return this.JsonResponseJson(0, "PublishAnnouncement", json);
        }


        [Route("Settings/MerchantRegisterAnnouncement")]
        public HttpResponseMessage GetMerchantRegisterAnnouncement()
        {
            var json = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/MerchantRegisterAnnouncement.json"));
            //var resultString = HttpUtility.UrlDecode(json);
            return this.JsonResponseJson(0, "MerchantRegisterAnnouncement", json);
        }


        [Route("Settings/HelpAnnouncement")]
        public HttpResponseMessage GetHelpAnnouncement()
        {
            var json = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/HelpAnnouncement.json"));
            //var resultString = HttpUtility.UrlDecode(json);
            return this.JsonResponseJson(0, "HelpAnnouncement", json);
        }


        [Route("Settings/AboutAnnouncement")]
        public HttpResponseMessage GetAboutAnnouncement()
        {
            var json = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/AboutAnnouncement.json"));
            var resultString = HttpUtility.UrlDecode(json);
            return this.JsonResponseJson(0, "AboutAnnouncement", resultString);
        }
    }
}