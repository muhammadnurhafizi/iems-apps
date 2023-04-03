using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("ip_negeri")]
    public class ip_negeri
    {
        public int id { get; set; }
        public string name { get; set; }

    }
}