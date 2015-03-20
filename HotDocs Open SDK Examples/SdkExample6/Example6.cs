using System;
using System.Collections.Generic;
using HotDocs.Sdk;
using HotDocs.Sdk.Server.Contracts;
using HotDocs.Sdk.Server.Local;

namespace SdkExample6
{
    /// <summary>
    /// This shows how to read variables and dialogs out of a component file
    /// </summary>
    internal class Example6
    {
        private static void Main(string[] args)
        {
            var componentInfo = GetComponentInfo();
            var dialogList = GetDialogList(componentInfo);
            var variableList = GetVariableList(componentInfo);
        }

        private static ComponentInfo GetComponentInfo()
        {
            var template = CreateTemplate();
            var service = CreateHotDocsService();
            var componentInfo = service.GetComponentInfo(template, true, "logref");
            return componentInfo;
        }

        private static Dictionary<string, bool> GetDialogList(ComponentInfo componentInfo)
        {
            Dictionary<string, bool> dialogList = new Dictionary<string, bool>();
            foreach (DialogInfo dialog in componentInfo.Dialogs)
            {
                dialogList.Add(dialog.Name, dialog.Repeat);
            }
            return dialogList;
        }

        private static Dictionary<string, string> GetVariableList(ComponentInfo componentInfo)
        {
            Dictionary<string, string> variableList = new Dictionary<string, string>();
            foreach (var variable in componentInfo.Variables)
            {
                variableList.Add(variable.Name, variable.Type);                
            }
            return variableList;
        }               

        private static Services CreateHotDocsService()
        {
            const string tempDirectoryPath = @"C:\temp\";
            var service = new Services(tempDirectoryPath);
            return service;
        }        

        private static Template CreateTemplate()
        {
            const string packagePath = @"C:\temp\Demo.hdpkg";
            string packageId = Guid.NewGuid().ToString();
            var templateLocation = new PackagePathTemplateLocation(packageId, packagePath);

            var template = new Template(templateLocation);
            return template;
        }
    }
}