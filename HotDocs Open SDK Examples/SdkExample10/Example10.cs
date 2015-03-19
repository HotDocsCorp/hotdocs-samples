using System.IO;
using HotDocs.Sdk;
using HotDocs.Sdk.Server.OnPremise;

namespace SdkExample10
{
    /// <summary>
    /// This demonstrates assembling a document from a template using the On Premise Web API. 
    /// Assumes we already have answer XML
    /// it assumes the package has already been uploaded there
    /// 
    ///This will test assembling a document. The assembled document (output.docx) will be found in the bin/Debug folder and should contain the text ‘Hello World’.
    ///To test the on premise web API replace the hardcoded host address passed into the OnPremiseServices class at line 23 with your relevant address.
    ///In order for this to be successful the package must exist in the TempFiles folder in the on premise web API solution. 
    ///It will be a package named ‘HelloWorld.hdpkg’ within a folder named by the package id ‘7A7BF8B9-C895-4BC9-BC1A-44E61D6008A2’ unless
    ///you alter these hardcoded values in the GetTemplate() method.
    /// </summary>
    internal class Example10
    {
        private static void Main(string[] args)
        {
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
                    assembleDocumentSettings, "YourDesiredLogRef");

                using (var fileStream = File.Create("output" + assembledDocumentResult.Document.FileExtension))
                {
                    assembledDocumentResult.Document.Content.CopyTo(fileStream);
                }
            }
        }

        private static Template GetTemplate()
        {
            const string packagePath = @"C:\Temp\HelloWorld.hdpkg";
            //First argument is unique ID for the package
            //Second argument is path to package. We're using a relative one here.
            var templateLocation = new PackagePathTemplateLocation("7A7BF8B9-C895-4BC9-BC1A-44E61D6008A2",
                packagePath);
            var template = new Template(templateLocation);
            return template;
        }
    }
}
