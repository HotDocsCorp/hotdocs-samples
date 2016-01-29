using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CloudServicesAPIExample3Interview
{
    public class InterviewRequest
    {
        public string GetInterviewFragment()
        {            
            // Get Interview Response from Cloud Services
            var interviewResponse = GetInterviewResponse();

            //Retrieve the multipart file stream containing interview files
            var individualInterviewFileStreams = GetIndividualStreamsFromMultipartStream(interviewResponse);

            // Extract the interview files
            var interviewFiles = GetInterviewFilesFromStream(individualInterviewFileStreams.Result);

            // Save the Template File JavaScript to the temp folder
            SaveInterviewFilesToTempDirectory(interviewFiles.Result);
            
            // Retrieve the Interview HTML fragment
            var interviewHtmlFragment = interviewFiles.Result["fragment.txt"];
            return interviewHtmlFragment;
        }                   

        public HttpResponseMessage GetInterviewResponse()
        {
            // Cloud Services Subscription Details
            string subscriberId = "example-subscriber-id";
            string signingKey = "example-signing-key";

            // HMAC calculation data      
            var timestamp = DateTime.UtcNow;
            var packageId = "HelloWorld";
            var format = "JavaScript";
            var templateName = "";
            var sendPackage = false;
            var billingRef = "ExampleBillingRef";
            var tempImageUrl = "http://localhost/HDServerFiles/temp";            
            var settings = new Dictionary<string, string>
            {
                {"HotDocsJsUrl", "https://cloud.hotdocs.ws/HDServerFiles/6.5/js/"},
                {"HotDocsCssUrl", "https://cloud.hotdocs.ws/HDServerFiles/6.5/stylesheets/hdsuser.css"}, 
                {"InterviewDefUrl", "http://localhost/CloudServicesAPIExample3Interview/Home/InterviewDefinition/"}, 
                {"SaveAnswersPageUrl", "http://localhost/CloudServicesAPIExample3Interview/Home/SaveAnswers/"},                
                {"FormActionUrl", "http://localhost/CloudServicesAPIExample3Interview/Home/InterviewFinish/"}
            };

            // Generate HMAC using Cloud Services signing key            
            string hmac = CalculateHMAC(signingKey, timestamp, subscriberId, packageId, templateName, sendPackage, billingRef, format, tempImageUrl, settings);            

            // Create assemble request            
            var request = CreateHttpRequestMessage(hmac, subscriberId, packageId, timestamp, billingRef, format, tempImageUrl, settings);

            //Send assemble request to Cloud Services
            var client = new HttpClient();
            var response = client.SendAsync(request);        
    
            return response.Result;
        }

        private async Task<IEnumerable<HttpContent>> GetIndividualStreamsFromMultipartStream(HttpResponseMessage response)
        {
            var individualInterviewFileStreams = Enumerable.Empty<HttpContent>();
            Task.Factory.StartNew(
                () => individualInterviewFileStreams = response.Content.ReadAsMultipartAsync().Result.Contents,
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default
            ).Wait();

            return individualInterviewFileStreams;
        }

        private async Task<Dictionary<string, string>> GetInterviewFilesFromStream(IEnumerable<HttpContent> individualInterviewFileStreams)
        {
            Dictionary<string, string> interviewFiles = new Dictionary<string, string>();
            foreach (var fileStream in individualInterviewFileStreams)
            {
                var fileContent = await fileStream.ReadAsStringAsync();
                var filename = fileStream.Headers.ContentDisposition.FileName;
                interviewFiles.Add(filename, fileContent);
            }
            return interviewFiles;
        }

        private void SaveInterviewFilesToTempDirectory(Dictionary<string, string> interviewFiles)
        {
            foreach (var file in interviewFiles)
            {
                var filePath = String.Format(@"C:\temp\{0}", file.Key);
                File.WriteAllText(filePath, file.Value);
            }
        } 

        private HttpRequestMessage CreateHttpRequestMessage(string hmac, string subscriberId, string packageId, DateTime timestamp, string billingRef, string format, string tempImageUrl, Dictionary<string, string> settings)
        {
            var partialInterviewUrl = string.Format("https://cloud.hotdocs.ws/hdcs/interview/{0}/{1}?billingRef={2}&format={3}&tempimageurl={4}", subscriberId, packageId, billingRef, format, tempImageUrl);
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
            request.Headers.TryAddWithoutValidation("Authorization", hmac);
            request.Headers.Add("Keep-Alive", "false");

            return request;
        }

        private StringContent GetAnswers()
        {
            return new StringContent(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?><AnswerSet version=""1.1""><Answer name=""TextExample-t""><TextValue>Hello World</TextValue></Answer></AnswerSet >");
        }

        private string CalculateHMAC(string signingKey, params object[] paramList)
        {
            byte[] key = Encoding.UTF8.GetBytes(signingKey);
            string stringToSign = CanonicalizeParameters(paramList);
            byte[] bytesToSign = Encoding.UTF8.GetBytes(stringToSign);
            byte[] signature;

            using (var hmac = new System.Security.Cryptography.HMACSHA1(key))
            {
                signature = hmac.ComputeHash(bytesToSign);
            }

            return Convert.ToBase64String(signature);
        }

        private string CanonicalizeParameters(params object[] paramList)
        {
            if (paramList == null)
            {
                throw new ArgumentNullException();
            }

            var strings = paramList.Select(param =>
            {
                if (param is string || param is int || param is Enum || param is bool)
                {
                    return param.ToString();
                }

                if (param is DateTime)
                {
                    DateTime utcTime = ((DateTime) param).ToUniversalTime();
                    return utcTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                }

                if (param is Dictionary<string, string>)
                {
                    var sorted = ((Dictionary<string, string>) param).OrderBy(kv => kv.Key);
                    var stringified = sorted.Select(kv => kv.Key + "=" + kv.Value).ToArray();
                    return string.Join("\n", stringified);
                }
                return "";
            });

            return string.Join("\n", strings.ToArray());
        }        
    }
}