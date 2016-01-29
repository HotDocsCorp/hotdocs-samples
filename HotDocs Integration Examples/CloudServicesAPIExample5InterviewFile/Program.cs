using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace CloudServicesAPIExample5InterviewFile
{
    class Program
    {
        static void Main(string[] args)
        {
            // Cloud Services Subscription Details
            string subscriberId = "example-subscriber-id";
            string signingKey = "example-signing-key";

            // HMAC calculation data      
            var timestamp = DateTime.UtcNow;
            var packageId = "HelloWorld";
            var fileName = "HelloWorld.docx.js";
            var billingRef = "ExampleBillingRef";

            // Generate HMAC using Cloud Services signing key            
            string hmac = CalculateHMAC(signingKey, timestamp, subscriberId, packageId, null, true, billingRef);

            // Create assemble request            
            var request = CreateHttpRequestMessage(hmac, subscriberId, packageId, fileName, timestamp, billingRef);

            //Send assemble request to Cloud Services
            var client = new HttpClient();
            var response = client.SendAsync(request);

            // Read Interview File content from Response
            var attachmentContent = response.Result.Content.ReadAsStringAsync();
            attachmentContent.Wait();            

            // Save Interview File
            SaveInterviewFilesToTempDirectory(fileName, attachmentContent.Result);
        }

        private static void SaveInterviewFilesToTempDirectory(string filename, string interviewFileContent)
        {
            var filePath = String.Format(@"C:\examplehostapplication\temp\{0}", filename);
            File.WriteAllText(filePath, interviewFileContent);
        }

        private static HttpRequestMessage CreateHttpRequestMessage(string hmac, string subscriberId, string packageId, string fileName, DateTime timestamp, string billingRef)
        {
            var interviewFileUrl = string.Format("https://cloud.hotdocs.ws/hdcs/interviewfile/{0}/{1}?filename={2}&billingRef={3}", subscriberId, packageId, fileName, billingRef);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(interviewFileUrl),
                Method = HttpMethod.Get
            };

            // Add request headers
            request.Headers.Add("x-hd-date", timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            request.Headers.TryAddWithoutValidation("Authorization", hmac);
            request.Headers.Add("Keep-Alive", "false");

            return request;
        }

        private static string CalculateHMAC(string signingKey, params object[] paramList)
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

        private static string CanonicalizeParameters(params object[] paramList)
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

