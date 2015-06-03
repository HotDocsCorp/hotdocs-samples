using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using HotDocs.Sdk;
using HotDocs.Sdk.Server;
using HotDocs.Sdk.Server.OnPremise;
using SdkExample12.Models;

namespace SdkExample12.Controllers
{
    /// <summary>
    /// This controller displays an interview and shows how to process the answers returned upon completion
    /// It uses the on premise API that should be configured in the web.config
    /// 
    ///    Setting Up
    ///----------------------------
    ///1. In the HomeController look at the const properties.
    ///1. Set the ServerFilesAddress setting to the address this example will be running on (example: http://localhost:52948) no '/' at the end
    ///2. Set the HostAddress setting to the address of the web API controller (example: http://localhost:55232/HDSWebAPI/api/hdcs)
    ///3. Set the PackageId setting to be the unique GUID assigned to the package. (This should be known when the package is uploaded)
    ///4. Optionally set the AnswerFileLocation to the phsyical path of an answerfile
    ///5. Set the DocumentOutputLocation to the location you want the documents to be output, you also have to specify the name of the file with no extension (example:@”C:\user\bob\thedocument”)

    ///Running the Application
    ///----------------------------
    ///1. When you run the application in visual studio the first page you should be taken to is the interview
    ///2. The interview should be built from all the details passed into the interview settings (line 36 in the HomeController.cs)
    ///3. The settings and template are then passed to the new service we have created in the SDK.
    ///4. The interview should now appear in your browser of choice.
    ///5. After filling in the interview click finish the page should refresh and your documents should be written to the DocumentOutputLocation
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
            string templateLocation = template.CreateLocator();
            var interviewSettings = GetInterviewSettings(templateLocation);

            var service = new OnPremiseServices("http://localhost:80/hdswebapi/api/hdcs");
            var interview = service.GetInterview(template, null, interviewSettings, null, "ExampleLogReference");
            return interview.HtmlFragment;
        }

        private static InterviewSettings GetInterviewSettings(string templateLocation)
        {
            string postInterviewUrl = "Home/PostInterviewProcessing";
            string interviewRuntimeUrl = "http://localhost/HDServerFiles/js";
            string interviewStylesheetUrl = "http://localhost/HDServerFiles/stylesheets";                                
            string interviewFileUrl = HttpUtility.UrlEncode("/Home/GetInterviewFile?templatelocater=" + templateLocation);

            var interviewSettings = new InterviewSettings(postInterviewUrl, interviewRuntimeUrl, interviewStylesheetUrl, interviewFileUrl) { RoundTripUnusedAnswers = true };
            return interviewSettings;
        }

        [HttpPost]
        public ActionResult PostInterviewProcessing()
        {
            var encodedAnswers = InterviewResponse.GetAnswers(Request.Form);            
            AssembleDocument(encodedAnswers);
            return null;
        }

        /// <summary>
        /// The GetInterviewFile method is called repeatedly by the interview JavaScript to retrieve the files needed for the interview
        /// </summary>
        /// <returns></returns>
        public FileContentResult GetInterviewFile(string filename, string templateLocation)
        {
            var service = new OnPremiseServices("http://localhost:80/hdswebapi/api/hdcs");
            var template = Template.Locate(templateLocation);
            var interviewFile = service.GetInterviewFile(template, filename, "");

            var stream = new MemoryStream();            
            interviewFile.CopyTo(stream);
            
            var fileContentResult = new FileContentResult(stream.ToArray(), Path.GetExtension(filename));
            stream.Close();

            return fileContentResult;
        }

        private static Template CreateTemplate()
        {            
            var templateLocation = new PackagePathTemplateLocation("7A7BF8B9-C895-4BC9-BC1A-44E61D6008A2", @"C:\Temp\HelloWorld.hdpkg");
            var template = new Template(templateLocation);
            return template;
        }

        private static void AssembleDocument(string encodedAnswers)
        {            
            var template = CreateTemplate();
            var answers = new StringReader(encodedAnswers);
            var assembleDocumentSettings = new AssembleDocumentSettings();

            var service = new OnPremiseServices("http://localhost:80/hdswebapi/api/hdcs");
            var assembledDocumentResult = service.AssembleDocument(template, answers, assembleDocumentSettings, "ExampleLogRef");
            SaveAssembledDocument(assembledDocumentResult);                       
        }

        private static void SaveAssembledDocument(AssembleDocumentResult assembledDocumentResult)
        {
            var fileStream = System.IO.File.Create(@"C:\temp\output" + assembledDocumentResult.Document.FileExtension);
            assembledDocumentResult.Document.Content.CopyTo(fileStream);
            fileStream.Close();
        }
    }
}