using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceExample3InterviewDisplay
{
    public class InterviewRequest
    {      
        public string GetInterviewFragment()
        {
            var interviewResult = GetInterview();                                    
            var multipartStream = GetMultipartStream(interviewResult);
            multipartStream.Wait();

            var interviewFiles = GetInterviewFiles(multipartStream.Result);
            return interviewResult.StatusCode.ToString();
        }

        static async Task<IEnumerable<HttpContent>> GetMultipartStream(HttpResponseMessage response)
        {
            IEnumerable<HttpContent> individualFileStreams = null;
            Task.Factory.StartNew(
                () => individualFileStreams = response.Content.ReadAsMultipartAsync().Result.Contents
            ).Wait();

            return individualFileStreams;            
        }

        static async Task<string> GetInterviewFiles(IEnumerable<HttpContent> multipartStream)
        {
            Dictionary<string, string> s = new Dictionary<string, string>();
            foreach (var attachment in multipartStream)
            {
                var writeAttachmentStream = await attachment.ReadAsStringAsync();
                s.Add(attachment.Headers.ContentType.ToString(), writeAttachmentStream);
            }
            return s["text/plain"];
        }

        public HttpResponseMessage GetInterview()
        {
            // Cloud Services Subscription Details
            string subscriberId = "0";            

            // HMAC calculation data
            var timestamp = DateTime.UtcNow;
            var packageId = "ed40775b-5e7d-4a51-b4d1-32bf9d6e9e29";
            var format = "Unspecified";
            var tempImageUrl = "test";
            var settings = new Dictionary<string, string>
            {
                {"HotDocsJsUrl", "https://localhost/HDServerFiles/js/"},
                {"HotDocsCssUrl", "https://localhost/HDServerFiles/stylesheets/"},
                {"FormActionUrl", "Disposition.aspx"},
            };                     

            // Create assemble request            
            var request = CreateHttpRequestMessage(subscriberId, packageId, timestamp, format, tempImageUrl, settings);

            //Send assemble request to Cloud Services
            var client = new HttpClient();
            var response = client.SendAsync(request);
            return response.Result;
        }

        private static HttpRequestMessage CreateHttpRequestMessage(string subscriberId, string packageId, DateTime timestamp, string format, string tempImageUrl, Dictionary<string, string> settings)
        {            
            var partialInterviewUrl = string.Format("http://localhost:80/HDSWebAPI/api/hdcs/interview/{0}/{1}?format={2}&tempimageurl={3}", subscriberId, packageId, format, tempImageUrl);
            var completedInterviewUrlBuilder = new StringBuilder(partialInterviewUrl);

            foreach (var kv in settings)
            {
                completedInterviewUrlBuilder.AppendFormat("&{0}={1}", kv.Key, kv.Value ?? "");
            }
            var InterviewUrl = completedInterviewUrlBuilder.ToString();

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(InterviewUrl),
                Method = HttpMethod.Post,
                Content = GetAnswers()
            };

            // Add request headers
            request.Headers.Add("x-hd-date", timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            request.Content.Headers.Remove("Content-Type");
            request.Content.Headers.Add("Content-Type", "text/xml");            
            request.Headers.Add("Keep-Alive", "false");

            return request;
        }

        private static StringContent GetAnswers()
        {
            return new StringContent(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?><AnswerSet version=""1.1""><Answer name=""TextExample-t""><TextValue>Hello World</TextValue></Answer></AnswerSet >");
        }
        
    }
}