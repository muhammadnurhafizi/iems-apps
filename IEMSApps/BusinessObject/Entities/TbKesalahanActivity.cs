using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbkesalahan_aktiviti")]
    public class TbKesalahanActivity : MasterBaseEntities
    {
        [Indexed(Name = "kodaktiviti", Order = 1, Unique = true)]
        [MaxLength(10)]
        public string KodAktivity { get; set; }

        [MaxLength(250)]
        public string Prgn { get; set; }
    }
}