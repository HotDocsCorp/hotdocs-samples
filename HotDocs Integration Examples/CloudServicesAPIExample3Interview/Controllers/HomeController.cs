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
            var templateFileName = Request.QueryString["template"];
            var templateJavaScriptFilePath = String.Format(@"C:\temp\{0}.js", templateFileName);
            return System.IO.File.ReadAllText(templateJavaScriptFilePath);            
        }
    }
}