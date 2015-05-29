using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SdkExample10
{
    /// <summary>
    ///     Upload a Template Package file using the On-Premise Service API
    ///     If successful a folder named with the package id ed40775b-5e7d-4a51-b4d1-32bf9d6e9e29 will be created 
    ///     within the TempFiles folder of the On Premise API solution, within which should be the file Demo.hdpkg
    /// 
    ///     This will test uploading the package named ‘HelloWorld.hdpkg’ to the on premise web API. 
    ///     To test replace the hardcoded host address for uriUpload at line 21 with your relevant address.
    ///     If successful, a folder named ‘ed40775b-5e7d-4a51-b4d1-32bf9d6e9e29’ will be created inside the ‘TempFiles’ folder in the on premise web API solution. 
    ///     Within that folder should be the actual package file ‘HelloWorld.hdpkg’.  If you replace the use of method CreateFileContent on line 34 with CreateFileContentNoDisposition then the file
    ///     will instead be named after the packageID.
    /// </summary>
    internal class Example10
    {
        private static void Main()
        {            
            
            //Create Http Request
            var request = CreateHttpRequestMessage();

            var result = GetResponse(request);
            
        }       

        private static async Task GetResponse(HttpRequestMessage request)
        {
            var result = await SendRequest(request);
            Console.WriteLine("Finished Uploading. Status Code: " + result.StatusCode);
            Console.ReadLine();

        }

        private static async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
        {
            
            //Create Http Client, to send the request and receive the response
            var client = new HttpClient();

            //Send request and receive result
            var result = client.SendAsync(request).Result;

            return result;
        }

        //Create new Http Request
        private static HttpRequestMessage CreateHttpRequestMessage()
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost:80/HDSWEBAPI/api/HDCS/0/7A7BF8B9-C895-4BC9-BC1A-44E61D6008A5"),
                Method = HttpMethod.Put,
                Content = CreateFileContent(),
            };

            request.Content.Headers.TryAddWithoutValidation("Content-Type", "application/binary");
            return request;
        }

        //Upload a template with a filename
        private static StreamContent CreateFileContent()
        {
            const string filePath = @"C:\temp\HelloWorld.hdpkg";
            var stream = File.OpenRead(filePath);

            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"files\"",
                FileName = "\"" + Path.GetFileName(filePath) + "\""
            };

            stream.Close();
            return fileContent;
        }

        //Upload a template without a filename
        private static StreamContent CreateFileContentNoDisposition(Stream stream)
        {
            var fileContent = new StreamContent(stream);
            stream.Close();
            return fileContent;
        }
    }
}
