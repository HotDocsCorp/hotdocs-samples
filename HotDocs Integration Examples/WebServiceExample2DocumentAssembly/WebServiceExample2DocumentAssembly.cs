using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebServiceExample2DocumentAssembly
{
    internal class WebServiceExample2DocumentAssembly
    {
        static void Main(string[] args)
        {
            // Web Services Subscriber Details
            var subscriberId = "0";            
                        
            // Assembly Request Parameters
            var packageId = "ed40775b-5e7d-4a51-b4d1-32bf9d6e9e29";
            var format = "Native";
            Dictionary<string, string> settings = new Dictionary<string, string>
            {
                {"UnansweredFormat", "[Variable]"}
            };            

            // Create assemble request            
            var request = CreateHttpRequestMessage(subscriberId, packageId, format, settings);

            // Send assemble request to Web Services
            var client = new HttpClient();            
            var response = client.SendAsync(request);
            Console.WriteLine("Assemble:" + response.Result.StatusCode);
            
            // Save Assembled Documents
            var saveDocumentsTask = Task.Run(async () => { await SaveAssembledDocuments(response.Result); });
            saveDocumentsTask.Wait();   
            
            Console.ReadKey();  
        }

        private static HttpRequestMessage CreateHttpRequestMessage(string subscriberId, string packageId, string format, Dictionary<string, string> settings)
        {
            var assembleUrl = CreateAssembleUrl(subscriberId, packageId, format, settings);
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(assembleUrl),
                Method = HttpMethod.Post,
                Content = GetAnswers()
            };

            // Add request headers           
            request.Headers.TryAddWithoutValidation("Content-Type", "text/xml");            
            request.Headers.Add("Keep-Alive", "false");           

            return request;
        }

        private static string CreateAssembleUrl(string subscriberId, string packageId, string format, Dictionary<string, string> settings)
        {
            var assembleUrl = string.Format("http://localhost:80/HDSWEBAPI/api/hdcs/assemble/{0}/{1}?format={2}", subscriberId, packageId, format);

            var assembleUrlWithSettings = new StringBuilder(assembleUrl);
            foreach (var kv in settings)
            {
                assembleUrlWithSettings.AppendFormat("&{0}={1}", kv.Key, kv.Value ?? "");
            }
            return assembleUrlWithSettings.ToString();
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
    }
}

