using IEMSApps.Utils;
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbsendonline_gambar")]
    public class TbSendOnlineGambar
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public string NoRujukan { get; set; }

        public Enums.StatusOnline Status { get; set; }

        public string CreatedDate { get; set; }

        public string UpdateDate { get; set; }
    }
}