using System;
using System.IO;
using System.Web.Mvc;
using HostApplicationUploadPageExample.Models;

namespace HostApplicationUploadPageExample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(int? totalTemplateCount)
        {            
            int TemplatesLeftToUpload = Convert.ToInt32(Request.QueryString["HD_NumberTemplates"]);
            PackageUploadViewModel model = new PackageUploadViewModel
            {
                HD_Template = new PublishTemplateMetaDataModel(),
                CurrentTemplateIndex = ((totalTemplateCount - TemplatesLeftToUpload) + 1) ?? 1,
                TotalTemplatesCount = totalTemplateCount ?? TemplatesLeftToUpload,                
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(PackageUploadViewModel model)
        {
            string packageName = Path.ChangeExtension(model.HD_Template.Title0, "hdpkg");            
            string packageLocation = Path.Combine(@"C:\temp5\", packageName);

            using (FileStream newFile = System.IO.File.Create(packageLocation))
            {
                model.HD_Upload0.InputStream.Seek(0, SeekOrigin.Begin);
                model.HD_Upload0.InputStream.CopyTo(newFile);
            }

            int currentTemplateIndex = Convert.ToInt32(Request.Form["hdnCurrentTemplateIndex"]);
            if (currentTemplateIndex == model.TotalTemplatesCount)
            {
                return View("Published");
            }

            return Index(model.TotalTemplatesCount);
        }

    }
}
