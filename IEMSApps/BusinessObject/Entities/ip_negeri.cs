using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("ip_negeri")]
    public class ip_negeri
    {
        public int id { get; set; }

        public string name { get; set; }

        public string name_long { get; set; }

        public string code1 { get; set; }

        public string code2 { get; set; }

        public string code3 { get; set; }

        public string capital { get; set; }

        public string ip_negara_id { get; set; }
    }
}