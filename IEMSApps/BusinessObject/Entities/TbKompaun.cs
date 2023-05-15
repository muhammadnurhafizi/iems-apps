
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbkompaun")]
    public class TbKompaun : BaseEntities
    {
        [PrimaryKey]
        [MaxLength(20)]
        public string NoKmp { get; set; }

        [MaxLength(6)]
        public string IdHh { get; set; }

        [MaxLength(1)]
        public string JenisKmp { get; set; } //*Jenis Kompaun*\n1 = KOTS\n2 = Kompaun Biasa',

        [MaxLength(3)]
        public string KodCawangan { get; set; }

        public int KodKatPremis { get; set; }

        [MaxLength(1)]
        public string JenisPesalah { get; set; } //*Jenis Pesalah"\n1 = Individu\n2 = Syarikat',

        [MaxLength(80)]
        public string NamaOkk { get; set; }

        [MaxLength(50)]
        public string NoKpOkk { get; set; }

        [MaxLength(150)]
        public string NamaPremis { get; set; }

        [MaxLength(15)]
        public string NoDaftarPremis { get; set; }

        [MaxLength(300)]
        public string AlamatOkk1 { get; set; }
        [MaxLength(80)]
        public string AlamatOkk2 { get; set; }
        [MaxLength(80)]
        public string AlamatOkk3 { get; set; }


        public string TrkhKmp { get; set; }

        [MaxLength(50)]
        public string NoLaporanPolis { get; set; }

        [MaxLength(50)]
        public string NoLaporanCwgn { get; set; }

        public string TrkhSalah { get; set; }

        [MaxLength(300)]
        public string TempatSalah { get; set; }

        [MaxLength(10)]
        public string KodAkta { get; set; }

        public int KodSalah { get; set; }

        [MaxLength(1000)]
        public string ButirSalah { get; set; }

        [MaxLength(1)]
        public string IsArahanSemasa { get; set; }

        public int TempohTawaran { get; set; }

        public decimal AmnKmp { get; set; }

        public decimal AmnByr { get; set; }

        [MaxLength(20)]
        public string NoResit { get; set; }

        public int PegawaiPengeluar { get; set; }

        [MaxLength(20)]
        public string NoRujukanKpp { get; set; }

        [MaxLength(80)]
        public string NamaPenerima { get; set; }

        [MaxLength(50)]
        public string NoKpPenerima { get; set; }

        [MaxLength(80)]
        public string AlamatPenerima1 { get; set; }
        [MaxLength(80)]
        public string AlamatPenerima2 { get; set; }
        [MaxLength(80)]
        public string AlamatPenerima3 { get; set; }

        public string TrkhPenerima { get; set; }

        public string IsCetakAkuan { get; set; } //Akuan Dicetak*\\\\n1 = Ya\\\\n2 = Tidak',

        [MaxLength(80)]
        public string NamaPenerima_Akuan { get; set; }

        [MaxLength(50)]
        public string NoKpPenerima_Akuan { get; set; }

        [MaxLength(80)]
        public string AlamatPenerima1_Akuan { get; set; }
        [MaxLength(80)]
        public string AlamatPenerima2_Akuan { get; set; }
        [MaxLength(80)]
        public string AlamatPenerima3_Akuan { get; set; }

        public string TrkhPenerima_Akuan { get; set; }

        [MaxLength(30)]
        public string NoEp { get; set; }

        [MaxLength(30)]
        public string NoIp { get; set; }

        [MaxLength(300)]
        public string BarangKompaun { get; set; }

        public int ip_identiti_pelanggan_id { get; set; }

        [MaxLength(5)]
        public string poskodpenerima { get; set; }

        [MaxLength(35)]
        public string bandarpenerima { get; set; }

        [MaxLength(2)]
        public string negeripenerima { get; set; }

        [MaxLength(2)]
        public string negarapenerima { get; set; }

        [MaxLength(15)]
        public string notelpenerima { get; set; }

        [MaxLength(100)]
        public string emelpenerima { get; set; }

        [MaxLength(5)]
        public string poskodpenerima_akuan { get; set; }

        [MaxLength(35)]
        public string bandarpenerima_akuan { get; set; }

        [MaxLength(2)]
        public string negeripenerima_akuan { get; set; }

        [MaxLength(2)]
        public string negarapenerima_akuan { get; set; }

        [MaxLength(15)]
        public string notelpenerima_akuan { get; set; }

        [MaxLength(100)]
        public string emelpenerima_akuan { get; set; }

        public int ip_identiti_pelanggan_id_akuan { get; set; }

        [MaxLength(2)]
        public int isbayarmanual { get; set; }

        public string gambarbuktibayaran { get; set; }
    }
}