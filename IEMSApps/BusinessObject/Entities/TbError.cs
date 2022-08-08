using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tberrorhh")]
    public class TbError : BaseEntities
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [MaxLength(20)]
        public string IdHH { get; set; }
        [MaxLength(3)]
        public string KodCawangan { get; set; }
        public string SqlStmt { get; set; }
    }
}