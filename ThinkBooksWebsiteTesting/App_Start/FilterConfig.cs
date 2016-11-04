using System.Web;
using System.Web.Mvc;

namespace ThinkBooksWebsiteTesting
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
