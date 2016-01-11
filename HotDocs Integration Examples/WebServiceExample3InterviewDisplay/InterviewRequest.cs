using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebServiceExample3InterviewDisplay
{
    public class InterviewRequest
    {      
        public string GetInterviewFragment()
        {
            // Get Interview from Web Service
            var interviewResult = GetInterview(); 
                                   
            // Get interview file stream from response
            var multipartStream = GetMultipartStream(interviewResult);
            multipartStream.Wait();

            // Get Interview HTML Fragment from file stream
            var interviewFiles = GetInterviewFiles(multipartStream.Result);
            return interviewFiles.Result;
        }

        private async Task<IEnumerable<HttpContent>> GetMultipartStream(HttpResponseMessage response)
        {
            var individualFileStreams = Enumerable.Empty<HttpContent>();
            Task.Factory.StartNew(
                () => individualFileStreams = response.Content.ReadAsMultipartAsync().Result.Contents,
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            ).Wait();

            return individualFileStreams;
        }

        private async Task<string> GetInterviewFiles(IEnumerable<HttpContent> multipartStream)
        {            
            foreach (var attachment in multipartStream)
            {
                if(attachment.Headers.ContentType.ToString() == "text/plain") {
                    var writeAttachmentStream = await attachment.ReadAsStringAsync();
                    return writeAttachmentStream;
                }
            }
            return null;
        }

        private HttpResponseMessage GetInterview()
        {
            // Web Services Subscription Details
            string subscriberId = "0";            

            // Request data            
            var packageId = "ed40775b-5e7d-4a51-b4d1-32bf9d6e9e29";
            var format = "Unspecified";
            var tempImageUrl = "http://localhost/HDServerFiles/temp";
            var settings = new Dictionary<string, string>
            {
                {"HotDocsJsUrl", "http://localhost/HDServerFiles/js/"},
                {"HotDocsCssUrl", "http://localhost/HDServerFiles/stylesheets/"},
                {"FormActionUrl", "InterviewFinish"},
            };                     

            // Create interview request  
            var interviewUrl = CreateInterviewUrl(subscriberId, packageId, format, tempImageUrl, settings);
            var request = CreateHttpRequestMessage(interviewUrl);

            //Send interview request to Web Services
            var client = new HttpClient();
            var response = client.SendAsync(request);

            return response.Result;
        }

        private HttpRequestMessage CreateHttpRequestMessage(string interviewUrl)
        {                       
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(interviewUrl),
                Method = HttpMethod.Post,
                Content = GetAnswers()
            };

            // Add request headers
            request.Content.Headers.Remove("Content-Type");
            request.Content.Headers.Add("Content-Type", "text/xml");            
            request.Headers.Add("Keep-Alive", "false");

            return request;
        }

        private string CreateInterviewUrl(string subscriberId, string packageId, string format, string tempImageUrl, Dictionary<string, string> settings)
        {
            var partialInterviewUrl = string.Format("http://localhost:80/HDSWebAPI/api/hdcs/interview/{0}/{1}?format={2}&tempimageurl={3}", subscriberId, packageId, format, tempImageUrl);
            var completedInterviewUrlBuilder = new StringBuilder(partialInterviewUrl);

            foreach (var kv in settings)
            {
                completedInterviewUrlBuilder.AppendFormat("&{0}={1}", kv.Key, kv.Value ?? "");
            }
            return completedInterviewUrlBuilder.ToString();
        }


        private StringContent GetAnswers()
        {
            return new StringContent(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?><AnswerSet version=""1.1""><Answer name=""TextExample-t""><TextValue>Hello World</TextValue></Answer></AnswerSet >");
        }
        
    }
}