

using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbpengguna")]
    public class TbPengguna : MasterBaseEntities
    {
        public int Id { get; set; }

        [PrimaryKey]
        [MaxLength(12)]
        public string NoKp { get; set; }

        [MaxLength(50)]
        public string Kata_Laluan { get; set; }
    
        [MaxLength(3)]
        public string KodCawangan { get; set; }

        [MaxLength(100)]
        public string Nama { get; set; }
      
        [MaxLength(100)]
        public string Nama_Bahagian { get; set; }

        [MaxLength(25)]
        public string Nama_Gelaran { get; set; }

        [MaxLength(100)]
        public string Nama_Gelaran_Jawatan { get; set; }

        [MaxLength(50)]
        public string Nama_Jawatan { get; set; }

        [MaxLength(10)]
        public string Gred { get; set; }

        [MaxLength(10)]
        public string Singkatan_Jawatan { get; set; }

   
    }
}