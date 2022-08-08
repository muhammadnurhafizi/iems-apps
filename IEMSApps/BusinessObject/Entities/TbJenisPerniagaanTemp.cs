using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbjenisperniagaanTemp")]
    public class TbJenisPerniagaanTemp : MasterBaseEntities
    {
        [PrimaryKey]
        public int KodJenis { get; set; }

        public string Prgn { get; set; }

        public int Status { get; set; }
    }
}