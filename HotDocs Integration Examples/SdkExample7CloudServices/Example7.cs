using System;
using System.IO;
using HotDocs.Sdk;
using HotDocs.Sdk.Server;
using HotDocs.Sdk.Server.Cloud;

namespace SdkExample7CloudServices
{
    /// <summary>
    /// This demonstrates assembling a document from a template using HotDocs Cloud Services and assuming we already have answer XML.
    /// This examples uses the same code as Example 1. Only the CreateHotDocsService method has changed.
    /// </summary>
    internal class Example7
    {
        private static void Main()
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
            // You must change the 'example-subscriber-id' and 'example-subscriber-key' values to your own HotDocs Cloud Services identification information.
            var service = new Services("example-subscriber-id", "example-subscriber-key");
            return service;
        }

        private static StringReader GetAnswers()
        {
            return new StringReader(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?><AnswerSet version=""1.1""><Answer name=""TextExample-t""><TextValue>Hello World</TextValue></Answer></AnswerSet >");
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