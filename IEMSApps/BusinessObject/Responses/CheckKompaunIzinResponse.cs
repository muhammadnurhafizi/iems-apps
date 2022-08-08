using IEMSApps.Utils;

namespace IEMSApps.BusinessObject.Responses
{
    public class CheckKompaunIzinResponse
    {
        public Enums.StatusIzinKompaun Status { get; set; }

        public string Catatan { get; set; }
    }
}