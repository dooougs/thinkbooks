using System.Web;
using System.Web.Mvc;

namespace ThinkBooksWebsiteValidationTest
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
