using IEMSApps.Utils;
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbpasukan_hh")]
    public class TbPasukanHh : MasterBaseEntities
    {
        [Indexed(Name = "IdxPsknHh", Order = 1, Unique = true)]
        [MaxLength(6)]
        public string IdHh { get; set; }

        [Indexed(Name = "IdxPsknHh", Order = 2, Unique = true)]
        public int Id { get; set; }

        [Indexed(Name = "IdxPsknHh", Order = 3, Unique = true)]
        public int KodPasukan { get; set; }

        [Indexed(Name = "IdxPsknHh", Order = 4, Unique = true)]
        [MaxLength(12)]
        public string NoKp { get; set; }

        [MaxLength(3)]
        public string KodCawangan { get; set; }

        public Enums.JenisPengguna JenisPengguna { get; set; }

        public int Turutan { get; set; }

        [MaxLength(250)]
        public string Catatan { get; set; }

        [MaxLength(1)]
        public string Status { get; set; }

   
    }
}