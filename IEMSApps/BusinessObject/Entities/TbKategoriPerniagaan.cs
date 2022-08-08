
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbkategoriperniagaan")]
    public class TbKategoriPerniagaan : MasterBaseEntities
    {
        [PrimaryKey, AutoIncrement]
        public int KodKatPerniagaan { get; set; }

        [MaxLength(150)]
        public string Prgn { get; set; }
    }

    [Table("tbkategoriperniagaanTemp")]
    public class TbKategoriPerniagaanTemp : TbKategoriPerniagaan
    {
        public int Status { get; set; }
    }
}