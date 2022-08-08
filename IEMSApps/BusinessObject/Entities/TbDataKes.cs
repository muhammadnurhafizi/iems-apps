using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbdatakes")]
    public class TbDataKes : BaseEntities
    {
        [PrimaryKey]
        [MaxLength(20)]
        public string NoKes { get; set; }

        [MaxLength(3)]
        public string KodCawangan { get; set; }

        [MaxLength(30)]
        public string NoEp { get; set; }

        [MaxLength(30)]
        public string NoIp { get; set; }

        public string TrkhSalah { get; set; }

        //[MaxLength(10)]
        //public string KodAkta { get; set; } //moved

        //public int KodSalah { get; set; } //moved

        //[MaxLength(1000)]
        //public string ButirSalah { get; set; } //moved

        [MaxLength(150)]
        public string Tempat { get; set; }

        [MaxLength(20)]
        public string NoKpp { get; set; }

        [MaxLength(20)]
        public string NoKmp { get; set; } //new
        [MaxLength(2)]
        public string KodKatKawasan { get; set; } //new
        public int KodTujuan { get; set; } //new

        public int? PegawaiSerbuan { get; set; }

        //[MaxLength(80)]
        //public string NamaOks { get; set; }//moved

        //[MaxLength(12)]
        //public string NoKpOks { get; set; }//moved

        //[MaxLength(80)]
        //public string AlamatOks1 { get; set; }//moved

        //[MaxLength(80)]
        //public string AlamatOks2 { get; set; }//moved

        //[MaxLength(80)]
        //public string AlamatOks3 { get; set; }//moved

        [MaxLength(80)]
        public string NamaPremis { get; set; }

        [MaxLength(15)]
        public string NoDaftarPremis { get; set; }

        public int? KodKatPerniagaan { get; set; }
        public int? KodJenama { get; set; }

        [MaxLength(50)]
        public string NoLaporanPolis { get; set; } //new
        [MaxLength(45)]
        public string KelasKes { get; set; } //new if tbkpp.tindakan = 1 and tbkpp.kodkatpremis = 1 then "A1", if tbkpp.tindakan = 1 and tbkpp.kodkatpremis = 2 then "A2"
        [MaxLength(2)]
        public string KodStatusKes { get; set; } //new default to "BS"
        [MaxLength(6)]
        public string KodStatusKes_Det { get; set; } //new if tbkpp.tindakan = 1 then "BS04", if tbkpp.tindakan = 2 then "BS01"

    }
}