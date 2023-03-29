using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("ip_identiti_pelanggans")]
    public class ip_identiti_pelanggans
    {
        public int id { get; set; }
        public string jenis_identiti { get; set; }
        public string kod_identiti { get; set; }
        public string kategori_identiti { get; set; }

    }
}