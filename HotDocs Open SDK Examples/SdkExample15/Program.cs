using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.IO;

namespace SdkExample15
{
    class Program
    {
        static void Main(string[] args)
        {
            var subscriberId = "example-subscriber-id";
            var signingKey = "example-signing-key";
            var timestamp = DateTime.UtcNow.ToUniversalTime();
            var packageId = "HelloWorld";

            // Generate HMAC using Cloud Services signing key
            var hmac = GetHMAC(signingKey, timestamp, subscriberId, packageId);

            // Create upload request
            var client = new HttpClient();
            var request = CreateHttpRequestMessage(hmac, subscriberId, packageId, timestamp);

            //Send upload request to Cloud Service
            var response = client.SendAsync(request);

            Console.WriteLine("Interview:" + response.Result.StatusCode);
            Console.ReadKey();
        }

        private static string GetHMAC(string signingKey, DateTime timestamp, string subscriberId, string packageId)
        {
            // Calculate the HMAC
            var hmac = CalculateHMAC(signingKey, timestamp, subscriberId, packageId);
            try
            {
                ValidateHMAC(hmac, signingKey, timestamp, subscriberId, packageId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return hmac;
        }

        //Create new Http Request
        private static HttpRequestMessage CreateHttpRequestMessage(string hmac, string subscriberId, string packageId, DateTime timestamp)
        {            
            string billingRef = "";
            var tempImageUrl = "";
            var markedVariables = "";
            var template = "HelloWorld";            
            string interviewFormat = "JavaScript";


            //"interview/{subscriberID}/{packageID}/{templatename=null}?format={format}&markedvariables={markedVariables}&tempimageurl={tempImageUrl}&billingref={billingRef}&date={date}&signature={signature}"

            var interviewURL = string.Format("https://cloud.hotdocs.ws/RestfulSvc.svc/interview/{0}/{1}/{2}?format={3}&markedvariables={4}&tempimageurl={5}&billingref={6}&date={7}&signature={8}", subscriberId, packageId, template, interviewFormat, string.Join(",", markedVariables), tempImageUrl, billingRef, timestamp, hmac);
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(interviewURL),
                Method = HttpMethod.Post,
                Content = new StringContent("")
            };

            // Add request headers                                          
            request.Content.Headers.TryAddWithoutValidation("x-hd-date", timestamp.ToString("r"));
            request.Content.Headers.TryAddWithoutValidation("Content-Type", "text/xml");
            request.Content.Headers.TryAddWithoutValidation("Authorization", hmac);

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

        public static void ValidateHMAC(string hmac, string signingKey, params object[] paramList)
        {
            string calculatedHMAC = CalculateHMAC(signingKey, paramList);

            if (hmac != calculatedHMAC)
            {
                throw new Exception("Invalid Request Parameters");
            }
        }
    }
}
