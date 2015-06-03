using System;
using System.IO;
using System.Web.Mvc;
using HotDocs.Sdk;
using HotDocs.Sdk.Server;
using HotDocs.Sdk.Server.Local;
using SdkExample4InterviewDisplay.Models;
using Util = HotDocs.Sdk.Util;

namespace SdkExample4InterviewDisplay.Controllers
{
    /// <summary>
    /// This controller displays an interview and shows how to process the answers returned upon completion
    /// </summary>
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var interviewFragment = GetInterviewFragment();
            var model = new InterviewModel { InterviewFragment = interviewFragment };
            return View(model);
        }

        private static string GetInterviewFragment()
        {            
            var template = CreateTemplate();
            var interviewSettings = GetInterviewSettings();

            var service = CreateHotDocsService();
            var interview = service.GetInterview(template, null, interviewSettings, null, "CloudLoggingReference");

            return interview.HtmlFragment;
        }

        private static InterviewSettings GetInterviewSettings()
        {
            string postInterviewUrl = "Home/PostInterviewProcessing";
            string interviewRuntimeUrl = "http://localhost/HDServerFiles/js";
            string interviewStylesheetUrl = "http://localhost/HDServerFiles/stylesheets";
            string interviewFileUrl = "Home/GetInterviewFile";

            var interviewSettings = new InterviewSettings(postInterviewUrl, interviewRuntimeUrl, interviewStylesheetUrl, interviewFileUrl) { RoundTripUnusedAnswers = true };

            return interviewSettings;
        }

        [HttpPost]
        public ActionResult PostInterviewProcessing()
        {
            //Retrieve encoded answers
            var encodedAnswers = InterviewResponse.GetAnswers(Request.Form);
            
            //If you need to parse new answers out of result but not roundtripped ones then
            var newAnswers = InterviewAnswerSet.GetDecodedInterviewAnswers(new StringReader(encodedAnswers));
            var newAnswersAsString = newAnswers.ReadToEnd();

            //If you need all answers including roundtripped
            var service = CreateHotDocsService();
            var allAnswers = service.GetAnswers(new[] { new StringReader(encodedAnswers) }, "logref");

            //If no need to use answers outside of assembly then just assemble using encoded ones
            AssembleDocument(encodedAnswers);

            return null;
        }

        /// <summary>
        /// This is called repeatedly by the interview JS to retrieve the files needed for the interview
        /// </summary>
        /// <returns></returns>
        public FileStreamResult GetInterviewFile()
        {
            //File name of template or image
            var fileName = Request.QueryString["template"];        

            //Template location
            var templateLocationData = Request.QueryString["loc"];
            var templateLocation = Template.Locate(templateLocationData);

            //Type of file requested
            var fileType = Request.QueryString["type"];
            fileType = fileType.ToLower();

            //Get interview stream            
            var service = CreateHotDocsService();
            var interviewFileStream = service.GetInterviewFile(templateLocation, fileName, fileType);

            //Get MIME type                          
            var outputFileName = fileType == "img" ? fileName : fileName + "." + fileType;
            var interviewResultMimeType = Util.GetMimeType(outputFileName, fileType == "img");

            var interviewFileStreamResult = new FileStreamResult(interviewFileStream, interviewResultMimeType);
            return interviewFileStreamResult;
        }

        private static Template CreateTemplate()
        {
            const string packagePath = @"C:\temp\HelloWorld.hdpkg";
            string packageId = Guid.NewGuid().ToString();
            var templateLocation = new PackagePathTemplateLocation(packageId, packagePath);

            var template = new Template(templateLocation);
            return template;
        }

        private static Services CreateHotDocsService()
        {
            const string tempDirectoryPath = @"C:\temp\";
            var service = new Services(tempDirectoryPath);
            return service;
        }

        private static void AssembleDocument(string encryptedAnswers)
        {
            var service = CreateHotDocsService();
            var template = CreateTemplate();
            var assembleDocumentSettings = new AssembleDocumentSettings();
            using (var answers = new StringReader(encryptedAnswers))
            {
                var assembledDocumentResult = service.AssembleDocument(template, answers, assembleDocumentSettings, "logref");

                using (var fileStream = System.IO.File.Create(@"c:\temp\output" + assembledDocumentResult.Document.FileExtension))
                {
                    assembledDocumentResult.Document.Content.CopyTo(fileStream);
                }
            }
        }
    }
}