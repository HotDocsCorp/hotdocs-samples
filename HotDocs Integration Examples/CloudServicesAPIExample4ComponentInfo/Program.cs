using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudServicesAPIExample4ComponentInfo
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
            string templateName = null;
            var sendPackage = false;
            var billingRef = "";
            var includeDialogs = false;            

            // Generate HMAC using Cloud Services signing key
            var hmac = CalculateHMAC(signingKey, timestamp, subscriberId, packageId, templateName, sendPackage, billingRef, includeDialogs);

            // Create component information request            
            var request = CreateHttpRequestMessage(hmac, subscriberId, packageId, timestamp, includeDialogs);                        

            // Send component information request to Cloud Service
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
        
        private static HttpRequestMessage CreateHttpRequestMessage(string hmac, string subscriberId, string packageId, DateTime timestamp, bool includeDialogs)
        {
            var componentInfoUrl = string.Format("https://cloud.hotdocs.ws/hdcs/componentinfo/{0}/{1}?includeDialogs={2}", subscriberId, packageId, includeDialogs);
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(componentInfoUrl),
                Method = HttpMethod.Get                
            };

            // Add request headers
            request.Headers.TryAddWithoutValidation("x-hd-date", timestamp.ToString("yyyy-MM-ddTHH:mm:ssZ"));
            request.Headers.TryAddWithoutValidation("Authorization", hmac);                            

            return request;
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
