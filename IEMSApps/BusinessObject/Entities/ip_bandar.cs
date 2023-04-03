using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("ip_bandar")]
    public class ip_bandar
    {
        public int id { get; set; }
        public string name { get; set; }
        public int state_id { get; set; }

    }
}