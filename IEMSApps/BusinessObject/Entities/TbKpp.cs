
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbkpp")]
    public class TbKpp : BaseEntities
    {
        [PrimaryKey]
        [MaxLength(20)]
        public string NoRujukanKpp { get; set; }

        [MaxLength(6)]
        public string IdHh { get; set; }

        [MaxLength(3)]
        public string KodCawangan { get; set; }

        public int KodTujuan { get; set; }

        public int KodAsas { get; set; }

        public int KodJenis { get; set; }

        [MaxLength(250)]
        public string CatatanLawatan { get; set; }

        [MaxLength(2)]
        public string KodKatKawasan { get; set; }

        [MaxLength(13)]
        public string NoRujukanAtr { get; set; }

        [MaxLength(13)]
        public string NoSiriBorangKpp { get; set; }
        
        public string TrkhMulaLawatankpp { get; set; }
        
        public string TrkhTamatLawatanKpp { get; set; }

        public int KodKatPremis { get; set; }

        [MaxLength(150)]
        public string NamaPremis { get; set; }

        [MaxLength(300)]
        public string AlamatPremis1 { get; set; }
        [MaxLength(80)]
        public string AlamatPremis2 { get; set; }
        [MaxLength(80)]
        public string AlamatPremis3 { get; set; }


        [MaxLength(15)]
        public string NoDaftarPremis { get; set; }

        [MaxLength(15)]
        public string NoLesenBKP_PDA { get; set; }

        [MaxLength(15)]
        public string NoLesenMajlis_Permit { get; set; }

        [MaxLength(12)]
        public string NoTelefonPremis { get; set; }

        [MaxLength(150)]
        public string CatatanPremis { get; set; }

        public int PengeluarKpp { get; set; }
        
        [MaxLength(15)]
        public string LongitudPremis { get; set; }

        [MaxLength(15)]
        public string LatitudPremis { get; set; }

        public int Amaran { get; set; } //1 yes, 2=no

        [MaxLength(80)]
        public string LokasiLawatan { get; set; }
      
        [MaxLength(15)]
        public string NoAduan { get; set; }

        [MaxLength(1000)]
        public string HasilLawatan { get; set; }

        public string GambarLawatan1 { get; set; }
        public string GambarLawatan2 { get; set; }

        public int Tindakan { get; set; } //'*Tindakan*\n1 = KOTS\n2 = Siasatan Lanjut\n3 = Tiada Kes',

        [MaxLength(50)]
        public string NamaPenerima { get; set; }
        [MaxLength(50)]
        public string NoKpPenerima { get; set; }

        [MaxLength(50)]
        public string Jawatanpenerima { get; set; }

        [MaxLength(80)]
        public string AlamatPenerima1 { get; set; }
        [MaxLength(80)]
        public string AlamatPenerima2 { get; set; }
        [MaxLength(80)]
        public string AlamatPenerima3 { get; set; }

        public string TrkhPenerima { get; set; }

        public int SetujuByr { get; set; } //'*Setuju Bayar*\n1 = X\n2 = \n',

        //kesalahan
        //`JenisPesalah`	varchar( 1 ),
        //    `KodAkta`	varchar( 10 ),
        //    `KodSalah`	INTEGER,
        //`ButirSalah`	varchar( 1000 ),
        //    `IsArahanSemasa`	varchar( 1 ),
        //    `TempohTawaran`	INTEGER,
        //`AmnKmp`	float,	
        [MaxLength(1)]
        public string JenisPesalah { get; set; } //*Jenis Pesalah"\n1 = Individu\n2 = Syarikat',

        [MaxLength(10)]
        public string KodAkta { get; set; }

        public int KodSalah { get; set; }

        [MaxLength(1000)]
        public string ButirSalah { get; set; }

        [MaxLength(1)]
        public string IsArahanSemasa { get; set; }

        public int TempohTawaran { get; set; }

        public decimal AmnKmp { get; set; }


        [MaxLength(30)]
        public string NoEp { get; set; }

        [MaxLength(30)]
        public string NoIp { get; set; }

        public int IsSkipIzin { get; set; }

        //new add : Penerima
        //[MaxLength(30)]
        //public string Kewarganegaraan { get; set; }

        //[MaxLength(15)]
        //public string NegaraAsal { get; set; }

        [MaxLength(15)]
        public string notelpenerima { get; set; }

        [MaxLength(100)] 
        public string emelpenerima { get; set; }

        [MaxLength(2)]
        public string negeripenerima { get; set; }

        [MaxLength(35)]
        public string bandarpenerima { get; set; }

        [MaxLength(5)]
        public string poskodpenerima { get; set;}

        [MaxLength(2)]
        public string negarapenerima { get; set; }

        public int ip_identiti_pelanggan_id { get; set; }

        //[MaxLength(1)] 
        //public int npmb { get; set; } //notis pengesahan maklumat barang

        //[MaxLength(1)]
        //public int nb { get; set; }   //nama barang

        //new add - lawatan
        //[MaxLength(2)]
        //public int lokalitikategorikhas { get; set; } //1-Pulau, 2-Sempadan, 3-Kawasan Peridustrian, 4-pemborong berlesen, 5-Pemegang permit biasa, 6-aktiviti bunkering 

        //[MaxLength(10)]
        //public string kodagensiterlibat { get; set; }

        [MaxLength(2)]
        public int kodkategoriperniagaan { get; set; }

        //[MaxLength(2)]
        //public int kodstesenminyak { get; set; }
    }
}