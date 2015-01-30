using System;
using System.IO;
using HotDocs.Sdk;
using HotDocs.Sdk.Server;
using HotDocs.Sdk.Server.Local;

namespace SdkExample1
{
    /// <summary>
    /// This demonstrates assembling a document from a template using HotDocs Server and assuming we already have answer XML
    /// </summary>
    internal class Example1
    {
        private static void Main(string[] args)
        {
            AssembleDocument();
        }

        private static void AssembleDocument()
        {
            var assembledDocumentResult = CreateAssembleDocumentResult();
            using (var fileStream = File.Create(@"C:\temp\output" + assembledDocumentResult.Document.FileExtension))
            {
                assembledDocumentResult.Document.Content.CopyTo(fileStream);
            }
        }

        private static AssembleDocumentResult CreateAssembleDocumentResult()
        {
            var template = CreateTemplate();
            var answers = GetAnswers();
            var assembleDocumentSettings = new AssembleDocumentSettings();

            var service = CreateHotDocsService();
            var assembledDocumentResult = service.AssembleDocument(template, answers, assembleDocumentSettings, "Example Assemble Document Log Text");

            return assembledDocumentResult;
        }

        private static Services CreateHotDocsService()
        {
            const string tempDirectoryPath = @"C:\temp\";
            var service = new Services(tempDirectoryPath);
            return service;
        }

        private static StringReader GetAnswers()
        {
            return new StringReader(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?><AnswerSet version=""1.1""><Answer name=""TextExample-t""><TextValue>World</TextValue></Answer></AnswerSet >");
        }

        private static Template CreateTemplate()
        {
            const string packagePath = @"C:\temp\HelloWorld.hdpkg";
            var packageId = Guid.NewGuid().ToString();
            var templateLocation = new PackagePathTemplateLocation(packageId, packagePath);

            var template = new Template(templateLocation);
            return template;
        }     
    }
}