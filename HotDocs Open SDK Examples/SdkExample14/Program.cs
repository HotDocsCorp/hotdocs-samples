using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace SdkExample14
{
    // Upload a HotDocs Package File to Cloud Services using an HMAC for authentication
    class Program
    {
        static void Main(string[] args)
        {
            var subscriberId = "example-subscribed-id";
            var signingKey = "example-signing-key";
            var timestamp = DateTime.UtcNow;
            var packageId = "HelloWorld";

            // Generate HMAC using Cloud Services signing key
            var hmac = GetHMAC(signingKey, timestamp, subscriberId, packageId);

            // Create upload request            
            var request = CreateHttpRequestMessage(hmac, subscriberId, packageId, timestamp);

            //Send upload request to Cloud Service
            var client = new HttpClient();
            var response = client.SendAsync(request);            

            Console.WriteLine("Upload:" + response.Result.StatusCode);
            Console.ReadKey();                             
        }

        private static string GetHMAC(string signingKey, DateTime timestamp, string subscriberId, string packageId)
        {
            // Calculate the HMAC
            var hmac = CalculateHMAC(signingKey, timestamp, subscriberId, packageId, null, true, "");
            return hmac;
        }
        
        private static HttpRequestMessage CreateHttpRequestMessage(string hmac, string subscriberId, string packageId, DateTime timestamp)
        {
            var uploadUrl = string.Format("https://cloud.hotdocs.ws/RestfulSvc.svc/{0}/{1}?signature={2}", subscriberId, packageId, hmac);
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(uploadUrl),
                Method = HttpMethod.Put,
                Content = CreateFileContent()
            };
            
            // Add request headers
            request.Content.Headers.TryAddWithoutValidation("x-hd-date", timestamp.ToString("r"));
            request.Content.Headers.TryAddWithoutValidation("Content-Type", "application/binary");
            request.Content.Headers.TryAddWithoutValidation("Authorization", hmac);            

            return request;
        }

        //Create a stream of a HotDocs Template Package file
        private static StreamContent CreateFileContent()
        {
            var filePath = @"C:\temp\HelloWorld.hdpkg";
            var stream = File.OpenRead(filePath);

            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"files\"",
                FileName = "\"" + Path.GetFileName(filePath) + "\""
            };
            
            return fileContent;
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
