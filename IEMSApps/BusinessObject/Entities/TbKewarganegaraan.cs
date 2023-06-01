using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbkewarganegaraan")]
    public class TbKewarganegaraan : BaseEntities
    {
        [PrimaryKey]
        [MaxLength(10)]
        public int id { get; set; }

        [MaxLength(500)]
        public string prgn { get; set; }

    }

}