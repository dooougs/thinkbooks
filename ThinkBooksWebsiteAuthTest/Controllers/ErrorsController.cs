using System.Web.Mvc;

namespace ThinkBooksWebsiteAuthTest.Controllers
{
    public class ErrorsController : Controller
    {
        public ActionResult Unauthorized()
        {
            return View();
        }
    }
}