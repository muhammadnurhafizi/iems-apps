using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbjenisperniagaan")]
    public class TbJenisPerniagaan : MasterBaseEntities
    {
        [PrimaryKey]
        public int KodJenis { get; set; }

        public string Prgn { get; set; }
    }
}