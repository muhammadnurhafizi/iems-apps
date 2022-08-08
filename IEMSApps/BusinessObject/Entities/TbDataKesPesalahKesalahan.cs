using IEMSApps.Utils;
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbdatakes_pesalah_kesalahan")]
    public class TbDataKesPesalahKesalahan
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string NoRujukan { get; set; }

        public string NoKes { get; set; }

        public Enums.StatusOnline Status { get; set; }

        public string CreatedDate { get; set; }

        public string UpdateDate { get; set; }
    }
}