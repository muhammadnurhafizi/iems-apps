using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbbandar")]
    public class TbBandar : MasterBaseEntities
    {
        [Indexed(Name = "IxABandarPk", Order = 1, Unique = true)]
        [MaxLength(2)]
        public string KodNegeri { get; set; }

        [Indexed(Name = "IxABandarPk", Order = 2, Unique = true)]
        public int KodBandar { get; set; }

        [MaxLength(100)]
        public string Prgn { get; set; }
    }
}