
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbkesalahan_butir")]
    public class TbKesalahanButir : MasterBaseEntities
    {
        [Indexed(Name = "IxTbKesalahanButirPk", Order = 1, Unique = true)]
        [MaxLength(10)]
        public string KodAkta { get; set; }

        [Indexed(Name = "IxTbKesalahanButirPk", Order = 2, Unique = true)]
       
        public int KodSalah { get; set; }

        [Indexed(Name = "IxTbKesalahanButirPk", Order = 3, Unique = true)]
        public int KodButir { get; set; }

        public string Prgn { get; set; }
    }
}