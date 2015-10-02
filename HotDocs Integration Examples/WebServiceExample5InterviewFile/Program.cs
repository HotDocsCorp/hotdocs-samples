using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebServiceExample5InterviewFile
{
    class Program
    {
        static void Main(string[] args)
        {
            // Web Services Subscription Details
            string subscriberId = "0";
            
            // Request data      
            var packageId = "ed40775b-5e7d-4a51-b4d1-32bf9d6e9e29";
            var fileName = "HelloWorld.docx.js";                                    

            // Create Interview File request            
            var request = CreateHttpRequestMessage(subscriberId, packageId, fileName);

            //Send Interview File request to Web Services
            var client = new HttpClient();
            var response = client.SendAsync(request);
            response.Wait();

            // Read Interview File content from Response
            var attachmentContent = response.Result.Content.ReadAsStringAsync();
            attachmentContent.Wait();

            // Save Interview File
            SaveInterviewFilesToTempDirectory(fileName, attachmentContent.Result);
        }

        private static void SaveInterviewFilesToTempDirectory(string filename, string interviewFile)
        {                
                var filePath = String.Format(@"C:\examplehostapplication\temp\{0}", filename);
                File.WriteAllText(filePath, interviewFile);            
        }

        private static HttpRequestMessage CreateHttpRequestMessage(string subscriberId, string packageId, string fileName)
        {
            var interviewFileUrl = string.Format("http://localhost:80/HDSWEBAPI/api/hdcs/interviewfile/{0}/{1}?filename={2}", subscriberId, packageId, fileName);

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(interviewFileUrl),
                Method = HttpMethod.Get
            };

            // Add request headers
            request.Headers.Add("Keep-Alive", "false");

            return request;
        }
    }
}
