using System;
using System.Collections.Generic;
using System.IO;
using HotDocs.Sdk;
using HotDocs.Sdk.Server;
using HotDocs.Sdk.Server.Local;
using Util = HotDocs.Sdk.Util;

namespace SdkExample5
{
    /// <summary>
    /// This shows how to use a worksession to repeatedly retrieve interviews, complete them, and then retrieve the assembled documents
    /// </summary>
    internal class Example5
    {
        private static void Main(string[] args)
        {            
            AssembleDocuments();
        }

        private static void AssembleDocuments()
        {
            Util.SetPersistentEncryptionKey("ExampleKey");   
            var session = CreateWorkSession();
            
            while (!session.IsCompleted)
            {
                var currentWorkItem = session.CurrentWorkItem;
                if (currentWorkItem is InterviewWorkItem)
                {
                    var interviewHtmlFragment = session.GetCurrentInterview();
                    //Show interview to user, receive postback containing answers
                    var answers = GetAnswers();                    
                    session.FinishInterview(answers);
                    
                }
                
                var docs = session.AssembleDocuments("logref");
                foreach (var doc in docs)
                {
                    //Process each document as needed. For example, save document to disk, as in Example 2.
                }
            }
        }

        private static WorkSession CreateWorkSession()
        {
            
            var service = CreateHotDocsService();
            var template = CreateTemplate();
            var interviewSettings = GetInterviewSettings();
            var session = new WorkSession(service, template, null, interviewSettings);

            return session;
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

        private static Services CreateHotDocsService()
        {
            const string tempDirectoryPath = @"C:\temp\";
            var service = new Services(tempDirectoryPath);
            return service;
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
            var multipleChoices = new List<string> { "Apple", "Orange" };

            mcAnswer.SetValue(new MultipleChoiceValue(multipleChoices.ToArray()));

            //Repeat answers
            var repeatAnswer = answerCollection.CreateAnswer<TextValue>("NameRepeatExample-t");
            repeatAnswer.SetValue<TextValue>("Steve", 0);
            repeatAnswer.SetValue<TextValue>("Carol", 1);
            repeatAnswer.SetValue<TextValue>("John", 2);

            return answerCollection;
        }

        private static Template CreateTemplate()
        {
            const string packagePath = @"C:\temp\HelloWorld.hdpkg";
            string packageId = Guid.NewGuid().ToString();
            var templateLocation = new PackagePathTemplateLocation(packageId, packagePath);

            var template = new Template(templateLocation);
            return template;
        }
    }
}