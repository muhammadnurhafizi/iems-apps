namespace IEMSApps.BusinessObject.Responses
{
    public class MaklumatPembayaranResponse
    {
        public string kod_proses { get; set; }
        public string kod_perkhidmatan_iPayment { get; set; }
        public string tarikh_dan_masa { get; set; }
        public string nombor_rujukan_message { get; set; }
        public string kod_respond { get; set; }
        public string perihal_respond { get; set; }
        public string mesej { get; set; }
    }
}