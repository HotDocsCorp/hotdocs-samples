using System;
using System.IO;
using HotDocs.Sdk;
using HotDocs.Sdk.Server.Local;
using Util = HotDocs.Sdk.Util;

namespace SdkExample3InterviewFragment
{
    /// <summary>
    /// This demonstrates how to retrieve the HTML fragment for an interview
    /// </summary>
    internal class Example3
    {
        private static void Main(string[] args)
        {          
            var interviewFragment = CreateInterviewFragment();            
            File.WriteAllText(@"C:\temp\InterviewExample.html", interviewFragment);
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

        private static string CreateInterviewFragment()
        {
            var interviewSettings = GetInterviewSettings();
            var template = CreateTemplate();                        
            var service = CreateHotDocsService();

            var interview = service.GetInterview(template, null, interviewSettings, null, "Logging Identifier Text");
            return interview.HtmlFragment;
        }

        private static InterviewSettings GetInterviewSettings()
        {
            string postInterviewUrl = "";
            string interviewRuntimeUrl = "http://localhost/HDServerFiles/js";
            string interviewStylesheetUrl = "http://localhost/HDServerFiles/stylesheets";
            string interviewFileUrl = "";

            var interviewSettings = new InterviewSettings(postInterviewUrl, interviewRuntimeUrl, interviewStylesheetUrl, interviewFileUrl);

            return interviewSettings;
        }
    }
}