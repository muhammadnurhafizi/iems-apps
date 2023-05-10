using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("ip_chargelines")]
    public class ip_chargelines
    {
        public int id { get; set; }

        [MaxLength(20)]
        public string kod_penjenisan { get; set; }

        [MaxLength(20)]
        public string pejabat { get; set; }

        [MaxLength(5)]
        public string vot_dana { get; set; }

        [MaxLength(2)]
        public string pegawai_pengawal_dipertanggung { get; set;}

        [MaxLength(8)]
        public string kumpulan_ptj_dan_ptj_dipertanggung { get; set; }

        [MaxLength(9)]
        public string program_aktiviti { get; set; }

        [MaxLength(16)]
        public string projek { get; set; }

        [MaxLength(8)]
        public string kod_akaun { get; set; }

        [MaxLength(3)]
        public string kodcawangan { get; set; }

        public string created_at { get; set; }

        public string updated_at { get; set; }

        public string deleted_at { get; set; }

    }
}