using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HotDocs.Sdk.Server.Contracts;
using HotDocs.Cloud.Client;
using CloudServicesEmbeddedExampleInterview.Models;

namespace CloudServicesEmbeddedExampleInterview.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            InterviewModel interviewModel = new InterviewModel
            {
                SessionId = GetSessionID()
            };

            return View(interviewModel);
        }

        protected string GetSessionID()
        {
            // Subscriber Information
            string subscriberId = "example-subscriber-id";
            string signingKey = "example-signing-key";

            // Template Package File Information
            string templateName = "HelloWorld";
            string templateFilePath = @"C:\temp\HelloWorld.hdpkg";

             //Create the Cloud Services session and return the Session ID
            var client = new RestClient(subscriberId, signingKey);            
            return client.CreateSession(templateName, Server.MapPath(templateFilePath));            
        }
    }
}