using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbcawanganTemp")]
    public class TbCawanganTemp : MasterBaseEntities
    {
        [PrimaryKey]
        [MaxLength(3)]
        public string KodCawangan { get; set; }

        [MaxLength(100)]
        public string Prgn { get; set; }

       
        public string Nama_Cawangan { get; set; }

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
        
        public int Status { get; set; }
    }
}