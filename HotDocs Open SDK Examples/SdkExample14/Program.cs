using System;
using System.Net.Http;
using System.Net.Http.Headers;
using HotDocs.Sdk.Server.Contracts;
using System.IO;

namespace SdkExample14
{
    // Upload a HotDocs Package File to Cloud Services using an HMAC for authentication
    class Program
    {
        static void Main(string[] args)
        {            
            var subscriberId = "example-subscriber-id";
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
            var hmac = HMAC.CalculateHMAC(signingKey, timestamp, subscriberId, packageId, null, true, "");

            // Validate the HMAC
            try
            {
                HMAC.ValidateHMAC(hmac, signingKey, timestamp, subscriberId, packageId, null, true, "");
            }
            catch (HMACException hmacException)
            {
                throw hmacException;
            }

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
    }
}
