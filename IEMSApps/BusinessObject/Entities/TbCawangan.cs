using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbcawangan")]
    public class TbCawangan : MasterBaseEntities
    {
        [PrimaryKey]
        [MaxLength(3)]
        public string KodCawangan { get; set; }

        [MaxLength(100)]
        public string Prgn { get; set; }

       
        public string Nama_Cawangan { get; set; }

        //public int IdBahagian { get; set; }

        //[MaxLength(20)]
        //public string Singkatan_Bahagian { get; set; }

        //[MaxLength(100)]
        //public string Nama_Bahagian { get; set; }

        [MaxLength(200)]
        public string Alamat1 { get; set; }

        [MaxLength(200)]
        public string Alamat2 { get; set; }
        
        [MaxLength(5)]
        public string Poskod { get; set; }

        [MaxLength(2)]
        public string KodNegeri { get; set; }

        [MaxLength(30)]
        public string Bandar { get; set; }

        [MaxLength(50)]
        public string Emel { get; set; }

        [MaxLength(10)]
        public string No_Faks { get; set; }

        [MaxLength(10)]
        public string No_Telefon { get; set; }

        //[MaxLength(2)]
        //public string IdKategori_Bahagian { get; set; }

        //public int NotUrutan_Kpp { get; set; }
        //public int NotUrutan_Atr { get; set; }
        //public int NotUrutan_Kots { get; set; }
        //public int NotUrutan_Kmp { get; set; }

    }
}