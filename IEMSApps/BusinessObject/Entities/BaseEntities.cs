using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    public class BaseEntities
    {
        [MaxLength(1)]
        public string Status { get; set; }

        public int PgnDaftar { get; set; }

        public string TrkhDaftar { get; set; }

        public int PgnAkhir { get; set; }

        public string TrkhAkhir { get; set; }
    }
}