using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace CloudServicesEmbeddedAPIExample
{
    public class EmbeddedInterview
    {
        // Subscriber Information
        static readonly string subscriberId = "example-subscriber-id";
        static readonly string signingKey = "example-signing-key";

        // HMAC calculation data
        static readonly DateTime timestamp = DateTime.UtcNow;
        static readonly string packageId = "HelloWorld";
        static readonly string billingRef = "";
        static readonly string interviewFormat = "JavaScript";
        static readonly string outputFormat = "DOCX";
        static readonly bool showDownloadLinks = true;
        static readonly string settings = null;

        public static string CreateCloudServicesSession()
        {
            // Generate HMAC using Cloud Services signing key            
            var hmac = CalculateHMAC(signingKey, timestamp, subscriberId, packageId, billingRef, interviewFormat, outputFormat, settings);

            // Create Session request                        
            var request = CreateHttpRequestMessage(hmac, subscriberId, packageId, timestamp, interviewFormat, outputFormat, showDownloadLinks);

            //Send upload request to Cloud Service
            var client = new HttpClient();
            var response = client.SendAsync(request);
            response.Wait();

            // Get Session ID from Response Content Stream
            var responseContentStream = response.Result.Content;
            var sessionId = responseContentStream.ReadAsStringAsync().Result;

            return sessionId;              
        }

        public static string ResumeCloudServicesSession()
        {
            // Get the Interview Snapshot from disk
            var snapshot = GetSnapshot();

            // Generate HMAC using Cloud Services signing key
            var hmac = CalculateHMAC(signingKey, timestamp, subscriberId, snapshot);

            // Create Session request                       
            var request = CreateResumeSessionHttpRequestMessage(hmac, subscriberId, packageId, snapshot, timestamp, showDownloadLinks);

            //Send Create Session request to Cloud Service
            var client = new HttpClient();
            var response = client.SendAsync(request);
            response.Wait();

            // Get Session ID from Response Content Stream
            var responseContentStream = response.Result.Content;
            var sessionId = responseContentStream.ReadAsStringAsync().Result;

            return sessionId;
        }

        private static HttpRequestMessage CreateHttpRequestMessage(string hmac, string subscriberId, string packageId, DateTime timestamp, string interviewFormat, string outputFormat, bool showDownloadLinks)
        {
            var newSessionUrl = string.Format("https://cloud.hotdocs.ws/embed/newsession/{0}/{1}?interviewFormat={2}&outputformat={3}&showdownloadlinks={4}", subscriberId, packageId, interviewFormat, outputFormat, showDownloadLinks);
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(newSessionUrl),
                Method = HttpMethod.Post,
                Content = GetAnswers()
            };

            // Add request headers       
            request.Headers.Add("x-hd-date", timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ"));            
            request.Headers.TryAddWithoutValidation("Authorization", hmac);   
            request.Headers.Add("Keep-Alive", "false");
            request.Headers.TryAddWithoutValidation("Content-Type", "text/xml");

            return request;
        }

        private static HttpRequestMessage CreateResumeSessionHttpRequestMessage(string hmac, string subscriberId, string packageId, string snapshot, DateTime timestamp, bool showDownloadLinks)
        {
            var resumeSessionUrl = string.Format("https://cloud.hotdocs.ws/embed/resumesession/{0}/{1}?date={2}&showdownloadlinks={3}", subscriberId, packageId, timestamp, showDownloadLinks);
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(resumeSessionUrl),
                Method = HttpMethod.Post,
                Content = new StringContent(snapshot)
            };

            // Add request headers       
            request.Headers.Add("x-hd-date", timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            request.Content.Headers.Remove("Content-Type");
            request.Content.Headers.Add("Content-Type", "text/plain");
            request.Headers.TryAddWithoutValidation("Authorization", hmac);
            request.Headers.Add("Keep-Alive", "false");

            return request;
        }

        public static void SaveSnapshot(string snapshot)
        {            
            if (snapshot != null)
            {
                System.IO.File.WriteAllText(@"C:\temp\snapshot.txt", snapshot);                
            }
        }

        public static string GetSnapshot()
        {
            if (System.IO.File.Exists(@"C:\temp\snapshot.txt"))
            {
                return System.IO.File.ReadAllText(@"C:\temp\snapshot.txt");
            }
            return "";
        }

        private static StringContent GetAnswers()
        {
            return new StringContent(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?><AnswerSet version=""1.1""><Answer name=""TextExample-t""><TextValue>Hello World</TextValue></Answer></AnswerSet >");
        }

        public static string CalculateHMAC(string signingKey, params object[] paramList)
        {
            byte[] key = Encoding.UTF8.GetBytes(signingKey);
            string stringToSign = Canonicalize(paramList);
            byte[] bytesToSign = Encoding.UTF8.GetBytes(stringToSign);
            byte[] signature;

            using (var hmac = new System.Security.Cryptography.HMACSHA1(key))
            {
                signature = hmac.ComputeHash(bytesToSign);
            }

            return Convert.ToBase64String(signature);
        }

        public static string Canonicalize(params object[] paramList)
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
                    DateTime utcTime = ((DateTime)param).ToUniversalTime();
                    return utcTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
                }

                if (param is Dictionary<string, string>)
                {
                    var sorted = ((Dictionary<string, string>)param).OrderBy(kv => kv.Key);
                    var stringified = sorted.Select(kv => kv.Key + "=" + kv.Value).ToArray();
                    return string.Join("\n", stringified);
                }
                return "";
            });

            return string.Join("\n", strings.ToArray());
        }

    }
}