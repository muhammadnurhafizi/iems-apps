using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbpremis")]
    public class TbPremis : MasterBaseEntities
    {
        [PrimaryKey]
        public int IdPremis { get; set; }
        [MaxLength(3)]
        public string KodCawangan { get; set; }
        [MaxLength(150)]
        public string NamaPremis { get; set; }
        [MaxLength(80)]
        public string AlamatPremis1 { get; set; }
        [MaxLength(80)]
        public string AlamatPremis2 { get; set; }
        [MaxLength(80)]
        public string AlamatPremis3 { get; set; }
        [MaxLength(15)]
        public string NoDaftarPremis { get; set; }
    }
}