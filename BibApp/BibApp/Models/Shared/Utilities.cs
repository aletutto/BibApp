using Microsoft.AspNetCore.Mvc.Rendering;

namespace BibApp.Models.Shared
{
    public static class Utilities
    {
        public static string IsActive(
            this IHtmlHelper html,
            string control,
            string action)
        {
            var routeData = html.ViewContext.RouteData;

            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];

            // both must match
            var returnActive = control == routeControl &&
                               action == routeAction;

            return returnActive ? "active" : "";
        }
    }
}
