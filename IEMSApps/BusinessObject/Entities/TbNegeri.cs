using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbnegeri")]
    public class TbNegeri : MasterBaseEntities
    {
        [PrimaryKey]
        [MaxLength(2)]
        public string KodNegeri { get; set; }

        [MaxLength(150)]
        public string Prgn { get; set; }

    }

    [Table("tbnegeriTemp")]
    public class TbNegeriTemp : TbNegeri
    {
        public int Status { get; set; }
    }    
}