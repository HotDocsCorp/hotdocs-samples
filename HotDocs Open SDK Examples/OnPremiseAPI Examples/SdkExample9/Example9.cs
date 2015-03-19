﻿using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SdkExample9
{
    /// <summary>
    ///     This shows how to use new On Premise API to upload a package
    ///     If successful a folder named with the package id ed40775b-5e7d-4a51-b4d1-32bf9d6e9e29 will be created 
    ///     within the TempFiles folder of the On Premise API solution, within which should be the file Demo.hdpkg
    /// </summary>
    internal class Example9
    {
        private static readonly string uriUpload = "http://localhost:80/HDSWEBAPI/api/HDCS/0/7A7BF8B9-C895-4BC9-BC1A-44E61D6008A2";

        private static void Main(string[] args)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(uriUpload),
                    Method = HttpMethod.Put,
                };
                using (var fs = File.OpenRead("Demo.hdpkg"))
                {
                    request.Content = CreateFileContent(fs,"Demo.hdpkg");
                    request.Content.Headers.TryAddWithoutValidation("Content-Type", "application/binary");
                    Console.WriteLine("Uploading");
                    var result = client.SendAsync(request).Result;
                    Console.WriteLine("Finished Uploading Status Code: "+result.StatusCode);
                    Console.ReadLine();
                }
            }
        }

        //You can upload a template with a filename
        private static StreamContent CreateFileContent(Stream stream, string fileName)
        {
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "\"files\"",
                FileName = "\""+fileName+"\""
            };
            return fileContent;
        }

        //Or without a filename
        private static StreamContent CreateFileContentNoDisposition(Stream stream)
        {
            var fileContent = new StreamContent(stream);
            return fileContent;
        }
    }
}
