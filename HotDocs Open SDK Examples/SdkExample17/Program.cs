using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SdkExample17
{
    class Program
    {
        static void Main(string[] args)
        {            
            // Cloud Services Subscription Details
            var subscriberId = "example-subscriber-id";
            var signingKey = "example-signing-key";
                        
            // HMAC calculation data
            var timestamp = DateTime.UtcNow;
            var packageId = "HelloWorld";
            var format = "Native";
            var templateName = "";
            var sendPackage = false;
            var billingRef = "";
            object[] settings = null;

            // Generate HMAC using Cloud Services signing key            
            var hmac = CalculateHMAC(signingKey, timestamp, subscriberId, packageId, templateName, sendPackage, billingRef, format, settings);

            // Create assemble request            
            var request = CreateHttpRequestMessage(hmac, subscriberId, packageId, timestamp, format);

            //Send assemble request to Cloud Services
            var client = new HttpClient();            
            var response = client.SendAsync(request);
            Console.WriteLine("Assemble:" + response.Result.StatusCode);
            
            // Save Assembled Documents
            var saveDocumentsTask = Task.Run(async () => { await SaveAssembledDocuments(response.Result); });
            saveDocumentsTask.Wait();   
            
            Console.ReadKey();  
        }

        private static HttpRequestMessage CreateHttpRequestMessage(string hmac, string subscriberId, string packageId, DateTime timestamp, string format)
        {
            var assembleUrl = string.Format("https://cloud.hotdocs.ws/RestfulSvc.svc/assemble/{0}/{1}?format={2}", subscriberId, packageId, format);
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(assembleUrl),
                Method = HttpMethod.Post,
                Content = GetAnswers()
            };

            // Add request headers
            request.Headers.TryAddWithoutValidation("x-hd-date", timestamp.ToString("r"));
            request.Headers.TryAddWithoutValidation("Content-Type", "text/xml");
            request.Headers.TryAddWithoutValidation("Authorization", hmac);
            request.Headers.Add("Keep-Alive", "false");           

            return request;
        }

        static async Task SaveAssembledDocuments(HttpResponseMessage response)
        {
            MultipartStreamProvider multipartStream = await response.Content.ReadAsMultipartAsync();
            foreach (var attachment in multipartStream.Contents)
            {                                   
                Stream writeAttachmentStream = await attachment.ReadAsStreamAsync();
                using (FileStream output = new FileStream(@"C:\temp\" + attachment.Headers.ContentDisposition.FileName, FileMode.Create))
                {
                    writeAttachmentStream.CopyTo(output);
                }
            }
        }

        private static StringContent GetAnswers()
        {
            return new StringContent(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?><AnswerSet version=""1.1""><Answer name=""TextExample-t""><TextValue>Hello World</TextValue></Answer></AnswerSet >");
        }

        public static string CalculateHMAC(string signingKey, params object[] paramList)
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

        public static string CanonicalizeParameters(params object[] paramList)
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
