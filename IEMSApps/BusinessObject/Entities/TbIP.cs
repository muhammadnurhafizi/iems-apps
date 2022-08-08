using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbgpslog")]
    public class TbIP
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [MaxLength(50)]
        public string DnsName { get; set; }
        [MaxLength(50)]
        public string Ip { get; set; }
    }
}