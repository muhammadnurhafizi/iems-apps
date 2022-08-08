
using IEMSApps.Utils;

namespace IEMSApps.BusinessObject.DTOs
{
    public class LoginDto
    {
        public string UserId { get; set; }
        public string Nama { get; set; }
        public string NoKp { get; set; }
        public string KodCawangan { get; set; }
        public int KodPasukan { get; set; }
        public int KodNegeri { get; set; }
        public string HandheldId { get; set; }

        public int StaffId { get; set; }

        public Enums.JenisPengguna JenisPengguna { get; set; }
    }
}