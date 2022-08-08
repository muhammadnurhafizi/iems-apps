namespace IEMSApps.BusinessObject.DTOs
{
    public class ConfigAppDto
    {
        public int IntervalInSecond { get; set; }

        public int DistanceInMeter { get; set; }
        public string WebServiceUrl { get; set; }
        public int IntervalBackgroundServiceInSecond { get; set; }
    }
}