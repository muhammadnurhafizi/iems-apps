
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbasastindakan")]
    public class TbAsasTindakan : MasterBaseEntities
    {
        [Indexed(Name = "IxAsasTindakanPk", Order = 1, Unique = true)]
        public int KodTujuan { get; set; }

        [Indexed(Name = "IxAsasTindakanPk", Order = 2, Unique = true)]
        public int KodAsas { get; set; }

        [MaxLength(250)]
        public string Prgn { get; set; }
    }
}