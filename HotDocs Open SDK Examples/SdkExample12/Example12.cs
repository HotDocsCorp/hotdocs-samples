using System;
using HotDocs.Sdk;
using HotDocs.Sdk.Server.OnPremise;

namespace SdkExample12
{
    /// <summary>
    /// This shows how to read variables and dialogs out of a component file
    /// using the on premise web API
    /// 
    ///This will test getting the component info from a package. It assumes relevant package exists in on premise API package folder (TempFiles).
    ///1. On line 20 and 21 set your unique packageID (Should be the GUID of your package on the server) 
    ///And also the address of the webapi as the HostAddress ("http://localhost:52948/HDSWebAPI/api/HDCS").
    ///2. The variables and dialogs should be returned in the console variables first then dialogs.
    /// </summary>
    internal class Example12
    {
        private static void Main()
        {
            const string packageId = "7A7BF8B9-C895-4BC9-BC1A-44E61D6008A2";
            const string hostAddress = "http://localhost:80/HDSWebAPI/api/HDCS";

            var template = GetTemplate(packageId, hostAddress);
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

        private static Template GetTemplate(string packageId, string hostAddress)
        {
            //First argument is unique ID for package
            //Second argument is path to package. We're using a relative one here.
            var templateLocation = new WebServiceTemplateLocation(packageId,
                hostAddress);
            var template = new Template(templateLocation);
            return template;
        }
    }
}
