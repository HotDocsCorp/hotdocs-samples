using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotDocs.Sdk;
using HotDocs.Sdk.Cloud;

using CloudServicesEmbeddedAPIExample.Models;
using HotDocs.Sdk.Server.Contracts;

namespace CloudServicesEmbeddedAPIExample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            InterviewModel interviewModel = new InterviewModel
            {
                SessionId = EmbeddedInterview.CreateCloudServicesSession(),
                Snapshot = false
            };

            return View(interviewModel);
        }

        public ActionResult ResumeSession()
        {
            var snapshot = EmbeddedInterview.GetSnapshot();
            ViewBag.Interview = EmbeddedInterview.ResumeCloudServicesSession(snapshot);

            return View();
        }

        public void SaveSnapshot(string snapshot)
        {
            EmbeddedInterview.SaveSnapshot(snapshot);
        }
    }
}
