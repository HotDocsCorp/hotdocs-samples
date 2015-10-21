using System.Web.Mvc;

using CloudServicesEmbeddedAPIExample.Models;

namespace CloudServicesEmbeddedAPIExample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            InterviewModel interviewModel = new InterviewModel
            {
                SessionId = EmbeddedInterview.CreateCloudServicesSession()                
            };

            return View(interviewModel);
        }

        public ActionResult ResumeSession()
        {            
            InterviewModel interviewModel = new InterviewModel
            {
                SessionId = EmbeddedInterview.ResumeCloudServicesSession()                
            };

            return View("Index", interviewModel);
        }

        public void SaveSnapshot(string snapshot)
        {
            EmbeddedInterview.SaveSnapshot(snapshot);
        }
    }
}
