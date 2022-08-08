namespace IEMSApps.BusinessObject.DTOs
{
    public class VersionDto
    {
        public string VersionId { get; set; }
        public string DeviceType { get; set; }
        public string Descr { get; set; }
        public string Priority { get; set; }
        public string Url { get; set; }
        public int VersionId_Int
        {
            get
            {
                var result = 1;
                try
                {
                    int.TryParse(VersionId.Replace(".", "").Replace(" ", ""), out result);
                }
                catch (System.Exception ex)
                {

                }
                return result;
            }
        }
    }
}