
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbkesalahanTemp")]
    public class TbKesalahanTemp : MasterBaseEntities
    {
        [Indexed(Name = "IxTbKesalahanPk", Order = 1, Unique = true)]
        [MaxLength(10)]
        public string KodAkta { get; set; }

        [Indexed(Name = "IxTbKesalahanPk", Order = 2, Unique = true)]
        public int KodSalah { get; set; }

        [MaxLength(150)]
        public string Seksyen { get; set; }
        [MaxLength(150)]
        public string Prgn { get; set; }
      
        public decimal AmnKmp_Ind { get; set; }
        [MaxLength(100)]
        public string AmnKmp_Ind_Word { get; set; }
        public decimal AmnKmp_Sya { get; set; }
        [MaxLength(100)]
        public string AmnKmp_Sya_Word { get; set; }

        public int TempohHari_Tetap { get; set; }
        public int TempohHari_Terbuka { get; set; }

        public int KOTS { get; set; }

        public int Status { get; set; }
    }
}