using System;
using System.Web;

namespace SdkExample9.Models
{
    public class PackageUploadViewModel
    {
        public HttpPostedFileBase HD_Upload0 { get; set; }
        public HttpPostedFileBase HD_Package_Manifest { get; set; }        
        public PublishTemplateMetaDataModel HD_Template { get; set; }

        public int? TotalTemplatesCount { get; set; }
        public int? CurrentTemplateIndex { get; set; }
    }

    public class PublishTemplateMetaDataModel
    {        
        public string Title0 { get; set; }   
        public string Description0 { get; set; }
        public string LibraryPath0 { get; set; }
        public DateTime? Expiration_Date0 { get; set; }
        public int Warning_Days0 { get; set; }
        public int Extension_Days0 { get; set; }
        public string CommandLineSwitches0 { get; set; }
        public string UploadItemType0 { get; set; }
        public string Filename0 { get; set; }        
    }
}