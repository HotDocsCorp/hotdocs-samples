using System;
using System.IO;
using HotDocs.Sdk;
using HotDocs.Sdk.Server.Cloud;
using HotDocs.Sdk.Server.OnPremise;

namespace SdkExample8
{
    /// <summary>
    /// This demonstrates assembling a document from a template using either Cloud services or On Premise Web API. 
    /// Assumes we already have answer XML
    /// If using On Premise Web API, it assumes the package has already been uploaded there
    /// </summary>
    internal class Example8
    {
        private static void Main(string[] args)
        {
            //cloud services
            //var service = new Services(subscriberId, signingKey);

            //on premise web api
            var service = new OnPremiseServices("http://localhost:80/hdswebapi/api/HDCS");

            var template = GetTemplate();
            var assembleDocumentSettings = new AssembleDocumentSettings();
            using (
                var answers =
                    new StringReader(
                        @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?><AnswerSet version=""1.1""><Answer name=""MyVar""><TextValue>World</TextValue></Answer></AnswerSet >")
                )
            {
                var assembledDocumentResult = service.AssembleDocument(template, answers,
                    assembleDocumentSettings, "YourDesiredCloudServiceLogRef");

                using (var fileStream = File.Create("output" + assembledDocumentResult.Document.FileExtension))
                {
                    assembledDocumentResult.Document.Content.CopyTo(fileStream);
                }
            }
        }

        private static Template GetTemplate()
        {
            const string packagePath = "HelloWorld.hdpkg";
            //First argument is unique ID for package in Cloud services
            //Second argument is path to package. We're using a relative one here.
            var templateLocation = new PackagePathTemplateLocation("7A7BF8B9-C895-4BC9-BC1A-44E61D6008A2",
                packagePath);
            var template = new Template(templateLocation);
            return template;
        }
    }
}
