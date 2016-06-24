using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MHD.Original.Award.Controllsers;
using MHD.Original.Award.Services;
using MHD.Original.Award.ViewModels;
using SimpleComm.Files;

namespace MHD.Original.Award.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/
        private UserService userService = new UserService();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult News()
        {
            return View();
        }

        public ActionResult Content()
        {
            return View();
        }

        public ActionResult Faq()
        {
            return View();
        }

        [OutputCache(Duration=5)]
        public ActionResult Vote()
        {
            if (HttpContext.Request.Browser.IsMobileDevice)
            {
                return RedirectToAction("M_Vote", "ComicBooks");
            }

            return View();
        }

        public ActionResult Except()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult Header()
        {
            HeaderViewModel model = new HeaderViewModel();
            try
            {
                var originalIdentity = HttpContext.User.Identity as OriginalIdentity;
                if (originalIdentity != null && originalIdentity.IsAuthenticated)
                {
                    var author_info = userService.getAuthorById(originalIdentity.UserData.Id);
                    if (author_info != null)
                    {
                        model.Name = string.IsNullOrEmpty(author_info.authorname) ? author_info.email : author_info.authorname;
                        model.IsLogOn = true;
                        model.CoverUrl = new Uri(new Uri(AppConfig.OriginalHost), author_info.coverurl).ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteCustomLog("Header", ex.ToString());
            }
            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult Footer()
        {
            return View();
        }
    }
}
