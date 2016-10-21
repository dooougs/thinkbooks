using System.Web.Mvc;

namespace ThinkBooksWebsite.Services.HtmlHelper
{
    public static class Util
    {
        public static MvcHtmlString SetSelected(string input, string match)
        {
            string x = "";
            if (match == input) x = "selected=\"selected\"";
            return MvcHtmlString.Create(x);
        }

    }
}