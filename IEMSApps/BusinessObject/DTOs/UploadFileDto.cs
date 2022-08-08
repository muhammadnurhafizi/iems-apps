using System.IO;

namespace IEMSApps.BusinessObject.DTOs
{
    public class UploadFileDto
    {
        public UploadFileDto()
        {
            ContentType = "application/octet-stream";
        }
        public string Name { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public Stream Stream { get; set; }
        public string Path { get; set; }
    }
}