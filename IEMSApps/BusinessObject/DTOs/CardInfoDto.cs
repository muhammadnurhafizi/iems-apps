namespace IEMSApps.BusinessObject.DTOs
{
    public class CardInfoDto
    {
        public string GMPCName { get; set; }
        public string KPTName { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string birthPlace { get; set; }
        public string cardVersion { get; set; }

        public string category { get; set; }
        public string citizenship { get; set; }
        public string city { get; set; }
        public string dob { get; set; }
        public string doi { get; set; }
        public string eastMalaysian { get; set; }
        public string gender { get; set; }
        public string greenCardNationality { get; set; }
        public string idNum { get; set; }
        public string oldIdNum { get; set; }
        public string originalName { get; set; }
        public string otherID { get; set; }
        public string postcode { get; set; }
        public string race { get; set; }
        public string religion { get; set; }
        public string state { get; set; }

        public bool IsSuccessRead { get; set; }
        public string Message { get; set; }
    }
}