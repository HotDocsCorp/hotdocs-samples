using System;
using System.Web.Mvc;

namespace CloudServicesAPIExample3Interview.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var interviewRequest = new InterviewRequest();
            var interviewFragment = interviewRequest.GetInterviewFragment();
            ViewBag.Message = interviewFragment;
            return View();
        }

        public string InterviewDefinition()
        {
            var templateName = Request.QueryString["template"];
            var templateJavaScriptFilePath = String.Format(@"\temp\{0}.js", templateName);
            return System.IO.File.ReadAllText(templateJavaScriptFilePath);
        }

        public string Stylesheets(string filename)
        {
            string stylesheetFilePath = String.Format(@"C:\temp\{0}", filename);
            return System.IO.File.ReadAllText(stylesheetFilePath);
        }
    }
}