
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbkategoripremis")]
    public class TbKategoriPremis : MasterBaseEntities
    {
        [PrimaryKey, AutoIncrement]
        public int KodKatPremis { get; set; }

        [MaxLength(150)]
        public string Prgn { get; set; }
    }

    [Table("tbkategoripremisTemp")]
    public class TbKategoriPremisTemp : TbKategoriPremis
    {
        public int Status { get; set; }
    }
}