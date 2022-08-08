using IEMSApps.Utils;
using SQLite.Net.Attributes;

namespace IEMSApps.BusinessObject.Entities
{
    [Table("tbgpslog")]
    public class TbGpsLog : MasterBaseEntities
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(3)]
        public string KodCawangan { get; set; }

        [MaxLength(6)]
        public string IdHh { get; set; }
       
        public int IdStaf { get; set; }

        public string TrkhLog { get; set; }

        [MaxLength(15)]
        public string Longitud { get; set; }

        [MaxLength(15)]
        public string Latitud { get; set; }

        public Enums.StatusOnline IsSendOnline { get; set; }
    }
}
