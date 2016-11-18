using System.Web.Mvc;

namespace ThinkBooksWebsiteAuthTest.Controllers
{
    // ThinkBooks Role
    public static class TR
    {
        public static string All = "QNRL\\All Quorum";
        public static string Dev = "QNRL\\Developers";
        public static string Admin = "QNRL\\Administrators";
    }

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            // Specifices which roles need to be in to run this method
            if (User.IsInRole(TR.Dev) || User.IsInRole(TR.Admin)) { }
            else return RedirectToAction("Unauthorized", "Errors", null);

            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            // Specifices which roles need to be in to run this method
            if (User.IsInRole(TR.Admin)) { }
            else return RedirectToAction("Unauthorized", "Errors", null);

            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}