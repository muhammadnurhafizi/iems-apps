using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("ip_poskod")]
    public class ip_poskod
    {
        public int id { get; set; }
        public string name { get; set; }
        public int city_id { get; set; }


    }
}