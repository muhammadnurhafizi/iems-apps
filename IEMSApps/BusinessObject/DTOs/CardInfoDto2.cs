namespace IEMSApps.BusinessObject.DTOs
{
    public class CardInfoDto2
    {

        public string name { get; set; }
        public string gmpcName { get; set; }
        public string kptName { get; set; }
        public string icNo { get; set; }
        public string oldIcNo { get; set; }
        public string dob { get; set; }
        public string pob { get; set; }
        public string gender { get; set; }
        public string citizenship { get; set; }
        public string issueDate { get; set; }
        public string race { get; set; }
        public string religion { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string postcode { get; set; }
        public string city { get; set; }
        public string state { get; set; }

        public bool IsSuccessRead { get; set; }
        public string Message { get; set; }

    }
}