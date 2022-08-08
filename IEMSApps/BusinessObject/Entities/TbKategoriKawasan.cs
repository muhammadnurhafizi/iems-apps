
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbkategorikawasan")]
    public class TbKategoriKawasan : MasterBaseEntities
    {
        [PrimaryKey]
        [MaxLength(2)]
        public string KodKatKawasan { get; set; }

        [MaxLength(150)]
        public string Prgn { get; set; }
    }

    [Table("tbkategorikawasanTemp")]
    public class TbKategoriKawasanTemp : TbKategoriKawasan
    {
        public int Status { get; set; }
    }
}