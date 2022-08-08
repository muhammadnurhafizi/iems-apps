using IEMSApps.Utils;
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbdatakes_kesalahan")]
    public class TbDataKesKesalahan
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(20)]
        public string NoKes { get; set; }

        [MaxLength(3)]
        public string KodCawangan { get; set; }

        [MaxLength(10)]
        public string KodAkta { get; set; } //moved

        public int KodSalah { get; set; } //moved

        [MaxLength(1000)]
        public string ButirSalah { get; set; } //moved

        public Enums.StatusOnline IsSendOnline { get; set; }

        public int PgnDaftar { get; set; }

        public string TrkhDaftar { get; set; }
    }
}