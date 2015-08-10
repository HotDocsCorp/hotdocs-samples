using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;

namespace WebServiceExample4ComponentInfo
{
    /// <summary>
    /// This shows how to read variables and dialogs out of a component file
    /// using the on premise web API
    /// 
    ///This tests retrieving the component info from a package. It assumes relevant package exists in on premise API package folder (TempFiles).
    ///1. On line 20 and 21 set your unique packageID (Should be the GUID of your package on the server) 
    ///And also the address of the webapi as the HostAddress ("http://localhost:52948/HDSWebAPI/api/HDCS").
    ///2. The variables and dialogs should be returned in the console variables first then dialogs.
    /// </summary>
    internal class WebServiceExample4ComponentInfo
    {
        static void Main(string[] args)
        {
            // Web Services Subscriber Details
            string subscriberId = "0";            
                        
            // Assembly Request Parameters
            var packageId = "ed40775b-5e7d-4a51-b4d1-32bf9d6e9e29";
            var includeDialogs = true;

            // Create assemble request            
            var request = CreateHttpRequestMessage(subscriberId, packageId, includeDialogs);

            // Send component information request
            var client = new HttpClient();            
            var response = client.SendAsync(request);            

            Console.WriteLine("Component File Info:" + response.Result.StatusCode);

            //Read Component Information from response and write to console
            var componentInfoXml = GetComponentInfoXmlFromResponse(response);
            Console.WriteLine(componentInfoXml);

            Console.ReadKey();         
        }

        private static string GetComponentInfoXmlFromResponse(Task<HttpResponseMessage> response)
        {
            var responseStream = response.Result.Content;
            return responseStream.ReadAsStringAsync().Result;
        }

        private static HttpRequestMessage CreateHttpRequestMessage(string subscriberId, string packageId, bool includeDialogs)
        {
            var assembleUrl = string.Format("http://localhost:80/HDSWEBAPI/api/hdcs/componentinfo/{0}/{1}?includeDialogs={2}", subscriberId, packageId, includeDialogs);
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(assembleUrl),
                Method = HttpMethod.Get
            };

            // Add request headers           
            request.Headers.TryAddWithoutValidation("Content-Type", "text/xml");            
            request.Headers.Add("Keep-Alive", "false");           

            return request;
        }          
    }
}
