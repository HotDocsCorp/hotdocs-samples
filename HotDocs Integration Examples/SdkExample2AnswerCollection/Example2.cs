using System;
using System.IO;
using HotDocs.Sdk;
using HotDocs.Sdk.Server;
using HotDocs.Sdk.Server.Local;

namespace SdkExample2AnswerCollection
{
    /// <summary>
    /// This demonstrates creating an answer collection containing various answer types and then reading out the resulting answer XML
    /// </summary>
    internal class SdkExample2AnswerCollection
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

        private static Template CreateTemplate()
        {
            const string packagePath = @"C:\temp\HelloWorld.hdpkg";
            var packageId = Guid.NewGuid().ToString();
            var templateLocation = new PackagePathTemplateLocation(packageId, packagePath);

            var template = new Template(templateLocation);
            return template;
        }

        private static StringReader GetAnswers()
        {
            var answerCollection = CreateAnswerCollection();
            var answerCollectionXml = answerCollection.XmlAnswers;
            return new StringReader(answerCollectionXml);
        }

        private static AnswerCollection CreateAnswerCollection()
        {
            var answerCollection = new AnswerCollection();

            //Text answers
            var answer = answerCollection.CreateAnswer<TextValue>("TextExample-t");
            answer.SetValue<TextValue>("World");

            //Number answers
            var numberAnswer = answerCollection.CreateAnswer<NumberValue>("NumberExample-n");
            numberAnswer.SetValue<NumberValue>(123);

            //Date answers
            var dateAnswer = answerCollection.CreateAnswer<DateValue>("DateExample-d");
            dateAnswer.SetValue<DateValue>(DateTime.Now);

            //True/False answers
            var trueFalseAnswer = answerCollection.CreateAnswer<TrueFalseValue>("TrueFalseExample-tf");
            trueFalseAnswer.SetValue<TrueFalseValue>(true);

            //Multiple choice answers          
            var mcAnswer = answerCollection.CreateAnswer<MultipleChoiceValue>("MultipleChoiceExample-mc");
            var multipleChoices = new string[] { "Apple", "Orange" };

            mcAnswer.SetValue(new MultipleChoiceValue(multipleChoices));

            //Repeat answers
            var repeatAnswer = answerCollection.CreateAnswer<TextValue>("NameRepeatExample-t");
            repeatAnswer.SetValue<TextValue>("Steve", 0);
            repeatAnswer.SetValue<TextValue>("Carol", 1);
            repeatAnswer.SetValue<TextValue>("William", 2);

            return answerCollection;
        }
    }
}