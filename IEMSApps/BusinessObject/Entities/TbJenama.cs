using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbbarang_jenama")]
    public class TbJenama : MasterBaseEntities
    {
        [PrimaryKey]
        public int KodJenama { get; set; }

        [MaxLength(150)]
        public string Prgn { get; set; }
    }

    [Table("tbbarang_jenamaTemp")]
    public class TbJenamaTemp : TbJenama
    {
        public int Status { get; set; }
    }
}