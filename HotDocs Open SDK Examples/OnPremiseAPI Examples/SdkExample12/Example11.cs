using System;
using HotDocs.Sdk;
using HotDocs.Sdk.Server.Contracts;
using HotDocs.Sdk.Server.OnPremise;

namespace SdkExample11
{
    /// <summary>
    /// This shows how to read variables and dialogs out of a component file
    /// using the on premise web API
    /// </summary>
    internal class Example11
    {
        private static void Main(string[] args)
        {
            const string packageID = "7A7BF8B9-C895-4BC9-BC1A-44E61D6008A2";
            const string hostAddress = "http://localhost:80/HDSWebAPI/api/HDCS";

            var template = GetTemplate(packageID, hostAddress);
            var service = new OnPremiseServices(hostAddress);
            var componentInfo = service.GetComponentInfo(template, true, "logref");

            foreach (var variable in componentInfo.Variables)
            {
                Console.WriteLine(variable.Name);
                Console.WriteLine(variable.Type);
                Console.WriteLine();
            }

            Console.WriteLine("DIALOGS:");

            foreach (var dialog in componentInfo.Dialogs)
            {
                Console.WriteLine(dialog.Name);
            }
            Console.ReadLine();
        }

        private static Template GetTemplate(string packageID, string hostAddress)
        {
            //First argument is unique ID for package in Cloud services
            //Second argument is path to package. We're using a relative one here.
            var templateLocation = new WebServiceTemplateLocation(packageID,
                hostAddress);
            var template = new Template(templateLocation);
            return template;
        }
    }
}
