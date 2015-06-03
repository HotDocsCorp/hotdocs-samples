using System.IO;
using HotDocs.Sdk;
using HotDocs.Sdk.Server;
using HotDocs.Sdk.Server.OnPremise;

namespace WebServiceExample2DocumentAssembly
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
    internal class WebServiceExample2DocumentAssembly
    {
        private static void Main(string[] args)
        {
            var assembledDocumentResult = AssembleDocument();
            var fileStream = File.Create(@"C:\temp\output" + assembledDocumentResult.Document.FileExtension);                
            assembledDocumentResult.Document.Content.CopyTo(fileStream);                            
        }

        private static AssembleDocumentResult AssembleDocument()
        {                                           
            var template = GetTemplate();            
            var answers = GetAnswers();
            var assembleDocumentSettings = new AssembleDocumentSettings();

            var service = new OnPremiseServices("http://localhost:80/hdswebapi/api/HDCS");
            return service.AssembleDocument(template, answers, assembleDocumentSettings, "ExampleLogRef");
        }

        private static Template GetTemplate()
        {
            var templateLocation = new WebServiceTemplateLocation("ed40775b-5e7d-4a51-b4d1-32bf9d6e9e29", "http://localhost:80/HDSWebAPI/api/HDCS");
            var template = new Template(templateLocation);
            return template;
        } 

        private static StringReader GetAnswers()
        {
            return new StringReader(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?><AnswerSet version=""1.1""><Answer name=""TextExample-t""><TextValue>World</TextValue></Answer></AnswerSet >");
        }

    }
}
