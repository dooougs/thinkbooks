using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace ThinkBooksWebsiteAuthTest.Controllers
{
    // ThinkBooks Role
    public static class TR
    {
        public const string All = "QNRL\\All Quorum";
        public const string Dev = "QNRL\\Developers";
        public const string Admin = "QNRL\\Administrators";
        public const string or = ",";

        public static bool AuthOn = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["AuthOn"]);
    }

    [Authorize]
    public class TBController : Controller { }

    public class HomeController : TBController
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = TR.Dev+TR.or+TR.Admin )]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize(Roles = TR.Admin)]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (TR.AuthOn)
            {
                bool pass = false;

                TBController ctrl = filterContext.Controller as TBController;
                if (Roles != String.Empty)
                {
                    foreach (var role in Roles.Split(TR.or.ToCharArray()))
                    {
                        if (pass = ctrl.User.IsInRole(role)) // deliberate assignment
                            break;
                    }
                }
                else
                {
                    pass = ctrl.User.IsInRole(TR.All);
                }
                if (!pass)
                {
                    filterContext.Result = new RedirectToRouteResult(
                                            new RouteValueDictionary
                                            {
                                            { "action", "Unauthorized" },
                                            { "controller", "Errors" }
                                            });
                    return;
                }
                base.OnAuthorization(filterContext);
            }

        }
        protected override void HandleUnauthorizedRequest(System.Web.Mvc.AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                filterContext.Result = new System.Web.Mvc.HttpStatusCodeResult((int)System.Net.HttpStatusCode.Forbidden);
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}