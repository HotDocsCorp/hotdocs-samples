using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using HotDocs.Sdk;
using HotDocs.Sdk.Server;
using HotDocs.Sdk.Server.OnPremise;
using SdkExample11.Models;

namespace SdkExample11.Controllers
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
        private const string HostAddress = "http://localhost:80/hdswebapi/api/hdcs";
        private const string ServerFilesAddress = "http://localhost";
        private const string PackageId = "7A7BF8B9-C895-4BC9-BC1A-44E61D6008A2";
        private const string AnswerFileLoc = @"";
        private const string DocumentOutputLoc = @"C:\Temp";

        public ActionResult Index()
        {
            var fragment = GetInterviewFragment();
            var model = new InterviewModel { InterviewFragment = fragment };
            return View(model);
        }

        private static string GetInterviewFragment()
        {
            var service = new OnPremiseServices(HostAddress);

            var template = GetTemplate();
            string locator = template.CreateLocator();
            string tempurl = HttpUtility.UrlEncode(String.Format("Home/GetInterviewFile?templatelocater={0}&filename=", locator));

            var interviewSettings = new InterviewSettings("Home/PostInterviewProcessing",
                 ServerFilesAddress + "/HDServerFiles/js",
                 ServerFilesAddress + "/HDServerFiles/stylesheets", tempurl) { RoundTripUnusedAnswers = true };

            if (String.IsNullOrEmpty(AnswerFileLoc))
            {
                var interview = service.GetInterview(template, null, interviewSettings, null,
                   "CloudLoggingReference");
                return interview.HtmlFragment;
            }

            using (TextReader reader = System.IO.File.OpenText(AnswerFileLoc))
            {
                var interview = service.GetInterview(template, reader, interviewSettings, null,
                    "CloudLoggingReference");
                return interview.HtmlFragment;
            }
        }

        [HttpPost]
        public ActionResult PostInterviewProcessing()
        {
            var encodedAnswers = InterviewResponse.GetAnswers(Request.Form);
            //If no need to use answers outside of assembly then just assemble using encoded ones
            AssembleDocument(encodedAnswers);
            return Redirect(ServerFilesAddress);
        }

        /// <summary>
        /// This is called repeatedly by the interview JS to retrieve the files needed for the interview
        /// </summary>
        /// <returns></returns>
        public FileContentResult GetInterviewFile(string filename, string templatelocater)
        {
            var service = new OnPremiseServices(HostAddress);
            var template = Template.Locate(templatelocater);

            using (var stream = new MemoryStream())
            {
                service.GetInterviewFile(template, filename, "").CopyTo(stream);
                return new FileContentResult(stream.ToArray(), Path.GetExtension(filename));
            }
        }

        private static byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private static Template GetTemplate()
        {
            string packageId = PackageId;
            string packagePath = HostAddress;

            //First argument is unique ID for package for our API
            //Second argument is the HostAddress for our API
            var templateLocation = new WebServiceTemplateLocation(packageId,
                packagePath);
            var template = new Template(templateLocation);
            return template;
        }

        private static void AssembleDocument(string encryptedAnswers)
        {
            var service = new OnPremiseServices(HostAddress);
            var template = GetTemplate();
            var assembleDocumentSettings = new AssembleDocumentSettings();
            using (var answers = new StringReader(encryptedAnswers))
            {
                var assembledDocumentResult = service.AssembleDocument(template, answers,
                    assembleDocumentSettings, "YourDesiredCloudServiceLogRef");

                using (var fileStream = System.IO.File.Create(DocumentOutputLoc + assembledDocumentResult.Document.FileExtension))
                {
                    assembledDocumentResult.Document.Content.CopyTo(fileStream);
                }
            }
        }
    }
}