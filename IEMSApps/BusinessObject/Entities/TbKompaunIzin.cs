using IEMSApps.Utils;
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbkompaun_izin")]
    public class TbKompaunIzin 
    {
        [PrimaryKey]
        [MaxLength(20)]
        public string NoRujukanKpp { get; set; }

        [MaxLength(3)]
        public string KodCawangan { get; set; }

        public string TrkhMohon { get; set; }

        public Enums.StatusIzinKompaun Status{ get; set; }

        public int PgnDaftar { get; set; }

        public string TrkhDaftar { get; set; }

        public int PgnAkhir { get; set; }

        public string TrkhAkhir { get; set; }

        public string Catatan { get; set; }

        public int ip_status_api { get; set; }
    }
}