
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbhandheld")]
    public class TbHandheld : BaseEntities
    {
        [PrimaryKey]
        [MaxLength(6)]
        public string IdHh { get; set; }

        [MaxLength(3)]
        public string KodCawangan { get; set; }
        [MaxLength(6)]
        public string KodAset_Peranti { get; set; }
        [MaxLength(6)]
        public string KodAset_Pencetak { get; set; }
        [MaxLength(30)]
        public string AppVer { get; set; }
        public int NotUrutan_Kpp { get; set; }
        public int Jumlah_Kpp { get; set; }
        public int Jumlah_Gambar_Kpp { get; set; }
        public int NotUrutan_Kots { get; set; }
        public int Jumlah_Kots { get; set; }
        public int Jumlah_Ak { get; set; }
        public int Jumlah_Gambar_Kots { get; set; }
        public int NotUrutan_DataKes { get; set; }
        public int Jumlah_DataKes{ get; set; }
        public int Jumlah_Gambar_DataKes { get; set; }
        public int Jumlah_Nota { get; set; }

        public string TrkhHhCheckin { get; set; }
        public string TrkhHhCheckout { get; set; }
        public string TrkhUpdateDate { get; set; }
        public int Year { get; set; }
    }
}