using ShiJu.Service.Models;
using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WindowsServer.Configuration;
using WindowsServer.Log;
using WindowsServer.Web;

namespace ShiJu.Service.Controllers
{
    public class AppController : Controller
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        [Route("App/About")]
        public ActionResult GetAbout()
        {
            using(var dbContext = Heart.CreateShiJuDbContext())
            {
                var aboutJson = AppDAO.GetPreferenceValue(dbContext, "about");
                if (string.IsNullOrEmpty(aboutJson))
                {
                    aboutJson = "{}";
                }
                return this.JsonResponseJson(0, "About", aboutJson);
            }
        }

        [Route("App/Versions")]
        public ActionResult GetVersions()
        {
            using (var dbContext = Heart.CreateShiJuDbContext())
            {
                var versionsJson = AppDAO.GetPreferenceValue(dbContext, "versions");
                if (string.IsNullOrEmpty(versionsJson))
                {
                    versionsJson = "{}";
                }
                return this.JsonResponseJson(0, "Versions", versionsJson);
            }
        }

        [HttpPost]
        [Route("App/Feedbacks/Send")]
        public ActionResult SendFeedback()
        {
            var json = this.GetRequestContentJson() as JsonObject;
            var text = json.GetStringValue("Text");

            using(var smtpClient = MailUtility.CreateSmtpClientFromConfiguration(Heart.FeedbackSmtpClientConfiguration))
            {
                var mail = new MailMessage(ConfigurationCenter.Global["FeedbackMailFrom"], ConfigurationCenter.Global["FeedbackMailTo"]);
                mail.Subject = "时聚意见反馈";
                mail.Body = text;
                mail.BodyEncoding = Encoding.UTF8;
                smtpClient.Send(mail);
            }

            return this.JsonResult(0, "Feedback has been sent.");
        }

    }

}