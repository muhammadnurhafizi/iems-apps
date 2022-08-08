using IEMSApps.Utils;
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
   
    [Table("tbsendonline_data")]
    public class TbSendOnlineData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public Enums.TableType Type { get; set; }

        public string NoRujukan { get; set; }

        public Enums.StatusOnline Status { get; set; }

        public string CreatedDate { get; set; }

        public string UpdateDate { get; set; }

    }
}