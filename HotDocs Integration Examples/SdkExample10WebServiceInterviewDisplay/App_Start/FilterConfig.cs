using System.Web.Mvc;

namespace SdkExample10WebServiceInterviewDisplay
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}