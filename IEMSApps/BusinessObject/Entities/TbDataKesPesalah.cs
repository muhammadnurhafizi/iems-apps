using IEMSApps.Utils;
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbdatakes_pesalah")]
    public class TbDataKesPesalah
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(20)]
        public string NoKes { get; set; }

        [MaxLength(3)]
        public string KodCawangan { get; set; }

        [MaxLength(80)]
        public string NamaOks { get; set; }

        [MaxLength(50)]
        public string NoKpOks { get; set; }

        [MaxLength(300)]
        public string AlamatOks1 { get; set; }

        [MaxLength(80)]
        public string AlamatOks2 { get; set; }

        [MaxLength(80)]
        public string AlamatOks3 { get; set; }

        public Enums.StatusOnline IsSendOnline { get; set; }

        public int PgnDaftar { get; set; }

        public string TrkhDaftar { get; set; }
    }
}